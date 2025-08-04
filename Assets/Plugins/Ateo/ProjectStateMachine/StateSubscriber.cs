using UnityEngine;
using UnityEngine.Events;

namespace Ateo.StateManagement
{
    public class StateSubscriber : MonoBehaviour
    {
#if STATEMACHINE
        public StatesEnum State;

        public UnityEvent OnStateStarted;
        public UnityEvent OnStateEnded;
        
        private void Awake()
        {
            StateManager.OnStateChanged += OnStateChanged;
        }
        

        private void OnDestroy()
        {
            StateManager.OnStateChanged -= OnStateChanged;
        }
        
        private void OnStateChanged(StatesEnum state, StatesEnum previous)
        {
            if (state == State)
            {
                OnStateStarted?.Invoke();
            }
            else if (previous == State)
            {
                OnStateEnded?.Invoke();
            }
        }
#endif
    }
}