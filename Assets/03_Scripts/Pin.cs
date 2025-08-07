using System;
using System.Collections;
using Ateo.Common;
using FMODUnity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Moreno.SewingGame
{
	public class Pin : MonoBehaviour
	{
		#region private Serialized Variables

		[SerializeField]
		private GameObject _positiveCollider;
		[SerializeField]
		private GameObject _negativeCollider;
		[SerializeField]
		private Rigidbody _rigidbody;

		[SerializeField]
		private Damageable _damageable;

		[SerializeField]
		private EventReference _extractSound;

		#endregion

		#region private Variables

		private Coroutine _routine;
		private Vector3 _dragTotal;

		#endregion

		#region Properties

		#endregion

		#region Delegates & Events

		#endregion

		#region Monobehaviour Callbacks

		private void OnEnable()
		{
			MouseWorldPointer.OnObjectClicked += OnObjectClicked;
			MouseWorldPointer.OnInteractableEntered += OnInteractableEntered;
			_damageable.OnDamageTaken += OnDamageTaken;
			_rigidbody.isKinematic = true;
		}

		private void OnDisable()
		{
			MouseWorldPointer.OnObjectClicked -= OnObjectClicked;
			MouseWorldPointer.OnInteractableEntered -= OnInteractableEntered;
			_damageable.OnDamageTaken -= OnDamageTaken;
			if (_routine != null)
			{
				StopCoroutine(_routine);
				_routine = null;
			}
		}

		private void OnDamageTaken(Hurtable obj)
		{
			var rand = Random.value;
			if (rand > MainManager.Instance.CurrentSettings.PinSurvivalChance)
			{
				SewingMachineController.Instance.BreakMachine();
			}
		}

		#endregion

		#region Public Methods

		#endregion

		#region Private Methods
		private void OnObjectClicked(GameObject obj)
		{
			if (obj == _positiveCollider)
			{
				if (_routine != null)
				{
					StopCoroutine(_routine);
					_routine = null;
				}
				//pinhead clicked
				_damageable.enabled = false;
				_routine = StartCoroutine(CheckIfDraggedInDirection());
			}

			if (obj == _negativeCollider)
			{
				CheckIfHurt(MainManager.Instance.CurrentSettings.PinDamageRange);
			}
		}

		private void CheckIfHurt(Vector2 damageRange)
		{
			var delta = MouseWorldPointer.Instance.DeltaPosition;
			var dot = Vector3.Dot(delta.normalized, _negativeCollider.transform.right);
			var damage = Mathf.Lerp(damageRange.x, damageRange.y, dot);
				
			DamageManager.Instance.CauseDamage(MouseWorldPointer.Instance.CurrentPosition,damage, dot);
		}

		private IEnumerator CheckIfDraggedInDirection()
		{
			Vector3 dragTotal = MouseWorldPointer.Instance.DeltaPosition;
			while (dragTotal.sqrMagnitude < MainManager.Instance.CurrentSettings.PinMagnitudeToRemove)
			{
				dragTotal += MouseWorldPointer.Instance.DeltaPosition;
				_dragTotal = dragTotal;
				yield return null;
			}

			var dot = Vector3.Dot(dragTotal, _negativeCollider.transform.right);
			if (dot > 0)
			{
				_rigidbody.isKinematic = false;
				_rigidbody.AddForce((_negativeCollider.transform.right+ Vector3.up * 0.2f) * 500f);
				RuntimeManager.PlayOneShot(_extractSound,transform.position);
				yield return new WaitForSeconds(5f);
				gameObject.SetActive(false);
			}
		}

		#endregion

		#region Event Callbacks
		
		private void OnInteractableEntered(GameObject obj)
		{
			if(obj != _negativeCollider) return;
			if(!_rigidbody.isKinematic) return;
			CheckIfHurt(MainManager.Instance.CurrentSettings.PinDamageEnteredRange);
		}

		#endregion
	}
}