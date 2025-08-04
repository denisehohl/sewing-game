using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Ateo.UI
{
	public abstract class ToggleAnimationBase : MonoBehaviour
	{
		[FormerlySerializedAs("Toggle"), Title("References"), SerializeField, Required]
		protected Toggle _toggle;

		protected void Awake() => Initialize();

		protected virtual void OnEnable()
		{
			if (_toggle == null)
			{
				_toggle = GetComponent<Toggle>();
			}

			if (_toggle == null || !Application.isPlaying) return;

			if (_toggle.IsOn)
			{
				EnableImmediate();
			}
			else
			{
				DisableImmediate();
			}

			_toggle.OnSetWithoutNotify += OnValueChanged;
			_toggle.OnSetWithoutNotifyImmediate += OnValueChangedImmediate;
		}

		private void OnDisable()
		{
			if (_toggle == null || !Application.isPlaying) return;

			_toggle.OnSetWithoutNotify -= OnValueChanged;
			_toggle.OnSetWithoutNotifyImmediate -= OnValueChangedImmediate;
		}

		private void OnValueChanged(bool value)
		{
			if (value)
			{
				Enable();
			}
			else
			{
				Disable();
			}
		}

		private void OnValueChangedImmediate(bool isOn)
		{
			if (isOn)
			{
				EnableImmediate();
			}
			else
			{
				DisableImmediate();
			}
		}

		protected abstract void Initialize();
		protected abstract void Enable();
		protected abstract void EnableImmediate();
		protected abstract void Disable();
		protected abstract void DisableImmediate();
	}
}