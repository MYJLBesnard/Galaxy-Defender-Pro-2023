using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Basic : MonoBehaviour
{
    [SerializeField] private Player _playerScript;
    [SerializeField] private GameObject _player;
    [SerializeField]  private GameManager _gameManager;
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private bool _normalEnemyMovement = true;

    [SerializeField] private float _enemySpeed;
    [SerializeField] private float _xPos = 0;
    [SerializeField] private Animator _animEnemyDestroyed;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _thrusters;

    [SerializeField] private GameObject _enemyBasicLaser;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private AudioClip _explosionAudioClip;

    [SerializeField] private float _wpnReadyToFire;
    [SerializeField] private float _enemyRateOfFire = 1.0f;
    [SerializeField] private bool _enemyDestroyed = false;

    //private Rigidbody _rigidbody;

    public Transform player;
    public float rotSpeed = 45.0f; // degrees per second

    void Start()
    {
        //_rigidbody = GetComponent<Rigidbody>();

        _playerScript = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _animEnemyDestroyed = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        player = _player.transform;

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

        if (_animEnemyDestroyed == null)
        {
            Debug.LogError("The Enemy_Basic Animator is NULL.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("The Enemy Basic AudioSource is NULL.");
        }
        else
        {
            _audioSource.clip = _explosionAudioClip;
        }
    }

    void Update()
    {
        EnemyMovement();

        /*
        if (_enemyDestroyed == false)
        {
            FireEnemyBasicLaser();
        }
        

        if (_normalEnemyMovement == false)
        {
            //LookAtPlayer();
            StartCoroutine(LookAtPlayer());
            

        }
        */
    }

    void FireEnemyBasicLaser()
    {
        if (Time.time > _wpnReadyToFire && _enemyDestroyed == false)

        {
            _enemyRateOfFire = _gameManager.currentEnemyRateOfFire;
            _wpnReadyToFire = Time.time + (_enemyRateOfFire * Random.Range(1f, 3f));

            if (_gameManager.currentEnemyRateOfFire != 0)
            {  
                Vector3 laserPx = new(_enemyPrefab.transform.position.x, _enemyPrefab.transform.position.y - 0.35f, _enemyPrefab.transform.position.z);
                GameObject enemyLaser = Instantiate(_enemyBasicLaser, laserPx, transform.rotation);
                enemyLaser.transform.parent = _spawnManager.EnemyLaserContainer.transform;    
            }        
        }
    }

   
    void EnemyMovement()
    {
        //_enemySpeed = _gameManager.currentEnemySpeed;
        _enemySpeed = 1f;

        if (_normalEnemyMovement == true)
        {
            StartCoroutine(LookAtPlayer());
        }

        {
            float radius = _spawnManager.Radius;
            Vector3 worldCenter = Vector3.zero;

            transform.Translate(_enemySpeed * Time.deltaTime * Vector3.down);

            if (Vector3.Distance(_enemyPrefab.transform.position, worldCenter) > radius && _playerScript.isPlayerAlive == true && _enemyDestroyed == false)
            {
                _xPos = Random.Range(-8f, 8f);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                transform.position = new(_xPos, 10.0f, 0);
            }
            else if (transform.position.y < -6.0F && _playerScript.isPlayerAlive == false)
            {
                Destroy(this.gameObject);
            }
        }

        /*
        //else if (_playerScript.normalEnemyMovement == false)
        {
            float radius = _spawnManager.Radius;
            Vector3 worldCenter = Vector3.zero;
            transform.Translate(_enemySpeed * Time.deltaTime * Vector3.down);

            if (Vector3.Distance(_enemyPrefab.transform.position, worldCenter) > radius)
            {
                if(_playerScript.isPlayerAlive == true && _enemyDestroyed == false)
                {
                    transform.position = Random.insideUnitCircle.normalized * radius;
                    //LookAtPlayer();
                    //RotateTowardsPlayer();
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
        }
        */
    }



    IEnumerator LookAtPlayer()
    {
        yield return new WaitForSeconds(5f);

        Debug.Log("Running LookAtPlayer() Coroutine");
        _normalEnemyMovement = false;


        if (_playerScript.isPlayerAlive == true && _normalEnemyMovement == false)
        {
            Vector3 playerPos = _playerScript.transform.position;
            Vector3 enemyPos = _enemyPrefab.transform.position;
            Vector2 direction = enemyPos - playerPos;

            float angle = Vector2.SignedAngle(Vector2.up, direction);

            Vector3 targetRotation = new Vector3(0, 0, angle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), rotSpeed * Time.deltaTime);
        }

        else if (_playerScript.isPlayerAlive == false)
        {
            Destroy(this.gameObject);
        }

        yield return new WaitForSeconds(5f);

        Debug.Log("Coroutine done");
        _normalEnemyMovement = true;
    }
  

    public void PlayClip(AudioClip soundEffectClip)
    {
        if (soundEffectClip != null)
        {
            _audioSource.PlayOneShot(soundEffectClip);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.transform.TryGetComponent<Player>(out var player))
            {
                player.Damage();
            }
            Debug.Log("Enemy_Basic : Player hit Enemy ship...");
            PlayClip(_explosionAudioClip);
            DestroyEnemyShip();
        }

        if (other.gameObject.CompareTag("LaserPlayer"))
        {
            Destroy(other.gameObject);
            PlayClip(_explosionAudioClip);
            DestroyEnemyShip();
            _playerScript.AddScore(10);
        }

        if (other.gameObject.CompareTag("PlayerHomingMissile"))
        {
            Destroy(other.gameObject);
            PlayClip(_explosionAudioClip);
            DestroyEnemyShip();
            _playerScript.AddScore(10);
        }
    }

    private void DestroyEnemyShip()
    {
        _enemyDestroyed = true;

        //Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        _animEnemyDestroyed.SetTrigger("OnEnemyBasicDeath");
        _spawnManager.EnemyShipsDestroyedCounter();
        StartCoroutine(TurnOffThrusters());

        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<BoxCollider>());
        Destroy(this.gameObject, 2.8f);
    }


    IEnumerator TurnOffThrusters()
    {
        yield return new WaitForSeconds(0.5f);
        _thrusters.SetActive(false);
    }

}
