using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using StarCitizenAPIWrapper.Models.Attributes;
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
                var currentValue = data?[propertyInfo.Name.ToLower()];
                var attributes = propertyInfo.GetCustomAttributes(true);

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
                        if (attributes.Any(x => x is ApiNameAttribute))
                        {
                            var nameAttribute = attributes.Single(x => x is ApiNameAttribute) as ApiNameAttribute;
                            currentValue = data?[nameAttribute?.Name!];
                        }

                        if (propertyInfo.PropertyType == typeof(bool)
                            && bool.TryParse(currentValue?.ToString(), out var boolResult))
                        {
                            propertyInfo.SetValue(org, boolResult);
                        }
                        else
                        {
                            propertyInfo.SetValue(org, currentValue?.ToString());
                        }

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
                    var currentValue = memberJson?[propertyInfo.Name.ToLower()];
                    var attributes = propertyInfo.GetCustomAttributes(true);

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
                            if (attributes.Any(x => x is ApiNameAttribute))
                            {
                                var nameAttribute = attributes.Single(x => x is ApiNameAttribute) as ApiNameAttribute;
                                currentValue = memberJson?[nameAttribute?.Name!];
                            }

                            if (propertyInfo.PropertyType == typeof(int)
                                && int.TryParse(currentValue?.ToString(), out var intResult))
                            {
                                propertyInfo.SetValue(member, intResult);
                            }
                            else
                            {
                                propertyInfo.SetValue(member, currentValue?.ToString());
                            }


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
                    JToken currentValue;
                    try
                    {
                        currentValue = shipAsJson[propertyInfo.Name.ToLower()];
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    var attributes = propertyInfo.GetCustomAttributes(true);

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
                            var nameAttribute = attributes.Single(x => x is ApiNameAttribute) as ApiNameAttribute;
                            currentValue = shipAsJson[nameAttribute?.Name!];

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
                            if (attributes.Any(x => x is ApiNameAttribute))
                            {
                                var nameAttribute = attributes.Single(x => x is ApiNameAttribute) as ApiNameAttribute;
                                currentValue = shipAsJson[nameAttribute?.Name!];
                            }

                            if (currentValue?.ToString() == string.Empty)
                                break;

                            if (propertyInfo.PropertyType == typeof(int)
                                && int.TryParse(currentValue?.ToString(), out var intResult))
                                propertyInfo.SetValue(ship, intResult);
                            else if (propertyInfo.PropertyType == typeof(double)
                                     && double.TryParse(currentValue?.ToString(), out var doubleResult))
                                propertyInfo.SetValue(ship, doubleResult);
                            else
                                propertyInfo.SetValue(ship, currentValue?.ToString());

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
                    var currentValue = roadmapJson[propertyInfo.Name.ToLower()];
                    var attributes = propertyInfo.GetCustomAttributes(true);

                    switch (propertyInfo.Name)
                    {
                        case nameof(RoadMap.RoadMapCards):
                        {
                            break;
                        }
                        case nameof(RoadMap.Released):
                        {
                            break;
                        }
                        default:
                        {
                            if (attributes.Any(x => x is ApiNameAttribute))
                            {
                                var nameAttribute = attributes.Single(x => x is ApiNameAttribute) as ApiNameAttribute;
                                currentValue = roadmapJson![nameAttribute?.Name!];
                            }

                            if (currentValue?.ToString() == string.Empty)
                                continue;

                            if (propertyInfo.PropertyType == typeof(int)
                                && int.TryParse(currentValue!.ToString(), out var intResult))
                                propertyInfo.SetValue(newRoadmap, intResult);
                            else if (propertyInfo.PropertyType == typeof(DateTime))
                            {
                                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                                propertyInfo.SetValue(newRoadmap, epoch.AddSeconds(double.Parse(currentValue!.ToString())));
                            }
                            else
                            {
                                propertyInfo.SetValue(newRoadmap, currentValue?.ToString());
                            }

                            break;
                        }
                    }
                }

                roadmaps.Add(newRoadmap);
            }

            return roadmaps;
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
                var value = currentValue?[property.Name.ToLower()];
                var propertyAttributes = property.GetCustomAttributes(true);

                if (propertyAttributes.Any(x => x is ApiNameAttribute))
                {
                    var nameAttribute = propertyAttributes.Single(x => x is ApiNameAttribute) as ApiNameAttribute;
                    value = currentValue?[nameAttribute?.Name!];
                }

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
                    try
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
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }

            return components;
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
