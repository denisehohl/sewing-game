using Ateo.Common;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Moreno.SewingGame
{
	public class DamageManager : ComponentPublishBehaviour<DamageManager>
	{
		#region private Serialized Variables

		[SerializeField]
		private EventReference _ouchSound;

		#endregion

		#region private Variables

		private float _lastTimeDamageTaken;

		#endregion

		#region Properties

		public float LastTimeDamageTaken => _lastTimeDamageTaken;

		#endregion

		#region Delegates & Events

		#endregion

		#region Monobehaviour Callbacks

		#endregion

		#region Public Methods

		public void CauseDamage(Vector3 position,float damage, float intensity)
		{
			if(intensity < 0) return;
			_lastTimeDamageTaken = Time.time;
			DebugExtension.DrawMarker(position,intensity,Color.red,depthTest: false);
			Debug.Log($"OUCH | {position}, {damage}, {intensity}");
			PlaySound(intensity);
		}

		#endregion

		#region Private Methods

		private void PlaySound(float intensity)
		{
			EventInstance eventInstance = RuntimeManager.CreateInstance(_ouchSound);
			eventInstance.setParameterByName("Intensity", intensity);

			// Start and release
			eventInstance.start();
			eventInstance.release(); // Ensures it will be cleaned up after playing
		}

		#endregion

		#region Event Callbacks

		#endregion

		
	}
}