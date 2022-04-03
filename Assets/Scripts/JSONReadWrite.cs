using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonReadWrite
{
    public List<DialogueNode> Deserialize(string fileName)
    {
        List<DialogueNode> jsonHolder;
        string jsonString = File.ReadAllText(fileName);
        jsonHolder = JsonConvert.DeserializeObject<List<DialogueNode>>(jsonString);
        return jsonHolder;
    }

    public void Serialize(List<DialogueNode> jsonHolder)
    {
        string jsonFile = "";
        foreach (DialogueNode item in jsonHolder)
        {
            jsonFile += JsonConvert.SerializeObject(item);
        }
        File.WriteAllText(Application.dataPath + "/Scripts/Dialogue.json", jsonFile);
    }
}