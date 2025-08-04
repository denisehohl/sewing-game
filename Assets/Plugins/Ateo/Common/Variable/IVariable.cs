using System;

namespace Ateo.Common
{
	public interface IVariable
	{
		/// <summary>
		/// Implement this property to get the initialization status of the variable
		/// </summary>
		/// <value>Indicates whether the value of the variable has been initialized</value>
		bool IsInitialized { get; }

		/// <summary>
		/// Implement this event to invoke when the value of this variable changes
		/// </summary>
		event Action OnValueChangedVoid;
		
		/// <summary>
		/// Implement this event to invoke before the value of this variable changes.
		/// </summary>
		event Action OnBeforeValueChangedVoid;
		
		/// <summary>
		/// Implement this method to invoke the event OnValueChanged
		/// </summary>
		void InvokeOnValueChanged();
		
		/// <summary>
		/// Implement this method to set the value back to the default value.
		/// </summary>
		void SetValueToDefault();
		
		/// <summary>
		/// Implement this method to set the value back to the default value.
		/// </summary>
		/// <param name="triggerCallbacks">Will invoke callbacks if true</param>
		/// <param name="force">If set to true, callbacks will be invoked even if the new and old value are identical.</param>
		void SetValueToDefault(bool triggerCallbacks, bool force = false);
		
		/// <summary>
		/// Implement this method to clear all callbacks.
		/// </summary>
		void ClearCallbacks();
		
		/// <summary>
		/// Implement this method to return if the was created this frame
		/// </summary>
		/// <returns>
		/// Indicates whether the variable was created this frame.
		/// </returns>
		bool WasCreatedThisFrame();
		
		/// <summary>
		/// Implement this method to return if the value has changed this frame.
		/// </summary>
		/// <returns>
		/// Indicates whether the value has changed this frame.
		/// </returns>
		bool HasChangedThisFrame();

		/// <summary>
		/// Implement this method to return if the value has changed last frame.
		/// </summary>
		/// <returns>
		/// Indicates whether the value has changed last frame.
		/// </returns>
		bool HasChangedLastFrame();

		/// <summary>
		/// Implement this method to return if the value has changed this or last frame.
		/// </summary>
		/// <returns>
		/// Indicates whether the value has changed this or last frame.
		/// </returns>
		bool HasChangedThisOrLastFrame();

		/// <summary>
		/// Implement this method to return the time when the value was changed last.
		/// </summary>
		/// <returns>
		/// Returns the time the value was last changed
		/// </returns>
		float GetChangedTime();
	}

	public interface IVariable<T> : IVariable
	{
		/// <summary>
		/// Implement this event to invoke when the value of this variable changes
		/// </summary>
		event Action<T> OnValueChanged;

		/// <summary>
		/// Implement this event to invoke before the value of this variable changes.
		/// The first value is the current value, the second value is the new value.
		/// </summary>
		event Action<T, T> OnBeforeValueChanged;

		/// <summary>
		/// Implement this method to invoke the event OnBeforeValueChanged
		/// </summary>
		void InvokeOnBeforeValueChanged(T newValue);
		
		/// <summary>
		/// Implement this method to return the stored variable value.
		/// If you are implementing IVariable, you should cache this value.
		/// </summary>
		/// <returns>
		/// The stored value.
		/// </returns>
		T GetValue();

		/// <summary>
		/// Implement this method to return the the default value for this variable.
		/// </summary>
		/// <returns>
		/// The default value for this variable.
		/// </returns>
		T GetDefaultValue();
		
		/// <summary>
		/// Implement this method to return the the previous value for this variable.
		/// </summary>
		/// <returns>
		/// The previous value for this variable.
		/// </returns>
		T GetPreviousValue();

		/// <summary>
		/// Implement this method to set the value for this variable.
		/// </summary>
		/// <param name="value">The new value.</param>
		void SetValue(T value);

		/// <summary>
		/// Implement this method to set the value for this variable.
		/// </summary>
		/// <param name="value">The new value.</param>
		/// <param name="triggerCallbacks">Will invoke callbacks if true</param>
		/// <param name="force">If set to true, callbacks will be invoked even if the new and old value are identical.</param>
		void SetValue(T value, bool triggerCallbacks, bool force = false);

		/// <summary>
		/// Implement this method to add a callback to be invoked when the value of this variable changes
		/// </summary>
		/// <param name="callback">The callback to be invoked</param>
		/// <returns>The instance of this <see cref="Variable{T}"/></returns>
		Variable<T> AddCallback(Action callback);

		/// <summary>
		/// Implement this method to add a callback to be invoked when the value of this variable changes
		/// </summary>
		/// <param name="callback">The callback to be invoked</param>
		/// <returns>The instance of this <see cref="Variable{T}"/></returns>
		Variable<T> AddCallback(Action<T> callback);

		/// <summary>
		/// Implement this method to remove a callback
		/// </summary>
		/// <param name="callback">The callback to be removed</param>
		/// <returns>The instance of this <see cref="Variable{T}"/></returns>
		Variable<T> RemoveCallback(Action callback);

		/// <summary>
		/// Implement this method to remove a callback
		/// </summary>
		/// <param name="callback">The callback to be removed</param>
		/// <returns>The instance of this <see cref="Variable{T}"/></returns>
		Variable<T> RemoveCallback(Action<T> callback);
	}
}