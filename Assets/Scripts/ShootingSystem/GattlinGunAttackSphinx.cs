using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GattlinGunAttackSphinx : MonoBehaviour
{
    [SerializeField] private float range = 1.5f;
    [SerializeField] private float rotationSpeed = 270;
    [SerializeField] private int bulletCount = 10;
    [SerializeField] private int shotDamage = 10;
    [SerializeField] private float bulletSpeed = 8;

    [SerializeField] private GameObject basicShot;

    private bool phaseBuildUp = true;
    private bool pausePhase1 = false;
    private float rotationLeftToSpawnBullet = 0;

    private float angleToShootAt = -999;

    private LinkedList<BasicShot> shots = new LinkedList<BasicShot>();
    private void Update() {
        float deltaR = rotationSpeed * Time.deltaTime;
        this.transform.rotation = Quaternion.Euler(this.transform.rotation.eulerAngles + new Vector3(0, 0, deltaR));
        
        bool[] shotAngleOverThresholdBeforeRotation = new bool[bulletCount];
        if (!phaseBuildUp) {
            if (angleToShootAt == -999) {
                Vector3 playerPos = PlayerMovement.Instance.transform.position;
                Vector3 diff = (playerPos - transform.position);
                float timeToPlayer = (diff.magnitude - range) / bulletSpeed;
                Vector3 targetPos = playerPos + PlayerMovement.Instance.GetPlayerPredictiveSpeed() * timeToPlayer;
                diff = (targetPos - transform.position);

                angleToShootAt = Mathf.Atan2(diff.y, diff.x) + Mathf.PI;

            }

            shots.Remove((BasicShot)null);
            int i = 0;
            foreach (BasicShot shot in shots) {
                if (shot == null) continue;
                Vector3 localPos = calcLocalPos(shot);
                float shotAnlge = Mathf.Atan2(localPos.y, localPos.x) + Mathf.PI;
 
                shotAngleOverThresholdBeforeRotation[i++] = shotAnlge < angleToShootAt;
                
                //Debug.Log($"{i - 1}: {shotAngleOverThresholdBeforeRotation[i - 1]} {shotAnlge} <? {angleToShootAt} {localPos}");
            }
        }

        if (phaseBuildUp) {
            if (pausePhase1) {
                return;
            }
            rotationLeftToSpawnBullet -= deltaR;
            if (rotationLeftToSpawnBullet <= 0) {
                BasicShot shot = Instantiate(basicShot, transform.position - new Vector3(0, -range, 0), Quaternion.identity, this.transform).GetComponent<BasicShot>();
                shot.Init(0, 0, shotDamage, Team.ENEMY);
                shots.AddLast(shot);

                if (shots.Count == bulletCount) {
                    pausePhase1 = true;
                    StartCoroutine(SwitchPhaseAfterTime(2));
                } else {
                    rotationLeftToSpawnBullet = 360 / (float)bulletCount;
                }
            }
        } else {

            int i = 0;
            foreach (BasicShot shot in shots) {
                if (shot == null) continue;
                Vector3 localPos = calcLocalPos(shot);
                float shotAnlge = Mathf.Atan2(localPos.y, localPos.x) + Mathf.PI;

                if (!shotAngleOverThresholdBeforeRotation[i] && shotAnlge >= angleToShootAt) {

                    shot.transform.parent = null;
                    float timeToPlayer = ((PlayerMovement.Instance.transform.position - transform.position).magnitude - range) / bulletSpeed;
                    Vector3 diff = (PlayerMovement.Instance.transform.position + PlayerMovement.Instance.GetPlayerPredictiveSpeed() * timeToPlayer) - transform.position;
                    shot.Init(Mathf.Atan2(diff.y, diff.x), bulletSpeed, shotDamage, Team.ENEMY);
                    shots.Remove(shot);
                    break;
                }
            }

            if (shots.Count == 0) {
                Destroy(this.gameObject);
            }
        }
    }

    public Vector3 calcLocalPos(BasicShot shot) {
        return (shot.transform.position - shot.transform.parent.position) * (1 / range);
    }

    private IEnumerator SwitchPhaseAfterTime(float sec) {
        yield return new WaitForSeconds(sec);
        phaseBuildUp = false;
    }
}
