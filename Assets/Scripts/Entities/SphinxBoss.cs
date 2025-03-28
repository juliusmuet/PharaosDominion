using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphinxBoss : MonoBehaviour
{
    private static readonly Vector3 INVISIBLE_POS = new Vector3(1000, 1000, 0);

    private Entity ent;
    private SphinxBossStates state = SphinxBossStates.WAIT;
    private bool update = true;
    [SerializeField] private float minTime = 0.2f, maxTime = 4f;
    [SerializeField, Range(0f, 1f)] private float tpChance = 0.25f;
    [SerializeField] private int hieroglyphAttackAmount = 5;

    [SerializeField] private int mulitShotAttackShotsPerShot = 3;
    [SerializeField] private int mulitShotAttackShotsPerSalve = 3;
    [SerializeField] private int mulitShotAttackSaleves = 3;
    [SerializeField] private float mulitShotAttackBulletSpeed = 15;
    [SerializeField] private int mulitShotAttackBulletDamage = 10;


    [Header("Static links")]
    [SerializeField] private GameObject gattlinGunAttackPrefab;
    [SerializeField] private GameObject[] hieroglyphAttackPrefabs;
    private Transform[] tpPositions;
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private GameObject basicShotPrefab;
    [SerializeField] private Animator anim;
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private GameObject tpVFX_Prefab;
    [SerializeField] private GameObject deathScenePrefab;

    private LinkedList<GameObject> clones = new LinkedList<GameObject>();

    public void Start() {
        ent = GetComponent<Entity>();
        ent.AddOnDeathEvent(PlayDeathScene);
        tpPositions = SphinxBossTPs.Instance.tps;

        Debug.Log("Sphinxboss spawned");

        SetLayerAllChildren(LayerMask.NameToLayer("Enemy"));
    }

    private void PlayDeathScene(Entity ent) {
        Instantiate(deathScenePrefab, transform.position, Quaternion.identity);

    }

    public void Update() {
        if (!update)
            return;
        Debug.Log("Sphinxboss State Changed");

        update = false;

        switch(state) {
            case SphinxBossStates.WAIT:
                StartCoroutine(WaitState());
                break;
            case SphinxBossStates.TELEPORT:
                StartCoroutine(TeleportState());
                break;
            case SphinxBossStates.ATTACK:
                if (Random.value > 0.33f)
                    if (Random.value > 0.5f)
                        StartCoroutine(AttackGattlinGun());
                    else
                        StartCoroutine(AttackMultiShot());
                else
                    StartCoroutine(AttackHieroglyphs());
                break;
        }
    }

    void SetLayerAllChildren(LayerMask layer)
    {
        var children = GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children)
        {
            child.gameObject.layer = layer;
        }
    }

    private IEnumerator WaitState() {
        float waitTime = (maxTime - minTime) * (ent.GetCurrentHP() / (float)ent.GetMaxHP()) + minTime;
        yield return new WaitForSeconds(waitTime);
        if (Random.value < tpChance)
            state = SphinxBossStates.TELEPORT;
        else
            state = SphinxBossStates.ATTACK;
        update = true;
    }

    private IEnumerator TeleportState() {
        Vector3 lastPos = transform.position;

        Instantiate(tpVFX_Prefab, transform.position, Quaternion.identity);
        transform.position = INVISIBLE_POS;
        

        yield return new WaitForSeconds(0.25f);
        // Spawn reappear particle effect
        yield return new WaitForSeconds(0.25f);

        int tpIndex;
        do {
            tpIndex = Random.Range(0, tpPositions.Length);
            transform.position = tpPositions[tpIndex].position;
        } while(transform.position == lastPos);
        Instantiate(tpVFX_Prefab, transform.position, Quaternion.identity);
        renderer.flipX = transform.position.x > 0;

        while(clones.Count > 0) {
            GameObject first = clones.First.Value;
            clones.RemoveFirst();
            if (first != null)
                Destroy(first);
        }

        if (ent.GetCurrentHP() < ent.GetMaxHP() / 2) {
            for (int i = 0; i < tpPositions.Length; i++) {
                if (i == tpIndex)
                    continue;
                GameObject clone = Instantiate(clonePrefab, tpPositions[i].position, Quaternion.identity, null);
                Instantiate(tpVFX_Prefab, tpPositions[i].position, Quaternion.identity);
                if (clone.transform.position.x > 0) {
                    clone.GetComponent<SpriteRenderer>().flipX = true;
                }
                clones.AddLast(clone);
            }
        }

        state = SphinxBossStates.WAIT;
        update = true;

    }

    private IEnumerator AttackGattlinGun() {
        anim.SetTrigger("att2");
        Instantiate(gattlinGunAttackPrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(4f);

        state = SphinxBossStates.WAIT;
        update = true;
    }

    private IEnumerator AttackHieroglyphs() {
        anim.SetTrigger("att2");
        for (int i = 0; i < hieroglyphAttackAmount; i++) {
            Vector3 offs = new Vector3(Random.Range(-2, 2), Random.Range(-2, 2) - 1);
            BulletCollection attack = Instantiate(hieroglyphAttackPrefabs[Random.Range(0, hieroglyphAttackPrefabs.Length)], transform.position + offs, Quaternion.identity).GetComponent<BulletCollection>();

            offs = new Vector3(Random.Range(-2, 2), Random.Range(-2, 2));
            attack.LockOnPlayer();
            attack.SetTargetOff(offs);
            attack.SetRotationSpeed(Random.Range(-270, 270));
            attack.Init(Team.ENEMY);

            yield return new WaitForSeconds(0.5f);
        }

        state = SphinxBossStates.WAIT;
        update = true;
    }

    private IEnumerator AttackMultiShot() {
        anim.SetTrigger("att1");
        for (int salve = 0; salve < this.mulitShotAttackSaleves; salve++) {
            for (int bigShots = 0; bigShots < this.mulitShotAttackShotsPerSalve; bigShots++) {

                //Vector3 offs = new Vector3(Random.Range(-2, 2), Random.Range(-2, 2) - 1);

                Vector3 playerPos = PlayerMovement.Instance.transform.position;
                Vector3 diff = (playerPos - transform.position);
                float timeToPlayer = (diff.magnitude) / mulitShotAttackBulletSpeed;
                Vector3 targetPos = playerPos + PlayerMovement.Instance.GetPlayerPredictiveSpeed() * timeToPlayer;
                diff = (targetPos - transform.position);

                float angleToShootAt = Mathf.Atan2(diff.y, diff.x);

                for (int shot = 0; shot < this.mulitShotAttackShotsPerShot; shot++) {
                    Instantiate(basicShotPrefab, transform.position, Quaternion.identity).GetComponent<BasicShot>().Init(angleToShootAt, mulitShotAttackBulletSpeed, mulitShotAttackBulletDamage, Team.ENEMY);
                    yield return new WaitForFixedUpdate();
                }

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.5f);

        }
        state = SphinxBossStates.WAIT;
        update = true;
    }

    enum SphinxBossStates {
        WAIT,
        TELEPORT,
        ATTACK
    }
}
