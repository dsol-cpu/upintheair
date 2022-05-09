using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class NPC_Controller : MonoBehaviour
{
    PlayerController player;

    private Transform NPCCharacter;

    public string Name;

    [TextArea(5, 10)]
    public string[] sentences;

    void Awake()
    {     
        NPCCharacter = this.gameObject.transform;
    }

    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            print("wow so fresh");
        }

    }

    public void OnTriggerExit(Collider other)
    {
        print("Out of range");
    }


}

