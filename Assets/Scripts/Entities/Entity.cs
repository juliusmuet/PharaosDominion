using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class Entity : ITeam
{
    [SerializeField] private int maxHp = 50;
    private int baseMaxHp;
    private int hp = 0;

    [SerializeField] private int touchDamage = 10;

    [SerializeField] private int costs = 100;

    private float defenseMultiplier = 1.0f;

    public delegate void OnDeath(Entity entity);

    private (int, int) bucketData;
    private Hitbox hitbox;

    private OnDeath onDeath;

    private EntityMaterial entityMaterial;  //for damage effect

    private HealthBar healthBar;    //update healthbar on taking damage

    public static UnityEvent<Entity> OnEntityKilled = new UnityEvent<Entity>();  //death event

    public PlayerTeamNPCData playerTeamNPCData; //stored for player team entities to update the health in the data class

    private DamageSound damageSound;
    private float damageSoundCooldown = 0.75f;
    private float damageSoundThreshold = 0.75f;

    private bool isInvincible = false;

    public void Start() {
        transform.rotation = Quaternion.identity;   //fix bug where rotation.x is sometimes -90 at start of level   
        
        if (hp == 0) hp = maxHp;
        baseMaxHp = maxHp;

        if (GetTeam() == Team.ENEMY) {
            hp *= (int)ChallengeController.Instance.enemyHealthModificator;
            maxHp *= (int)ChallengeController.Instance.enemyHealthModificator;
        }

        hitbox = GetComponent<Hitbox>();
        bucketData = SpacialGrouping.currentGrouping.Add(hitbox);

        entityMaterial = GetComponent<EntityMaterial>();

        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar)
            healthBar.updateHealthBar(hp, maxHp);   //set healthbar at creation

        if (GetTeam() == Team.PLAYER)
        {
            ParticleSystem particles = GetComponentInChildren<ParticleSystem>();
            if (particles != null) particles.Stop();    //disable enemy particles if in player's team
            defenseMultiplier = PlayerPrefs.GetFloat("TeamDefenseMultiplier", 1.0f);
        }

        damageSound = GetComponent<DamageSound>();
    }

    public void Update() {
        // Debug.Log($"Team: {this.name} {GetTeam()}");

        bucketData = SpacialGrouping.currentGrouping.UpdateHitboxBucket(bucketData, hitbox);

        damageSoundCooldown += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.I) && CompareTag("Player"))
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (isInvincible)
            {
                isInvincible = false;
                spriteRenderer.color = Color.white;
            }
            else
            {
                isInvincible = true;
                spriteRenderer.color = Color.yellow;
            }
        }
    }

    public void AddOnDeathEvent(OnDeath func) {
        onDeath += func;
    }

    public void Damage(int amount) {
        if (isInvincible) return;
        
        amount = (int)(amount / defenseMultiplier);
        hp -= amount;

        //Do not die during Tutorial
        if (Tutorial.tutorialActivated && PlayerMovement.Instance.GetEntity() == this && hp <= 0) {
            hp = 1;
        }

        entityMaterial.startDamageFlash();  //damage animation
        
        if (healthBar != null)
            healthBar.updateHealthBar(hp, maxHp);   //update healthbar

        if (damageSoundCooldown >= damageSoundThreshold)
        {
            damageSound.PlayRandomDamageSound();    //damage sound
            damageSoundCooldown = 0f;
        }

        if (GetTeam() == Team.PLAYER) {
            playerTeamNPCData.health = hp;    //update the health in the data class if player team npc
            if (PlayerMovement.Instance.gameObject == this.gameObject)
                CameraController.Instance.Shake();
        }

        //Debug.Log("HP: " + hp);
        if (hp <= 0) {
            SpacialGrouping.currentGrouping.Remove(hitbox, bucketData);

            if (onDeath != null) onDeath(this);

            if (GetTeam() != Team.PLAYER) MoneyManager.instance.CreateRandomCoinAtPosition(transform);    //create coin on death position

            OnEntityKilled.Invoke(this); //invoke death event

            Debug.Log("Killed enemy: " + name);
            Destroy(this.gameObject);
            return;
        }
    }

    public int GetCurrentHP()
    {
        return hp;
    }

    public void SetHP(int hp_value)
    {
        Debug.Log("SET HP for: " + name + " with value " + hp_value);
        hp = Mathf.Min(hp_value, GetMaxHP());
        if (healthBar != null) healthBar.updateHealthBar(hp, maxHp);
    }

    public void SetMaxHp(int maxHP, int currentHp) {
        Debug.Log("SET MAX HP for: " + name + " with value " + maxHP);
        this.maxHp = maxHP;
        this.hp = currentHp;
    }

    public int GetMaxHP()
    {
        return maxHp;
    }
    public int GetBaseMaxHp() {
        if (baseMaxHp == 0)
            return maxHp;
        return baseMaxHp;
    }

    public int GetCosts()
    {
        return costs;
    }

    public Hitbox GetHitbox() {
        return hitbox;
    }
    public int GetTouchDamage() {
        return touchDamage;
    }

}
