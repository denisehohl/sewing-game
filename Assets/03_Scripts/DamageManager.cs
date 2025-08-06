using Ateo.Common;
using UnityEngine;

namespace Moreno.SewingGame
{
	public class DamageManager : ComponentPublishBehaviour<DamageManager>
	{
		#region private Serialized Variables

		#endregion

		#region private Variables

		#endregion

		#region Properties

		#endregion

		#region Delegates & Events

		#endregion

		#region Monobehaviour Callbacks

		#endregion

		#region Public Methods

		public void CauseDamage(Vector3 position,float damage, float intensity)
		{
			DebugExtension.DrawMarker(position,intensity,Color.red,depthTest: false);
			Debug.Log($"OUCH | {position}, {damage}, {intensity}");
		}

		#endregion

		#region Private Methods

		#endregion

		#region Event Callbacks

		#endregion

		
	}
}