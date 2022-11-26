using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
///<summary>
///This class is responsible for displaying the dialogue to the player.
///</summary>
public class DialogueManager : MonoBehaviour
{
	//[SerializeField] DialogueSequencer dialogueSeq;
	[Header("Dialogue UI")]
	[SerializeField] GameObject dialogueGUIPrefab;
	[SerializeField] GameObject optionListGUIPrefab;
	GameObject dialogueGUI;
	GameObject textPanel;
	[SerializeField] GameObject optionListGUI;
	TMP_Text optionsBox;
	GameObject optionsBG;
	[SerializeField] GameObject continueButton;
	[SerializeField] TMP_Text continueText;
	[SerializeField] TMP_Text currentOption;
	TMP_Text textBox;
	TMP_Text nameBox;
	string kAlphaCode = "<color=#00000000>";
	//const float kMaxTestTime = 0.1f;

	//from tutorial
	[Header("Variables")]
	[SerializeField] DialogueNode currentNode;
	[SerializeField] ChoiceNode currentChoice;
	[SerializeField] float textSpeed = 10f;
	private int index = 0;
	private string checkpointID = ""; //stores last checkpoint in specific NPC's dialogue
	//storing checkpoints in a database/json might work
	private int optionIndex = 0;
    public bool isTalking = false;
	public bool inDialogue = false;
	public bool inOptionList = false;
	[SerializeField] Animator animator;
	Coroutine lastCoroutine = null;

