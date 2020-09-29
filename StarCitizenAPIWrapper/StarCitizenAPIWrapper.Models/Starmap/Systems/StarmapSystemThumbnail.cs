using System.Collections.Generic;

namespace StarCitizenAPIWrapper.Models.Starmap.Systems
{
    /// <summary>
    /// The information about a thumbnail of a starmap system.
    /// </summary>
    public class StarmapSystemThumbnail
    {
        /// <summary>
        /// The images of the thumbnail.
        /// </summary>
        public Dictionary<string, string> Images { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// The slug of this thumbnail.
        /// </summary>
        public string Slug { get; set; }
        /// <summary>
        /// The source of this thumbnail
        /// </summary>
        public string Source { get; set; }
    }
}
