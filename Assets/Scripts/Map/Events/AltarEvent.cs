using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AltarEvent : WorldMapEvent
{
    public override void Build() {
        mainText.SetTextTyping(
            "You enter a deserted chamber,<scary> but feel like you are being watched.</scary> Ancient deities are piled up on three plinths in front of you. Seth, with his anubis head, which symbolises <red>strength.</red> Then Sekhmet, an elegant woman with a lion's head, known for her <red>healing powers.</red> And finally Khonsus, half eagle half man, who promises travellers a <red>fast journey.</red> You decide to pray to one of the deities, but which one?"
            , false);

        optionButtons[0].txt.SetTextTyping("Worship Khonsus", false);
        optionButtons[1].txt.SetTextTyping("Worship Sekhmet", false);
        optionButtons[2].txt.SetTextTyping("Worship Seth", false);
    }

    public override int OptionButtonsUsed() {
        return 3;
    }

    protected override WorldMapEvent Create() {
        return new AltarEvent();
    }

    protected override void UpdateEvent() {
        const UPGRADE_TYPE defaultType = UPGRADE_TYPE.UPGRADE_TEAM_DEFENSE;
        UPGRADE_TYPE type = defaultType;

        if (optionButtons[0].pressedThisFrame) {
            type = UPGRADE_TYPE.UPGRADE_PLAYER_MOVE_SPEED;
        } else if (optionButtons[1].pressedThisFrame) {
            type = UPGRADE_TYPE.UPGRADE_PLAYER_HP;
        } else if (optionButtons[2].pressedThisFrame) {
            type = UPGRADE_TYPE.UPGRADE_PLAYER_ATTACK_STRENGHT;
        }

        if (type != defaultType) {
            mainText.SetTextTyping("<wobbly>A sparkle plays around the god's head.</wobbly> You feel changed as you receive the god's blessing. As you turn to leave, <scarry>you feel the eyes of the other two boring into your back.</scarry>");
            Upgrade up = UpgradeManager.instance.GetUpgradeByType(type);
            up.Invoke();
            up.Invoke();
            CloseAfterClick();
        }
    }
}
