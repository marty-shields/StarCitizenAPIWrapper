using System;
using StarCitizenAPIWrapper.Models.Attributes;

namespace StarCitizenAPIWrapper.Models.RoadMap
{
    /// <summary>
    /// The information about a roadmap card.
    /// </summary>
    public class RoadMapCard
    {
        /// <summary>
        /// The id of the board this card belongs to.
        /// </summary>
        [ApiName("board_id")]
        public int BoardId { get; set; }
        /// <summary>
        /// The body of this card.
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// The id of the category of this card.
        /// </summary>
        [ApiName("category_id")]
        public int CategoryId { get; set; }
        /// <summary>
        /// Indicates how much of this card is completed.
        /// </summary>
        public int Completed { get; set; }
        /// <summary>
        /// The description of this card.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The id of this card.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The id of the importer.
        /// </summary>
        [ApiName("importer_id")]
        public string ImporterId { get; set; }
        /// <summary>
        /// Indicates how much is already in progress.
        /// </summary>
        public int InProgress { get; set; }
        /// <summary>
        /// The name of this card.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The order of this card.
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// The id of the release of this card.
        /// </summary>
        [ApiName("release_id")]
        public int ReleaseId { get; set; }
        /// <summary>
        /// Indicates if this card was already released.
        /// </summary>
        public bool Released { get; set; }
        /// <summary>
        /// The datetime for when this card is scheduled at.
        /// </summary>
        public DateTime ScheduledAt { get; set; }
        /// <summary>
        /// The amount of tasks of this card.
        /// </summary>
        public int Tasks { get; set; }
        /// <summary>
        /// The <see cref="RoadMapCardThumbnail"/> containing the information of this card.
        /// </summary>
        public RoadMapCardThumbnail Thumbnail { get; set; }
        /// <summary>
        /// The datetime when this card was created.
        /// </summary>
        [ApiName("time_created")]
        public DateTime TimeCreated { get; set; }
        /// <summary>
        /// The datetime when this card was last modified.
        /// </summary>
        [ApiName("time_modified")]
        public DateTime TimeModified { get; set; }
        /// <summary>
        /// The url title of this card.
        /// </summary>
        [ApiName("url_slug")]
        public string UrlSlug { get; set; }
    }
}
