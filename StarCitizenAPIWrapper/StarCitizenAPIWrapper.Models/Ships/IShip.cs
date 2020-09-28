using System;
using System.Collections.Generic;
using StarCitizenAPIWrapper.Models.Attributes;
using StarCitizenAPIWrapper.Models.Ships.Compiled;
using StarCitizenAPIWrapper.Models.Ships.Manufacturer;
using StarCitizenAPIWrapper.Models.Ships.Media;

namespace StarCitizenAPIWrapper.Models.Ships
{
    /// <summary>
    /// Interface for the information about a ship.
    /// </summary>
    public interface IShip
    {
        /// <summary>
        /// The after burner speed of this ship.
        /// </summary>
        [ApiName("afterburner_speed")]
        int AfterburnerSpeed { get; set; }

        /// <summary>
        /// The beam of this ship.
        /// </summary>
        double Beam { get; set; }

        /// <summary>
        /// The cargo capacity of this ship.
        /// </summary>
        int CargoCapacity { get; set; }

        /// <summary>
        /// The id of the chassis of this ship.
        /// </summary>
        int ChassisId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        List<KeyValuePair<ShipCompiledClasses, List<KeyValuePair<string, RsiShipComponent>>>> Compiled { get; set; }

        /// <summary>
        /// The description of this ship.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// The focus of this ship.
        /// </summary>
        string Focus { get; set; }

        /// <summary>
        /// The height of this ship.
        /// </summary>
        double Height { get; set; }

        /// <summary>
        /// The id of this ship on the API database.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// The length of this ship.
        /// </summary>
        double Length { get; set; }

        /// <summary>
        /// The manufacturer of this ship.
        /// </summary>
        ShipManufacturer Manufacturer { get; set; }

        /// <summary>
        /// The id of the manufacturer of this ship.
        /// </summary>
        int ManufacturerId { get; set; }

        /// <summary>
        /// The mass of this ship.
        /// </summary>
        int Mass { get; set; }

        /// <summary>
        /// The maximal amount of crew members for this ship.
        /// </summary>
        int MaxCrew { get; set; }

        /// <summary>
        /// Array of urls of images for this ship.
        /// </summary>
        ApiMedia[] Media { get; set; }

        /// <summary>
        /// The minimal amount of crew members for this ship.
        /// </summary>
        int MinCrew { get; set; }

        /// <summary>
        /// The name of this ship.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The maximal amount of pitch this ship has.
        /// </summary>
        double PitchMax { get; set; }

        /// <summary>
        /// The price of this ship.
        /// </summary>
        double Price { get; set; }

        /// <summary>
        /// The production note of this ship.
        /// </summary>
        string ProductionNote { get; set; }

        /// <summary>
        /// The current status of the production of this ship.
        /// </summary>
        [ApiName("production_status")]
        ProductionStatusTypes ProductionStatus { get; set; }

        /// <summary>
        /// The maximal amount of roll this ship has.
        /// </summary>
        double RollMax { get; set; }

        /// <summary>
        /// The scm speed of this ship.
        /// </summary>
        int ScmSpeed { get; set; }

        /// <summary>
        /// The size of this ship.
        /// </summary>
        ShipSizes Size { get; set; }

        /// <summary>
        /// The last time this ship was modified.
        /// </summary>
        DateTime TimeModified { get; set; }

        /// <summary>
        /// The type of this ship.
        /// </summary>
        ShipTypes Type { get; set; }

        /// <summary>
        /// The url of this ship on the RSI website.
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// The acceleration on the x-axis of this ship.
        /// </summary>
        double XAxisAcceleration { get; set; }

        /// <summary>
        /// The maximal amount of yaw of this ship.
        /// </summary>
        double YawMax { get; set; }

        /// <summary>
        /// The acceleration on the y-axis of this ship.
        /// </summary>
        double YAxisAcceleration { get; set; }

        /// <summary>
        /// The acceleration on the z-axis of this ship.
        /// </summary>
        double ZAxisAcceleration { get; set; }
    }

    #region Helper enums

    /// <summary>
    /// The different types of production status for ships.
    /// </summary>
    public enum ProductionStatusTypes
    {
#pragma warning disable 1591
        FlightReady,
        InConcept,
        InProduction
    }

    /// <summary>
    /// The different sizes of ships.
    /// </summary>
    public enum ShipSizes
    {
        Snub,
        Small,
        Medium,
        Vehicle,
        Large,
        Capital
    }

    /// <summary>
    /// The different types of ships.
    /// </summary>
    public enum ShipTypes
    {
        Multi,
        Combat,
        Transport,
        Exploration,
        Industrial,
        Support,
        Competition,
        Ground

    }

    /// <summary>
    /// The different classes of components of a ship.
    /// </summary>
    public enum ShipCompiledClasses
    {
        RSIAvionic,
        RSIModular,
        RSIPropulsion,
        RSIThruster,
        RSIWeapon
#pragma warning restore 1591
    }

    #endregion

}