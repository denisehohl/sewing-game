using System;
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
		private float _totalDamageTaken = 0;

		#endregion

		#region Properties

		public float LastTimeDamageTaken => _lastTimeDamageTaken;
		public float TotalDamageTaken => _totalDamageTaken;

		#endregion

		#region Delegates & Events

		public static event Action<float,float> OnDamageTaken;

		#endregion

		#region Monobehaviour Callbacks

		public override void ResetStatics()
		{
			base.ResetStatics();
			OnDamageTaken = null;
		}

		#endregion

		#region Public Methods

		public void ResetValues()
		{
			_totalDamageTaken = 0;
		}

		public void CauseDamage(Vector3 position,float damage, float intensity)
		{
			if(intensity < 0) return;
			_lastTimeDamageTaken = Time.time;
			_totalDamageTaken += damage;
			DebugExtension.DrawMarker(position,intensity,Color.red,depthTest: false);
			Debug.Log($"OUCH | {position}, {damage}, {intensity}");
			PlaySound(intensity);
			OnDamageTaken?.Invoke(damage, intensity);
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