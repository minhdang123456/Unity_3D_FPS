
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MeleeEnemy : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health = 100;
    //damgedeal
    public float damagedeal = 5f;
    //Patroling
    public Vector3 walkPoint; 	//điểm đến
    bool walkPointSet;		    //bool để hoạt động
    public float walkPointRange = 30;//tầm hoạt động

    //Attacking
    public float timeBetweenAttacks = 1;	//delay attack
    bool alreadyAttacked = false;		        //bool để delay
    public GameObject projectile;

    //States
    public float sightRange = 30, attackRange = 3.5f; 		        // tầm nhìn và tầm bắn
    public bool playerInSightRange, playerInAttackRange;//bool hoạt động
    //public bool patroling, chasing, attacking;
    public Animator anim;
    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        anim.speed = 1f;
    }
    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        //TakeDamage(1);
        MovControll();
        if (health <= 0)
            Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void MovControll()
    {
        if (!playerInSightRange && !playerInAttackRange)
        {
            Patroling();   //tuần tra
                           //patroling = true;
        }

        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();  //rượt
                            //chasing = true;
        }


        if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer();  //tấn công
                             //attacking = true;

        }
        anim.SetFloat("speed", agent.speed);
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.speed = 5;
            agent.SetDestination(walkPoint);
            //anim.SetBool("Walk Forward", true);

        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            //anim.SetBool("Walk Forward", false);
            Waiting(30);
            walkPointSet = false;           
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
        agent.speed = 12;
        //anim.SetBool("Run Forward", true);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        //agent.SetDestination(transform.position);

        agent.isStopped = true;
        agent.speed = 0;
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            ///Attack code here
            agent.speed = 0;
            anim.SetTrigger("Stab Attack");
            ///End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
        ContinueMoving();
    }

    public float immuDamageTime = 0.5f;

    public void TakeDamage(int damage)
    {
        //agent.isStopped = true;
         //ngung hd 0.5s
        health -= damage;
        anim.SetTrigger("Take Damage");
        agent.isStopped = true;
        agent.speed = 0;
        Waiting(4);
        ContinueMoving();
    }

    private IEnumerator Waiting(int time) {
        agent.isStopped = true;
        yield return new WaitForSeconds(time/2);
    }

    private void ContinueMoving()
    {
        agent.isStopped = false;
    }
    private void DestroyEnemy()
    {
        agent.SetDestination(transform.position);
        anim.SetTrigger("Die");
        //Waiting(20);
        Destroy(gameObject,5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
