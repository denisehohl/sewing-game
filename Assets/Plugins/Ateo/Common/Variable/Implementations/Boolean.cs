using System;
using Sirenix.OdinInspector;

namespace Ateo.Common
{
	[Serializable, InlineProperty]
	public sealed class Boolean : Variable<bool>
	{
		#region Constructors

		/// <summary>
		/// Initializes and returns an instance of a Boolean.
		/// </summary>
		/// <param name="value">The default value for this Boolean</param>
		public Boolean(bool value = default) : base(value)
		{
		}

		#endregion
		
		#region Events

		/// <summary>
		/// Gets invoked when the <see cref="Variable{T}.Value"/> changes to True
		/// </summary>
		public event Action OnTrue;

		/// <summary>
		/// Gets invoked when the <see cref="Variable{T}.Value"/> changes to False
		/// </summary>
		public event Action OnFalse;

		#endregion

		#region Variable{T} Overrides

		/// <summary>
		/// <inheritdoc cref="Variable{T}.InvokeOnValueChanged"/>
		/// </summary>
		public override void InvokeOnValueChanged()
		{
			base.InvokeOnValueChanged();

			if (_value)
			{
				OnTrue?.Invoke();
			}
			else
			{
				OnFalse?.Invoke();
			}
		}

		#endregion

		#region Setters

		/// <summary>
		/// Sets the value for this Boolean to True.
		/// </summary>
		public void SetValueToTrue()
		{
			SetValue(true);
		}

		/// <summary>
		/// Sets the value for this Boolean to False.
		/// </summary>
		public void SetValueToFalse()
		{
			SetValue(false);
		}

		#endregion

		#region Callbacks

		/// <summary>
		/// Add a callback to be invoked when the value of this variable changes
		/// </summary>
		/// <param name="callback">The callback to be invoked</param>
		/// <returns>The instance of this <see cref="Boolean"/></returns>
		public new Boolean AddCallback(Action callback)
		{
			OnValueChangedVoid -= callback;
			OnValueChangedVoid += callback;
			return this;
		}

		/// <summary>
		/// Add a callback to be invoked when the value of this variable changes
		/// </summary>
		/// <param name="callback">The callback to be invoked</param>
		/// <returns>The instance of this <see cref="Boolean"/></returns>
		public new Boolean AddCallback(Action<bool> callback)
		{
			OnValueChanged -= callback;
			OnValueChanged += callback;
			return this;
		}

		/// <summary>
		/// Remove a callback
		/// </summary>
		/// <param name="callback">The callback to be removed</param>
		/// <returns>The instance of this <see cref="Boolean"/></returns>
		public new Boolean RemoveCallback(Action callback)
		{
			OnValueChangedVoid -= callback;
			return this;
		}

		/// <summary>
		/// Remove a callback
		/// </summary>
		/// <param name="callback">The callback to be removed</param>
		/// <returns>The instance of this <see cref="Boolean"/></returns>
		public new Boolean RemoveCallback(Action<bool> callback)
		{
			OnValueChanged -= callback;
			return this;
		}

		/// <summary>
		/// Add a callback to be invoked when the value of this variable changes to True
		/// </summary>
		/// <param name="callback">The callback to be invoked</param>
		/// <returns>The instance of this <see cref="Boolean"/></returns>
		public Boolean AddCallbackOnTrue(Action callback)
		{
			OnTrue -= callback;
			OnTrue += callback;
			return this;
		}

		/// <summary>
		/// Add a callback to be invoked when the value of this variable changes to False
		/// </summary>
		/// <param name="callback">The callback to be invoked</param>
		/// <returns>The instance of this <see cref="Boolean"/></returns>
		public Boolean AddCallbackOnFalse(Action callback)
		{
			OnFalse -= callback;
			OnFalse += callback;
			return this;
		}

		/// <summary>
		/// Remove a callback from OnTrue
		/// </summary>
		/// <param name="callback">The callback to be removed</param>
		/// <returns>The instance of this <see cref="Boolean"/></returns>
		public Boolean RemoveCallbackOnTrue(Action callback)
		{
			OnTrue -= callback;
			return this;
		}

		/// <summary>
		/// Remove a callback from OnFalse
		/// </summary>
		/// <param name="callback">The callback to be removed</param>
		/// <returns>The instance of this <see cref="Boolean"/></returns>
		public Boolean RemoveCallbackOnFalse(Action callback)
		{
			OnFalse -= callback;
			return this;
		}

		#endregion

		#region Processor

		/// <summary>
		/// The processor function processes the value passed to <see cref="Variable{T}.SetValue"/> before it is set. 
		/// </summary>
		/// <param name="preprocessor">Takes two parameters of type T. The first one receives the value of <see cref="Variable{T}.Value"/>,
		/// the second one receives the value passed to <see cref="Variable{T}.SetValue"/>. Returns the processed value.</param>
		/// <returns>The instance of this <see cref="Boolean"/></returns>
		public new Boolean SetProcessor(Func<bool, bool, bool> preprocessor)
		{
			return (Boolean) base.SetProcessor(preprocessor);
		}

		#endregion

		#region Backing Variable

		/// <summary>
		/// Sets the backing variable.
		/// A backing variable can be configured to automatically mirror the <see cref="Variable{T}.Value"/> of this variable.
		/// You can also manually update the value by calling <see cref="Variable{T}.UpdateBackingVariable"/>
		/// </summary>
		/// <param name="backingVariable">Instance of a <see cref="Variable{T}"/> which will be configured as a backing variable</param>
		/// <param name="autoUpdate">Automatically updates the value of the <paramref name="backingVariable"/>, whenever the value of this instance changes</param>
		/// <returns>The instance of this <see cref="Boolean"/></returns>
		public new Boolean SetBackingVariable(Variable<bool> backingVariable, bool autoUpdate = true)
		{
			return (Boolean) base.SetBackingVariable(backingVariable, autoUpdate);
		}

		/// <summary>
		/// When enabled and the <see cref="Variable{T}.Value"/> of this instance changes, the value of the <see cref="Variable{T}.BackingVariable"/> will change to the same value.
		/// </summary>
		/// <param name="enable">Defines whether to enable or disable the automatic updating of the <see cref="Variable{T}.BackingVariable"/></param>
		/// <returns>The instance of this <see cref="Boolean"/></returns>
		public new Boolean SetAutoUpdateBackingVariable(bool enable)
		{
			return (Boolean) base.SetAutoUpdateBackingVariable(enable);
		}

		#endregion
	}
}