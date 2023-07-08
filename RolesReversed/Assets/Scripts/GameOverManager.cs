using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] Text highscoreText;
    [SerializeField] Text frogText;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("FrogScore", PlayerPrefs.GetInt("FrogScore") + 1);
        highscoreText.text =  "Highscore: " + PlayerPrefs.GetInt("HighScore");
        frogText.text = "FROGS: " + PlayerPrefs.GetInt("FrogScore");
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetKeyDown(KeyCode.R)) 
        {
            //Change Scene Here
            GameManager.instance.ResetVars();
            SceneManager.LoadScene("MainScene");
            //GameManager.instance.GenerateStuff();
        }
    }
}
