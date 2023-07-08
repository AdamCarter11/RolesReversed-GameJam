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
    GameObject streetLightObj;

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
            Instantiate(frogPrefab, new Vector3(Random.Range(-8f, spawnRange), spawnHeight, frogPrefab.transform.position.z), Quaternion.identity);
        }
        else
        {
            // streetlights
            int streetLightOdds = Random.Range(0, 10);
            if (streetLightOdds < 2)
            {
                streetLightObj = Instantiate(streetLightPrefab, new Vector3(Random.Range(-2f, 2f), 0, streetLightPrefab.transform.position.z), Quaternion.identity);
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

        int spawnCap = 0;
        Vector3 spawnPosition = GetRandomSpawnPosition();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition, 1f); // Adjust the radius as needed
        while(colliders.Length > 0 && spawnCap < 3)
        {
            spawnPosition = GetRandomSpawnPosition();
            colliders = Physics2D.OverlapCircleAll(spawnPosition, 1f);
            spawnCap++;
        }

        if (colliders.Length == 0) // No overlapping objects found
        {
            Instantiate(frogPrefab, spawnPosition, Quaternion.identity);
            amountOfFrogs++;
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        return new Vector3(Random.Range(-8f, spawnRange), spawnHeight, frogPrefab.transform.position.z);
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
            
            if(streetLightObj != null)
            {
                Destroy(streetLightObj);
            }
            GenerateLevel();

            // car
            carObj.GetComponent<Cars>().MoveForceReset();
            carObj.transform.position = new Vector3(-17.5f, Random.Range(-6.5f, 6.5f), carObj.transform.position.z);
            carObj.transform.rotation = Quaternion.Euler(0f, 0f, 270f);
            
            //carObj.GetComponent<Cars>().ClearHelper();
        }
        if(carObj.transform.position.x >= 16)
        {
            // lose a life and generate level
            carObj.GetComponent<Cars>().ClearHelper();
            GenerateLevel();
            carObj.GetComponent<Cars>().MoveForceReset();
            carObj.transform.position = new Vector3(-17.5f, Random.Range(-6.5f, 6.5f), carObj.transform.position.z);
            carObj.transform.rotation = Quaternion.Euler(0f, 0f, 270f);
        }
    }

    

}
