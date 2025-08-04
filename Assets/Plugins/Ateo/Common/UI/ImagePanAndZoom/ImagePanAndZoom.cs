using Ateo.Common;
using Ateo.Common.Inputs;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ateo.UI
{
	public class ImagePanAndZoom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField]
		private RectTransform _movingTransform;

		[SerializeField]
		private float _scrollSpeedMouse = 1f;

		[SerializeField]
		private float _scrollSpeedTrackpad = 0.25f;

		[SerializeField]
		private float _minScale, _maxScale;

		[SerializeField]
		private Ease _ease;

		private readonly Vector2 _resetVector = Vector2.one * 0.5f;

		private float _scale;
		private Vector2 _cursorPosition;
		private Vector2 _localPivotPoint;
		private bool _isMouseHovering;

		private void OnDisable()
		{
			_isMouseHovering = false;
		}

		private void Update()
		{
			if (_isMouseHovering)
			{
				if (ScrollDelta.ScrollStartedThisFrame)
				{
					_cursorPosition = CursorPosition.Position;
				}

				float delta = ScrollDelta.Delta * (ScrollDelta.Source == ScrollSource.Mouse ? _scrollSpeedMouse : _scrollSpeedTrackpad);
				
				// Vector3 localScale = _movingTransform.localScale;
				_scale += delta;
				_scale = Mathf.Clamp(_scale, 0, 1);

				if (_scale <= 0)
				{
					_movingTransform.DOAnchorPos(_resetVector, 0.1f);
					_movingTransform.localScale = Vector3.one * _minScale;
					return;
				}
				
				float eased = DOVirtual.EasedValue(_minScale, _maxScale, _scale, _ease);
				Vector2 localPivotPoint = _movingTransform.InverseTransformPoint(_cursorPosition);
				Scale(_movingTransform, localPivotPoint, Vector3.one * eased);
			}
		}
		
		public void ResetPanAndZoom()
		{
			_movingTransform.localScale = Vector3.one;
			_movingTransform.anchoredPosition = _resetVector;
			_scale = 0f;
		}

		private static void Scale(RectTransform target, Vector2 pivot, Vector3 newScale)
		{
			Vector2 startLocalPosition = target.anchoredPosition;

			Vector2 offset = startLocalPosition - pivot; // diff from object pivot to desired pivot/origin

			float relativeScaleFactor = newScale.x / target.localScale.x;

			// calc final position post-scale
			Vector2 finalPosition = pivot + offset * relativeScaleFactor;

			// finally, actually perform the scale/translation
			target.localScale = newScale;
			target.anchoredPosition = finalPosition;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_isMouseHovering = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_isMouseHovering = false;
		}
	}
}