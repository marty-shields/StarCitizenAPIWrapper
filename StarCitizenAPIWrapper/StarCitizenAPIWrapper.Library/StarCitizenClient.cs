using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json.Linq;
using StarCitizenAPIWrapper.Library.Helpers;
using StarCitizenAPIWrapper.Models.Organization;
using StarCitizenAPIWrapper.Models.Organization.Implementations;
using StarCitizenAPIWrapper.Models.Organization.Members;
using StarCitizenAPIWrapper.Models.Organization.Members.Implementations;
using StarCitizenAPIWrapper.Models.RoadMap;
using StarCitizenAPIWrapper.Models.Ships;
using StarCitizenAPIWrapper.Models.Ships.Compiled;
using StarCitizenAPIWrapper.Models.Ships.Implementations;
using StarCitizenAPIWrapper.Models.Ships.Manufacturer;
using StarCitizenAPIWrapper.Models.Ships.Media;
using StarCitizenAPIWrapper.Models.Starmap.Affiliations;
using StarCitizenAPIWrapper.Models.Starmap.Object;
using StarCitizenAPIWrapper.Models.Starmap.Search;
using StarCitizenAPIWrapper.Models.Starmap.Species;
using StarCitizenAPIWrapper.Models.Starmap.Systems;
using StarCitizenAPIWrapper.Models.Starmap.Systems.Implementations;
using StarCitizenAPIWrapper.Models.Starmap.Tunnels;
using StarCitizenAPIWrapper.Models.Stats;
using StarCitizenAPIWrapper.Models.User;
using StarCitizenAPIWrapper.Models.User.Implementations;
using StarCitizenAPIWrapper.Models.Version;
using StarCitizenAPIWrapper.Models.Version.Implementations;

namespace StarCitizenAPIWrapper.Library
{
    /// <summary>
    /// Client to connect to the Star Citizen API.
    /// </summary>
    public class StarCitizenClient
    {
        #region const variables

        private const string ApiRequestUrl = "https://api.starcitizen-api.com/{0}/v1/eager/{1}";
        private const string ApiLiveRequestUrl = "https://api.starcitizen-api.com/{0}/v1/live/{1}";
        private const string ApiCacheRequestUrl = "https://api.starcitizen-api.com/{0}/v1/cache/{1}";

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
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);

            var content = await response.Content.ReadAsStringAsync();

            var data = JObject.Parse(content);
            var profileData = data["data"]?["profile"];
            var organizationData = data["data"]?["organization"];

            var user = new StarCitizenUser
            {
                Profile = ParseUserProfile(profileData),
                Organization = ParseUserOrganizationInfo(organizationData)
            };

