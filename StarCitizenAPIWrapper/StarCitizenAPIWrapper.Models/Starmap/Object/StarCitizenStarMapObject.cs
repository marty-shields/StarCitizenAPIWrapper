using System;
using System.Collections.Generic;
using StarCitizenAPIWrapper.Models.Attributes;
using StarCitizenAPIWrapper.Models.Starmap.Systems;

namespace StarCitizenAPIWrapper.Models.Starmap.Object
{
    /// <summary>
    /// The information from the API of an object of the starmap.
    /// </summary>
    public class StarCitizenStarMapObject
    {
        /// <summary>
        /// The affiliations of this object.
        /// </summary>
        [ApiName("affiliation")]
        public List<StarmapSystemAffiliation> Affiliations { get; set; }
        /// <summary>
        /// The age of this object.
        /// </summary>
        public double Age { get; set; }
        /// <summary>
        /// The appearance of this object.
        /// </summary>
        public string Appearance { get; set; }
        /// <summary>
        /// The tilt of the axis of this object.
        /// </summary>
        [ApiName("axial_tilt")]
        public double AxialTilt { get; set; }
        /// <summary>
        /// The children of this object.
        /// </summary>
        public List<StarCitizenStarMapObject> Children { get; set; }
        /// <summary>
        /// The code of this object.
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// The description of this object.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The designation of this object.
        /// </summary>
        public string Designation { get; set; }
        /// <summary>
        /// The distance of this object.
        /// </summary>
        public double Distance { get; set; }
        /// <summary>
        /// The fair chance act.
        /// </summary>
        public string FairChanceAct { get; set; }
        /// <summary>
        /// Indicates if this object is habitable.
        /// </summary>
        public bool Habitable { get; set; }
        /// <summary>
        /// The id of this object.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The info url of this object.
        /// </summary>
        [ApiName("info_url")]
        public string InfoUrl { get; set; }
        /// <summary>
        /// The latitude of this object.
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// The longitude of this object.
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// The name of this object.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Indicates how long one orbit of this object is.
        /// </summary>
        [ApiName("orbit_period")]
        public double OrbitPeriod { get; set; }
        /// <summary>
        /// The id of the parent of this object.
        /// </summary>
        [ApiName("parent_id")]
        public int ParentId { get; set; }
        /// <summary>
        /// The population amount of this object.
        /// </summary>
        public string Population { get; set; }
        /// <summary>
        /// The sensor danger of this object.
        /// </summary>
        [ApiName("sensor_danger ")]
        public double SensorDanger { get; set; }
        /// <summary>
        /// The sensor population of this object.
        /// </summary>
        [ApiName("sensor_population")]
        public double SensorPopulation { get; set; }
        /// <summary>
        /// The sensor economy of this object.
        /// </summary>
        [ApiName("sensor_economy")]
        public double SensorEconomy { get; set; }
        /// <summary>
        /// The sensor data of this object.
        /// </summary>
        [ApiName("shader_data")]
        public string ShaderData { get; set; }
        /// <summary>
        /// Indicates if the label of this object should be shown on the starmap.
        /// </summary>
        [ApiName("show_label")]
        public bool ShowLabel { get; set; }
        /// <summary>
        /// Indicates if the orbit lines of this object should be shown on the starmap.
        /// </summary>
        [ApiName("show_orbit_lines")]
        public bool ShowOrbitLines { get; set; }
        /// <summary>
        /// The size of this object.
        /// </summary>
        public double Size { get; set; }
        /// <summary>
        /// The subtype of this object.
        /// </summary>
        public StarMapObjectSubType SubType { get; set; }
        /// <summary>
        /// The Id of the subtype.
        /// </summary>
        [ApiName("subtype_id")]
        public int SubTypeId { get; set; }
        /// <summary>
        /// The textures of this object.
        /// </summary>
        public StarMapObjectTextures Textures { get; set; }
        /// <summary>
        /// The last time this object was modified.
        /// </summary>
        [ApiName("time_modified")]
        public DateTime TimeModified { get; set; }
        /// <summary>
        /// The type of this object.
        /// </summary>
        public string Type { get; set; }
    }

    /// <summary>
    /// The texture information of an <see cref="StarCitizenStarMapObject"/>.
    /// </summary>
    public class StarMapObjectTextures
    {
        /// <summary>
        /// The different textures.
        /// </summary>
        public Dictionary<string, string> Images { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// Slug of the textures.
        /// </summary>
        public string Slug { get; set; }
        /// <summary>
        /// The source url of the textures.
        /// </summary>
        public string Source { get; set; }
    }

    /// <summary>
    /// The subtype of an <see cref="StarCitizenStarMapObject"/>.
    /// </summary>
    public struct StarMapObjectSubType
    {
        /// <summary>
        /// The id of this subtype.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of this subtype.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The type of this subtype.
        /// </summary>
        public string Type { get; set; }
    }
}
