using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

 public enum ENEMY_STATE {WANDERING, SEARCHING, IDLE, TALKING, EATING, ATTACKING,};
public class EnemyMovementAI : MonoBehaviour
{
    private ENEMY_STATE currentState;

    public void SetState(ENEMY_STATE value)
    {
        currentState = value;
    }

    public ENEMY_STATE GetState()
    {
        return currentState;
    }
    Animator animator;
    AnimatorManager animatorManager;
    [SerializeField] private NavMeshAgent agent;
    [Range(1, 100)] public float speed;
    [Range(1, 500)] public float walkRadius;
    private ENEMY_STATE current_state = ENEMY_STATE.IDLE;
    private float waitTimer = 0.0f;
    private Vector3 targetRotation;

    // Start is called before the first frame update
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animatorManager = GetComponent<AnimatorManager>();
    }

    private void Update()
    {
        MovementAI();
    }

    public void MovementAI()
    {
        switch (current_state)
        {
            case ENEMY_STATE.IDLE:
                //Idle
                DoIdle();
                break;
            case ENEMY_STATE.WANDERING:
                //Wander
                DoWander();
                break;
            case ENEMY_STATE.ATTACKING:
                //Attack
                DoAttack();
                break;
            case ENEMY_STATE.SEARCHING:
                //Search for anything near
                break;
            case ENEMY_STATE.TALKING:
                //look at speaker
                //check if dialogue is over
                //then transition to Idle again
                break;
            case ENEMY_STATE.EATING:
                //Eating
                break;
        }
    }
    private void DoIdle()
    {
        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        agent.SetDestination(RandomNavSphere(transform.position, 10.0f));
        current_state = ENEMY_STATE.WANDERING;
    }

    private void DoAttack(){
        //attack
        //animatorManager.PlayTargetAnimation("Attack", true);
        // animator.SetTrigger("Attack");
    }

    private void DoWander()
    {
        if (agent.pathStatus != NavMeshPathStatus.PathComplete)
            animatorManager.UpdateAnimatorValues(0, GetComponent<Rigidbody>().velocity.magnitude, false);
        return;

        waitTimer = Random.Range(1.0f, 4.0f);
        current_state = ENEMY_STATE.IDLE;
    }

    public void DoTalk(Vector3 rotation)
    {
        transform.LookAt(rotation);
        agent.isStopped = true;
        current_state = ENEMY_STATE.TALKING;
    }

    private Vector3 RandomNavSphere(Vector3 origin, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, 1);
        return navHit.position;
    }

    public void LookAtCharacter(GameObject character)
    {
        //print("looking respectfully");
        //make head look for a certain
    }

    private void OnTriggerEnter(Collider other)
    {
        //"hello!"
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LookAtCharacter(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //make dialogue change
        //"good day!"
    }
}