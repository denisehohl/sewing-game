using UnityEngine;
using Ateo.Common;

namespace Ateo.StateManagement
{
    public class StateBackAndroid : ComponentPublishBehaviour<StateBackAndroid>
    {
#if UNITY_ANDROID && STATEMACHINE
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StateManager.Back();
            }
        }
#endif
    }
}