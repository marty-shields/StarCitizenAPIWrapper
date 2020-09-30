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

            var org = new StarCitizenOrganization();

            foreach (var propertyInfo in typeof(IOrganization).GetProperties())
            {
                var currentValue = propertyInfo.GetCorrectValueFromProperty(data);
                
                switch (propertyInfo.Name)
                {
                    case nameof(IOrganization.Archetype):
                    {
                        if (!Enum.TryParse(currentValue?.ToString(), out Archetypes type))
                            break;

                        propertyInfo.SetValue(org, type);

                        break;
                    }
                    case nameof(IOrganization.Focus):
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

                        propertyInfo.SetValue(org, focus);

                        break;
                    }
                    case nameof(IOrganization.Headline):
                    {
                        var headlineInfo = data?["headline"];
                        var html = headlineInfo?["html"]?.ToString();
                        var plainText = headlineInfo?["plaintext"]?.ToString();

                        propertyInfo.SetValue(org, (html, plainText));

                        break;
                    }
                    case nameof(IOrganization.Members):
                    {
                        if (int.TryParse(data?["members"]?.ToString(), out var members))
                            propertyInfo.SetValue(org, members);

                        break;
                    }
                    default:
                    {
                        propertyInfo.SetValue(org, GenericJsonParser.ParseValueIntoSupportedTypeSafe(currentValue?.ToString(), propertyInfo.PropertyType));

                        break;
                    }
                }
            }

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

            var members = new List<IOrganizationMember>();
            var memberJsonArray = data as JArray;
            foreach (var memberJson in memberJsonArray!)
            {
                var member = new StarCitizenOrganizationMember();
                foreach (var propertyInfo in typeof(IOrganizationMember).GetProperties())
                {
                    var currentValue = propertyInfo.GetCorrectValueFromProperty(memberJson);
                    
                    switch (propertyInfo.Name)
                    {
                        case nameof(IOrganizationMember.Roles):
                        {
                            var roles = memberJson?["roles"] as JArray;
                            var roleList = roles?.Select(x => x.ToString()).ToArray();

                            propertyInfo.SetValue(member, roleList);
                            break;
                        }
                        default:
                        {
                            propertyInfo.SetValue(member, GenericJsonParser.ParseValueIntoSupportedTypeSafe(currentValue?.ToString(), propertyInfo.PropertyType));

                            break;
                        }
                    }
                }

                members.Add(member);
            }

            return members;
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

            var ships = new List<IShip>();

            var shipsAsJson = data as JArray;
            foreach (var shipAsJson in shipsAsJson!)
            {
                if (shipAsJson.ToString() == string.Empty)
                    continue;

                var ship = new StarCitizenShip();
                foreach (var propertyInfo in typeof(IShip).GetProperties())
                {
                    var currentValue = propertyInfo.GetCorrectValueFromProperty(shipAsJson);
                    
                    switch (propertyInfo.Name)
                    {
                        case nameof(IShip.Media):
                        {
                            var mediaArray = shipAsJson["media"] as JArray;

                            propertyInfo.SetValue(ship, mediaArray!.Select(ParseShipMedia).ToArray());

                            break;
                        }
                        case nameof(IShip.Size):
                        {
                            if(Enum.TryParse(currentValue?.ToString(), true, out ShipSizes sizeResult))
                                propertyInfo.SetValue(ship, sizeResult);

                            break;
                        }
                        case nameof(IShip.Type):
                        {
                            if (Enum.TryParse(currentValue?.ToString(), true, out ShipTypes typeResult))
                                propertyInfo.SetValue(ship, typeResult);

                            break;
                        }
                        case nameof(IShip.ProductionStatus):
                        {
                            var valueToParse = currentValue?.ToString().Replace("-", "");

                            if(Enum.TryParse(valueToParse, true, out ProductionStatusTypes productionStatusTypeResult))
                                propertyInfo.SetValue(ship, productionStatusTypeResult);
                    
                            break;
                        }
                        case nameof(IShip.Compiled):
                        {
                            propertyInfo.SetValue(ship, ParseShipCompiled(currentValue));

                            break;
                        }
                        case nameof(IShip.Manufacturer):
                        {
                            propertyInfo.SetValue(ship, ParseManufacturer(currentValue));

                            break;
                        }
                        default:
                        {
                            if (currentValue?.ToString() == string.Empty)
                                break;

                            propertyInfo.SetValue(ship, GenericJsonParser.ParseValueIntoSupportedTypeSafe(currentValue?.ToString(), propertyInfo.PropertyType));

                            break;
                        }
                    }
                }

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
            
            var roadmaps =  new List<RoadMap>();

            foreach (var roadmapJson in data!)
            {
                var newRoadmap = new RoadMap();

                foreach (var propertyInfo in typeof(RoadMap).GetProperties())
                {
                    var currentValue = propertyInfo.GetCorrectValueFromProperty(roadmapJson);

                    switch (propertyInfo.Name)
                    {
                        case nameof(RoadMap.RoadMapCards):
                        {
                            propertyInfo.SetValue(newRoadmap, ParseRoadmapCards(roadmapJson["cards"]));

                            break;
                        }
                        case nameof(RoadMap.Released):
                        {
                            propertyInfo.SetValue(newRoadmap, currentValue!.ToString() == "1");

                            break;
                        }
                        default:
                        {
                            if (currentValue?.ToString() == string.Empty)
                                continue;

                            propertyInfo.SetValue(newRoadmap, GenericJsonParser.ParseValueIntoSupportedTypeSafe(currentValue?.ToString(), propertyInfo.PropertyType, true));

                            break;
                        }
                    }
                }

                roadmaps.Add(newRoadmap);
            }

            return roadmaps;
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

            var stats = new StarCitizenStats {CurrentLive = data?["current_live"]?.ToString()};

            if (long.TryParse(data?["fans"]?.ToString(), out var longResult))
                stats.Fans = longResult;

            var funds = data?["funds"]?.ToString();

            if (decimal.TryParse(funds, out var decimalResult))
                stats.Funds = decimalResult;

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

                    if (propertyInfo.PropertyType == typeof(int) && int.TryParse(currentValue?.ToString(), out var intResult))
                        propertyInfo.SetValue(newSpecies, intResult);
                    else
                        propertyInfo.SetValue(newSpecies, currentValue?.ToString());
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

                    if (propertyInfo.PropertyType == typeof(int)
                        && int.TryParse(currentValue?.ToString(), out var intResult))
                        propertyInfo.SetValue(affiliation, intResult);
                    else
                        propertyInfo.SetValue(affiliation, currentValue?.ToString());
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
        var userProfile = new StarCitizenUserProfile();
        foreach (var property in typeof(IUserProfile).GetProperties())
        {
            var currentValue = property.GetCorrectValueFromProperty(profileData);

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
                        property.SetValue(userProfile, GenericJsonParser.ParseValueIntoSupportedTypeSafe(currentValue?.ToString(), property.PropertyType));
                        break;
                    }
            }
        }

        return userProfile;
    }

        /// <summary>
        /// Parses the given organization information of a user into a <see cref="IUserOrganizationInfo"/>.
        /// </summary>
        private static IUserOrganizationInfo ParseUserOrganizationInfo(JToken userOrganizationData)
        {
            var organizationData = new UserOrganizationInfo();

            foreach (var property in typeof(UserOrganizationInfo).GetProperties())
            {
                var value = userOrganizationData[property.Name.ToLower()];
                property.SetValue(organizationData, value?.ToString());
            }

            return organizationData;
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
            if (int.TryParse(matchingSize?["height"]?.ToString(), out var intResult))
                newImage.Height = intResult;
            if (int.TryParse(matchingSize?["width"]?.ToString(), out intResult))
                newImage.Width = intResult;

            newImage.Mode = matchingSize?["mode"]?.ToString();

            return newImage;
        }

        /// <summary>
        /// Parses the given manufacturer information into a <see cref="ShipManufacturer"/>.
        /// </summary>
        private static ShipManufacturer ParseManufacturer(JToken currentValue)
        {
            var manufacturer = new ShipManufacturer();

            foreach (var property in typeof(ShipManufacturer).GetProperties())
            {
                var value = property.GetCorrectValueFromProperty(currentValue);
                
                if (property.PropertyType == typeof(int)
                    && int.TryParse(value?.ToString(), out var intResult))
                    property.SetValue(manufacturer, intResult);
                else
                    property.SetValue(manufacturer, value?.ToString());
            }

            return manufacturer;
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
                        Type = componentOfCurrentType["type"]?.ToString()
                    };

                    if (int.TryParse(componentOfCurrentType["mounts"]!.ToString(), out var intResult))
                        rsiComponent.Mounts = intResult;

                    if (int.TryParse(componentOfCurrentType["quantity"]!.ToString(), out intResult))
                        rsiComponent.Quantity = intResult;

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
            var list = new List<RoadMapCard>();

            var array = cardsAsJson as JArray;

            foreach (var cardAsJson in array!)
            {
                var card = new RoadMapCard();

                foreach (var propertyInfo in typeof(RoadMapCard).GetProperties())
                {
                    var currentValue = propertyInfo.GetCorrectValueFromProperty(cardAsJson);

                    if(currentValue == null)
                        continue;

                    switch (propertyInfo.Name)
                    {
                        case nameof(RoadMapCard.Thumbnail):
                        {
                            propertyInfo.SetValue(card, ParseRoadMapCardThumbnail(currentValue));

                            break;
                        }
                        default:
                        {
                            propertyInfo.SetValue(card, GenericJsonParser.ParseValueIntoSupportedTypeSafe(currentValue?.ToString(), propertyInfo.PropertyType, true));

                            break;
                        }
                    }
                }

                list.Add(card);
            }

            return list;
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
                                Name = affiliation["name"]?.ToString()
                            };

                            if (int.TryParse(affiliation["id"]?.ToString(), out var intResult))
                                newAffiliation.Id = intResult;

                            if(int.TryParse(affiliation["membership.id"]?.ToString(), out intResult))
                                newAffiliation.MembershipId = intResult;

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
