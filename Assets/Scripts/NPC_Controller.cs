using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    //TODO:
    //NPC Movement AI 
    //Make a certain distance they can walk around
    //Randomize the movement and possibly have them interact with surroundings like sit or stand
    //Trigger dialogue when player presses input when facing them

    //public AudioClip AlienScream;
    public float minSpeed = 10;  // minimum range of speed to move
    public float maxSpeed = 50;  // maximum range of speed to move

    //public string[] collisionTags = {"Wall", "Player"};             //  What are the GO tags that will act as colliders that trigger a
                                               //  direction change? Tags like for walls, room objects, etc.
    public AudioClip collisionSound;
    float timeCounter = 0.0f;
    float timeCounterSpeed = 10.0f;
    float timeDuration = 10.0f;
    bool isMoving = false;

    float speed = 0.5f;

    Vector3 desiredPos;
    float xPos;
    float zPos;
    Rigidbody rb;

    private void Start()
    {        
        rb = GetComponent<Rigidbody>();
        //yPos = Random.Range(-0f, 10f);
        xPos = Random.Range(-4.5f, 4.5f);
        zPos = Random.Range(-4.5f, 4.5f);
        desiredPos = new Vector3(xPos, transform.position.y, zPos);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Wall")
        {                   //  Tag it with a wall or other object
            GetComponent<AudioSource>().PlayOneShot(collisionSound, 2.0f);         //  Plays a sound on collision
                                                                                   // Switch to a new direction on collision
            xPos = Random.Range(-0f, 10f);
            zPos = Random.Range(-0f, 10f);

            desiredPos = new Vector3(xPos, transform.position.y, zPos);
            timeCounter = 0.0f;


            // use the above code as a template for all the collisionTags
            // add here.. and on.. and on..
        }
    }
   
    void Update()
    {
        timeCounter += Time.deltaTime * timeCounterSpeed;
        if (timeCounter >= timeDuration)
     {
            transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * speed);
            if (Vector3.Distance(transform.position, desiredPos) <= 0.01f)
            {
                xPos = Random.Range(-0f, 10f);
                zPos = Random.Range(-0f, 10f);

                desiredPos = new Vector3(xPos, transform.position.y, zPos);
                timeCounter = 0.0f;
            }
        }
    }
}