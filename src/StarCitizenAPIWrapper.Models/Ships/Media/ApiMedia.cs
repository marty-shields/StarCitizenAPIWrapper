using System.Collections.Generic;

namespace StarCitizenAPIWrapper.Models.Ships.Media
{
    /// <summary>
    /// Information about the media of a ship from the API.
    /// </summary>
    public class ApiMedia
    {
        /// <summary>
        /// The name of the source image.
        /// </summary>
        public string SourceName { get; set; }
        /// <summary>
        /// The url of the source image.
        /// </summary>
        public string SourceUrl { get; set; }

        /// <summary>
        /// The images of the ship media.
        /// </summary>
        public List<ShipMediaImage> MediaImages { get; set; }
    }
}
