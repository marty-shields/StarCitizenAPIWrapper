namespace StarCitizenAPIWrapper.Models.Starmap.Search
{
    /// <summary>
    /// The star-system of a certain search object.
    /// </summary>
    public class StarmapSearchObjectStarSystem
    {
        /// <summary>
        /// The code of this star system.
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// The id of this star system.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of this star system.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The type of this star system.
        /// </summary>
        public string Type { get; set; }
    }
}