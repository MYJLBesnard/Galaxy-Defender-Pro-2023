using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGamePowerUp : MonoBehaviour
{
    [SerializeField] private AudioManager _audioManager;
    //[SerializeField] private AudioClip _powerUpAudioClip = null;
    [SerializeField] private int _powerUpID;
    [SerializeField] private bool _isMissiles = false;
    [SerializeField] private bool _isAmmo = false;


    //private Rigidbody _rigidbody;

    void Start()
    {
        //_rigidbody = GetComponent<Rigidbody>();

        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        if (_audioManager == null)
        {
            Debug.LogError("Dialogue Player is NULL.");
        }
    }

 private void OnTriggerEnter2D(Collider2D other)
         //private void OnTriggerEnter(Collider other)

    {
        if (other.tag == "Player")
        {
            Debug.Log("Player collecting start game PU.");
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {

                if (_audioManager.PowerUpAudioIsBossDefeated == false)
                {
                    _audioManager.PlayPowerUpDialogue(_powerUpID);
                }

                player.newGamePowerUpCollected++;

                if (player.newGamePowerUpCollected == 2)
                {
                    player.LaserIsWpnsFree();
                }

                if (_isMissiles == true)
                {
                    player.PlayerHomingMissiles(5);
                }

                if (_isAmmo == true)
                {
                    player.PlayerRegularAmmo(5);
                }

                Destroy(this.gameObject);
            }
        }
    }
}
