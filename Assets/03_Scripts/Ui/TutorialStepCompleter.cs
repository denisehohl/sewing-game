using UnityEngine;

namespace Moreno.SewingGame.Ui
{
    public class TutorialStepCompleter : MonoBehaviour
    {
        private TutorialStep _tutorialStepToComplete;

        public void CompleteTutorialStep()
        {
            if(_tutorialStepToComplete == TutorialStep.None) return;
            TutorialManager.Instance.TryCompleteStep(_tutorialStepToComplete);
        }
    }
}