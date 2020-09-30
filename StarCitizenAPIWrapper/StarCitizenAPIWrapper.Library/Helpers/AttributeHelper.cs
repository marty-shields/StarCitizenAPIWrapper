using System;
using System.Collections.Generic;
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
            var attributes = propertyInfo.GetCustomAttributesIncludingBaseInterfaces<ApiNameAttribute>()?.ToList();

            if (attributes == null || !attributes.Any()) 
                return currentJson[propertyInfo.Name.ToLower()];
            
            var nameAttribute = attributes.Single();
            return currentJson![nameAttribute?.Name!];
        }

        /// <summary>
        /// Gets custom attributes even if they are from the derived interface.
        /// </summary>
        public static IEnumerable<T> GetCustomAttributesIncludingBaseInterfaces<T>(this PropertyInfo propertyInfo)
        {
            var interfaces = propertyInfo.ReflectedType?.GetInterfaces() ?? new List<Type>().ToArray();
            var attributeType = typeof(T);
            var customAttributesDirect = propertyInfo.GetCustomAttributes(attributeType, true);

            if (interfaces.Any(x => x.GetProperty(propertyInfo.Name) != null))
            {
                var customAttributesOnInterface = interfaces.SelectMany(x => x.GetProperty(propertyInfo.Name)
                    ?.GetCustomAttributes(attributeType,
                        true));

                return customAttributesDirect
                    .Union(customAttributesOnInterface)
                    .Distinct().Cast<T>();
            }

            return customAttributesDirect.Cast<T>();
        }
    }
}
