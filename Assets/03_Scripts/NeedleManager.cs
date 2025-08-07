using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace Moreno.SewingGame
{
	public class NeedleManager : MonoBehaviour
	{
		#region private Serialized Variables
		
		[SerializeField, Required]
		[BoxGroup("Needle")]
		private GameObject _intactNeedle;
		[SerializeField, Required]
		[BoxGroup("Needle")]
		private Interactable _brokenNeedle;

		[SerializeField, Required]
		private List<GameObject> _objectsToDisableWhenAnimating;

		[SerializeField, Required]
		private CinemachineCamera _closeupCam;
		
		[SerializeField]
		private float _animationDistanceOffset = 0.1f;
		[SerializeField]
		private float _animationJumpPower = -0.1f;
		[SerializeField]
		private float _animationTime = 1f;

		#endregion

		#region private Variables

		private Vector3 _targetLocalPos;

		#endregion

		#region Properties

		#endregion

		#region Delegates & Events

		#endregion

		#region Monobehaviour Callbacks

		private void Start()
		{
			StoreStartLocalPosition();
			_closeupCam.gameObject.SetActive(false);
		}

		#endregion

		#region Public Methods
		
		public void SetNeedleBrokenVisual(bool isBroken)
		{
			_brokenNeedle.transform.DOKill();
			_intactNeedle.transform.DOKill();
			SetObjectsInactiveWhileAnimation(false);
			_brokenNeedle.gameObject.SetActive(isBroken);
			_brokenNeedle.transform.localPosition = _targetLocalPos;
			_intactNeedle.transform.localPosition = _targetLocalPos;
			_intactNeedle.SetActive(!isBroken);
		}

		public IEnumerator StartReplaceTask(Action callback = null)
		{
			bool needleReplaced = !_brokenNeedle.gameObject.activeSelf;
			_brokenNeedle.OnClicked += ()=> needleReplaced = true;
			_closeupCam.gameObject.SetActive(true);

			while (!needleReplaced)
			{
				yield return null;
			}
			
			_brokenNeedle.ClearListeners();
			SwapNewNeedleIn();

			yield return StartThreadTask();
			
			_closeupCam.gameObject.SetActive(false);
			callback?.Invoke();
		}

		#endregion

		#region Private Methods

		private IEnumerator StartThreadTask()
		{
			yield return new WaitForSeconds(2f);
		}

		private void SwapNewNeedleIn()
		{
			float offsetStrength = _animationDistanceOffset;
			_brokenNeedle.gameObject.SetActive(true);
			_intactNeedle.gameObject.SetActive(true);
			SetObjectsInactiveWhileAnimation(true);
			_brokenNeedle.transform.DOLocalJump( _targetLocalPos + Vector3.left * offsetStrength, _animationJumpPower, 1, _animationTime)
				.OnComplete(()=>_brokenNeedle.gameObject.SetActive(false));
			_intactNeedle.transform.localPosition = _targetLocalPos + Vector3.right * offsetStrength;
			_intactNeedle.transform.DOLocalJump(_targetLocalPos, _animationJumpPower, 1, _animationTime)
				.SetDelay(_animationTime * 0.5f)
				.OnComplete(()=> SetObjectsInactiveWhileAnimation(false));
		}

		private void StoreStartLocalPosition()
		{
			_targetLocalPos = _intactNeedle.transform.localPosition;
		}

		private void SetObjectsInactiveWhileAnimation(bool isAnimating)
		{
			foreach (GameObject obj in _objectsToDisableWhenAnimating)
			{
				obj.SetActive(!isAnimating);
			}
		}

		#endregion

		#region Event Callbacks

		#endregion

		
	}
}