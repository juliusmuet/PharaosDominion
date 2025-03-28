using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public static PlayerMovement Instance { get; private set; }

    [Header("Entity stuff")]
    [SerializeField] private Entity entity;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float invizDuration = 0.5f;
    private float currentInviz = 0f;
    [SerializeField] private Hitbox hitbox;
    [SerializeField] private BasicWeapon weapon;

    [Header("Animation")]
    [SerializeField] private Transform animationBase;
    private Animator anim;

    [SerializeField] public Sprite baseSprite;

    [SerializeField] private AudioClip stepsAudioFX;

    private Vector3 lastMovementAxisInput;

    private static readonly int PLAYER_PREDICTION_FRAME_BUFFER = 30;
    private Queue<Vector3> latestPlayerPositions = new Queue<Vector3>();
    private Vector3 currentPlayerPredictiveSpeedPerFixedUpdateFrame;
    private Rigidbody2D rb;

    [SerializeField] private AnimationCurve mummySpeedModificator;
    private int mummyCurseWiggleDir = -1;
    [SerializeField] private float mummyCurseSpeedCycleDuration;
    private float mummyCycleStarted;
    private bool keyInputEnabled = true;
  
    public void Awake() {
        
        Instance = this;
        anim = animationBase.GetComponent<Animator>();
        speed *= PlayerPrefs.GetFloat("PlayerMovementSpeedMultiplier", 1.0f);
        this.rb = GetComponent<Rigidbody2D>();

    }

    private void Start() {
        if (PlayerTeamManager.instance != null && PlayerTeamManager.instance.playerBaseHp == -1) {
            PlayerTeamManager.instance.playerBaseHp = entity.GetBaseMaxHp();
            PlayerTeamManager.instance.playerCurrentHP = entity.GetCurrentHP();
            PlayerTeamManager.instance.playerMaxHp = entity.GetMaxHP();
        } else {
            entity.SetMaxHp(PlayerTeamManager.instance.playerMaxHp, PlayerTeamManager.instance.playerCurrentHP);
        }
        mummyCycleStarted = Time.time;
    }

    public void FixedUpdate() {
        if (keyInputEnabled)
        {
            UpdatePlayerMovement();
            CheckCollisionWithEnemies();

            UpdatePlayerPositionPrediction();
        }
    }

    private void UpdatePlayerPositionPrediction() {
        latestPlayerPositions.Enqueue(transform.position);

        Vector3 oldPlayerPos = latestPlayerPositions.Peek();
        if (latestPlayerPositions.Count > PLAYER_PREDICTION_FRAME_BUFFER) {
            latestPlayerPositions.Dequeue();
        }

        currentPlayerPredictiveSpeedPerFixedUpdateFrame = (transform.position - oldPlayerPos) / (PLAYER_PREDICTION_FRAME_BUFFER * Time.fixedDeltaTime);

    }

    public void Update() {
        weapon.PullTrigger();
    }

    public Vector3 GetPlayerPredictiveSpeed() {
        return currentPlayerPredictiveSpeedPerFixedUpdateFrame;
    }

    private void CheckCollisionWithEnemies() {
        if(currentInviz <= 0) {
            Hitbox collider = SpacialGrouping.currentGrouping.CollisionWith(hitbox);
            if (collider != null) {
                Entity ent = collider.GetComponent<Entity>();
                if (ent != null) {
                    entity.Damage(ent.GetTouchDamage());
                    //Debug.Log("PlayerHit!");
                    currentInviz = invizDuration;
                }
            }
        } else {
            currentInviz -= Time.fixedDeltaTime;
        }
    }

    private void UpdatePlayerMovement() {
        lastMovementAxisInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        float currentSpeed = speed;
        if (PlayerTeamManager.instance.isMummyCursed) {
            float eval = mummySpeedModificator.Evaluate((Time.time - mummyCycleStarted) / mummyCurseSpeedCycleDuration);
            //Debug.Log($"Eval = {eval} at: {(Time.time - mummyCycleStarted) / mummyCurseSpeedCycleDuration}");
            currentSpeed *= eval;
            if (eval < 0.25) {
                transform.position += new Vector3(0.1f, 0) * mummyCurseWiggleDir;
                mummyCurseWiggleDir *= -1;
            }
        }
        if (lastMovementAxisInput != Vector3.zero) {
            SetDirection(animationBase, lastMovementAxisInput.x > 0);
            //transform.position += lastMovementAxisInput * speed * Time.fixedDeltaTime;
            
            rb.velocity = lastMovementAxisInput * currentSpeed;
            anim.SetBool("walk", true);
        } else {
            anim.SetBool("walk", false);
            rb.velocity = Vector3.zero;
        }


    }

    public Entity GetEntity() {
        return entity;
    }



    public Hitbox GetPlayerHitbox() {
        return hitbox;
    }

    public static void SetDirection(Transform trans, bool right) {
        trans.localScale = new Vector3(right ? 1 : -1, 1, 1);
        //trans.localRotation = Quaternion.Euler(0, right ? 0 : 180, trans.localRotation.eulerAngles.z);
    }

    public void InitialisePositon(Transform startTransform, bool keyInputEnabled = true)
    {
        transform.position = startTransform.position;
        this.keyInputEnabled = keyInputEnabled;
    }

    public IEnumerator EnterLevel(Transform startTransform, Transform endTransform, float duration, System.Action onComplete)
    {
        float elapsedTime = 0f;

        transform.position = startTransform.position;

        AudioManager.instance.PlaySoundFX(stepsAudioFX);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            anim.SetBool("walk", true);
            transform.position = Vector3.Lerp(startTransform.position, endTransform.position, t);
            yield return null;
        }

        transform.position = endTransform.position;

        anim.SetBool("walk", false);

        keyInputEnabled = true;

        onComplete?.Invoke();
    }

}
