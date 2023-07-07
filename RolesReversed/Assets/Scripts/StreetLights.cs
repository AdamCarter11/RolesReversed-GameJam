using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class StreetLights : MonoBehaviour
{
    private float waitingTimer;
    private float requiredWaitingTime = 3f;
    private bool playerIsIn = false;
    private bool turnsGreen = false;
    void Start()
    {
        
    }

    void Update()
    {
        if (playerIsIn)
        {
            waitingTimer += Time.deltaTime;
        }
        if (waitingTimer > requiredWaitingTime)
        {
            turnsGreen = true;
            GetComponent<SpriteRenderer>().color = Color.green;
            //Change the sprite of street lights
        }
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        waitingTimer = 0;
        if (collision.tag == "Player") 
        {
            playerIsIn = true;

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerIsIn = false;
            if (!turnsGreen && collision.transform.position.x > transform.position.x)
            {
                print("Run red lights:-1 HP");
                //minus health
            }
        }
    }
}
