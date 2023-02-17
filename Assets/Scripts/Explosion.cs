using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _explosionAudioClip;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.LogError("The Enemy Basic AudioSource is NULL.");
        }
        else
        {
            _audioSource.clip = _explosionAudioClip;
        }

        PlayClip(_explosionAudioClip);
        Destroy(this.gameObject, 3.5f);
    }

    public void PlayClip(AudioClip soundEffectClip)
    {
        if (soundEffectClip != null)
        {
            _audioSource.PlayOneShot(soundEffectClip);
        }
    }


}
