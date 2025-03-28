using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DelayedAimedBulletDisperser : MonoBehaviour
{
    [SerializeField] private int bulletCount = 10;
    [SerializeField] private float spawnTime = 1f;
    [SerializeField] private GameObject delayedAimingBulletPrefab;

    private float timeNextBulletSpawns = -1;
    private Team team;
    private Hitbox target;
    private Vector3 lastTargetPos = Vector3.zero;
    private int bulletsShot = 0;

    public void Init(Team team) {
        this.team = team;
        target = SpacialGrouping.currentGrouping.GetClosestHitbox(transform.position, 100, team.GetOpposingTeam());
        if (target == null) {
            Destroy(gameObject);
            return;
        }
        lastTargetPos = target.transform.position;
    }

    public void Update() {
        if (target != null) {
            lastTargetPos = target.transform.position;
        }

        if (Time.time > timeNextBulletSpawns) {
            timeNextBulletSpawns = Time.time + spawnTime / bulletCount;
            
            Instantiate(delayedAimingBulletPrefab, transform.position, Quaternion.identity).GetComponent<DelayedAimingBullet>().Init(target, lastTargetPos, team);

            if (++bulletsShot >= bulletCount) {
                Destroy(this.gameObject);
                return;
            }
        }
    }
}
