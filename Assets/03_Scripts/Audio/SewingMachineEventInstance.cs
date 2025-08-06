using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Moreno.SewingGame.Audio
{
	public class SewingMachineEventInstance : MonoBehaviour
	{
		#region private Serialized Variables

		[SerializeField]
		private EventReference _eventReference;

		#endregion

		#region private Variables

		private EventInstance _eventInstance;
		
		private PARAMETER_ID _speedParameter;
		private PARAMETER_ID _activeParameter;

		private float _speedValue;

		#endregion
		
		#region Monobehaviour Callbacks

		private void Start()
		{
			Init();
		}

		private void OnDestroy()
		{
			Release();
		}

		#endregion

		#region Public Methods

		public void StartSound()
		{
			_eventInstance.setParameterByID(_activeParameter, 1);
		}

		public void StopSound()
		{
			_eventInstance.setParameterByID(_activeParameter, 0);
		}

		public void SetSpeed(float value)
		{
			_speedValue = Mathf.Abs(value);
			_eventInstance.setParameterByID(_speedParameter, _speedValue);
		}

		#endregion

		#region Private Methods

		private void Init()
		{
			_eventInstance = RuntimeManager.CreateInstance(_eventReference);
			_speedParameter = CacheParameterId(_eventInstance,"Speed");
			_activeParameter = CacheParameterId(_eventInstance,"State");

			_eventInstance.StartAttached(transform);
			StopSound();
		}

		private void Release()
		{
			_eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
			_eventInstance.release();
		}

		private PARAMETER_ID CacheParameterId(EventInstance instance, string parameterName)
		{
			instance.getDescription(out EventDescription pitchEventDescription);
			pitchEventDescription.getParameterDescriptionByName(parameterName, out PARAMETER_DESCRIPTION pitchParameterDescription);
			return pitchParameterDescription.id;
		}

		#endregion
	}
}