using StarCitizenAPIWrapper.Models.Attributes;

namespace StarCitizenAPIWrapper.Models.Stats
{
    /// <summary>
    /// The information about the current star citizen stats.
    /// </summary>
    public class StarCitizenStats
    {
        /// <summary>
        /// The current live version.
        /// </summary>
        [ApiName("current_live")]
        public string CurrentLive { get; set; }
        /// <summary>
        /// Number of current registered accounts.
        /// </summary>
        public long Fans { get; set; }
        /// <summary>
        /// Current funds gathered.
        /// </summary>
        public decimal Funds { get; set; }
    }
}
