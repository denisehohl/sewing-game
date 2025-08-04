using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ateo.Common
{
	public class CoroutineRunner : ComponentPublishBehaviour<CoroutineRunner>
	{
		private static readonly Dictionary<IEnumerator, Coroutine> Coroutines = new Dictionary<IEnumerator, Coroutine>();

		#region ComponentPublishBehaviour Callbacks

		public override void ResetStatics()
		{
			base.ResetStatics();
			Coroutines.Clear();
		}

		protected override void OnWithdraw()
		{
			StopAllCoroutines();
			Coroutines.Clear();
		}

		#endregion

		public static void CreateInstance()
		{
			if (ComponentPublisher.TryGetComponent(out CoroutineRunner _)) return;
			
			GameObject gos = new GameObject();
			{
				gos.AddComponent<CoroutineRunner>();
				gos.name = "Coroutine Runner";
				DontDestroyOnLoad(gos);
			}
		}

		public static void Run(IEnumerator instance)
		{
			CreateInstance();
			Stop(instance);

			Coroutine c = Instance.StartCoroutine(instance);
			Coroutines.Add(instance, c);
		}

		public static void Stop(IEnumerator instance)
		{
			if (Instance == null || !Coroutines.ContainsKey(instance)) return;

			try
			{
				if (!Coroutines.TryGetValue(instance, out Coroutine coroutine)) return;

				if (coroutine != null)
				{
					Instance.StopCoroutine(coroutine);
					Coroutines.Remove(instance);
				}
			}
			catch (Exception e)
			{
				DebugDev.LogError(e.Message);
			}
		}

		private static IEnumerator RunCoroutine(IEnumerator instance)
		{
			if (instance == null) yield break;

			yield return instance;
		}
	}
}