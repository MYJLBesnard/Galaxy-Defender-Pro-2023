using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasers : MonoBehaviour
{
    [SerializeField] private float _laserSpeed;
    [SerializeField] private float _playerLaserSpeed = 8.0f;
    [SerializeField] private float _enemyLaserSpeed;

    [SerializeField] private Player _player;
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private bool _isPlayerLaser = false,
                                  _isPlayerLateralLaser = false,
                                  _isEnemyLaser = false,
                                  _isEnemyRearShootingLaser = false,
                                  _isEnemyArcLaser = false;

    [SerializeField] private bool _playSound = true;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _playerLaserAudioClip;
    [SerializeField] private AudioClip _enemyLaserBasicAudioClip;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_player == null)
        {
            Debug.LogError("The Player is NULL.");
        }

        if (_spawnManager == null)
        {
            Debug.LogError("The SpawnManager is null.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("The AudioSource on the Lasers script is NULL!");
        }
        else
        {
            _audioSource.clip = _playerLaserAudioClip; // default
        }

        _playerLaserSpeed = _gameManager.currentPlayerLaserSpeed;
        _enemyLaserSpeed = _gameManager.currentEnemyLaserSpeed;

    }

    void Update()
    {
        if (_isPlayerLaser == true)
        {
            _playerLaserSpeed = _gameManager.currentPlayerLaserSpeed;
            _laserSpeed = _playerLaserSpeed;
            LaserMoveUp();
        }

        if (_isPlayerLateralLaser == true)
        {
            _playerLaserSpeed = _gameManager.currentPlayerLaserSpeed;
            _laserSpeed = _playerLaserSpeed;
            LaserMoveLateral();
        }

        if (_isEnemyLaser == true)
        {
            _enemyLaserSpeed = _gameManager.currentEnemyLaserSpeed;
            _laserSpeed = _enemyLaserSpeed;
            LaserMoveDown();
        }

        if (_isEnemyRearShootingLaser == true)
        {
            _enemyLaserSpeed = _gameManager.currentEnemyLaserSpeed;
            _laserSpeed = _enemyLaserSpeed;
            LaserMoveUp();
        }
    }

    public void PlayClip(AudioClip soundEffectClip)
    {
        if (soundEffectClip != null)
        {
            _audioSource.PlayOneShot(soundEffectClip);
        }
    }

    void PlayLaserAudioClip()
    {
        if (_playSound == true)
        {
            if (_isPlayerLaser == true || _isPlayerLateralLaser == true)
            {
                PlayClip(_playerLaserAudioClip);
            }

            if (_isEnemyLaser == true || _isEnemyRearShootingLaser == true || _isEnemyArcLaser == true)
            {
                PlayClip(_enemyLaserBasicAudioClip);
            }
        }
        _playSound = false;


    }

    void LaserMoveUp()
    {
        transform.Translate(_laserSpeed * Time.deltaTime * Vector3.up);
        PlayLaserAudioClip();
        DestroyObjectsOutsideRadius();
    }

    void LaserMoveDown()
    {
        transform.Translate(_laserSpeed * Time.deltaTime * Vector3.down);
        PlayLaserAudioClip();
        DestroyObjectsOutsideRadius();
    }

    void LaserMoveLateral()
    {
        transform.Translate(_laserSpeed * Time.deltaTime * Vector3.left);
        transform.Translate(_laserSpeed * Time.deltaTime * Vector3.right);
        PlayLaserAudioClip();
        DestroyObjectsOutsideRadius();
    }

    void DestroyObjectsOutsideRadius()
    {
        Vector3 worldCenter = Vector3.zero;
        float radius = _spawnManager.radius;
        if (Vector3.Distance(transform.position, worldCenter) > radius)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            if (transform.parent == null)
            {
                Destroy(this.gameObject);
            }

            PlayLaserAudioClip();
            Destroy(this.gameObject, 5.0f);
        }
    }


    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;

        if (_isEnemyRearShootingLaser == true)
        {
            _isEnemyLaser = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _isEnemyLaser == true || other.CompareTag("Player") && _isEnemyRearShootingLaser == true)
        {
            //Player player = other.GetComponent<Player>();

            if (_player != null)
            {
                _player.Damage();
                Destroy(this.gameObject);
            }

            CleanUpContainers();

        }

        if (other.CompareTag("PowerUpsBasic") && _isEnemyLaser == true || other.CompareTag("PowerUpsBasic") && _isEnemyRearShootingLaser == true)
        {
            CleanUpContainers();
        }

        if (other.CompareTag("PowerUpsHealth") && _isEnemyLaser == true || other.CompareTag("PowerUpsHealth") && _isEnemyRearShootingLaser == true)
        {
            CleanUpContainers();
        }

        if (other.CompareTag("PowerUpsWeapons") && _isEnemyLaser == true || other.CompareTag("PowerUpsWeapons") && _isEnemyRearShootingLaser == true)
        {
            CleanUpContainers();
        }

        if (other.CompareTag("Player") && _isEnemyArcLaser == true)
        {
            if (_player != null)
            {
                _player.Damage();
            }

            CleanUpContainers();
        }

        if (other.CompareTag("Enemy") && _isPlayerLaser == true || other.CompareTag("Enemy") && _isPlayerLateralLaser == true)
        {
            CleanUpContainers();
            /*
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            if (transform.parent == null)
            {
                Destroy(this.gameObject);
            }

            Destroy(this.gameObject);
            */
        }

        if (other.CompareTag("PowerUpsBasic") && _isPlayerLaser == true || other.CompareTag("PowerUpsBasic") && _isPlayerLateralLaser == true)
        {
            CleanUpContainers();
        }

        if (other.CompareTag("PowerUpsHealth") && _isPlayerLaser == true || other.CompareTag("PowerUpsHealth") && _isPlayerLateralLaser == true)
        {
            CleanUpContainers();
        }

        if (other.CompareTag("PowerUpsWeapons") && _isPlayerLaser == true || other.CompareTag("PowerUpsWeapons") && _isPlayerLateralLaser == true)
        {
            CleanUpContainers();
        }

        if (other.CompareTag("Asteroid") && _isPlayerLaser == true || other.CompareTag("Asteroid") && _isPlayerLateralLaser == true)
        {
            CleanUpContainers();
        }
    }

    void CleanUpContainers()
    {
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }

        Destroy(this.gameObject, 5.0f);
    }

}


