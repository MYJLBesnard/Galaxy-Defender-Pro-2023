using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private Player _player;

    public Transform player;
    [SerializeField] private GameObject depletedLateralLaserCanons; //*********

    //public float radius = 13f;
    //public float spawnRate = 5.0f;

    private float _xPos;
    private float _yPos;
    private float _zPos;

    [Header("Object Containers")]
    public GameObject playerLaserContainer;
    public GameObject powerUpContainer;
    public GameObject enemyContainer;
    public GameObject enemyLaserContainer;
    public GameObject explosionContainer;

    [Header("Power Up Related")]
    [SerializeField] private GameObject[] _playerBasicPowerUps; //*********
    [SerializeField] private GameObject[] _playerWeaponsPowerUps; //*********
    [SerializeField] private GameObject[] _healthAndNegPowerUps; //*********

    [Header("Enemy Related")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _typeOfEnemy; //*********
    [SerializeField] private GameObject[] _typesOfEnemy; //*********
    [SerializeField] private int _enemiesInWave = 15;
    [SerializeField] private int _enemiesSpawnedInWave = 0;
    public int totalEnemyShipsDestroyed = 0;

    public bool isBossActive = false;
    public bool isPlayerDestroyed = false;
    
    [SerializeField] private bool _stopSpawning = true;
    [SerializeField] private bool _stopSpawningEnemy = false;
    [SerializeField] private bool _stopSpawningPlayerPwrUp = false;

    [SerializeField] private AudioClip _warningIncomingWave;
    [SerializeField] private AudioClip _attackWaveRepelled;

    //[SerializeField] private float _powerUpSpeed = 13f;
    //public float powerUpSpeed { get { return _powerUpSpeed; } }

    public int waveCurrent = 0;
    private int _shipsInWave = 0;
    public int enemyType = 0;
    private float _triggerMineLayer = 0f;

    [SerializeField] private bool _asteroidHit = false;
    public bool asteroidHit { get { return _asteroidHit; } }

    [SerializeField] private float _radius = 13f;
    public float radius { get { return _radius; } }


    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_gameManager == null)
        {
            Debug.LogError("The SpawnManager : Game Manager is null.");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager on the Canvas is null.");
        }

        if (_player == null)
        {
            Debug.LogError("The SpawnManager : Player is null.");
        }

        waveCurrent = _gameManager.currentWave;
        Debug.Log("Current Wave capture from Start(): " + waveCurrent);
    }

    public void StartGameAsteroidDestroyed()
    {
            _asteroidHit = true;
    }

