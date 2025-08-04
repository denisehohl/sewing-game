using System;
using Ateo.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ateo.StateManagement
{
	public abstract class StateScene<T> : State<T> where T: IState, new()
	{
#if STATEMACHINE
		public abstract string SceneKey { get; }
		public abstract LoadSceneMode Mode { get; }
		public abstract bool MakeSceneActive { get; }
		public abstract float Delay { get; }

		public override void Start()
		{
			SceneReference sceneReference = SceneReferenceManager.GetSceneReference(SceneKey);

			if (sceneReference != null)
			{
				if (SceneLoader.IsSceneLoaded(sceneReference))
				{
					base.Start();
				}
				else
				{
					SceneLoader.LoadScene(sceneReference, Mode, MakeSceneActive, Delay, OnSceneLoaded);
				}
			}
			else
			{
				Debug.LogException(new Exception($"{GetType().Name}.Start(): No SceneReference with key '{SceneKey}' found."));
			}
		}

		protected virtual void OnSceneLoaded()
		{
			if (StateNext == null)
			{
				Debug.LogException(new Exception($"{GetType().Name}.Start(): StateNext is null. This is not permitted"));
				return;
			}
			
			StateManager.ChangeTo(StateNext);
		}
#endif
	}
}