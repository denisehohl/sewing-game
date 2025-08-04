using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Ateo.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(UI_CanvasGroup))]
    public class UI_Multiple : MonoBehaviour
    {
        public enum MultipleType { Integer, Custom };

        [SerializeField]
        private bool m_Interactable = true;

        [SerializeField]
        private float m_AnimationDuration = 0.25f;

        [SerializeField]
        protected MultipleType m_Type;

        [SerializeField]
        private bool m_Clamp = true;

        [SerializeField]
        protected int m_Default = 0;

        [SerializeField]
        protected int m_Min = 0;

        [SerializeField]
        protected int m_Max = 1;

        [SerializeField]
        protected string[] m_CustomLabels;

        [SerializeField]
        private Button m_ButtonLeft;

        [SerializeField]
        private Button m_ButtonRight;

        [SerializeField]
        private Text m_Label;

        [System.Serializable]
        public class UnityEventInt : UnityEvent<int>
        {
        }

        public UnityEventInt onValueChanged = new UnityEventInt();


        private int m_Value;
        private bool m_isFadedOut = false;
        private UI_CanvasGroup m_CanvasGroup;

        // Properties
        public bool interactable
        {
            get { return m_Interactable; }
            set
            {
                m_Interactable = value;
                StyleInteractable();
            }
        }

        // Unity-Callback
        private void OnEnable()
        {
            if (Application.isPlaying)
            {
                m_ButtonLeft.onClick.AddListener(Decrease);
                m_ButtonRight.onClick.AddListener(Increase);
            }

            m_Value = m_Default;
            m_CanvasGroup = GetComponent<UI_CanvasGroup>();

            if (m_Type == MultipleType.Custom)
            {
                if (m_CustomLabels.Length != (m_Max + 1))
                {
                    DebugDev.LogError("UI_Multiple (" + gameObject.name + "): Please asign " + m_Max + " labels in the inspector.");
                    return;
                }
            }

            StyleInteractable();
            StyleLabel();
            StyleButtons();
        }
        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                m_ButtonLeft.onClick.RemoveListener(Decrease);
                m_ButtonRight.onClick.RemoveListener(Increase);
            }
        }

        // Public Functions
        public void SetValue(int value, int min, int max, bool send = true)
        {
            m_Min = min;
            m_Max = max;
            SetValue(value);
        }
        public void SetValue(int value)
        {
            SetValue(value, true);
        }
        public void SetValue(int value, bool send = true)
        {
            var previous = m_Value;
            m_Value = Mathf.Clamp(value, m_Min, m_Max);

            if (previous != m_Value && send)
            {
                if (onValueChanged != null)
                    onValueChanged.Invoke(m_Value);
            }

            StyleLabel();
            StyleButtons();
        }
        public void SetMinMax(int min, int max)
        {
            m_Min = min;
            m_Max = max;

            SetValue(m_Value);
        }

        // Event-Listeners
        private void Increase()
        {
            SetValue(IncrementValue(1));
        }
        private void Decrease()
        {
            SetValue(IncrementValue(-1));
        }

        // Private Functions
        private int IncrementValue(int i)
        {
            int output = m_Value + i;

            if (m_Clamp)
            {
                output = Mathf.Clamp(output, m_Min, m_Max);
            }
            else
            {
                if (output < m_Min)
                    output = m_Max;
                else if (output > m_Max)
                    output = m_Min;
            }

            return output;
        }
        private void StyleInteractable()
        {
            if (!Application.isPlaying)
            {
                m_CanvasGroup.alpha = interactable ? 1f : 0.5f;
                m_isFadedOut = !interactable;
                return;
            }

            if (!interactable)
            {
                if (!m_isFadedOut)
                {
                    m_isFadedOut = true;

                    if (m_CanvasGroup != null)
                        m_CanvasGroup.Fade(0.5f, m_AnimationDuration);
                    else
                        DebugDev.LogError("UI_Multiple " + name + " has no m_CanvasGroup");
                }
            }
            else
            {
                if (!m_isFadedOut)
                {
                    m_isFadedOut = false;

                    if (m_CanvasGroup != null)
                        m_CanvasGroup.Fade(1f, m_AnimationDuration);
                    else
                        DebugDev.LogError("UI_Multiple " + name + " has no m_CanvasGroup");
                }
            }
        }

        private void StyleLabel()
        {
            if (m_Type == MultipleType.Integer)
            {
                m_Label.text = m_Value.ToString();
            }
            else
            {
                m_Label.text = m_CustomLabels[m_Value];
            }
        }

        private void StyleButtons()
        {
            if (m_Clamp)
            {
                m_ButtonLeft.interactable = m_Value != m_Min;
                m_ButtonRight.interactable = m_Value != m_Max;
            }
            else
            {
                m_ButtonLeft.interactable = m_Min != m_Max;
                m_ButtonRight.interactable = m_Min != m_Max;
            }
        }
    }
}
