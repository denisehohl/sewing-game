#if TIMELINE
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Ateo.StateManagement.Playables
{
	[TrackColor(0.18f, 0.75f, 0.4f), TrackClipType(typeof(StateManagerClip))]
	public class StateManagerTrack : TrackAsset
	{
		public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
		{
			return ScriptPlayable<StateManagerMixerBehaviour>.Create(graph, inputCount);
		}
	}
}
#endif