using UnityEngine;

/// <summary>
/// Attached to the enemy characters in the game.
/// </summary>
public class Enemy : MonoBehaviour
{
	[SerializeField] private EnemyScriptableObj enemyScriptableObj; 
	//[SerializeField] private DialogueRunner dialogueRunner;
	private EnemyMovementAI movementAI;

	private void Start()
	{
		//dialogueRunner = FindObjectOfType<DialogueRunner>();
		movementAI = GetComponent<EnemyMovementAI>();
	}

	private void Update()
	{

	}

	public void Talk(Vector3 rotation)
	{
		movementAI.DoTalk(rotation);
	}

	public void Attack()
	{
		movementAI.SetState(ENEMY_STATE.ATTACKING);
	}

	private void OnTriggerEnter(Collider other)
	{
		//"hello!"
	}

	private void OnTriggerStay(Collider other)
	{

	}

	private void OnTriggerExit(Collider other)
	{
		//dialogueRunner.IsDialogueRunning = false;
		//make dialogue change
		//"good day!"
	}
}