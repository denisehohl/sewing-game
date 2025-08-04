// Source: https://forum.unity.com/threads/audio-mixer-and-asset-bundle.338077/#post-4410481

#if UNITY_EDITOR && ADDRESSABLES

using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

namespace Ateo.Common.AddressableAssets
{
	[CustomPropertyDrawer(typeof(AudioMixerGroupWrapper))]
	public class AudioMixerGroupWrapperPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			property.NextVisible(true);
			Assert.AreEqual("groupID", property.name);
			int value = property.intValue;

			AudioMixerGroupBinding binding = AudioMixerGroupBinding.Instance;
			AudioMixerGroup oldGroup = binding.ResolveMixerGroupID(value);
			AudioMixerGroup newGroup = (AudioMixerGroup) EditorGUI.ObjectField(position, "Output", oldGroup, typeof(AudioMixerGroup), false);

			if (newGroup != oldGroup)
			{
				property.intValue = binding.GetOrCreateMixerGroupID(newGroup);
			}
		}
	}
}

#endif