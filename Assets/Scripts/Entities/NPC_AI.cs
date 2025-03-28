using UnityEngine;
using UnityEngine.AI;

public class NPC_AI : MonoBehaviour
{
    private NavMeshAgent agent; //for navigation of NPC

    private Transform closestEnemy = null;  //store closest enemy

    private LayerMask whatIsTeam; //layer for own team
    private LayerMask whatIsEnemy, whatIsObstacle;  //layers for detection of enemies

    private Animator animator;  //control animations

    //States
    public float sightRange = 4f;   //range in which NPC can see enemies
    public float attackRange = 2f;  //range in which NPC can attack enemies
    public GameObject attackPrefab;
    public float attackDelay = 2f;
    public int attackDamage = 10;
    public bool distanceAttack = true;
    public bool playerInSightRange = false;
    public bool playerInAttackRange = false;
    private float findClosestEnemyCooldown = 1f;    //cooldown to check if current enemy is still closest enemy
    public bool recheckForClosestEnemy = true;  //check if current enemy is still closest enemy
    public bool loseSight = true;   //NPC can lose sight to closest enemy

    //Patroling
    private Vector3 walkPoint;
    private bool walkPointSet = false;
    public float minWalkPointRange = 1f;    //min bound for distance to next walkpoint
    public float maxWalkPointRange = 4f;    //max bound for distance to next walkpoint
    public float minWalkCooldown = 1f;  //min cooldown to find next walkpoint
    public float maxWalkCooldown = 3f;  //max cooldown to find next walkpoint
    private float walkCooldown = 0f;

    //Stats
    public bool isFacingRight = true;   //if sprite's default facing direction is to the right
    private int isFacingRightInt;

    //Attacking
    private float nextAttackTime;

    //team
    Team team;

    private void Start()
    {
        //set facing variables
        if (isFacingRight) isFacingRightInt = 1;
        else isFacingRightInt = -1;

        //setup NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        //setup animator
        animator = GetComponent<Animator>();

        //get obstacle layer
        whatIsObstacle = 1 << LayerMask.NameToLayer("Obstacle");

        //setup other layers
        Entity entitiyScript = GetComponent<Entity>();
        team = entitiyScript.GetTeam();
        if (team == Team.PLAYER)
        {
            whatIsTeam = 1 << LayerMask.NameToLayer("Player");
            whatIsEnemy = 1 << LayerMask.NameToLayer("Enemy");
            SetLayerAllChildren(LayerMask.NameToLayer("Player"));

        } else if (team == Team.ENEMY)
        {
            whatIsEnemy = 1 << LayerMask.NameToLayer("Player");
            whatIsTeam = 1 << LayerMask.NameToLayer("Enemy");
            SetLayerAllChildren(LayerMask.NameToLayer("Enemy"));
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

    private void Update()
    {
        //find closest enemy if enemies are in range
        GetClosestEnemy();

        //rotate according to movement direction,  prevent heavy rotation if npc and enemy are on one line along x-axis
        if (closestEnemy)
        {
            if (Mathf.Abs(closestEnemy.transform.localPosition.x - agent.transform.localPosition.x) >= 0.1f)
            {
                if (agent.velocity.x >= 0.1f) transform.localScale = new Vector3(1 * isFacingRightInt, 1, 1); //moving right
                else if (agent.velocity.x <= -0.1f) transform.localScale = new Vector3(-1 * isFacingRightInt, 1, 1);    //moving left
            }
        } else
        {
            if (agent.velocity.x >= 0.1f) transform.localScale = new Vector3(1 * isFacingRightInt, 1, 1); //moving right
            else if (agent.velocity.x <= -0.1f) transform.localScale = new Vector3(-1 * isFacingRightInt, 1, 1);    //moving left
        }

        //update cooldowns
        walkCooldown -= Time.deltaTime;
        if (recheckForClosestEnemy) findClosestEnemyCooldown -= Time.deltaTime;

        //State machine
        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void GetClosestEnemy()
    {
        //find new closest enemy after cooldown
        if (findClosestEnemyCooldown <= 0)
        {
            FindEnemy();
            findClosestEnemyCooldown = 1f;
            return;
        }

        //find enemy if no enemy stored
        if (!closestEnemy)
        {
            FindEnemy();
            return;
        }

        //check if closest enemy still visible
        Vector2 directionToEnemy = closestEnemy.position - transform.position;
        float distanceToEnemy = directionToEnemy.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToEnemy, distanceToEnemy, whatIsObstacle);
        if (hit.collider != null) //closest enemy not visible anymore, so try to find new enemy
        {
            playerInAttackRange = false;
            if (loseSight) FindEnemy(); //only try to find new enemy if NPC can lose sight to player
            return;
        } 

        //re-check if closest enemy is in attack range
        if (distanceToEnemy <= attackRange) playerInAttackRange = true;
        else playerInAttackRange = false;
    }

    private void FindEnemy()
    {
        Transform prevClosestEnemy = closestEnemy;

        //Reset all values
        closestEnemy = null;
        playerInSightRange = false;
        playerInAttackRange = false;
        float closestDistance = Mathf.Infinity;

        //Find all enemies within sight range
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, sightRange, whatIsEnemy);

        //Find closest visible enemy via raycast
        foreach (Collider2D enemy in enemiesInRange)
        {
            Vector2 directionToEnemy = enemy.transform.position - transform.position;
            float distanceToEnemy = directionToEnemy.magnitude;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToEnemy, distanceToEnemy, whatIsObstacle);
            
            if (hit.collider == null && distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
                playerInSightRange = true;
                if (distanceToEnemy <= attackRange) playerInAttackRange = true;
                else playerInAttackRange = false;
            }
        }

        if (closestEnemy != null) return;

        //if no closest enemy found and enemy cannot lose sight, re-assign previous closest enemy if available
        if (prevClosestEnemy == null) return;
        if (!loseSight)
        {
            closestEnemy = prevClosestEnemy;
            playerInSightRange = true;
            Vector2 directionToEnemy = prevClosestEnemy.position - transform.position;
            float distanceToEnemy = directionToEnemy.magnitude;
            if (distanceToEnemy <= attackRange) playerInAttackRange = true;
            else playerInAttackRange = false;
        }
    }

