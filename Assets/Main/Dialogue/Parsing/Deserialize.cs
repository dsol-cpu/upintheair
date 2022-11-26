using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
public class Deserialize
{
    public List<DialogueNode> DeserializeFromJSON(string filename)
    {
        List<DialogueNode> jsonHolder;
        string jsonString = File.ReadAllText(filename);
        jsonHolder = JsonConvert.DeserializeObject<List<DialogueNode>>(jsonString);
        return jsonHolder;
    }

    void Update()
    {
        
    }
}