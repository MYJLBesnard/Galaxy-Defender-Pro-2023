using UnityEngine;

public class SinusMove : MonoBehaviour
{

    [SerializeField] private bool _stopUpdating = false;
    [SerializeField] public bool _speedBurstActive = false;

    [SerializeField] private Player _playerScript;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private SpawnManager _spawnManager;

    [SerializeField] private float _enemySpeed;
    [SerializeField] private float _xPos = 0;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _thrusters;

    [SerializeField] private bool _enemyDestroyed = false;



    private float x, y, z;

    public float _randomXStartPos = 0;

    [SerializeField] private bool _incomingPlayerLaser = false;
    public int randomNumber;


    enum Type { Vertical, Lateral, Circumference };
    [Header("Types of Movement")]
    [SerializeField] Type _movementType = new Type();
    [SerializeField] private bool _isMovementSinusoidal = false;


    void Start()
    {
        _playerScript = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        _randomXStartPos = Random.Range(-8.0f, 8.0f);
        randomNumber = Random.Range(-10, 10); // used to randomly pick left or right dodge


        if (_playerScript == null)
        {
            Debug.LogError("The Player is null.");
        }

        if (_spawnManager == null)
        {
            Debug.LogError("The SpawnManager is null.");
        }

        if (_gameManager == null)
        {
            Debug.LogError("The Game Manager is null.");
        }


        LookAtPlayer();
    }





    void Update()
    {
        MoveEnemy();

    }

    void MoveEnemy()
    {
        _enemySpeed = _gameManager.currentEnemySpeed;
        //transform.Translate(_enemySpeed * Time.deltaTime * Vector3.down);
        transform.Translate(_enemySpeed * Time.deltaTime * Vector3.down, Space.Self);



        float radius = _spawnManager.Radius;
        Vector3 worldCenter = Vector3.zero;

        if (Vector3.Distance(_enemyPrefab.transform.position, worldCenter) > radius)
        {
            if (_playerScript.isPlayerAlive == true && _enemyDestroyed == false)
            {
                transform.position = Random.insideUnitCircle.normalized * radius;
                LookAtPlayer();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    void LookAtPlayer()
    {
        if (_playerScript.isPlayerAlive == true)
        {
            Vector3 playerPos = _playerScript.transform.position;
            Vector3 enemyPos = _enemyPrefab.transform.position;
            Vector2 direction = enemyPos - playerPos;

            float angle = Vector2.SignedAngle(Vector2.up, direction);
            _enemyPrefab.transform.eulerAngles = new Vector3(0, 0, angle);
        }
        else if (_playerScript.isPlayerAlive == false)
        {
            Destroy(this.gameObject);
        }
    }
}
