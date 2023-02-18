using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrustersScale : MonoBehaviour
{
    [SerializeField] private Player _player;
    //[SerializeField] private bool _canThrustersScale = false;

    void Start()
    {
        transform.localScale = new Vector3(0.1f, 0.63f, 0.5f);

        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.LogError("The ThrustersScale : Player script is null.");
        }
    }

    void Update()
    {
        if (_player.canPlayerUseThrusters == true)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.localScale = new Vector3(0.3f, 0.63f, 0.5f);
            }
            else
            {
                transform.localScale = new Vector3(0.1f, 0.63f, 0.5f);
            }
        }

        if (_player.canPlayerUseThrusters == false)
        {
            transform.localScale = new Vector3(0.1f, 0.63f, 0.5f);
        }

    }
}
