using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    [SerializeField] Image health1;
    [SerializeField] Image health2;
    [SerializeField] Image health3;
    [SerializeField] Text scoreText;
    // Start is called before the first frame update
    void Start()
    {
        health1.enabled = true;
        health2.enabled = true;
        health3.enabled= true;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.health == 2)
        {
            health3.enabled = false;
        }
        if(GameManager.instance.health == 1)
        {
            health2.enabled = false;
        }
        scoreText.text = "Score: " + GameManager.instance.score;
    }
}
