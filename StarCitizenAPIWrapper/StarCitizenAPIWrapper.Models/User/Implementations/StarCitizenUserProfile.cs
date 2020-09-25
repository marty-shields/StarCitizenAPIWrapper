using System;
using StarCitizenAPIWrapper.Models.Attributes;

namespace StarCitizenAPIWrapper.Models.User.Implementations
{
    /// <summary>
    /// The default implementation of <see cref="IUserProfile"/>.
    /// </summary>
    public class StarCitizenUserProfile : IUserProfile
    {
        /// <inheritdoc />
        public string Badge { get; set; }

        /// <inheritdoc />
        [ApiName("badge_image")]
        public string BadgeImage { get; set; }

        /// <inheritdoc />
        public string Display { get; set; }

        /// <inheritdoc />
        public DateTime Enlisted { get; set; }

        /// <inheritdoc />
        public string[] Fluency { get; set; }

        /// <inheritdoc />
        public string Handle { get; set; }

        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        public string Image { get; set; }

        /// <inheritdoc />
        public (string Title, string Url) Page { get; set; }
    }
}