public void EnemyShipsDestroyedCounter()
    {
        totalEnemyShipsDestroyed++;

        if (totalEnemyShipsDestroyed == _gameManager.currentSizeOfWave)
        {
            if (waveCurrent == _gameManager.howManyLevels - 1)

            {
                // No more waves
                _stopSpawning = true;
                _stopSpawningEnemy = true;
            }
            else
            {
                _gameManager.WaveComplete();
                totalEnemyShipsDestroyed = 0;
                StartCoroutine(StartNewWave());
            }
        }
    }

    IEnumerator StartNewWave()
    {
        _stopSpawning = true;
        _shipsInWave = 0;
        totalEnemyShipsDestroyed = 0;
        yield return new WaitForSeconds(3.5f);

        _stopSpawning = false;
        _stopSpawningEnemy = false;
        OnPlayerReady();
        Debug.Log("IEnumerator SpawnManager : StartNewWave running.");
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyWave());
        StartCoroutine(SpawnBasicPowerUps());
        StartCoroutine(SpawnHealthAndNegPowerUps());
    }

   
        IEnumerator SpawnEnemyWave()
        {
        Debug.Log("Start SpawnEnemyWave");
        yield return new WaitForSeconds(2.0f); // delays start of enemy spawns

            while (_stopSpawningEnemy == false && _stopSpawning == false && _player.isPlayerAlive == true)
            {

                if (_stopSpawningEnemy == false)
                {
                    _typeOfEnemy = null;
                    waveCurrent = _gameManager.currentWave;

                Debug.Log("From SpawnEnemyWave() coroutine... Wave: " + waveCurrent);

                    switch (_gameManager.currentWave)
                    {
                        case 0: // spawn basic Enenmy
                            int type0 = 0;
                            _typeOfEnemy = _typesOfEnemy[type0];
                            enemyType = type0;
                            break;

                        case 1: // spawn basic Enemy
                            int type1 = 1;
                            _typeOfEnemy = _typesOfEnemy[type1];
                            enemyType = type1;
                            break;

                        default:
                            break;
                    }

                        if (isBossActive != true)
                        {
                            if (_player.normalEnemyMovement == true)
                            {
                                CreateEnemyRandomPositionAtTop();
                            }
                
                            else if (_player.normalEnemyMovement == false)
                            {
                                CreateEnemyRandomPositionOnCircumference();
                            }
                        }
                }

            yield return new WaitForSeconds(_gameManager.currentEnemyRateOfSpawning);

        }
    }
    

    public void CreateEnemyRandomPositionAtTop()
    {
        _xPos = Random.Range(-8.0f, 8.0f);
        Vector3 pxToSpawn = new Vector3(_xPos, 9, 0);

        GameObject newEnemy = Instantiate(_enemyPrefab);

        newEnemy.transform.parent = enemyContainer.transform;
        newEnemy.transform.position = pxToSpawn;

        _enemiesSpawnedInWave++;
        if (_enemiesSpawnedInWave == _gameManager.currentSizeOfWave)
        {
            _stopSpawningEnemy = true;
        }
    }

    // this method could be used to spawn asteroids 360 degrees around player and translate them towards Players position 
    public void CreateEnemyRandomPositionOnCircumference()
    {
        GameObject newEnemy = Instantiate(_enemyPrefab); // as GameObject;

        newEnemy.transform.parent = enemyContainer.transform;
        newEnemy.transform.position = Random.insideUnitCircle.normalized * radius;

        Vector3 playerPos = player.transform.position;
        Vector3 enemyPos = newEnemy.transform.position;
        Vector2 direction = enemyPos - playerPos;

        float angle = Vector2.SignedAngle(Vector2.up, direction);

        newEnemy.transform.eulerAngles = new Vector3(0, 0, angle);

        _enemiesSpawnedInWave++;
        if (_enemiesSpawnedInWave == _gameManager.currentSizeOfWave)
        {
            _stopSpawningEnemy = true;
        }
    }

    IEnumerator SpawnBasicPowerUps()
    {
        yield return new WaitForSeconds(2.0f);
        while (_stopSpawning != true)
        {
            Vector3 pxToSpawn = new Vector3(Random.Range(-9f, 9f), 8f, 0);
            int randomPowerUp = Random.Range(0, _playerBasicPowerUps.Length); // spawn Power Ups Elements 0 to Length of Array
            GameObject newPowerUp = Instantiate(_playerBasicPowerUps[randomPowerUp], pxToSpawn, Quaternion.identity);
            newPowerUp.transform.parent = powerUpContainer.transform;

            yield return new WaitForSeconds(Random.Range(2f, 5f)); // original figures were 15f & 25f
        }
    }

    IEnumerator SpawnHealthAndNegPowerUps()
    {
        yield return new WaitForSeconds(2.0f);
        while (_stopSpawning != true)
        {
            Vector3 pxToSpawn = new Vector3(Random.Range(-9f, 9f), 8f, 0);
            int randomPowerUp = Random.Range(0, _healthAndNegPowerUps.Length); // spawn Power Ups Elements 0 to Length of Array
            GameObject newPowerUp = Instantiate(_healthAndNegPowerUps[randomPowerUp], pxToSpawn, Quaternion.identity);
            newPowerUp.transform.parent = powerUpContainer.transform;

            yield return new WaitForSeconds(Random.Range(2f, 5f)); // original figures were 15f & 25f
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
        isPlayerDestroyed = true;
    }

    public void OnPlayerReset()
    {
        _stopSpawning = true;
        isPlayerDestroyed = true;
    }

    public void OnPlayerReady()
    {
        _stopSpawning = false;
        isPlayerDestroyed = false;
    }

    public void AdvanceToNextLevel()
    {
        if (isPlayerDestroyed == false)
        {
            StartCoroutine(StartNewWave());
        }
    }

    IEnumerator TurnBossWeaponsBackOn()
    {
        yield return new WaitForSeconds(2.5f);
    }
}
