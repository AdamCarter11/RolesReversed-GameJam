using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] Text dialogueText;
    [SerializeField] Text highscoreText;
    private float typingSpeed = 0.04f;
    [SerializeField] AudioSource notificationSound;
    private int lineNum = 0;
    [SerializeField] Sprite[] frogFrames;
    [SerializeField] Image frogImage;
    bool frameChange = true;

    // Start is called before the first frame update
    void Start()
    {
        dialogueText.text = "Welcome To The Frontlines Soldier. Are You Ready To Serve Your Country?";
        StartCoroutine(DisplayLine(dialogueText.text));
        StartCoroutine(FrogFrameChange());

    }

    // Update is called once per frame
    void Update()
    {
        highscoreText.text = "Highscore: " + PlayerPrefs.GetInt("HighScore");

        if (Input.anyKeyDown)
        {
            lineNum++;
            if(lineNum == 1) 
            {
                dialogueText.text = "Since We Lost The First Great Frog War In August of 1981 The World Has Gone To Shit.";
                StopAllCoroutines();
                StartCoroutine(DisplayLine(dialogueText.text));
                frameChange = true;
                StartCoroutine(FrogFrameChange());
            }
            if(lineNum == 2)
            {
                dialogueText.text = "Thats Why We Called You. The Last Line Of Defense Against The Frogs, The Drifters. An Elite Task Force Trained In Car To Frog Combat.";
                StopAllCoroutines();
                StartCoroutine(DisplayLine(dialogueText.text));
                frameChange = true;
                StartCoroutine(FrogFrameChange());
                
            }
            if (lineNum == 3)
            {
                dialogueText.text = "Now Go Out There And Make Me Proud, Son!";
                StopAllCoroutines();
                StartCoroutine(DisplayLine(dialogueText.text));
                frameChange = true;
                StartCoroutine(FrogFrameChange());
            }
            if (lineNum == 4)
            {
                //Change Scene Here
                SceneManager.LoadScene("MainScene");
            }

        }
    }
    public IEnumerator DisplayLine(string line)
    {
        dialogueText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            if (!notificationSound.isPlaying)
            {
                notificationSound.Play();
            }
            yield return new WaitForSeconds(typingSpeed);
            
        }
        StopCoroutine(FrogFrameChange());
        frameChange = false;
        frogImage.sprite = frogFrames[0];
        if (lineNum == 1)
        {
            dialogueText.text = "Since We Lost The First Great Frog War In August of 1981 The World Has Gone To <color=red>Shit</color>.";
            
        }
    }
    IEnumerator FrogFrameChange()
    {
        int i = 1;
        while (frameChange)
        {
            frogImage.sprite = frogFrames[i % 2];
            yield return new WaitForSeconds(.3f);
            i++;
        }
    }
}
