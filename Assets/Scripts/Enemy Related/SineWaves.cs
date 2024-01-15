using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineWaves : MonoBehaviour
{
    /*

    public float speed;

    public float amplitude;

    float startingVal;

    void Start()
    {
        startingVal = transform.position.y;
    }
    void Update()
    {
        Sine(speed, amplitude);
    }
    void Sine(float Speed, float Amplitude)
    {
        float x = transform.position.x;
        float z = transform.position.z;
        float y = Mathf.Sin(Time.time * Speed) * Amplitude;

        transform.position = new Vector3(x, startingVal + y, z);
    }



    // transform.Translate(new Vector3(0,0,1) * Time.deltaTime* 2,Space.Self);
    */



    //[SerializeField] private Player _playerScript;
    //[SerializeField] private GameObject _player;
    [SerializeField] private GameManager _gameManager;

    [SerializeField] public float _enemySpeed;
    [SerializeField] private float _sineAmplitude = 0.5f;
    [SerializeField] private float _sineFrequency = 0.5f;
    private float x, y, z;
    public float _randomXStartPos = 0;
    public int randomNumber;


    void Start()
    {
        //_playerScript = GameObject.Find("Player").GetComponent<Player>();
       // _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        _randomXStartPos = Random.Range(-8.0f, 8.0f);
        _sineAmplitude = Random.Range(1.0f, 2.5f);
        randomNumber = Random.Range(-10, 10); // used to randomly pick left or right dodge

        /*
        if (_playerScript == null)
        {
            Debug.LogError("The Player is null.");
        }

        if (_spawnManager == null)
        {
            Debug.LogError("The SpawnManager is null.");
        }
        */

        if (_gameManager == null)
        {
            Debug.LogError("The Game Manager is null.");
        }
    }

    void Update()
    {
        EnemyMovementSinusoidal();
    }

    void EnemyMovementSinusoidal()
    {
        _enemySpeed = _gameManager.currentEnemySpeed;

            y = transform.position.y;
            //z = transform.position.z;
            x = Mathf.Cos((_enemySpeed * Time.time * _sineFrequency) * _sineAmplitude);

            transform.position = new Vector3((x + _randomXStartPos), y, 0);
            transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime, Space.Self);

            
            if (transform.position.y < -7.0f)
            {
                float randomX = Random.Range(-8f, 8f);
                transform.position = new Vector3(randomX, 7.0f, 0);
            }
            

    }


}
