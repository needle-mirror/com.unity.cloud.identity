using System;
using System.Collections.Generic;

namespace Unity.Cloud.Identity
{
    internal class ExchangeGenesisTokenRequest
    {
        public string grantType { get; internal set;  }
        public string accessToken { get; internal set; }
        public string targetClientId { get; internal set; }
    }

    public class TargetClientIdToken
    {
        public string token { get; set; }
    }

    internal class ExchangeGenesisAccessTokenResponse
    {
        public string access_token { get; set; }
    }

    internal class ExchangeTargetClientIdTokenResponse
    {
        public string token { get; set; }
    }

    /// <summary>
    /// Holds the token information related to a user authenticated session.
    /// </summary>
    public class DeviceToken
    {
        readonly DateTime m_AccessTokenExpiry;

        /// <summary>
        /// Requests authenticated access to cloud endpoints.
        /// </summary>
        public string AccessToken { get; }

        /// <summary>
        /// The token used on the refresh token cloud endpoint to generate a new <see cref="DeviceToken"/>.
        /// </summary>
        public string RefreshToken { get; }

        /// <summary>
        /// The TimeSpan value before the current <see cref="DeviceToken"/> expires.
        /// </summary>
        public TimeSpan AccessTokenExpiresIn => m_AccessTokenExpiry - DateTime.UtcNow;

        /// <summary>
        /// Creates a `DeviceToken`.
        /// </summary>
        /// <param name="accessToken">The string value of the issued access token.</param>
        /// <param name="refreshToken">The string value of the issued refresh token.</param>
        /// <param name="accessTokenExpiryDateTime">The DateTime value of the expiry date of the issued access token.</param>
        public DeviceToken(string accessToken, string refreshToken, DateTime accessTokenExpiryDateTime)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            m_AccessTokenExpiry = accessTokenExpiryDateTime;
        }

        /// <summary>
        /// Creates a `DeviceToken`.
        /// </summary>
        /// <param name="accessToken">The string value of the issued access token.</param>
        /// <param name="refreshToken">The string value of the issued refresh token.</param>
        /// <param name="accessTokenExpiryInSeconds">The int value in seconds of the remaining time before expiratoion of the issued access token.</param>
        public DeviceToken(string accessToken, string refreshToken, int accessTokenExpiryInSeconds)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            m_AccessTokenExpiry = ConvertExpiryInSecondsToDateTime(accessTokenExpiryInSeconds);
        }

        /// <summary>
        /// Creates a `DeviceToken`.
        /// </summary>
        /// <param name="accessToken">The string value of the issued access token.</param>
        /// <param name="refreshToken">The string value of the issued refresh token.</param>
        /// <param name="accessTokenExpiryDateTime">The DateTime value of the expiry date of the issued access token.</param>
        /// <param name="oldRefreshToken">The string value of the previously issued refresh token. This value will be used if provided refreshToken is null or empty.</param>
        public DeviceToken(string accessToken, string refreshToken, DateTime accessTokenExpiryDateTime, string oldRefreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = string.IsNullOrEmpty(refreshToken) ? oldRefreshToken : refreshToken;
            m_AccessTokenExpiry = accessTokenExpiryDateTime;
        }

        /// <summary>
        /// Creates a `DeviceToken`.
        /// </summary>
        /// <param name="accessToken">The string value of the issued access token.</param>
        /// <param name="refreshToken">The string value of the issued refresh token.</param>
        /// <param name="accessTokenExpiryInSeconds">The int value in seconds of the remaining time before expiratoion of the issued access token.</param>
        /// <param name="oldRefreshToken">The string value of the previously issued refresh token. This value will be used if provided refreshToken is null or empty.</param>
        public DeviceToken(string accessToken, string refreshToken, int accessTokenExpiryInSeconds, string oldRefreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = string.IsNullOrEmpty(refreshToken) ? oldRefreshToken : refreshToken;
            m_AccessTokenExpiry = ConvertExpiryInSecondsToDateTime(accessTokenExpiryInSeconds);
        }

        DateTime ConvertExpiryInSecondsToDateTime(int accessTokenExpiryInSeconds)
        {
            return DateTime.UtcNow + TimeSpan.FromSeconds(accessTokenExpiryInSeconds);
        }
    }

    /// <summary>
    /// Holds information about a <see cref="ExchangeCodeToken"/>.
    /// </summary>
    class ExchangeCodeToken
    {
        /// <summary>
        /// The type of token.
        /// </summary>
        public string token_type { get; set; }
        /// <summary>
        /// The access token.
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// The id token.
        /// </summary>
        public string id_token { get; set; }
        /// <summary>
        /// The refresh token.
        /// </summary>
        public string refresh_token { get; set; }
        /// <summary>
        /// The expiry date in seconds.
        /// </summary>
        public int expires_in { get; set; }
    }

    /// <summary>
    /// Contains information about a <see cref="RefreshDeviceToken"/>.
    /// </summary>
    class RefreshDeviceToken
    {
        /// <summary>
        /// The type of token.
        /// </summary>
        public string token_type { get; set; }
        /// <summary>
        /// The access token.
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// The refresh token.
        /// </summary>
        public string refresh_token { get; set; }
        /// <summary>
        /// The id token.
        /// </summary>
        public string id_token { get; set; }
        /// <summary>
        /// The expiry date in seconds.
        /// </summary>
        public int expires_in { get; set; }
    }

    public interface IAuthenticatedUserInfoProvider
    {
        string GetUserInfo(string key);
    }

    public static class AuthenticatedUserInfoClaims
    {
        public const string AccessToken = "access_token";
        public const string Id = "id";
        public const string Name = "name";
        public const string Email = "email";
        public const string Picture = "picture";
        public const string GivenName = "given_name";
        public const string FamilyName = "family_name";
    }

    internal class PkceUserInfoClaims : IAuthenticatedUserInfoProvider
    {
        /// <summary>
        /// The name of the user.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The subject identifier for the user.
        /// </summary>
        public string sub { get; set; }
        /// <summary>
        /// The primary email for the user.
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// The profile picture url.
        /// </summary>
        public string picture { get; set; }
        /// <summary>
        /// The preferred username.
        /// </summary>
        public string preferred_username { get; set; }
        /// <summary>
        /// The given name or first name.
        /// </summary>
        public string given_name { get; set; }
        /// <summary>
        /// The family name or last name.
        /// </summary>
        public string family_name { get; set; }
        /// <summary>
        /// The valid access token used to fetch the authenticated values.
        /// </summary>
        public string access_token { get; set; }

        public string GetUserInfo(string key)
        {
            return key switch
            {
                AuthenticatedUserInfoClaims.AccessToken => access_token,
                AuthenticatedUserInfoClaims.Id => sub,
                AuthenticatedUserInfoClaims.Name => name,
                AuthenticatedUserInfoClaims.Email => email,
                AuthenticatedUserInfoClaims.Picture => picture,
                AuthenticatedUserInfoClaims.GivenName => given_name,
                AuthenticatedUserInfoClaims.FamilyName => family_name,
                _ => null
            };
        }
    }
}
