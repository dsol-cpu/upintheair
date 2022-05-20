using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public class UIDialogueOption : MonoBehaviour
    {

        DialogueManager dialogueManager;
        DialogueObject dialogueObject;

        [SerializeField] Text dialogueText;

        public void Setup(DialogueManager _dialogueManager, DialogueObject _dialogueObject, string _dialogueText)
        {
            dialogueManager = _dialogueManager;
            dialogueObject = _dialogueObject;
            dialogueText.text = _dialogueText;
        }

        public void SelectOption()
        {
            dialogueManager.OptionSelected(dialogueObject);
        }
    }
