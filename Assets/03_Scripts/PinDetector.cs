using System;
using UnityEngine;
using UnityEngine.Events;

namespace Moreno.SewingGame
{
    public class PinDetector : MonoBehaviour
    {
        public UnityEvent OnPinDetected;

        private void Start()
        {
            Context.OnTutorialChanged += OnTutorialChanged;
            OnTutorialChanged(Context.InTutorial);
        }

        private void OnDestroy()
        {
            Context.OnTutorialChanged -= OnTutorialChanged;
        }

        private void OnTutorialChanged(bool tutorialActive)
        {
            gameObject.SetActive(tutorialActive);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Pin")) return;
            OnPinDetected?.Invoke();
            gameObject.SetActive(false);
        }
    }
}