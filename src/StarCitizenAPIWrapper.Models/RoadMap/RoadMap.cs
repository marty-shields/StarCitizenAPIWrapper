using System;
using System.Collections.Generic;
using StarCitizenAPIWrapper.Models.Attributes;

namespace StarCitizenAPIWrapper.Models.RoadMap
{
    /// <summary>
    /// The information of a certain roadmap from RSI.
    /// </summary>
    public class RoadMap
    {
        /// <summary>
        /// The id of the board of this roadmap.
        /// </summary>
        [ApiName("board_id")]
        public int BoardId { get; set; }
        /// <summary>
        /// The roadmap cards.
        /// </summary>
        [ApiName("cards")]
        public List<RoadMapCard> RoadMapCards { get; set; }
        /// <summary>
        /// The description of this roadmap.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The id of this roadmap.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The id of the importer of this roadmap.
        /// </summary>
        [ApiName("importer_id")] 
        public string ImporterId { get; set; }
        /// <summary>
        /// The name of this roadmap.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The order of this roadmap.
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Indicates if this roadmap has been released.
        /// </summary>
        public bool Released { get; set; }
        /// <summary>
        /// DateTime when this roadmap is scheduled at.
        /// </summary>
        [ApiName("scheduled_at")]
        [TimeStamp]
        public DateTime ScheduledAt { get; set; }
        /// <summary>
        /// DateTime when this roadmap was created.
        /// </summary>
        [ApiName("time_created")]
        [TimeStamp]
        public DateTime TimeCreated { get; set; }
        /// <summary>
        /// DateTime when this roadmap was last modified.
        /// </summary>
        [ApiName("time_modified")]
        [TimeStamp]
        public DateTime TimeModified { get; set; }
        /// <summary>
        /// The url slug of this roadmap.
        /// </summary>
        [ApiName("url_slug")]
        public string UrlSlug { get; set; }
    }
}
