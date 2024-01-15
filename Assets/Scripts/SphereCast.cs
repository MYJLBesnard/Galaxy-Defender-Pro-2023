using UnityEngine;

public class SphereCast : MonoBehaviour
{
    public float radius;
    public float maxDistance;
    public LayerMask layerMask;
    RaycastHit hit;



    // Update is called once per frame
    void Update()
    {
      Cast();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position - transform.up * maxDistance, radius);
    }
    void Cast()
    {
        if(Physics.SphereCast(transform.position, radius, -transform.up, out hit, maxDistance, ~layerMask))
        {
            Debug.Log(hit.collider.gameObject);

        }

    }
}