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
using StarCitizenAPIWrapper.Models.Organization;
using StarCitizenAPIWrapper.Models.Organization.Implementations;
using StarCitizenAPIWrapper.Models.Organization.Members;
using StarCitizenAPIWrapper.Models.Organization.Members.Implementations;
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
        private const string ApiLiveRequestUrl = "https://api.starcitizen-api.com/{0}/v1/live/{1}";

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

        #endregion

    }
}
