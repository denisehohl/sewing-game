using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Ateo.StateManagement
{
    /// <summary>
    /// Provides functionality to change the ApplicationState.
    /// </summary>
    public class StateRequest : MonoBehaviour
    {
#if STATEMACHINE
        [ValueDropdown("GetStates")]
        public string State = "None";

        [ShowIf("HasState")]
        public bool RequestStateOnStart;

        [ShowIf("RequestStateOnStart")]
        public float RequestOnStartDelay;

        private void Start()
        {
            // Automatically change to a desired state
            if (RequestStateOnStart)
            {
                Invoke(nameof(RequestState), RequestOnStartDelay);
            }
        }

        /// <summary>
        /// Request to change to a different ApplicationState.
        /// Use this method in UnityEvents.
        /// </summary>
        public void RequestState()
        {
            StateManager.ChangeTo(StateHelper.GetState(State));
        }

        /// <summary>
        /// Request to go down by one level in the ApplicationState hierarchy.
        /// </summary>
        public void RequestNext()
        {
            StateManager.Next();
        }

        /// <summary>
        /// Request to go up by one level in the ApplicationState hierarchy.
        /// </summary>
        public void RequestBack()
        {
            StateManager.Back();
        }

        public void RequestQuit()
        {
            Application.Quit();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Use this method for testing in the Editor
        /// </summary>
        [Button, ShowIf("HasState")]
        private void ChangeState()
        {
            StateManager.ChangeTo(StateHelper.GetState(State));
        }

        private IEnumerable<string> GetStates()
        {
            return StateHelper.GetStateNames().Prepend("None");
        }
        private bool HasState()
        {
            return StateHelper.GetStatesEnum(State) != StatesEnum.None;
        }
#endif
#endif
    }
}