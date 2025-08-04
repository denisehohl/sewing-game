using System.Collections.Generic;
using System.Linq;
using Ateo.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ateo.Common.Editor
{
	public static partial class EditorTools
	{
		[MenuItem("Tools/Editor Tools/Check Used Layers")]
		public static void CheckUsedLayers()
		{
			Dictionary<string, int> layerCount = new Dictionary<string, int>();
			List<string> layerNames = new List<string>();

			if (Resources.FindObjectsOfTypeAll(typeof(GameObject)) is GameObject[] gameObjects && gameObjects.Length > 0)
			{
				// iterate objects and save to dictionary
				foreach (GameObject gos in gameObjects)
				{
					string layerName = LayerMask.LayerToName(gos.layer);
					
					if (layerCount.ContainsKey(layerName))
					{
						layerCount[layerName]++;
					}
					else
					{
						layerCount.Add(layerName, 1);
					}
				}

				// log to console
				foreach (KeyValuePair<string, int> entry in layerCount)
				{
					Debug.Log(entry.Key + ": " + entry.Value);
				}

				// unused layers
				for (int i = 8; i <= 31; i++) //user defined layers start with layer 8 and unity supports 31 layers
				{
					string layerN = LayerMask.LayerToName(i); //get the name of the layer
					if (layerN.Length > 0) //only add the layer if it has been named (comment this line out if you want every layer)
					{
						layerNames.Add(layerN);
					}
				}
			}

			List<string> listOfKeys = layerCount.Keys.ToList();
			List<string> unusedLayers = layerNames.Except(listOfKeys).ToList();
			string joined = string.Join(", ", unusedLayers);
			Scene scene = SceneManager.GetActiveScene();
			Debug.Log("Unused layers in " + scene.name + ": " + joined);
		}

		public static void CheckUsedLayer(LayerMask layerMask)
		{
			if (!(Resources.FindObjectsOfTypeAll(typeof(GameObject)) is GameObject[] gameObjects) || gameObjects.Length <= 0) return;
			
			foreach (GameObject gos in gameObjects)
			{
				if (layerMask.Contains(gos.layer))
				{
					Debug.Log(gos.EditorGetFriendlyPath());
				}
			}
		}
	}
}