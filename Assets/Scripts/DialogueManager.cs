using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
	//This dialogue manager deals with the following tasks:
	//Dialogue: 
	//the GUI of the Dialogue
	//the text scrolling
	//dialogue tree
	//sound font

	public Text nameText;
	public Text dialogueText;

	public GameObject dialogueGUI;
	public Transform dialogueBoxGUI;

	public float letterDelay = 0.1f;
	public float letterMultiplier = 0.5f;

	[Header("Dialogue Input")]

	private PlayerInput playerInput;
	public bool dialogueInput;

	public string Names;

	public string[] dialogueLines;

	public bool letterIsMultiplied = false;
	public bool dialogueActive = false;
	public bool dialogueEnded = false;
	public bool outOfRange = true;

	public AudioClip audioClip;
	AudioSource audioSource;

	void Awake()
	{
	//	audioSource = GetComponent<AudioSource>();
	//	playerInput = new PlayerInput();
		dialogueGUI = GameObject.FindGameObjectWithTag("DialogueManager");
		dialogueGUI.SetActive(false);
		dialogueText = dialogueGUI.GetComponentInChildren<Text>();
		dialogueText.text = "";
	}

	void Update()
	{
		
	}



	public void NPCName()
	{

	}

	private IEnumerator StartDialogue()
	{
		if (outOfRange == false)
		{
			int dialogueLength = dialogueLines.Length;
			int currentDialogueIndex = 0;

			while (currentDialogueIndex < dialogueLength || !letterIsMultiplied)
			{
				if (!letterIsMultiplied)
				{
					letterIsMultiplied = true;
					StartCoroutine(DisplayString(dialogueLines[currentDialogueIndex++]));

					if (currentDialogueIndex >= dialogueLength)
					{
						dialogueEnded = true;
					}
				}
				yield return 0;
			}

			while (true)
			{
				if (dialogueInput && dialogueEnded == false)
				{
					break;
				}
				yield return 0;
			}
			dialogueEnded = false;
			dialogueActive = false;
			DropDialogue();
		}
	}

	private IEnumerator DisplayString(string stringToDisplay)
	{
		if (outOfRange == false)
		{
			int stringLength = stringToDisplay.Length;
			int currentCharacterIndex = 0;

			dialogueText.text = "";

			while (currentCharacterIndex < stringLength)
			{
				dialogueText.text += stringToDisplay[currentCharacterIndex];
				currentCharacterIndex++;

				if (currentCharacterIndex < stringLength)
				{
					if (dialogueInput)
					{
						yield return new WaitForSeconds(letterDelay * letterMultiplier);

						if (audioClip) audioSource.PlayOneShot(audioClip, 0.5F);
					}
					else
					{
						yield return new WaitForSeconds(letterDelay);

						if (audioClip) audioSource.PlayOneShot(audioClip, 0.5F);
					}
				}
				else
				{
					dialogueEnded = false;
					break;
				}
			}
			while (true)
			{
				if (dialogueInput)
				{
					break;
				}
				yield return 0;
			}
			dialogueEnded = false;
			letterIsMultiplied = false;
			dialogueText.text = "";
		}
	}
	public void EnterRangeOfNPC()
	{
		print("in range");
		outOfRange = false;
	}

	public void TriggerDialogue()
    {
		dialogueGUI.SetActive(true);
    }

	public void DropDialogue()
	{
		dialogueGUI.SetActive(false);
	}

	public void OutOfRange()
	{
		print("out of range");
		outOfRange = true;
		dialogueGUI.SetActive(false);
	}

}
