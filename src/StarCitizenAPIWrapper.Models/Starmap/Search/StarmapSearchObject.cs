using StarCitizenAPIWrapper.Models.Attributes;

namespace StarCitizenAPIWrapper.Models.Starmap.Search
{
    /// <summary>
    /// The information from the API about an object found with the given name.
    /// </summary>
    public class StarmapSearchObject
    {
        /// <summary>
        /// The code of this starmap object.
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// The description of this starmap object.
        /// </summary>
        public string Designation { get; set; }
        /// <summary>
        /// The id of this starmap object.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of this starmap object.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The star system of this object.
        /// </summary>
        [ApiName("star_system")]
        public StarmapSearchObjectStarSystem StarSystem { get; set; }
        /// <summary>
        /// The id of the star system of this object.
        /// </summary>
        [ApiName("star_system_id")]
        public int StarSystemId { get; set; }
        /// <summary>
        /// The status of this starmap object.
        /// </summary>
        public char Status { get; set; }
        /// <summary>
        /// The type of this starmap object.
        /// </summary>
        public string Type { get; set; }
    }
}
