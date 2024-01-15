using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// An object that will move towards an object marked with the tag 'targetTag'. 

public class HomingMissile : MonoBehaviour
{
    /// The base movement speed of the missile, in units per second. 
   // [SerializeField] private float speed = 15;
    [SerializeField] private float rotationSpeed = 1000;
    [SerializeField] private float focusDistance = 5;

    private Transform target;
    private bool isLookingAtObject = true;

    [SerializeField] private string targetTag;

    private string enterTagPls = "Please enter the tag of the object you'd like to target, in the field 'Target Tag' in the Inspector.";

    private Rigidbody _rb;
    public GameObject homingMissileExplosionEffect;
    private GameObject _closestEnemy;
    [SerializeField] private float _missileSpeed = 12.0f;
    [SerializeField] private float _rotateSpeed = 350f;
    [SerializeField] private GameObject _selfdestructExplosion;

    void Start()
    {
        //_rb = GetComponent<Rigidbody2D>();
        _rb = GetComponent<Rigidbody>();
 
        if (targetTag == "")
        {
            Debug.LogError(enterTagPls);
            return;
        }

        target = GameObject.FindGameObjectWithTag(targetTag).transform;
    }

    private void Update()
    {
        if (targetTag == "")
        {
            Debug.LogError(enterTagPls);
            return;
        }

        if (_closestEnemy == null)
        {
            _closestEnemy = FindClosestEnemy();
        }
        if (_closestEnemy != null)
        {
            MoveTowardsEnemy();
        }
        else
        {
            transform.Translate((_missileSpeed / 2) * Time.deltaTime * Vector3.up);
        }

        if (transform.position.y > 13)
        {
            Destroy(gameObject);
        }

        StartCoroutine(MissileSelfDestruct());
    }

    IEnumerator MissileSelfDestruct()
    {
        yield return new WaitForSeconds(2.5f); // sets the missile self-destructs seconds after launch
        Instantiate(_selfdestructExplosion, transform.position, Quaternion.identity);

        //Destroy(this.gameObject, 0.025f); // using this time delay on the destroy gives a neat little effect
        Destroy(this.gameObject);

    }

    private GameObject FindClosestEnemy()
    {
        try // try the following block of code
        {
            GameObject[] enemies; // create an array names "enemies"
            enemies = GameObject.FindGameObjectsWithTag("Enemy"); // find game object tagged as Enemy and store into array

            GameObject closest = null; // set "closest" to null (default)
            float distance = Mathf.Infinity; // set the initial value of "distance" to infinity
            Vector3 position = transform.position;  // position of the missile

            foreach (GameObject enemy in enemies) // run through the array and compare each game object by storing it into "enemy"
            {
                Vector3 diff = enemy.transform.position - position; // difference between the enemy position and the missile position
                float curDistance = diff.sqrMagnitude; // stores into current distance the squared length measured (diff)
                if (curDistance < distance) // if the current distance between the enemy position and the missile position is
                                            // less than the float value stored in "distance", then do the following:...
                {
                    closest = enemy; // stores the enemy from the array into the variable "closest"
                    distance = curDistance; // updates the value of distance with the current distance
                }
            }
            return closest; // return the closest enemy position and stores it in "_closestEnemy"
        }
        catch // if an error occurs in the "try" block above, return null.
        {
            return null;
        }
    }

    private void MoveTowardsEnemy()
    {
        /*
        Vector2 direction = (Vector2)_closestEnemy.transform.position - _rb.position;
        //Vector3 direction = (Vector3)_closestEnemy.transform.position - _rb.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        _rb.angularVelocity = -rotateAmount * _rotateSpeed;
        _rb.velocity = transform.up * _missileSpeed;
        */

        Vector3 targetDirection = _closestEnemy.transform.position - transform.position;
        targetDirection.Normalize();
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0F);

        transform.Translate(Vector3.forward * Time.deltaTime * _missileSpeed, Space.Self);

        if (Vector3.Distance(transform.position, target.position) < focusDistance)
        {
            isLookingAtObject = false;
        }

        if (isLookingAtObject)
        {
            transform.rotation = Quaternion.LookRotation(newDirection);
        }

        /*
        Quaternion rotation = Quaternion.LookRotation(direction);

        //transform.rotation = Quaternion.Euler(Vector3.right * 0f);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _rotateSpeed * Time.deltaTime);

        //transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
        */

        //_rb.velocity = transform.up * _missileSpeed;

    }
}
