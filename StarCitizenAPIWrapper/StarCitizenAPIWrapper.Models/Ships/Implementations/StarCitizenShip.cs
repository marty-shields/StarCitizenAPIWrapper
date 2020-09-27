using System;
using StarCitizenAPIWrapper.Models.Ships.Media;

namespace StarCitizenAPIWrapper.Models.Ships.Implementations
{
    /// <summary>
    /// The default implementation of <see cref="IShip"/>.
    /// </summary>
    public class StarCitizenShip : IShip
    {
        /// <inheritdoc />
        public int AfterburnerSpeed { get; set; }
        /// <inheritdoc />
        public double Beam { get; set; }
        /// <inheritdoc />
        public int CargoCapacity { get; set; }
        /// <inheritdoc />
        public int ChassisId { get; set; }
        /// <inheritdoc />
        public string Compiled { get; set; }
        /// <inheritdoc />
        public string Description { get; set; }
        /// <inheritdoc />
        public string Focus { get; set; }
        /// <inheritdoc />
        public double Height { get; set; }
        /// <inheritdoc />
        public int Id { get; set; }
        /// <inheritdoc />
        public double Length { get; set; }
        /// <inheritdoc />
        public string Manufacturer { get; set; }
        /// <inheritdoc />
        public int ManufacturerId { get; set; }
        /// <inheritdoc />
        public int Mass { get; set; }
        /// <inheritdoc />
        public int MaxCrew { get; set; }
        /// <inheritdoc />
        public ApiMedia[] Media { get; set; }
        /// <inheritdoc />
        public int MinCrew { get; set; }
        /// <inheritdoc />
        public string Name { get; set; }
        /// <inheritdoc />
        public double PitchMax { get; set; }
        /// <inheritdoc />
        public double Price { get; set; }
        /// <inheritdoc />
        public string ProductionNote { get; set; }
        /// <inheritdoc />
        public ProductionStatusTypes ProductionStatus { get; set; }
        /// <inheritdoc />
        public double RollMax { get; set; }
        /// <inheritdoc />
        public int ScmSpeed { get; set; }
        /// <inheritdoc />
        public ShipSizes Size { get; set; }
        /// <inheritdoc />
        public DateTime TimeModified { get; set; }
        /// <inheritdoc />
        public ShipTypes Type { get; set; }
        /// <inheritdoc />
        public string Url { get; set; }
        /// <inheritdoc />
        public double XAxisAcceleration { get; set; }
        /// <inheritdoc />
        public double YawMax { get; set; }
        /// <inheritdoc />
        public double YAxisAcceleration { get; set; }
        /// <inheritdoc />
        public double ZAxisAcceleration { get; set; }
    }
}
