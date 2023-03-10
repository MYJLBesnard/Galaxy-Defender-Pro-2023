using UnityEngine;

public class TractorBeamCollectPwrUps : MonoBehaviour
{
    private GameObject _player;
    public static bool IsPowrUpTractorBeamActive = false;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPowrUpTractorBeamActive && _player != null)
        {
            MoveTowardsPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        transform.position = Vector3.Lerp(a: this.transform.position, b: _player.transform.position, t: 2.5f * Time.deltaTime);
    }
}
