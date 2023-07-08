using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] Text highscoreText;
    // Start is called before the first frame update
    void Start()
    {
        highscoreText.text =  "Highscore: " + PlayerPrefs.GetInt("HighScore");
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetKeyDown(KeyCode.R)) 
        {
            //Change Scene Here
            GameManager.instance.ResetVars();
            SceneManager.LoadScene("MainScene");
            GameManager.instance.GenerateStuff();
        }
    }
}
