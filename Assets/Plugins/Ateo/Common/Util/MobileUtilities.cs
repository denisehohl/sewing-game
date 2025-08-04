// Source: https://forum.unity.com/threads/keyboard-height.291038/#post-4929095

using UnityEngine;
 
public static class MobileUtilities
{
    /// <summary>
    /// Returns the keyboard height ratio.
    /// </summary>
    public static float GetKeyboardHeightRatio(bool includeInput)
    {
        return Mathf.Clamp01((float) GetKeyboardHeight(includeInput) / Display.main.systemHeight);
    }
 
    /// <summary>
    /// Returns the keyboard height in display pixels.
    /// </summary>
    public static int GetKeyboardHeight(bool includeInput)
    {
#if UNITY_ANDROID
        using (AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject unityPlayer = unityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer");
            AndroidJavaObject view = unityPlayer.Call<AndroidJavaObject>("getView");
            AndroidJavaObject dialog = unityPlayer.Get<AndroidJavaObject>("b");
 
            if (view == null || dialog == null)
                return 0;
 
            int decorHeight = 0;
 
            if (includeInput)
            {
                AndroidJavaObject decorView = dialog.Call<AndroidJavaObject>("getWindow").Call<AndroidJavaObject>("getDecorView");
 
                if (decorView != null)
                    decorHeight = decorView.Call<int>("getHeight");
            }
 
            using (AndroidJavaObject rect = new AndroidJavaObject("android.graphics.Rect"))
            {
                view.Call("getWindowVisibleDisplayFrame", rect);
                return Display.main.systemHeight - rect.Call<int>("height") + decorHeight;
            }
        }
#else
        int height = Mathf.RoundToInt(TouchScreenKeyboard.area.height);
        return height >= Display.main.systemHeight ? 0 : height;
#endif
    }
}
