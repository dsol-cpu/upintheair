using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class NPC_Controller : MonoBehaviour
{

    public Transform ChatBackGround;
    private Transform NPCCharacter;

    public DialogueManager dialogueManager;

    public string Name;

    [TextArea(5, 10)]
    public string[] sentences;

    void Awake()
    {     
        NPCCharacter = this.gameObject.transform;
    }

    void Update()
    {

    }
/*
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && dialogueManager.dialogueInput)
        {
            print("wow so fresh");
            dialogueManager.Names = Name;
            dialogueManager.dialogueLines = sentences;
            FindObjectOfType<DialogueManager>().NPCName();
        }
    }
*/


}

