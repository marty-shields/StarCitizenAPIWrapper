namespace StarCitizenAPIWrapper.Models.User
{
    /// <summary>
    /// Interface for the information about a organization a user is currently member of.
    /// </summary>
    public interface IUserOrganizationInfo
    {
        /// <summary>
        /// The image URL of the organization image.
        /// </summary>
        string Image { get; set; }
        /// <summary>
        /// The name of the organization.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// The rank of the user inside this organization.
        /// </summary>
        string Rank { get; set; }
        /// <summary>
        /// The SID of the organization.
        /// </summary>
        string Sid { get; set; }
    }
}
