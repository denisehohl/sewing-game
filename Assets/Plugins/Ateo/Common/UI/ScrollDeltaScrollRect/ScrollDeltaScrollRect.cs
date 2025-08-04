using System;
using Ateo.Common.Inputs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ateo.Common.UI
{
	namespace UnityEngine.UI.Extensions
	{
		public class ScrollDeltaScrollRect : ScrollRect, IPointerEnterHandler, IPointerExitHandler
		{
			[SerializeField]
			private float _scrollSpeedMouse = 1f;

			[SerializeField]
			private float _scrollSpeedTrackpad = 0.25f;

			private readonly PointerEventData _eventData = new PointerEventData(null);

			public event Action OnEnter;
			public event Action OnExit;
			public event Action OnScrollStart;
			public event Action OnScrollEnd;

			public bool IsMouseHovering { get; private set; }
			public bool IsScrolling { get; private set; }
			public bool IsDragging { get; private set; }

			private Bounds ViewBounds => new Bounds(viewRect.rect.center, viewRect.rect.size);

			protected override void OnDisable()
			{
				base.OnDisable();
				IsMouseHovering = IsScrolling = IsDragging = false;
			}

			private void Update()
			{
				if (IsMouseHovering && !IsDragging)
				{
					if (ScrollDelta.ScrollStartedThisFrame)
					{
						if (!IsScrolling)
						{
							IsScrolling = true;
							inertia = false;
							OnScrollStart?.Invoke();
						}
					}

					if (IsScrolling)
					{
						float delta = ScrollDelta.Delta * (ScrollDelta.Source == ScrollSource.Mouse ? _scrollSpeedMouse : _scrollSpeedTrackpad);
						_eventData.scrollDelta = new Vector2(0f, delta);
						
						base.OnScroll(_eventData);
						return;
					}
				}

				EndScroll();
			}

			private void EndScroll()
			{
				if (!IsScrolling) return;

				if (!IsDragging)
				{
					_eventData.scrollDelta = new Vector2(0f, ScrollDelta.Dampen(_eventData.scrollDelta.y));

					base.OnScroll(_eventData);

					if (!(Mathf.Abs(_eventData.scrollDelta.y) < 0.025f)) return;
				}

				IsScrolling = false;
				OnScrollEnd?.Invoke();
			}

			public override void OnScroll(PointerEventData eventData)
			{
			}

			public void OnPointerEnter(PointerEventData eventData)
			{
				IsMouseHovering = true;
				OnEnter?.Invoke();
			}

			public void OnPointerExit(PointerEventData eventData)
			{
				IsMouseHovering = false;
				OnExit?.Invoke();
			}

			public override void OnBeginDrag(PointerEventData eventData)
			{
				IsDragging = true;
				IsScrolling = false;
				inertia = true;
				base.OnBeginDrag(eventData);
			}

			public override void OnEndDrag(PointerEventData eventData)
			{
				IsDragging = false;
				base.OnEndDrag(eventData);
			}
		}
	}
}