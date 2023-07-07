using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton Structure
    private static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [HideInInspector] public int score;
    [HideInInspector] public int health;
    [HideInInspector] public int amountOfFrogs;

    [SerializeField] GameObject frogPrefab;
    [SerializeField] GameObject humanPrefab;
    [SerializeField] GameObject streetLightPrefab;
    [SerializeField] GameObject carObj;

    private void Start()
    {
        GenerateLevel();
    }
    private void GenerateLevel()
    {
        if (score == 0)
        {
            // first level should always be roughly the same difficulty
            Instantiate(frogPrefab, new Vector3(Random.Range(-5f, 5f), -5, frogPrefab.transform.position.z), Quaternion.identity);

            // car
            carObj.transform.position = new Vector3(-5, Random.Range(-3, 3), carObj.transform.position.z);
        }
        else
        {
            // frog(s)
            Instantiate(frogPrefab, new Vector3(Random.Range(-5f, 5f), -5, frogPrefab.transform.position.z), Quaternion.identity);
            amountOfFrogs++;
            int frogOdds = Random.Range(0, 10);
            print("frogOdds" + frogOdds);
            if (frogOdds < 6)
            {
                Instantiate(frogPrefab, new Vector3(Random.Range(-5f, 5f), -5, frogPrefab.transform.position.z), Quaternion.identity);
                amountOfFrogs++;
                if (frogOdds < 3)
                {
                    Instantiate(frogPrefab, new Vector3(Random.Range(-5f, 5f), -5, frogPrefab.transform.position.z), Quaternion.identity);
                    amountOfFrogs++;
                }
            }

            // streetlights
            if (frogOdds > 3)
            {
                Instantiate(streetLightPrefab, new Vector3(Random.Range(-1f, 2f), 0, streetLightPrefab.transform.position.z), Quaternion.identity);
            }

            // humans

            // car
            carObj.transform.position = new Vector3(-5, Random.Range(-3, 3), carObj.transform.position.z);
        }
    }

    private void Update()
    {
        if(health <= 0)
        {
            // gameover

            if(score > PlayerPrefs.GetInt("HighScore"))
            {
                PlayerPrefs.SetInt("HighScore", score);
            }
        }
    }

    

}
