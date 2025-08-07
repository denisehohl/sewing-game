using System;
using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Moreno.SewingGame.Audio
{
	public class MusicController : MonoBehaviour
	{
		#region private Serialized Variables

		[SerializeField, Required]
		private Interactable _playButton;
		[SerializeField, Required]
		private Interactable _skipButton;

		[SerializeField]
		private EventReference _radioEvent;
		[SerializeField]
		private EventReference _dynamicMusic;
		[SerializeField]
		private EventReference _radioStatic;

		[SerializeField, Required]
		private StudioBankLoader _bankLoader;

		#endregion

		#region private Variables

		private EventInstance _currentMusicInstance;
		private PARAMETER_ID _musicIntensityId;
		private bool _isDynamic = false;
		private bool _radioBanksLoaded = false;

		#endregion

		#region Properties

		#endregion

		#region Delegates & Events

		#endregion

		#region Monobehaviour Callbacks

		private void Start()
		{
			StartDynamicMusic();
		}

		private void OnEnable()
		{
			_playButton.OnClicked += ToggleDynamic;
			_skipButton.OnClicked += StartRadio;
		}

		private void OnDisable()
		{
			_playButton.OnClicked -= ToggleDynamic;
			_skipButton.OnClicked -= StartRadio;
		}

		#endregion

		#region Public Methods

		public void SetMusicIntensity(float value)
		{
			if (!_isDynamic) return;
			_currentMusicInstance.setParameterByID(_musicIntensityId, value);
		}
		
		#endregion

		#region Private Methods

		private void StartDynamicMusic(bool withStatic = false)
		{
			ReleaseCurrentInstance();
			_currentMusicInstance = RuntimeManager.CreateInstance(_dynamicMusic);
			_musicIntensityId = _currentMusicInstance.GetParameterId("Intensity");
			_isDynamic = true;
			
			PlayInstance(withStatic);
		}

		private void StartRadio()
		{
			if (!_radioBanksLoaded)
			{
				_bankLoader.Load();
				_radioBanksLoaded = true;
			}
			ReleaseCurrentInstance();
			_currentMusicInstance = RuntimeManager.CreateInstance(_radioEvent);
			PlayInstance();
		}

		private void ReleaseCurrentInstance()
		{
			if (_currentMusicInstance.isValid())
			{
				_currentMusicInstance.stop(STOP_MODE.ALLOWFADEOUT);
				_currentMusicInstance.release();
			}
			_isDynamic = false;
		}

		private void PlayInstance(bool withStatic = false)
		{
			if (withStatic)
			{
				RuntimeManager.PlayOneShot(_radioStatic);
			}
			_currentMusicInstance.StartAttached(transform);
		}


		#endregion

		#region Event Callbacks
		
		private void ToggleDynamic()
		{
			if (_isDynamic)
			{
				StartRadio();
			}
			else
			{
				StartDynamicMusic(true);
			}
		}

		#endregion


	}
}