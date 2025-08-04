#if TIMELINE
using Sirenix.Utilities;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace Ateo.StateManagement.Playables.Editor
{
	[CustomTimelineEditor(typeof(StateManagerClip))]
	public class StateManagerClipEditor : ClipEditor
	{
		public override void OnClipChanged(TimelineClip clip)
		{
			StateManagerClip stateManagerClip = (StateManagerClip)clip.asset;

			if (stateManagerClip == null)
			{
				return;
			}

			clip.displayName = stateManagerClip.Template.State.ToString().SplitPascalCase();
		}
	}
}
#endif