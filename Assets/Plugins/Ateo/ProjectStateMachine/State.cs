using System;

namespace Ateo.StateManagement
{
	public interface IState
	{
		string Name { get; }
		string NodeName { get; }
		bool CanComplete { get; }
		IState StateParent { get; }
		IState StateNext { get; }
		IState StateBack { get; }
		Action OnStart { get; set; }
		Action OnEnd { get; set; }
		Action OnSubStateChanged { get; set; }
		void Start();
		void Complete();
		void NotifySubStateChanged();
	}

	public abstract class State<T> : IState where T : IState, new()
	{
		public static T Instance { get; protected set; } = new T();

		public virtual string Name => GetType().Name;
		public virtual string NodeName => Name;
		public virtual bool CanComplete => true;
		public abstract IState StateParent { get; }
		public abstract IState StateNext { get; }
		public abstract IState StateBack { get; }
		public Action OnStart { get; set; }
		public Action OnEnd { get; set; }
		public Action OnSubStateChanged { get; set; }

		public virtual void Start()
		{
			OnStart?.Invoke();
		}

		public virtual void Complete()
		{
			OnEnd?.Invoke();
		}

		public virtual void NotifySubStateChanged()
		{
			OnSubStateChanged?.Invoke();
		}
	}
}