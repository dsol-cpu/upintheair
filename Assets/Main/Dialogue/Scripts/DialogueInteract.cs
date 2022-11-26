using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
#if USE_INPUTSYSTEM && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
public class DialogueInteract : MonoBehaviour, PlayerMovement.IDialogueActions
{
	public float interactionRadius = 2.0f;

	// because we are using the same button press for both starting and skipping dialogue they collide
	// so we are going to make it so that the input gets turned off
	[SerializeField] GameObject dialogueGUIPrefab;
	DialogueManager dialogueManager;
	PlayerMovement dialogueInteract;
	NPC currentNPC;

	void Start()
	{
		dialogueManager = dialogueGUIPrefab.transform.Find("Dialogue UI").gameObject.GetComponent<DialogueManager>();
	}

	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update()
	{

	}

	/// <summary>
	/// Draw the range at which we'll start talking to people.
	/// </summary>
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;

		// Flatten the sphere into a disk, which looks nicer in 2D
		// games
		Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1, 1, 1));

		// Need to draw at position zero because we set position in the
		// line above
		Gizmos.DrawWireSphere(Vector3.zero, interactionRadius);
	}

	void OnEnable()
	{
		if (dialogueInteract == null)
		{
			dialogueInteract = new PlayerMovement();
			dialogueInteract.Dialogue.Continue.performed += ctx => OnContinue(ctx);
			dialogueInteract.Dialogue.End.performed += ctx => OnEnd(ctx);
			dialogueInteract.Dialogue.Select.performed += ctx => OnSelect(ctx);
			dialogueInteract.Dialogue.Confirm.performed += ctx => OnConfirm(ctx);
		}
		dialogueInteract.Dialogue.Enable();
	}

	void OnDisable()
	{
		dialogueInteract.Dialogue.Continue.performed -= ctx => OnContinue(ctx);
		dialogueInteract.Dialogue.End.performed -= ctx => OnEnd(ctx);
		dialogueInteract.Dialogue.Select.performed -= ctx => OnSelect(ctx);
		dialogueInteract.Dialogue.Confirm.performed += ctx => OnConfirm(ctx);
		dialogueInteract.Dialogue.Disable();
	}

	/// <summary>
	/// Find all DialogueParticipants
	/// </summary>
	/// <remarks>
	/// Filter them to those that have a Yarn start node and are in
	/// range; then start a conversation with the first one
	/// </remarks>
	public bool CheckForNearbyNPC()
	{
		var allParticipants = new List<NPC>(FindObjectsOfType<NPC>());

		var target = allParticipants.Find(delegate (NPC p)
		{
			return (p.talkToNode==null) == false && // has a conversation node?
			(p.transform.position - this.transform.position)// is in range?
			.magnitude <= interactionRadius;
		});
		if (target != null)
		{
			// Kick off the dialogue at this node

			target.LookAtSpeaker(transform.position);
			dialogueManager.StartDialogue(target.talkToNode);

			//disable camera look
			return true;
		}
		return false;
	}

	public void OnContinue(InputAction.CallbackContext context)
	{
		print("haha yes I tapped the E button");
		//dialogueSeq.NextNode();
		if (!dialogueManager.isTalking)
		{
			CheckForNearbyNPC();
		}
		else
		{
			dialogueManager.NextNode();
		}
	}

	public void OnEnd(InputAction.CallbackContext context)
	{
		dialogueManager.StopDialogue();
	}

	public void OnSelect(InputAction.CallbackContext context)
	{
		if(dialogueManager.inOptionList)
		{
			dialogueManager.SelectOption((int)context.ReadValue<float>());
		}
		//TODO: add select options function
	}

	public void OnConfirm(InputAction.CallbackContext context)
	{
		//TODO: add confirm option function
	}


	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("NPC"))
		{

		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("NPC"))
		{

		}
	}
}