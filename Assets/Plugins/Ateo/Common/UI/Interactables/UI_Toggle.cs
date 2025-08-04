using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ateo.UI
{
    public class UI_Toggle : UnityEngine.UI.Toggle
    {
/*         public enum AnimationMode { None, Animate };
        private AnimationMode m_AnimationMode = AnimationMode.Animate; */

        [SerializeField]
        private float m_AnimationDuration = 0.25f;

        private RectTransform m_RectTrans;
        private UI_CanvasGroup m_CanvasGroup;

        [SerializeField]
        private UI_CanvasGroup m_Highlighted;

        [SerializeField]
        private UI_CanvasGroup m_Pressed;

        [SerializeField]
        private Graphic m_ToggleOn;

        [SerializeField]
        private Graphic m_ToggleOff;

        [SerializeField]
        private float m_CrossFadeTime = 0.1f;


        [SerializeField]
        public Text m_ToogleText;

        [SerializeField]
        public string m_TextOn;

        [SerializeField]
        public string m_TextOff;

        // Runtime
        private bool m_IsSelected = false;
        private bool m_IsFadedIn = false;
        private bool m_IsFadedOut = false;
        private int m_ClickCounter = 0;
        private bool m_Initialized = false;

        protected override void Awake()
        {
            base.Awake();
            m_CanvasGroup = gameObject.GetComponent<UI_CanvasGroup>();
            graphic = null;
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            onValueChanged.AddListener(OnValueChanged);
            OnValueChanged(true);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            onValueChanged.RemoveListener(OnValueChanged);
        }
        protected override void Start()
        {
            base.Start();
            m_Initialized = true;
        }

/*         public void SetupGraphics()
        {
            m_CanvasGroup = targetGraphic.GetComponent<UI_CanvasGroup>();
            if (m_CanvasGroup == null)
                m_CanvasGroup = targetGraphic.gameObject.AddComponent<UI_CanvasGroup>();

            m_CanvasGroup.Init();

            if (spriteState.highlightedSprite != null)
            {
                if (m_Highlighted != null)
                {
                    GameObject.Destroy(m_Highlighted.gameObject);
                }

                m_Highlighted = CloneTargetGraphic("Highlighted", spriteState.highlightedSprite, 0);
            }
            if (spriteState.pressedSprite != null)
            {
                if (m_Pressed != null)
                {
                    GameObject.Destroy(m_Pressed.gameObject);
                }

                m_Pressed = CloneTargetGraphic("Pressed", spriteState.pressedSprite, 1);
            }

            HideGraphics();
        }

        public void HideGraphics()
        {
            if (m_Highlighted != null)
                m_Highlighted.SetActive(false);
            if (m_Pressed != null)
                m_Pressed.SetActive(false);
        }
        private UI_CanvasGroup CloneTargetGraphic(string name, Sprite sprite, int index)
        {
            var gos = new GameObject();
            gos.transform.SetParent(targetGraphic.transform);
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
*/

        /// <summary>
        /// React to clicks.
        /// </summary>
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsActive() || !IsInteractable())
                return;

            isOn = !isOn; //This triggers the event in the base class
            PressButton();
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
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if(!m_Initialized)
                return;

            if (!Application.isPlaying)
            {
                m_CanvasGroup.SetActive(interactable);
                return;
            }

            // Toogle is not interactable anymore
            if (state == SelectionState.Disabled)
            {
                if (!m_IsFadedOut)
                {
                    m_IsFadedIn = false;
                    m_IsFadedOut = true;

                    m_CanvasGroup.Fade(0.5f, m_AnimationDuration);
                }
            }
            else
            {
                if (!m_IsFadedIn)
                {
                    m_IsFadedIn = true;
                    m_IsFadedOut = false;

                    m_CanvasGroup.Fade(1f, m_AnimationDuration);
                }
            }
        }

        // Button Press
        public void PressButton()
        {
            m_ClickCounter++;

            if (m_ClickCounter == 1)
                AnimateButtonPress();
        }

        private void AnimateButtonPress()
        {
            if (m_Pressed != null)
            {
                m_Pressed.Fade(1f, m_AnimationDuration);
                Invoke("AnimateButtonRelease", m_AnimationDuration);
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

        private void OnValueChanged(bool value)
        {
            if(m_ToogleText != null)
            {
                m_ToogleText.text = value ? m_TextOn : m_TextOff;
            }
            
            if (m_ToggleOn != null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    m_ToggleOn.canvasRenderer.SetAlpha(isOn ? 1f : 0f);
                else
#endif
                m_ToggleOn.CrossFadeAlpha(isOn ? 1f : 0f, m_CrossFadeTime, true);
            }

            if (m_ToggleOff != null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    m_ToggleOff.canvasRenderer.SetAlpha(isOn ? 0f : 1f);
                else
#endif
                m_ToggleOff.CrossFadeAlpha(isOn ? 0f : 1f, m_CrossFadeTime, true);
            }
        }
    }
}
