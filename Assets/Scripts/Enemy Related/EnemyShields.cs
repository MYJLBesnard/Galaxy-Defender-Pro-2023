using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyShields : MonoBehaviour
{
    [SerializeField] private Player _playerScript;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private SpawnManager _spawnManager;
    //[SerializeField] private AudioSource _audioSource;
    //[SerializeField] private EnemyCore _enemyCore;
    [SerializeField] private GameObject _enemyShield;
    [SerializeField] private GameObject _explosionPrefab;

    [SerializeField] private bool _isEnemyHitByPlayerLaser = false;
    [SerializeField] private bool _isEnemyEquippedWithShields = true;
    //    [SerializeField] private bool _isEnemyEquippedWithShieldsFrontal = false;
    //    [SerializeField] private bool _isEnemyEquippedWithShieldsSurround = false;

    /*
    enum TypeShield { None, Frontal, Surround, BossShields };
    [Header("Types of Shield")]
    [SerializeField] TypeShield shieldType = new TypeShield();

    [SerializeField] private GameObject[] _enemyShieldPrefabs;

    [SerializeField] private GameObject _enemyShield; // place holder for shield type equipped
    */
    // [SerializeField] private GameObject _enemyShieldFrontal;
    // [SerializeField] private GameObject _enemyShieldSurround;

    [SerializeField] private int _shieldHits = 0;
    [SerializeField] private float _enemyShieldAlpha = 1.0f;


    void Start()
    {
        _playerScript = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //_animEnemyDestroyed = GetComponent<Animator>();
        //_audioSource = GetComponent<AudioSource>();
        //_enemyCore = GameObject.Find("EnemyCore").GetComponent<EnemyCore>();


        if (_playerScript == null)
        {
            Debug.LogError("The Player script is null.");
        }

        if (_spawnManager == null)
        {
            Debug.LogError("The SpawnManager is null.");
        }

        if (_gameManager == null)
        {
            Debug.LogError("The Game Manager is null.");
        }

        // if (_animEnemyDestroyed == null)
        // {
        //     Debug.LogError("The Enemy_Basic Animator is NULL.");
        // }

        //if (_audioSource == null)
        //{
        //    Debug.LogError("The Enemy Basic AudioSource is NULL.");
        //}
    }

    /*
    // Update is called once per frame
    void Update()
    {
        if (_isEnemyEquippedWithShieldsFrontal == true || _isEnemyEquippedWithShieldsSurround == true)
        {
            //           _enemyShield.SetActive(true);
            //       }

            if (_enemyCore.enemyDestroyed == false)
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
                            _enemyShieldPrefabs[0].SetActive(true);
                            _enemyShieldPrefabs[1].SetActive(false);
                            _enemyShield = _enemyShieldPrefabs[0];
                        }

                        break;

                    case TypeShield.Surround:
                        {
                            _isEnemyEquippedWithShieldsFrontal = false;
                            _isEnemyEquippedWithShieldsSurround = true;
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
    */

    public void EnemyDamage()
    {
        if (_isEnemyEquippedWithShields == true)
        {
            _shieldHits++;

            switch (_shieldHits)
            {
                case 1:
                    _enemyShieldAlpha = 0.75f;
                    _enemyShield.GetComponent<SpriteRenderer>().material.color = new Color(1f, 1f, 1f, _enemyShieldAlpha);
                    break;
                case 2:
                    _enemyShieldAlpha = 0.40f;
                    _enemyShield.GetComponent<SpriteRenderer>().material.color = new Color(1f, 1f, 1f, _enemyShieldAlpha);
                    break;
                case 3:
                    _enemyShieldAlpha = 0.0f;
                    //_enemyCore.ResetShieldToDefault();
                    _enemyShield.SetActive(false);
                    _isEnemyEquippedWithShields = false;
                    break;
           }

            StartCoroutine(ResetLaserHitDetection());

            return;
        }

        DestroyEnemyShip(25); // points worth
    }

    IEnumerator ResetLaserHitDetection()
    {
        yield return new WaitForSeconds(2.0f);
        _isEnemyHitByPlayerLaser = false; // resets bool
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))// && _enemyCore.isDodgingEnemy == false && _enemyCore.isRearShootingEnemy == false)
        {
            if (other.transform.TryGetComponent<Player>(out var player))
            {
                player.Damage();
            }
            Debug.Log("EnemyShieldsAndDestruction : Player hit Enemy ship...");
            //    PlayClip(_explosionAudioClip);
            EnemyDamage();

        }

        if (other.gameObject.CompareTag("LaserPlayer"))// && _enemyCore.isDodgingEnemy == false && _enemyCore.isRearShootingEnemy == false)
        {
            if (_isEnemyHitByPlayerLaser == false)
            {
                _isEnemyHitByPlayerLaser = true;    // flipped to true upon first collision with Player laser, so
                                                    // that a double shot doesn't potentially register two hits
                                                    // since each laser shot has its own rigidbody and collider.
                Destroy(other.gameObject);
                //    PlayClip(_explosionAudioClip);
                EnemyDamage();
            }
        }

        if (other.gameObject.CompareTag("PlayerHomingMissile"))
        {

            Destroy(other.gameObject);
            //    PlayClip(_explosionAudioClip);
            EnemyDamage();

        }
    }

    /*
    public void PlayClip(AudioClip soundEffectClip)
    {
        if (soundEffectClip != null)
        {
            _audioSource.PlayOneShot(soundEffectClip);
        }
    }
    */

    private void DestroyEnemyShip(int addToScore)
    {
        _playerScript.AddScore(addToScore);
        //_enemyCore.enemyDestroyed = true;

        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        //   _animEnemyDestroyed.SetTrigger("OnEnemyBasicDeath");
        _spawnManager.EnemyShipsDestroyedCounter();
        StartCoroutine(TurnOffThrusters());

        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<BoxCollider>());
        //Destroy(this.gameObject, 2.8f); // used if enemy ship has associated destruction animation
        Destroy(this.gameObject);

    }


    IEnumerator TurnOffThrusters()
    {
        yield return new WaitForSeconds(0.5f);
        //        _thrusters.SetActive(false);
    }


}
