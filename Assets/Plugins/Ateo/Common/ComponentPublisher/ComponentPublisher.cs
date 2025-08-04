using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ateo.Common
{
	public static class ComponentPublisher
	{
		private static readonly Dictionary<Type, IComponentPublish> Components = new Dictionary<Type, IComponentPublish>();

#if UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics()
		{
			foreach (IComponentPublish component in Components.Values)
			{
				component.ResetStatics();
			}

			Components.Clear();
		}
#endif

		public static bool Publish<T>(IComponentPublish<T> component, bool destroy) where T : Component
		{
			Type type = typeof(T);

			if (Components.TryGetValue(type, out IComponentPublish value))
			{
				if (value == null)
				{
					Components[type] = component;
					return true;
				}
			}
			else
			{
				Components.Add(type, component);
				return true;
			}

			DebugDev.LogWarning($"ComponentPublisher: Component of type {type} has already been published.");

			if (!destroy) return false;
			
			Object obj = (Object) component;

			if (obj != null)
			{
				Object.Destroy(obj);
			}

			return false;
		}

		public static bool Withdraw<T>(IComponentPublish<T> component) where T : Component
		{
			Type type = typeof(T);

			if (Components.TryGetValue(type, out IComponentPublish value))
			{
				if (value != component) return false;

				Components.Remove(type);
				component.ResetStatics();
				return true;
			}

			DebugDev.LogWarning($"ComponentPublisher: Component of type {type} could not be unpublished.");
			return false;
		}

		public static T GetComponent<T>() where T : Component
		{
			Type type = typeof(T);

			if (Components.TryGetValue(type, out IComponentPublish component))
			{
				return component as T;
			}

			return null;
		}

		public static bool TryGetComponent<T>(out T value) where T : Component
		{
			Type type = typeof(T);
			value = null;

			if (Components.TryGetValue(type, out IComponentPublish component))
			{
				value = component as T;
			}

			return value != null;
		}
	}

	public interface IComponentPublish
	{
		void ResetStatics();
	}

	public interface IComponentPublish<T> : IComponentPublish where T : Component
	{
	}

	public static class ComponentPublishHelper
	{
		public static bool Publish<T>(this IComponentPublish<T> component, bool destroy = true) where T : Component
		{
			return ComponentPublisher.Publish(component, destroy);
		}

		public static bool Withdraw<T>(this IComponentPublish<T> component) where T : Component
		{
			if (ComponentPublisher.Withdraw(component))
			{
#if UNITY_EDITOR
				component.ResetStatics();
#endif
				return true;
			}

			return false;
		}

		public static T Resolve<T>(this IComponentPublish<T> component) where T : Component
		{
			return ComponentPublisher.GetComponent<T>();
		}
	}
}