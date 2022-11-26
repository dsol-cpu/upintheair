using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMovementAI : MonoBehaviour
{
	Animator animator;
	AnimatorManager animatorManager;
	[SerializeField] private NavMeshAgent agent;
	[Range(1, 100)] public float speed;
	[Range(1, 500)] public float walkRadius;

	private NPC_STATE current_state = NPC_STATE.IDLE;
	private float waitTimer = 0.0f;
	private Vector3 targetRotation;

	public enum NPC_STATE
	{
		WANDERING,
		SEARCHING,
		IDLE,
		TALKING,
		EATING,
	}
	
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
			case NPC_STATE.IDLE:
				//Idle
				DoIdle();
				break;
			case NPC_STATE.WANDERING:
				//Wander
				DoWander();
				break;
			case NPC_STATE.SEARCHING:
				//Search for anything near
				break;
			case NPC_STATE.TALKING:
				//look at speaker

				//check if dialogue is over
				//then transition to Idle again
				break;
			case NPC_STATE.EATING:
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
		current_state = NPC_STATE.WANDERING;
	}

	private void DoWander()
	{
		if (agent.pathStatus != NavMeshPathStatus.PathComplete)
			animatorManager.UpdateAnimatorValues(0, GetComponent<Rigidbody>().velocity.magnitude, false);
			return;

		waitTimer = Random.Range(1.0f, 4.0f);
		current_state = NPC_STATE.IDLE;
	}

	public void LookAtSpeaker(Vector3 rotation)
	{
		transform.LookAt(rotation);
		agent.isStopped = true;
		current_state = NPC_STATE.TALKING;
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
		if (other.CompareTag("Player") || other.CompareTag("NPC"))
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