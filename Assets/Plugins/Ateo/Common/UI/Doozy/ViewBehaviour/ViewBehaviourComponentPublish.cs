#if DOOZY_3 || DOOZY_4
using Ateo.Common;

namespace Ateo.UI
{
	public abstract class ViewBehaviourComponentPublish<T> : ViewBehaviour, IComponentPublish<T> where T : ViewBehaviour
	{
		public static T Instance { get; private set; }

		protected override void Awake()
		{
			if (!this.Publish()) return;
			
			base.Awake();
			Instance = this as T;
			OnPublish();
		}

		protected override void OnDestroy()
		{
			if (!this.Withdraw()) return;
			
			base.OnDestroy();
			OnWithdraw();
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
		/// Called during Awake after the Instance has been published to the <see cref="Ateo.Common.ComponentPublisher"/>
		/// </summary>
		protected virtual void OnPublish()
		{
		}

		/// <summary>
		/// Called during OnDestroy after the Instance has been withdrawn from the <see cref="Ateo.Common.ComponentPublisher"/>
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

		/// <summary>
		/// Shows the <see cref="Doozy.Runtime.UIManager.Containers.UIView"/> of this <see cref="Ateo.UI.ViewBehaviour"/>.
		/// </summary>
		/// <param name="instant">Instantly show the the view</param>
		/// <param name="triggerCallbacks">Trigger view callbacks</param>
		public static void Show(bool instant = false, bool triggerCallbacks = true)
		{
			if (Instance != null)
			{
				Instance.ShowView(instant, triggerCallbacks);
			}
		}
		
		/// <summary>
		/// Hides the <see cref="Doozy.Runtime.UIManager.Containers.UIView"/> of this <see cref="Ateo.UI.ViewBehaviour"/>.
		/// </summary>
		/// <param name="instant">Instantly hide the the view</param>
		/// <param name="triggerCallbacks">Trigger view callbacks</param>
		public static void Hide(bool instant = false, bool triggerCallbacks = true)
		{
			if (Instance != null)
			{
				Instance.HideView(instant, triggerCallbacks);
			}
		}
	}
}
#endif