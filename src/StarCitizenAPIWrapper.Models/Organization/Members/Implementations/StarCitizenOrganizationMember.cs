namespace StarCitizenAPIWrapper.Models.Organization.Members.Implementations
{
    /// <summary>
    /// The default implementation of a <see cref="IOrganizationMember"/>.
    /// </summary>
    public class StarCitizenOrganizationMember : IOrganizationMember
    {
        /// <inheritdoc />
        public string Display { get; set; }
        /// <inheritdoc />
        public string Handle { get; set; }
        /// <inheritdoc />
        public string Image { get; set; }
        /// <inheritdoc />
        public string Rank { get; set; }
        /// <inheritdoc />
        public int Stars { get; set; }
        /// <inheritdoc />
        public string[] Roles { get; set; }
    }
}
