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

    private void Start()
    {
        _asteroidSpeed = 2.0f;

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

        if (transform.position.y <= 7.0f)
        {
            transform.position = new Vector3(transform.position.x, 7.0f, 0);
        }

        if (transform.position.y <= 7.0f && _startOfGame == true)
        {
            SpaceCommandDestroyAsteroid();
        }
    }

    public void SpaceCommandDestroyAsteroid()
    {
        _startOfGame = false;
        _player.AsteroidBlockingSensors();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "LaserPlayer" || other.tag == "HomingMissilePlayer")
        {
            if (_spawnManager.asteroidHit == false)
            {
                _spawnManager.asteroidHit = true; // prevents second (or multi shot) player laser to be detected.  Only the first trigger gets reported.
                GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                explosion.transform.parent = _spawnManager.explosionContainer.transform;

                _player.AsteroidDestroyed();

                {
                    if (transform.parent != null)
                    {
                        Destroy(transform.parent.gameObject);
                    }
                }
                Destroy(other.gameObject);
                Destroy(GetComponent<Rigidbody2D>());
                Destroy(GetComponent<BoxCollider2D>());
                Destroy(this.gameObject, 0.5f);

            }
        }
    }
}

