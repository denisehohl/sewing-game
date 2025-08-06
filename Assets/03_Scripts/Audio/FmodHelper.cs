using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Moreno.SewingGame.Audio
{
	    /// <summary>
    /// for now: short cuts to a) create and b) start 3d events at positions / attached to objects
    /// because a) fmod throws warnings if 3d events are created without a position and b) attaching the event to a transform has to be done every time a event is played again
    /// </summary>
	public static class FmodHelper
	{
        /// <summary> Creates an event instance and sets it's 3d attributes </summary>
        public static EventInstance CreateInstance3D(GUID guid, Vector3 position = new Vector3())
        {
            EventInstance instance = RuntimeManager.CreateInstance(guid);
            instance.set3DAttributes(position.To3DAttributes());
            return instance;
        }
        
        /// <summary> Creates an event instance and sets it's 3d attributes </summary>
        public static EventInstance CreateInstance3D(GUID guid, Transform transform)
        {
            return CreateInstance3D(guid, transform.position);
        }
        
        /// <summary> Creates an event instance and sets it's 3d attributes </summary>
        public static EventInstance CreateInstance3D(string path, Transform transform)
        {
            return CreateInstance3D(RuntimeManager.PathToGUID(path), transform);
        }
        
        /// <summary> Creates an event instance and sets it's 3d attributes </summary>
        public static EventInstance CreateInstance3D(string path, Vector3 position = new Vector3())
        {
            return CreateInstance3D(RuntimeManager.PathToGUID(path), position);
        }
        
        /// <summary> Creates an event instance and sets it's 3d attributes </summary>
        public static EventInstance CreateInstance3D(EventReference eventReference, Transform transform)
        {
            return CreateInstance3D(eventReference.Guid, transform);
        } 
        
        public static EventInstance CreateInstance3D(EventReference eventReference, Vector3 position = new Vector3())
        {
            return CreateInstance3D(eventReference.Guid, position);
        }
        
        /// <summary> Starts the event and attaches it to a GameObject via RuntimeManager.AttachInstanceToGameObject </summary>
        public static void StartAttached(this EventInstance eventInstance, Transform transform, Rigidbody rigidbody)
        {
            eventInstance.start();
            RuntimeManager.AttachInstanceToGameObject(eventInstance, transform, rigidbody);
        }
        
        /// <summary> Starts the event and attaches it to a GameObject via RuntimeManager.AttachInstanceToGameObject </summary>
        public static void StartAttached(this EventInstance eventInstance, Transform transform)
        {
            eventInstance.start();
            RuntimeManager.AttachInstanceToGameObject(eventInstance, transform);
        }
    }
}