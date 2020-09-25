using System;
using StarCitizenAPIWrapper.Models.Attributes;

namespace StarCitizenAPIWrapper.Models.User
{
    /// <summary>
    /// Information about the user profile.
    /// </summary>
    public interface IUserProfile
    {
        /// <summary>
        /// The current badge of the user.
        /// </summary>
        string Badge { get; set; }
        /// <summary>
        /// The url to the image of the <see cref="Badge"/>.
        /// </summary>
        [ApiName("badge_image")]
        string BadgeImage { get; set; }
        /// <summary>
        /// The display name of this user.
        /// </summary>
        string Display { get; set; }
        /// <summary>
        /// The datetime when this user enlisted.
        /// </summary>
        DateTime Enlisted { get; set; }
        /// <summary>
        /// The languages this user is fluent in.
        /// </summary>
        string[] Fluency { get; set; }
        /// <summary>
        /// The handle of this user.
        /// </summary>
        string Handle { get; set; }
        /// <summary>
        /// The id of this user.
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// The url of the image of this user.
        /// </summary>
        string Image { get; set; }
        /// <summary>
        /// The information about the user page.
        /// </summary>
        (string Title, string Url) Page { get; set; }
    }
}
