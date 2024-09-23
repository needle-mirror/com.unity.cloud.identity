using System;

namespace Unity.Cloud.Identity
{
    /// <summary>
    /// A class representing a JWT token.
    /// </summary>
    public class JwtToken
    {
        /// <summary>
        /// The required sub property of a JWT token.
        /// </summary>
        public string sub { get; set; }

        /// <summary>
        /// The required exp property of a JWT token.
        /// </summary>
        public int exp { get; set; }
    }
}
