namespace StarCitizenAPIWrapper.Models.Version
{
    /// <summary>
    /// Interface for the api information about versions.
    /// </summary>
    public interface IVersion
    {
        /// <summary>
        /// Array of all existing versions
        /// </summary>
        string[] Versions { get; set; }
    }
}
