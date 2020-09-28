namespace StarCitizenAPIWrapper.Models.Ships.Compiled
{
    /// <summary>
    /// The component of a ship.
    /// </summary>
    public class RsiShipComponent
    {
        /// <summary>
        /// The class of this component.
        /// </summary>
        public string Class { get; set; }
        /// <summary>
        /// The size of this component.
        /// </summary>
        public string Size { get; set; }
        /// <summary>
        /// The details of this component.
        /// </summary>
        public string Details { get; set; }
        /// <summary>
        /// The manufacturer of this component.
        /// </summary>
        public string Manufacturer { get; set; }
        /// <summary>
        /// The amount of mounts this component has.
        /// </summary>
        public int Mounts { get; set; }
        /// <summary>
        /// The name of this component.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The quantity of this component.
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// The type of this component.
        /// </summary>
        public string Type { get; set; }
    }
}
