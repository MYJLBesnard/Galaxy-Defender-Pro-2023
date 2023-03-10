using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
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

    [SerializeField] private AudioClip _speedBoostCollected;
    [SerializeField] private AudioClip _multiShotCollected;
    [SerializeField] private AudioClip _ammunitionCollected;
    [SerializeField] private AudioClip _homingMissilesCollected;
    [SerializeField] private AudioClip _lateralLaserCollected;

    // Singleton Accessor - Only one AudioManager should ever exist
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
