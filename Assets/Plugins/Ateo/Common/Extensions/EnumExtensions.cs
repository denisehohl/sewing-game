using System;
using System.ComponentModel;
using System.Reflection;

namespace Ateo.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns the string value of a <see cref="System.ComponentModel.DescriptionAttribute"/> attribute of an enum value
        /// </summary>
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
        
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
            
                if (field != null)
                {
                    if (Attribute.GetCustomAttribute(field, 
                            typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                    {
                        return attr.Description;
                    }
                }
            }
            
            return null;
        }
    }
}