using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollection : MonoBehaviour
{
    [SerializeField] private BasicShot[] basicShots;

    [SerializeField] private Animator anim;

    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;

    [SerializeField, Range(0, 1f)] private float predictPlayerMovement;

    private bool buildUpPhaseFinished = false;
    private Vector3 dir;
    private Team team;

    private bool lockOnPlayer = false;
    private Vector3 targetOffs = Vector3.zero;

    public void FixedUpdate() {
        if (!CheckIfBulletsExist()) {
            Destroy(this.gameObject);
            return;
        }

        if (buildUpPhaseFinished) {
            Move();
        }

    }

    public void Init(Team team, int damage = 10)
    {
        this.team = team;

        if (this.team == Team.ENEMY) {
            this.speed *= ChallengeController.Instance.bulletSpeedMultiplicator;
            //Debug.Log($"Speed {this.speed} Mul {ChallengeController.Instance.bulletSpeedMultiplicator}");
        }

        float buildUpLength = anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        StartCoroutine(EnterFirePhase(buildUpLength));

        foreach (BasicShot shot in basicShots)
        {
            shot.Init(0, 0, damage, team);
        }
    }

    public void LockOnPlayer() {
        lockOnPlayer = true;
    }

    public void SetRotationSpeed(float speed) {
        rotationSpeed = speed;
    }

    public void SetTargetOff(Vector3 targetOff) {
        this.targetOffs = targetOff;
    }

    private void Move() {
        transform.position += (speed * Time.fixedDeltaTime) * dir;
        if (rotationSpeed != 0) {
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + rotationSpeed * Time.fixedDeltaTime);
        }

    }

    private bool CheckIfBulletsExist() {
        for (int i = 0; i < basicShots.Length; i++)
            if (basicShots[i] != null)
                return true;
        return false;
    }

    private IEnumerator EnterFirePhase(float delay) {
        yield return new WaitForSeconds(delay);

        Hitbox hitbox = SpacialGrouping.currentGrouping.GetClosestHitbox(transform.position, 100, team.GetOpposingTeam());

        if (hitbox == null) {
            foreach(BasicShot s in basicShots) {
                s.DestroyThis();
            }
            Destroy(this.gameObject);
            Debug.Log("Hitbox Unavailable Problem");
            yield break;
        }

        Vector3 hitPos;

        if ( (predictPlayerMovement == 0 || PlayerMovement.Instance.GetPlayerHitbox() != hitbox) && !lockOnPlayer) {
            hitPos = hitbox.GetCenter() + targetOffs;


        } else {
            hitPos = PlayerMovement.Instance.transform.position + targetOffs;

            float dist = (hitPos - transform.position).magnitude;
            float time = dist / speed;

            hitPos += PlayerMovement.Instance.GetPlayerPredictiveSpeed() * predictPlayerMovement * time;

            /*Vector3 playerPos = PlayerMovement.Instance.transform.position;
            Vector3 playerSpeed = PlayerMovement.Instance.GetPlayerPredictiveSpeed() * predictPlayerMovement;
            float a = playerPos.x,
                b = playerPos.y,
                c = transform.position.x,
                d = transform.position.y,
                e = playerSpeed.x,
                f = playerSpeed.y,
                g = speed;

            if (playerSpeed.sqrMagnitude == speed * speed) {
                g *= 1.1f; // Cheeky hack dont mind the math
            }
            float sqrt = Mathf.Sqrt(
                4 * Mathf.Pow( (a * e + f * (b - d) - c * e), 2) - 4 * (e * e + f * f - g * g) * (a * a - 2 * a * c + b * b - 2 * b * d + c * c + d * d)
                );
            if (sqrt is float.NaN) {
                sqrt = 0;
            }

            float t1 = (-1 / 2 * sqrt - a * e - b * f + c * e + d * f) / (e * e + f * f - g * g);
            float t2 = (1 / 2 * sqrt - a * e - b * f + c * e + d * f) / (e * e + f * f - g * g);

            float t = Mathf.Max(t1, t2);


            if (t < 0) {
                Debug.Log("FAILED");
                t = -1 * t;
            }

            if (t < 0) {
                hitPos = playerPos;
            } else {
                hitPos = playerPos + playerSpeed * t;
            }
            Debug.Log($"t: {t} t1: {t1} t2: {t2} sqrt: {sqrt} div: {(e * e + f * f - g * g)} minus: {a * e - b * f + c * e + d * f} ps: {playerSpeed}");*/
        }
        float dir = Mathf.Atan2(-transform.position.y + hitPos.y, -transform.position.x + hitPos.x);
        this.dir = new Vector3(Mathf.Cos(dir), Mathf.Sin(dir));

        buildUpPhaseFinished = true;


    }
}
