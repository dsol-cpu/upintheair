using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/Choice Node")]

public class ChoiceNode : ScriptableObject
{
    [SerializeField] public string id;
    [SerializeField] public string[] nextNodes;
    [SerializeField] public List<OptionNode> options;
}