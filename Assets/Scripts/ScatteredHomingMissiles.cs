using UnityEngine;

public class ScatteredHomingMissiles : MonoBehaviour
{
    [SerializeField] private AudioManager _audioManager; 
   // [SerializeField] private AudioClip _powerUpAudioClip = null;
    [SerializeField] private GameObject _explosionPrefab;

    private void Start()
    {
        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        if (_audioManager == null)
        {
            Debug.Log("Dialogue Player is NULL.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "EnemyLaser")
        {
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
        }

        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                if (_audioManager.PowerUpAudioIsBossDefeated == false)
                {
                    _audioManager.PlayAudioClip(8);
                }

                player.PlayerHomingMissiles(5);
                Destroy(this.gameObject);
            }
        }
    }
}