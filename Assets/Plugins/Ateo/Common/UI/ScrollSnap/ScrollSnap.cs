using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ateo.Common;
using Ateo.Extensions;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ateo.UI
{
	#region Enumerators

	public enum MovementType
	{
		Fixed,
		Free
	}

	public enum MovementAxis
	{
		Horizontal,
		Vertical
	}

	public enum Direction
	{
		Up,
		Down,
		Left,
		Right
	}

	public enum SnapTarget
	{
		Nearest,
		Previous,
		Next
	}

	public enum SizeControl
	{
		Manual,
		Fit
	}

	#endregion

	[RequireComponent(typeof(RectTransform)), AddComponentMenu("UI/Ateo Scroll-Snap"), RequireComponent(typeof(ScrollRect))]
	public class ScrollSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
	{
		#region Fields

		[FoldoutGroup("Movement and Layout Settings")]
		public MovementType MovementType = MovementType.Fixed;

		[FoldoutGroup("Movement and Layout Settings")]
		public MovementAxis MovementAxis = MovementAxis.Horizontal;

		[FoldoutGroup("Movement and Layout Settings")]
		public bool AutomaticallyLayout = true;

		[FoldoutGroup("Movement and Layout Settings")]
		public SizeControl SizeControl = SizeControl.Fit;

		[FoldoutGroup("Movement and Layout Settings")]
		public Vector2 Padding;

		/*[FoldoutGroup("Movement and Layout Settings")]
		public bool infinitelyScroll;

		[FoldoutGroup("Movement and Layout Settings")]
		public float infiniteScrollingEndSpacing;*/

		[FoldoutGroup("Movement and Layout Settings")]
		public bool UseOcclusionCulling;

		[FoldoutGroup("Movement and Layout Settings"), PropertyRange(0, "$MaxLayoutSpacing")]
		public int StartingPanel;

		[FoldoutGroup("Navigation Settings")]
		public bool SwipeGestures = true;

		[FoldoutGroup("Navigation Settings")]
		public float MinimumSwipeSpeed;

		[FoldoutGroup("Navigation Settings")]
		public Button PreviousButton;

		[FoldoutGroup("Navigation Settings")]
		public Button NextButton;

		[FoldoutGroup("Navigation Settings")]
		public Pagination Pagination;

		[FoldoutGroup("Snap Settings")]
		public SnapTarget SnapTarget = SnapTarget.Next;

		[FoldoutGroup("Snap Settings")]
		public float SnappingSpeed = 10f;

		[FoldoutGroup("Snap Settings")]
		public float ThresholdSnappingSpeed = -1f;

		[FoldoutGroup("Snap Settings")]
		public bool HardSnap = true;

		[FoldoutGroup("Snap Settings")]
		public bool useUnscaledTime;

		[FoldoutGroup("Event Handlers")]
		public UnityEvent OnPanelChanged, OnPanelSelecting, OnPanelSelected, OnPanelChanging;

		protected bool SetupComplete;

		private bool _dragging, _selected = true, _pressing, _setupComplete;
		private float _releaseSpeed;
		private Direction _releaseDirection;
		private Vector2 _previousContentAnchoredPosition, _velocity;

		#endregion

		#region Properties

		public RectTransform RectTransform { get; protected set; }
		public ScrollRect ScrollRect { get; protected set; }
		public RectTransform Content => ScrollRect.content;
		public RectTransform Viewport => ScrollRect.viewport;
		public int NumberOfPanels => Content.childCount;

		public int CurrentPanel { get; protected set; }
		public int PreviousPanel { get; protected set; }
		public int TargetPanel { get; protected set; }
		public int NearestPanel { get; protected set; }

		protected List<RectTransform> PanelsRT { get; set; }
		public List<ScrollPanel> PanelsComp { get; protected set; }
		public List<GameObject> Panels { get; protected set; }

		protected virtual float Width => RectTransform.rect.width;
		protected virtual float Height => RectTransform.rect.height;
		protected virtual Vector2 Size => new Vector2(Width, Height);

		#endregion

#if UNITY_EDITOR

		#region Editor-Methods

		private bool ShowSize()
		{
			return SizeControl != SizeControl.Fit;
		}

		private int MaxLayoutSpacing()
		{
			if (ScrollRect == null)
				ScrollRect = GetComponent<ScrollRect>();

			return Mathf.Clamp(NumberOfPanels - 1, 0, int.MaxValue);
		}

		#endregion

#endif

		#region Methods

		protected virtual void Awake()
		{
			Initialize();
		}

		protected virtual void Start()
		{
			Setup();
		}

		protected virtual void Update()
		{
			if (!SetupComplete || NumberOfPanels == 0) return;

			OnOcclusionCulling();
			OnSelectingAndSnapping();
			/*OnInfiniteScrolling();*/
			OnTransitionEffects();
			OnSwipeGestures();

			DetermineVelocity();
		}

#if UNITY_EDITOR
		protected virtual void OnValidate()
		{
			Initialize();
		}
#endif

		protected virtual void OnRectTransformDimensionsChange()
		{
			if (!_setupComplete)
				return;

			if (Viewport.rect.height > 0f)
			{
				UpdateLayout();
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			_pressing = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_pressing = false;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (HardSnap)
			{
				ScrollRect.inertia = true;
			}

			_selected = false;
			_dragging = true;
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (_dragging)
			{
				OnPanelSelecting.Invoke();
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			_dragging = false;

			if (MovementAxis == MovementAxis.Horizontal)
			{
				_releaseDirection = ScrollRect.velocity.x > 0 ? Direction.Right : Direction.Left;
			}
			else if (MovementAxis == MovementAxis.Vertical)
			{
				_releaseDirection = ScrollRect.velocity.y > 0 ? Direction.Up : Direction.Down;
			}

			_releaseSpeed = ScrollRect.velocity.magnitude;
		}

		protected virtual void Initialize()
		{
			RectTransform = GetComponent<RectTransform>();
			ScrollRect = GetComponent<ScrollRect>();
		}

		private bool Validate()
		{
			bool valid = true;

			if (SnappingSpeed < 0)
			{
				Debug.LogError("<b>[ScrollSnap]</b> Snapping speed cannot be negative.", gameObject);
				valid = false;
			}

			return valid;
		}

		protected virtual async void Setup()
		{
			if (NumberOfPanels == 0) return;

			// ScrollRect
			if (MovementType == MovementType.Fixed)
			{
				ScrollRect.horizontal = MovementAxis == MovementAxis.Horizontal;
				ScrollRect.vertical = MovementAxis == MovementAxis.Vertical;
			}
			else
			{
				ScrollRect.horizontal = ScrollRect.vertical = true;
			}

			Panels = new List<GameObject>(new GameObject[NumberOfPanels]);
			PanelsRT = new List<RectTransform>(new RectTransform[NumberOfPanels]);
			PanelsComp = new List<ScrollPanel>(new ScrollPanel[NumberOfPanels]);

			for (int i = 0; i < NumberOfPanels; i++)
			{
				Panels[i] = Content.GetChild(i).gameObject;
				PanelsRT[i] = Panels[i].GetComponent<RectTransform>();
				PanelsComp[i] = Panels[i].GetComponent<ScrollPanel>();

				if (PanelsComp[i] != null)
				{
					if (!PanelsComp[i].IsInitialized)
						PanelsComp[i].Initialize(this);
				}
			}

			// Previous Button
			if (PreviousButton != null)
			{
				PreviousButton.onClick.RemoveAllListeners();
				PreviousButton.onClick.AddListener(GoToPreviousPanel);
			}

			// Next Button
			if (NextButton != null)
			{
				NextButton.onClick.RemoveAllListeners();
				NextButton.onClick.AddListener(GoToNextPanel);
			}

			while (Viewport.rect.height <= 0f)
				await Task.Yield();

			if (!Application.isPlaying)
			{
				return;
			}

			SetupComplete = true;

			// Pagination
			if (Pagination != null)
			{
				Pagination.Initialize(Panels.Count);
			}

			CurrentPanel = PreviousPanel = TargetPanel = NearestPanel = StartingPanel;
			GoToPanel(CurrentPanel);

			_setupComplete = true;

			// Update Layout
			UpdateLayout();
		}

		protected virtual void RefreshPanelReferences()
		{
			for (int i = 0; i < NumberOfPanels; i++)
			{
				Panels[i] = Content.GetChild(i).gameObject;
				PanelsRT[i] = Panels[i].GetComponent<RectTransform>();
				PanelsComp[i] = Panels[i].GetComponent<ScrollPanel>();
			}
		}

		public virtual void UpdateLayout()
		{
			UpdatePanels();
			UpdateContent();
		}

		protected virtual void UpdatePanels()
		{
			if (!_setupComplete)
				return;

			for (int i = 0; i < NumberOfPanels; i++)
			{
				if (MovementType == MovementType.Fixed && AutomaticallyLayout)
				{
					PanelsRT[i].anchorMin = new Vector2(MovementAxis == MovementAxis.Horizontal ? 0f : 0.5f,
						MovementAxis == MovementAxis.Vertical ? 0f : 0.5f);
					PanelsRT[i].anchorMax = new Vector2(MovementAxis == MovementAxis.Horizontal ? 0f : 0.5f,
						MovementAxis == MovementAxis.Vertical ? 0f : 0.5f);

					Vector2 size = SizeControl == SizeControl.Manual ? (PanelsComp[i] != null ? PanelsComp[i].Size : Size) : Size;
					Margin margin = PanelsComp[i] != null ? PanelsComp[i].Margin : new Margin(Vector4.zero);

					float x = (margin.Right + margin.Left) / 2f - margin.Left;
					float y = (margin.Top + margin.Bottom) / 2f - margin.Bottom;
					Vector2 marginOffset = new Vector2(x / size.x, y / size.y);

					PanelsRT[i].pivot = new Vector2(0.5f, 0.5f) + marginOffset;
					PanelsRT[i].sizeDelta = size - new Vector2(margin.Left + margin.Right, margin.Top + margin.Bottom);

					float panelPosX = 0f;

					if (MovementAxis == MovementAxis.Horizontal)
					{
						if (i > 0)
						{
							RectTransform prev = PanelsRT[i - 1];
							Vector2 prevSize = PanelsComp[i - 1] != null ? PanelsRT[i - 1].rect.size : Size;
							Margin prevMargin = PanelsComp[i - 1] != null ? PanelsComp[i - 1].Margin : new Margin(Vector4.zero);

							panelPosX = prev.anchoredPosition.x +
							            prevSize.x / 2f +
							            size.x / 2f +
							            /*(prevMargin.Right < 0f ? Mathf.Abs(prevMargin.Right) : 0f) +*/
							            (margin.Left < 0f ? Mathf.Abs(margin.Left) : 0f) +
							            (i > 0f ? Padding.x : 0f);
						}
						else
						{
							panelPosX = size.x / 2f;
						}
					}

					float panelPosY = 0f;

					if (MovementAxis == MovementAxis.Vertical)
					{
						if (i > 0)
						{
							RectTransform prev = PanelsRT[i - 1];
							Vector2 prevSize = PanelsComp[i - 1] != null ? PanelsRT[i - 1].rect.size : Size;
							Margin prevMargin = PanelsComp[i - 1] != null ? PanelsComp[i - 1].Margin : new Margin(Vector4.zero);

							panelPosY = prev.anchoredPosition.y +
							            prevSize.y / 2f +
							            size.y / 2f +
							            (prevMargin.Bottom < 0f ? Mathf.Abs(prevMargin.Bottom) : 0f) +
							            (margin.Top < 0f ? Mathf.Abs(margin.Top) : 0f) +
							            (i > 0f ? Padding.y : 0f);
						}
						else
						{
							panelPosY = size.y / 2f;
						}
					}

					PanelsRT[i].anchoredPosition = new Vector2(panelPosX, panelPosY);
				}
			}
		}

		protected virtual void UpdateContent()
		{
			if (!_setupComplete)
				return;

			// Content
			if (MovementType == MovementType.Fixed)
			{
				// Automatic Layout
				if (AutomaticallyLayout)
				{
					Content.anchorMin = new Vector2(MovementAxis == MovementAxis.Horizontal ? 0f : 0.5f,
						MovementAxis == MovementAxis.Vertical ? 0f : 0.5f);
					Content.anchorMax = new Vector2(MovementAxis == MovementAxis.Horizontal ? 0f : 0.5f,
						MovementAxis == MovementAxis.Vertical ? 0f : 0.5f);
					Content.pivot = new Vector2(MovementAxis == MovementAxis.Horizontal ? 0f : 0.5f,
						MovementAxis == MovementAxis.Vertical ? 0f : 0.5f);

					float contentWidth = Width;

					if (MovementAxis == MovementAxis.Horizontal)
					{
						contentWidth = 0f;

						for (int i = 0; i < NumberOfPanels; i++)
						{
							if (PanelsRT[i] != null)
								contentWidth += PanelsRT[i].rect.width + (i > 0f && i < NumberOfPanels ? Padding.x : 0f);
						}
					}

					float contentHeight = Height;

					if (MovementAxis == MovementAxis.Vertical)
					{
						contentHeight = 0f;

						for (int i = 0; i < NumberOfPanels; i++)
						{
							if (PanelsComp[i] != null && SizeControl == SizeControl.Manual)
								contentHeight += PanelsRT[i].rect.height + (i > 0f && i < NumberOfPanels ? Padding.y : 0f);
						}
					}

					Content.sizeDelta = new Vector2(contentWidth, contentHeight);
				}

				// Infinite Scrolling
				/*if (infinitelyScroll)
				{
				    _scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
				    
				    var rect = Content.rect;

				    _contentLength = MovementAxis == MovementAxis.Horizontal
				        ? rect.width + Width * infiniteScrollingEndSpacing
				        : rect.height + Height * infiniteScrollingEndSpacing;

				    OnInfiniteScrolling(true);
				}*/

				// Occlusion Culling
				if (UseOcclusionCulling)
				{
					OnOcclusionCulling(true);
				}
			}
			else
			{
				AutomaticallyLayout = /*infinitelyScroll =*/ UseOcclusionCulling = false;
			}

			// Starting Panel
			float xOffset = MovementAxis == MovementAxis.Horizontal || MovementType == MovementType.Free ? Viewport.rect.width / 2f : 0f;
			float yOffset = MovementAxis == MovementAxis.Vertical || MovementType == MovementType.Free ? Viewport.rect.height / 2f : 0f;
			Vector2 offset = new Vector2(xOffset, yOffset);
			_previousContentAnchoredPosition = Content.anchoredPosition = -PanelsRT[CurrentPanel].anchoredPosition + offset;
		}

		private Vector2 DisplacementFromCenter(int index)
		{
			return PanelsRT[index].anchoredPosition + Content.anchoredPosition - new Vector2(Viewport.rect.width * (0.5f - Content.anchorMin.x),
				Viewport.rect.height * (0.5f - Content.anchorMin.y));
		}

		private int DetermineNearestPanel()
		{
			int panelNumber = NearestPanel;
			float[] distances = new float[NumberOfPanels];
			for (int i = 0; i < Panels.Count; i++)
			{
				distances[i] = DisplacementFromCenter(i).magnitude;
			}

			float minDistance = Mathf.Min(distances);
			for (int i = 0; i < Panels.Count; i++)
			{
				if (Math.Abs(minDistance - distances[i]) < 0.001f)
				{
					panelNumber = i;
					break;
				}
			}

			return panelNumber;
		}

		private void DetermineVelocity()
		{
			Vector2 anchoredPosition = Content.anchoredPosition;
			Vector2 displacement = anchoredPosition - _previousContentAnchoredPosition;
			float time = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

			_velocity = displacement / time;
			_previousContentAnchoredPosition = anchoredPosition;
		}

		private void SelectTargetPanel()
		{
			Vector2 displacementFromCenter = DisplacementFromCenter(NearestPanel = DetermineNearestPanel());

			if (SnapTarget == SnapTarget.Nearest || _releaseSpeed <= MinimumSwipeSpeed)
			{
				GoToPanel(NearestPanel);
			}
			else if (SnapTarget == SnapTarget.Previous)
			{
				if (_releaseDirection == Direction.Right && displacementFromCenter.x < 0f ||
				    _releaseDirection == Direction.Up && displacementFromCenter.y < 0f)
				{
					GoToNextPanel();
				}
				else if (_releaseDirection == Direction.Left && displacementFromCenter.x > 0f ||
				         _releaseDirection == Direction.Down && displacementFromCenter.y > 0f)
				{
					GoToPreviousPanel();
				}
				else
				{
					GoToPanel(NearestPanel);
				}
			}
			else if (SnapTarget == SnapTarget.Next)
			{
				if (_releaseDirection == Direction.Right && displacementFromCenter.x > 0f ||
				    _releaseDirection == Direction.Up && displacementFromCenter.y > 0f)
				{
					GoToPreviousPanel();
				}
				else if (_releaseDirection == Direction.Left && displacementFromCenter.x < 0f ||
				         _releaseDirection == Direction.Down && displacementFromCenter.y < 0f)
				{
					GoToNextPanel();
				}
				else
				{
					GoToPanel(NearestPanel);
				}
			}
		}

		private void SnapToTargetPanel(bool immediate = false)
		{
			float xOffset = MovementAxis == MovementAxis.Horizontal || MovementType == MovementType.Free ? Viewport.rect.width / 2f : 0f;
			float yOffset = MovementAxis == MovementAxis.Vertical || MovementType == MovementType.Free ? Viewport.rect.height / 2f : 0f;
			Vector2 offset = new Vector2(xOffset, yOffset);

			Vector2 targetPosition = -PanelsRT[TargetPanel].anchoredPosition + offset;

			if (immediate)
			{
				Content.anchoredPosition = targetPosition;
			}
			else
			{
				Content.anchoredPosition = Vector2.Lerp(Content.anchoredPosition, targetPosition,
					(useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * SnappingSpeed);
			}

			if (CurrentPanel != TargetPanel)
			{
				if (DisplacementFromCenter(TargetPanel).magnitude < Viewport.rect.width / 10f)
				{
					PreviousPanel = CurrentPanel;
					CurrentPanel = TargetPanel;

					OnPanelChanged.Invoke();
				}
				else
				{
					OnPanelChanging.Invoke();
				}
			}
		}

		private void OnSelectingAndSnapping()
		{
			if (_selected)
			{
				if (!((_dragging || _pressing) && SwipeGestures))
				{
					SnapToTargetPanel();
				}
			}
			else if (!_dragging && (ScrollRect.velocity.magnitude <= ThresholdSnappingSpeed || ThresholdSnappingSpeed == -1f))
			{
				SelectTargetPanel();
			}
		}

		private void OnOcclusionCulling(bool forceUpdate = false)
		{
			if (UseOcclusionCulling && (_velocity.magnitude > 0f || forceUpdate))
			{
				float width = 0f;

				if (MovementAxis == MovementAxis.Horizontal)
				{
					width = Screen.width * 0.5f + Viewport.rect.width;
				}
				else
				{
					width = Screen.height * 0.5f + Viewport.rect.height;
				}

				for (int i = 0; i < NumberOfPanels; i++)
				{
					if (MovementAxis == MovementAxis.Horizontal)
					{
						PanelsComp[i].EnableComponents(Mathf.Abs(DisplacementFromCenter(i).x) < width);
					}
					else if (MovementAxis == MovementAxis.Vertical)
					{
						PanelsComp[i].EnableComponents(Mathf.Abs(DisplacementFromCenter(i).y) < width);
					}
				}
			}
		}

		/*private void OnInfiniteScrolling(bool forceUpdate = false)
		{
		    if (infinitelyScroll && (_velocity.magnitude > 0 || forceUpdate))
		    {
		        if (MovementAxis == MovementAxis.Horizontal)
		        {
		            for (var i = 0; i < NumberOfPanels; i++)
		            {
		                if (DisplacementFromCenter(i).x > Content.rect.width / 2f)
		                {
		                    PanelsRT[i].anchoredPosition -= new Vector2(_contentLength, 0);
		                }
		                else if (DisplacementFromCenter(i).x < Content.rect.width / -2f)
		                {
		                    PanelsRT[i].anchoredPosition += new Vector2(_contentLength, 0);
		                }
		            }
		        }
		        else if (MovementAxis == MovementAxis.Vertical)
		        {
		            for (var i = 0; i < NumberOfPanels; i++)
		            {
		                if (DisplacementFromCenter(i).y > Content.rect.height / 2f)
		                {
		                    PanelsRT[i].anchoredPosition -= new Vector2(0, _contentLength);
		                }
		                else if (DisplacementFromCenter(i).y < Content.rect.height / -2f)
		                {
		                    PanelsRT[i].anchoredPosition += new Vector2(0, _contentLength);
		                }
		            }
		        }
		    }
		}*/

		private void OnTransitionEffects()
		{
			if (PanelsComp.Count == 0) return;

			for (int i = 0; i < NumberOfPanels; i++)
			{
				// Displacement
				float displacement = 0f;

				if (MovementType == MovementType.Fixed)
				{
					if (MovementAxis == MovementAxis.Horizontal)
					{
						displacement = DisplacementFromCenter(i).x;
					}
					else if (MovementAxis == MovementAxis.Vertical)
					{
						displacement = DisplacementFromCenter(i).y;
					}
				}
				else
				{
					displacement = DisplacementFromCenter(i).magnitude;
				}

				if (PanelsComp[i] != null && PanelsComp[i].AreComponentsEnabled)
				{
					PanelsComp[i].SetTransitionProgress(displacement);
				}
			}
		}

		private void OnSwipeGestures()
		{
			if (SwipeGestures)
			{
				ScrollRect.horizontal = MovementAxis == MovementAxis.Horizontal || MovementType == MovementType.Free;
				ScrollRect.vertical = MovementAxis == MovementAxis.Vertical || MovementType == MovementType.Free;
			}
			else
			{
				ScrollRect.horizontal = ScrollRect.vertical = !_dragging;
			}
		}

		public async UniTask GoToPanelDelayed(ScrollPanel scrollPanel, float delay) => await GoToPanelDelayed(scrollPanel.gameObject, delay);

		public async UniTask GoToPanelDelayed(GameObject scrollPanel, float delay)
		{
			for (int i = 0; i < PanelsComp.Count; i++)
			{
				if (Panels[i] != scrollPanel) continue;
				await GoToPanelDelayed(i, delay);
				return;
			}
		}

		public async UniTask GoToPanelDelayed(int panelNumber, float delay)
		{
			await UniTask.Delay(Timef.SecondsToMilliseconds(delay));
			GoToPanel(panelNumber, false);
		}

		public void GoToPanel(ScrollPanel scrollPanel, bool immediate = false, bool forceUpdate = false) =>
			GoToPanel(scrollPanel.gameObject, immediate, forceUpdate);

		public void GoToPanel(GameObject scrollPanel, bool immediate = false, bool forceUpdate = false)
		{
			for (int i = 0; i < PanelsComp.Count; i++)
			{
				if (Panels[i] != scrollPanel) continue;
				GoToPanel(i, _setupComplete, forceUpdate);
				return;
			}
		}

		public void GoToPanel(int panelNumber, bool immediate = false, bool forceUpdate = false)
		{
			if (!SetupComplete)
				return;

			if (PanelsComp[TargetPanel] != null && TargetPanel != panelNumber) // If Target is equal to panelNumber, don't deselect
			{
				PanelsComp[TargetPanel].SelectPanel(false, immediate);
			}

			TargetPanel = panelNumber;
			_selected = true;
			OnPanelSelected.Invoke();

			// Select if TargetPanel is not equal to panelNumber or if it's equal, only select if it's not yet selected
			if (PanelsComp[TargetPanel] != null && (TargetPanel != panelNumber || !PanelsComp[TargetPanel].IsSelected || forceUpdate))
			{
				PanelsComp[TargetPanel].SelectPanel(true, forceUpdate);
			}

			if (Pagination != null)
			{
				Pagination.SelectElement(TargetPanel, immediate);
			}

			if (HardSnap)
			{
				ScrollRect.inertia = false;
			}

			if (immediate)
			{
				SnapToTargetPanel(true);
			}
		}

		public void GoToPreviousPanel()
		{
			if (!SetupComplete)
				return;

			NearestPanel = DetermineNearestPanel();

			if (NearestPanel != 0)
			{
				GoToPanel(NearestPanel - 1);
			}
			else
			{
				/*if (infinitelyScroll)
				{
				    GoToPanel(NumberOfPanels - 1);
				}
				else*/
				{
					GoToPanel(NearestPanel);
				}
			}
		}

		public void GoToNextPanel()
		{
			if (!SetupComplete)
				return;

			NearestPanel = DetermineNearestPanel();

			if (NearestPanel != NumberOfPanels - 1)
			{
				GoToPanel(NearestPanel + 1);
			}
			else
			{
				/*if (infinitelyScroll)
				{
				    GoToPanel(0);
				}
				else*/
				{
					GoToPanel(NearestPanel);
				}
			}
		}

		public GameObject AddToFront(ScrollPanel panel) => AddToFront(panel.gameObject);

		public GameObject AddToFront(GameObject panel)
		{
			return Add(panel, 0);
		}

		public GameObject AddToBack(ScrollPanel panel) => AddToBack(panel.gameObject);

		public GameObject AddToBack(GameObject panel)
		{
			return Add(panel, NumberOfPanels);
		}

		public GameObject Add(GameObject panel, int index)
		{
			if (NumberOfPanels != 0 && (index < 0 || index > NumberOfPanels))
			{
				Debug.LogError("<b>[ScrollSnap]</b> Index must be an integer from 0 to " + NumberOfPanels + ".", gameObject);
				return null;
			}

			if (!AutomaticallyLayout)
			{
				Debug.LogError("<b>[ScrollSnap]</b> \"Automatic Layout\" must be enabled for content to be dynamically added during runtime.");
				return null;
			}

			GameObject gos = Instantiate(panel, Content, false);
			gos.transform.SetSiblingIndex(index);

			Initialize();

			if (Validate())
			{
				if (TargetPanel <= index)
				{
					StartingPanel = TargetPanel;
				}
				else
				{
					StartingPanel = TargetPanel + 1;
				}

				Setup();
			}

			return gos;
		}

		public void RemoveFromFront()
		{
			Remove(0);
		}

		public void RemoveFromBack()
		{
			if (NumberOfPanels > 0)
			{
				Remove(NumberOfPanels - 1);
			}
			else
			{
				Remove(0);
			}
		}

		public void Remove(ScrollPanel panel) => Remove(panel.gameObject);

		public void Remove(GameObject gos)
		{
			for (int i = 0; i < Panels.Count; i++)
			{
				if (Panels[i] != gos) continue;
				Remove(i);
				return;
			}

			Debug.LogError("<b>[ScrollSnap]</b> Can't remove GameObject because it is not a child of this ScrollSnap");
		}

		public void Remove(int index)
		{
			if (NumberOfPanels == 0)
			{
				Debug.LogError("<b>[ScrollSnap]</b> There are no panels to remove.", gameObject);
				return;
			}

			if (index < 0 || index > NumberOfPanels - 1)
			{
				Debug.LogError("<b>[ScrollSnap]</b> Index must be an integer from 0 to " + (NumberOfPanels - 1) + ".", gameObject);
				return;
			}

			if (!AutomaticallyLayout)
			{
				Debug.LogError(
					"<b>[ScrollSnap]</b> \"Automatic Layout\" must be enabled for content to be dynamically removed during runtime.");
				return;
			}

			DestroyImmediate(Panels[index]);

			Initialize();
			if (Validate())
			{
				if (TargetPanel == index)
				{
					if (index == NumberOfPanels)
					{
						StartingPanel = TargetPanel - 1;
					}
					else
					{
						StartingPanel = TargetPanel;
					}
				}
				else if (TargetPanel < index)
				{
					StartingPanel = TargetPanel;
				}
				else
				{
					StartingPanel = TargetPanel - 1;
				}

				Setup();
			}
		}

		public void MoveToIndex(int fromIndex, int toIndex)
		{
			if (fromIndex >= 0 && fromIndex < NumberOfPanels)
			{
				GameObject panel = Panels[fromIndex];
				MoveToIndex(panel, toIndex);
			}
			else
			{
				Debug.LogError("<b>[ScrollSnap]</b> Can't move panel because fromIndex is out of bounds");
			}
		}

		public void MoveToIndex(ScrollPanel panel, int index) => MoveToIndex(panel.gameObject, index);

		public void MoveToIndex(GameObject panel, int index)
		{
			if (!AutomaticallyLayout)
			{
				Debug.LogError("<b>[ScrollSnap]</b> \"Automatic Layout\" must be enabled for content to be dynamically added during runtime.");
				return;
			}

			if (!Panels.Contains(panel))
			{
				Debug.LogError("<b>[ScrollSnap]</b> Can't move panel to front since it's not registered");
				return;
			}

			if (index < 0)
			{
				Debug.LogError("<b>[ScrollSnap]</b> Can't move panel because index is negative");
				return;
			}

			if (index >= NumberOfPanels)
			{
				Debug.LogError("<b>[ScrollSnap]</b> Can't move panel because index is too large");
				return;
			}

			panel.transform.SetSiblingIndex(index);
			RefreshPanelReferences();
			UpdateLayout();
		}

		public void MoveToFront(ScrollPanel panel) => MoveToFront(panel.gameObject);

		public void MoveToFront(GameObject panel)
		{
			if (!AutomaticallyLayout)
			{
				Debug.LogError("<b>[ScrollSnap]</b> \"Automatic Layout\" must be enabled for content to be dynamically added during runtime.");
				return;
			}

			if (!Panels.Contains(panel))
			{
				Debug.LogError("<b>[ScrollSnap]</b> Can't move panel to front since it's not registered");
				return;
			}

			panel.transform.SetAsFirstSibling();
			RefreshPanelReferences();
			UpdateLayout();
		}

		public void MoveToBack(ScrollPanel panel) => MoveToBack(panel.gameObject);

		public void MoveToBack(GameObject panel)
		{
			if (!AutomaticallyLayout)
			{
				Debug.LogError("<b>[ScrollSnap]</b> \"Automatic Layout\" must be enabled for content to be dynamically added during runtime.");
				return;
			}

			if (!Panels.Contains(panel))
			{
				Debug.LogError("<b>[ScrollSnap]</b> Can't move panel to front since it's not registered");
				return;
			}

			panel.transform.SetAsLastSibling();
			RefreshPanelReferences();
			UpdateLayout();
		}

		public void AddVelocity(Vector2 velocity)
		{
			ScrollRect.velocity += velocity;
			_selected = false;
		}

		#endregion
	}
}