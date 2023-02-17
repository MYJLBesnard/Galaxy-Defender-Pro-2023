using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;


[Serializable]
public class LevelInfo // Contains the game settings for a given level of the game
{
    public int LevelNumber = 1;  // Number of Level
    public string Name = null;  // Name of the Level
    public int SizeOfWave = 10;  // How many enemy ships in the wave
    public float PowerUpSpeed = 0.5f; // Default speed of Power Ups
    public float PlayerLaserSpeed = 8.0f; // Default speed of Player's laser

    public float EnemySpeed = 5.0f;    // Default speed of Enemy ships(seconds)
    public float EnemyLaserSpeed = 7.0f;     // Default speed of Enemy laser(seconds)

    public float EnemyRateOfSpawning = 5.0f;   // How often Enemy spawns (seconds)
    public float EnemyRateOfFire = 2.5f; // How often Enemy ships fire (seconds)
    public float EnemySensorRange = 3.0f; // How far the RayCast can sense a hit
    public float EnemyMineLayerChance = 5.0f; // % (out of 100) that a Enemy Mine Layer will spawn
    public float BossEnemySpeed = 1.5f;    // Default speed of Boss Enemy ships(seconds)
    public float BossSensorRange = 2.0f; // How far the RayCast can sense a hit
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private int _playerLivesRemaining = 3;
    [SerializeField] private bool _isGameOver = false;
    [SerializeField] private Player _player;

    public int difficultyLevel = 1; // default difficulty level if not modified in Options Scene
    public int graphicQualityLevel = 1; // default graphic quality set to Low
    public float musicVolume = -10f;
    public float SFXVolume = -13f;
    public float dialogueVolume = 0f;
    public bool isPlayerDestroyed = false;
    public bool isBossDefeated = false;
    public bool enemyMineLayerDirectionRight = true;
    public bool continueToNextDifficultyLevel = false;
    public bool readyToLoadNextScene = false;
    public bool comingFromInstructionsScene = false;

    // A list of all the levels in the game (Inspector Assigned)
    [SerializeField] private List<LevelInfo> Waves = new List<LevelInfo>();

    // A list of properties allowing external objects access to all the properties of the current level.
    // These are modified by the difficulty level selected.
    public int howManyLevels { get { return Waves.Count; } }
    public int currentLevelNumber { get { return Waves[_currentWave].LevelNumber; } }
    public string currentLevelName { get { return Waves[_currentWave].Name; } }
    public int currentSizeOfWave { get { return Waves[_currentWave].SizeOfWave + difficultyLevel; } }
    public float currentPowerUpSpeed { get { return Waves[_currentWave].PowerUpSpeed + (difficultyLevel / 2); } }
    public float currentPlayerLaserSpeed { get { return Waves[_currentWave].PlayerLaserSpeed + (difficultyLevel/2); } }

    public float currentEnemySpeed { get { return Waves[_currentWave].EnemySpeed + difficultyLevel; } }
    public float currentEnemyLaserSpeed { get { return Waves[_currentWave].EnemyLaserSpeed + (currentEnemySpeed * 1.25f) + (difficultyLevel / 2); } }

    public float currentEnemyRateOfSpawning { get { return Waves[_currentWave].EnemyRateOfSpawning - difficultyLevel; } }
    public float currentEnemyRateOfFire { get { return Waves[_currentWave].EnemyRateOfFire - difficultyLevel; } }
    public float currentEnemySensorRange { get { return Waves[_currentWave].EnemySensorRange + difficultyLevel; } }
    public float currentEnemyMineLayerChance { get { return Waves[_currentWave].EnemyMineLayerChance - (currentLevelNumber * difficultyLevel); } }
    public float currentBossEnemySpeed { get { return Waves[_currentWave].BossEnemySpeed + (difficultyLevel / 2); } }
    public float currentBossSensorRange { get { return Waves[_currentWave].EnemySensorRange + (difficultyLevel / 2); } }

    // Number of lives remaining
    private int _lives = 3;
    public int lives { get { return _lives; } }

    // Player's current score
    private int _currentScore = 0;
    public int currentScore { get { return _currentScore; } }

    // Player's current level
    private int _currentWave = 0;
    public int currentWave { get { return _currentWave; } }

    // Singleton Accessor - Only one GameManager should ever exist
    private static GameManager _instance = null;
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (GameManager)FindObjectOfType(typeof(GameManager));
            }
            return _instance;
        }
    }

    private void Awake() // Initialize the Game Manager state 
    {
        _lives = 3;
        _currentScore = 0;
        _currentWave = 0;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();


        if (_spawnManager == null)
        {
            Debug.LogError("The SpawnManager is null.");
        }
        
        if (_uiManager == null)
        {
            Debug.LogError("The UIManager is null.");
        }
    }
    

    public void IncreaseScore(int points)
    {
        _currentScore += points;
    }

    public void StartNewGame()
    {
        // Reset score and lives.
        _currentScore = 0;
        _currentWave = 0;
        _lives = 3;

        SceneManager.LoadScene(1);
    }

    public void WaveComplete()
    {
        if (_currentWave < Waves.Count - 1)
            _currentWave++;
    }

    public int DecreaseLives()
    {
        if (_lives > 0) _lives--;
        return _lives;
    }


    private void Update()
    {
        if (_isGameOver == true && Input.GetKeyDown(KeyCode.R)) // Restart Game
        {
            //SceneManager.LoadScene(1);
            StartNewGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit(); // Quits the Application, no credits
        }
    }

    public void UpdateLivesRemaining(int playerLives)
    {
        _playerLivesRemaining = playerLives;
        if (_playerLivesRemaining == 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        isPlayerDestroyed = true;
        _isGameOver = true;
        _uiManager.GameOverMsg();
    }
}
