namespace StarCitizenAPIWrapper.Models.Starmap.Tunnels
{
    /// <summary>
    /// The information about a tunnel inside of a star system.
    /// </summary>
    public class StarmapTunnel
    {
        /// <summary>
        /// The direction of this tunnel.
        /// </summary>
        public char Direction { get; set; }
        /// <summary>
        /// The two entries this tunnel has.
        /// </summary>
        public (StarmapTunnelEntry, StarmapTunnelEntry ) Entry { get; set; }
        /// <summary>
        /// The id of the exit of this tunnel.
        /// </summary>
        public int ExitId { get; set; }
        /// <summary>
        /// The id of this tunnel.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of this tunnel.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The size of this tunnel.
        /// </summary>
        public char Size { get; set; }
    }
}
