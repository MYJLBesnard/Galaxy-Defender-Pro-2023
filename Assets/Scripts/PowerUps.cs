using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private EndOfLevelDialogue _endOfLevelDialogue;

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


    //public AudioClip[] PowerUpAudioClips;
    //[SerializeField] private AudioClip _powerUpAudioClip = null;


    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _endOfLevelDialogue = GameObject.Find("DialoguePlayer").GetComponent<EndOfLevelDialogue>();

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

        if (_endOfLevelDialogue == null)
        {
            Debug.Log("Dialogue Player is NULL.");
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
            Player player = other.transform.GetComponent<Player>();
            //float SFXClipAtPointVol = _gameManager.SFXPwrUpVolume;
            //AudioSource.PlayClipAtPoint(_powerUpAudioClip, new (0,0,-10), _gameManager.SFXPwrUpVolume);
            

            if (player != null)
            {
                /*
                if (_endOfLevelDialogue.PowerUpAudioIsBossDefeated == false)
                {
                    _endOfLevelDialogue.PlayPowerUpDialogue(_powerUpAudioClip);
                }
                */

                switch (_powerUpID)
                {
                    case 0:
                        player.MultiShotActivate();
                        //_powerUpAudioClip = _endOfLevelDialogue.PowerUpAudioClips[_powerUpID];
                        break;
                    case 1:
                        player.SpeedBoostActivate();
                        //_powerUpAudioClip = _endOfLevelDialogue.PowerUpAudioClips[_powerUpID];
                        break;
                    case 2:
                        player.ShieldsActivate();
                        //_powerUpAudioClip = _endOfLevelDialogue.PowerUpAudioClips[_powerUpID];
                        break;
                    case 3:
                        player.PlayerRegularAmmo(Random.Range(3,6));
                        //_powerUpAudioClip = _endOfLevelDialogue.PowerUpAudioClips[_powerUpID];
                        break;
                    case 4:
                        player.PlayerHomingMissiles(5);
                        //_powerUpAudioClip = _endOfLevelDialogue.PowerUpAudioClips[_powerUpID];
                        break;
                    case 5:
                        player.LateralLaserShotActive();
                        //_powerUpAudioClip = _endOfLevelDialogue.PowerUpAudioClips[_powerUpID];
                        break;
                    case 6:
                        player.HealthBoostActivate();
                        //_powerUpAudioClip = _endOfLevelDialogue.PowerUpAudioClips[_powerUpID];
                        break;
                    case 7:
                        player.NegativePowerUpCollision();
                        //_powerUpAudioClip = _endOfLevelDialogue.PowerUpAudioClips[_powerUpID];
                        break;
                    default:
                        break;
                }
            }

            //AudioSource.PlayClipAtPoint(_powerUpAudioClip, new(0, 0, -10), _gameManager.SFXPwrUpVolume);

            //AudioSource.PlayClipAtPoint(_powerUpAudioClip, transform.position);

            Destroy(this.gameObject);
        }

        if (other.CompareTag("LaserPlayer"))
        {
            GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            explosion.transform.parent = _spawnManager.explosionContainer.transform;

            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }

        if (other.CompareTag("LaserEnemy"))
        {
            if (_powerUpID != 7)
            {
                GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                explosion.transform.parent = _spawnManager.explosionContainer.transform;

                Destroy(other.gameObject);
                Destroy(this.gameObject);
            }
        }
    }
}

