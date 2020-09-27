using StarCitizenAPIWrapper.Models.Attributes;

namespace StarCitizenAPIWrapper.Models.Ships.Manufacturer
{
    /// <summary>
    /// The information about a manufacturer of a ship.
    /// </summary>
    public class ShipManufacturer
    {
        /// <summary>
        /// The code of this manufacturer.
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// The description of this manufacturer.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The id of this manufacturer.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Whats this manufacturer is known for.
        /// </summary>
        [ApiName("known_for")]
        public string KnownFor { get; set; }
        /// <summary>
        /// The name of this manufacturer.
        /// </summary>
        public string Name { get; set; }
    }
}
