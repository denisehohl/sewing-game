// Based on UnityEngine.UI.Toggle

using UnityEngine;
using Ateo.Events;
using Ateo.Extensions;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Ateo.UI
{
	[RequireComponent(typeof(Button))]
	public class Toggle : MonoBehaviour
	{
		[SerializeField, FormerlySerializedAs("IsOn"),]
		private bool _isOn;

		[SerializeField, PropertySpace(0f, 8f)]
		private ToggleGroup _group;

#if EVENT2
		[BoxGroup("Events"), SerializeField, FormerlySerializedAs("OnValueChanged"), PropertySpace(8f, 0f)]
		private Event2Bool _onValueChanged;

		[BoxGroup("Events"), SerializeField, PropertySpace(8f, 0f)]
		private Event2Void _onEnabled;

		[BoxGroup("Events"), SerializeField, PropertySpace(8f, 0f)]
		private Event2Void _onDisabled;
#else
        [BoxGroup("Events"), SerializeField, FormerlySerializedAs("OnValueChanged"), PropertySpace(8f, 0f)]
        private EventBool _onValueChanged;

        [BoxGroup("Events"), SerializeField, PropertySpace(8f, 0f)]
        private EventVoid _onEnabled;

        [BoxGroup("Events"), SerializeField, PropertySpace(8f, 0f)]
        private EventVoid _onDisabled;
#endif

		private Button _button;

		public delegate void ToggleHandler(bool isOn);

		public event ToggleHandler OnSetWithoutNotify;
		public event ToggleHandler OnSetWithoutNotifyImmediate;

		#region Properties

		public bool IsOn => _isOn;

		public Button Button => _button;

		public ToggleGroup ToggleGroup
		{
			get => _group;
			set => SetToggleGroup(value);
		}

#if EVENT2
		public Event2Bool OnValueChanged => _onValueChanged;
		public Event2Void OnEnabled => _onEnabled;
		public Event2Void OnDisabled => _onDisabled;
#else
        public EventBool OnValueChanged => _onValueChanged;
		public EventVoid OnEnabled => _onEnabled;
		public EventVoid OnDisabled => _onDisabled;
#endif

		#endregion

		#region MonoBehvaiour Callbacks

		private void OnEnable()
		{
			_button = gameObject.GetOrAddComponent<Button>();
			_button.onClick.AddListener(OnPointerClick);

			SetToggleGroup(_group);
		}

		private void OnDisable()
		{
			_button.onClick.RemoveListener(OnPointerClick);

			SetToggleGroup(null);
		}

		private void OnDestroy()
		{
			if (_group != null)
				_group.EnsureValidState();
		}

		#endregion

		#region Public Methods

		public void SetValue(bool isOn, bool sendCallback = true, bool immediate = false)
		{
			if (_isOn == isOn) return;

			_isOn = isOn;

			if (_group != null /*&& _group.isActiveAndEnabled && isActiveAndEnabled*/)
			{
				if (_isOn || (!_group.AnyTogglesOn() && !_group.AllowSwitchOff))
				{
					_isOn = true;
					_group.NotifyToggleOn(this, sendCallback, immediate);
				}
			}

			if (sendCallback)
			{
				_onValueChanged.Invoke(_isOn);

				if (_isOn)
					_onEnabled.Invoke();
				else
					_onDisabled.Invoke();
			}

			if (!immediate)
			{
				OnSetWithoutNotify?.Invoke(_isOn);
			}
			else
			{
				OnSetWithoutNotifyImmediate?.Invoke(_isOn);
			}
		}

		public void ToggleValue()
		{
			if (_group != null && _group.ActiveToggle == this)
				return;

			SetValue(!_isOn);
		}

		#endregion

		#region Button

		private void OnPointerClick()
		{
			ToggleValue();
		}

		#endregion

		#region Toggle Group

		private void SetToggleGroup(ToggleGroup newGroup)
		{
			if (_group != null)
				_group.UnregisterToggle(this);

			if (newGroup != null)
				_group = newGroup;

			if (newGroup != null && isActiveAndEnabled)
				newGroup.RegisterToggle(this);

			if (newGroup != null && _isOn && isActiveAndEnabled)
				newGroup.NotifyToggleOn(this);
		}

		#endregion
	}
}