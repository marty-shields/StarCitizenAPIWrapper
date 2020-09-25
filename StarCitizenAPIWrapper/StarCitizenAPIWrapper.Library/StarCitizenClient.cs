using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using StarCitizenAPIWrapper.Models.Attributes;
using StarCitizenAPIWrapper.Models.User;
using StarCitizenAPIWrapper.Models.User.Implementations;

namespace StarCitizenAPIWrapper.Library
{
    /// <summary>
    /// Client to connect to the Star Citizen API.
    /// </summary>
    public class StarCitizenClient
    {
        #region const variables

        private const string ApiRequestUrl = "https://api.starcitizen-api.com/{0}/v1/eager/{1}";

        #endregion

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
        public static StarCitizenClient GetClient(string apiKey)
        {
            return _currentClient ??= new StarCitizenClient(apiKey);
        }

        #endregion

        #region public methods
        /// <summary>
        /// Sends an API request for user information.
        /// </summary>
        /// <param name="handle">The handle of the requested user.</param>
        /// <returns>An instance of <see cref="IUser"/> containing the information about the requested user.</returns>
        public async Task<IUser> GetUser(string handle)
        {
            var requestUrl = string.Format(ApiRequestUrl, _apiKey, $"user/{handle}");
            using var client = new HttpClient();
            var response = await client.GetAsync(requestUrl);
            if(!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);

            var content = await response.Content.ReadAsStringAsync();

            var data = JObject.Parse(content);
            var profileData = data["data"]?["profile"];

            var user = new StarCitizenUser
            {
                Profile = ParseUserProfile(profileData)
            };

            return user;
        }

        /// <summary>
        /// Parses the given profile json into a <see cref="IUserProfile"/>.
        /// </summary>
        /// <param name="profileData">The <see cref="JToken"/> containing the profile information.</param>
        /// <returns>A new instance of <see cref="IUserProfile"/> containing the parsed information.</returns>
        private static IUserProfile ParseUserProfile(JToken profileData)
        {
            var userProfile = new StarCitizenUserProfile();
            foreach (var property in typeof(StarCitizenUserProfile).GetProperties())
            {
                var currentValue = profileData?[property.Name.ToLower()];
                var attributes = property.GetCustomAttributes(true);

                switch (property.Name)
                {
                    case nameof(StarCitizenUserProfile.Page):
                    {
                        property.SetValue(userProfile, (currentValue?["title"]?.ToString(), currentValue?["url"]?.ToString()));
                        break;
                    }
                    case nameof(StarCitizenUserProfile.Enlisted):
                    {
                        var dateTime = DateTime.Parse(currentValue?.ToString());
                        property.SetValue(userProfile, dateTime);
                        break;
                    }
                    case nameof(StarCitizenUserProfile.Fluency):
                    {
                        var array = currentValue as JArray;
                        var languageList = array!.Select(arrayItem => arrayItem.ToString()).ToList();
                        property.SetValue(userProfile, languageList.ToArray());

                        break;
                    }
                    default:
                    {
                        if (attributes.Any(x => x is ApiNameAttribute))
                        {
                            var nameAttribute = attributes.Single(x => x is ApiNameAttribute) as ApiNameAttribute;
                            currentValue = profileData?[nameAttribute?.Name!];
                        }

                        property.SetValue(userProfile, currentValue?.ToString());
                        break;
                    }
                }
            }

            return userProfile;
        }

        #endregion

    }
}
