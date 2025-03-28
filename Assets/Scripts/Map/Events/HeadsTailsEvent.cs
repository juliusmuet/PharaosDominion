using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadsTailsEvent : WorldMapEvent
{
    public override void Build() {
        mainText.SetTextTyping(
            "You reach a <scary>dark, ominous room</scary>. It is completely empty except for the pedestal in the centre. A single incredibly beautiful golden coin is draped on the raised platform. You feel drawn to the coin and pick it up. Horus, the <red>god of strength</red>, is emblazoned on one side, Anubis, the <red>god of death</red>, on the other. Will you throw the coin?"
            , false);

        optionButtons[0].txt.SetTextTyping("[Get Cursed!] Steal!", false);
        optionButtons[1].txt.SetTextTyping("Heads!", false);
        optionButtons[2].txt.SetTextTyping("Tails!", false);
    }

    public override int OptionButtonsUsed() {
        return 3;
    }

    protected override WorldMapEvent Create() {
        return new HeadsTailsEvent();
    }

    protected override void UpdateEvent() {

        if (optionButtons[0].pressedThisFrame) {
            mainText.SetTextTyping("You take the coin and run! A <scary>hoarse, bone-piercing scream</scary> resounds from the depths of the hangover combs. It makes you collapse on the spot, trembling and regretting your decision. <wobbly>\"Nobody steals from the gods!\"</wobbly>");
            CameraController.Instance.Shake();
            PlayerTeamManager.instance.playerMaxHp = 10;
            PlayerTeamManager.instance.playerCurrentHP = 1;
            PlayerTeamManager.instance.playerBaseHp = 10;
            MoneyManager.instance.AddToMoney(9999);
            CloseAfterClick();
            return;
        } else if (optionButtons[1].pressedThisFrame || optionButtons[2].pressedThisFrame) {

            bool correct = Random.value > 0.5;
            bool onePressed = optionButtons[1].pressedThisFrame;
            if (!correct)
                onePressed = !onePressed;
            string landsOn = onePressed ? "Heads" : "Tails";

            if (correct) {
                mainText.SetTextTyping($"The coin flies through the air in a wide arc and lands clattering on the ground. <red>{landsOn}</red> up! <wobbly>A warm feeling runs through your body and you feel invigorated!</wobbly>");

                Upgrade up = UpgradeManager.instance.GetUpgradeByType(UPGRADE_TYPE.UPGRADE_PLAYER_ATTACK_STRENGHT);

                up.Invoke();
                up.Invoke();
            } else {
                mainText.SetTextTyping($"The coin flies through the air in a wide arc and lands on the ground with an <scary>ominous bang.</scary> <red>{landsOn}</red> up. The grimace of the <red>god of death</red> smiles cruelly at you as a sudden weakness spreads through you. Powerless, you limp out of the room.");
                Upgrade up = UpgradeManager.instance.GetUpgradeByType(UPGRADE_TYPE.UPGRADE_PLAYER_ATTACK_STRENGHT);
                
                if (up.level != 0)
                {
                    up.level = -1;
                    up.Invoke();
                }
            }

            CloseAfterClick();
        }

    }
}