            return user;
        }

        /// <summary>
        /// Sends an API request for organization information.
        /// </summary>
        /// <param name="sid">The SID of the organization</param>
        public async Task<IOrganization> GetOrganization(string sid)
        {
            var requestUrl = string.Format(ApiRequestUrl, _apiKey, $"organization/{sid}");
            using var client = new HttpClient();
            var response = await client.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);

            var content = await response.Content.ReadAsStringAsync();

            var data = JObject.Parse(content)?["data"];

            var customBehaviour = new Dictionary<string, Func<JToken, object>>
            {
                {
                    nameof(IOrganization.Archetype), delegate(JToken currentValue)
                    {
                        if (!Enum.TryParse(currentValue?.ToString(), out Archetypes type))
                            return Archetypes.Undefined;

                        return type;
                    }
                },
                {
                    nameof(IOrganization.Focus), delegate(JToken currentValue)
                    {
                        var focus = new Focus();
                        var primary = data?["focus"]?["primary"];
                        var secondary = data?["focus"]?["secondary"];

                        focus.PrimaryFocusImage = primary?["image"]?.ToString();
                        focus.SecondaryFocusImage = secondary?["image"]?.ToString();

                        if (Enum.TryParse(primary?["name"]?.ToString(), out FocusTypes focusType))
                            focus.PrimaryFocus = focusType;

                        if (Enum.TryParse(secondary?["name"]?.ToString(), out focusType))
                            focus.SecondaryFocus = focusType;

                        return focus;
                    }
                },
                {
                    nameof(IOrganization.Headline), delegate(JToken currentValue)
                    {
                        var headlineInfo = data?["headline"];
                        var html = headlineInfo?["html"]?.ToString();
                        var plainText = headlineInfo?["plaintext"]?.ToString();

                        return (html, plainText);
                    }
                }
            };

            var org = GenericJsonParser.ParseJsonIntoNewInstanceOfGivenType<StarCitizenOrganization>(data,
                customBehaviour);

            return org;
        }

        /// <summary>
        /// Sends an API request for members of an organization.
        /// </summary>
        /// <param name="sid">The SID of the organization.</param>
        public async Task<List<IOrganizationMember>> GetOrganizationMembers(string sid)
        {
            var requestUrl = string.Format(ApiLiveRequestUrl, _apiKey, $"organization_members/{sid}");
            using var client = new HttpClient();
            var response = await client.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);

            var content = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(content)?["data"];

            var customParseBehaviour = new Dictionary<string, Func<JToken, object>>
            {
                {
                    nameof(IOrganizationMember.Roles), delegate(JToken currentValue)
                    {
                        var roles = currentValue as JArray;
                        return roles?.Select(x => x.ToString()).ToArray();
                    }
                }
            };


            var memberJsonArray = data as JArray;

            return memberJsonArray!
                .Select(memberJson =>
                    GenericJsonParser.ParseJsonIntoNewInstanceOfGivenType<StarCitizenOrganizationMember>(memberJson,
                        customParseBehaviour)).Cast<IOrganizationMember>().ToList();
        }

        /// <summary>
        /// Sends an API request for current existing versions.
        /// </summary>
        /// <returns></returns>
        public async Task<IVersion> GetVersions()
        {
            var requestUrl = string.Format(ApiRequestUrl, _apiKey, "versions");
            using var client = new HttpClient();
            var response = await client.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);

            var content = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(content)?["data"];

            var version = new StarCitizenVersion {Versions = ((JArray) data)!.Select(x => x.ToString()).ToArray()};

            return version;
        }

        /// <summary>
        /// Sends an API request for the ships within the specified parameters.
        /// </summary>
        public async Task<List<IShip>> GetShips(ShipRequest request)
        {
            var requestUrl = string.Format(ApiCacheRequestUrl, _apiKey, $"/ships?{string.Join("&", request.RequestParameters)}");
            using var client = new HttpClient();
            var response = await client.GetAsync(requestUrl);
            if(!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);

            var content = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(content)?["data"];

            var customParseBehaviour = new Dictionary<string, Func<JToken, object>>
            {
                {
                    nameof(IShip.Media),
                    currentValue => (currentValue as JArray)!.Select(ParseShipMedia).ToArray()
                },
                {
                    nameof(IShip.Size), currentValue =>
                        Enum.TryParse(currentValue?.ToString(), true, out ShipSizes sizeResult)
                            ? sizeResult
                            : ShipSizes.Undefined
                },
                {
                    nameof(IShip.Type), currentValue =>
                        Enum.TryParse(currentValue?.ToString(), true, out ShipTypes typeResult)
                            ? typeResult
                            : ShipTypes.Undefined
                },
                {
                    nameof(IShip.ProductionStatus), delegate(JToken currentValue)
                    {
                        var valueToParse = currentValue?.ToString().Replace("-", "");

                        return Enum.TryParse(valueToParse,
                            true,
                            out ProductionStatusTypes productionStatusTypeResult)
                            ? productionStatusTypeResult
                            : ProductionStatusTypes.Undefined;
                    }
                },
                {
                    nameof(IShip.Compiled), ParseShipCompiled
                },
                {
                    nameof(IShip.Manufacturer), ParseManufacturer
                }
            };

            var ships = new List<IShip>();

            var shipsAsJson = data as JArray;

            foreach (var shipAsJson in shipsAsJson!)
            {
                if (shipAsJson.ToString() == string.Empty)
                    continue;

                var ship = GenericJsonParser.ParseJsonIntoNewInstanceOfGivenType<StarCitizenShip>(shipAsJson,
                    customParseBehaviour);
                
                ships.Add(ship);
            }

            return ships;
        }

        /// <summary>
        /// Sends an API request for the roadmap of the given type.
        /// </summary>
        public async Task<List<RoadMap>> GetRoadmap(RoadmapTypes roadmapType, string version)
        {
            var requestUrl = string.Format(ApiRequestUrl, _apiKey, $"roadmap/{roadmapType.ToString().ToLower()}?version={version}");
            using var client = new HttpClient();
            var response = await client.GetAsync(requestUrl);
            if(!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);

            var content = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(content)["data"] as JArray;

            var customParseBehaviour = new Dictionary<string, Func<JToken, object>>
            {
                {
                    nameof(RoadMap.RoadMapCards), ParseRoadmapCards
                },
                {
                    nameof(RoadMap.Released), currentValue => currentValue!.ToString() == "1"
                }
            };

            return data!.Select(roadmapJson => GenericJsonParser.ParseJsonIntoNewInstanceOfGivenType<RoadMap>(
                    roadmapJson,
                    customParseBehaviour))
                .ToList();
        }

        /// <summary>
        /// Sends an API request for the current star citizen stats.
        /// </summary>
        /// <returns></returns>
        public async Task<StarCitizenStats> GetStats()
        {
            var requestUrl = string.Format(ApiRequestUrl, _apiKey, "stats");
            using var client = new HttpClient();
            var response = await client.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);

            var content = await response.Content.ReadAsStringAsync();

            var data = JObject.Parse(content)["data"];

            var stats = new StarCitizenStats
            {
                CurrentLive = data?["current_live"]?.ToString(),
                Fans = GenericJsonParser.ParseValueIntoSupportedTypeSafe<long>(data?["fans"]?.ToString()),
                Funds = GenericJsonParser.ParseValueIntoSupportedTypeSafe<decimal>(data?["funds"]?.ToString())
            };

            return stats;
        }

        /// <summary>
        /// Sends an API request for all star systems;
        /// </summary>
        public async Task<List<IStarmapSystem>> GetAllSystems()
        {
            var requestUrl = string.Format(ApiRequestUrl, _apiKey, "starmap/systems");
            return await GetSystems(requestUrl);
        }

        /// <summary>
        /// Sends an API request for the star system information with the given name.
        /// </summary>
        public async Task<List<IStarmapSystem>> GetSystem(string name)
        {
            var requestUrl = string.Format(ApiRequestUrl, _apiKey, $"starmap/systems?name={name}");
            return await GetSystems(requestUrl);
        }

        /// <summary>
        /// Gets the tunnel with the given id or all tunnels.
        /// </summary>
        public async Task<List<StarmapTunnel>> GetTunnels(string id = "")
        {
            var requestUrl = string.Format(ApiRequestUrl, _apiKey,
                string.IsNullOrEmpty(id) ? "starmap/tunnels" : $"starmap/tunnels?id={id}");
            using var client = new HttpClient();
            var response = await client.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);

            var content = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(content)["data"];

            var tunnelList = new List<StarmapTunnel>();

            if(data?.Type == JTokenType.Array)
                tunnelList.AddRange((data as JArray)!.Select(ParseStarmapTunnel));
            else
                tunnelList.Add(ParseStarmapTunnel(data));

            return tunnelList;
        }

        /// <summary>
        /// Gets the information of the species from the API.
        /// </summary>
        /// <param name="name">The name if a specific one is requested.</param>
        public async Task<List<StarCitizenSpecies>> GetSpecies(string name = "")
        {
            var requestUrl = string.Format(ApiRequestUrl, _apiKey, 
                string.IsNullOrEmpty(name) 
                    ? "starmap/species"
                    : $"starmap/species?name={name}");

            using var client = new HttpClient();
            var response = await client.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);

            var content = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(content)["data"];

            var speciesList = new List<StarCitizenSpecies>();

            if (string.IsNullOrEmpty(data?.ToString()))
                return speciesList;

            static StarCitizenSpecies ParseSpecies(JToken speciesAsJson)
            {
                var newSpecies = new StarCitizenSpecies();

                foreach (var propertyInfo in typeof(StarCitizenSpecies).GetProperties())
                {
                    var currentValue = propertyInfo.GetCorrectValueFromProperty(speciesAsJson);

                    propertyInfo.SetValue(newSpecies, GenericJsonParser.ParseValueIntoSupportedTypeSafe(currentValue?.ToString(), propertyInfo.PropertyType));
                }

                return newSpecies;
            }

            if (data?.Type == JTokenType.Array)
                speciesList.AddRange((data as JArray)!.Select(ParseSpecies));
            else
                speciesList.Add(ParseSpecies(data));

            return speciesList;
        }

        /// <summary>
        /// Gets the information of the affiliations from the API.
        /// </summary>
        public async Task<List<StarCitizenAffiliation>> GetAffiliations(string name = "")
        {
            var requestUrl = string.Format(ApiRequestUrl, _apiKey,
                string.IsNullOrEmpty(name)
                    ? "starmap/affiliations"
                    : $"starmap/affiliations?name={name}");
            using var client = new HttpClient();
            var response = await client.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);

            var content = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(content)["data"];

            var affiliations = new List<StarCitizenAffiliation>();

            if (string.IsNullOrEmpty(data?.ToString()))
                return affiliations;

            static StarCitizenAffiliation ParseAffiliation(JToken affiliationJson)
            {
                var affiliation = new StarCitizenAffiliation();

                foreach (var propertyInfo in typeof(StarCitizenAffiliation).GetProperties())
                {
                    var currentValue = propertyInfo.GetCorrectValueFromProperty(affiliationJson);
                    
                    propertyInfo.SetValue(affiliation, GenericJsonParser.ParseValueIntoSupportedTypeSafe(currentValue?.ToString(), propertyInfo.PropertyType));
                }

                return affiliation;
            }
            
            if(data?.Type == JTokenType.Array)
                affiliations.AddRange((data as JArray)!.Select(ParseAffiliation));
            else
                affiliations.Add(ParseAffiliation(data));

            return affiliations;
        }

        /// <summary>
        /// Gets the information from the API of the given object code.
        /// </summary>
        public async Task<StarCitizenStarMapObject> GetObject(string code)
        {
            var requestUrl = string.Format(ApiRequestUrl, _apiKey, $"starmap/object?code={code}");
            using var client = new HttpClient();
            var response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(content)["data"];

            return ParseStarCitizenStarMapObject(data);
        }

        /// <summary>
        /// Gets the information from the API of the given system code.
        /// </summary>
        public async Task<StarmapSystemDetail> GetStarmapSystem(string code)
        {
            var requestUrl = string.Format(ApiRequestUrl, _apiKey, $"starmap/star-system?code={code}");
            using var client = new HttpClient();
            var response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(content)["data"];

            var newSystemDetail = (StarmapSystemDetail)ParseStarmapSystem<StarmapSystemDetail>(data);

            newSystemDetail.CelestialObjects = ((JArray) data!["celestial_objects"]!)!.Select(ParseStarCitizenStarMapObject).ToList();

            newSystemDetail.FrostLine = double.Parse(data["frost_line"]!.ToString());
            newSystemDetail.HabitableZoneInner = double.Parse(data["habitable_zone_inner"]!.ToString());
            newSystemDetail.HabitableZoneOuter = double.Parse(data["habitable_zone_outer"]!.ToString());

            return newSystemDetail;
        }

        /// <summary>
        /// Gets a specific starmap object with the given name.
        /// </summary>
        public async Task<StarmapSearchResult> GetStarmapObjectFromName(string name)
        {
            var requestUrl = string.Format(ApiRequestUrl, _apiKey, $"starmap/search?name={name}");
            using var client = new HttpClient();
            var response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(content)["data"];

            var searchResult =  new StarmapSearchResult();
            
            if (string.IsNullOrEmpty(data?.ToString()))
                return searchResult;

            searchResult.StarmapSearchObjects = (data!["objects"]!.Children())!.Select(ParseStarmapSearchObject).ToList();
            searchResult.StarSystems = ((JArray) data!["systems"]!)!.Select(ParseStarmapSearchObjectSystem).ToList();
            return searchResult;
        }

        #endregion

        #region private helper methods


        /// <summary>
        /// Parses the given profile json into a <see cref="IUserProfile"/>.
        /// </summary>
        /// <param name="profileData">The <see cref="JToken"/> containing the profile information.</param>
        /// <returns>A new instance of <see cref="IUserProfile"/> containing the parsed information.</returns>
        private static IUserProfile ParseUserProfile(JToken profileData)
        {
            var customBehaviour = new Dictionary<string, Func<JToken, object>>
            {
                {
                    nameof(StarCitizenUserProfile.Page),
                    currentValue => (currentValue?["title"]?.ToString(), currentValue?["url"]?.ToString())
                },
                {
                    nameof(StarCitizenUserProfile.Enlisted), currentValue => DateTime.Parse(currentValue?.ToString())
                },
                {
                    nameof(StarCitizenUserProfile.Fluency), delegate(JToken currentValue)
                    {
                        var array = currentValue as JArray;
                        return array!.Select(arrayItem => arrayItem.ToString()).ToArray();
                    }
                }
            };

            var userProfile =
                GenericJsonParser.ParseJsonIntoNewInstanceOfGivenType<StarCitizenUserProfile>(profileData,
                    customBehaviour);
            
            return userProfile;
        }

        /// <summary>
        /// Parses the given organization information of a user into a <see cref="IUserOrganizationInfo"/>.
        /// </summary>
        private static IUserOrganizationInfo ParseUserOrganizationInfo(JToken userOrganizationData)
        {
            return GenericJsonParser.ParseJsonIntoNewInstanceOfGivenType<UserOrganizationInfo>(userOrganizationData,
                new Dictionary<string, Func<JToken, object>>());
        }

        #region Parse Ships

        /// <summary>
        /// Parses the given media information into a <see cref="ApiMedia"/>.
        /// </summary>
        private static ApiMedia ParseShipMedia(JToken shipMediaData)
        {
            var media = new ApiMedia
            {
                SourceName = shipMediaData["source_name"]?.ToString(),
                SourceUrl = shipMediaData["source_url"]?.ToString()
            };

            var sizes = shipMediaData["derived_data"]?["sizes"];

            var mediaList = (from JToken jToken in shipMediaData["images"]?.Children()! select ParseShipMediaImage(jToken, sizes)).ToList();

            media.MediaImages = mediaList;

            return media;
        }

        /// <summary>
        /// Parses the given media image information into a <see cref="ShipMediaImage"/>.
        /// </summary>
        private static ShipMediaImage ParseShipMediaImage(JToken shipMediaImageData, JToken sizes)
        {
            var imageMedia = (JProperty)shipMediaImageData;
            var newImage = new ShipMediaImage { ImageUrl = imageMedia.Value.ToString() };
            var matchingSize = sizes?[imageMedia.Name];
            
            newImage.Height = GenericJsonParser.ParseValueIntoSupportedTypeSafe<int>(matchingSize?["height"]?.ToString());
            newImage.Width = GenericJsonParser.ParseValueIntoSupportedTypeSafe<int>(matchingSize?["width"]?.ToString());

            newImage.Mode = matchingSize?["mode"]?.ToString();

            return newImage;
        }

        /// <summary>
        /// Parses the given manufacturer information into a <see cref="ShipManufacturer"/>.
        /// </summary>
        private static ShipManufacturer ParseManufacturer(JToken currentValue)
        {
            return GenericJsonParser.ParseJsonIntoNewInstanceOfGivenType<ShipManufacturer>(currentValue,
                new Dictionary<string, Func<JToken, object>>());
        }

        /// <summary>
        /// Parses the given compiled information of a ship into a 
        /// </summary>
        private static List<KeyValuePair<ShipCompiledClasses, List<KeyValuePair<string, RsiShipComponent>>>> ParseShipCompiled(JToken currentValue)
        {
            var compiled = new List<KeyValuePair<ShipCompiledClasses, List<KeyValuePair<string, RsiShipComponent>>>>();

            var shipComponentGroups = currentValue!.Children();
            foreach (var shipComponentGroupJson in shipComponentGroups!)
            {
                var shipComponentGroup = shipComponentGroupJson as JProperty;
                var componentTypes = shipComponentGroup!.First!.Children();

                var shipComponentClass = (ShipCompiledClasses)Enum.Parse(typeof(ShipCompiledClasses), shipComponentGroup.Name);
                compiled.Add(
                    new KeyValuePair<ShipCompiledClasses, List<KeyValuePair<string, RsiShipComponent>>>(
                        shipComponentClass, ParseShipComponents(componentTypes)));
            }

            return compiled;
        }

        /// <summary>
        /// Parses the given component types into a list of ship components.
        /// </summary>
        private static List<KeyValuePair<string, RsiShipComponent>> ParseShipComponents(
            JEnumerable<JToken> componentTypes)
        {
            var components = new List<KeyValuePair<string, RsiShipComponent>>();

            foreach (var componentTypeJson in componentTypes)
            {
                var componentType = componentTypeJson as JProperty;

                var componentsOfCurrentType = componentTypeJson!.First!.Children();

                foreach (var componentOfCurrentType in componentsOfCurrentType)
                {
                    var rsiComponent = new RsiShipComponent
                    {
                        Name = componentOfCurrentType["name"]?.ToString(),
                        Class = componentOfCurrentType["component_class"]?.ToString(),
                        Details = componentOfCurrentType["details"]?.ToString(),
                        Manufacturer = componentOfCurrentType["manufacturer"]?.ToString(),
                        Size = componentOfCurrentType["size"]?.ToString(),
                        Type = componentOfCurrentType["type"]?.ToString(),
                        Mounts = GenericJsonParser.ParseValueIntoSupportedTypeSafe<int>(
                            componentOfCurrentType["mounts"]!
                                .ToString()),
                        Quantity = GenericJsonParser.ParseValueIntoSupportedTypeSafe<int>(
                            componentOfCurrentType["quantity"]!
                                .ToString())
                    };

                    components.Add(new KeyValuePair<string, RsiShipComponent>(componentType!.Name, rsiComponent));
                }
            }

            return components;
        }

        #endregion

        #region Parse Roadmap

        /// <summary>
        /// Parses the given information of roadmap cards into a list of <see cref="RoadMapCard"/>
        /// </summary>
        private static List<RoadMapCard> ParseRoadmapCards(JToken cardsAsJson)
        {
            var array = cardsAsJson as JArray;

            var customBehaviour = new Dictionary<string, Func<JToken, object>>
            {
                {nameof(RoadMapCard.Thumbnail), ParseRoadMapCardThumbnail}
            };

            return array!.Select(cardAsJson => GenericJsonParser.ParseJsonIntoNewInstanceOfGivenType<RoadMapCard>(cardAsJson, customBehaviour)).ToList();
        }

        /// <summary>
        /// Parses the given information into a <see cref="RoadMapCardThumbnail"/>.
        /// </summary>
        private static RoadMapCardThumbnail ParseRoadMapCardThumbnail(JToken currentValue)
        {
            var thumbnail = new RoadMapCardThumbnail {Id = currentValue["id"]?.ToString()};

            foreach (var x in currentValue["urls"]?.Children().Select(x => x as JProperty).ToList()!) 
                thumbnail.Urls.Add(x!.Name, x.ToString());

            return thumbnail;
        }

        #endregion

        #region Parse Starstytems

        /// <summary>
        /// Sends an API request for the star system information with the given name.
        /// </summary>
        private static async Task<List<IStarmapSystem>> GetSystems(string requestUrl)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);

            var content = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(content)["data"];

            var systemList = new List<IStarmapSystem>();

            if (data?.Type == JTokenType.Array)
            {
                systemList.AddRange(data.Select(ParseStarmapSystem<StarmapSystem>).ToList());
            }
            else
            {
                systemList.Add((StarmapSystem) ParseStarmapSystem<StarmapSystem>(data));
            }

            return systemList;
        }

        /// <summary>
        /// Parses the given json data into a <see cref="StarmapSystem"/>.
        /// </summary>
        private static IStarmapSystem ParseStarmapSystem<T>(JToken starmapSystemJson) where T : IStarmapSystem
        {
            var system = (T) Activator.CreateInstance(typeof(T));

            foreach (var propertyInfo in typeof(IStarmapSystem).GetProperties())
            {
                var currentValue = propertyInfo.GetCorrectValueFromProperty(starmapSystemJson);

                if(currentValue == null)
                    continue;

                switch (propertyInfo.Name)
                {
                    case nameof(IStarmapSystem.Affiliations):
                    {
                        var affiliationList = new List<StarmapSystemAffiliation>();

                        foreach (var affiliation in currentValue)
                        {
                            var newAffiliation = new StarmapSystemAffiliation
                            {
                                Code = affiliation["code"]?.ToString(),
                                Color = affiliation["color"]?.ToString(),
                                Name = affiliation["name"]?.ToString(),
                                Id = GenericJsonParser.ParseValueIntoSupportedTypeSafe<int>(affiliation["id"]
                                    ?.ToString()),
                                MembershipId = GenericJsonParser.ParseValueIntoSupportedTypeSafe<int>(
                                    affiliation["membership.id"]
                                        ?.ToString())
                            };

                            affiliationList.Add(newAffiliation);
                        }
                        
                        propertyInfo.SetValue(system, affiliationList);
                        break;
                    }
                    case nameof(IStarmapSystem.Thumbnail):
                    {
                        var thumbnail = new StarmapSystemThumbnail
                        {
                            Slug = currentValue["slug"]?.ToString(), 
                            Source = currentValue["source"]?.ToString()
                        };

                        foreach (var jToken in currentValue["images"]!)
                        {
                            var child = (JProperty) jToken;
                            thumbnail.Images.Add(child.Name, child.Value.ToString());
                        }

                        propertyInfo.SetValue(system, thumbnail);

                        break;
                    }
                    default:
                    {
                        propertyInfo.SetValue(system, GenericJsonParser.ParseValueIntoSupportedTypeSafe(currentValue?.ToString(), propertyInfo.PropertyType));
                     
                        break;
                    }
                }
            }

            return system;
        }

        /// <summary>
        /// Parses the given tunnel information into a <see cref="StarmapTunnel"/>.
        /// </summary>
        private static StarmapTunnel ParseStarmapTunnel(JToken starmapTunnelJson)
        {
            var newTunnel = new StarmapTunnel();

            foreach (var propertyInfo in typeof(StarmapTunnel).GetProperties())
            {
                var currentValue = propertyInfo.GetCorrectValueFromProperty(starmapTunnelJson);

                switch (propertyInfo.Name)
                {
                    case nameof(StarmapTunnel.Entry):
                    {
                        propertyInfo.SetValue(newTunnel, ParseStarmapTunnelEntry(currentValue));

                        break;
                    }
                    case nameof(StarmapTunnel.Exit):
                    {
                        propertyInfo.SetValue(newTunnel, ParseStarmapTunnelEntry(currentValue));

                        break;
                    }
                    default:
                    {
                        propertyInfo.SetValue(newTunnel, GenericJsonParser.ParseValueIntoSupportedTypeSafe(currentValue?.ToString(), propertyInfo.PropertyType));

                        break;
                    }
                }
            }

            return newTunnel;
        }

        /// <summary>
        /// Parses the given json data into a <see cref="StarmapTunnelEntry"/>.
        /// </summary>
        private static StarmapTunnelEntry ParseStarmapTunnelEntry(JToken starmapTunnelEntryJson)
        {
            var tunnelEntry = new StarmapTunnelEntry();

            foreach (var propertyInfo in typeof(StarmapTunnelEntry).GetProperties())
            {
                var currentValue = propertyInfo.GetCorrectValueFromProperty(starmapTunnelEntryJson);

                switch (propertyInfo.Name)
                {
                    default:
                    {
                        propertyInfo.SetValue(tunnelEntry, GenericJsonParser.ParseValueIntoSupportedTypeSafe(currentValue?.ToString(), propertyInfo.PropertyType));

                        break;
                    }
                }
            }

            return tunnelEntry;
        }
        /// <summary>
        /// Parses the given json data into a <see cref="StarCitizenStarMapObject"/>.
        /// </summary>
        private static StarCitizenStarMapObject ParseStarCitizenStarMapObject(JToken objectJson)
        {
            var newObject = new StarCitizenStarMapObject();

            foreach (var propertyInfo in typeof(StarCitizenStarMapObject).GetProperties())
            {
                var currentValue = propertyInfo.GetCorrectValueFromProperty(objectJson);

                if (string.IsNullOrEmpty(currentValue?.ToString()))
                    continue;

                switch (propertyInfo.Name)
                {
                    case nameof(StarCitizenStarMapObject.Affiliations):
                    {
                        var affiliationList = new List<StarmapSystemAffiliation>();

                        foreach (var arrayItemJson in (JArray)currentValue)
                        {
                            var affiliation = new StarmapSystemAffiliation
                            {
                                Name = arrayItemJson["name"]!.ToString(),
                                Id = int.Parse(arrayItemJson["id"]!.ToString()),
                                Code = arrayItemJson["code"]!.ToString(),
                                Color = arrayItemJson["color"]!.ToString(),
                                MembershipId = int.Parse(arrayItemJson["membership_id"]!.ToString())
                            };

                            affiliationList.Add(affiliation);
                        }

                        propertyInfo.SetValue(newObject, affiliationList);
                        break;
                    }
                    case nameof(StarCitizenStarMapObject.Children):
                    {
                        var list = currentValue.Children().Select(ParseStarCitizenStarMapObject).ToList();

                        propertyInfo.SetValue(newObject, list);

                        break;
                    }
                    case nameof(StarCitizenStarMapObject.SubType):
                    {
                        var subType = new StarMapObjectSubType
                        {
                            Name = currentValue["name"]!.ToString(),
                            Type = currentValue["type"]!.ToString(),
                            Id = int.Parse(currentValue["id"]!.ToString())
                        };

                        propertyInfo.SetValue(newObject, subType);
                        break;
                    }
                    case nameof(StarCitizenStarMapObject.Textures):
                    {
                        var textures = new StarMapObjectTextures
                        {
                            Source = currentValue["source"]!.ToString(),
                            Slug = currentValue["slug"]!.ToString()
                        };

                        foreach (var jToken in currentValue["images"]!.Children())
                        {
                            var child = (JProperty) jToken;
                            textures.Images.Add(child.Name, child.Value.ToString());
                        }

                        propertyInfo.SetValue(newObject, textures);

                        break;
                    }
                    default:
                        {
                            propertyInfo.SetValue(newObject, GenericJsonParser.ParseValueIntoSupportedTypeSafe(currentValue?.ToString(), propertyInfo.PropertyType));

                            break;
                        }
                }
            }

            return newObject;
        }
        /// <summary>
        /// Parses the given json data into a <see cref="StarmapSearchObjectStarSystem"/>.
        /// </summary>
        private StarmapSearchObjectStarSystem ParseStarmapSearchObjectSystem(JToken currentValue)
        {
            var system = new StarmapSearchObjectStarSystem
            {
                Name = currentValue["name"]!.ToString(), Type = currentValue["type"]!.ToString(),
                Id = int.Parse(currentValue["id"]!.ToString()), Code = currentValue["code"]!.ToString()
            };

            return system;
        }
        /// <summary>
        /// Parses the given json data into a <see cref="StarmapSearchObject"/>.
        /// </summary>
        private StarmapSearchObject ParseStarmapSearchObject(JToken data)
        {
            var newSearchObject = new StarmapSearchObject();

            foreach (var propertyInfo in typeof(StarmapSearchObject).GetProperties())
            {
                var currentValue = propertyInfo.GetCorrectValueFromProperty(data);

                switch (propertyInfo.Name)
                {
                    case nameof(StarmapSearchObject.StarSystem):
                    {
                        propertyInfo.SetValue(newSearchObject, ParseStarmapSearchObjectSystem(currentValue));

                        break;
                    }
                    default:
                    {
                        propertyInfo.SetValue(newSearchObject, GenericJsonParser.ParseValueIntoSupportedTypeSafe(currentValue?.ToString(), propertyInfo.PropertyType));

                        break;
                    }
                }
            }

            return newSearchObject;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// The different types of roadmap.
    /// </summary>
    public enum RoadmapTypes
    {
#pragma warning disable 1591
        StarCitizen,
        Squadron42
#pragma warning restore 1591
    }
}
