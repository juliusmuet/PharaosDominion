using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestingEventPart2 : WorldMapEvent
{
    public static int investedMoney = 0;

    private GameObject background;
    public override void Build() {
        mainText.SetTextTyping(
            "The little person in <red>Dedun's mask</red> suddenly steps into the light in front of you. She gives you a rakish smile. <wobbly>\"Nice doing business with you!\"</wobbly> She drops a <green>bulging purse</green> and disappears into the darkness, <scarry>giggling softly.</scarry>"
            , false);

        optionButtons[0].txt.SetTextTyping("Take your earnings", false);
        background = Object.Instantiate(WorldMapFactory.Instance.rainingGems);

        investedMoney = (int)(investedMoney * (1 + Random.Range(0.5f, 1.5f)));
    }

    public override int OptionButtonsUsed() {
        return 1;
    }

    protected override WorldMapEvent Create() {
        return new InvestingEventPart2();
    }

    protected override void UpdateEvent() {
        if (optionButtons[0].pressedThisFrame) {
            Object.Instantiate(WorldMapFactory.Instance.gemExplosion, CameraController.Instance.GetMouseWorld(), Quaternion.identity);
            mainText.SetTextTyping($"With a smile on your face you pick up {investedMoney} dollar!");
            MoneyManager.instance.AddToMoney(investedMoney);
            investedMoney = 0;
            Util.StopParticleSystemEmissionAndDestroy(background.GetComponent<ParticleSystem>(), 3f);
            CloseAfterClick();
            return;
        }
    }
}
