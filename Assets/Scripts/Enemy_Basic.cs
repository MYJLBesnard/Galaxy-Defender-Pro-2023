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

    [SerializeField] private float _enemySpeed;
    [SerializeField] private float _enemyLaserSpeed;
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

    void Start()
    {
        _playerScript = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _animEnemyDestroyed = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

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
        FireEnemyBasicLaser();
    }

    void FireEnemyBasicLaser()
    {
        if (Time.time > _wpnReadyToFire && _enemyDestroyed == false)

        {
            _enemyLaserSpeed = _gameManager.currentEnemyLaserSpeed;
            _enemyRateOfFire = _gameManager.currentEnemyRateOfFire;
            _wpnReadyToFire = Time.time + _enemyRateOfFire;

            if (_gameManager.currentEnemyRateOfFire != 0)
            {  
                Vector3 laserPx = _enemyPrefab.transform.position;
                GameObject enemyLaser = Instantiate(_enemyBasicLaser, laserPx, transform.rotation);

                enemyLaser.transform.parent = _spawnManager.enemyLaserContainer.transform;    
            }        
        }
    }

    void EnemyMovement()
    {

        _enemySpeed = _gameManager.currentEnemySpeed;


        if (_playerScript.normalEnemyMovement == true)
        {
            float radius = _spawnManager.radius;
            Vector3 worldCenter = Vector3.zero;

            transform.Translate(_enemySpeed * Time.deltaTime * Vector3.down);

            if (Vector3.Distance(_enemyPrefab.transform.position, worldCenter) > radius && _playerScript.isPlayerAlive == true && _enemyDestroyed == false)
            //if (transform.position.y < -6.0f && _playerScript.isPlayerAlive == true)

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

        else if (_playerScript.normalEnemyMovement == false)
        {
            float radius = _spawnManager.radius;
            Vector3 worldCenter = Vector3.zero;
            transform.Translate(_enemySpeed * Time.deltaTime * Vector3.down);

            if (Vector3.Distance(_enemyPrefab.transform.position, worldCenter) > radius)
            {
                if(_playerScript.isPlayerAlive == true && _enemyDestroyed == false)
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
    }

    void LookAtPlayer()
    {
        if (_playerScript.isPlayerAlive == true && _playerScript.normalEnemyMovement == false)
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

    public void PlayClip(AudioClip soundEffectClip)
    {
        if (soundEffectClip != null)
        {
            _audioSource.PlayOneShot(soundEffectClip);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            Debug.Log("Enemy collided with Player!");

            if (other.transform.TryGetComponent<Player>(out var player))
            {
                player.Damage();
            }

            PlayClip(_explosionAudioClip);

            DestroyEnemyShip();
        }

        if (other.gameObject.tag == "LaserPlayer")
        {
            Debug.Log("Player laser has collided with Enemy!");
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
        _thrusters.SetActive(false);
        Destroy(GetComponent<Rigidbody2D>());
        Destroy(GetComponent<BoxCollider2D>());
        Destroy(this.gameObject, 2.8f);
    }
}
