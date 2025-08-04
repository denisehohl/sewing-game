using UnityEngine;
using UnityEngine.Events;

namespace Ateo.Events
{
    public delegate void DelegateVoid();
    public delegate void DelegateInt(int value);
    public delegate void DelegateFloat(float value);
    public delegate void DelegateBool(bool value);
    public delegate void DelegateString(string value);
    public delegate void DelegateObject(object value);

    [System.Serializable]
    public class EventVoid : UnityEvent
    {
    }

    [System.Serializable]
    public class EventInt : UnityEvent<int>
    {
    }

    [System.Serializable]
    public class EventFloat : UnityEvent<float>
    {
    }

    [System.Serializable]
    public class EventBool : UnityEvent<bool>
    {
    }

    [System.Serializable]
    public class EventString : UnityEvent<string>
    {
    }

    [System.Serializable]
    public class EventVector2 : UnityEvent<Vector2>
    {
    }

    [System.Serializable]
    public class EventVector3 : UnityEvent<Vector3>
    {
    }
}
// © 2019 Ateo GmbH (https://www.ateo.ch)
