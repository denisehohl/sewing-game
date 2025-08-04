using UnityEngine;
using Ateo.Extensions;
using UnityEngine.Serialization;

namespace Ateo
{
	public class IgnoreCollisionSelf : MonoBehaviour
	{
		[FormerlySerializedAs("m_LayerMask"), SerializeField]
		private LayerMask _layerMask;

		private void Start()
		{
			Invoke(nameof(Ignore), 2f);
		}

		private void Ignore()
		{
			gameObject.DisableSelfCollisions(_layerMask);
		}
	}
}