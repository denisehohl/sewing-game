using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Ateo.Animation;
using Ateo.Events;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI.Extensions;

namespace Ateo.UI
{
    [AddComponentMenu("UI/Dropdown (Ateo)", 35)]
    [RequireComponent(typeof(RectTransform))]
    public class Dropdown : Selectable, IPointerClickHandler, ISubmitHandler, ICancelHandler
    {
        [Serializable]
        public class DropdownHandler : UnityEvent<List<DropdownItemData>>
        {
        }

        [Title("References"), SerializeField, Required]
        private TextMeshProUGUI m_CaptionText;

        [SerializeField, Required]
        private AnimationBehaviour m_AnimationShow;

        [SerializeField, Required]
        private AnimationBehaviour m_AnimationHide;

        [AssetsOnly, AssetSelector, Required]
        public GameObject m_DropdownItemPrefab;

        [ChildGameObjectsOnly, Required]
        public Transform m_DropdownItemParent;

        [ChildGameObjectsOnly, Required]
        public UI_ScrollRect m_ScrollRect;

        [ChildGameObjectsOnly, Required]
        public UI_Scrollbar m_Scrollbar;

        [Title("Properties"), SerializeField]
        private List<DropdownItemData> m_ItemsToAdd = new List<DropdownItemData>();

        [SerializeField]
        private string m_DefaultCaptionText;

        [SerializeField]
        private bool m_IsMultiSelectable = true;

        [SerializeField]
        private bool m_CanHaveNoSelection = true;

        [Title("Events"), Space]
        public DropdownHandler OnValueChanged = new DropdownHandler();

        [System.NonSerialized]
        private Dictionary<string, DropdownItemData> m_Items = new Dictionary<string, DropdownItemData>();

        [System.NonSerialized]
        private Dictionary<string, DropdownItemData> m_ItemsEnabled = new Dictionary<string, DropdownItemData>();

        private bool m_NewSelection = false;

        public event DelegateVoid OnOpen;
        public event DelegateVoid OnClose;

        public bool IsOpen { get; protected set; } = false;
        public List<DropdownItemData> Items => m_Items.Values.ToList();
        public List<DropdownItemData> ItemsEnabled => m_ItemsEnabled.Values.ToList();
        public List<string> ItemsEnabledByName => m_ItemsEnabled.Keys.ToList();

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!Application.isPlaying) return;

            m_ScrollRect.OnEnter += OnScrollRectPointerEnter;
            m_ScrollRect.OnScrollEnd += OnScrollEnd;
            m_Scrollbar.OnCompleteDrag += OnScrollbarEndDrag;

