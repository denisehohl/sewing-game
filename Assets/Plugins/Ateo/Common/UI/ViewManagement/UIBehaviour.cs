using UnityEngine;

namespace Ateo.ViewManagement
{
	public abstract class UIBehaviour : MonoBehaviour
	{
		public abstract void OnShow();
		public abstract void OnHide();
	}
}
