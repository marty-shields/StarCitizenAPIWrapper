using StarCitizenAPIWrapper.Models.Attributes;

namespace StarCitizenAPIWrapper.Models.Starmap.Systems
{
    /// <summary>
    /// The information about an affiliation of a starmap system/
    /// </summary>
    public class StarmapSystemAffiliation
    {
        /// <summary>
        /// The code of the affiliated instance.
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// The color of the affiliated instance.
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// The id of this affiliation.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The id of the membership of the affiliated instance.
        /// </summary>
        [ApiName("membership_id")]
        public int MembershipId { get; set; }
        /// <summary>
        /// The name of the affiliated instance.
        /// </summary>
        public string Name { get; set; }
    }
}
