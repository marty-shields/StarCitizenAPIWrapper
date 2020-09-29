using System.Collections.Generic;

namespace StarCitizenAPIWrapper.Models.RoadMap
{
    /// <summary>
    /// The information of a thumbnail of a roadmap card.
    /// </summary>
    public class RoadMapCardThumbnail
    {
        /// <summary>
        /// The id of the roadmap card thumbnail.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The image urls of this thumbnail.
        /// </summary>
        public Dictionary<string, string> Urls { get; set; }
    }
}
