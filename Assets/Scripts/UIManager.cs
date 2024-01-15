using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private Player _player;
    [SerializeField] private BoxCollider2D _playerBoxCollider2D;
    [SerializeField] private FadeEffect _fadeEffect;
    [SerializeField] private int _maxAmmoStoresUI;
    [SerializeField] private int _maxMissileStoresUI;


    [Header("UI Images")]
    [SerializeField] private Sprite[] _livesSprite;
    [SerializeField] private Image _LivesImg;

    [Header("UI Text Fields")]
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _ammoCountText;
    [SerializeField] private TMP_Text _homingMissileCountText;
    [SerializeField] private TMP_Text _readyText;
    [SerializeField] private TMP_Text _setText;
    [SerializeField] private TMP_Text _defendText;
    [SerializeField] private TMP_Text _weaponsFree;
    [SerializeField] private TMP_Text _coreTempWarning;
    [SerializeField] private TMP_Text _coreShutdownText;
    [SerializeField] private TMP_Text _coreTempStableText;
    [SerializeField] private Button _newGame;
    [SerializeField] private Button _quitToCredits;

    public TMP_Text continueOptionTxt;
    public Button playerDecidesYes;
    public Button playerDecidesNo;

    [Header("Debugging HUD Display")]
    [SerializeField] private TMP_Text _TotalLevels;
    [SerializeField] private TMP_Text _LevelNumber;
    [SerializeField] private TMP_Text _LevelName;
    [SerializeField] private TMP_Text _SizeOfWave;
    [SerializeField] private TMP_Text _EnemySpeed;
    [SerializeField] private TMP_Text _EnemyLaserSpeed;
    [SerializeField] private TMP_Text _EnemyROS;
    [SerializeField] private TMP_Text _EnemyROF;
    [SerializeField] private TMP_Text _PowerUpROS;
    [SerializeField] private TMP_Text _EnemySensorRng;
    [SerializeField] private TMP_Text _EnemyMineLayerProb;

    [Header("Game Over Display")]
    [SerializeField] private TMP_Text _gameOverText;
    [SerializeField] private GameObject _gameOverTextBlock;
    [SerializeField] private float _letterDisplayDelay;
    [SerializeField] private float _flashDelay;
    [SerializeField] private int _flashCount;

    void Start()
    {
        _scoreText.text = "SCORE: " + 0;
        _gameOverText.enabled = true;
        _gameOverTextBlock.gameObject.SetActive(false);
        _newGame.gameObject.SetActive(false);
        _quitToCredits.gameObject.SetActive(false);

        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _fadeEffect = GameObject.Find("CanvasFader").GetComponent<FadeEffect>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _playerBoxCollider2D = GameObject.Find("Player").GetComponent<BoxCollider2D>();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

        if (_gameManager == null)
        {
            Debug.Log("Game Manager is NULL.");
        }

        if (_player == null)
        {
            Debug.Log("The PlayerScript is null.");
        }

        if (_playerBoxCollider2D == null)
        {
            Debug.Log("Player BoxCollider is NULL.");
        }

        if (_spawnManager == null)
        {
            Debug.Log("The Spawn Manager is null.");
        }

        UpdateLevelInfo();

        _letterDisplayDelay = 0.5f;
        _flashDelay = 0.5f;
        _flashCount = 5;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && _gameManager.isPlayerDestroyed == true)
        {
            FadeOutToGameScene();
        }

        if (Input.GetKeyDown(KeyCode.X) && _gameManager.isPlayerDestroyed == true)
        {
            FadeOutToCredits();
        }
    }

    public void UpdateLevelInfo()
    {
        _TotalLevels.text = "Levels: " + _gameManager.howManyLevels.ToString();
        _LevelNumber.text = "Current Level: " + _gameManager.currentLevelNumber.ToString();
        _LevelName.text = "Level Name: " + _gameManager.currentLevelName.ToString();
        _SizeOfWave.text = "Size of Wave: " + _gameManager.currentSizeOfWave.ToString();
        _EnemySpeed.text = "Enemy Speed: " + _gameManager.currentEnemySpeed.ToString();
        _EnemyLaserSpeed.text = "Enemy Laser Speed: " + _gameManager.currentEnemyLaserSpeed.ToString();
        _EnemyROS.text = "Enemy ROS: " + _gameManager.currentEnemyRateOfSpawning.ToString();
        _EnemyROF.text = "Enemy ROF: " + _gameManager.currentEnemyRateOfFire.ToString();
        _EnemySensorRng.text = "Enemy Sensor Rng: " + _gameManager.currentEnemySensorRange.ToString();
        _EnemyMineLayerProb.text = "Mine Layer Prob: " + _gameManager.currentEnemyMineLayerChance.ToString();
    }

    public void ReadySetGo()
    {
        StartCoroutine(CountDownToDefend());
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "SCORE: " + playerScore.ToString();
    }

    public void SetMaxAmmoCount(int maxAmmo)
    {
        _maxAmmoStoresUI = maxAmmo;
    }

    public void SetMaxMissileCount(int maxMissiles)
    {
        _maxMissileStoresUI = maxMissiles;
    }

    public void UpdateAmmoCount(int ammoCount)
    {
        _ammoCountText.text = "AMMO: " + ammoCount.ToString() + "/" + _maxAmmoStoresUI;

        if (ammoCount > (_maxAmmoStoresUI * 0.66))
        {
            _ammoCountText.color = Color.green;
        }

        else if (ammoCount <= (_maxAmmoStoresUI * 0.66) && ammoCount > (_maxAmmoStoresUI * 0.33))
        {
            _ammoCountText.color = Color.yellow;
        }
        else if (ammoCount <= (_maxAmmoStoresUI * 0.33))
        {
            _ammoCountText.color = Color.red;
        }
    }

    public void UpdateHomingMissileCount(int homingMissileCount)
    {
        _homingMissileCountText.text = "Missiles: " + homingMissileCount.ToString() + "/" + _maxMissileStoresUI;

        if (homingMissileCount > (_maxMissileStoresUI * 0.66))
        {
            _homingMissileCountText.color = Color.green;
        }
        else if (homingMissileCount <= (_maxMissileStoresUI * 0.66) && homingMissileCount > (_maxMissileStoresUI * 0.33))
        {
            _homingMissileCountText.color = Color.yellow;
        }
        else if (homingMissileCount <= (_maxMissileStoresUI * 0.33))
        {
            _homingMissileCountText.color = Color.red;
        }
    }

    public void UpdateLives(int currentLives)
    {
        _LivesImg.sprite = _livesSprite[currentLives];

        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    public void CoreTempWarning(bool state)
    {
        if (state == true)
        {
            _coreTempWarning.enabled = true;

        }
        else
        {
            _coreTempWarning.enabled = false;

        }
    }

    public void CoreShutdown(bool state)
    {
        if (state == true)
        {
            _coreShutdownText.enabled = true;
            _coreTempWarning.enabled = false;

        }
        else
        {
            _coreShutdownText.enabled = false;
        }
    }

    public void CoreTempStable(bool state)
    {
        if (state == true)
        {
            _coreTempStableText.enabled = true;
        }
        else
        {
            _coreTempStableText.enabled = false;
        }
    }

    public void ContinueOption()
    {
        continueOptionTxt.gameObject.SetActive(true);
        playerDecidesYes.gameObject.SetActive(true);
        playerDecidesNo.gameObject.SetActive(true);
    }

    public void ContinueOptionTxt()
    {
        continueOptionTxt.gameObject.SetActive(true);
        playerDecidesYes.gameObject.SetActive(true);
        playerDecidesNo.gameObject.SetActive(true);
    }

    public void TurnOffContinueOptionText()
    {
        continueOptionTxt.gameObject.SetActive(false);
        playerDecidesYes.gameObject.SetActive(false);
        playerDecidesNo.gameObject.SetActive(false);
    }

    private void GameOverSequence()
    {
        _gameManager.GameOver();

        _gameOverTextBlock.SetActive(true);
        string msg = _gameOverText.text;
        _gameOverText.text = null;
        StartCoroutine(GameOverRoutine(msg));
    }

    IEnumerator GameOverRoutine(string msg) // Types out msg and flashes text
    {
        yield return new WaitForSeconds(2.0f);
        WaitForSeconds letterDelay = new WaitForSeconds(_letterDisplayDelay);

        for (int i = 0; i < msg.Length; i++)
        {
            _gameOverText.text += msg[i].ToString();
            yield return letterDelay;
        }

        yield return new WaitForSeconds(2.0f);
        _gameOverText.text = null;
        _gameOverTextBlock.SetActive(false);

        WaitForSeconds flashDelay = new WaitForSeconds(_flashDelay);
        bool flashGameOver = true;
        int flashCount = 0;

        _newGame.gameObject.SetActive(true);
        _quitToCredits.gameObject.SetActive(true);

        while (flashGameOver)
        {
            yield return flashDelay;
            _gameOverText.enabled = false;
            yield return flashDelay;
            _gameOverText.enabled = true;

            flashCount++;
            if (flashCount >= _flashCount)
            {
                flashGameOver = false;
            }

            _gameOverTextBlock.SetActive(false);
        }
    }

    IEnumerator CountDownToDefend()
    {
        //_playerBoxCollider2D.GetComponent<BoxCollider2D>().enabled = false;
        //_playerBoxCollider2D.GetComponent<BoxCollider>().enabled = false;

        _spawnManager.OnPlayerReset();

        yield return new WaitForSeconds(0.25f);
        _readyText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        _readyText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.25f);
        _setText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        _setText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.25f);
        _defendText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        _defendText.gameObject.SetActive(false);

        //_playerBoxCollider2D.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void WeaponsFreeMsg()
    {
        StartCoroutine(DisplayWeapnsFreeMsg());
    }

    IEnumerator DisplayWeapnsFreeMsg()
    {
        _weaponsFree.gameObject.SetActive(true);
        yield return new WaitForSeconds(4.5f);
        _weaponsFree.gameObject.SetActive(false);
    }

    public void FadeOutToGameScene() // Fades out and loads a new game at the default difficulty level (Rookie)
                                     // or to whichever difficulty level was set in the options scene.
                                     // (Player lost all lives and Game Over)
    {
        _gameOverText.text = null;
        _gameManager.StopMusic(2.0f);
        _fadeEffect.FadeOut();
        StartCoroutine(LoadMainGame());
    }

    IEnumerator LoadMainGame() // Loads a new game from Game scene when Player has lost all lives
    {
        yield return new WaitForSeconds(2.0f);
        _gameManager.StartNewGame();
    }

    public void FadeOutToCredits() // Quits and fades out to the credits scene (Player lost all lives and Game Over)
    {
        _gameManager.StopMusic(2.0f);
        _fadeEffect.FadeOut();
        StartCoroutine(LoadCredits());
    }

    IEnumerator LoadCredits() // Loads credit scene from Game scene when Player has lost all lives
    {
        yield return new WaitForSeconds(2.0f);
        _gameManager.QuitGame();
    }
}
