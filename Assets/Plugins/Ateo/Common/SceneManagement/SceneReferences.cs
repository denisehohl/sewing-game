using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Ateo.Common
{
    [System.Serializable, CreateAssetMenu(fileName = "SceneReferences", menuName = "SceneLoader/Create SceneReferences", order = 0)]
    public class SceneReferences : SerializedScriptableObject
    {
        // ReSharper disable once CollectionNeverUpdated.Global
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        [InfoBox("Please add all scenes to this list that you want to load at runtime.\nDon't forget to click the 'Add' button.")]
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine, ValueLabel = "Scene Reference")]
        public Dictionary<string, SceneReference> References = new Dictionary<string, SceneReference>();
    }
}