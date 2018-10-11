using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using MLAgents;
//using System;

public class Done_GameController : MonoBehaviour
{
    public GameObject playerShip;
    public GameObject[] hazards;
    public GameObject earth;
    public float spawnEarth = 8;
    public float earthScale = 0.4f;
    public Vector3 spawnValues;
    public int hazardCount;
    public float spawnWait;
    public float startWait;
    public float waveWait;

    public Text scoreText;
    public Text restartText;
    public Text gameOverText;
    public Text brainSwitch;

    private float startTime;
    private bool gameOver;
    private bool restart;
    private bool earthPassed;
    private int score;
    private int numGames;
    private int overallScore;
    private List<GameObject> currentHazards = new List<GameObject>();
    private PlayerAgent playerAgent;
    private GameObject spawnedEarth;

    public List<GameObject> CurrentHazards
    {
        get
        {
            return currentHazards;
        }
    }

    void Start()
    {
        playerAgent = FindObjectOfType<PlayerAgent>();
        gameOver = false;
        restart = false;
        earthPassed = false;
        restartText.text = "";
        gameOverText.text = "";
        score = 0;
        UpdateScore();
        playerAgent.Initialize(Instantiate(playerShip), this);
        StartCoroutine(SpawnWaves());
        startTime = Time.time;
    }

    void Update()
    {
        // There is more to it, then this.
        if (Input.GetKeyDown(KeyCode.P) && playerAgent.brain.brainType != BrainType.Player)
        {
            //Switch to player brain
            playerAgent.ManualBrain();
            brainSwitch.text = "Press 'M' for auto pilot.";
        }

        if (Input.GetKeyDown(KeyCode.M) && playerAgent.brain.brainType == BrainType.Player)
        {
            //Switch to machine learning brain
            playerAgent.MLBrain();
            brainSwitch.text = "Press 'P' to play!";
        }

        if (restart)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                restart = false;
                Restart();
            }
        }

        if (spawnedEarth == null && Time.time >= startTime + spawnEarth && !gameOver && !earthPassed) SpawnEarth();
        
    }

    private void Restart()
    {
        // Changed: Restore Scenario without reloading the scene
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // Destroy all remaining hazards
        currentHazards.ForEach(h => Destroy(h));
        //Destroy earth (I always wanted to do this.)
        Destroy(spawnedEarth);
        earthPassed = false;
        // Reset score
        score = 0;
        UpdateScore();
        startTime = Time.time;
        // Restart spawning for human players
        if (gameOver) StartCoroutine(SpawnWaves());
        gameOver = false;
        // Respawn player
        playerAgent.Initialize(Instantiate(playerShip), this);
        restartText.text = "";
        gameOverText.text = "";

    }

    private void SpawnEarth()
    {
        spawnedEarth = Instantiate(earth, new Vector3(-3, -14, 30), Quaternion.Euler(0,0,30)); 
        spawnedEarth.transform.localScale = new Vector3(earthScale, earthScale, earthScale);
        earthPassed = true;
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startWait);
        while (true)
        {
            for (int i = 0; i < hazardCount; i++)
            {
                GameObject hazard = hazards[Random.Range(0, hazards.Length)];
                Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
                Quaternion spawnRotation = Quaternion.identity;
                currentHazards.Add(Instantiate(hazard, spawnPosition, spawnRotation));
                yield return new WaitForSeconds(startWait);
            }
            yield return new WaitForSeconds(startWait);

            if (gameOver)
            {
                restartText.text = "Press 'R' for Restart";
                restart = true;
                break;
            }
        }
    }

    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        // Added: Set reward for player agent
        playerAgent.AddReward(newScoreValue);
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        // Added: Game Over text only for human players
        if (playerAgent.brain.brainType == BrainType.Player)
        {
            gameOver = true;
            gameOverText.text = "Game Over!";
        }
        else
        {
            // Added: notify player agent with big penalty and done()
            // Size of reward chosen in relation to score to what would be a "good game".
            playerAgent.SetReward(-100);
            playerAgent.Done();
            //Debug.Log("Average AI score after " + ++numGames + " games:" + ((overallScore += score) / numGames));
            // Immediate restart - no actual destroy.
            Restart();
        }
        
    }

    public void TidyUpHazardList()
    {
        // ABAP programmers behold! A lambda expression: 
        currentHazards.RemoveAll(o => o == null);
    }

    public void RegisterHazard(GameObject hazard)
    {
        currentHazards.Add(hazard);
    }
}