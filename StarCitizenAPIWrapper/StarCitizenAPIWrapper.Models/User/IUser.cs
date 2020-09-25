using StarCitizenAPIWrapper.Models.Organization;
using StarCitizenAPIWrapper.Models.User.Implementations;

namespace StarCitizenAPIWrapper.Models.User
{
    /// <summary>
    /// Interface for user info from the API.
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// The profile information about this user.
        /// </summary>
        IUserProfile Profile { get; set; }
        /// <summary>
        /// The current <see cref="UserOrganizationInfo"/> of the organization this user is currently member of.
        /// </summary>
        IUserOrganizationInfo Organization { get; set; }
    }
}
