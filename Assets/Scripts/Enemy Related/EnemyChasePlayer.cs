using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyChasePlayer : MonoBehaviour
{
    [SerializeField] private Player _playerScript;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private SpawnManager _spawnManager;

    [SerializeField] private float _enemySpeed;
    [SerializeField] private bool _normalEnemyMovement = true;
    [SerializeField] private bool _turningToTarget = false;

    [Range(3.0f, 5.0f)][SerializeField] private float _delaySwitchingMovementState = 4.0f;
    [Range(30f, 75f)][SerializeField] private float _huntingForTargetRotationSpeed = 60f;

    [SerializeField] private GameObject _laserNode1;
    [SerializeField] private GameObject _enemyLaser;
    [SerializeField] private float _wpnReadyToFire;
    [SerializeField] private float _enemyRateOfFire;
    [SerializeField] private bool _isEnemyHitByPlayerLaser = false;

    [SerializeField] private int _pointsWorth;

    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private GameObject _thrusters;
    [SerializeField] private bool _enemyDestroyed = false;
    [SerializeField] private bool _stopUpdating = false;

    public float radius;
    public Vector3 worldCenter;
    public Vector3 playerPos;
    public Vector3 enemyPos;

    void Start()
    {
        _playerScript = GameObject.Find("Player").GetComponent<Player>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();


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
            Debug.LogError("The GameManager is null.");
        }

        radius = _spawnManager.Radius;
        worldCenter = Vector3.zero;
    }

    void Update()
    {
        playerPos = _playerScript.transform.position;
        enemyPos = transform.position;

        if (_normalEnemyMovement == true)
        {
            _turningToTarget = false;
            EnemyMovement();
        }

        if (_turningToTarget == true)
        {
            _normalEnemyMovement = false;
            LookForTarget();
        }

        FireEnemyBasicLaser();
    }

    void EnemyMovement()
    {
        _enemySpeed = _gameManager.currentEnemySpeed;
        transform.Translate(_enemySpeed * Time.deltaTime * Vector3.down);

            if (Vector3.Distance(enemyPos, worldCenter) > radius && _playerScript.isPlayerAlive == true && _enemyDestroyed == false)
            {
                float _xPos = Random.Range(-8f, 8f);
                transform.SetPositionAndRotation(new(_xPos, 10.0f, 0), Quaternion.Euler(0, 0, 0));
            }

            else if (Vector3.Distance(enemyPos, worldCenter) > radius && _playerScript.isPlayerAlive == false)
            {
                Destroy(this.gameObject);
            }

            StartCoroutine(SwitchEnemyMovement());
    }


    void LookForTarget()
    {
        Vector2 direction = enemyPos - playerPos;
        Debug.Log("direction: " + direction);
        var direction2 = enemyPos - playerPos;
        Debug.Log("direction 2: " + direction2);

        transform.Translate(_enemySpeed * Time.deltaTime * Vector3.down);

            if (_playerScript.isPlayerAlive == true)
            //if (Vector3.Distance(enemyPos, worldCenter) > radius && _playerScript.isPlayerAlive == true && _enemyDestroyed == false)
            {
                float angle = Vector2.SignedAngle(Vector2.up, direction);
            Debug.Log("angle: " + angle);
                Vector3 targetRotation = new Vector3(0, 0, angle);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), _huntingForTargetRotationSpeed * Time.deltaTime);
            }

            else if (Vector3.Distance(enemyPos, worldCenter) > radius && _playerScript.isPlayerAlive == false)
            {
                Destroy(this.gameObject);
            }

            StartCoroutine(SwitchEnemyMovement());
    }


    IEnumerator SwitchEnemyMovement()
    {
        if (_normalEnemyMovement == true)
        {
            yield return new WaitForSecondsRealtime(_delaySwitchingMovementState);
            _normalEnemyMovement = false;
            _turningToTarget = true;
        }

        if (_turningToTarget == true)
        {
            yield return new WaitForSecondsRealtime(_delaySwitchingMovementState);
            _normalEnemyMovement = true;
            _turningToTarget = false;
        }
    }

    void FireEnemyBasicLaser()
    {
        if (Time.time > _wpnReadyToFire && _enemyDestroyed == false && _playerScript.isPlayerAlive == true)
        {
            _enemyRateOfFire = _gameManager.currentEnemyRateOfFire;
            _wpnReadyToFire = Time.time + (_enemyRateOfFire * Random.Range(1f, 3f));

            if (_gameManager.currentEnemyRateOfFire != 0 && _playerScript.isPlayerAlive == true)
            {
                Vector3 laserPx = _laserNode1.transform.position;
                GameObject enemyLaser = Instantiate(_enemyLaser, laserPx, transform.rotation);
                enemyLaser.transform.parent = _spawnManager.EnemyLaserContainer.transform;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.transform.TryGetComponent<Player>(out var player))
            {
                player.Damage();
                DestroyEnemyShip(_pointsWorth);
            }
        }

        if (other.gameObject.CompareTag("LaserPlayer"))
        {
            if (_isEnemyHitByPlayerLaser == false)
            {
                _isEnemyHitByPlayerLaser = true;    // flipped to true upon first collision with Player laser, so
                                                    // that a double shot doesn't potentially register two hits
                                                    // since each laser shot has its own rigidbody and collider.
                Destroy(other.gameObject);
                DestroyEnemyShip(_pointsWorth);
            }
        }

        if (other.gameObject.CompareTag("PlayerHomingMissile"))
        {
            Destroy(other.gameObject);
            DestroyEnemyShip(_pointsWorth);
        }
    }

    IEnumerator ResetLaserHitDetection()
    {
        yield return new WaitForSeconds(2.0f);
        _isEnemyHitByPlayerLaser = false; // resets bool
    }

    private void DestroyEnemyShip(int addToScore)
    {
        _playerScript.AddScore(addToScore);
        _enemyDestroyed = true;

        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        _spawnManager.EnemyShipsDestroyedCounter();
        //StartCoroutine(TurnOffThrusters());

        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<BoxCollider>());
        //Destroy(this.gameObject, 2.8f); // used if enemy ship has associated destruction animation
        Destroy(this.gameObject);
    }

    /*
    IEnumerator TurnOffThrusters()
    {
        yield return new WaitForSeconds(0.5f);
        _thrusters.SetActive(false);
    }
    */
}
