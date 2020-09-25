namespace StarCitizenAPIWrapper.Models.User.Implementations
{
    /// <summary>
    /// The default implementation of <see cref="IUserOrganizationInfo"/>.
    /// </summary>
    public class UserOrganizationInfo : IUserOrganizationInfo
    {
        /// <inheritdoc />
        public string Image { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public string Rank { get; set; }

        /// <inheritdoc />
        public string Sid { get; set; }
    }
}
