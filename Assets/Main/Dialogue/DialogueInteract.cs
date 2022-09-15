/*

The MIT License (MIT)

Copyright (c) 2015-2017 Secret Lab Pty. Ltd. and Yarn Spinner contributors.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Yarn.Unity;
#if USE_INPUTSYSTEM && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
public class DialogueInteract : MonoBehaviour, PlayerMovement.IDialogueActions
{
    public float interactionRadius = 2.0f;

    // because we are using the same button press for both starting and skipping dialogue they collide
    // so we are going to make it so that the input gets turned off
    [SerializeField] private DialogueAdvanceInput dialogueInput;
    private PlayerMovement dialogueInteract;
    private void OnEnable()
    {
        if (dialogueInteract == null)
        {
            dialogueInteract = new PlayerMovement();
            dialogueInteract.Dialogue.Continue.performed += ctx => OnContinue(ctx);
            dialogueInteract.Dialogue.End.performed += ctx => OnEnd(ctx);
        }
        dialogueInteract.Dialogue.Enable();
    }
    private void OnDisable()
    {
        dialogueInteract.Dialogue.Continue.performed -= ctx => OnContinue(ctx);
        dialogueInteract.Dialogue.End.performed -= ctx => OnEnd(ctx);
        dialogueInteract.Dialogue.Disable();
    }
    void Start()
    {
        dialogueInput = FindObjectOfType<DialogueAdvanceInput>();
        dialogueInput.enabled = false;
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

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Remove all mouse control when we're in dialogue
        if (FindObjectOfType<DialogueRunner>().IsDialogueRunning == true)
        {
            return;
        }

        // every time we LEAVE dialogue we have to make sure we disable the input again
        if (dialogueInput.enabled)
        {
            dialogueInput.enabled = false;
        }
    }
    /// <summary>
    /// Find all DialogueParticipants
    /// </summary>
    /// <remarks>
    /// Filter them to those that have a Yarn start node and are in
    /// range; then start a conversation with the first one
    /// </remarks>
    public void CheckForNearbyNPC()
    {
        var allParticipants = new List<NPC>(FindObjectsOfType<NPC>());

        var target = allParticipants.Find(delegate (NPC p)
        {
            return string.IsNullOrEmpty(p.talkToNode) == false && // has a conversation node?
            (p.transform.position - this.transform.position)// is in range?
            .magnitude <= interactionRadius;
        });
        if (target != null)
        {
            // Kick off the dialogue at this node.
            FindObjectOfType<DialogueRunner>().StartDialogue(target.talkToNode);
            target.Talking(transform.position);
            // reenabling the input on the dialogue
            dialogueInput.enabled = true;
            //disable camera look
        }
    }

    public void OnContinue(InputAction.CallbackContext context)
    {
        CheckForNearbyNPC();
    }

    public void OnEnd(InputAction.CallbackContext context)
    {
        FindObjectOfType<DialogueRunner>().Stop();
        //Exit dialogue
    }

    void OnTriggerEnter(Collider other)
    {
            
    }

    void OnTriggerExit(Collider other)
    {
            
    }
}
