using System;
using System.Collections.Generic;
using System.Text;
using StarCitizenAPIWrapper.Models.Attributes;

namespace StarCitizenAPIWrapper.Models.Starmap.Systems.Implementations
{
    /// <inheritdoc />
    public class StarmapSystem : IStarmapSystem
    {
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
    }
}
