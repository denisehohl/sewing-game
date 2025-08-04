using System;
using UnityEngine.EventSystems;

namespace Ateo.Common.UI
{
	public interface ITooltip
	{
		event Action OnShown;
		event Action OnHidden;
		
		bool IsEnabled { get; }
		bool IsActive { get; }
		bool IsVisible { get; }
		float Delay { get; set; }
		TooltipPositionUpdateBehaviour UpdateBehaviour { get; set; }

		void Enable();
		void Disable();
		void Show(PointerEventData eventData);
		void Hide();
	}

	public interface ITooltip<in T> : ITooltip
	{
		void Style(T data);
	}
}