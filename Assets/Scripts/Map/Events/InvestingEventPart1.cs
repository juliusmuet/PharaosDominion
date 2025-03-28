using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InvestingEventPart1 : WorldMapEvent {
    private static readonly int INVEST_RATE = 50;

    private static readonly string[] BANKERS_ANSWERS = new string[] {
        "Thaaaaaaank you!",
        "You're welcome!",
        "Good money for good purpose!",
        "Money is in good hands!",
        "Deal's on!",
        "Gimme a dollar, I make a dime!",
        "Great making business with you!"
    };

    private GameObject background;
    public override void Build() {
        mainText.SetTextTyping(
            "A short, stocky person comes waddling towards you from a dark corner. She carries a flat briefcase and wears a worn mask that is supposed to represent the <red>god of wealth</red>, Dedun. She looks at your purse and a fire seems to <scarry>ignite in her mask-covered eyes.</scarry> She wants to invest your money in a new type of pyramid construction, <wobbly>a Keops pyramid or something?</wobbly> It sounds promising, but will probably take some time. Do you want to entrust your savings to a derelict banker?"
            , false);

        optionButtons[1].txt.SetTextTyping($"[<red>Lose {INVEST_RATE}$</red>] Invest!!!", false);
        optionButtons[0].txt.SetTextTyping("Leave", false);
        background = Object.Instantiate(WorldMapFactory.Instance.rainingGems);
    }

    public override int OptionButtonsUsed() {
        return 2;
    }

    protected override WorldMapEvent Create() {
        return new InvestingEventPart1();
    }

    protected override void UpdateEvent() {
        if (optionButtons[0].pressedThisFrame) {
            mainText.SetTextTyping("The little creature scurries away.");
            Util.StopParticleSystemEmissionAndDestroy(background.GetComponent<ParticleSystem>(), 3f);
            CloseAfterClick();
            return;
        }
        if (optionButtons[1].pressedThisFrame) {
            if (MoneyManager.instance.GetMoney() < INVEST_RATE) {
                mainText.SetTextTyping($"<scary><red>You have NO money!</red></scary>");
            } else {
                mainText.SetTextTyping($"<wobbly>{BANKERS_ANSWERS[Random.Range(0, BANKERS_ANSWERS.Length)]}</wobbly>");
                Object.Instantiate(WorldMapFactory.Instance.gemExplosion, CameraController.Instance.GetMouseWorld(), Quaternion.identity);
                MoneyManager.instance.SubtractFromMoney(INVEST_RATE);
                InvestingEventPart2.investedMoney += INVEST_RATE;
            }
        }
    }
}
