using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Option Node")]

public class OptionNode : ScriptableObject
{
    [SerializeField] public string id;
    [SerializeField] public string[] nextNodes;
    [SerializeField] public string text;
    [SerializeField] public DialogueNode nextNode;
}