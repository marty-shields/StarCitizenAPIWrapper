using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using StarCitizenAPIWrapper.Models.Attributes;

namespace StarCitizenAPIWrapper.Library.Helpers
{
    /// <summary>
    /// Helper class to for using the custom attributes.
    /// </summary>
    public static class AttributeHelper
    {
        /// <summary>
        /// Gets the value from a json object with the correct name specified in the <see cref="ApiNameAttribute"/>.
        /// </summary>
        public static JToken GetCorrectValueFromProperty(this PropertyInfo propertyInfo, JToken currentJson)
        {
            var attributes = propertyInfo.GetCustomAttributes(true);
            
            if (!attributes.Any(x => x is ApiNameAttribute)) 
                return currentJson[propertyInfo.Name.ToLower()];
            
            var nameAttribute = attributes.Single(x => x is ApiNameAttribute) as ApiNameAttribute;
            return currentJson![nameAttribute?.Name!];
        }
    }
}
