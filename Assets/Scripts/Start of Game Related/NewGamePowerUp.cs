using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGamePowerUp : MonoBehaviour
{
    [SerializeField] private EndOfLevelDialogue _endOfLevelDialogue;
    [SerializeField] private AudioClip _powerUpAudioClip = null;
    [SerializeField] private bool _isMissiles = false;
    [SerializeField] private bool _isAmmo = false;

    void Start()
    {
        _endOfLevelDialogue = GameObject.Find("DialoguePlayer").GetComponent<EndOfLevelDialogue>();

        if (_endOfLevelDialogue == null)
        {
            Debug.LogError("Dialogue Player is NULL.");
        }
    }

 private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {

                if (_endOfLevelDialogue.PowerUpAudioIsBossDefeated == false)
                {
                    _endOfLevelDialogue.PlayPowerUpDialogue(_powerUpAudioClip);
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
