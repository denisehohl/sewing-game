using UnityEngine;

namespace Moreno.SewingGame
{
	[CreateAssetMenu(fileName = "Settings Data", menuName = "Create SettingsData", order = 0)]
	public class SettingsData : ScriptableObject
	{
		[SerializeField]
		private Vector2 _pinDamageRange = new Vector2(0.2f, 4f);
		[SerializeField]
		private Vector2 _pinDamageEnteredRange = new Vector2(0.2f, 2f);

		[SerializeField]
		private float _pinMagnitudeToRemove = 1f;

		public Vector2 PinDamageRange => _pinDamageRange;

		public Vector2 PinDamageEnteredRange => _pinDamageEnteredRange;

		public float PinMagnitudeToRemove => _pinMagnitudeToRemove;
	}
}