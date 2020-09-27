namespace StarCitizenAPIWrapper.Models.Ships.Media
{
    /// <summary>
    /// Information from the API about a ship media image.
    /// </summary>
    public class ShipMediaImage
    {
        /// <summary>
        /// The Url of this image.
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// The height of this image.
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// The image mode of this image.
        /// </summary>
        public string Mode { get; set; }
        /// <summary>
        /// The width of this image.
        /// </summary>
        public int Width { get; set; }
    }
}
