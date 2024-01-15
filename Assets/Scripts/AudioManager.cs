using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Player _player;
    [SerializeField] private AudioSource _audioSource;

    public bool PlayOnAwake = false;
    public bool Loop = false;
    public bool PlayerAcceptsMsn = true;
    public bool MsgDonePlaying = false;
    public bool PowerUpAudioIsBossDefeated = false;

    [SerializeField] private bool _lastLevel = false;
    [SerializeField] private bool _rookie = true;
    [SerializeField] private bool _spaceCadet = false;
    [SerializeField] private bool _spaceCaptain = false;
    [SerializeField] private bool _galaxyDefender = false;

    [Header("AudioClips")]
    [SerializeField] private AudioClip[] _audioClips;
    //[SerializeField] private AudioClip _audioClip = null;

    /*
    [SerializeField] private AudioClip _asteroidBlockingSensors;
    [SerializeField] private AudioClip _warningIncomingWave;
    [SerializeField] private AudioClip _playerLaserShotAudioClip;
    [SerializeField] private AudioClip _warningCoreTempCritical;
    [SerializeField] private AudioClip _warningCoreTempExceeded;
    [SerializeField] private AudioClip _coreTempNominal;
    [SerializeField] private AudioClip _playerShields65AudioClip;
    [SerializeField] private AudioClip _playerShields35AudioClip;
    [SerializeField] private AudioClip _playerShieldsDepletedAudioClip;
    [SerializeField] private AudioClip _explosionSoundEffect;
    */

    [Header("PowerUp AudioClips")]
    [SerializeField] private AudioClip[] _powerUpAudioClips;
    //[SerializeField] private AudioClip _powerUpAudioClip = null;

    [Header("Dialgue AudioClips")]
    [SerializeField] private AudioClip[] _congratulations;
    [SerializeField] private AudioClip[] _threatPresent;
    [SerializeField] private AudioClip[] _threatCleared;
    [SerializeField] private AudioClip[] _newMsn;
    [SerializeField] private AudioClip[] _mayNotSurvive;
    [SerializeField] private AudioClip[] _pleaseAccept;
    [SerializeField] private AudioClip[] _decisionYes;
    [SerializeField] private AudioClip[] _decisionNo;
    [SerializeField] private AudioClip[] _ranks;
    [SerializeField] private AudioClip[] _returnToBase;
    [SerializeField] private AudioClip[] _promoted;

    private List<AudioClip> _audioMsgThreatStillExists = new();
    private List<AudioClip> _audioMsgPlayerDecision = new();
    private List<AudioClip> _audioMsgNoThreatExists = new();

    private AudioClip _congrats;
    private AudioClip _introRank;
    private AudioClip _threat;
    private AudioClip _msn;
    private AudioClip _risk;
    private AudioClip _plea;
    private AudioClip _decision;
    private AudioClip _rtb;
    private AudioClip _promo;
    private AudioClip _promoRank;

    // Singleton Accessor - Only one Audio Manager should ever exist
    private static AudioManager _instance = null;
    public static AudioManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (AudioManager)FindObjectOfType(typeof(AudioManager));
            }
            return _instance;
        }
    }

    private void Awake() // Initialize the Audio Manager state 
    {
        //_lives = 3;
        //_currentScore = 0;
        //_currentWave = 0;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _player = GameObject.Find("Player").GetComponent<Player>();


        if (_gameManager == null)
        {
            Debug.Log("Game Manager is NULL.");
        }

        if (_player == null)
        {
            Debug.Log("Player script is NULL.");
        }

        List<AudioClip> _audioMsgThreatStillExists = new();
        List<AudioClip> _audioMsgPlayerDecision = new();
        List<AudioClip> _audioMsgNoThreatExists = new();

        // make sure we have a reference to the AudioSource
        if (_audioSource == null)
        {
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
            else
            {
                _audioSource = gameObject.GetComponent<AudioSource>();
            }
        }
        _audioSource.playOnAwake = PlayOnAwake;
        _audioSource.loop = Loop;

        // this loop just make sure that alll the AudioClips in the soundList are valid
        for (int i = (_audioMsgThreatStillExists.Count - 1); i >= 0; i--)
        {
            if (_audioMsgThreatStillExists[i] == null)
            {
                _audioMsgThreatStillExists.RemoveAt(i);
            }
        }

        for (int i = (_audioMsgPlayerDecision.Count - 1); i >= 0; i--)
        {
            if (_audioMsgPlayerDecision[i] == null)
            {
                _audioMsgPlayerDecision.RemoveAt(i);
            }
        }

        for (int i = (_audioMsgNoThreatExists.Count - 1); i >= 0; i--)
        {
            if (_audioMsgNoThreatExists[i] == null)
            {
                _audioMsgNoThreatExists.RemoveAt(i);
            }
        }

        RookieState(); // sets default rank and difficulty level to "Rookie"
        DetermineDifficultyLevel();
    }

    public void DetermineDifficultyLevel()
    {
        switch (_gameManager.difficultyLevel)
        {
            case 1:
                _rookie = true;
                _spaceCadet = false;
                _spaceCaptain = false;
                _galaxyDefender = false;
                _introRank = _ranks[0];
                _promoRank = _ranks[1];
                _lastLevel = false;
                break;
            case 2:
                _spaceCadet = true;
                _rookie = false;
                _spaceCaptain = false;
                _galaxyDefender = false;
                _introRank = _ranks[1];
                _promoRank = _ranks[2];
                _lastLevel = false;
                break;
            case 3:
                _spaceCaptain = true;
                _rookie = false;
                _spaceCadet = false;
                _galaxyDefender = false;
                _introRank = _ranks[2];
                _promoRank = _ranks[3];
                _lastLevel = false;
                break;
            case 4:
                _galaxyDefender = true;
                _rookie = false;
                _spaceCadet = false;
                _spaceCaptain = false;
                _introRank = _ranks[3];
                _lastLevel = true;
                break;
        }
    }

    public void RookieState()
    {
        _rookie = true;
        _spaceCadet = false;
        _spaceCaptain = false;
        _galaxyDefender = false;
        _introRank = _ranks[0];
        _promoRank = _ranks[1];
        _lastLevel = false;
    }

    public void PlaySequentialSounds()
    {
        if (_lastLevel == false)
        {
            _audioMsgThreatStillExists.Clear();
            GenerateThreatDialogue();
            StartCoroutine(PlayThreatPresent());
        }
        else if (_lastLevel == true)
        {
            _audioMsgNoThreatExists.Clear();
            GenerateNoThreatDialogue();
            StartCoroutine(PlayThreatGone());
            StartCoroutine(QuitToCredits());
        }
    }

    public void GenerateThreatDialogue()
    {
        _audioMsgThreatStillExists.Clear();
        _audioMsgPlayerDecision.Clear();
        _audioMsgNoThreatExists.Clear();

        int randomCongrats = Random.Range(0, _congratulations.Length);
        _congrats = _congratulations[randomCongrats];

        // intro rank audio clip played next

        int randomThreatPresent = Random.Range(0, _threatPresent.Length);
        _threat = _threatPresent[randomThreatPresent];

        int randomMsn = Random.Range(0, _newMsn.Length);
        _msn = _newMsn[randomMsn];

        int randomRisk = Random.Range(0, _mayNotSurvive.Length);
        _risk = _mayNotSurvive[randomRisk];

        int randomPlea = Random.Range(0, _pleaseAccept.Length);
        _plea = _pleaseAccept[randomPlea];

        _audioMsgThreatStillExists.Add(_congrats);
        _audioMsgThreatStillExists.Add(_introRank);
        _audioMsgThreatStillExists.Add(_threat);
        _audioMsgThreatStillExists.Add(_msn);
        _audioMsgThreatStillExists.Add(_risk);
        _audioMsgThreatStillExists.Add(_plea);
    }

    public void GenerateNoThreatDialogue()
    {
        int randomCongrats = Random.Range(0, _congratulations.Length);
        _congrats = _congratulations[randomCongrats];

        // intro rank audio clip played next

        int randomThreatCleared = Random.Range(0, _threatCleared.Length);
        _threat = _threatCleared[randomThreatCleared];

        int randomRTB = Random.Range(0, _returnToBase.Length);
        _rtb = _returnToBase[randomRTB];

        _audioMsgNoThreatExists.Add(_congrats);
        _audioMsgNoThreatExists.Add(_introRank);
        _audioMsgNoThreatExists.Add(_threat);
        _audioMsgNoThreatExists.Add(_rtb);
    }

    public void PlayerAccepts()
    {
        PlayerAcceptsMsn = true;
        GeneratePlayerDecision();
    }

    public void PlayerRefuses()
    {
        PlayerAcceptsMsn = false;
        GeneratePlayerDecision();
    }

    public void GeneratePlayerDecision()
    {
        if (PlayerAcceptsMsn == true)
        {
            int randomDecisionYes = Random.Range(0, _decisionYes.Length);
            _decision = _decisionYes[randomDecisionYes];
            _audioMsgPlayerDecision.Add(_decision);

            _promo = _promoted[0];
            _audioMsgPlayerDecision.Add(_promo);
            _audioMsgPlayerDecision.Add(_promoRank);
            _gameManager.difficultyLevel++;
            _gameManager.TurnOffContinueOptionText();
            DetermineDifficultyLevel();
            StartCoroutine(PlayerDecidesToContinue());
            StartCoroutine(ContinueToNextLevel());
        }
        else if (PlayerAcceptsMsn == false)
        {
            _decision = _decisionNo[0];
            _audioMsgPlayerDecision.Add(_decision);
            _audioMsgPlayerDecision.Add(_introRank);

            int randomRTB = Random.Range(0, _returnToBase.Length);
            _rtb = _returnToBase[randomRTB];
            _audioMsgPlayerDecision.Add(_rtb);

            _gameManager.TurnOffContinueOptionText();
            StartCoroutine(PlayerDecidesToQuit());
        }
    }

    public void PlayDialogueClip(AudioClip dialogueClip)
    {
        if (dialogueClip != null)
        {
            _audioSource.PlayOneShot(dialogueClip);
        }
    }


    /*
    //public void PlayClip(AudioClip soundEffectClip)
    public void PlayClip()

    {
        if (soundEffectClip != null)
        {
            _audioSource.PlayOneShot(soundEffectClip, 1.0f); // 1.0f is the volume component.  Need to attach it to Audio Mixer....
        }
    }
    */

    public void PlayAudioClip(int element)
    {
        _audioSource.PlayOneShot(_audioClips[element], 1.0f);
    }

    public void PlayPowerUpDialogue(int element)
    {
        _audioSource.PlayOneShot(_powerUpAudioClips[element], 1.0f);
    }


    /*
    public void PlayPowerUpDialogue(AudioClip powerUpDialogueClip)
    {
        if (powerUpDialogueClip != null)
        {
            //_audioSource.PlayOneShot(powerUpDialogueClip);
            AudioSource.PlayClipAtPoint(powerUpDialogueClip, new(0, 0, -10), _gameManager.SFXPwrUpVolume);
        }
    }
    */

    public void StopDialogueAudio()
    {
        _audioSource.Stop();
    }

    IEnumerator PlayThreatPresent()
    {
        yield return new WaitForSeconds(3.0f);

        if (_audioMsgThreatStillExists.Count > 0)
        {
            for (int i = 0; i < _audioMsgThreatStillExists.Count; i++)
            {
                PlayDialogueClip(_audioMsgThreatStillExists[i]);

                // wait for the lenght of the clip to finish playing
                yield return new WaitForSeconds(_audioMsgThreatStillExists[i].length + 0.15f);
            }
        }

        MsgDonePlaying = true;
        _gameManager.DisplayContinueOptionText();

        yield return null;
    }

    IEnumerator PlayerDecidesToContinue()
    {
        if (_audioMsgPlayerDecision.Count > 0)
        {
            for (int i = 0; i < _audioMsgPlayerDecision.Count; i++)
            {
                PlayDialogueClip(_audioMsgPlayerDecision[i]);

                // wait for the lenght of the clip to finish playing
                yield return new WaitForSeconds(_audioMsgPlayerDecision[i].length + 0.15f);
            }
        }

        yield return null;
    }

    IEnumerator PlayerDecidesToQuit()
    {
        if (_audioMsgPlayerDecision.Count > 0)
        {
            for (int i = 0; i < _audioMsgPlayerDecision.Count; i++)
            {
                PlayDialogueClip(_audioMsgPlayerDecision[i]);

                // wait for the lenght of the clip to finish playing
                yield return new WaitForSeconds(_audioMsgPlayerDecision[i].length + 0.15f);
            }
        }

        yield return new WaitForSeconds(6.0f);
        RunCreditsDelayCoroutine();
    }

    IEnumerator PlayThreatGone()
    {
        yield return new WaitForSeconds(3.0f);

        if (_audioMsgNoThreatExists.Count > 0)
        {
            for (int i = 0; i < _audioMsgNoThreatExists.Count; i++)
            {
                PlayDialogueClip(_audioMsgNoThreatExists[i]);

                // wait for the lenght of the clip to finish playing
                yield return new WaitForSeconds(_audioMsgNoThreatExists[i].length + 0.15f);
            }
        }
        yield return null;
    }

    IEnumerator ContinueToNextLevel()
    {
        PowerUpAudioIsBossDefeated = false; // resets value
        PlayerAcceptsMsn = false; // resets value
        yield return new WaitForSeconds(7.0f);
        _gameManager.NextLevel();
    }

    IEnumerator QuitToCredits()
    {
        yield return new WaitForSeconds(12.5f);
        RunCreditsDelayCoroutine();
    }

    public void RunCreditsDelayCoroutine()
    {
        _player.FadeOut();
        StartCoroutine(LoadCreditsDelay());
    }

    IEnumerator LoadCreditsDelay() // Loads a new game
    {
        yield return new WaitForEndOfFrame();
        FadeMusicOutToCredits();
    }

    public void FadeMusicOutToCredits() // Quits and fades out to the credits scene
    {
        _gameManager.StopMusic(2.0f);
        StartCoroutine(LoadCredits());
    }

    IEnumerator LoadCredits() // Loads credit scene
    {
        yield return new WaitForSeconds(2.0f);
        _gameManager.QuitGame();
    }
}
