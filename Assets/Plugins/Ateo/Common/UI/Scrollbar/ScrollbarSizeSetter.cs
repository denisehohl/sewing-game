using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Ateo.UI
{
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollbarSizeSetter : MonoBehaviour
	{
		[SerializeField]
		private float m_ScrollbarSize = 0.25f;

		[SerializeField, Required, ChildGameObjectsOnly]
		private Scrollbar m_Scrollbar;

		[System.NonSerialized]
		private ScrollRect m_ScrollRect;

		private void Awake()
		{
			m_ScrollRect = GetComponent<ScrollRect>();

			if (m_ScrollRect != null)
				m_ScrollRect.onValueChanged.AddListener(SetScrollbarSize);
		}

		private void OnDestroy()
		{
			if (m_ScrollRect != null)
				m_ScrollRect.onValueChanged.RemoveListener(SetScrollbarSize);
		}

		private void OnEnable()
		{
			SetScrollbarSize();
		}

		private void Update()
		{
			SetScrollbarSize();
		}

		private void SetScrollbarSize(Vector2 value)
		{
			SetScrollbarSize();
		}

		private void SetScrollbarSize()
		{
			m_Scrollbar.size = m_ScrollbarSize;
		}
	}
}