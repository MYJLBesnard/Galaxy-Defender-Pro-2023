using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Player _playerScript;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private EnemyCore _enemyCore;
   // [SerializeField] private EnemiesWeapons _enemiesWeapons;

    [SerializeField] private GameObject _enemyPrefab;


    [SerializeField] private float _enemySpeed;
    [SerializeField] private float _sineAmplitude = 0.5f;
    [SerializeField] private float _sineFrequency = 0.5f;
    private float _x, _y, _z;
    private float _xPos = 0;
    public int randomNumber;

    //[SerializeField] public bool _speedBurstActive = false;
    [SerializeField] private bool _incomingPlayerLaser = false;

    enum TypeMovement { Vertical, Lateral, Circumference, Random };
    [Header("Types of Movement")]
    [SerializeField] TypeMovement movementType = new TypeMovement();
    [SerializeField] private bool _isMovementSinusoidal = false;

    [SerializeField] private float _avoidanceTurnSpeed = 0.5f;
    private bool _movingLeft = false;
    private bool _movingRight = false;

    public bool enemyDestroyed = false;
    [SerializeField] private bool _stopUpdating = false;



    void Start()
    {
        _playerScript = GameObject.Find("Player").GetComponent<Player>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _enemyCore = GameObject.Find("EnemyCore").GetComponent<EnemyCore>();
       // _enemiesWeapons = GameObject.Find("EnemiesWeapons").GetComponent<EnemiesWeapons>();

        _sineAmplitude = Random.Range(1.0f, 2.5f);
        //_randomXStartPos = Random.Range(-8.0f, 8.0f);
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

        _enemyPrefab = _enemyCore.enemyPrefab;

    }

    void Update()
    {
        CalculateMovement();
    }

    void CalculateMovement()
    {
        switch (movementType)
        {
            case TypeMovement.Vertical:
                {
                    EnemyMovementVertical();
                }
                break;

            case TypeMovement.Lateral:
                {
                    _isMovementSinusoidal = false;
                    EnemyMovementLateral();
                }
                break;

            case TypeMovement.Circumference:
                {
                    _isMovementSinusoidal = false;
                    EnemyMovementCircumference();
                }
                break;
            default:
                break;
        }
    }

    void EnemyMovementVertical()
    {
        //_enemySpeed = _gameManager.currentEnemySpeed;
        //
        _enemySpeed = 0;

        if (_stopUpdating == false)
        {
            if (_isMovementSinusoidal == false)
            {
                float radius = _spawnManager.Radius;
                Vector3 worldCenter = Vector3.zero;
                transform.Translate(_enemySpeed * Time.deltaTime * Vector3.down);

                //_enemyPrefab = _enemyCore.enemyPrefab;

                if (Vector3.Distance(_enemyPrefab.transform.position, worldCenter) > radius && _playerScript.isPlayerAlive == true && enemyDestroyed == false)
                {
                    _xPos = Random.Range(-8f, 8f);
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    transform.position = new(_xPos, 10.0f, 0);
                }
                else if (transform.position.y < -7.0f && _playerScript.isPlayerAlive == false)
                {
                    Destroy(this.gameObject);
                }
            }

            else if (_isMovementSinusoidal == true)
            {
                _y = transform.position.y;
                _z = transform.position.z;
                _x = Mathf.Cos((_enemySpeed * Time.time * _sineFrequency) * _sineAmplitude);

                transform.position = new Vector3((_x + _xPos), _y, _z);
                transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

                if (transform.position.y < -7.0f)
                {
                    _xPos = Random.Range(-8.0f, 8.0f);
                    transform.position = new Vector3(_xPos, 7.0f, 0);
                }
            }
        }
    }

    
    void EnemyMovementLateral()
    {
        //_enemySpeed = _gameManager.currentEnemySpeed;
        //
        _enemySpeed = 1;

        Debug.Log("Moving Mine Layer across screen at position x: " + transform.position.x);

        if (_stopUpdating == false)
        {
            if (_gameManager.enemyMineLayerDirectionRight == true)
            {
                transform.Translate(_enemySpeed * Time.deltaTime * Vector3.right);

                /*
                if (transform.position.x >= _enemiesWeapons.releasePoint && _enemiesWeapons.isMineLayerArmed == true)
                {
                    _enemiesWeapons.DeployMines();
                    _enemiesWeapons.isMineLayerArmed = false;
                }
                */

                if (transform.position.x > 13.5f)
                {
                    Debug.Log("Reached right boundary");
                    //_enemiesWeapons.noSound = true;
                    //_enemiesWeapons.DestroyEnemyShip();
                    Destroy(this.gameObject);
                }
            }

            /*
            if (_gameManager.enemyMineLayerDirectionRight == false)
            {
                transform.Translate(_enemySpeed * Time.deltaTime * Vector3.right);

                
                if (transform.position.x <= _enemiesWeapons.releasePoint && _enemiesWeapons.isMineLayerArmed == true)
                {
                    _enemiesWeapons.DeployMines();
                    _enemiesWeapons.isMineLayerArmed = false;
                }
                

                if (transform.position.x < -13.5f)
                {
                    Debug.Log("Reached left boundary");
                    //noSound = true;
                    //_enemiesWeapons.DestroyEnemyShip();
                    Destroy(this.gameObject);

                }
            }
            */
        }
    }
    

    void EnemyMovementCircumference()
    {
        _enemySpeed = _gameManager.currentEnemySpeed;

        float radius = _spawnManager.Radius;
        Vector3 worldCenter = Vector3.zero;
        transform.Translate(_enemySpeed * Time.deltaTime * Vector3.down);

        if (Vector3.Distance(_enemyPrefab.transform.position, worldCenter) > radius)
        {
            if (_playerScript.isPlayerAlive == true && enemyDestroyed == false)
            {
                transform.position = Random.insideUnitCircle.normalized * radius;
                LookAtPlayer();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        if (Vector3.Distance(_enemyPrefab.transform.position, worldCenter) > radius)
        {
            if (_playerScript.isPlayerAlive == true && enemyDestroyed == false)
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



    public IEnumerator SpeedBurst()
    {
        if (_stopUpdating == false)
        {
            Debug.Log("Running EnemiesCoreMovement: SpeedBurst coroutine");

            _enemySpeed = 10.0f;
            transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.1f);
        }
    }


    public IEnumerator DodgePlayerLaser()
    {

        _incomingPlayerLaser = true;
        yield return new WaitForSeconds(0.25f);
        _incomingPlayerLaser = false;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_enemyCore.isDodgingEnemy == true)
        {
            if (other.tag != "Player" && other.transform.position.x - transform.position.x > 0)
            {
                _movingRight = true;
            }

            if (other.tag != "Player" && other.transform.position.x - transform.position.x < 0)
            {
                _movingLeft = true;
            }

            //    if (other.tag == "Player" || other.tag == "Asteroid") Shoot();

            CalculateDodgeMovement();
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _movingRight = false;
        _movingLeft = false;

    }

    void CalculateDodgeMovement()
    {
        Vector3 forward = new Vector3(0, 0, 1);
        Vector3 left = new Vector3(-_avoidanceTurnSpeed, 0, 1);
        Vector3 right = new Vector3(_avoidanceTurnSpeed, 0, 1);
        Vector3 position = transform.position;

        if (_movingLeft)
        {
            transform.Translate(left * _enemySpeed * Time.deltaTime);
            //_anim.SetBool("MovingLeft", true);
        }
        else if (_movingRight)
        {
            transform.Translate(right * _enemySpeed * Time.deltaTime);
            //_anim.SetBool("MovingRight", true);
        }
        else if (!_movingLeft || !_movingRight)
        {
            transform.Translate(forward * _enemySpeed * Time.deltaTime);
            //_anim.SetBool("MovingLeft", false);
            //_anim.SetBool("MovingRight", false);
        }
    }

    /*
    void EnemyMovementDodgePlayerLaser()
    {
        _enemySpeed = _gameManager.currentEnemySpeed;

        if (_stopUpdating == false)
        {
            transform.Translate(_enemySpeed * Time.deltaTime * Vector3.down);

            if (_incomingPlayerLaser == true) // dodges incoming Player laser
            {
                if (randomNumber > 0)
                {
                    transform.Translate(20f * Time.deltaTime * Vector3.left);
                    transform.Translate(_enemySpeed * Time.deltaTime * Vector3.down);
                }
                else if (randomNumber < 0)
                {
                    transform.Translate(20f * Time.deltaTime * Vector3.right);
                    transform.Translate(_enemySpeed * Time.deltaTime * Vector3.down);
                }
            }

            if (transform.position.x > 10.25f)
            {
                transform.position = new Vector3(-10.25f, transform.position.y, 0);
            }
            else if (transform.position.x < -10.25f)
            {
                transform.position = new Vector3(10.25f, transform.position.y, 0);
            }

            if (transform.position.y < -7.0f)
            {
                _xPos = Random.Range(-8f, 8f);
                transform.position = new Vector3(_xPos, 7.0f, 0);
            }
        }
    }
    */

}

