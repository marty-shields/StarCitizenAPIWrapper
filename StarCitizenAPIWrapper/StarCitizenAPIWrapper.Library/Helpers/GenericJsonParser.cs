using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

namespace StarCitizenAPIWrapper.Library.Helpers
{ 
    /// <summary>
    /// Helper class to parse json objects into specific types.
    /// </summary>
    public static class GenericJsonParser
    {
        /// <summary>
        /// Parses the given value into an object of the given type if the conversion was successful.
        /// Otherwise gives back the value as string.
        /// </summary>
        public static object ParseValueIntoSupportedTypeSafe(string value, Type type, bool parseDateTimeFromTimeStamp = false)
        {
            if (type == typeof(string))
                return value;

            if (type == typeof(int) && int.TryParse(value, out var intResult))
                return intResult;
            if (type == typeof(double) && double.TryParse(value, out var doubleResult))
                return doubleResult;
            if(type == typeof(char) && char.TryParse(value, out var charResult))
                return charResult;
            if (type == typeof(decimal) && decimal.TryParse(value, out var decimalResult))
                return decimalResult;
            if (type == typeof(long) && long.TryParse(value, out var longResult))
                return longResult;
            if (type == typeof(bool))
                return value == "1";

            if (type == typeof(DateTime))
            {
                if (parseDateTimeFromTimeStamp)
                {
                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    return epoch.AddSeconds(double.Parse(value));
                }

                if(DateTime.TryParse(value, out var dateResult))
                    return dateResult;
            }
            return value;
        }

        /// <summary>
        /// Parses the given value into an object of the given type if the conversion was successful.
        /// Otherwise gives back the value as string.
        /// </summary>
        public static T ParseValueIntoSupportedTypeSafe<T>(string value, bool parseDateTimeFromTimeStamp = false)
        {
            return (T) ParseValueIntoSupportedTypeSafe(value, typeof(T), parseDateTimeFromTimeStamp);
        }
    }
}
