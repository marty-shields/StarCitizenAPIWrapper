using System;

namespace StarCitizenAPIWrapper.Models.Attributes
{
    /// <summary>
    /// A custom name attribute for properties.
    /// </summary>
    public class ApiNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ApiNameAttribute"/>/
        /// </summary>
        /// <param name="name"></param>
        public ApiNameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name set by the property attribute.
        /// </summary>
        public string Name { get; }
    }
}
