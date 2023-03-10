using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = -50.0f;

    void Update()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
    }
}
