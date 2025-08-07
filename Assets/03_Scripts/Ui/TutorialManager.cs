using System;
using System.Collections.Generic;
using Ateo.Common;
using UnityEngine;

namespace Moreno.SewingGame.Ui
{

    public enum TutorialStep
    {
        None,
        Foot,
        Speed,
        Drag,
        Pin,
        Line,
        Needle,
        Thread
    }
    
    public class TutorialManager : ComponentPublishBehaviour<TutorialManager>
    {
        [Serializable]
        public class StepGameObjectEntry
        {
            public TutorialStep Step;
            public GameObject GameObject;
        }
        
        [SerializeField]
        private List<StepGameObjectEntry> _entries = new List<StepGameObjectEntry>();
        private TutorialStep _currentStep;
        private HashSet<TutorialStep> _completedSteps = new HashSet<TutorialStep>();

        protected override void OnPublish()
        {
            Context.OnTutorialChanged += OnTutorialEnabled;
            if (!Context.InTutorial)
            {
                DisplayTutorialStep(TutorialStep.None);
            }
        }

        protected override void OnWithdraw()
        {
            Context.OnTutorialChanged -= OnTutorialEnabled;
        }

        public override void ResetStatics()
        {
            base.ResetStatics();
            
        }

        private void OnTutorialEnabled(bool tutorialActive)
        {
            DisplayTutorialStep(TutorialStep.Foot);
        }
        
        public static void DisplayTutorial(TutorialStep step)
        {
            if(Instance == null) return;
            Instance.DisplayTutorialStep(step);
        }
        

        public void DisplayTutorialStep(TutorialStep step)
        {
            _currentStep = step;
            foreach (StepGameObjectEntry entry in _entries)
            {
                entry.GameObject.SetActive(entry.Step == step);
            }
        }

        public void TryCompleteStep(TutorialStep step)
        {
            if (_currentStep == step)
            {
                _completedSteps.Add(step);
            }
            DisplayTutorialStep(TutorialStep.None);
        }
        
    }
}