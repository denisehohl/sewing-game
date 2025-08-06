using System;
using Ateo.Extensions;
using UnityEngine;

namespace Moreno.SewingGame
{
	public class Hurtable : MonoBehaviour
	{
		#region private Serialized Variables

		[SerializeField]
		private LayerMask _hurtableLayers;

		[SerializeField]
		private float _damageValue;
		[SerializeField]
		private float _intensityValue;
		[SerializeField]
		private float _damageCooldown = 0.3f;

		#endregion

		#region private Variables

		private float _lastTimeDamageTaken;

		#endregion

		#region Properties

		#endregion

		#region Delegates & Events

		#endregion

		#region Monobehaviour Callbacks

		private void OnTriggerEnter(Collider other)
		{
			if(_hurtableLayers.Contains(other.gameObject.layer))
			{
				if(other.TryGetComponent(out IDamageable damageable))
				{
					damageable.TakeDamage(this);
				}
			}
		}

		#endregion

		#region Public Methods

		public void CheckIfMouseIsOverHurtable()
		{
			if (MouseWorldPointer.Instance.CurrentInteractionObject == gameObject)
			{
				if(DamageManager.Instance.LastTimeDamageTaken + _damageCooldown >= Time.time) return;
				DamageManager.Instance.CauseDamage(transform.position, _damageValue, _intensityValue);
			}
		}

		#endregion

		#region Private Methods

		#endregion

		#region Event Callbacks

		#endregion

		
	}
}