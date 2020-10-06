namespace StarCitizenAPIWrapper.Models.Starmap.Species
{
    /// <summary>
    /// The information about a species found in the star citizen verse.
    /// </summary>
    public class StarCitizenSpecies
    {
        /// <summary>
        /// The code of this species.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The id of this species.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of this species.
        /// </summary>
        public string Name { get; set; }
    }
}