	// Start is called before the first frame update
	void Start()
	{
		try{
			dialogueGUI = dialogueGUIPrefab.transform.Find("Dialogue UI").gameObject;
			textPanel = dialogueGUI.transform.Find("TextPanel").gameObject;
			nameBox = dialogueGUI.transform.Find("Name").gameObject.GetComponent<TMP_Text>();
			textBox = dialogueGUI.transform.Find("Textbox").gameObject.GetComponent<TMP_Text>();	
			textPanel = dialogueGUI.transform.Find("TextPanel").gameObject;
			optionListGUI = optionListGUIPrefab.transform.Find("Option List GUI").gameObject;
			optionsBox = optionListGUI.transform.Find("Option").GetComponent<TMP_Text>();
			optionsBG = optionListGUI.transform.Find("Option BG").gameObject;
			dialogueGUIPrefab.SetActive(false);
			optionListGUIPrefab.SetActive(false);
		}
		catch(System.Exception e){
			Debug.Log(e);
		}
		Load();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void StartDialogue(DialogueNode node) //should just say node
	{
		//read shit into the textbox GUI and play from a Node
		//index should be the last checkpoint in dialogue
		//hardcoded for now to be the first index

		//animate the textbox popping up
		isTalking = true;
		// dialogueSequencer = new DialogueSequencer();

		currentNode = node;
		PlayDialogue();
		//this will get replaced when I have a node class to abstract nodes
		
		//animator.SetBool("Open", true);
	}

	public void PlayDialogue()
	{
		//have the dialogue manager run a timer and check for events when the node is playing
		//if the node is a checkpoint, save the index
		//read access the dialogue node via node id
		dialogueGUIPrefab.SetActive(true);
		dialogueGUI.SetActive(true);
		nameBox.text = currentNode.speakerName;
		inDialogue = true;
		//scrolling text
		lastCoroutine = StartCoroutine(TypeLine());
	}

	IEnumerator TypeLine()
	{
		//TODO: need to add soundfont to play after each character is inserted 
		textBox.text = string.Empty;
		int alphaIndex = 0;
		foreach (char c in currentNode.text[index].ToCharArray())
		{
			alphaIndex++;
			textBox.text += c;
			textBox.text.Insert(alphaIndex, kAlphaCode);

			yield return new WaitForSecondsRealtime(textSpeed / 2);
		}
		yield return null;
	}

	public void NextNode()
	{
		if(inDialogue)
		{
			NextLine();
		}
		if(inOptionList)
		{
			ConfirmOption();
		}
	}

	public void NextLine()
	{
		//Work inside the dialogue nodes to determine whether to continue
		//Try dealing with node information
		//refresh the textboxes and play the next one
		
		if (textBox.text == currentNode.text[index])
		{
			if(index+1 == currentNode.text.Length)
			{
				print("aight lets go next node");
				//go to the next node
				//close and hide everything
				// currentNode = currentNode.nextDialogue ?? null;
				//TODO: check for null reference 
				currentChoice = currentNode.nextChoice;
				inDialogue = false;
				dialogueGUI.SetActive(false);
				index = 0;
				SelectNextNode();
			}
			else
			{
				//progress the line
				index++;
				textBox.text = string.Empty;
				lastCoroutine = StartCoroutine(TypeLine());
			}
		}
		else if(textBox.text != currentNode.text[index])
		{
			//fill in remaining text
			StopCoroutine(lastCoroutine);
			textBox.text = currentNode.text[index];
		}
	}

	void DisplayOptions()
	{
		print("displaying the options");
		///group all options into a node and instead of going to the next
		textBox.text = string.Empty;
		optionListGUIPrefab.SetActive(true);
		optionListGUI.SetActive(true);
		int optionIndex = 0;
		//make the GUI look nice while you load it in
		//TODO: make each option element feel better

		//possibly populate a list of options in the middle of the screen instead of in the text pane
		TMP_Text lastChild = null;

		foreach(OptionNode node in currentNode.nextChoice.options)
		{
			//make a list of options in the options box and make their positions scale by the number of options
			// if(optionIndex == 0)
			print("# of options: " + optionListGUI.transform.childCount);
			TMP_Text option = Instantiate(optionsBox, optionListGUI.transform);
			option.name = "Option " + optionIndex;
			option.text = currentNode.nextChoice.options[optionIndex].text;
			if(optionIndex > 0)
				option.rectTransform.localPosition -= new Vector3(0, lastChild.rectTransform.sizeDelta.y * optionIndex, 0);	
			//store recent option 
			lastChild = option;
			optionIndex++;
		}
		optionIndex = 0;
	}
	
	public void StopDialogue()
	{
		//animate the textbox popping up in reverse

		isTalking = false;
		//animator.SetBool("Open", false);
		//save where you are in dialogue and turn the gui off
		Save();
		if(inDialogue)
		{
			dialogueGUI.SetActive(false);
			inDialogue = false;
			StopCoroutine(lastCoroutine);
		}
		if(inOptionList)
		{
			optionListGUI.SetActive(false);
			foreach(TMP_Text child in optionListGUI.GetComponentsInChildren<TMP_Text>())
			{
				Destroy(child.gameObject);
			}
			inOptionList = false;
		}
	}

	void SelectNextNode(){
		///TODO: ADD code for selecting from the list of nodes in an option list in the GUI
		///TODO: Swap out code for "next node" for next ID instead and search from a dictionary 
		///of ID's for the next node 
		if(currentNode.nextDialogue)
		{
			inDialogue = true;
			PlayDialogue();
		}	
		if(currentNode.nextChoice)
		{
			inOptionList = true;
			DisplayOptions();
		}
		else
		{
			StopDialogue();
		}	
	}

	public void ConfirmOption()
	{
		if(currentOption != null)
		{
			print("confirming option");
			inOptionList = false;
			currentNode = currentChoice.options[optionIndex].nextNode;
			optionListGUI.SetActive(false);
			foreach(TMP_Text child in optionListGUI.GetComponentsInChildren<TMP_Text>())
			{
				Destroy(child.gameObject);
			}
			//play next node
			PlayDialogue();
		}
	}
	public void SelectOption(int keyInput)
	{
		//basically swap selection between the previous and the current option
		//reset current option to white

		if(currentOption != null)
		{
			currentOption.color = Color.white;
			optionIndex -= keyInput;
		}

		#region wrap index
			if(optionIndex > currentChoice.options.Count-1)
				optionIndex = 0;
			else if(optionIndex < 0)
				optionIndex = currentChoice.options.Count-1;
		#endregion

		//turn next option to selected color
		//optionIndex + 1 to factor for background game object ._. so dumb
		currentOption = optionListGUI.GetComponentsInChildren<TMP_Text>()[optionIndex+1];
		currentOption.color = Color.red;
		///TODO: ADD code for selecting from the list of nodes in an option list in the GUI
	}

	void Load()
	{
		//load dialogue in as list of lines
		DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Main/Dialogue/Lines/Test Folder/");
		FileInfo[] info = dir.GetFiles("*.*");
		foreach(FileInfo f in info)
		{
			
		}
		//load facts as key value pairs
		dir = new DirectoryInfo(Application.dataPath + "/Main/Dialogue/Lines/Test Folder/");
		info = dir.GetFiles("*.*");
		foreach(FileInfo f in info)
		{
			
		}
		//event scripts

	}
	void Save()
	{
		// checkpointID

		//TODO: ADD save at checkpoint
		//currentNode.checkpoint
	}
}