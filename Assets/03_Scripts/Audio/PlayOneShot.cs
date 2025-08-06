using FMODUnity;
using UnityEngine;

namespace Moreno.SewingGame.Audio
{
	public class PlayOneShot : MonoBehaviour
	{
		#region private Serialized Variables

		[SerializeField]
		private EventReference _event;

		#endregion

		#region Public Methods

		public void PlayEvent()
		{
			RuntimeManager.PlayOneShot(_event,transform.position);
		}
		#endregion

	}
}