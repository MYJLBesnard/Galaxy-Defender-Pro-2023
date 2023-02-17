using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool normalEnemyMovement = true;
    [SerializeField] private float _playerRotateSpeed = 20.0f;

    private int _score;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private GameManager _gameManager;

    [Header("AudioClips")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _powerupAudioClip;
    [SerializeField] private AudioClip _asteroidBlockingSensors;
    [SerializeField] private AudioClip _warningIncomingWave;
    [SerializeField] private AudioClip _playerLaserShotAudioClip;
    [SerializeField] private AudioClip _warningCoreTempCritical;
    [SerializeField] private AudioClip _warningCoreTempExceeded;
    [SerializeField] private AudioClip _coreTempNominal;
    [SerializeField] private AudioClip _playerShields100AudioClip;
    [SerializeField] private AudioClip _playerShields65AudioClip;
    [SerializeField] private AudioClip _playerShields35AudioClip;
    [SerializeField] private AudioClip _playerShieldsDepletedAudioClip;
    [SerializeField] private AudioClip _shipRepairsUnderwayAudioClip;
    [SerializeField] private AudioClip _explosionSoundEffect;

    [Header("First Start / New Game Variables")]
    [SerializeField] private bool _asteroidDestroyed = false;

    [Header("Player Laser Variables")]
    [SerializeField] private bool _hasPlayerLaserCooledDown = true;
    [SerializeField] private float _wpnCoolDown;
    [SerializeField] private float _wpnReadyToFire;
    [SerializeField] private float _playerRateOfFire = 0.15f;

    [Header("Multi Shot Variables")]
    [SerializeField] private bool _isPlayerMultiShotActive = false; 
    [SerializeField] private int _numberOfProjectiles = 3;
    [Range(0, 360)][SerializeField] private float _spreadAngle = 30;

    [Header("Speed Variables")]
    [SerializeField] private float _playerSpeed = 5.0f;
    [SerializeField] private bool _isPlayerSpeedBoostActive = false;
    [SerializeField] private float _speedMultiplier = 1.75f;

    [Header("Shields / Damage Variables")]
    [SerializeField] private bool _isPlayerShieldsActive = false;
    [SerializeField] private int _shieldHits = 0;
    [SerializeField] private float _playerShieldAlpha = 1.0f;
    private GameObject[] _playerDamage;
    [SerializeField] private List<GameObject> poolDamageAnimations = new List<GameObject>();
    [SerializeField] private List<GameObject> activatedDamageAnimations = new List<GameObject>();

    [Header("Inspector assigned")]
    [SerializeField] private int _playerLives = 3;
    public bool isPlayerAlive = true;

    [Header("UI Elements")]
    [SerializeField] private int _playerScore = 0;

    [Header("Game Objects")]
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _laserNode1; 
    [SerializeField] private GameObject _playerLaserPrefab;  
    [SerializeField] private GameObject _playerDoubleShotLaserPrefab;
    [SerializeField] private GameObject _playerMultiShotLaserPrefab;
    public GameObject playerLaserContainer;  


    [SerializeField] private GameObject _playerShield, _playerHealthPowerUpPrefab;
    [SerializeField] private GameObject _playerThrusterLeft, _playerThrusterRight;
    [SerializeField] private GameObject _playerNoseThrusterLeft, _playerNoseThrusterRight;
    [SerializeField] private GameObject _bigExplosionPrefab;
    public GameObject enemyBoss;

    // Internals
    private Transform _myTransform = null;                            // Cached transform component
    private Vector3 _myPosition = Vector3.zero;                       // Current position
    private Vector3 _startPosition = new(0.0f, -3.5f, 0.0f);


    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("The Game Manager is null.");
        }

        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("The SpawnManager is null.");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("The UIManager is null.");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("The AudioSource on the Player is NULL!");
        }
        else
        {
            _audioSource.clip = _playerLaserShotAudioClip;
        }

        transform.position = new Vector3(0, 0, 0); // sets start px of Player

    }

    void Update()
    {
        PlayerMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _wpnReadyToFire)
        {
            PlayerFiresLaser();
        }

        if (Input.GetKeyDown(KeyCode.N)) // testing purposes only, causes waves to attack from top or 360 degrees (See SpawnManager script)
        {
            if (normalEnemyMovement == false)
            {
                normalEnemyMovement = true;
            }
            else
            {
                normalEnemyMovement = false;
            }
        }
    }

    // Start Asteroid Logic
    public void AsteroidBlockingSensors()
    {
        StartCoroutine(WeaponsFree());
    }

    IEnumerator WeaponsFree()
    {
        yield return new WaitForSeconds(4.5f);
        Debug.Log("Weapons Free!...");
    }

    public void LaserIsWpnsFree()
    {
        _hasPlayerLaserCooledDown = true;
    }

    public void AsteroidDestroyed()
    {
        _asteroidDestroyed = true;
        StartCoroutine(WarningIncomingWave(3.0f));
    }

    IEnumerator WarningIncomingWave(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("Incoming Wave!...");
        _spawnManager.StartSpawning();
    }




    // Player Fire Laser Logic
    void PlayerFiresLaser()
    {
        if (_isPlayerMultiShotActive == true && _hasPlayerLaserCooledDown == true)
        {
            _wpnCoolDown = 1.0f;
            _wpnReadyToFire = Time.time + _wpnCoolDown;
            _playerRateOfFire = _wpnCoolDown;

            float angleStep = _spreadAngle / _numberOfProjectiles;
            float centeringOffset = (_spreadAngle / 2) - (angleStep / 2);
            float playerRotationOffset = transform.eulerAngles.z;

            for (int i = 0; i < _numberOfProjectiles; i++)
            {
                float currentBulletAngle = (angleStep * i) + playerRotationOffset;
                Vector3 laserPx = _laserNode1.transform.position;
                Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, currentBulletAngle - centeringOffset));
                GameObject playerLaser = Instantiate(_playerMultiShotLaserPrefab, laserPx, rotation);

                playerLaser.transform.parent = _spawnManager.playerLaserContainer.transform;

                _hasPlayerLaserCooledDown = false;
                StartCoroutine(PlayerLaserCoolDownTimer());
            }
        }

        else if (_isPlayerMultiShotActive == false && _hasPlayerLaserCooledDown == true)
        {
            _wpnCoolDown = 0.5f;
            _wpnReadyToFire = Time.time + _wpnCoolDown;
            _playerRateOfFire = _wpnCoolDown;

            Vector3 laserPx = _laserNode1.transform.position;
            GameObject playerLaser = Instantiate(_playerDoubleShotLaserPrefab, laserPx, transform.rotation);

            playerLaser.transform.parent = _spawnManager.playerLaserContainer.transform;

            _hasPlayerLaserCooledDown = false;
            StartCoroutine(PlayerLaserCoolDownTimer());
        }

        PlayClip(_playerLaserShotAudioClip);

    }

    public void PlayClip(AudioClip soundEffectClip)
    {
        if (soundEffectClip != null)
        {
            _audioSource.PlayOneShot(soundEffectClip);
        }
    }

    // -----------------------------------------------------------------------------
    // Name	:	Player Laser Cool Down - Coroutine
    // Desc	:	Called when the Player presses fire to set the 
    //			_hasPlayerLasedCooledDownbool to false for duration of rate of fire.
    //			This stops the Player having a rapid fire capability
    // -----------------------------------------------------------------------------
    IEnumerator PlayerLaserCoolDownTimer()
    {
        yield return new WaitForSeconds(_playerRateOfFire);
        _hasPlayerLaserCooledDown = true;
    }

    void PlayerMovement()
    {
        float playerHorizontalInput = Input.GetAxis("Horizontal");
        float playerVerticalInput = Input.GetAxis("Vertical");

        if (Input.GetKey("z")) // Player rotate CCW
        {
            _playerNoseThrusterLeft.SetActive(true);
            Vector3 playerRotateLeft = new Vector3(0, 0, 15.0f);
            transform.Rotate(_playerRotateSpeed * Time.deltaTime * playerRotateLeft);
        }
        else
        {
            _playerNoseThrusterLeft.SetActive(false);
        }

        if (Input.GetKey("c")) // Player rotate CW
        {
            _playerNoseThrusterRight.SetActive(true);
            Vector3 playerRotateRight = new Vector3(0, 0, -15.0f);
            transform.Rotate(_playerRotateSpeed * Time.deltaTime * playerRotateRight);
        }
        else
        {
            _playerNoseThrusterRight.SetActive(false);

        }

        Vector3 playerMovement = new Vector3(playerHorizontalInput, playerVerticalInput, 0);
        transform.Translate(_playerSpeed * Time.deltaTime * playerMovement);

        // x-axis boundaries
        if (transform.position.x > 11.25f)
        {
            transform.position = new Vector3(-11.25f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.25f)
        {
            transform.position = new Vector3(11.25f, transform.position.y, 0);
        }

        // y-axis boundaries
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -4f, 5f), 0);
    }

    // Player Damage Logic
    public void Damage()
    {
        if (_isPlayerShieldsActive == true)
        {
            _shieldHits++;

            switch (_shieldHits)
            {
                case 1:
                    _playerShieldAlpha = 0.75f;
                    _playerShield.GetComponent<SpriteRenderer>().material.color = new Color(1f, 1f, 1f, _playerShieldAlpha);
                    break;
                case 2:
                    _playerShieldAlpha = 0.40f;
                    _playerShield.GetComponent<SpriteRenderer>().material.color = new Color(1f, 1f, 1f, _playerShieldAlpha);
                    break;
                case 3:
                    _isPlayerShieldsActive = false;
                    _playerShield.SetActive(false);
                    break;
            }
            return;
        }

        // randomly selects damaged area and sets it active.  The removes from pool to "Active" list.
        if (poolDamageAnimations.Count > 0)
        {
            var rdmDamage = Random.Range(0, poolDamageAnimations.Count);
            var temp = poolDamageAnimations[rdmDamage];
            activatedDamageAnimations.Add(temp);
            temp.SetActive(true);
            poolDamageAnimations.Remove(temp);
            return;
        }

        if (poolDamageAnimations.Count == 0)
        {
            _playerLives--;
            _uiManager.UpdateLives(_playerLives);
            _gameManager.UpdateLivesRemaining(_playerLives);

            Instantiate(_bigExplosionPrefab, transform.position, Quaternion.identity);
            PlayClip(_explosionSoundEffect);

            //if (_gameManager.lives != 0)
            if (_playerLives != 0)

            {
                ResetDamageAnimationList();
                _gameManager.GameOver();
                isPlayerAlive = false;
                _player.SetActive(false);
            }
        }
    }

    public void ResetDamageAnimationList()
    {
     //   _spawnManager.OnPlayerReset();

        while (activatedDamageAnimations.Count > 0)
        {
            var rdmDamage = Random.Range(0, activatedDamageAnimations.Count);
            var temp = activatedDamageAnimations[rdmDamage];
            poolDamageAnimations.Add(temp);
            temp.SetActive(false);
            activatedDamageAnimations.Remove(temp);
        }
    }

    public void AddScore(int points)
    {
        _playerScore += points;
        _uiManager.UpdateScore(_playerScore);
    }

    // ----------------------------------------------------------------------------
    // Name	:	PowerUp Collected
    // Desc	:	Called when the Player collects a Power Up
    // ----------------------------------------------------------------------------
    public void MultiShotActivate()
    {
        _isPlayerMultiShotActive = true;
        Debug.Log("Multi Shot collected!");

        StartCoroutine(MultiShotCoolDownTimer());
    }

    IEnumerator MultiShotCoolDownTimer()
    {
        yield return new WaitForSeconds(5.0f);
        _isPlayerMultiShotActive = false;
    }

    public void SpeedBoostActivate()
    {
        if (_isPlayerSpeedBoostActive == false) // only give the Player a temp speed boost if the PowerUp is not already collected
        {
            _isPlayerSpeedBoostActive = true;
            _playerSpeed *= _speedMultiplier;
            StartCoroutine(SpeedBoostPowerDownTimer());
        }
        else
        {
            return;
        }
    }

    IEnumerator SpeedBoostPowerDownTimer()
    {
        yield return new WaitForSeconds(5.0f);
        _playerSpeed /= _speedMultiplier;
        _isPlayerSpeedBoostActive = false;
    }

    public void ShieldsActivate()
    {
        _isPlayerShieldsActive = true;
        _playerShield.SetActive(true);
        _shieldHits = 0;
        _playerShieldAlpha = 1.0f;
        _playerShield.GetComponent<SpriteRenderer>().material.color = new Color(1f, 1f, 1f, _playerShieldAlpha);
    }

    public void PlayerRegularAmmo()
    {
        Debug.Log("Ammo collected!");
    }

    public void PlayerHomingMissiles()
    {
        Debug.Log("Homing Missiles collected!");
    }

    public void LateralLaserShotActive()
    {
        Debug.Log("Lateral Laser collected!");
    }

    public void HealthBoostActivate()
    {
        Debug.Log("Health Boost collected!");

        // Reverses damage by removing random (if more than 1 active) damages area and returning it to the pool.
        if (activatedDamageAnimations.Count > 0)
        {
            var rdmDamage = Random.Range(0, activatedDamageAnimations.Count);
            var temp = activatedDamageAnimations[rdmDamage];
            poolDamageAnimations.Add(temp);
            temp.SetActive(false);
            activatedDamageAnimations.Remove(temp);
        }
    }

    public void NegativePowerUpCollision()
    {
        Debug.Log("Collided with Negative PU!");
    }

}
