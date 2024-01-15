
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemiesCoreMovement : MonoBehaviour
{
    [SerializeField] private Player _playerScript;
    //[SerializeField] private GameObject _player;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private SpawnManager _spawnManager;

    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyRayCast;
    [SerializeField] private GameObject _forwardDetectionBox;
    [SerializeField] private GameObject _rearDetectionBox;
    [SerializeField] private GameObject _thrusters;
    [SerializeField] private GameObject[] _enemyThrusterPrefabs;


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

    enum TypeEnemy { Basic, LaserBurst, Raming, Dodging, Arc, RearShooting };
    [Header("Types of Enemy")]
    [SerializeField] TypeEnemy enemyType = new TypeEnemy();
    private bool _isBasicEnemy = true;
    private bool _isLaserBurstEnemy = false;
    private bool _isRamingEnemy = false;
    private bool _isDodgingEnemy = false;
    private bool _isArcShootingEnemy = false;
    private bool _isRearShootingEnemy = false;

    private int _pointsWorth = 10;

    // Set values of public variables
    public bool isBasicEnemy { get { return _isBasicEnemy; } }
    public bool isLaserBurstEnemy { get { return _isLaserBurstEnemy; } }
    public bool isRamingEnemy { get { return _isRamingEnemy; } }
    public bool isDodgingEnemy { get { return _isDodgingEnemy; } }
    public bool isArcShootingEnemy { get { return _isArcShootingEnemy; } }
    public bool isRearShootingEnemy { get { return _isRearShootingEnemy; } }
    public int pointsWorth { get { return _pointsWorth; } }



    [SerializeField] private float _avoidanceTurnSpeed = 0.5f;
    private bool _movingLeft = false;
    private bool _movingRight = false;

    public bool enemyDestroyed = false;
    [SerializeField] private bool _stopUpdating = false;


    enum EnemySprite { Enemy0, Enemy1, Enemy2, EnemyBasic, EnemyBasicDodger, EnemyRearShooting, EnemyShipLightBrown, MineLayingSaucer };
    [Header("Enemy Sprite Selection")]
    [SerializeField] EnemySprite enemySprite = new EnemySprite();

    private SpriteRenderer _spriteRenderer;
    private Sprite _spriteSelected;

    [SerializeField] private Sprite[] _enemySprite;

    BoxCollider2D m_Collider;
    float m_ScaleX, m_ScaleY, m_OffsetX, m_OffsetY;
    [SerializeField] private bool _setColliderManually = false;
    [Range(0.1f, 30)][SerializeField] private float _colliderSizeX = 1f;
    [Range(0.1f, 30)][SerializeField] private float _colliderSizeY = 1f;
    [Range(0f, 30)][SerializeField] private float _colliderOffsetX = 0f;
    [Range(0f, 30)][SerializeField] private float _colliderOffsetY = 0f;


    void Start()
    {
        _playerScript = GameObject.Find("Player").GetComponent<Player>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

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

        _thrusters = null;

        m_Collider = GetComponent<BoxCollider2D>();

        //These are the starting sizes for the Collider component
        m_ScaleX = 1.0f;
        m_ScaleY = 1.0f;
        m_OffsetX = 1.0f;
        m_OffsetY = 1.0f;

    }

    void Update()
    {
        CalculateMovement();
        EnemyTypeSorting();

        if (Input.GetKeyDown(KeyCode.L))
        {
            ShowDebugLog();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            if (_thrusters != null)
            {
                _thrusters.SetActive(false);
            }

            EnemySpriteSelection();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {

            _thrusters.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {

            _thrusters.SetActive(false);
        }

        m_ScaleX = _colliderSizeX;
        m_ScaleY = _colliderSizeY;
        m_OffsetX = _colliderOffsetX;
        m_OffsetY = _colliderOffsetY;

        if (_setColliderManually == true) // used to overide the default collider size and offset for each enemy template
        {
            m_Collider.size = new Vector2(m_ScaleX, m_ScaleY);
            m_Collider.offset = new Vector2(m_OffsetX, m_OffsetY);
        }

    }

    void ShowDebugLog()
    {
        Debug.Log("is Basic Enemy: " + isBasicEnemy);
        Debug.Log("is Laser Burst Enemy: " + isLaserBurstEnemy);
        Debug.Log("is Raming Enemy: " + isRamingEnemy);
        Debug.Log("is Dodging Enemy: " + isDodgingEnemy);
        Debug.Log("is Arc Shooting Enemy: " + isArcShootingEnemy);
        Debug.Log("is Rear Shooting Enemy: " + isRearShootingEnemy);
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
                    //EnemyMovementLateral();
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

    void EnemyTypeSorting()
    {
        switch (enemyType)
        {

            case TypeEnemy.Basic:
                {
                    _isBasicEnemy = true;
                    _isLaserBurstEnemy = false;
                    _isRamingEnemy = false;
                    _isDodgingEnemy = false;
                    _isArcShootingEnemy = false;
                    _isRearShootingEnemy = false;
                    _enemyRayCast.SetActive(false);
                    _forwardDetectionBox.SetActive(false);
                    _rearDetectionBox.SetActive(false);
                    _pointsWorth = 10;
                }

                break;

            case TypeEnemy.LaserBurst:
                {
                    _isBasicEnemy = false;
                    _isLaserBurstEnemy = true;
                    _isRamingEnemy = false;
                    _isDodgingEnemy = false;
                    _isArcShootingEnemy = false;
                    _isRearShootingEnemy = false;
                    _enemyRayCast.SetActive(true);
                    _forwardDetectionBox.SetActive(false);
                    _rearDetectionBox.SetActive(false);
                    _pointsWorth = 15;

                }

                break;

            case TypeEnemy.Raming:
                {
                    _isBasicEnemy = false;
                    _isLaserBurstEnemy = false;
                    _isRamingEnemy = true;
                    _isDodgingEnemy = false;
                    _isArcShootingEnemy = false;
                    _isRearShootingEnemy = false;
                    _enemyRayCast.SetActive(true);
                    _forwardDetectionBox.SetActive(false);
                    _rearDetectionBox.SetActive(false);
                    _pointsWorth = 15;

                }

                break;
            case TypeEnemy.Dodging:
                {
                    _isBasicEnemy = false;
                    _isLaserBurstEnemy = false;
                    _isRamingEnemy = false;
                    _isDodgingEnemy = true;
                    _isArcShootingEnemy = false;
                    _isRearShootingEnemy = false;
                    _enemyRayCast.SetActive(false);
                    _forwardDetectionBox.SetActive(true);
                    _rearDetectionBox.SetActive(false);
                    _pointsWorth = 20;

                }

                break;

            case TypeEnemy.Arc:
                {
                    _isBasicEnemy = false;
                    _isLaserBurstEnemy = false;
                    _isRamingEnemy = false;
                    _isDodgingEnemy = false;
                    _isArcShootingEnemy = true;
                    _isRearShootingEnemy = false;
                    _enemyRayCast.SetActive(true);
                    _forwardDetectionBox.SetActive(false);
                    _rearDetectionBox.SetActive(false);
                    _pointsWorth = 25;


                }

                break;

            case TypeEnemy.RearShooting:
                {
                    _isBasicEnemy = false;
                    _isLaserBurstEnemy = false;
                    _isRamingEnemy = false;
                    _isDodgingEnemy = false;
                    _isArcShootingEnemy = false;
                    _isRearShootingEnemy = true;
                    _enemyRayCast.SetActive(false);
                    _forwardDetectionBox.SetActive(false);
                    _rearDetectionBox.SetActive(true);
                    _pointsWorth = 25;

                }

                break;
            default:
                break;
        }
    }
   
    void EnemySpriteSelection()
    {

        switch (enemySprite)
        {
            case EnemySprite.Enemy0:
                {
                    _spriteSelected = _enemySprite[0];
                    _thrusters = _enemyThrusterPrefabs[0];
                    Debug.Log("Sprite selected: " + _spriteSelected);
                    _spriteRenderer.sprite = _spriteSelected;
                    transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
                    _thrusters.SetActive(true);
                    m_Collider.size = new Vector2(1.246837f, 2.817182f);
                    m_Collider.offset = new Vector2(-0.01325393f, 0f);


                }
                break;

            case EnemySprite.Enemy1:
                {
                    _spriteSelected = _enemySprite[1];
                    _thrusters = _enemyThrusterPrefabs[1];
                    Debug.Log("Sprite selected: " + _spriteSelected);
                    _spriteRenderer.sprite = _spriteSelected;
                    transform.localScale = new Vector3(0.07f, 0.1f, 0.1f);
                    _thrusters.SetActive(true);
                    m_Collider.size = new Vector2(11.22427f, 11.77472f);
                    m_Collider.offset = new Vector2(-0.03585529f, 0.05971336f);


                }
                break;

            case EnemySprite.Enemy2:
                {
                    _spriteSelected = _enemySprite[2];
                    _thrusters = _enemyThrusterPrefabs[2];
                    Debug.Log("Sprite selected: " + _spriteSelected);
                    _spriteRenderer.sprite = _spriteSelected;
                    transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    _thrusters.SetActive(true);
                    m_Collider.size = new Vector2(1.413621f, 2.817182f);
                    m_Collider.offset = new Vector2(0.0187459f, 0f);



                }
                break;

            case EnemySprite.EnemyBasic: // Laser Burst
                {
                    _spriteSelected = _enemySprite[3];
                    _thrusters = _enemyThrusterPrefabs[3];
                    Debug.Log("Sprite selected: " + _spriteSelected);
                    _spriteRenderer.sprite = _spriteSelected;
                    transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    _thrusters.SetActive(true);
                    m_Collider.size = new Vector2(2.249659f, 2.817182f);
                    m_Collider.offset = new Vector2(0.006802082f, -5.960464e-08f);



                }
                break;

            case EnemySprite.EnemyBasicDodger: // Dodger
                    {
                    _spriteSelected = _enemySprite[4];
                    _thrusters = _enemyThrusterPrefabs[4];
                    Debug.Log("Sprite selected: " + _spriteSelected);
                    _spriteRenderer.sprite = _spriteSelected;
                    transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    _thrusters.SetActive(true);
                    m_Collider.size = new Vector2(2.249659f, 2.817182f);
                    m_Collider.offset = new Vector2(0.006802082f, -5.960464e-08f);



                }
                break;

            case EnemySprite.EnemyRearShooting:
                {
                    _spriteSelected = _enemySprite[5];
                    _thrusters = _enemyThrusterPrefabs[5];
                    Debug.Log("Sprite selected: " + _spriteSelected);
                    _spriteRenderer.sprite = _spriteSelected;
                    transform.localScale = new Vector3(0.2f, 0.15f, 1.0f);
                    _thrusters.SetActive(true);
                    m_Collider.size = new Vector2(5.826554f, 0.05707085f);
                    m_Collider.offset = new Vector2(0.4708622f, -0.09240818f);



                }
                break;

            case EnemySprite.EnemyShipLightBrown:
                {
                    _spriteSelected = _enemySprite[6];
                    _thrusters = _enemyThrusterPrefabs[6];
                    Debug.Log("Sprite selected: " + _spriteSelected);
                    _spriteRenderer.sprite = _spriteSelected;
                    transform.localScale = new Vector3(0.2f, 0.15f, 1.0f);
                    _thrusters.SetActive(true);
                    m_Collider.size = new Vector2(5.68f, 7.63f);
                    m_Collider.offset = new Vector2(0f, 0f);



                }
                break;

            case EnemySprite.MineLayingSaucer:
                {
                    _spriteSelected = _enemySprite[7];
                    _thrusters = _enemyThrusterPrefabs[7];
                    Debug.Log("Sprite selected: " + _spriteSelected);
                    _spriteRenderer.sprite = _spriteSelected;
                    transform.localScale = new Vector3(0.4f, .075f, 1.0f);
                    _thrusters.SetActive(true);
                    m_Collider.size = new Vector2(3.193728f, 7.303684f);
                    m_Collider.offset = new Vector2(-0.6410794f, -0.09240818f);


                }
                break;

            default:
                break;
        }

    }

    void EnemyMovementVertical()
    {
        //_enemySpeed = _gameManager.currentEnemySpeed;
        _enemySpeed = 0;

        if (_stopUpdating == false)
        {
            if (_isMovementSinusoidal == false)
            {
                float radius = _spawnManager.Radius;
                Vector3 worldCenter = Vector3.zero;
                transform.Translate(_enemySpeed * Time.deltaTime * Vector3.down);

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

    /*
    void EnemyMovementLateral()
    {
        _enemySpeed = _gameManager.currentEnemySpeed;

        if (_stopUpdating == false)
        {
            if (_gameManager.enemyMineLayerDirectionRight == true)
            {
                transform.Translate(_enemySpeed * Time.deltaTime * Vector3.right);

                if (transform.position.x >= _enemiesWeapons.releasePoint && _enemiesWeapons.isMineLayerArmed == true)
                {
                    _enemiesWeapons.DeployMines();
                    _enemiesWeapons.isMineLayerArmed = false;
                }

                if (transform.position.x > 13.5f)
                {
                    _enemiesWeapons.noSound = true;
                    _enemiesWeapons.DestroyEnemyShip();
                }
            }

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
                    _enemiesWeapons.noSound = true;
                    _enemiesWeapons.DestroyEnemyShip();
                }
            }
        }
    }
    */

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
        if (isDodgingEnemy == true)
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
