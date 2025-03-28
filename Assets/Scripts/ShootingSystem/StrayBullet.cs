using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrayBullet : BasicShot
{

    [SerializeField] private float stry_speed = 7;
    [SerializeField] private int stry_damage = 10;

    protected override void Awake() {
        base.Awake();
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
    }

    private void Start() {
        Hitbox hb = SpacialGrouping.currentGrouping.GetClosestHitbox(transform.position, 100, Team.PLAYER);
        if (hb == null) {
            Destroy(this.gameObject);
            return;
        }
        Vector3 diff = transform.position - hb.GetCenter();
        float angle = Mathf.Atan2(-diff.y, -diff.x);
        Init(angle, stry_speed * ChallengeController.Instance.bulletSpeedMultiplicator, stry_damage, Team.ENEMY);
    }

    
}
