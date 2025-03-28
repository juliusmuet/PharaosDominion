using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Challenge {
    private static ChanceSystem<Challenge> randomChallenges = null;

    static Challenge() {
        if (randomChallenges != null)
            return;

        randomChallenges = new ChanceSystem<Challenge>();
        randomChallenges.AddItem(new FasterBulletsChallenge(), 10);
        randomChallenges.AddItem(new DarknessChallenge(), 10);
        randomChallenges.AddItem(new StrayBulletsChallenge(), 10);
        randomChallenges.AddItem(new TrapChallenge(), 10);
        randomChallenges.AddItem(new StrongerEnemiesChallenge(), 10);
        randomChallenges.AddItem(new TimeRushChallenge(), 10);
    }

    private string description;
    private ChallengeReward reward;

    public Challenge(string description) {
        this.description = description;

        reward = Factory.Instance.challengeRewards.Generate();
    }   

    public ChallengeReward GetChallengeReward() { 
        return reward;
    }

    public string GetDescription() {
        return description;
    }

    public static Challenge GetRandomChallenge() {
        //Debug.Log(randomChallenges != null);
        return randomChallenges.Generate().create();
    }
    public abstract void Apply(ChallengeController controller);
    public abstract void Remove(ChallengeController controller);

    public abstract Challenge create();

}

public class FasterBulletsChallenge : Challenge {
    public FasterBulletsChallenge() : base("Faster Bullets") {
    }

    public override void Apply(ChallengeController controller) {
        controller.bulletSpeedMultiplicator += 0.3f;
    }

    public override Challenge create() {
        return new FasterBulletsChallenge();
    }

    public override void Remove(ChallengeController controller) {
        controller.bulletSpeedMultiplicator -= 0.3f;
    }
}

public class DarknessChallenge : Challenge {
    public DarknessChallenge() : base("Darkness") {
    }


    public override void Apply(ChallengeController controller) {
        controller.darkness += 1;
    }

    public override Challenge create() {
        return new DarknessChallenge();
    }

    public override void Remove(ChallengeController controller) {
        controller.darkness -= 1;
    }
}

public class StrayBulletsChallenge : Challenge {
    public StrayBulletsChallenge() : base("Stray Bullets") {
    }

    public override void Apply(ChallengeController controller) {
        controller.stryBullets++;
    }

    public override Challenge create() {
        return new StrayBulletsChallenge();
    }

    public override void Remove(ChallengeController controller) {
        controller.stryBullets--;
    }
}

public class TrapChallenge : Challenge {
    private static readonly int TRAPS = 30;
    public TrapChallenge() : base("Traps") {
    }

    public override void Apply(ChallengeController controller) {
        controller.trapAmount += TRAPS;
    }

    public override Challenge create() {
        return new TrapChallenge(); 
    }

    public override void Remove(ChallengeController controller) {
        controller.trapAmount -= TRAPS;
    }
}

public class StrongerEnemiesChallenge : Challenge {
    private static readonly float HEALTH_MULTIPLIER = 0.5f;

    public StrongerEnemiesChallenge() : base("Healthier Enemies") {
    }

    public override void Apply(ChallengeController controller) {
        controller.enemyHealthModificator += HEALTH_MULTIPLIER;
    }

    public override Challenge create() {
        return new StrongerEnemiesChallenge();
    }

    public override void Remove(ChallengeController controller) {
        controller.enemyHealthModificator -= HEALTH_MULTIPLIER;
    }
}

public class TimeRushChallenge : Challenge {
    private static readonly float TIME_SPEEDUP = 0.33f;

    public TimeRushChallenge() : base("Time Rush") {
    }

    public override void Apply(ChallengeController controller) {
        controller.timeSpeedUpModificator += TIME_SPEEDUP;
    }

    public override Challenge create() {
        return new TimeRushChallenge();
    }

    public override void Remove(ChallengeController controller) {
        controller.timeSpeedUpModificator -= TIME_SPEEDUP;
    }
}