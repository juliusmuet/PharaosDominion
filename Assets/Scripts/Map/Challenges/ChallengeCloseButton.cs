using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeCloseButton : GameObjButton {
    protected override void performAction() {
        if (Tutorial.tutorialActivated && Tutorial.GetCurrentStep() != 12)
            return;
        if (Tutorial.tutorialActivated) {
            Tutorial.Instance.IncreaseStep();
        }

        ChallengeBordUI.Instance.Close();
    }
}
