using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBG : MonoBehaviour
{
    // Adaptation from Alex Somerville

    [SerializeField] private float _speed = 0.1f;
    private Renderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null) Debug.LogError(message: "Renderer: ScrollingBG is NULL.");
    }

    // Update is called once per frame
    void Update()
    {
        _renderer.material.mainTextureOffset = new Vector2(x: 0, y: Time.time * _speed);
    }
}
