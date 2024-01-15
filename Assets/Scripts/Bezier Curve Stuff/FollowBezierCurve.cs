using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBezierCurve : MonoBehaviour
{
    [SerializeField] private Transform[] _routes;
    private int _routeToGo;
    private float _tParam;
    private Vector3 _objectPosition;
    [SerializeField] private float _speedModifier;
    private bool _coroutineAllowed;
    private float _angle;

    [SerializeField] private Transform _player;


    //private Transform position;

    private void Start()
    {
        _routeToGo = 0;
        _tParam = 0f;
        _speedModifier = 0.5f;
        _coroutineAllowed = true;
    }

    private void Update()
    {
        if (_coroutineAllowed)
        {
            StartCoroutine(FollowTheRoute(_routeToGo));
        }
    }

    private IEnumerator FollowTheRoute(int routeNumber)
    {
        _coroutineAllowed = false;

        Vector3 p0 = _routes[routeNumber].GetChild(0).position;
        Vector3 p1 = _routes[routeNumber].GetChild(1).position;
        Vector3 p2 = _routes[routeNumber].GetChild(2).position;
        //Vector3 p3 = _routes[routeNumber].GetChild(3).position;
        Vector3 p3 = _player.position;

        while (_tParam < 1)
        {
            _tParam += Time.deltaTime * _speedModifier;

            _objectPosition = Mathf.Pow(1 - _tParam, 3) * p0 + 3 * Mathf.Pow(1 - _tParam, 2) * _tParam * p1 + 3 * (1 - _tParam) * Mathf.Pow(_tParam, 2) * p2 + Mathf.Pow(_tParam, 3) * p3;


            Vector3 relative = transform.InverseTransformPoint(_objectPosition);
            _angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
            transform.Rotate(0, 0, -_angle - 180);
            transform.position = _objectPosition;



            yield return new WaitForEndOfFrame();
        }

        _tParam = 0f;

        _routeToGo += 1;

        if (_routeToGo > _routes.Length -1) 
            _routeToGo = 0;

        _coroutineAllowed = true;
    }
}
