namespace StarCitizenAPIWrapper.Models.User
{
    /// <summary>
    /// Implementation of a default <see cref="IUser"/>.
    /// </summary>
    public class StarCitizenUser : IUser
    {
        /// <inheritdoc />
        public IUserProfile Profile { get; set; }
        /// <inheritdoc />
        public UserOrganizationInfo Organization { get; set; }
    }
}
