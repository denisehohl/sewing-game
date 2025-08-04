#if ADDRESSABLES
using UnityEngine;

namespace Ateo.Common.AddressableAssets
{
	[RequireComponent(typeof(AudioSource))]
	public class SetAudioSourceMixerGroup : MonoBehaviour
	{
		[SerializeField]
		private AudioMixerGroupWrapper _mixerGroup;

		private void Start()
		{
			if (!TryGetComponent(out AudioSource audioSource)) return;

			_mixerGroup.Resolve();
			audioSource.outputAudioMixerGroup = _mixerGroup.Group;

			DebugDev.Log($"{gameObject.name} - Set AudioMixerGroup {_mixerGroup.Group.name} - Mixer ID = {_mixerGroup.Group.audioMixer.GetInstanceID()}");
		}
	}
}
#endif