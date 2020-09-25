namespace StarCitizenAPIWrapper.Library
{
    /// <summary>
    /// Client to connect to the Star Citizen API.
    /// </summary>
    public class StarCitizenClient
    {
        #region Static Instances

        /// <summary>
        /// The current <see cref="StarCitizenClient"/> instance.
        /// </summary>
        private static StarCitizenClient _currentClient;
        #endregion

        #region private fields

        private readonly string _apiKey;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="StarCitizenClient"/>.
        /// </summary>
        /// <param name="apiKey"></param>
        private StarCitizenClient(string apiKey)
        {
            _apiKey = apiKey;
        }

        #endregion

        #region static Methods

        /// <summary>
        /// Gives the current <see cref="StarCitizenClient"/>.
        /// Creates one if there isn't a current instance.
        /// </summary>
        public static StarCitizenClient GetClient()
        {
            return _currentClient ?? (_currentClient = new StarCitizenClient(""));
        }

        #endregion

    }
}
