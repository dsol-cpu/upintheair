/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Dialogue : MonoBehaviour, PlayerMovement.IDialogueActions
{
    PlayerMovement dialogueInteract;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //Interact
    void OnEnable()
    {
        if (dialogueInteract == null)
        {
            dialogueInteract = new PlayerMovement();
            dialogueInteract.Dialogue.Continue.performed += ctx => OnContinue(ctx);
            dialogueInteract.Dialogue.End.performed += ctx => OnEnd(ctx);
        }
        dialogueInteract.Dialogue.Enable();
    }

    void OnDisable()
    {
        dialogueInteract.Dialogue.Continue.performed -= ctx => OnContinue(ctx);
        dialogueInteract.Dialogue.End.performed -= ctx => OnEnd(ctx);
        dialogueInteract.Dialogue.Disable();
    }


    public void OnEnd(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
}
*/