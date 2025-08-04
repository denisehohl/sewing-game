// Make sure that the RectTransform Pivot Point is [0, 0], otherwise this script does not work

using Sirenix.OdinInspector;
using UnityEngine;

namespace Ateo.Common.Inputs
{
	public class CursorPosition : ComponentPublishBehaviour<CursorPosition>
	{
		[Required]
		public RectTransform RectTransform;

		public static Vector2 ViewPortPosition => _localPointNormalized;
		public static Vector2 ViewPortCenter => _centerPoint;
		public static Vector2 Position => _localPoint;

		private static Vector2 _localPoint;
		private static Vector2 _localPointNormalized;
		private static Vector2 _centerPoint;
		
		#region ComponentPublishBehaviour Callbacks
		
		public override void ResetStatics()
		{
			base.ResetStatics();
			_localPoint = Vector2.zero;
			_localPointNormalized = Vector2.zero;
		}

		#endregion

		private void Update()
		{
			Rect rect = RectTransform.rect;

			if (rect.size.magnitude <= 0f)
			{
				return;
			}
			
			_centerPoint = new Vector2(rect.size.x * 0.5f, rect.size.y * 0.5f);

			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, Input.mousePosition, null, out _localPoint))
			{
				_localPointNormalized = new Vector2(1f / rect.size.x * _localPoint.x, 1f / rect.size.y * _localPoint.y);
			}
		}
	}
}