    private void Patroling()
    {
        //check if walk cooldown is reached
        if (walkCooldown > 0f) return;

        //Set new walkpoint or go to walkpoint
        if (!walkPointSet) SearchWalkPoint();
        else agent.SetDestination(walkPoint);

        //manage animations
        animator.SetBool("walk", true); //play walk animation
        animator.SetBool("attack", false); //deactivate attack animation

        //Check if walkpoint is reached
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;   //reset walkpoint
            animator.SetBool("walk", false);    //play idle animation
            walkCooldown = Random.Range(minWalkCooldown, maxWalkCooldown);  //generate random walk cooldown within bounds
            agent.SetDestination(transform.position);   //avoid moving after arriving at walkpoint
        }
            
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomValue = Random.Range(minWalkPointRange, maxWalkPointRange);
        bool isNegative = Random.value > 0.5f;
        float randomX = isNegative ? -randomValue : randomValue;
        randomValue = Random.Range(minWalkPointRange, maxWalkPointRange);
        isNegative = Random.value > 0.5f;
        float randomY = isNegative ? -randomValue : randomValue;
        Vector3 newWalkPoint = new Vector3(transform.position.x + randomX, transform.position.y + randomY, transform.position.z);

        //Calculate point on NavMesh closest to random point
        NavMeshHit hit;
        if (NavMesh.SamplePosition(newWalkPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        walkPointSet = false;   //reset walkpoint
        if (closestEnemy) agent.SetDestination(closestEnemy.position);    //follow player
        animator.SetBool("walk", true); //play walk animation
        animator.SetBool("attack", false); //deactivate attack animation
    }

    private void AttackPlayer()
    {
        //Orient towards enemy, prevent heavy rotation if npc and enemy are on one line along x-axis
        if (closestEnemy)
        {
            if (Mathf.Abs(closestEnemy.transform.localPosition.x - agent.transform.localPosition.x) >= 0.1f)
            {
                if (agent.transform.localPosition.x - closestEnemy.localPosition.x <= 0) transform.localScale = new Vector3(1 * isFacingRightInt, 1, 1); //face right
                else transform.localScale = new Vector3(-1 * isFacingRightInt, 1, 1);    //face left
            }
        }
        else
        {
            if (agent.transform.localPosition.x - closestEnemy.localPosition.x <= 0) transform.localScale = new Vector3(1 * isFacingRightInt, 1, 1); //face right
            else transform.localScale = new Vector3(-1 * isFacingRightInt, 1, 1);    //face left
        }
        
        //No movement while attacking
        agent.SetDestination(transform.position);   //do not move
        animator.SetBool("walk", false);    //deactivate walk animation

        animator.SetBool("attack", true); //play attack animation

        if (Time.time > nextAttackTime) {
            if (distanceAttack)
            {
                GameObject attack = Instantiate(attackPrefab, transform.position, Quaternion.identity);
                BulletCollection attackScript = attack.GetComponent<BulletCollection>();
                if (attackScript) attackScript.Init(team, attackDamage);
                DelayedAimedBulletDisperser attackScriptDelayedAimDisperser = attack.GetComponent<DelayedAimedBulletDisperser>();
                if (attackScriptDelayedAimDisperser) attackScriptDelayedAimDisperser.Init(team);
            }
            else attackPrefab.SetActive(true);

            nextAttackTime = Time.time + attackDelay;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
