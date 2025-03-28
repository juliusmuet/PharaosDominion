using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventOptionButton : GameObjButton {

    public bool pressedThisFrame = false;
    public EnhancedTexts txt;
    public Animator anim;

    protected override void Update() {
        pressedThisFrame = false;
        base.Update();
    }

    protected override void performAction() {
        pressedThisFrame = true;
    }
}
