using System;
using StarCitizenAPIWrapper.Models.Attributes;
using System.Collections.Generic;
using StarCitizenAPIWrapper.Models.Starmap.Object;

namespace StarCitizenAPIWrapper.Models.Starmap.Systems
{
    /// <summary>
    /// The more detailed information about a specific star system.
    /// </summary>
    public class StarmapSystemDetail : IStarmapSystem
    {
        /// <summary>
        /// All the objects inside this system.
        /// </summary>
        [ApiName("celestial_objects")]
        public List<StarCitizenStarMapObject> CelestialObjects { get; set; }
        /// <summary>
        /// The frost line of this system.
        /// </summary>
        [ApiName("frost_line")]
        public double FrostLine { get; set; }
        /// <summary>
        /// The habitable zone on the inner layer.
        /// </summary>
        [ApiName("habitable_zone_inner")]
        public double HabitableZoneInner { get; set; }
        /// <summary>
        /// The habitable zone on the outer layer.
        /// </summary>
        [ApiName("habitable_zone_outer")]
        public double HabitableZoneOuter { get; set; }

        #region Interface implementations
        /// <inheritdoc />
        public List<StarmapSystemAffiliation> Affiliations { get; set; }
        /// <inheritdoc />
        public double AggregatedDanger { get; set; }
        /// <inheritdoc />
        public double AggregatedEconomy { get; set; }
        /// <inheritdoc />
        public double AggregatedPopulation { get; set; }
        /// <inheritdoc />
        public double AggregatedSize { get; set; }
        /// <inheritdoc />
        public string Code { get; set; }
        /// <inheritdoc />
        public string Description { get; set; }
        /// <inheritdoc />
        public int Id { get; set; }
        /// <inheritdoc />
        public string InfoUrl { get; set; }
        /// <inheritdoc />
        public string Name { get; set; }
        /// <inheritdoc />
        public double PositionX { get; set; }
        /// <inheritdoc />
        public double PositionY { get; set; }
        /// <inheritdoc />
        public double PositionZ { get; set; }
        /// <inheritdoc />
        public char Status { get; set; }
        /// <inheritdoc />
        public StarmapSystemThumbnail Thumbnail { get; set; }
        /// <inheritdoc />
        public DateTime TimeModified { get; set; }
        /// <inheritdoc />
        public string Type { get; set; }
        #endregion
    }
}
