using Sirenix.OdinInspector;
using UnityEngine;

namespace Ateo.Common
{
	public abstract class ComponentPublishBehaviour<T> : MonoBehaviour, IComponentPublish<T> where T : Component
	{
		public static T Instance { get; private set; }

		protected void Awake()
		{
			if (this.Publish())
			{
				Instance = this as T;
				OnPublish();
			}
		}

		protected void OnDestroy()
		{
			if (this.Withdraw())
			{
				Instance = null;
				OnWithdraw();
			}
		}

		private void Start()
		{
			if (Instance == this)
			{
				OnStart();
			}
		}

		/// <summary>
		/// Override this method to reset any static variables
		/// </summary>
		public virtual void ResetStatics()
		{
			Instance = null;
		}

		/// <summary>
		/// Called during OnEnable after the Instance has been published
		/// </summary>
		protected virtual void OnPublish()
		{
		}

		/// <summary>
		/// Called during OnDisable after the Instance has been withdrawn
		/// </summary>
		protected virtual void OnWithdraw()
		{
		}

		/// <summary>
		/// Called during Start of the Instance
		/// </summary>
		protected virtual void OnStart()
		{
		}
	}

	public abstract class ComponentPublishSerializedBehaviour<T> : SerializedMonoBehaviour, IComponentPublish<T> where T : Component
	{
		public static T Instance { get; private set; }

		protected void Awake()
		{
			if (this.Publish())
			{
				Instance = this as T;
				OnPublish();
			}
		}

		protected void OnDestroy()
		{
			if (this.Withdraw())
			{
				Instance = null;
				OnWithdraw();
			}
		}

		private void Start()
		{
			if (Instance == this)
			{
				OnStart();
			}
		}

		/// <summary>
		/// Override this method to reset any static variables
		/// </summary>
		public virtual void ResetStatics()
		{
			Instance = null;
		}

		/// <summary>
		/// Called during OnEnable after the Instance has been published
		/// </summary>
		protected virtual void OnPublish()
		{
		}

		/// <summary>
		/// Called during OnDisable after the Instance has been withdrawn
		/// </summary>
		protected virtual void OnWithdraw()
		{
		}

		/// <summary>
		/// Called during Start of the Instance
		/// </summary>
		protected virtual void OnStart()
		{
		}
	}
}