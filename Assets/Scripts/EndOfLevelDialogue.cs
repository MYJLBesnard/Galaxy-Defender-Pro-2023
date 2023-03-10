using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfLevelDialogue : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Player _player;

    private AudioSource _audioSource;
    public bool PlayOnAwake = false;
    public bool Loop = false;

    public bool LastLevel = false;
    public bool PlayerAcceptsMsn = true;
    public bool Rookie = true;
    public bool SpaceCadet = false;
    public bool SpaceCaptain = false;
    public bool GalaxyDefender = false;
    public bool MsgDonePlaying = false;
    public bool PowerUpAudioIsBossDefeated = false;

    public AudioClip[] Congratulations;
    public AudioClip[] ThreatPresent;
    public AudioClip[] ThreatCleared;
    public AudioClip[] NewMsn;
    public AudioClip[] MayNotSurvive;
    public AudioClip[] PleaseAccept;
    public AudioClip[] DecisionYes;
    public AudioClip[] DecisionNo;
    public AudioClip[] Ranks;
    public AudioClip[] ReturnToBase;
    public AudioClip[] Promoted;

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

    public List<AudioClip> AudioMsgThreatStillExists = new();
    public List<AudioClip> AudioMsgPlayerDecision = new();
    public List<AudioClip> AudioMsgNoThreatExists = new();

    //public AudioClip[] PowerUpAudioClips;
    //[SerializeField] private AudioClip _powerUpAudioClip = null;

    // Start is called before the first frame update
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

        List<AudioClip> AudioMsgThreatStillExists = new();
        List<AudioClip> AudioMsgPlayerDecision = new();
        List<AudioClip> AudioMsgNoThreatExists = new();

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
        for (int i = (AudioMsgThreatStillExists.Count - 1); i >= 0; i--)
        {
            if (AudioMsgThreatStillExists[i] == null)
            {
                AudioMsgThreatStillExists.RemoveAt(i);
            }
        }

        for (int i = (AudioMsgPlayerDecision.Count - 1); i >= 0; i--)
        {
            if (AudioMsgPlayerDecision[i] == null)
            {
                AudioMsgPlayerDecision.RemoveAt(i);
            }
        }

        for (int i = (AudioMsgNoThreatExists.Count - 1); i >= 0; i--)
        {
            if (AudioMsgNoThreatExists[i] == null)
            {
                AudioMsgNoThreatExists.RemoveAt(i);
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
                Rookie = true;
                SpaceCadet = false;
                SpaceCaptain = false;
                GalaxyDefender = false;
                _introRank = Ranks[0];
                _promoRank = Ranks[1];
                LastLevel = false;
                break;
            case 2:
                SpaceCadet = true;
                Rookie = false;
                SpaceCaptain = false;
                GalaxyDefender = false;
                _introRank = Ranks[1];
                _promoRank = Ranks[2];
                LastLevel = false;
                break;
            case 3:
                SpaceCaptain = true;
                Rookie = false;
                SpaceCadet = false;
                GalaxyDefender = false;
                _introRank = Ranks[2];
                _promoRank = Ranks[3];
                LastLevel = false;
                break;
            case 4:
                GalaxyDefender = true;
                Rookie = false;
                SpaceCadet = false;
                SpaceCaptain = false;
                _introRank = Ranks[3];
                LastLevel = true;
                break;
        }
    }

    public void RookieState()
    {
        Rookie = true;
        SpaceCadet = false;
        SpaceCaptain = false;
        GalaxyDefender = false;
        _introRank = Ranks[0];
        _promoRank = Ranks[1];
        LastLevel = false;
    }

    public void PlaySequentialSounds()
    {
        if (LastLevel == false)
        {
            AudioMsgThreatStillExists.Clear();
            GenerateThreatDialogue();
            StartCoroutine(PlayThreatPresent());
        }
        else if (LastLevel == true)
        {
            AudioMsgNoThreatExists.Clear();
            GenerateNoThreatDialogue();
            StartCoroutine(PlayThreatGone());
            StartCoroutine(QuitToCredits());
        }
    }

    public void GenerateThreatDialogue()
    {
        AudioMsgThreatStillExists.Clear();
        AudioMsgPlayerDecision.Clear();
        AudioMsgNoThreatExists.Clear();

        int randomCongrats = Random.Range(0, Congratulations.Length);
        _congrats = Congratulations[randomCongrats];

        // intro rank audio clip played next

        int randomThreatPresent = Random.Range(0, ThreatPresent.Length);
        _threat = ThreatPresent[randomThreatPresent];

        int randomMsn = Random.Range(0, NewMsn.Length);
        _msn = NewMsn[randomMsn];

        int randomRisk = Random.Range(0, MayNotSurvive.Length);
        _risk = MayNotSurvive[randomRisk];

        int randomPlea = Random.Range(0, PleaseAccept.Length);
        _plea = PleaseAccept[randomPlea];

        AudioMsgThreatStillExists.Add(_congrats);
        AudioMsgThreatStillExists.Add(_introRank);
        AudioMsgThreatStillExists.Add(_threat);
        AudioMsgThreatStillExists.Add(_msn);
        AudioMsgThreatStillExists.Add(_risk);
        AudioMsgThreatStillExists.Add(_plea);
    }

    public void GenerateNoThreatDialogue()
    {
        int randomCongrats = Random.Range(0, Congratulations.Length);
        _congrats = Congratulations[randomCongrats];

        // intro rank audio clip played next

        int randomThreatCleared = Random.Range(0, ThreatCleared.Length);
        _threat = ThreatCleared[randomThreatCleared];

        int randomRTB = Random.Range(0, ReturnToBase.Length);
        _rtb = ReturnToBase[randomRTB];

        AudioMsgNoThreatExists.Add(_congrats);
        AudioMsgNoThreatExists.Add(_introRank);
        AudioMsgNoThreatExists.Add(_threat);
        AudioMsgNoThreatExists.Add(_rtb);
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
            int randomDecisionYes = Random.Range(0, DecisionYes.Length);
            _decision = DecisionYes[randomDecisionYes];
            AudioMsgPlayerDecision.Add(_decision);

            _promo = Promoted[0];
            AudioMsgPlayerDecision.Add(_promo);
            AudioMsgPlayerDecision.Add(_promoRank);
            _gameManager.difficultyLevel++;
            _gameManager.TurnOffContinueOptionText();
            DetermineDifficultyLevel();
            StartCoroutine(PlayerGivesDecision());
            StartCoroutine(ContinueToNextLevel());
        }
        else if (PlayerAcceptsMsn == false)
        {
            _decision = DecisionNo[0];
            AudioMsgPlayerDecision.Add(_decision);
            AudioMsgPlayerDecision.Add(_introRank);

            int randomRTB = Random.Range(0, ReturnToBase.Length);
            _rtb = ReturnToBase[randomRTB];
            AudioMsgPlayerDecision.Add(_rtb);

            _gameManager.TurnOffContinueOptionText();
            //StartCoroutine(PlayerGivesDecision());    // original script is also commented out....
            StartCoroutine(PlayerChicken());
        }
    }

    public void PlayDialogueClip(AudioClip dialogueClip)
    {
        if (dialogueClip != null)
        {
            _audioSource.PlayOneShot(dialogueClip);
        }
    }


    public void PlayPowerUpDialogue(AudioClip powerUpDialogueClip)
    {
        if (powerUpDialogueClip != null)
        {
            //_audioSource.PlayOneShot(powerUpDialogueClip);
            AudioSource.PlayClipAtPoint(powerUpDialogueClip, new(0, 0, -10), _gameManager.SFXPwrUpVolume);


        }
    }


    public void StopDialogueAudio()
    {
        _audioSource.Stop();
    }

    IEnumerator PlayThreatPresent()
    {
        yield return new WaitForSeconds(3.0f);

        if (AudioMsgThreatStillExists.Count > 0)
        {
            for (int i = 0; i < AudioMsgThreatStillExists.Count; i++)
            {
                PlayDialogueClip(AudioMsgThreatStillExists[i]);

                // wait for the lenght of the clip to finish playing
                yield return new WaitForSeconds(AudioMsgThreatStillExists[i].length + 0.15f);
            }
        }

        MsgDonePlaying = true;
        _gameManager.DisplayContinueOptionText();

        yield return null;
    }

    IEnumerator PlayerGivesDecision()
    {
        if (AudioMsgPlayerDecision.Count > 0)
        {
            for (int i = 0; i < AudioMsgPlayerDecision.Count; i++)
            {
                PlayDialogueClip(AudioMsgPlayerDecision[i]);

                // wait for the lenght of the clip to finish playing
                yield return new WaitForSeconds(AudioMsgPlayerDecision[i].length + 0.15f);
            }
        }

        yield return null;
    }

    IEnumerator PlayThreatGone()
    {
        yield return new WaitForSeconds(3.0f);

        if (AudioMsgNoThreatExists.Count > 0)
        {
            for (int i = 0; i < AudioMsgNoThreatExists.Count; i++)
            {
                PlayDialogueClip(AudioMsgNoThreatExists[i]);

                // wait for the lenght of the clip to finish playing
                yield return new WaitForSeconds(AudioMsgNoThreatExists[i].length + 0.15f);
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

    IEnumerator PlayerChicken()
    {
        if (AudioMsgPlayerDecision.Count > 0)
        {
            for (int i = 0; i < AudioMsgPlayerDecision.Count; i++)
            {
                PlayDialogueClip(AudioMsgPlayerDecision[i]);

                // wait for the lenght of the clip to finish playing
                yield return new WaitForSeconds(AudioMsgPlayerDecision[i].length + 0.15f);
            }
        }

        yield return new WaitForSeconds(6.0f);
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
