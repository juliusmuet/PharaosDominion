using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldMapEvent
{
    public static readonly int MAX_OPTIONS = 3;

    private static ChanceSystem<WorldMapEvent> randomEvents = null;

    private static LinkedList<System.Type> latestEvents = new LinkedList<System.Type>();

    static WorldMapEvent() {
        if (randomEvents != null)
            return;

        randomEvents = new ChanceSystem<WorldMapEvent>();
        randomEvents.AddItem(new GemRainEvent(), 10);
        randomEvents.AddItem(new InvestingEventPart1(), 10);
        randomEvents.AddItem(new AltarEvent(), 10);
        randomEvents.AddItem(new HeadsTailsEvent(), 10);
        randomEvents.AddItem(new SnakeCharmerEvent(), 10);
        randomEvents.AddItem(new SadMummyEvent(), 10);
    }

    //CHANGE TO MY TMP controller
    public EnhancedTexts mainText;
    public EventOptionButton[] optionButtons;

    private bool finished = false;
    private bool stopUpdating = false;
    private float closeIn = -1;
    private bool closeAfterClick = false;


    public void Update() {
        if (!stopUpdating) {
            UpdateEvent();
        } else {

            if (closeIn != -1) {
                closeIn -= Time.deltaTime;
                if (closeIn < 0) {
                    Close();
                }
            } else
            if (closeAfterClick) {
                if (Input.GetMouseButtonDown(0))
                    Close();
            }
        }
    }
    
    public abstract void Build();
    protected abstract void UpdateEvent();

    public abstract int OptionButtonsUsed();
    protected abstract WorldMapEvent Create();

    public bool GetFinished() {
        return finished;
    }

    public void Close() {
        if (Tutorial.tutorialActivated)
            Tutorial.Instance.IncreaseStep();
        finished = true;
    }

    public void CloseIn(float sec) {
        stopUpdating = true;
        closeIn = sec;
    }

    public void CloseAfterClick() {
        stopUpdating = true;
        closeAfterClick = true;
    }

    public static WorldMapEvent GenerateEvent() {
        if (Tutorial.tutorialActivated)
            return new AltarEvent();

        if (InvestingEventPart2.investedMoney > 0)
            return new InvestingEventPart2();

        WorldMapEvent ev;
        int iteration = 0;
        do {
            ev = randomEvents.Generate().Create();

            if (ev is SnakeCharmerEvent && PlayerTeamManager.instance.GetSnakes().Count <= 0)
                continue;
            if (ev is SadMummyEvent && PlayerTeamManager.instance.isMummyCursed)
                continue;
            if (iteration++ < 30 && latestEvents.Contains(ev.GetType()))
                continue;

            break;
        } while (true);

        latestEvents.AddLast(ev.GetType());
        return ev;
    }

    public static void ResetLatestEvents() {
        latestEvents.Clear();
    }

}
