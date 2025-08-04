#if (DOOZY_3 || DOOZY_4) && STATEMACHINE

using UnityEngine;
#if DOOZY_3
using Doozy.Engine.Nody;
#elif DOOZY_4
using Doozy.Runtime.Nody;
#endif

namespace Ateo.StateManagement
{
#if DOOZY_3
    [RequireComponent(typeof(GraphController))]
#elif DOOZY_4
	[RequireComponent(typeof(FlowController))]
#endif
	public class StateToDoozyNode : MonoBehaviour
	{
		[SerializeField]
		private StatesEnum _parentState;

#if DOOZY_3
		private GraphController _controller;
#elif DOOZY_4
		private FlowController _controller;
#endif

		private IState _parent;
		private bool _subscribed;

		private void Awake()
		{
			if (TryGetComponent(out _controller))
			{
				_subscribed = true;
				_parent = StateHelper.GetState(_parentState);
				StateManager.OnStateChanged += OnStateChanged;
			}
		}

		private void OnDestroy()
		{
			if (_subscribed)
			{
				_subscribed = false;
				StateManager.OnStateChanged -= OnStateChanged;
			}
		}

		private void OnStateChanged(StatesEnum current, StatesEnum previous)
		{
			IState state;

			if (_parentState == StatesEnum.None)
			{
				state = StateManager.Current.StateParent ?? StateManager.Current;
			}
			else
			{
				state = StateManager.Current;
			}

#if DOOZY_3
			string nodeName = _controller.Graph.ActiveNode != null
                ? _controller.Graph.ActiveNode.Name
                : string.Empty;
#elif DOOZY_4
			string nodeName = _controller.flow.activeNode != null
				? _controller.flow.activeNode.nodeName
				: string.Empty;
#endif

			if (state.StateParent == _parent)
			{
				if (!string.Equals(state.NodeName, nodeName))
				{
#if DOOZY_3
					_controller.GoToNodeByName(state.NodeName);
#elif DOOZY_4
					_controller.SetActiveNodeByName(state.NodeName);
#endif
				}
			}
		}
	}
}
#endif