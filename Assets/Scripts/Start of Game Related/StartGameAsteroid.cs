using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StartGameAsteroid : MonoBehaviour
{
    [SerializeField] private GameObject _asteroid;
    [SerializeField] private float _asteroidSpeed;
    [SerializeField] private bool _startOfGame = true;
    [SerializeField] private GameObject _explosionPrefab;
    private GameManager _gameManager;
    private SpawnManager _spawnManager;
    private Player _player;


    void Start()
    {
        _asteroidSpeed = 1.75f;

        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_gameManager == null)
        {
            Debug.Log("The Game Manager is null.");
        }

        if (_spawnManager == null)
        {
            Debug.Log("The Spawn Manager is null.");
        }

        if (_player == null)
        {
            Debug.Log("The Player script is null.");
        }
    }

    void Update()
    {
        transform.Translate(Vector3.down * _asteroidSpeed * Time.deltaTime);

        if (transform.position.y <= 5f)
        {
            transform.position = new Vector3(transform.position.x, 5f, 0f);
        }

        if (transform.position.y <= 5f && _startOfGame == true)
        {
            SpaceCommandDestroyAsteroid();
        }
    }

    public void SpaceCommandDestroyAsteroid()
    {
        _startOfGame = false;
        _player.AsteroidBlockingSensors();
        Debug.Log("Done giving order to destroy asteroid!");
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("1 - Start game asteroid hit!");

        if (other.CompareTag("LaserPlayer") || other.CompareTag("PlayerHomingMissile"))
        {
            Debug.Log("2 - Start game asteroid hit!");
            if (_spawnManager.AsteroidHit == false)
            {
                //_spawnManager.asteroidHit = true; // prevents second (or multi shot) player laser to be detected.  Only the first trigger gets reported.
                _spawnManager.StartGameAsteroidDestroyed();
                GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                explosion.transform.parent = _spawnManager.ExplosionContainer.transform;

                _player.AsteroidDestroyed();

                {
                    if (transform.parent != null)
                    {
                        Destroy(transform.parent.gameObject, 0.5f);
                    }
                }
                Destroy(other.gameObject);
                Destroy(GetComponent<Rigidbody>());
                Destroy(GetComponent<CapsuleCollider>());
                //Destroy(this.gameObject, 2.5f);

            }
        }
    }
}

