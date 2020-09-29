using StarCitizenAPIWrapper.Models.Attributes;

namespace StarCitizenAPIWrapper.Models.Starmap.Tunnels
{
    /// <summary>
    /// The information about the starmap tunnels.
    /// </summary>
    public class StarmapTunnelEntry
    {
        /// <summary>
        /// The code of this tunnel entry.
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// The designation of this tunnel entry.
        /// </summary>
        public string Designation { get; set; }
        /// <summary>
        /// The distance the tunnel of this tunnel entry will be.
        /// </summary>
        public double Distance { get; set; }
        /// <summary>
        /// The id of this tunnel entry.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The latitude of this tunnel entry in the system.
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// The longitude of this tunnel entry in the system.
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// The name of this tunnel entry.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The id of the system this tunnel entry belongs to.
        /// </summary>
        [ApiName("star_system_id")]
        public int StarSystemId { get; set; }
        /// <summary>
        /// The current status of this tunnel entry.
        /// </summary>
        public char Status { get; set; }
        /// <summary>
        /// The type of this tunnel entry.
        /// </summary>
        public string Type { get; set; }
    }
}
