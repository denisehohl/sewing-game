#if TIMELINE
using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Ateo.StateManagement.Playables
{
	[Serializable]
	public class StateManagerClip : PlayableAsset, ITimelineClipAsset
	{
		[InlineProperty, HideLabel]
		public StateManagerBehaviour Template = new StateManagerBehaviour();

		public ClipCaps clipCaps => ClipCaps.None;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			ScriptPlayable<StateManagerBehaviour> playable = ScriptPlayable<StateManagerBehaviour>.Create(graph, Template);
			return playable;
		}
	}
}
#endif