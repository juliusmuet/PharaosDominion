using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedAimingBullet : MonoBehaviour
{
    private static readonly float E_THA = 0.01f;

    [SerializeField] private BasicShot shot;
    [SerializeField] private float disperseSpeed = 3f;
    [SerializeField] private float speedDampFactor = 3f;
    [SerializeField] private float aimStartSpeed = 2f;
    [SerializeField] private float aimSpeedUp = 3f;
    [SerializeField] private int damage = 10;


    private Hitbox target;
    private Vector3 lastTargetPos = Vector3.zero;
    
    private Vector3 velocity = Vector3.zero;

    private bool buildUpPhase = true;
    public void Init(Hitbox target, Vector3 lastHitboxPos, Team team) {
        shot.Init(0, 0, damage, team);
        this.buildUpPhase = true;
        this.target = target;
        this.lastTargetPos = lastHitboxPos;

        shot.SetTeam(team);

        float angleRAD = Random.Range(0, Mathf.PI * 2);
        velocity = new Vector3(Mathf.Cos(angleRAD), Mathf.Sin(angleRAD)) * (Random.Range(0.5f, 1.5f) * disperseSpeed);

    }

    public void Update() {

        if (shot == null) {
            Destroy(this.gameObject);
            return;
        }
        //Debug.Log($"Phase: {buildUpPhase}");
        transform.position += velocity * Time.deltaTime;
        

        if (buildUpPhase) {
            if (target != null) {
                lastTargetPos = target.transform.position;
            }
            velocity *= (1 - speedDampFactor * Time.deltaTime);

            if (velocity.sqrMagnitude < E_THA) {
                buildUpPhase = false;
                Vector3 diff = lastTargetPos - this.transform.position;

                float angleRAD = Mathf.Atan2(diff.y, diff.x);
                velocity = new Vector3(Mathf.Cos(angleRAD), Mathf.Sin(angleRAD)) * aimStartSpeed;
            }
        } else {
            velocity += velocity * (aimSpeedUp * Time.deltaTime);
        }


    }



}
