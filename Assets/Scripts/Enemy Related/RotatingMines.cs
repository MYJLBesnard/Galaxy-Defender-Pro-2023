using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingMines : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 0f;

    private void Start()
    {
        _rotationSpeed = Random.Range(-75f, 75f);
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
    }
}
