using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ateo.Common.UI
{
	public abstract class TooltipCaller<T, U> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler 
		where U: Tooltip<T>
	{
		#region Fields

		[BoxGroup("Tooltip Instance"), SerializeField, RequiredIn(PrefabKind.PrefabInstance)]
		protected U _tooltip;
		
		[BoxGroup("Data"), SerializeField]
		protected T _data;

		protected bool _isEntered;
		protected bool _preventTooltip;
		protected Coroutine _coroutine;

		#endregion
		
		#region MonoBehaviour Callbacks

		protected virtual void Awake()
		{
			if (_tooltip == null)
			{
				Destroy(this);
			}
		}

		#endregion

		#region Private Methods

		protected virtual void ShowTooltip(PointerEventData eventData)
		{
			if (_data != null)
			{
				_tooltip.Style(_data);
				_tooltip.Show(eventData);
			}
		}

		protected virtual IEnumerator DelayShowTooltip(PointerEventData eventData)
		{
			// check if tooltip display is prevented
			if (_preventTooltip) yield break;

			float delay = _tooltip.Delay;
			
			for (float time = 0.0f; time < delay; time += Time.deltaTime)
			{
				yield return null;
			}

			if (_isEntered)
			{
				ShowTooltip(eventData);
			}
		}

		#endregion

		#region Interface Implementations

		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			_isEntered = true;
			
			// starts tooltip delay counter coroutine
			_coroutine = StartCoroutine(DelayShowTooltip(eventData));
			eventData.Use();
		}

		public virtual void OnPointerExit(PointerEventData eventData)
		{
			_isEntered = false;
				
			// if delay coroutine is running, stop it
			if (_coroutine != null)
			{
				StopCoroutine(_coroutine);
			}
			
			// if tooltip is set, hide it
			_tooltip.Hide();
			
			// unset flag to prevent tooltip displayed after click
			_preventTooltip = false;
		}

		public virtual void OnPointerDown(PointerEventData eventData)
		{
			// call to pointer exit method as behaviour is identical
			OnPointerExit(eventData);
			
			// set flag to prevent tooltip displayed after click
			_preventTooltip = true;
		}

		public virtual void OnPointerUp(PointerEventData eventData)
		{
			if (!_isEntered) return;
			_coroutine = StartCoroutine(DelayShowTooltip(eventData));
		}

		#endregion
	}
}