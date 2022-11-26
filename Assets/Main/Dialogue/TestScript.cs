/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
public class TestScript : MonoBehaviour
{
    [SerializeField] private DialogueRunner dialogueRunner;

    void Awake()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.AddCommandHandler("expression",
        (string characterName, string emotion, int intensity) => {
            switch (emotion.ToUpper())
            {
                case "SURPRISED":
                    foreach (NPC npc in FindObjectsOfType<NPC>())
                    {
                        if (npc.characterName == characterName)
                        {
                            npc.GetComponent<Rigidbody>().AddForce(new Vector3(0, 9.8f * intensity, 0));
                            //surprised animation
                        }
                    }
                    print("Surprised");
                    break;
                case "SAD":
                    Debug.Log("Sad");
                    break;
                case "EXCITED":
                    Debug.Log("Excited");
                    break;
                case "DEPRESSED":
                    Debug.Log("Depressed");
                    break;
                default:
                    Debug.Log("Expression not found");
                    break;
            }
        });

        dialogueRunner.AddCommandHandler(
            "jump",
            (string nodeTitle) => {
                return StartCoroutine(JumpToNodeCoroutine(nodeTitle));
            }
        );

    }
    private IEnumerator JumpToNodeCoroutine(string nodeTitle)
    {
        yield return new WaitForSeconds(0.01f);
        dialogueRunner.StartDialogue(nodeTitle);
    }
}*/