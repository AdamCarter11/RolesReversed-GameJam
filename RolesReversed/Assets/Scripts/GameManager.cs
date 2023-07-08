using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton Structure
    public static GameManager instance;
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
    [SerializeField] float spawnRange = 5f; // Adjust the spawn range as needed
    [SerializeField] float spawnHeight = -5f; // Adjust the spawn height as needed

    private void Start()
    {
        score = 0;
        GenerateLevel();
    }
    private void GenerateLevel()
    {
        if (score == 0)
        {
            // first level should always be roughly the same difficulty
            amountOfFrogs++;
            Instantiate(frogPrefab, new Vector3(Random.Range(-5f, 5f), -5, frogPrefab.transform.position.z), Quaternion.identity);
        }
        else
        {
            // streetlights
            int streetLightOdds = Random.Range(0, 10);
            if (streetLightOdds < 2)
            {
                Instantiate(streetLightPrefab, new Vector3(-1.5f, 0, streetLightPrefab.transform.position.z), Quaternion.identity);
            }

            // frog(s)
            SpawnFrog();
            int frogOdds = Random.Range(0, 10);
            //print("frogOdds" + frogOdds);
            if (frogOdds < 6)
            {
                SpawnFrog();
                if (frogOdds < 3)
                {
                    SpawnFrog();
                }
            }

            // humans
        }
    }
    void SpawnFrog()
    {
        if (amountOfFrogs >= 3) // Limit the number of frogs
            return;

        Vector3 spawnPosition = GetRandomSpawnPosition();

        Collider[] colliders = Physics.OverlapSphere(spawnPosition, 1f); // Adjust the radius as needed

        if (colliders.Length == 0) // No overlapping objects found
        {
            Instantiate(frogPrefab, spawnPosition, Quaternion.identity);
            amountOfFrogs++;
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        return new Vector3(Random.Range(-spawnRange, spawnRange), spawnHeight, frogPrefab.transform.position.z);
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
        if(amountOfFrogs <= 0)
        {
            score++;
            carObj.GetComponent<Cars>().ClearHelper();
            
            GenerateLevel();

            // car
            carObj.GetComponent<Cars>().MoveForceReset();
            carObj.transform.position = new Vector3(-9, Random.Range(-3, 3), carObj.transform.position.z);
            carObj.transform.rotation = Quaternion.Euler(0f, 0f, 270f);
            
            //carObj.GetComponent<Cars>().ClearHelper();
        }
    }

    

}
