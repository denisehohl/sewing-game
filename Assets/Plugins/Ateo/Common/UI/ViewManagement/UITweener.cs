using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;

namespace Ateo.ViewManagement
{
	public abstract class UITweener : MonoBehaviour
    {
        public abstract void Show(TweenCallback callback = null);
        public abstract void Hide(TweenCallback callback = null);
    }
}