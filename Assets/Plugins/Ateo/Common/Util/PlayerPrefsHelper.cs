using UnityEngine;
using Ateo.Common.Util;

namespace Ateo.Common
{
    public static class PlayerPrefsHelper
    {
        public static void Save<T>(string key, T value)
        {
            PlayerPrefs.SetString(key, JsonHelper.SerializeToString(value));
        }

        public static T Load<T>(string key)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                Debug.LogError("PlayerPrefsHelper: Key not found.");
                return default;
            }

            string json = PlayerPrefs.GetString(key);
            return JsonHelper.DeserializeFromString<T>(json);
        }

        public static bool TryLoad<T>(string key, out T value)
        {
            if (PlayerPrefs.HasKey(key))
            {
                value = Load<T>(key);
                return !Equals(value, default(T));
            }

            value = default;
            return false;
        }
    }
}
