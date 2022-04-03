using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class NPC_Controller : MonoBehaviour
{

    public Transform ChatBackGround;
    private Transform NPCCharacter;

    private DialogueManager dialogueManager;

    public string Name;

    [TextArea(5, 10)]
    public string[] sentences;

    void Awake()
    {
        dialogueManager = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
        NPCCharacter = this.gameObject.transform;
    }

    void Update()
    {
        Vector3 Pos = Camera.main.WorldToScreenPoint(NPCCharacter.position);

        //Offset by height of npc_character
        Pos.y += this.gameObject.GetComponent<Renderer>().bounds.size.y/2;
        ChatBackGround.position = Pos;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && dialogueManager.dialogueInput)
        {
            this.gameObject.GetComponent<NPC_Controller>().enabled = true;
            print("wow so fresh");
            dialogueManager.Names = Name;
            dialogueManager.dialogueLines = sentences;
            FindObjectOfType<DialogueManager>().NPCName();
        }
    }

    public void OnTriggerExit()
    {
        //FindObjectOfType<DialogueManager>().OutOfRange();
        this.gameObject.GetComponent<NPC_Controller>().enabled = false;
    }

}

