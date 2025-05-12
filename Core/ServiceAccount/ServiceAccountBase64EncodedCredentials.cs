namespace Unity.Cloud.Identity
{
    /// <summary>
    /// This struct holds information about service account base64 encoded credentials.
    /// </summary>
    public readonly struct ServiceAccountBase64EncodedCredentials
    {
        readonly string m_String;

        /// <summary>
        /// Returns the value of an identifier representing an invalid service account base64 encoded credentials
        /// </summary>
        public static readonly ServiceAccountBase64EncodedCredentials None = new(string.Empty);

        /// <summary>
        /// Creates a <see cref="ServiceAccountBase64EncodedCredentials"/> using a <see cref="string"/>.
        /// </summary>
        /// <param name="value">The string representing the service account base64 encoded credentials</param>
        public ServiceAccountBase64EncodedCredentials(string value) => m_String = value;

        /// <summary>
        /// Returns whether two <see cref="ServiceAccountBase64EncodedCredentials"/> objects are equals.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>
        /// <see langword="true"/> if both instances have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool Equals(ServiceAccountBase64EncodedCredentials other) => m_String == other.m_String;

        /// <summary>
        /// Validates that  <paramref name="obj"/> is a <see cref="ServiceAccountBase64EncodedCredentials"/> instance and that it has the same value as this instance.
        /// </summary>
        /// <param name="obj">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is ServiceAccountBase64EncodedCredentials other && Equals(other);

        /// <summary>
        /// Computes a hash code for the object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        /// <remarks>
        /// * You should not assume that equal hash codes imply object equality.
        /// * You should never persist or use a hash code outside the application domain in which it was created,
        ///   because the same object may hash differently across application domains, processes, and platforms.
        /// </remarks>
        public override int GetHashCode() => m_String != null ? m_String.GetHashCode() : 0;

        /// <summary>
        /// Get the string representation of this <see cref="ServiceAccountBase64EncodedCredentials"/>.
        /// </summary>
        /// <returns>The string result.</returns>
        public override string ToString() => m_String;

        /// <summary>
        /// Returns whether two <see cref="ServiceAccountBase64EncodedCredentials"/> instances are equal.
        /// </summary>
        /// <param name="left">First instance.</param>
        /// <param name="right">Second instance.</param>
        /// <returns>
        /// <see langword="true"/> if the instances are equal;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(ServiceAccountBase64EncodedCredentials left, ServiceAccountBase64EncodedCredentials right) => left.Equals(right);

        /// <summary>
        /// Returns whether two <see cref="ServiceAccountBase64EncodedCredentials"/> instances are different.
        /// </summary>
        /// <param name="left">First instance.</param>
        /// <param name="right">Second instance.</param>
        /// <returns>
        /// <see langword="true"/> if the instances are different;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator !=(ServiceAccountBase64EncodedCredentials left, ServiceAccountBase64EncodedCredentials right) => !left.Equals(right);

        /// <summary>
        /// Explicitly cast a <see cref="ServiceAccountBase64EncodedCredentials"/> to a <see cref="string"/>
        /// </summary>
        /// <param name="serviceAccountBase64EncodedCredentials">Object to cast</param>
        /// <returns>The resulting <see cref="string"/></returns>
        public static explicit operator string(ServiceAccountBase64EncodedCredentials serviceAccountBase64EncodedCredentials) => serviceAccountBase64EncodedCredentials.m_String;
    }
}
