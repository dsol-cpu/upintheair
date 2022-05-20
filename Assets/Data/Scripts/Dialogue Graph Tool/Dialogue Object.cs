using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Dialogue Node", menuName = "ScriptableObject/DialogueNode", order = 1)]
public class DialogueObject : ScriptableObject
{
    [Header("Dialogue")]
    public List<DialogueSegment> dialogueSegments = new List<DialogueSegment>();
}

[System.Serializable]
public struct DialogueSegment
{
    public string dialogueText;
    public float dialogueDisplayTime;
    public List<DialogueChoice> dialogueChoices;
}

[System.Serializable]
public struct DialogueChoice
{
    public string dialogueChoice;
    public DialogueObject followOnDialogue;
}