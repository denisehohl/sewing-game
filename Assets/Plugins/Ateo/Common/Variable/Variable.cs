using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Ateo.Common.Util;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using UnityEngine;

namespace Ateo.Common
{
	/// <summary>
	/// A generic implementation of <see cref="IVariable{T}"/>.
	/// </summary>
	/// <typeparam name="T">Type of value.</typeparam>
	[Serializable, InlineProperty]
	public class Variable<T> : IVariable<T>, IEquatable<Variable<T>>
	{
		#region Fields

		[HorizontalGroup("Variable"), ShowInInspector, NonSerialized, HideLabel, HideReferenceObjectPicker,
		 OnValueChanged(nameof(InspectorValueChanged), IncludeChildren = true), EnableIf(nameof(InspectorEnableIf)),
		 ShowIf(nameof(InspectorShowIf))]
		protected T _inspectorValue;

		[SerializeField, HideInInspector]
		protected T _value;

		[NonSerialized]
		protected T _previousValue;

		[NonSerialized]
		protected T _defaultValue;

		[NonSerialized]
		protected bool _isInitialized;

		[NonSerialized]
		protected int _createdFrame = -1;

		[NonSerialized]
		protected int _valueChangedFrame = -1;

		[NonSerialized]
		protected float _valueChangedTime = -1;

		#endregion

		#region Constructors

		private Variable()
		{
		}

