using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private float _powerUpSpeed;
    [SerializeField] private AudioClip _audioClip;

    [SerializeField] private int _powerUpID;    // ID for PwrUp:
                                                // 0 = Triple Shot
                                                // 1 = Speed Boost
                                                // 2 = Shields
                                                // 6 = Health (Ship Repairs)


                                                // remainder will be developed in next phase of game development  
                                                // 3 = Ammo
                                                // 4 = Homing Missiles
                                                // 5 = Lateral Laser Canon
                                                // 7 = Negative PowerUp


    private void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_spawnManager == null)
        {
            Debug.LogError("The SpawnManager is NULL.");
        }

        if (_player == null)
        {
            Debug.LogError("The Player is NULL.");
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
        _powerUpSpeed = _spawnManager.powerUpSpeed;
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

            AudioSource.PlayClipAtPoint(_audioClip, transform.position);

            if (player != null)
            {
                switch (_powerUpID)
                {
                    case 0:
                        player.MultiShotActivate();
                        break;
                    case 1:
                        player.SpeedBoostActivate();
                        break;
                    case 2:
                        player.ShieldsActivate();
                        break;
                    case 3:
                        player.PlayerRegularAmmo();
                        break;
                    case 4:
                        player.PlayerHomingMissiles();
                        break;
                    case 5:
                        player.LateralLaserShotActive();
                        break;
                    case 6:
                        player.HealthBoostActivate();
                        break;
                    case 7:
                        player.NegativePowerUpCollision();
                        break;
                    default:
                        break;
                }
            }
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

