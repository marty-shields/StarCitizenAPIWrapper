namespace StarCitizenAPIWrapper.Models.Starmap.Affiliations
{
    /// <summary>
    /// The information about an affiliation of species in the star citizen verse.
    /// </summary>
    public class StarCitizenAffiliation
    {
        /// <summary>
        /// The code of this affiliation.
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// The color of this affiliation.
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// The id of this affiliation.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of this affiliation.
        /// </summary>
        public string Name { get; set; }
    }
}
