using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Dialogue Node")]

public class DialogueNode : ScriptableObject
{
    [SerializeField] public string id;
    [SerializeField] public string[] nextNodes;
    [SerializeField] public string speakerName;
    [SerializeField] public string[] text;
    [SerializeField] public DialogueNode nextDialogue;
    [SerializeField] public ChoiceNode nextChoice;
    [SerializeField] public string checkpoint;
}