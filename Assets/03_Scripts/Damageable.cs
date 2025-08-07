using System;
using UnityEngine;

namespace Moreno.SewingGame
{
	public class Damageable : MonoBehaviour, IDamageable
	{
		#region Delegates & Events

		public event Action<Hurtable> OnDamageTaken;

		#endregion
		
		#region Event Callbacks
		public void TakeDamage(Hurtable hurtable)
		{
			if(!enabled) return;
			OnDamageTaken?.Invoke(hurtable);
		}
		
		#endregion


	}
}