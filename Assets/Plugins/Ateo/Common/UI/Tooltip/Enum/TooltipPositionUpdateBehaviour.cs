namespace Ateo.Common.UI
{
	public enum TooltipPositionUpdateBehaviour
	{
		Always = 0, // the tooltip is positioned when displayed and the position is updated when cursor changes
		WhenShown, // the tooltip is positioned once it is displayed (and not afterwards)
		Never // the tooltip object will not be re-positioned by mouse/tap position change
	}
}