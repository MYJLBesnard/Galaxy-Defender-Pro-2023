using UnityEngine;

public class BoxTrigger : MonoBehaviour
{
    void Update()
    {
        //transform.position = _enemyCore.transform.position;
        //transform.localScale = new Vector3(0.5f, 3.5f, 0.5f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("BoxTrigger : Player detected...");
        }

        if (other.gameObject.CompareTag("LaserPlayer"))
        {
            Debug.Log("BoxTrigger : Player Laser detected...");
        }
    }
}