		/// <summary>
		/// Initializes and returns an instance of the Variable.
		/// </summary>
		/// <param name="value">The default value for this key.</param>
		public Variable(T value = default)
		{
			_value = value;
			_defaultValue = JsonHelper.Copy(_value);
#if UNITY_EDITOR
			_inspectorValue = JsonHelper.Copy(value);
#endif
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the currently stored value.
		/// </summary>
		[JsonIgnore]
		public virtual T Value
		{
			get => GetValue();
			set => SetValue(value);
		}

		/// <summary>
		/// Gets the default value for this variable.
		/// </summary>
		[JsonIgnore]
		public virtual T PreviousValue
		{
			get => _previousValue;
			set => _previousValue = value;
		}

		/// <summary>
		/// Gets the default value for this variable.
		/// </summary>
		public virtual T DefaultValue => JsonHelper.Copy(_defaultValue);

		/// <summary>
		/// Gets whether this variable is initialized.
		/// </summary>
		[JsonIgnore]
		public bool IsInitialized
		{
			get => _isInitialized;
			protected set => _isInitialized = value;
		}

		#endregion

		#region Protected Methods

		protected virtual void Initialize()
		{
			if (IsInitialized) return;

			_createdFrame = _valueChangedFrame = Time.frameCount;
			_valueChangedTime = Time.time;

			IsInitialized = true;
		}

		#endregion

		#region IVariable

		/// <summary>
		/// Gets invoked when the <see cref="Value"/> changes
		/// </summary>
		public event Action OnValueChangedVoid;

		/// <summary>
		/// Gets invoked when the <see cref="Value"/> changes
		/// </summary>
		public event Action<T> OnValueChanged;

		/// <summary>
		/// Gets invoked before the <see cref="Value"/> changes
		/// </summary>
		public event Action OnBeforeValueChangedVoid;

		/// <summary>
		/// Gets invoked before the <see cref="Value"/> changes.
		/// The first value is the current value, the second value is the new value.
		/// </summary>
		public event Action<T, T> OnBeforeValueChanged;

		/// <summary>
		/// Invokes the event OnValueChanged.
		/// </summary>
		/// <seealso cref="IVariable{T}.InvokeOnValueChanged"/>
		public virtual void InvokeOnValueChanged()
		{
			OnValueChanged?.Invoke(Value);
			OnValueChangedVoid?.Invoke();
		}

		/// <summary>
		/// Invokes the event OnValueChanged.
		/// </summary>
		/// <seealso cref="IVariable{T}.InvokeOnBeforeValueChanged"/>
		public virtual void InvokeOnBeforeValueChanged(T newValue)
		{
			OnBeforeValueChanged?.Invoke(Value, newValue);
			OnBeforeValueChangedVoid?.Invoke();
		}

		/// <summary>
		/// Returns a copy of the default value.
		/// </summary>
		/// <returns>
		/// The default value.
		/// </returns>
		/// <seealso cref="IVariable{T}.GetDefaultValue" />
		public virtual T GetDefaultValue()
		{
			return DefaultValue;
		}

		/// <summary>
		/// Returns the the previous value for this variable.
		/// </summary>
		/// <returns>
		/// The previous value for this variable.
		/// </returns>
		/// /// <seealso cref="IVariable{T}.GetPreviousValue" />
		public T GetPreviousValue()
		{
			return PreviousValue;
		}

		/// <summary>
		/// Returns the currently stored value.
		/// </summary>
		/// <returns>
		/// The value that is currently set.
		/// </returns>
		/// <seealso cref="IVariable{T}.GetValue" />
		public virtual T GetValue()
		{
			Initialize();
			return _value;
		}

		/// <summary>
		/// Sets the value for this Variable.
		/// </summary>
		/// <param name="value">The new value to set.</param>
		/// <seealso cref="IVariable{T}.SetValue(T)" />
		public virtual void SetValue(T value) => SetValue(value, true, false);

		/// <summary>
		/// Sets the value for this variable.
		/// </summary>
		/// <param name="value">The new value.</param>
		/// <param name="triggerCallbacks">Will invoke callbacks if true</param>
		/// <param name="force">If set to true, callbacks will be invoked even if the new and old value are identical.</param>
		public virtual void SetValue(T value, bool triggerCallbacks, bool force)
		{
			Initialize();

			if (_processor != null)
			{
				value = _processor.Invoke(_value, value);
			}

			if (EqualityComparer<T>.Default.Equals(_value, value) && !force)
			{
				return;
			}

			if (triggerCallbacks)
			{
				InvokeOnBeforeValueChanged(value);
			}

			PreviousValue = _value;
			_value = value;

#if UNITY_EDITOR
			_inspectorValue = JsonHelper.Copy(value);
#endif

			_valueChangedFrame = Time.frameCount;
			_valueChangedTime = Time.time;

			if (triggerCallbacks)
			{
				InvokeOnValueChanged();
			}
		}

		/// <summary>
		/// Sets the value back to the <see cref="DefaultValue"/>.
		/// </summary>
		/// <seealso cref="IVariable.SetValueToDefault()"/>
		public virtual void SetValueToDefault() => SetValueToDefault(false, false);

		/// <summary>
		/// Sets the current value back to to the <see cref="DefaultValue"/>.
		/// </summary>
		/// <param name="triggerCallbacks">Will invoke callbacks if true</param>
		/// <param name="force">If set to true, callbacks will be invoked even if the new and old value are identical.</param>
		/// <seealso cref="IVariable.SetValueToDefault(bool,bool)"/>
		public virtual void SetValueToDefault(bool triggerCallbacks, bool force)
		{
			Initialize();
			SetValue(DefaultValue, triggerCallbacks, force);
		}

		/// <summary>
		/// Resets the object to its default state.
		/// The value will be reset to its default and all other internal values will be reset as well as all callbacks set to null.
		/// </summary>
		public virtual void Reset()
		{
			SetValueToDefault(false, true);
		}

		/// <summary>
		/// Return if the was created this frame.
		/// </summary>
		/// <returns>
		/// Indicates whether the variable was created this frame.
		/// </returns>
		public bool WasCreatedThisFrame()
		{
			return Time.frameCount == _createdFrame;
		}

		public bool HasChangedThisFrame()
		{
			return Time.frameCount == _valueChangedFrame;
		}

		public bool HasChangedLastFrame()
		{
			return Time.frameCount == _valueChangedFrame + 1;
		}

		public bool HasChangedThisOrLastFrame()
		{
			return HasChangedThisFrame() || HasChangedLastFrame();
		}

		public float GetChangedTime()
		{
			return _valueChangedTime;
		}

		#endregion

		#region Callbacks

		/// <summary>
		/// Add a callback to be invoked when the value of this variable changes
		/// </summary>
		/// <param name="callback">The callback to be invoked</param>
		/// <returns>The instance of this <see cref="Variable{T}"/></returns>
		public Variable<T> AddCallback(Action callback)
		{
			OnValueChangedVoid -= callback;
			OnValueChangedVoid += callback;
			return this;
		}

		/// <summary>
		/// Add a callback to be invoked when the value of this variable changes
		/// </summary>
		/// <param name="callback">The callback to be invoked</param>
		/// <returns>The instance of this <see cref="Variable{T}"/></returns>
		public Variable<T> AddCallback(Action<T> callback)
		{
			OnValueChanged -= callback;
			OnValueChanged += callback;
			return this;
		}

		/// <summary>
		/// Remove a callback
		/// </summary>
		/// <param name="callback">The callback to be removed</param>
		/// <returns>The instance of this <see cref="Variable{T}"/></returns>
		public Variable<T> RemoveCallback(Action callback)
		{
			OnValueChangedVoid -= callback;
			return this;
		}

		/// <summary>
		/// Remove a callback
		/// </summary>
		/// <param name="callback">The callback to be removed</param>
		/// <returns>The instance of this <see cref="Variable{T}"/></returns>
		public Variable<T> RemoveCallback(Action<T> callback)
		{
			OnValueChanged -= callback;
			return this;
		}
		
		/// <summary>
		/// Clear all callbacks.
		/// </summary>
		public virtual void ClearCallbacks()
		{
			OnValueChangedVoid = null;
			OnValueChanged = null;
			OnBeforeValueChangedVoid = null;
			OnBeforeValueChanged = null;
		}

		#endregion

		#region Processor

		#region Fields

		[NonSerialized]
		protected Func<T, T, T> _processor;

		#endregion

		#region Properties

		public Func<T, T, T> Processor => _processor;

		#endregion

		#region Methods

		/// <summary>
		/// The processor function processes the value passed to <see cref="SetValue(T)"/> before it is set. 
		/// </summary>
		/// <param name="preprocessor">Takes two parameters of type T. The first one receives the value of <see cref="Value"/>,
		/// the second one receives the value passed to <see cref="SetValue(T)"/>. Returns the processed value.</param>
		/// <returns>The instance of this <see cref="Variable{T}"/></returns>
		public Variable<T> SetProcessor(Func<T, T, T> preprocessor)
		{
			_processor = preprocessor;
			return this;
		}

		#endregion

		#endregion

		#region Backing Variable

		#region Fields

		[NonSerialized]
		protected Variable<T> _backingVariable;

		[NonSerialized]
		protected bool _autoUpdateBackingVariable;

		#endregion

		#region Properties

		/// <summary>
		/// The backing variable. It can be configured to automatically mirror the <see cref="Value"/> of this variable.
		/// </summary>
		/// <seealso cref="SetBackingVariable"/>
		[JsonIgnore]
		public Variable<T> BackingVariable => _backingVariable;

		/// <summary>
		/// Automatically update value of <see cref="BackingVariable"/> whenever <see cref="Value"/> changes
		/// </summary>
		/// <seealso cref="SetAutoUpdateBackingVariable"/>
		[JsonIgnore]
		public bool AutoUpdateBackingVariable => _autoUpdateBackingVariable;

		#endregion

		#region Methods

		/// <summary>
		/// Sets the backing variable.
		/// A backing variable can be configured to automatically mirror the <see cref="Value"/> of this variable.
		/// You can also manually update the value by calling <see cref="Variable{T}.UpdateBackingVariable"/>
		/// </summary>
		/// <param name="backingVariable">Instance of a <see cref="Variable{T}"/> which will be configured as a backing variable</param>
		/// <param name="autoUpdate">Automatically updates the value of the <paramref name="backingVariable"/>, whenever the value of this instance changes</param>
		/// <returns>The instance of this <see cref="Variable{T}"/></returns>
		public Variable<T> SetBackingVariable(Variable<T> backingVariable, bool autoUpdate = true)
		{
			SetAutoUpdateBackingVariable(false); // Removes event subscriptions
			_backingVariable = backingVariable;
			return SetAutoUpdateBackingVariable(autoUpdate);
		}

		/// <summary>
		/// When enabled and the <see cref="Value"/> of this instance changes, the value of the <see cref="BackingVariable"/> will change to the same value.
		/// </summary>
		/// <param name="enable">Defines whether to enable or disable the automatic updating of the <see cref="BackingVariable"/></param>
		/// <returns></returns>
		public Variable<T> SetAutoUpdateBackingVariable(bool enable)
		{
			_autoUpdateBackingVariable = enable;

			if (_backingVariable != null)
			{
				if (_autoUpdateBackingVariable)
				{
					OnValueChanged += _backingVariable.SetValue;
					_backingVariable.SetValue(GetValue());
				}
				else
				{
					OnValueChanged -= _backingVariable.SetValue;
				}
			}

			return this;
		}

		/// <summary>
		/// Update the value of the <see cref="BackingVariable"/> to match the value of this <see cref="Variable{T}.Value"/>
		/// </summary>
		public void UpdateBackingVariable()
		{
			_backingVariable.SetValue(_value);
		}

		#endregion

		#endregion

		#region Inspector

		/// <summary>
		/// This method gets executed exclusively by Odin Inspector when the <see cref="_value"/> gets changed in the Inspector.
		/// Don't call this method yourself.
		/// </summary>
		/// <param name="value"></param>
		protected virtual void InspectorValueChanged(T value)
		{
			SetValue(value);
		}

		// -->	This war removed because OnValueChanged(IncludeChildren = true) attribute provides the desired functionality.
		//		In addition, the code below turned out to be called with every inspector update for reference type variables,
		//		which in turn called SetValue. This was highly undesirable. I leave this code here just in case.
		//		Attribute to add to _inspectorValue --> ValidateInput(nameof(InspectorValidateValue))
		
		/*/// <summary>
		/// This method gets executed exclusively by Odin Inspector before the <see cref="_value"/> gets changed in the Inspector.
		/// Don't call this method yourself.
		/// </summary>
		/// <param name="value">The value that the user entered</param>
		/// <param name="errorMessage">The error message to display in the inspector if the value is not valid</param>
		protected virtual bool InspectorValidateValue(T value, ref string errorMessage)
		{
			// This could potentially be dangerous. Needs testing.
			// Reason for inclusion: Changes to serialized classes don't invoke the "OnValueChanged" attribute callback,
			// but they invoke this callback. Therefore we misuse this callback to call SetValue if any of the values
			// of the serialized class has changed.
			
			if (EqualityComparer<T>.Default.Equals(_inspectorValue, default))
			{
				_inspectorValue = JsonHelper.Copy(GetValue());
			}
			else if (!EqualityComparer<T>.Default.Equals(_inspectorValue, _value))
			{
				SetValue(JsonHelper.Copy(_inspectorValue));
			}
			else
			{
				_inspectorValue = JsonHelper.Copy(GetValue());
			}
            
			return true;
		}*/

		/// <summary>
		/// This method gets executed exclusively by Odin Inspector. Don't call this method yourself.
		/// </summary>
		protected virtual bool InspectorEnableIf()
		{
			return true;
		}

		/// <summary>
		/// This method gets executed exclusively by Odin Inspector. Don't call this method yourself.
		/// </summary>
		protected virtual bool InspectorShowIf()
		{
			return true;
		}

		#endregion

		#region Operators

		/// <summary>
		/// Implicit casts this variable to the backing type `T`.
		/// </summary>
		/// <param name="variable">The Variable to cast to `T`.</param>
		/// <returns>
		/// The currently stored <see cref="Value"/>.
		/// </returns>
		public static implicit operator T(Variable<T> variable)
		{
			return variable.Value;
		}

		#endregion

		#region UnityEngine.Object Overrides

		/// <summary>
		/// Returns a string representation of this variable.
		/// </summary>
		/// <returns>A string summary of this variable</returns>
		public override string ToString()
		{
			if (default(T) == null)
			{
				return _value != null ? _value.ToString() : string.Empty;
			}

			return _value.ToString();
		}

		#endregion

		#region IEquatable

		public bool Equals(Variable<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return EqualityComparer<T>.Default.Equals(_value, other._value)
			       && EqualityComparer<T>.Default.Equals(_defaultValue, other._defaultValue)
			       && _valueChangedFrame == other._valueChangedFrame
			       && Equals(_backingVariable, other._backingVariable)
			       && EqualityComparer<T>.Default.Equals(PreviousValue, other.PreviousValue)
			       && Equals(_processor, other._processor)
			       && _autoUpdateBackingVariable == other._autoUpdateBackingVariable;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((Variable<T>) obj);
		}

		[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = EqualityComparer<T>.Default.GetHashCode(_value);
				hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(_defaultValue);
				hashCode = (hashCode * 397) ^ _valueChangedFrame;
				hashCode = (hashCode * 397) ^ (_backingVariable != null ? _backingVariable.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(PreviousValue);
				hashCode = (hashCode * 397) ^ (_processor != null ? _processor.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ _autoUpdateBackingVariable.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(Variable<T> left, Variable<T> right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Variable<T> left, Variable<T> right)
		{
			return !Equals(left, right);
		}

		#endregion
	}
}