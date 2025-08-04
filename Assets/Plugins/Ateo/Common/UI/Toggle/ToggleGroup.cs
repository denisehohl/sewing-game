// Based on UnityEngine.UI.ToggleGroup

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ateo.UI
{
	public sealed class ToggleGroup : MonoBehaviour
	{
		[SerializeField]
		private bool _allowSwitchOff = false;

		public bool AllowSwitchOff
		{
			get => _allowSwitchOff;
			set => _allowSwitchOff = value;
		}

		private readonly List<Toggle> _toggles = new List<Toggle>();
		private readonly List<Toggle> _activeToggles = new List<Toggle>();
		private Toggle _activeToggle;

		public List<Toggle> Toggles => _toggles;
		public List<Toggle> ActiveToggles => _activeToggles;
		public Toggle ActiveToggle => _activeToggle;

		private void Start()
		{
			EnsureValidState(false, true);
		}

		private void OnEnable()
		{
			EnsureValidState(false, true);
		}

		private void ValidateToggleIsInGroup(Toggle toggle)
		{
			if (toggle == null || !_toggles.Contains(toggle))
				throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}", new object[] {toggle, this}));
		}

		public void NotifyToggleOn(Toggle toggle, bool sendCallback = true, bool immediate = false)
		{
			ValidateToggleIsInGroup(toggle);

			_activeToggle = toggle;

			foreach (Toggle t in _toggles)
			{
				if (t == toggle)
					continue;

				t.SetValue(false, sendCallback, immediate);
			}
		}

		public void RegisterToggle(Toggle toggle)
		{
			if (!_toggles.Contains(toggle))
				_toggles.Add(toggle);
		}

		public void UnregisterToggle(Toggle toggle)
		{
			if (_toggles.Contains(toggle))
				_toggles.Remove(toggle);
		}

		public void EnsureValidState(bool sendCallback = true, bool immediate = false)
		{
			if (!AllowSwitchOff && !AnyTogglesOn() && _toggles.Count != 0)
			{
				_toggles[0].SetValue(false, sendCallback, immediate);
				NotifyToggleOn(_toggles[0], sendCallback, immediate);
			}

			if (GetActiveToggles(_activeToggles) <= 1) return;

			_activeToggle = GetFirstActiveToggle();

			foreach (Toggle toggle in _activeToggles)
			{
				if (toggle == _activeToggle)
				{
					continue;
				}

				toggle.SetValue(false);
			}
		}

		public bool AnyTogglesOn()
		{
			foreach (Toggle t in _toggles)
			{
				if (t.IsOn) return true;
			}

			return false;
		}

		public int GetActiveToggles(List<Toggle> list)
		{
			list.Clear();

			int count = 0;

			foreach (Toggle t in _toggles)
			{
				if (t.IsOn)
				{
					list.Add(t);
					count++;
				}
			}

			return count;
		}

		public Toggle GetFirstActiveToggle()
		{
			foreach (Toggle t in _toggles)
			{
				if (t.IsOn)
				{
					return t;
				}
			}

			return null;
		}

		public void SetAllTogglesOff(bool sendCallback = true, bool immediate = false)
		{
			bool oldAllowSwitchOff = _allowSwitchOff;

			_allowSwitchOff = true;

			foreach (Toggle t in _toggles)
				t.SetValue(false, sendCallback, immediate);

			_allowSwitchOff = oldAllowSwitchOff;
			_activeToggle = null;
		}
	}
}