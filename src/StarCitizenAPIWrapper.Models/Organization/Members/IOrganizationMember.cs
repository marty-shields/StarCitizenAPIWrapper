namespace StarCitizenAPIWrapper.Models.Organization.Members
{
    /// <summary>
    /// Interface for organization member info from the API.
    /// </summary>
    public interface IOrganizationMember
    {
        /// <summary>
        /// Display name of this organization member.
        /// </summary>
        string Display { get; set; }
        /// <summary>
        /// Handle of this organization member.
        /// </summary>
        string Handle { get; set; }
        /// <summary>
        /// Image URL of this organization member.
        /// </summary>
        string Image { get; set; }
        /// <summary>
        /// Rank of this member in the organization
        /// </summary>
        string Rank { get; set; }
        /// <summary>
        /// Amount of stars this member has.
        /// </summary>
        int Stars { get; set; }
        /// <summary>
        /// Roles this member has.
        /// </summary>
        string[] Roles { get; set; }
    }
}
