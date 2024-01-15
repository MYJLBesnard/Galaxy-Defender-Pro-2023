using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private Player _player;

    public Transform Player;
    public GameObject DepletedLateralLaserCanons; 

    //public float radius = 13f;
    //public float spawnRate = 5.0f;

    private float _xPos;
    private float _yPos;
    private float _zPos;

    [Header("Object Containers")]
    public GameObject PlayerLaserContainer;
    public GameObject PowerUpContainer;
    public GameObject EnemyContainer;
    public GameObject EnemyLaserContainer;
    public GameObject ExplosionContainer;

    [Header("Power Up Related")]
    [SerializeField] private GameObject[] _playerBasicPowerUps; 
    [SerializeField] private GameObject[] _playerWeaponsPowerUps; 
    [SerializeField] private GameObject[] _healthAndNegPowerUps; 

    [Header("Enemy and Other Hazards Related")]
    [SerializeField] private GameObject _hazardPrefab; // left there, possibly used in spawning asteroids???
                                                       // See: public void CreateEnemyRandomPositionOnCircumference()
    [SerializeField] private GameObject _typeOfEnemy; //*********
    [SerializeField] private GameObject[] _typesOfEnemy; //*********

    [SerializeField] private int _enemiesInWave = 0;
    [SerializeField] private int _enemiesSpawnedInWave = 0;
    public int TotalEnemyShipsDestroyed = 0;

    public bool IsBossActive = false;
    public bool IsPlayerDestroyed = false;
    
    [SerializeField] private bool _stopSpawning = true;
    [SerializeField] private bool _stopSpawningEnemy = false;
    [SerializeField] private bool _stopSpawningPlayerPwrUp = false;
    [SerializeField] public bool isMineLayerDeployed = false;

    [SerializeField] private AudioClip _warningIncomingWave;
    [SerializeField] private AudioClip _attackWaveRepelled;

    public int WaveCurrent = 0;
    //private int _shipsInWave = 0;
    public int EnemyType = 0;
    private float _triggerMineLayer = 0f;

    [SerializeField] private bool _asteroidHit = false;
    public bool AsteroidHit { get { return _asteroidHit; } }

    [SerializeField] private float _radius = 15f;
    public float Radius { get { return _radius; } }


    public Transform[] enemyWaypoints;


    //************* Old variables, Boss ones will still be used, as well as waypoints

    /*
    //[SerializeField] private EndOfLevelDialogue _endOfLevelDialogue;
    
    public Transform[] bossWaypoints;

    [SerializeField] private EnemyBoss _enemyBossScript;
    [SerializeField] private GameObject _enemyBoss;
    public int bossTurretsDestroyed = 0;
    [SerializeField] private bool _bossTurretsDestroyed = false;
    public int bossMiniGunsDestroyed = 0;
    [SerializeField] private bool _bossMiniGunsDestroyed = false;
    public int bossCanonsDestroyed = 0;
    [SerializeField] private bool _bossCanonsDestroyed = false;

    */


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

        //WaveCurrent = _gameManager.currentWave;
        _enemiesInWave = _gameManager.currentSizeOfWave;
        Debug.Log("How many waves total: " + _gameManager.howManyLevels);
        Debug.Log("Current wave is " + _gameManager.currentWave);
        Debug.Log("Enemies in wave " + _gameManager.currentWave + " is " + _enemiesInWave);
    }




    public void StartGameAsteroidDestroyed()
    {
            _asteroidHit = true;
    }




    public void EnemyShipsDestroyedCounter()
    {
        TotalEnemyShipsDestroyed++;

        //if (TotalEnemyShipsDestroyed == _gameManager.currentSizeOfWave)
        if (TotalEnemyShipsDestroyed == _enemiesInWave)

        {
            Debug.Log("Counter - How many waves total: " + _gameManager.howManyLevels);
            Debug.Log("Counter - Current wave is: " + (WaveCurrent));
            //Debug.Log("Counter - Enemies in wave is: " + _enemiesInWave);

            if (_gameManager.currentWave == _gameManager.howManyLevels - 1)

            {
                // No more waves
                _stopSpawning = true;
                _stopSpawningEnemy = true;
                // need audio manager attack wave repelled clip...
            }
            else
            {
                // Current wave complete
                _gameManager.WaveComplete();
                // need audio manager attack wave repelled clip...

                //TotalEnemyShipsDestroyed = 0;
                StartCoroutine(StartNewWave());
            }
        }
    }

    IEnumerator StartNewWave()
    {
        _stopSpawning = true;
        _enemiesInWave = 0;
        _enemiesSpawnedInWave = 0;
        TotalEnemyShipsDestroyed = 0;
        yield return new WaitForSeconds(3.5f);

        _stopSpawning = false;
        _stopSpawningEnemy = false;
        OnPlayerReady();
        _enemiesInWave = _gameManager.currentSizeOfWave;
        Debug.Log("IEnumerator SpawnManager : StartNewWave running.");
        Debug.Log("StartNewWave: How many waves total: " + _gameManager.howManyLevels);
        Debug.Log("StartNewWave: Current wave is " + _gameManager.currentWave);
        Debug.Log("StartNewWave: Enemies in wave " + _gameManager.currentWave + " is " + _gameManager.currentSizeOfWave);

        StartCoroutine(SpawnEnemyWave());

    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyWave());
        //StartCoroutine(SpawnBasicPowerUps());
        //StartCoroutine(SpawnWeaponsPowerUps());
        //StartCoroutine(SpawnHealthAndNegPowerUps());
    }





    IEnumerator SpawnEnemyWave()
        {
        Debug.Log("Start SpawnEnemyWave");
        yield return new WaitForSeconds(2.0f); // delays start of enemy spawns

            while (_stopSpawningEnemy == false && _stopSpawning == false && _player.isPlayerAlive == true)
            {
            _xPos = Random.Range(-8.0f, 8.0f);
            _yPos = Random.Range(-5.5f, 5.5f);
            Vector3 pxToSpawn = new Vector3(_xPos, 9, 0);
            _triggerMineLayer = Random.Range(0, 100);

            /* Left this here to explain logic for spawning Mine Layer
            Debug.Log("Trigger Mine Layer: " + _triggerMineLayer);
            Debug.Log("Game Manager Mine Layer Chance: " + _gameManager.currentEnemyMineLayerChance);
            Debug.Log("If Trigger greater than Chance, Mine Layer should get spawned.");
            */

            if (_stopSpawningEnemy == false)
            {
                _typeOfEnemy = null;
                //WaveCurrent = _gameManager.currentWave;

                switch (_gameManager.currentWave)
                {
                    case 0: // spawn basic Enenmy
                        int type0 = 0;
                        _typeOfEnemy = _typesOfEnemy[type0];
                        EnemyType = type0;
                        break;

                    case 1: // spawn basic Enemy
                        int type1 = 1;
                        _typeOfEnemy = _typesOfEnemy[type1];
                        EnemyType = type1;
                        break;

                    case 2: // spawn dodging Enemy
                        int type2 = 2;
                        _typeOfEnemy = _typesOfEnemy[type2];
                        EnemyType = type2;
                        break;

                    case 3: // spawn speed burst Enemy
                        int type3 = 3;
                        //int type3 = Random.Range(2, 4); // replace top line with this to generate random mix within specific parameters
                        _typeOfEnemy = _typesOfEnemy[type3];
                        EnemyType = type3;
                        break;

                    case 4: // spawn laser burst / Player Laser avoidance Enemy 
                        int type4 = 4;
                        //int type4 = Random.Range(2, 5); // replace top line with this to generate random mix within specific parameters
                        _typeOfEnemy = _typesOfEnemy[type4];
                        EnemyType = type4;
                        break;

                    case 5: // spawn rear shooting Enemy
                        int type5 = 5;
                        //int type5 = Random.Range(3, 6); // replace top line with this to generate random mix within specific parameters
                        _typeOfEnemy = _typesOfEnemy[type5];
                        EnemyType = type5;
                        break;

                    case 6: // spawn arc laser Enemy
                        int type6 = 6;
                        //int type6 = Random.Range(4, 7); // replace top line with this to generate random mix within specific parameters
                        _typeOfEnemy = _typesOfEnemy[type6];
                        EnemyType = type6;
                        break;

                    case 7: // Boss
                 //       SetupBossAlien();
                 //       _enemyBossScript.isEnemyBossActive = true;
                        break;

                    default:
                        break;
                }


/*
            // Mine Layer Logic
            if (_triggerMineLayer >= _gameManager.currentEnemyMineLayerChance && isMineLayerDeployed == false)
            {
                int type7 = 7;
                _typeOfEnemy = _typesOfEnemy[type7];
                int enemyDirection = Random.Range(0, 100);

                if (enemyDirection <= 50) // Mine Layer travels Left to Right
                {
                    Vector3 pxToSpawnEnemy7 = new Vector3(-13.0f, _yPos, 0);
                    GameObject newEnemy = Instantiate(_typeOfEnemy, pxToSpawnEnemy7, Quaternion.identity);
                    newEnemy.transform.parent = EnemyContainer.transform;
                    _gameManager.enemyMineLayerDirectionRight = true;
                }
                else if (enemyDirection > 51) // Mine Layer travels Right to Left
                {
                    Vector3 pxToSpawnEnemy7 = new Vector3(13.0f, _yPos, 0);
                    GameObject newEnemy = Instantiate(_typeOfEnemy, pxToSpawnEnemy7, Quaternion.identity);
                    newEnemy.transform.Rotate(0, 0, 180);
                    newEnemy.transform.parent = EnemyContainer.transform;
                    _gameManager.enemyMineLayerDirectionRight = false;
                }

                isMineLayerDeployed = true;

            }
            else
*/
            {
                if (IsBossActive != true)
                {
                    GameObject newEnemy = Instantiate(_typeOfEnemy, pxToSpawn, Quaternion.identity);
                    newEnemy.transform.parent = EnemyContainer.transform;
                    _enemiesSpawnedInWave++;
                }
            }
        }

            if (_enemiesSpawnedInWave == _gameManager.currentSizeOfWave)
            {
                _stopSpawningEnemy = true;
            }

            yield return new WaitForSeconds(_gameManager.currentEnemyRateOfSpawning);

        }
    }




    /*
    public void CreateEnemyRandomPositionAtTop()
    {
        _xPos = Random.Range(-8.0f, 8.0f);
        Vector3 pxToSpawn = new Vector3(_xPos, 9, 0);

        GameObject newEnemy = Instantiate(_enemyPrefab);

        newEnemy.transform.parent = EnemyContainer.transform;
        newEnemy.transform.position = pxToSpawn;

        _enemiesSpawnedInWave++;
        if (_enemiesSpawnedInWave == _gameManager.currentSizeOfWave)
        {
            _stopSpawningEnemy = true;
        }
    }
    */




    // this method could be used to spawn asteroids 360 degrees around player and translate them towards Players position
    // needs fine tunig if to be implemented (Game Manager)
    public void CreateEnemyRandomPositionOnCircumference()
    {
        GameObject newEnemy = Instantiate(_hazardPrefab); // as GameObject;

        newEnemy.transform.parent = EnemyContainer.transform;
        newEnemy.transform.position = Random.insideUnitCircle.normalized * Radius;

        Vector3 playerPos = Player.transform.position;
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
            newPowerUp.transform.parent = PowerUpContainer.transform;

            yield return new WaitForSeconds(Random.Range(2f, 5f)); // original figures were 15f & 25f
        }
    }

    IEnumerator SpawnWeaponsPowerUps()
    {
        yield return new WaitForSeconds(2.0f);
        while (_stopSpawning != true)
        {
            Vector3 pxToSpawn = new Vector3(Random.Range(-9f, 9f), 8f, 0);
            int randomPowerUp = Random.Range(0, _playerWeaponsPowerUps.Length); // spawn Power Ups Elements 0 to Length of Array
            GameObject newPowerUp = Instantiate(_playerWeaponsPowerUps[randomPowerUp], pxToSpawn, Quaternion.identity);
            newPowerUp.transform.parent = PowerUpContainer.transform;

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
            newPowerUp.transform.parent = PowerUpContainer.transform;

            yield return new WaitForSeconds(Random.Range(2f, 5f)); // original figures were 15f & 25f
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
        IsPlayerDestroyed = true;
    }

    public void OnPlayerReset()
    {
        _stopSpawning = true;
        IsPlayerDestroyed = true;
    }

    public void OnPlayerReady()
    {
        _stopSpawning = false;
        IsPlayerDestroyed = false;
    }

    public void AdvanceToNextLevel()
    {
        if (IsPlayerDestroyed == false)
        {
            StartCoroutine(StartNewWave());
        }
    }

    IEnumerator TurnBossWeaponsBackOn()
    {
        yield return new WaitForSeconds(2.5f);
    }
}
