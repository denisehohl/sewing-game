using System;
using System.Collections;
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
        Thread,
        Speed2,
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
        [SerializeField]
        private float _minTraveledDistanceToCompleteLineStep = 400f;
        [SerializeField]
        private float _minCompletedSpeedTime = 1f;
        [SerializeField]
        private float _minCompletedRotationTime = 1f;
        private TutorialStep _currentStep;
        private HashSet<TutorialStep> _completedSteps = new HashSet<TutorialStep>();
        private float _followLineEnteredCachedDistance;
        private float _completedTime = 0;

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

        private void Update()
        {
            switch (_currentStep)
            {
                case TutorialStep.None:
                    return;
                case TutorialStep.Foot:
                    if (SewingMachineController.Instance.FootDown)
                    {
                        TryCompleteStep(TutorialStep.Foot);
                        StartDelayedTutorial(TutorialStep.Speed,2f);
                    }
                    break;
                case TutorialStep.Speed:
                    if (SewingMachineController.Instance.CurrentSpeed != 0)
                    {
                        _completedTime += Time.deltaTime;

                        if (_completedTime > _minCompletedSpeedTime)
                        {
                            TryCompleteStep(TutorialStep.Speed);
                            StartDelayedTutorial(TutorialStep.Drag,0.5f);
                        }
                    }
                    break;
                case TutorialStep.Speed2:
                    if (SewingMachineController.Instance.CurrentSpeed != 0)
                    {
                        _completedTime += Time.deltaTime;

                        if (_completedTime > _minCompletedSpeedTime)
                        {
                            TryCompleteStep(TutorialStep.Speed2);
                        }
                    }
                    break;
                case TutorialStep.Drag:
                    if (SewingMachineController.Instance.CurrentRotationSpeed != 0)
                    {
                        _completedTime += Time.deltaTime;

                        if (_completedTime > _minCompletedRotationTime)
                        {
                            TryCompleteStep(TutorialStep.Drag);
                            StartDelayedTutorial(TutorialStep.Line,0.5f);
                        }
                    }
                    break;
                case TutorialStep.Line:
                    float currentDistance = SewingMachineController.Instance.CurrentTraveledDistance;
                    if (currentDistance - _minTraveledDistanceToCompleteLineStep >= _followLineEnteredCachedDistance)
                    {
                        TryCompleteStep(TutorialStep.Line);
                    }
                    break;
                    //Pin
                    //Needle
                    //Thread
            }
        }

        private void OnStateEnter(TutorialStep newStep, TutorialStep previousStep)
        {
            _completedTime = 0;
            switch (newStep)
            {
                case TutorialStep.Pin:
                    Pin.OnPinRemoved += OnPinRemoved;
                    break;
                case TutorialStep.Line:
                    _followLineEnteredCachedDistance = SewingMachineController.Instance.CurrentTraveledDistance;
                    break;
                case TutorialStep.Needle:
                    NeedleManager.OnNeedleFixed += OnNeedleFixed;
                    break;
                case TutorialStep.Thread:
                    NeedleManager.OnThreadingCompleted += OnThreadingCompleted;
                    break;
            }
        }

        private void OnStateExit(TutorialStep newStep,TutorialStep previousStep)
        {
            switch (previousStep)
            {
                case TutorialStep.Pin:
                    Pin.OnPinRemoved -= OnPinRemoved;
                    if (newStep == TutorialStep.None)
                    {
                        StartDelayedTutorial(TutorialStep.Speed2,1);
                    }
                    break;
                case TutorialStep.Line:
                    break;
                case TutorialStep.Needle:
                    NeedleManager.OnNeedleFixed -= OnNeedleFixed;
                    break;
                case TutorialStep.Thread:
                    NeedleManager.OnThreadingCompleted -= OnThreadingCompleted;
                    if (newStep == TutorialStep.None)
                    {
                        StartDelayedTutorial(TutorialStep.Speed2,1);
                    }
                    break;
            }
        }

        private void OnTutorialEnabled(bool tutorialActive)
        {
            if (!tutorialActive)
            {
                DisplayTutorialStep(TutorialStep.None);
                NeedleManager.OnThreadingStarted -= OnThreadingStarted;
                NeedleManager.OnNeedleBroken -= OnNeedleBroken;
                return;
            }
            _completedSteps.Clear();
            DisplayTutorialStep(TutorialStep.Foot);
            NeedleManager.OnThreadingStarted += OnThreadingStarted;
            NeedleManager.OnNeedleBroken += OnNeedleBroken;
        }

        public static void DisplayTutorial(TutorialStep step)
        {
            if(Instance == null) return;
            Instance.DisplayTutorialStep(step);
        }
        

        public void DisplayTutorialStep(TutorialStep step)
        {
            if(_completedSteps.Contains(step))return;
            if(_currentStep == step) return;
            var previous = _currentStep;
            _currentStep = step;
            OnStateExit(step,previous);
            OnStateEnter(step, previous);
            
            foreach (StepGameObjectEntry entry in _entries)
            {
                entry.GameObject.SetActive(entry.Step == step);
            }
        }

        private void StartDelayedTutorial(TutorialStep step, float delay)
        {
            StartCoroutine(Routine());
            return;
            
            IEnumerator Routine()
            {
                yield return new WaitForSeconds(delay);
                if(_currentStep != TutorialStep.None) yield break;
                DisplayTutorialStep(step);
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
        
        private void OnPinRemoved(Pin pin)
        {
            TryCompleteStep(TutorialStep.Pin);
        }
        
        private void OnThreadingCompleted()
        {
            TryCompleteStep(TutorialStep.Thread);
        }

        private void OnNeedleFixed()
        {
            TryCompleteStep(TutorialStep.Needle);
        }
        
        private void OnThreadingStarted()
        {
            DisplayTutorialStep(TutorialStep.Thread);
        }

        private void OnNeedleBroken()
        {
            DisplayTutorialStep(TutorialStep.Needle);
        }
        
    }
}