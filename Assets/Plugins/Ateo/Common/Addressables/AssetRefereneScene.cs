#if ADDRESSABLES
using System;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ateo.Common.AddressableAssets
{
	[System.Serializable]
	public class AssetReferenceScene : AssetReference
	{
		/// <summary>
		/// Construct a new AssetReference object.
		/// </summary>
		/// <param name="guid">The guid of the asset.</param>
		public AssetReferenceScene(string guid) : base(guid)
		{
		}


		/// <inheritdoc/>
		public override bool ValidateAsset(Object obj)
		{
#if UNITY_EDITOR
			Type type = obj.GetType();
			return typeof(SceneAsset).IsAssignableFrom(type);
#else
            return false;
#endif
		}

		/// <inheritdoc/>
		public override bool ValidateAsset(string path)
		{
#if UNITY_EDITOR
			Type type = AssetDatabase.GetMainAssetTypeAtPath(path);
			return typeof(SceneAsset).IsAssignableFrom(type);
#else
            return false;
#endif
		}

#if UNITY_EDITOR
		/// <summary>
		/// Type-specific override of parent editorAsset.  Used by the editor to represent the asset referenced.
		/// </summary>
		public new SceneAsset editorAsset => (SceneAsset) base.editorAsset;
#endif
	}
}
#endif