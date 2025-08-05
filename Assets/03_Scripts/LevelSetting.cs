using UnityEngine;

namespace Moreno.SewingGame
{
	public class LevelSetting : ScriptableObject
	{
		[SerializeField]
		private float _machineDirectionRange;
		[SerializeField]
		private float _machineDirectionChangeRange;
		[SerializeField]
		private float _pinSpawnRate = 0;
		[SerializeField]
		private bool _canPinsFlip = false;
		[SerializeField]
		private float _pinRandomRotationRange;

		public float MachineDirectionRange => _machineDirectionRange;

		public float MachineDirectionChangeRange => _machineDirectionChangeRange;

		public float PinSpawnRate => _pinSpawnRate;

		public bool CanPinsFlip => _canPinsFlip;

		public float PinRandomRotationRange => _pinRandomRotationRange;
	}
}