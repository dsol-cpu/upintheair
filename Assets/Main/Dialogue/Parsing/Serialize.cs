using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class Serialize
{
    void SerializeFromJSON(List<DialogueNode> jsonHolder)
    {
        string jsonFile = "";
        foreach (DialogueNode node in jsonHolder)
        {
            jsonFile += JsonConvert.SerializeObject(node);
        }
        File.WriteAllText(Application.dataPath + "/Scripts/PreviousSave", jsonFile);
    }

    void Update()
    {
        
    }
}