using UnityEngine;
//using Yarn.Unity;

/// <summary>
/// Attached to the non-player characters, and stores the name of the Yarn
/// node that should be run when you talk to them.
/// </summary>
public class NPC : MonoBehaviour
{
	public string characterName = "";
	public DialogueNode talkToNode;
	//[SerializeField] private DialogueRunner dialogueRunner;
	private NPCMovementAI movementAI;

	private void Start()
	{
		//dialogueRunner = FindObjectOfType<DialogueRunner>();
		movementAI = GetComponent<NPCMovementAI>();
	}

	private void Update()
	{
		
	}

	public void LookAtSpeaker(Vector3 rotation)
	{
		movementAI.LookAtSpeaker(rotation);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{

		}
		//"hello!"
	}

	void OnTriggerStay(Collider other) 
	{
		if (other.CompareTag("Player"))
		{

		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			// dialogueManager.IsDialogueRunning = false;
		}
		
		//make dialogue change
		//"good day!"
	}
}

