using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using TMPro;

namespace Ateo.UI
{
    [RequireComponent(typeof(UnityEngine.UI.Toggle))]
    public class DropdownItem : MonoBehaviour
    {
        [ReadOnly]
        public DropdownItemData m_Data;

        public TextMeshProUGUI m_Text;
        public Image m_Image;
        public RectTransform m_RectTransform;
        public UnityEngine.UI.Toggle m_Toggle;
        private Dropdown m_Dropdown;

        public UnityEngine.UI.Toggle Toggle
        {
            get => m_Toggle;
            set => m_Toggle = value;
        }

        public DropdownItemData Data
        {
            get => m_Data;
        }

        public void Initialize(Dropdown dropdown, DropdownItemData data, bool isOn)
        {
            m_Dropdown = dropdown;
            m_Data = data;
            m_Data.Item = this;

            if (m_Text != null)
            {
                m_Text.text = data.Text;
            }

            if (m_Image != null)
            {
                m_Image.sprite = data.Image;
                m_Image.enabled = (m_Image.sprite != null);
            }

            if (Toggle != null)
            {
                Toggle.isOn = isOn;
            }

            m_Toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        public void SetToggleState(bool value)
        {
            if (Toggle == null) return;
            
            if (Toggle.isOn != value)
                Toggle.SetIsOnWithoutNotify(value);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!Application.isPlaying) return;
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public void OnCancel(BaseEventData eventData)
        {
            if (!Application.isPlaying) return;
            m_Dropdown.Hide();
        }

        private void OnToggleValueChanged(bool value)
        {
            m_Dropdown.SetItemChanged(value, Data);
        }
    }
}
// © 2019 Ateo GmbH (https://www.ateo.ch)