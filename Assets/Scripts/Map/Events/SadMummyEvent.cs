using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SadMummyEvent : WorldMapEvent
{
    private static readonly int DAMAGE = 40;
    private static readonly float ESCAPE_PROBABILITY = 0.25f;
    public override void Build() {
        mainText.SetTextTyping(
            "The room you enter reminds you of a dungeon. As soon as you cross the threshold, bandages come out of nowhere and wrap themselves tightly around your hands and ankles. A <red>creepy mummy</red> steps out of the shadows. <scary>\"I was cursed sooo long ago!\"</scary> she says, <scary>\"I was lonely for sooo long!\"</scary>. You see sandy tears where her eyes were bandaged. <scary>\"You will share my curse so that I am no longer alone!\"</scary>. With a crunch reminiscent of dry paper, the <red>mummy</red> comes closer."
            , false);

        optionButtons[1].txt.SetTextTyping($"[<red>Take {DAMAGE} dmg</red>] Try to break free", false);
        optionButtons[0].txt.SetTextTyping("[<red>Get Cursed</red>] Accept the curse", false);
    }

    public override int OptionButtonsUsed() {
        return 2;
    }

    protected override WorldMapEvent Create() {
        return new SadMummyEvent();
    }

    protected override void UpdateEvent() {
        if (optionButtons[0].pressedThisFrame) {
            mainText.SetTextTyping("Bandages wrap around your body and squeeze the air out of your body. You notice that they <red>tighten over time and hinder your running</red>. As you look into the mummy's face, you see that the anger and grief have disappeared. <scary>\"I am no longer alone,\"</scary> says the mummy and disappears into the darkness.");
            PlayerTeamManager.instance.isMummyCursed = true;
            CloseAfterClick();
            return;
        } else if (optionButtons[1].pressedThisFrame){
            if (Random.value < ESCAPE_PROBABILITY) {
                mainText.SetTextTyping("You manage to <red>tear the bandages</red> around your limbs. Without another moment's pause, you sprint towards the exit of the room. You hear that the mummy is not following you, but its <red>anger and grief are unquenched.</red>");
                CloseAfterClick();
                return;
            } else {
                mainText.SetTextTyping("You pull and tug, but your shackles won't give way.");
                PlayerTeamManager.instance.playerCurrentHP -= DAMAGE;
                if (PlayerTeamManager.instance.playerCurrentHP <= 0) {
                    PlayerTeamManager.instance.playerCurrentHP = 0;
                    WorldMapGameOverMenu.Instance.Show();
                }
                return;
            }
        }
    }
}
