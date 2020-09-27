using System.Collections.Generic;

namespace StarCitizenAPIWrapper.Library
{
    /// <summary>
    /// A builder class to help creating a request for specific ships.
    /// </summary>
    public class ShipRequestBuilder
    {
        #region private fields

        private readonly List<string> _parameters = new List<string>();

        #endregion

        #region Builder configration methods

        /// <summary>
        /// Adds the specified classification to the request.
        /// </summary>
        /// <param name="classification">The classification to be added.</param>
        public ShipRequestBuilder AddClassification(string classification)
        {
            _parameters.Add($"classification={classification}");
            return this;
        }

        /// <summary>
        /// Sets the name of the ships to search.
        /// </summary>
        public ShipRequestBuilder WithName(string name)
        {
            _parameters.Add($"name={name}");
            return this;
        }

        /// <summary>
        /// Sets the minimal length of the ships to search.
        /// </summary>
        public ShipRequestBuilder WithLengthMin(int lengthMin)
        {
            _parameters.Add($"length_min={lengthMin}");
            return this;
        }

        /// <summary>
        /// Sets the maximal length of the ships to search.
        /// </summary>
        public ShipRequestBuilder WithLengthMax(int lengthMax)
        {
            _parameters.Add($"length_max={lengthMax}");
            return this;
        }

        /// <summary>
        /// Sets the minimal amount of crew members of the ships to search.
        /// </summary>
        public ShipRequestBuilder WithCrewMin(int crewMin)
        {
            _parameters.Add($"crew_min={crewMin}");
            return this;
        }

        /// <summary>
        /// Sets the maximal amount of crew members of the ships to search.
        /// </summary>
        public ShipRequestBuilder WithCrewMax(int crewMax)
        {
            _parameters.Add($"crew_max={crewMax}");
            return this;
        }

        /// <summary>
        /// Sets the minimal price of the ships to search.
        /// </summary>
        public ShipRequestBuilder WithPriceMin(int priceMin)
        {
            _parameters.Add($"price_min={priceMin}");
            return this;
        }

        /// <summary>
        /// Sets the maximal price of the ships to search.
        /// </summary>
        public ShipRequestBuilder WithPriceMax(int priceMax)
        {
            _parameters.Add($"price_max={priceMax}");
            return this;
        }
        
        /// <summary>
        /// Sets the minimal mass of the ships to search.
        /// </summary>
        public ShipRequestBuilder WithMassMin(int massMin)
        {
            _parameters.Add($"mass_min={massMin}");
            return this;
        }

        /// <summary>
        /// Sets the maximal mass of the ships to search.
        /// </summary>
        public ShipRequestBuilder WithMassMax(int massMax)
        {
            _parameters.Add($"mass_max={massMax}");
            return this;
        }

        /// <summary>
        /// Sets the maximal pages on the RSI website of the ships to search with the specified parameters.
        /// <remarks>
        /// Careful that per page it takes around 15 seconds to process those results.
        /// </remarks>
        /// </summary>
        public ShipRequestBuilder WithPageMax(int pageMax)
        {
            _parameters.Add($"page_max={pageMax}");
            return this;
        }
        /// <summary>
        /// Sets the id of the ship to search
        /// <remarks>
        /// This works only with the cached database
        /// because its the id of the API database.
        /// </remarks>
        /// </summary>
        public ShipRequestBuilder WithId(int id)
        {
            _parameters.Add($"id={id}");
            return this;
        }

        #endregion

        /// <summary>
        /// Builds the configured request for the api.
        /// </summary>
        /// <returns></returns>
        public ShipRequest Build()
        {
            return new ShipRequest {RequestParameters = _parameters.ToArray()};
        }
    }
}
