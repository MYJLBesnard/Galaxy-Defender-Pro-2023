using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCore : MonoBehaviour
{
    [SerializeField] private Player _playerScript;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private DetectionBoxManager _detectionBoxManager;
    //[SerializeField] private EnemyMovement _enemyMovement;
    public Transform enemyTransform;

    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyRayCast;

    [SerializeField] private GameObject _thrusters;
    [SerializeField] private GameObject[] _enemyThrusterPrefabs;

    [SerializeField] private GameObject _colliders;
    [SerializeField] private GameObject[] _enemyColliderPrefabs;

    //   [SerializeField] private bool _incomingPlayerLaser = false;

    enum TypeEnemy { Basic, LaserBurst, Raming, Dodging, Arc, RearShooting, MineLayer };
    [Header("Types of Enemy")]
    [SerializeField] TypeEnemy enemyType = new TypeEnemy();
    private bool _isBasicEnemy = true;
    private bool _isLaserBurstEnemy = false;
    private bool _isRamingEnemy = false;
    private bool _isDodgingEnemy = false;
    private bool _isArcShootingEnemy = false;
    private bool _isRearShootingEnemy = false;
    private bool _isMineLayerEnemy = false;
    private int _pointsWorth = 10;

    // Set values of public variables
    public bool isBasicEnemy { get { return _isBasicEnemy; } }
    public bool isLaserBurstEnemy { get { return _isLaserBurstEnemy; } }
    public bool isRamingEnemy { get { return _isRamingEnemy; } }
    public bool isDodgingEnemy { get { return _isDodgingEnemy; } }
    public bool isArcShootingEnemy { get { return _isArcShootingEnemy; } }
    public bool isRearShootingEnemy { get { return _isRearShootingEnemy; } }
    public bool isMineLayerEnemy {  get { return _isMineLayerEnemy; } }
    public int pointsWorth { get { return _pointsWorth; } }
    public GameObject enemyPrefab { get { return _enemyPrefab; } }
    public GameObject enemyShield { get { return _enemyShield; } }

    [SerializeField] private bool _forwardDB = false;
    [SerializeField] private bool _rearDB = false;
    [SerializeField] private float _avoidanceTurnSpeed = 0.5f;
    private bool _movingLeft = false;
    private bool _movingRight = false;

    public bool enemyDestroyed = false;
    [SerializeField] private bool _stopUpdating = false;

    enum EnemySprite { Enemy0, Enemy1, Enemy2, EnemyBasic, EnemyBasicDodger, EnemyRearShooting, EnemyArcShooting, EnemyMineLayer };
    [Header("Enemy Sprite Selection")]
    [SerializeField] EnemySprite enemySprite = new EnemySprite();
    // private bool _isEnemySpriteChanging = false;

    private SpriteRenderer _spriteRenderer;
    private Sprite _spriteSelected;

    [SerializeField] private Sprite[] _enemySprite;

    /*
    // [Header("Sprite Collider Adjustments")]
    BoxCollider2D m_Collider;
    float m_ScaleX, m_ScaleY, m_OffsetX, m_OffsetY;
    [SerializeField] private bool _setColliderManually = false;
    [Range(0.1f, 30)][SerializeField] private float _colliderSizeX = 1f;
    [Range(0.1f, 30)][SerializeField] private float _colliderSizeY = 1f;
    [Range(0f, 30)][SerializeField] private float _colliderOffsetX = 0f;
    [Range(0f, 30)][SerializeField] private float _colliderOffsetY = 0f;
    */

    enum TypeShield { None, Frontal, Surround, BossShields };
    [Header("Types of Shield")]
    [SerializeField] TypeShield shieldType = new TypeShield();

    [SerializeField] private GameObject[] _enemyShieldPrefabs;
    [SerializeField] private GameObject _enemyShield; // place holder for shield type equipped
    [SerializeField] private bool _isEnemyEquippedWithShieldsFrontal = false;
    [SerializeField] private bool _isEnemyEquippedWithShieldsSurround = false;
    public bool isEnemyEquippedWithShieldsFrontal { get { return _isEnemyEquippedWithShieldsFrontal; } }
    public bool isEnemyEquippedWithShieldsSurround { get { return _isEnemyEquippedWithShieldsSurround; } }

    // [Header("Shield Adjustments")]
    Transform shieldTransform;
    float s_PosX, s_PosY, s_PosZ, s_RotX, s_RotY, s_RotZ, s_ScaleX, s_ScaleY, s_ScaleZ;
    [SerializeField] private bool _setShieldsManually = false;
    [Range(0f, 30)][SerializeField] private float _shieldPosX = 0f;
    [Range(0f, 30)][SerializeField] private float _shieldPosY = 0f;
    [Range(0f, 30)][SerializeField] private float _shieldPosZ = 0f;

    [Range(0f, 30)][SerializeField] private float _shieldRotX = 0f;
    [Range(0f, 30)][SerializeField] private float _shieldRotY = 0f;
    [Range(0f, 30)][SerializeField] private float _shieldRotZ = 0f;

    [Range(0f, 30)][SerializeField] private float _shieldScaleX = 1f;
    [Range(0f, 30)][SerializeField] private float _shielScaleY = 1f;
    [Range(0f, 30)][SerializeField] private float _shielScaleZ = 1f;




    void Start()
    {
        _playerScript = GameObject.Find("Player").GetComponent<Player>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        //_enemyMovement = GameObject.Find("EnemyMovement").GetComponent<EnemyMovement>();
        _detectionBoxManager = GameObject.Find("DetectionBoxes").GetComponent<DetectionBoxManager>();

        enemyTransform = GetComponent<Transform>();

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
        //_enemySprite = null;

        EnemySpriteSelection();
        UpdateEnemyShieldSelection();

        /*
        m_Collider = GetComponent<BoxCollider2D>();

        //These are the starting values for the Collider component
        m_ScaleX = 1.0f;
        m_ScaleY = 1.0f;
        m_OffsetX = 0.0f;
        m_OffsetY = 0.0f;
        */

        shieldTransform = GetComponent<Transform>();
        //These are the starting values for the Shield component
        s_PosX = 0f;
        s_PosY = 0f;
        s_PosZ = 0f;

        s_RotX = 0f;
        s_RotY = 0f;
        s_RotZ = 0f;

        s_ScaleX = 1.0f;
        s_ScaleY = 1.0f;
        s_ScaleZ = 1.0f;

    }

    void Update()
    {
        EnemyTypeSorting();
        //EnemySpriteSelection();
        //UpdateEnemyShieldSelection();
        UpdateDetectionBoxStatus();


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

            if (_colliders != null)
            {
                _colliders.SetActive(false);
            }

            //EnemyTypeSorting();
            EnemySpriteSelection();
            UpdateEnemyShieldSelection();
        }

        /*
        m_ScaleX = _colliderSizeX;
        m_ScaleY = _colliderSizeY;
        m_OffsetX = _colliderOffsetX;
        m_OffsetY = _colliderOffsetY;

        if (_setColliderManually == true) // used to overide the default collider size and offset for each enemy template
        {
            m_Collider.size = new Vector2(m_ScaleX, m_ScaleY);
            m_Collider.offset = new Vector2(m_OffsetX, m_OffsetY);
        }
        */
        
        s_PosX = _shieldPosX;
        s_PosY = _shieldPosY;
        s_PosZ = _shieldPosZ;
        s_RotX = _shieldRotX;
        s_RotY = _shieldRotY;
        s_RotZ = _shieldRotZ;
        s_ScaleX = _shieldScaleX;
        s_ScaleY = _shielScaleY;
        s_ScaleZ = _shielScaleZ;


        if (_setShieldsManually == true) // used to overide the default collider size and offset for each enemy template
        {
            _enemyShield.transform.position = new Vector3(s_PosX, s_PosY, s_PosZ);
            _enemyShield.transform.Rotate(s_RotX, s_RotY, s_RotZ, Space.Self);
            _enemyShield.transform.localScale = new Vector3(s_ScaleX, s_ScaleY, s_ScaleZ);
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
        Debug.Log("is Mine Layer Enemy: " + isMineLayerEnemy);
        Debug.Log("is Enemy Raycast ON: " + _enemyRayCast);
        Debug.Log("is Forward Detector ON: " + _forwardDB);
        Debug.Log("is Rear Detector ON: " + _rearDB);
        Debug.Log("points: " + pointsWorth);
    }


    void UpdateDetectionBoxStatus()
    {
        if(_forwardDB == true)
        {
            _detectionBoxManager.forwardDetectionBox2.SetActive(true);
        }
        else
        {
            _detectionBoxManager.forwardDetectionBox2.SetActive(false);
        }

        if (_rearDB == true)
        {
            _detectionBoxManager.rearDetectionBox2.SetActive(true);
        }
        else
        {
            _detectionBoxManager.rearDetectionBox2.SetActive(false);
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
                    _isMineLayerEnemy = false;
                    _enemyRayCast.SetActive(false);
                    _forwardDB = false; // test
                    _rearDB = false; // test
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
                    _isMineLayerEnemy = false;
                    _enemyRayCast.SetActive(true);
                    _forwardDB = false; // test
                    _rearDB = false; // test
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
                    _isMineLayerEnemy = false;
                    _enemyRayCast.SetActive(true);
                    _forwardDB = false; // test
                    _rearDB = false; // test
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
                    _isMineLayerEnemy = false;
                    _enemyRayCast.SetActive(false);
                    _forwardDB = true; // test
                    _rearDB = false; // test
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
                    _isMineLayerEnemy = false;
                    _enemyRayCast.SetActive(true);
                    _forwardDB = false; // test
                    _rearDB = false; // test
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
                    _isMineLayerEnemy = false;
                    _enemyRayCast.SetActive(false);
                    _forwardDB = false; // test
                    _rearDB = true; // test
                    _pointsWorth = 25;
                }
                break;

            case TypeEnemy.MineLayer:
                {
                    _isBasicEnemy = false;
                    _isLaserBurstEnemy = false;
                    _isRamingEnemy = false;
                    _isDodgingEnemy = false;
                    _isArcShootingEnemy = false;
                    _isRearShootingEnemy = false;
                    _isMineLayerEnemy = true;
                    _enemyRayCast.SetActive(false);
                    _forwardDB = false; // test
                    _rearDB = false; // test
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
                    _colliders = _enemyColliderPrefabs[0];
                    _colliders.SetActive(true);
                    _spriteRenderer.sprite = _spriteSelected;
                    _thrusters.SetActive(true);
                }
                break;

            case EnemySprite.Enemy1:
                {
                    _spriteSelected = _enemySprite[1];
                    _thrusters = _enemyThrusterPrefabs[1];
                    _colliders = _enemyColliderPrefabs[1];
                    _colliders.SetActive(true);
                    _spriteRenderer.sprite = _spriteSelected;
                    _thrusters.SetActive(true);
                }
                break;

            case EnemySprite.Enemy2:
                {
                    _spriteSelected = _enemySprite[2];
                    _thrusters = _enemyThrusterPrefabs[2];
                    _colliders = _enemyColliderPrefabs[2];
                    _colliders.SetActive(true);
                    _spriteRenderer.sprite = _spriteSelected;
                    _thrusters.SetActive(true);
                }
                break;

            case EnemySprite.EnemyBasic: // Laser Burst
                {
                    _spriteSelected = _enemySprite[3];
                    _thrusters = _enemyThrusterPrefabs[3];
                    _colliders = _enemyColliderPrefabs[3];
                    _colliders.SetActive(true);
                    _spriteRenderer.sprite = _spriteSelected;
                    _thrusters.SetActive(true);
                }
                break;

            case EnemySprite.EnemyBasicDodger: // Dodger
                {
                    _spriteSelected = _enemySprite[4];
                    _thrusters = _enemyThrusterPrefabs[4];
                    _colliders = _enemyColliderPrefabs[4];
                    _colliders.SetActive(true);
                    _spriteRenderer.sprite = _spriteSelected;
                    _thrusters.SetActive(true);
                }
                break;

            case EnemySprite.EnemyRearShooting:
                {
                    _spriteSelected = _enemySprite[5];
                    _thrusters = _enemyThrusterPrefabs[5];
                    _colliders = _enemyColliderPrefabs[5];
                    _colliders.SetActive(true);
                    _spriteRenderer.sprite = _spriteSelected;
                    _thrusters.SetActive(true);
                }
                break;

            case EnemySprite.EnemyArcShooting:
                {
                    _spriteSelected = _enemySprite[6];
                    _thrusters = _enemyThrusterPrefabs[6];
                    _colliders = _enemyColliderPrefabs[6];
                    _colliders.SetActive(true);
                    _spriteRenderer.sprite = _spriteSelected;
                    _thrusters.SetActive(true);
                }
                break;

            case EnemySprite.EnemyMineLayer:
                {
                    _spriteSelected = _enemySprite[7];
                    _thrusters = _enemyThrusterPrefabs[7];
                    _colliders = _enemyColliderPrefabs[7];
                    _colliders.SetActive(true);
                    _spriteRenderer.sprite = _spriteSelected;
                    _thrusters.SetActive(true);
                }
                break;

            default:
                break;
        }
    }

    void UpdateEnemyShieldSelection()
    {
        transform.localPosition = new Vector3(0, 0, 0);

        //_enemyShieldPrefabs[0].transform = _enemyTransform;

        if (_isEnemyEquippedWithShieldsFrontal == true || _isEnemyEquippedWithShieldsSurround == true)
        {
            //           _enemyShield.SetActive(true);
            //       }

            if (enemyDestroyed == false)
            {
                switch (shieldType)
                {
                    case TypeShield.None:
                        {
                            _isEnemyEquippedWithShieldsFrontal = false;
                            _isEnemyEquippedWithShieldsSurround = false;
                            _enemyShieldPrefabs[0].SetActive(false);
                            _enemyShieldPrefabs[1].SetActive(false);
                        }

                        break;

                    case TypeShield.Frontal:
                        {
                            _isEnemyEquippedWithShieldsFrontal = true;
                            _isEnemyEquippedWithShieldsSurround = false;
                            _enemyShieldPrefabs[0].transform.position = new Vector3(s_PosX, s_PosY, s_PosZ);
                            _enemyShieldPrefabs[0].transform.Rotate(s_RotX, s_RotY, s_RotZ, Space.Self);
                            _enemyShieldPrefabs[0].transform.localScale = new Vector3(s_ScaleX, s_ScaleY, s_ScaleZ);

                            if (_isArcShootingEnemy == true)
                            {
                                _enemyShieldPrefabs[0].transform.position = new Vector3(-0.01f, -0.91f, 0);
                                _enemyShieldPrefabs[0].transform.Rotate(0, 0, 0, Space.Self);
                                _enemyShieldPrefabs[0].transform.localScale = new Vector3(3.61f, 2.41f, 1.0f);
                            }

                            _enemyShieldPrefabs[0].SetActive(true);
                            _enemyShieldPrefabs[1].SetActive(false);
                            _enemyShield = _enemyShieldPrefabs[0];
                        }

                        break;

                    case TypeShield.Surround:
                        {
                            _isEnemyEquippedWithShieldsFrontal = false;
                            _isEnemyEquippedWithShieldsSurround = true;

                            _enemyShieldPrefabs[1].transform.position = new Vector3(s_PosX, s_PosY, s_PosZ);
                            _enemyShieldPrefabs[1].transform.Rotate(s_RotX, s_RotY, s_RotZ, Space.Self);
                            _enemyShieldPrefabs[1].transform.localScale = new Vector3(s_ScaleX, s_ScaleY, s_ScaleZ);

                            if (_isRearShootingEnemy == true)
                            {
                                _enemyShieldPrefabs[1].transform.position = new Vector3(0f, -0.75f, 0);
                                _enemyShieldPrefabs[1].transform.Rotate(0, 0, 0, Space.Self);
                                _enemyShieldPrefabs[1].transform.localScale = new Vector3(1.9f, 2.41f, 1.0f);
                            }

                            _enemyShieldPrefabs[0].SetActive(false);
                            _enemyShieldPrefabs[1].SetActive(true);
                            _enemyShield = _enemyShieldPrefabs[1];

                        }

                        break;

                    case TypeShield.BossShields: // likely will remove this case.  Boss will have all of its own unique scripts
                        {
                            _isEnemyEquippedWithShieldsFrontal = false;
                            _isEnemyEquippedWithShieldsSurround = false;
                            _enemyShieldPrefabs[0].SetActive(false);
                            _enemyShieldPrefabs[1].SetActive(false);
                            // this may get moved to unique Boss scripts...
                        }

                        break;

                    default:
                        break;
                }
            }
        }
    }

    public void ResetShieldToDefault()
    {
        shieldType = TypeShield.None;

        _isEnemyEquippedWithShieldsFrontal = false;
        _isEnemyEquippedWithShieldsSurround = false;

        _enemyShieldPrefabs[0].SetActive(false);
        _enemyShieldPrefabs[1].SetActive(false);
    }
}

