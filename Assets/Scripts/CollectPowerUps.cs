using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectPowerUps : MonoBehaviour
{
    private GameObject _player;
    public static bool isPwrUpTractorBeamActive = false;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (isPwrUpTractorBeamActive && _player != null)
        {
            MoveTowardsPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        transform.position = Vector3.Lerp(a: this.transform.position, b: _player.transform.position, t: 2.5f * Time.deltaTime);
        
    }
}
