using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GemRainEvent : WorldMapEvent {
    private static readonly int DMG = 50, GEMS = 50;

    private GameObject background;
    public override void Build() {
        mainText.SetTextTyping("The sky darkens and <scary>all of a sudden it starts raining</scary> <green>gems!</green> You cannot grasp you luck and reach out for one of the " +
                               "<green>gems.</green> You quickly retrieve your arm as <red>devastating pain</red> strikes from the rapidly falling stones strikes you.\n<wobbly>Will you withstand your greed?</wobbly>"
            , false);


        optionButtons[1].txt.SetTextTyping($"[<red>Take {DMG} dmg</red>] Reach out again!", false);
        optionButtons[0].txt.SetTextTyping("Leave!", false);

        background = Object.Instantiate(WorldMapFactory.Instance.rainingGems);
    }

    public override int OptionButtonsUsed() {
        return 2;
    }

    protected override WorldMapEvent Create() {
        return new GemRainEvent();
    }

    protected override void UpdateEvent() {
        if (optionButtons[0].pressedThisFrame) {
            mainText.SetTextTyping("The clicking sound of impacable wealth vanishes in the distance.");
            Util.StopParticleSystemEmissionAndDestroy(background.GetComponent<ParticleSystem>(), 3f);
            CloseAfterClick();
            return;
        } 
        if (optionButtons[1].pressedThisFrame) {
            CameraController.Instance.Shake();
            Object.Instantiate(WorldMapFactory.Instance.gemExplosion, CameraController.Instance.GetMouseWorld(), Quaternion.identity);
            PlayerTeamManager.instance.playerCurrentHP = Mathf.Max(PlayerTeamManager.instance.playerCurrentHP - DMG, 0);
            if (PlayerTeamManager.instance.playerCurrentHP <= 0) {
                WorldMapGameOverMenu.Instance.Show();
                return;
            }

            MoneyManager.instance.AddToMoney(GEMS);
            
        }
    }
}
