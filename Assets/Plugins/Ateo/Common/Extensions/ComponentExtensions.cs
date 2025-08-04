using UnityEngine;

namespace Ateo.Extensions
{
	public static class ComponentExtensions
	{
		public static T GetOrAddComponent<T>(this Component c) where T : Component
		{
			T component = c.gameObject.GetComponent<T>();

			if (component == null)
				component = c.gameObject.AddComponent<T>() as T;

			return component;
		}
	}
}