// Source: https://forum.unity.com/threads/audio-mixer-and-asset-bundle.338077/#post-4410481

#if ADDRESSABLES
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Ateo.Common.AddressableAssets
{
	[CreateAssetMenu(fileName = "AudioMixerGroupBinding", menuName = "Addressables/Ateo/AudioMixerGroupBinding")]
	public class AudioMixerGroupBinding : ScriptableObject
	{
		private static AudioMixerGroupBinding _instance;

		public static AudioMixerGroupBinding Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Resources.Load<AudioMixerGroupBinding>("AudioMixerGroupBinding");
				}

				return _instance;
			}
		}

		public AudioMixerGroup[] GroupReferences;

		public AudioMixerGroup ResolveMixerGroupID(int id)
		{
			return (uint) (id - 1) >= (uint) GroupReferences.Length ? null : GroupReferences[id - 1];
		}

#if UNITY_EDITOR
		public int GetOrCreateMixerGroupID(AudioMixerGroup group)
		{
			if (group == null) return 0;

			for (int index = 0; index < GroupReferences.Length; index++)
			{
				if (GroupReferences[index] == group)
				{
					return index + 1;
				}
			}

			Array.Resize(ref GroupReferences, GroupReferences.Length + 1);
			GroupReferences[GroupReferences.Length - 1] = group;
			EditorUtility.SetDirty(this);
			return GroupReferences.Length;
		}
#endif

		#region Reset Statics

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics()
		{
			_instance = null;
		}

		#endregion
	}
}
#endif