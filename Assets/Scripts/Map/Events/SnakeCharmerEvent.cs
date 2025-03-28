using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeCharmerEvent : WorldMapEvent
{
    private static readonly int PRICE = 400;
    public override void Build() {
        mainText.SetTextTyping(
            "Long before you enter the chamber, you hear the <wobbly>soft melody of a flute.</wobbly> In the centre of the room, a slumped figure sits cross-legged, immersed in her <rainbow>beautiful flute playing.</rainbow> Baskets are arranged in a semicircle in front of her. As the flute playing reaches its climax, beautiful golden snakes stretch their heads out of the baskets and <wobbly>sway dreamily to the music.</wobbly> The Charmane interrupts his playing and offers you his services."
            , false);

        optionButtons[1].txt.SetTextTyping($"[Lose <red>{PRICE}</red>] Accept", false);
        optionButtons[0].txt.SetTextTyping("Decline", false);
    }

    public override int OptionButtonsUsed() {
        return 2;
    }

    protected override WorldMapEvent Create() {
        return new SnakeCharmerEvent();
    }

    protected override void UpdateEvent() {

        if (optionButtons[0].pressedThisFrame) {
            mainText.SetTextTyping("You leave the beautiful room. You may not have bought anything, but the music creates a <wobbly>feeling of inner peace.</wobbly>");
            CloseAfterClick();
            return;
        }
        if (optionButtons[1].pressedThisFrame) {
            if (MoneyManager.instance.GetMoney() < PRICE) {
                mainText.SetTextTyping("No <red>money</red>, no deal!");

            } else {
                mainText.SetTextTyping("The snake charmer smiles kindly and starts a new, even more beautiful piece. You feel all the snakes in your team start to <wobbly>vibrate to the music.</wobbly> Slowly but surely, your snakes turn a beautiful <yellow>golden colour.</yellow>");
                PlayerTeamManager.instance.ReplaceAllSnakesWithGolden_WORLDMAP();
                MoneyManager.instance.SubtractFromMoney(PRICE);
                CloseAfterClick();
                return;
            }
        }
    }
}
