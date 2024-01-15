using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private float _powerUpSpeed;
    [SerializeField] private bool _isStartOfGamePwrUp = false;

    [SerializeField] private int _powerUpID;    // ID for PwrUp:
                                                // 0 = Triple Shot
                                                // 1 = Speed Boost
                                                // 2 = Shields
                                                // 3 = Ammo
                                                // 4 = Homing Missiles
                                                // 5 = Lateral Laser Canon
                                                // 6 = Health (Ship Repairs)
                                                // 7 = Negative PowerUp

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        if (_player == null)
        {
            Debug.LogError("The PowerUps : Player is NULL.");
        }

        if (_spawnManager == null)
        {
            Debug.LogError("The PowerUps : SpawnManager is NULL.");
        }

        if (_gameManager == null)
        {
            Debug.LogError("The PowerUps : GameManager is NULL.");
        }

        if (_audioManager == null)
        {
            Debug.LogError("The Audio MAnager is null.");
        }
    }

    void Update()
    {
        Movement();
    }

    // ----------------------------------------------------------------------------
    // Name	:	Movement
    // Desc	:	Called to move the PowerUps top-bottom on game scene
    // ----------------------------------------------------------------------------
    void Movement()
    {
        if (_isStartOfGamePwrUp == true)
        {
            _powerUpSpeed = 0f;
        }
        else
        {
            _powerUpSpeed = _gameManager.currentPowerUpSpeed;
        }

        transform.Translate(_powerUpSpeed * Time.deltaTime * Vector3.down);

        if (transform.position.y < -9.0F)
        {
            Destroy(this.gameObject);
        }
    }

    // ----------------------------------------------------------------------------
    // Desc	:	Collision Detection
    // ----------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //float SFXClipAtPointVol = _gameManager.SFXPwrUpVolume;
            //AudioSource.PlayClipAtPoint(_powerUpAudioClip, new (0,0,-10), _gameManager.SFXPwrUpVolume);
          
            if (_player != null)
            {
                /*
                if (_audioManager.PowerUpAudioIsBossDefeated == false)
                {
                    _audioManager.PlayPowerUpDialogue(_powerUpAudioClip);
                }
                */
                
                switch (_powerUpID)
                {
                    case 0:
                        _player.MultiShotActivate();
                        break;
                    case 1:
                        _player.SpeedBoostActivate();
                        break;
                    case 2:
                        _player.ShieldsActivate();
                        break;
                    case 3:
                        _player.PlayerRegularAmmo(Random.Range(3,6));
                        break;
                    case 4:
                        _player.PlayerHomingMissiles(5);
                        break;
                    case 5:
                        _player.LateralLaserShotActive();
                        break;
                    case 6:
                        _player.HealthBoostActivate();
                        break;
                    case 7:
                        _player.NegativePowerUpCollision();
                        break;
                    default:
                        break;
                }

                _audioManager.PlayPowerUpDialogue(_powerUpID);

            }

            Destroy(this.gameObject);
        }

        if (_powerUpID != 7)
        {
            if (other.CompareTag("LaserPlayer") || other.CompareTag("LaserEnemy"))
            {
                GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                explosion.transform.parent = _spawnManager.ExplosionContainer.transform;

                Destroy(other.gameObject);
                Destroy(this.gameObject);
            }
        }
    }
}

