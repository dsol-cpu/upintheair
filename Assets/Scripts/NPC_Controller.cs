using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class NPC_Controller : MonoBehaviour
{

    public Transform ChatBackGround;
    public Transform NPCCharacter;

    private DialogueManager dialogueManager;

    public string Name;

    [TextArea(5, 10)]
    public string[] sentences;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    void Update()
    {
        Vector3 Pos = Camera.main.WorldToScreenPoint(NPCCharacter.position);
        Pos.y += 175;
        ChatBackGround.position = Pos;
    }

    public void OnTriggerStay(Collider other)
    {
        this.gameObject.GetComponent<NPC_Controller>().enabled = true;
        //FindObjectOfType<DialogueManager>().EnterRangeOfNPC();
        if ((other.gameObject.tag == "Player") && Input.GetKeyDown(KeyCode.F))
        {
            this.gameObject.GetComponent<NPC_Controller>().enabled = true;
            dialogueManager.Names = Name;
            dialogueManager.dialogueLines = sentences;
            FindObjectOfType<DialogueManager>().NPCName();
        }
    }

    public void OnTriggerExit()
    {
        FindObjectOfType<DialogueManager>().OutOfRange();
        this.gameObject.GetComponent<NPC_Controller>().enabled = false;
    }
}

