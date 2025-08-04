// Source: Tobias Pott, https://assetstore.unity.com/packages/tools/gui/uitooltip-64893

using System;
using Ateo.Animation;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ateo.Common.UI
{
	public abstract class Tooltip<T> : MonoBehaviour, ITooltip<T>
	{
		#region Fields
		
		[BoxGroup("Settings"), SerializeField, MinValue(0f)]
		private float _delay = 1f;

		[BoxGroup("Settings"), SerializeField]
		protected TooltipPositionUpdateBehaviour _updateBehaviour = TooltipPositionUpdateBehaviour.Always;
		
		[BoxGroup("Settings"), SerializeField, ShowIf("@this._updateBehaviour != TooltipPositionUpdateBehaviour.Never")]
		protected int _cursorPositionOffsetX;
		
		[BoxGroup("Settings"), SerializeField, ShowIf("@this._updateBehaviour != TooltipPositionUpdateBehaviour.Never")]
		protected int _cursorPositionOffsetY;

		[BoxGroup("References"), SerializeField, Required]
		protected AnimationBehaviour _animationShow;

		[BoxGroup("References"), SerializeField, Required]
		protected AnimationBehaviour _animationHide;

		protected Vector2 _initialPivot = Vector2.one * 0.5f;
		protected RectTransform _rectTransform;
		protected PointerEventData _eventData;
		protected Vector2Int _overflowWeights;

		#endregion
		
		#region Events

		public event Action OnShown;
		public event Action OnHidden;

		#endregion

		#region Properties

		public bool IsEnabled { get; protected set; } = true;
		public bool IsActive { get; protected set; }
		public bool IsVisible { get; protected set; }
		public float Delay
		{
			get => _delay;
			set => _delay = value;
		}

		public TooltipPositionUpdateBehaviour UpdateBehaviour
		{
			get => _updateBehaviour;
			set => _updateBehaviour = value;
		}

		#endregion

		#region MonoBehaviour Callbacks

		protected virtual void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
			_initialPivot = _rectTransform.pivot;
		}

		protected virtual void Start()
		{
			if (!IsActive)
			{
				_animationHide.ExecuteAnimationImmediate();
			}
		}

		protected virtual void Update()
		{
			if (!IsEnabled) return;
			
			if (_updateBehaviour == TooltipPositionUpdateBehaviour.Always)
			{
				UpdatePosition();
			}
		}
		
		#endregion

		#region Public Methods

		/// <summary>
		/// Enables this Tooltip instance
		/// </summary>
		public virtual void Enable()
		{
			IsEnabled = true;
		}

		/// <summary>
		/// Disables this Tooltip instance
		/// </summary>
		public virtual void Disable()
		{
			if(!IsEnabled) return;
			
			if (IsActive)
			{
				Hide();
			}

			IsEnabled = false;
		}

		/// <summary>
		/// Shows the tooltip
		/// </summary>
		/// <param name="eventData">ui event data to process correct positioning</param>
		public virtual void Show(PointerEventData eventData)
		{
			if (!IsEnabled || IsActive || eventData == null) return;

			IsActive = true;
			IsVisible = true;

			// if tooltip should update it's position, save required data to calculate position during Update
			if (_updateBehaviour == TooltipPositionUpdateBehaviour.Always || _updateBehaviour == TooltipPositionUpdateBehaviour.WhenShown)
			{
				_eventData = eventData;
				UpdatePosition();
			}

			RectTransform pointerEnterRect = null;

			if (eventData.pointerEnter != null)
			{
				pointerEnterRect = eventData.pointerEnter.GetComponent<RectTransform>();
			}

			// begin fade in the tooltip
			_animationHide.Abort();
			InvokeOnShown();

			if (_updateBehaviour != TooltipPositionUpdateBehaviour.Never
			    && pointerEnterRect != null
			    && RectTransformUtility.ScreenPointToLocalPointInRectangle(pointerEnterRect, eventData.position, eventData.enterEventCamera,
				    out Vector2 localPoint))
			{
				// reset pivot to lower-left corner & force rebuild layout
				_rectTransform.pivot = _initialPivot;
				LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
				
				// Auto-correct pivot depending on overflowing corners
				{
					Vector3[] objectCorners = new Vector3[4];
					_rectTransform.GetWorldCorners(objectCorners);
					
					_overflowWeights = DetermineOverflowWeight(new Rect(0, 0, Screen.width, Screen.height), objectCorners);

					Vector2 pivot = Vector2.up;
					
					if (!_overflowWeights.Equals(Vector2Int.zero))
					{
						pivot.x = _overflowWeights.x > 0 ? 1.0f : 0.0f;
						pivot.y = _overflowWeights.y < 0 ? 0.0f : 1.0f;
					}

					// set back the pivot determined by the overflow corners
					_rectTransform.pivot = pivot;
				}
			}

			_animationShow.Execute(OnShowComplete);
		}

		/// <summary>
		/// Hides the tooltip
		/// </summary>
		public virtual void Hide()
		{
			if (!IsActive) return;

			IsActive = false;

			_animationShow.Abort();
			_animationHide.Execute(OnHideComplete);
		}
		
		public abstract void Style(T data);
		
		#endregion
		
		#region Event Callbacks

		protected virtual void OnShowComplete()
		{
			InvokeOnShown();
		}

		protected virtual void OnHideComplete()
		{
			if (_updateBehaviour != TooltipPositionUpdateBehaviour.Never)
			{
				_rectTransform.pivot = _initialPivot;
			}

			IsVisible = false;
			InvokeOnHidden();
		}
		
		#endregion

		#region Protected Methods

		protected virtual void UpdatePosition()
		{
			if (_eventData == null || _eventData.pointerEnter == null || !IsVisible) return;

			RectTransform rectTransform = _eventData.pointerEnter.GetComponent<RectTransform>();
			Vector2 mousePosition = Input.mousePosition;
			Camera cam = _eventData.enterEventCamera;

			Vector3 lossyScale = rectTransform.transform.lossyScale;
			Vector2 multiplier = Vector2.one;
			
			if (!_overflowWeights.Equals(Vector2Int.zero))
			{
				multiplier.x = _overflowWeights.x < 0 ? 1 : -1;
				multiplier.y = _overflowWeights.y > 0 ? -1 : 1;
			}

			if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, mousePosition, cam, out Vector3 worldPoint)) return;

			worldPoint += new Vector3(
				_cursorPositionOffsetX * multiplier.x * lossyScale.x, 
				_cursorPositionOffsetY * multiplier.y * lossyScale.y, 
				0);
			
			_rectTransform.position = worldPoint;
		}

		protected static Vector2Int DetermineOverflowWeight(Rect clipRect, Vector3[] corners)
		{
			Vector2Int result = Vector2Int.zero;
			
			// 0: Bottom Left
			if (!clipRect.Contains(corners[0]))
			{
				result.x--;
				result.y--;
			}

			// 1: Top Left
			if (!clipRect.Contains(corners[1]))
			{
				result.x--;
				result.y++;
			}

			// 2: Top Right
			if (!clipRect.Contains(corners[2]))
			{
				result.x++;
				result.y++;
			}

			// 3: Bottom Right
			if (!clipRect.Contains(corners[3]))
			{
				result.x++;
				result.y--;
			}

			return result;
		}

		protected void InvokeOnShown() => OnShown?.Invoke();
		protected void InvokeOnHidden() => OnHidden?.Invoke();

		#endregion
	}
}