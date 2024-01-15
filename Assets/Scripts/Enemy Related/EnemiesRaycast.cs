using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesRaycast : MonoBehaviour
{
    private GameManager _gameManager;

    [SerializeField] private EnemiesCoreMovement _enemiesCoreMovement;
    [SerializeField] private EnemiesWeapons _enemiesWeapons;

    [SerializeField] private bool _drawRaycast = false;


    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _enemiesCoreMovement = GameObject.Find("EnemiesCoreMovement").GetComponent<EnemiesCoreMovement>();
        _enemiesWeapons = GameObject.Find("EnemiesWeapons").GetComponent<EnemiesWeapons>();



    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _gameManager.currentEnemySensorRange);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Raycast detected Player");
                if (_enemiesCoreMovement.isRamingEnemy == true) StartCoroutine(_enemiesCoreMovement.SpeedBurst());
                else { StartCoroutine(_enemiesWeapons.LaserBurst()); }
            }

            if (hit.collider.CompareTag("PlayerPowerUps") || hit.collider.CompareTag("PowerUpsWeapons"))
            {
                Debug.Log("Raycast detected PowerUp");
                StartCoroutine(_enemiesWeapons.LaserBurst());
            }

            if (hit.collider.CompareTag("LaserPlayer"))
            {
                Debug.Log("Raycast detected Player Laser");
                StartCoroutine(_enemiesCoreMovement.DodgePlayerLaser());
            }



        }

        if (_drawRaycast == true)
        {
            Debug.DrawRay(transform.position, Vector2.down * _gameManager.currentEnemySensorRange, Color.red);

        }


    }




}
