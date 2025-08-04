using System;
using Ateo.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace Ateo.UI
{
	public class DropDownScrollToSelected : MonoBehaviour
	{
		[Required, SerializeField, FormerlySerializedAs("m_DropDown")]
		private Dropdown _dropDown;

		private ScrollRect _scrollRect;
		private Transform _content;

		private void Awake()
		{
			_scrollRect = _dropDown.m_ScrollRect;
			_content = _scrollRect.content;
		}

		private void Start()
		{
			Invoke(nameof(ShowSelected), 0.01f);
		}

		private void OnEnable()
		{
			_dropDown.OnOpen += ShowSelected;
		}

		private void OnDisable()
		{
			_dropDown.OnOpen -= ShowSelected;
		}

		private void ShowSelected()
		{
			if (!gameObject.activeInHierarchy)
				return;

			if (_scrollRect == null || _dropDown == null) return;

			int index = -1;

			for (int i = 0; i < _dropDown.Items.Count; i++)
			{
				foreach (DropdownItemData t in _dropDown.ItemsEnabled)
				{
					if (_dropDown.Items[i] != t) continue;

					index = i;
					break;
				}

				if (index != -1)
					break;
			}

			if (index == -1)
				return;

			try
			{
				index = Mathf.Clamp(index + 1, 0, _content.childCount - 1);
				RectTransform child = _content.GetChild(index).GetComponent<RectTransform>();
				_content.localPosition = _scrollRect.GetSnapToPositionToBringChildIntoView(child);
			}
			catch (Exception e)
			{
				DebugDev.LogError(e);
				DebugDev.Log(index);
				DebugDev.Log(_content.childCount);
			}
		}
	}
}