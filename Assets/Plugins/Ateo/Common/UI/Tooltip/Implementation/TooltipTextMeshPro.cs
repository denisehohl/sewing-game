using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Ateo.Common.UI
{
	public class TooltipTextMeshPro : Tooltip<string>
	{
		#region Fields
		
		[BoxGroup("References"), SerializeField]
		private List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>();

		#endregion

		#region Tooltip Override Methods

		public override void Style(string data)
		{
			string message = !string.IsNullOrEmpty(data) ? data : string.Empty;

			foreach (TextMeshProUGUI text in _texts)
			{
				if (text != null)
				{
					text.text = message;
				}
			}
		}

		#endregion
	}
}