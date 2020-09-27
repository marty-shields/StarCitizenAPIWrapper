namespace StarCitizenAPIWrapper.Models.Organization.Implementations
{
    /// <summary>
    /// The default implementation of <see cref="IOrganization"/>.
    /// </summary>
    public class StarCitizenOrganization : IOrganization
    {
        /// <inheritdoc />
        public Archetypes Archetypes { get; set; }
        /// <inheritdoc />
        public string Banner { get; set; }
        /// <inheritdoc />
        public string Commitment { get; set; }
        /// <inheritdoc />
        public Focus Focus { get; set; }
        /// <inheritdoc />
        public (string html, string plaintext) Headline { get; set; }
        /// <inheritdoc />
        public string Href { get; set; }
        /// <inheritdoc />
        public string Language { get; set; }
        /// <inheritdoc />
        public string Logo { get; set; }
        /// <inheritdoc />
        public int Members { get; set; }
        /// <inheritdoc />
        public string Name { get; set; }
        /// <inheritdoc />
        public bool Recruiting { get; set; }
        /// <inheritdoc />
        public bool RolePlaying { get; set; }
        /// <inheritdoc />
        public string SID { get; set; }
        /// <inheritdoc />
        public string Url { get; set; }
    }
}
