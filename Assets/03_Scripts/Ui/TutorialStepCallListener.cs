using UnityEngine;

namespace Moreno.SewingGame.Ui
{
    public class TutorialStepCallListener : MonoBehaviour
    {
        [SerializeField]
        private TutorialStep _tutorialStepToRequest;
        [SerializeField]
        private TutorialStep _tutorialStepToComplete;

        public void CompleteTutorialStep()
        {
            if(_tutorialStepToComplete == TutorialStep.None) return;
            TutorialManager.Instance.TryCompleteStep(_tutorialStepToComplete);
        }

        public void RequestTutorialStep()
        {
            TutorialManager.DisplayTutorial(_tutorialStepToRequest);
        }
    }
}