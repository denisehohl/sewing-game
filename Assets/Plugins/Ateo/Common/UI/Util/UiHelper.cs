using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ateo.Common.UI
{
	public static class UiHelper
	{
		public static bool IsCursorOverUserInterface(bool defaultValue = true)
		{
			EventSystem eventSystem = EventSystem.current;

			return eventSystem != null ?  eventSystem.IsPointerOverGameObject() : defaultValue;
		}
		
		public static void RefreshLayoutGroupsImmediateAndRecursive(GameObject root)
		{
			foreach (LayoutGroup layoutGroup in root.GetComponentsInChildren<LayoutGroup>())
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
			}
		}
	}
}