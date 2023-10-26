using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Identity
{

    internal interface IProjectProvider
    {
        IAsyncEnumerable<IProject> GetOrganizationProjects(OrganizationId organizationId, IEntityRoleProvider entityRoleProvider, Range range, CancellationToken cancellationToken);
    }

    internal class AuthenticatorProjectProvider : IProjectProvider
    {
        readonly IServiceHostResolver m_ServiceHostResolver;
        readonly IServiceHttpClient m_ServiceHttpClient;

        public AuthenticatorProjectProvider(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
        {
            m_ServiceHostResolver = serviceHostResolver.CreateCopyWithDomainResolverOverride(new UnityServicesDomainResolver(true));
            m_ServiceHttpClient = serviceHttpClient;
        }

        public async IAsyncEnumerable<IProject> GetOrganizationProjects(OrganizationId organizationId, IEntityRoleProvider entityRoleProvider, Range range, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var initialOffsetAndLength = await GetOffsetAndLengthAsync(organizationId, range, cancellationToken);

            if (initialOffsetAndLength.Length <= 0)
                yield break;

            int expectedAmount;
            int itemsCount = 0;
            int totalCount;
            var subRange = initialOffsetAndLength;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                var projectsJson = await GetOrganizationProjects(organizationId, subRange.Offset, subRange.Length, cancellationToken);

                var amountReturned = projectsJson.Results.Count();
                itemsCount += amountReturned;
                totalCount = projectsJson.Total;
                expectedAmount = CorrectExpectedAmount(initialOffsetAndLength, totalCount);

                foreach (var projectJson in projectsJson.Results)
                {
                    yield return new Project(projectJson, entityRoleProvider);
                }

                subRange = (subRange.Offset + amountReturned, amountReturned);
            }
            while (itemsCount < expectedAmount && subRange.Offset < totalCount);
        }

        async Task<(int Offset, int Length)> GetOffsetAndLengthAsync(OrganizationId organizationId, Range range, CancellationToken cancellationToken)
        {
            (int Offset, int Length) values = (0, int.MaxValue);

            if (range.Start.IsFromEnd || (range.End.IsFromEnd && range.End.Value != 0))
            {
                var count = await GetSourceCountAsync(organizationId, cancellationToken);

                values.Offset = CheckIndex(range.Start, count);
                var endIndex =  CheckIndex(range.End, count);

                values.Length = Math.Max(0, endIndex - values.Offset);
            }
            else
            {
                values.Offset = range.Start.Value;
                if (!range.End.Equals(Index.End))
                {
                    values.Length = Math.Max(0, range.End.Value - range.Start.Value);
                }
            }

            return values;
        }

        async Task<int> GetSourceCountAsync(OrganizationId organizationId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return (await GetOrganizationProjects(organizationId, 0, 1, cancellationToken)).Total;
        }

        int CheckIndex(Index index, int totalLength)
        {
            return (index.IsFromEnd)
                ? Math.Max(0, totalLength - index.Value)
                : index.Value;
        }

        async Task<ProjectsJson> GetOrganizationProjects(OrganizationId organizationId, int offset, int limit, CancellationToken cancellationToken)
        {
            var url = m_ServiceHostResolver.GetResolvedRequestUri($"/api/unity/legacy/v1/organizations/{organizationId}/projects?offset={offset}&limit={limit}");
            var response = await m_ServiceHttpClient.GetAsync(url, cancellationToken: cancellationToken);
            return await response.JsonDeserializeAsync<ProjectsJson>();
        }

        static int CorrectExpectedAmount((int Offset, int Length) range, int totalCount)
        {
            return range.Length > totalCount
                ? totalCount - range.Offset // to correctly track Index.End/Range.All or when requested more then exists
                : range.Length; // for cases when TotalCount changes between requests and becomes grater then Range length
        }
    }
}

