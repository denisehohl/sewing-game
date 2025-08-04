// Source: https://forum.unity.com/threads/audio-mixer-and-asset-bundle.338077/#post-4410481

#if ADDRESSABLES
using System;
using UnityEngine.Audio;

namespace Ateo.Common.AddressableAssets
{
	[Serializable]
	public struct AudioMixerGroupWrapper
	{
		public int groupID;

		public AudioMixerGroup Group { get; private set; }

		/// This must be called in Awake().
		public void Resolve()
		{
			Group = AudioMixerGroupBinding.Instance.ResolveMixerGroupID(groupID);
		}

		public static implicit operator AudioMixerGroup(AudioMixerGroupWrapper mixer)
		{
			return mixer.Group;
		}
	}
}
#endif