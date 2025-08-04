using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ateo.Common.Util
{
	public static class AssemblyUtilities
	{
		/// <summary>
		/// Get types from the current AppDomain
		/// </summary>
		/// <returns>Types from the current AppDomain</returns>
		public static IEnumerable<Type> GetTypes()
		{
			Assembly[] assemblyArray = AppDomain.CurrentDomain.GetAssemblies();

			foreach (Assembly assembly in assemblyArray)
			{
				Type[] typeArray = assembly.SafeGetTypes();

				foreach (Type t in typeArray)
				{
					yield return t;
				}
			}
		}

		public static Type[] SafeGetTypes(this Assembly assembly)
		{
			try
			{
				return assembly.GetTypes();
			}
			catch
			{
				return Type.EmptyTypes;
			}
		}
		
		/// <summary>
		/// Determines whether a type inherits or implements another type. Also include support for open generic base types such as List&lt;&gt;.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="baseType"></param>
		public static bool InheritsFrom(this Type type, Type baseType)
		{
			if (baseType.IsAssignableFrom(type))
				return true;
			if (type.IsInterface && !baseType.IsInterface)
				return false;
			if (baseType.IsInterface)
				return type.GetInterfaces().Contains(baseType);
			
			for (Type type1 = type; type1 != (Type) null; type1 = type1.BaseType)
			{
				if (type1 == baseType || baseType.IsGenericTypeDefinition && type1.IsGenericType && type1.GetGenericTypeDefinition() == baseType)
					return true;
			}
			
			return false;
		}
	}
}