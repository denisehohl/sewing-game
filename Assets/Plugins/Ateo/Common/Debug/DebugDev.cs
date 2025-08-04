using System.Diagnostics;

public static class DebugDev
{
    [Conditional("DEBUG_DEV")]
    public static void Log(object message) => UnityEngine.Debug.Log(message);

    [Conditional("DEBUG_DEV")]
    public static void Log(object message, UnityEngine.Object context) => UnityEngine.Debug.Log(message, context);

    [Conditional("DEBUG_DEV_WARNING")]
    public static void LogWarning(object message) => UnityEngine.Debug.LogWarning(message);

    [Conditional("DEBUG_DEV_WARNING")]
    public static void LogWarning(object message, UnityEngine.Object context) => UnityEngine.Debug.LogWarning(message, context);

    [Conditional("DEBUG_DEV_ERROR")]
    public static void LogError(object message) => UnityEngine.Debug.LogError(message);

    [Conditional("DEBUG_DEV_ERROR")]
    public static void LogError(object message, UnityEngine.Object context) => UnityEngine.Debug.LogError(message, context);
}