using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicShot : ITeam
{
    public static readonly int COLLISION_CHECK_DELAY_IN_FRAMES = 3;
    private static readonly float TIME_ALIVE = 10f;
    [SerializeField] private bool adjustRotation = true;

    [SerializeField] private Color playerTeamColor= Color.blue;
    [SerializeField] private Color enemyTeamColor = Color.red;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private GameObject shotDestroyVFX;

    private float speed;
    private int damage;

    private Hitbox hitbox;
    private Vector3 direction;

    private float destructionTime;

    private DoEveryXFrame collisionCheck;

    private bool destroyThis = false;
    protected virtual void Awake() {
        hitbox = GetComponent<Hitbox>();
        collisionCheck = new DoEveryXFrame(COLLISION_CHECK_DELAY_IN_FRAMES, CollisionCheck);
    }

    public void Init(float angleRAD, float speed, int damage, Team team) {
        this.speed = speed;

        if (team == Team.ENEMY) {
            this.speed *= ChallengeController.Instance.bulletSpeedMultiplicator;
        }

        this.damage = damage;
        this.SetTeam(team);

        float angleDEG = angleRAD * Mathf.Rad2Deg;
        this.direction = new Vector3(Mathf.Cos(angleRAD), Mathf.Sin(angleRAD));
        if (adjustRotation) {
            transform.rotation = Quaternion.Euler(0, 0, angleDEG);
        }

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = this.GetTeam() == Team.PLAYER ? playerTeamColor : enemyTeamColor;

        destructionTime = Time.time + TIME_ALIVE;
    }

    protected virtual void FixedUpdate() {
        
        collisionCheck.UpdateFrame();
        if (destroyThis) {
            DestroyThis();
            return;
        }

        transform.position += Time.fixedDeltaTime * speed * direction;

        if (Physics2D.OverlapCircle(hitbox.GetCenter(), hitbox.GetRadius(), Factory.Instance.walls)) {
            DestroyThis();
            return;
        }

        if (destructionTime <= Time.time) {
            DestroyThis();
            return;
        }
    }

    public void CollisionCheck() {
        //Debug.Log($"Hitbox: {hitbox}");
        Hitbox hit = SpacialGrouping.currentGrouping.CollisionWith(hitbox);
        if (hit != null) {
            hit.GetComponent<Entity>()?.Damage(damage);
            destroyThis = true;
        }
    }
    public void DestroyThis() {
        Instantiate(shotDestroyVFX, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}


public enum Team { ENEMY, PLAYER, NONE }

public static class TeamExtension {
    public static Team GetOpposingTeam(this Team team) {
        if (team == Team.ENEMY)
            return Team.PLAYER;
        else if (team == Team.PLAYER)
            return Team.ENEMY;
        else
            return Team.NONE;
    }
}