            m_AnimationHide.ExecuteAnimationImmediate();
            RefreshCaption();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            m_ScrollRect.OnEnter -= OnScrollRectPointerEnter;
            m_ScrollRect.OnScrollEnd -= OnScrollEnd;
            m_Scrollbar.OnCompleteDrag -= OnScrollbarEndDrag;
        }

        protected override void Start()
        {
            base.Start();

            if (!Application.isPlaying) return;

            Add(m_ItemsToAdd);
            RefreshCaption();
        }

        protected override void OnDestroy()
        {
            if (Application.isPlaying) return;
            base.OnDestroy();
            Clear();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);

            if (!Application.isPlaying) return;
            Invoke(nameof(DecideHide), 0.15f);
        }

        private void DecideHide()
        {
            if (m_ScrollRect.IsMouseHovering && !m_NewSelection) return;
            Hide();
        }

        private void CheckIfHidden()
        {
            if (IsOpen)
                Hide();
        }

        public void RefreshCaption()
        {
            string text = string.Empty;
            int count = m_ItemsEnabled.Count;
            List<DropdownItemData> itemsEnabled = ItemsEnabled;
            itemsEnabled.Sort();

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    if (i > 0 && i < count)
                    {
                        text += ", ";
                    }

                    text += itemsEnabled[i].ToString();
                }
            }
            else
            {
                text = m_DefaultCaptionText;
            }

            m_CaptionText.text = text;
        }


        public void EnableItem(string name)
        {
            DropdownItemData data = null;
            m_Items.TryGetValue(name, out data);

            if (data != null)
            {
                EnableItem(data);
            }
        }

        public void EnableItem(DropdownItemData data)
        {
            if (!m_IsMultiSelectable)
            {
                if (m_ItemsEnabled.Values.Count > 0)
                {
                    var keys = m_ItemsEnabled.Keys.ToList();

                    foreach (var key in keys)
                    {
                        if (string.Equals(key, data.Text)) continue;

                        m_ItemsEnabled[key].Item.SetToggleState(false);
                        m_ItemsEnabled.Remove(key);
                    }
                }
            }

            if (!m_ItemsEnabled.ContainsKey(data.Text))
            {
                m_ItemsEnabled.Add(data.Text, data);
            }

            m_NewSelection = true;
            data.Item.SetToggleState(true);

            if (!m_IsMultiSelectable)
                Invoke(nameof(CheckIfHidden), 0.2f);
        }

        public void EnableItems(List<string> names, bool disableOthers = true)
        {
            foreach (var key in m_Items.Keys)
            {
                bool contains = false;

                foreach (var name in names)
                {
                    if (string.Equals(key, name))
                        contains = true;
                }

                if (contains)
                {
                    EnableItem(key);
                }
                else if (disableOthers)
                {
                    DisableItem(key);
                }
            }
        }

        public void DisableItem(string name)
        {
            DropdownItemData data = null;
            m_Items.TryGetValue(name, out data);

            if (data != null)
            {
                DisableItem(data);
            }
        }

        public void DisableItem(DropdownItemData data)
        {
            if (!m_IsMultiSelectable && !m_CanHaveNoSelection)
            {
                if (m_ItemsEnabled.Count == 1 && m_ItemsEnabled.ContainsKey(data.Text))
                {
                    data.Item.SetToggleState(true);
                    m_NewSelection = true;
                    return;
                }
            }

            if (!m_ItemsEnabled.ContainsKey(data.Text)) return;

            m_ItemsEnabled.Remove(data.Text);
            data.Item.SetToggleState(false);
        }

        public void DisableItems(List<string> names)
        {
            foreach (var name in names)
            {
                if (m_ItemsEnabled.ContainsKey(name))
                {
                    DisableItem(name);
                }
            }
        }

        public void DisableAllItems()
        {
            foreach (var item in m_ItemsEnabled.Keys.ToList())
            {
                if (m_ItemsEnabled.ContainsKey(item))
                {
                    DisableItem(item);
                }
            }

            RefreshCaption();
        }

        private DropdownItem CreateItem(string name)
        {
            var gos = Instantiate(m_DropdownItemPrefab, m_DropdownItemParent, false);

            gos.SetActive(true);
            gos.name = "Item " + m_Items.Count + (name != null ? ": " + name : "");

            return gos.GetComponent<DropdownItem>();
        }

        public void Add(string name, Sprite sprite = null, bool enabled = false)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = "Item " + m_Items.Count;
            }

            var data = new DropdownItemData(name, sprite);
            Add(data, enabled);
        }

        public void Add(DropdownItemData data, bool enabled = false)
        {
            if (!m_Items.ContainsKey(data.Text))
            {
                var item = CreateItem(data.Text);
                item.Initialize(this, data, enabled);
                m_Items.Add(data.Text, data);
            }
            else
            {
                DebugDev.LogWarning($"Dropdown.AddItem(): There already exists an item with the name {data.Text}");
            }
        }

        public void Add(List<string> names, bool enabled = false)
        {
            for (int i = 0; i < names.Count; i++)
            {
                Add(names[i], null, enabled);
            }

            RefreshCaption();
        }

        public void Add(List<DropdownItemData> data, bool enabled = false)
        {
            foreach (var d in data)
            {
                Add(d, enabled);
            }

            RefreshCaption();
        }

        public void Add(List<Sprite> sprites, bool enabled = false)
        {
            for (var i = 0; i < sprites.Count; i++)
            {
                Add(null, sprites[i], enabled);
            }

            RefreshCaption();
        }

        public void Remove(DropdownItemData data)
        {
            if (m_Items.ContainsKey(data.Text))
            {
                m_Items.Remove(data.Text);
            }

            if (m_ItemsEnabled.ContainsKey(data.Text))
            {
                m_ItemsEnabled.Remove(data.Text);
            }

            Destroy(data.Item.gameObject);
            RefreshCaption();
        }

        public void Clear()
        {
            foreach (var data in m_Items.Values)
            {
                Destroy(data.Item.gameObject);
            }

            m_Items.Clear();
            m_ItemsEnabled.Clear();
            RefreshCaption();
        }

        private void OnScrollRectPointerEnter()
        {
            Select();
        }

        private void OnScrollEnd()
        {
            Select();
        }

        private void OnScrollbarEndDrag()
        {
            Select();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!Application.isPlaying) return;

            if (!IsOpen)
            {
                Select();
                Show();
            }
            else
            {
                Hide();
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!Application.isPlaying) return;

            if (!IsOpen)
            {
                Show();
            }
        }

        public void OnCancel(BaseEventData eventData)
        {
            if (!Application.isPlaying) return;

            if (IsOpen)
            {
                Hide();
            }
        }

        public void Show()
        {
            if (!IsActive() || !IsInteractable())
                return;

            m_NewSelection = false;
            Select();

            if (IsOpen) return;
            IsOpen = true;

            if (m_AnimationHide.IsRunning)
                m_AnimationHide.Abort();

            m_AnimationShow.Execute(false);
            OnOpen?.Invoke();
        }

        public void Hide()
        {
            if (!IsOpen) return;

            IsOpen = false;

            if (m_AnimationShow.IsRunning)
                m_AnimationShow.Abort();

            m_AnimationHide.Execute(false);
            OnClose?.Invoke();
        }

        public void SetItemChanged(bool value, DropdownItemData data)
        {
            if (value)
            {
                EnableItem(data);
            }
            else
            {
                DisableItem(data);
            }

            RefreshCaption();
            OnValueChanged.Invoke(ItemsEnabled);
        }
    }
}
// © 2019 Ateo GmbH (https://www.ateo.ch)