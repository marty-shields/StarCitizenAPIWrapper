using StarCitizenAPIWrapper.Models.Attributes;
using System.Collections.Generic;

namespace StarCitizenAPIWrapper.Models.Starmap.Search
{
    /// <summary>
    /// The API information containing the search result of the starmap.
    /// </summary>
    public class StarmapSearchResult
    {
        /// <summary>
        /// The objects which were found.
        /// </summary>
        [ApiName("objects")]
        public List<StarmapSearchObject> StarmapSearchObjects { get; set; }

        /// <summary>
        /// The star systems the search results are in.
        /// </summary>
        [ApiName("systems")]
        public List<StarmapSearchObjectStarSystem> StarSystems { get; set; }
    }
}
