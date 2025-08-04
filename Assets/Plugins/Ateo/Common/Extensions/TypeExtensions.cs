using System;

namespace Ateo.Extensions
{
	public static class TypeExtensions
	{
		public static string SimpleQualifiedName(this Type type)
		{
			string typeString = type.AssemblyQualifiedName;
			
			// We remove unnecessary bloat from the AssemblyQualifiedName
			// This bloat string may change in a future version of C#? Unsure...
			if (typeString != null)
			{
				int index = typeString.IndexOf(", mscorlib", StringComparison.Ordinal);

				if (index == -1)
				{
					index = typeString.IndexOf(", Version", StringComparison.Ordinal);
				}

				while (index > -1)
				{
					int start = index;
							
					while (index < typeString.Length)
					{
						char c = typeString[index];

						if (c == ']')
						{
							break;
						}

						index++;
					}

					typeString = typeString.Remove(start, index - start);
					index = typeString.IndexOf(", mscorlib", StringComparison.Ordinal);

					if (index == -1)
					{
						index = typeString.IndexOf(", Version", StringComparison.Ordinal);
					}
				}

				return typeString;
			}

			return string.Empty;
		}
	}
}