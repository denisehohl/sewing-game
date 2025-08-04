using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ateo.UI
{
    [System.Serializable]
    [ExecuteInEditMode]
    public class UI_Button : Button
    {
        public enum ButtonClickMode { OnClick, OnPointerDown, OnPointerUp, OnDelay };

        [SerializeField]
        public ButtonClickMode m_ButtonClickMode = ButtonClickMode.OnClick;

        [SerializeField]
        private float m_ClickDelay = 0.25f;

        [SerializeField]
        private float m_AnimationDuration = 0.25f;
        
        [SerializeField]
        private float m_DisabledAlpha = 0.5f;

        [SerializeField]
        public UI_CanvasGroup m_CanvasGroup;

        [SerializeField]
        public UI_CanvasGroup m_Highlighted;

        [SerializeField]
        public UI_CanvasGroup m_Pressed;
        
        [SerializeField]
        public UI_CanvasGroup m_Selected;

        // Runtime
        private bool m_BlockPointerClick = false;
        private bool m_IsSelected = false;
        private bool m_IsFadedIn = false;
        private bool m_IsFadedOut = false;
        private int m_ClickCounter = 0;
        private bool m_Initialized = false;

        protected override void Awake()
        {
            if (!gameObject.TryGetComponent<UI_CanvasGroup>(out var canvasGroup))
            {
                m_CanvasGroup = gameObject.AddComponent<UI_CanvasGroup>();
            }

            base.Awake();
        }

        protected override void OnEnable()
        {
            if (!gameObject.TryGetComponent<UI_CanvasGroup>(out var canvasGroup))
            {
                m_CanvasGroup = gameObject.AddComponent<UI_CanvasGroup>();
            }

            if(Application.isPlaying && !interactable)
            {
                if (m_CanvasGroup != null)
                    m_CanvasGroup.Fade(m_DisabledAlpha, 0f);
            }

            m_Initialized = true;
            base.OnEnable();
        }
        public void SetupGraphics()
        {
            if (!gameObject.TryGetComponent<UI_CanvasGroup>(out var canvasGroup))
            {
                m_CanvasGroup = gameObject.AddComponent<UI_CanvasGroup>();
            }

            m_CanvasGroup.Init();
            
            if (m_Highlighted == null)
            {
                m_Highlighted = CloneTargetGraphic("Highlighted", spriteState.highlightedSprite, 0);
            }
            
            if (m_Pressed == null)
            {
                m_Pressed = CloneTargetGraphic("Pressed", spriteState.pressedSprite, 1);
            }
            
            if (m_Selected == null)
            {
                m_Selected = CloneTargetGraphic("Selected", spriteState.selectedSprite, 2);
            }

            HideGraphics();
        }

        private void HideGraphics()
        {
            if (m_Highlighted != null)
                m_Highlighted.SetActive(false);
            if (m_Pressed != null)
                m_Pressed.SetActive(false);
            if(m_Selected != null)
                m_Selected.SetActive(false);
        }

        private UI_CanvasGroup CloneTargetGraphic(string name, Sprite sprite, int index)
        {
            var gos = new GameObject();
            gos.transform.SetParent(this.transform);
            gos.name = name;

            var rectTrans = gos.AddComponent<RectTransform>();
            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.one;
            rectTrans.pivot = Vector2.one * 0.5f;
            rectTrans.sizeDelta = Vector2.zero;
            rectTrans.anchoredPosition = Vector3.zero;
            rectTrans.localRotation = Quaternion.identity;
            rectTrans.localScale = Vector3.one;
            rectTrans.SetSiblingIndex(index);

            var image = gos.AddComponent<Image>();
            image.sprite = sprite;

            var canvasgroup = gos.AddComponent<UI_CanvasGroup>();
            canvasgroup.Init();
            return canvasgroup;
        }

        protected virtual bool InvokeEvent()
        {
            if (!IsActive() || !IsInteractable())
                return false;

            onClick.Invoke();
            return true;
        }
        
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (m_BlockPointerClick)
                return;

            if (m_ButtonClickMode == ButtonClickMode.OnDelay)
                StartCoroutine(OnPointerClickDelayed());
            else if (m_ButtonClickMode == ButtonClickMode.OnClick)
                InvokeEvent();
        }
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (IsInteractable())
            {
                PressButton();
            }

            base.OnPointerDown(eventData);
            
            if (m_ButtonClickMode == ButtonClickMode.OnPointerDown)
                InvokeEvent();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            
            if (m_ButtonClickMode == ButtonClickMode.OnPointerUp)
                InvokeEvent();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (IsInteractable())
            {
                m_IsSelected = true;

                if (m_Highlighted != null)
                    m_Highlighted.Fade(1f, m_AnimationDuration);
            }

            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (m_IsSelected)
            {
                if (m_Highlighted != null)
                    m_Highlighted.Fade(0f, m_AnimationDuration);
            }

            m_IsSelected = false;

            base.OnPointerExit(eventData);
        }
        private IEnumerator OnPointerClickDelayed()
        {
            m_BlockPointerClick = true;
            yield return new WaitForSeconds(m_ClickDelay);
            m_BlockPointerClick = false;

            InvokeEvent();
        }
        
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if(!m_Initialized)
                return;

            if (!Application.isPlaying || !gameObject.activeInHierarchy)
            {
                m_CanvasGroup.SetActive(interactable, interactable ? 1f : m_DisabledAlpha);
                m_IsFadedOut = interactable;
                return;
            }

            if (state == SelectionState.Disabled)
            {
                targetGraphic = null;

                if (!m_IsFadedOut)
                {
                    m_IsFadedIn = false;
                    m_IsFadedOut = true;

                    if (m_CanvasGroup != null)
                        m_CanvasGroup.Fade(m_DisabledAlpha, m_AnimationDuration);
                    else
                        DebugDev.LogError("UI_Button " + name + " has no UI_CanvasGroup");
                }
            }
            else
            {
                if (!m_IsFadedIn)
                {
                    m_IsFadedIn = true;
                    m_IsFadedOut = false;

                    if (m_CanvasGroup != null)
                        m_CanvasGroup.Fade(1f, m_AnimationDuration);
                    else
                        DebugDev.LogError("UI_Button " + name + " has no UI_CanvasGroup");
                }
            }
        }
        
        public virtual void PressButton()
        {
            m_ClickCounter++;

            if (m_ClickCounter == 1)
                AnimateButtonPress();
        }

        public virtual void SelectButton()
        {
            if (m_Selected != null)
            {
                m_Selected.Fade(1f, m_AnimationDuration);
            }
        }

        public virtual void DeselectButton()
        {
            if (m_Selected != null)
            {
                m_Selected.Fade(0f, m_AnimationDuration);
            }
        }

        private void AnimateButtonPress()
        {
            if (m_Pressed != null)
            {
                m_Pressed.Fade(1f, m_AnimationDuration);
                Invoke(nameof(AnimateButtonRelease), m_AnimationDuration);
            }
            else
            {
                ReleaseButton();
            }
        }
        
        private void AnimateButtonRelease()
        {
            if (m_Pressed != null)
            {
                m_Pressed.Fade(0f, m_AnimationDuration);
                Invoke("ReleaseButton", m_AnimationDuration);
            }
            else
            {
                ReleaseButton();
            }
        }
        
        private void ReleaseButton()
        {
            m_ClickCounter--;

            if (m_ClickCounter > 0)
                AnimateButtonPress();
        }
    }
}
