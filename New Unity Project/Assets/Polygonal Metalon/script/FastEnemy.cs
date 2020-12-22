
using UnityEngine;
using UnityEngine.AI;

public class FastEnemy : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public Object stab;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;
    //damgedeal
    public float damagedeal = 5f;
    //Patroling
    public Vector3 walkPoint; 	//điểm đến
    bool walkPointSet;		    //bool để hoạt động
    public float walkPointRange;//tầm hoạt động

    //Attacking
    public float timeBetweenAttacks;	//delay attack
    bool alreadyAttacked= false;		        //bool để delay
    public GameObject projectile;	

    //States
    public float sightRange, attackRange; 		        // tầm nhìn và tầm bắn
    public bool playerInSightRange, playerInAttackRange;//bool hoạt động
    //public bool patroling, chasing, attacking;
    public Animator anim;
    
    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        anim.speed = 2f;
    }
    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        //TakeDamage(1);
        
            if (!playerInSightRange && !playerInAttackRange)
            {
                Patroling();   //tuần tra
                               //patroling = true;
            }
            else
            {
            //patroling = false;
                anim.SetBool("Walk Forward", false);
            }
            if (playerInSightRange && !playerInAttackRange)
            {
                ChasePlayer();  //rượt
                                //chasing = true;
            }
            else
            {
                //chasing = false;
                anim.SetBool("Run Forward", false);
            }

            if (playerInAttackRange && playerInSightRange)
            {
                AttackPlayer();  //tấn công
                                 //attacking = true;
                
            }
            //else attacking = false;
        

        }
    

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.speed = 3;
            agent.SetDestination(walkPoint);
            anim.SetBool("Walk Forward", true);
        }
            
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            anim.SetBool("Walk Forward", false);
            walkPointSet = false;
            //for(int i=0;i<100; i++) { }
            
        }
           
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        //agent.isStopped = false;
        agent.SetDestination(player.position);
        agent.speed = 8;
        anim.SetBool("Run Forward", true);
    }
    
    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        //agent.SetDestination(transform.position);
        agent.isStopped = true;
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            ///Attack code here
            anim.SetTrigger("Stab Attack");
            ///End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
        agent.isStopped = false;
    }

    public void TakeDamage(int damage)
    {
        agent.isStopped = true;
        health -= damage;
        anim.SetTrigger("Take Damage");
        if (health <= 0)
            Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void DestroyEnemy()
    {
        agent.isStopped = true;
        anim.SetTrigger("Die");
        Destroy(gameObject,10);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    private void activateAtk()
    {

    }
    private void deactivateAtk()
    {

    }
}
