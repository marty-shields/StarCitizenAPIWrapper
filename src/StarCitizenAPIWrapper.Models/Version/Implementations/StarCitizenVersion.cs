namespace StarCitizenAPIWrapper.Models.Version.Implementations
{
    /// <summary>
    /// The default implementation of <see cref="IVersion"/>.
    /// </summary>
    public class StarCitizenVersion : IVersion
    {
        /// <inheritdoc />
        public string[] Versions { get; set; }
    }
}
