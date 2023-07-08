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
    [HideInInspector] public float offset = 0;
    [HideInInspector] public int frogsDestroyed;

    [SerializeField] GameObject frogPrefab;
    [SerializeField] GameObject humanPrefab;
    [SerializeField] GameObject streetLightPrefab;
    [SerializeField] GameObject carObj;
    [SerializeField] float spawnRange = 5f; // Adjust the spawn range as needed
    [SerializeField] float spawnHeight = -5f; // Adjust the spawn height as needed
    GameObject streetLightObj;
    private int scoreIncrease = 0;
    

    private void Start()
    {
        score = 0;
        health = 3;
        SceneManager.sceneLoaded += OnSceneLoaded;
        GenerateLevel();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        print("load scene");
        if(SceneManager.GetActiveScene().name == "MainScene")
        {
            if (carObj == null)
            {
                carObj = GameObject.FindGameObjectWithTag("Player");
            }
            GenerateStuff();
        }
    }
    private void GenerateLevel()
    {
        if (score == 0 && health == 3)
        {
            print("GENERATE INITIAL FROG");
            // first level should always be roughly the same difficulty
            amountOfFrogs++;
            scoreIncrease++;
            Instantiate(frogPrefab, new Vector3(Random.Range(-8f, spawnRange) + offset, spawnHeight, frogPrefab.transform.position.z), Quaternion.identity);
        }
        else
        {
            print("NEW FROGS");
            // streetlights
            /*
            int streetLightOdds = Random.Range(0, 10);
            if (streetLightOdds < 2)
            {
                streetLightObj = Instantiate(streetLightPrefab, new Vector3(Random.Range(-2f, 2f) + offset, 0, streetLightPrefab.transform.position.z), Quaternion.identity);
            }
            */

            // frog(s)
            SpawnObjects(frogPrefab);
            int frogOdds = Random.Range(0, 10);
            //print("frogOdds" + frogOdds);
            if (frogOdds < 6)
            {
                SpawnObjects(frogPrefab);
                if (frogOdds < 3)
                {
                    SpawnObjects(frogPrefab);
                }
            }

            // humans
            int humanOdds = Random.Range(0, 10);
            if (humanOdds < 10)
            {
                SpawnObjects(humanPrefab);
            }
            

        }
    }
    void SpawnObjects(GameObject ObjectToSpawn)
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
            Instantiate(ObjectToSpawn, spawnPosition, Quaternion.identity);
            if(ObjectToSpawn.gameObject.name == "Frog")
            {
                amountOfFrogs++;
                scoreIncrease++;
            }
                
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        float randoX = Random.Range(-8f, spawnRange) + offset;
        float randoY = Random.Range(-8f, -2.5f);
        //print("frog x: " + randoX);
        return new Vector3(randoX, randoY, frogPrefab.transform.position.z);
    }

    private void Update()
    {
        //print(amountOfFrogs);
        if(health <= 0  && SceneManager.GetActiveScene().name == "MainScene")
        {
            // gameover

            if(score > PlayerPrefs.GetInt("HighScore"))
            {
                PlayerPrefs.SetInt("HighScore", score);
            }
            SceneManager.LoadScene("GameOver");
        }
        
        /*
        if(carObj.transform.position.x >= 16 + offset)
        {
            // lose a life and generate level
            NextLevel();
        }
        */
    }
    private void ClearAllFrogs()
    {
        GameObject[] gameObjectsWithTag = GameObject.FindGameObjectsWithTag("Frog");

        foreach (GameObject obj in gameObjectsWithTag)
        {
            Destroy(obj);
        }
        amountOfFrogs = 0;

        GameObject[] humansWithTag = GameObject.FindGameObjectsWithTag("Human");

        foreach (GameObject obj in humansWithTag)
        {
            Destroy(obj);
        }
    }
    public void NextLevel()
    {
        if (amountOfFrogs > 0)
        {
            health--;
            scoreIncrease = 0;
        } 
        if (streetLightObj != null)
        {
            Destroy(streetLightObj);
        }
        if (amountOfFrogs <= 0)
        {
            carObj = GameObject.FindGameObjectWithTag("Player");
            score += scoreIncrease;
            scoreIncrease = 0;
            carObj.GetComponent<Cars>().IncreaseSpeed();

        }
        ClearAllFrogs();
        //GenerateStuff();
        //carObj.transform.rotation = Quaternion.Euler(0f, 0f, 270f);
    }
    public void GenerateStuff()
    {
        print("STUFF");
        carObj = GameObject.FindGameObjectWithTag("Player");
        print(carObj.name);
        carObj.GetComponent<Cars>().ClearHelper();

        GenerateLevel();
        //carObj.GetComponent<Cars>().MoveForceReset();
        //carObj.transform.position = new Vector3(-17.5f + offset, transform.position.y, carObj.transform.position.z);
    }
    public void ResetVars()
    {
        score = 0;
        health = 3;
        frogsDestroyed = 0;
        offset = 0;
        amountOfFrogs = 0;
        scoreIncrease = 0;
    }
}
