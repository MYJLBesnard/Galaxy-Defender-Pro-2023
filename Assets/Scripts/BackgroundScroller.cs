using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private float _speed = 0.01f;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private bool _isParticle = false;
    [SerializeField] private float _test = 0f;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        _renderer = GetComponent<MeshRenderer>();

        if (_player == null)
        {
            Debug.Log("The Player script is null.");
        }
    }

    void Update()
    {
        //_renderer.material.mainTextureOffset = new Vector2(0f, Time.time * _speed); 
        //_renderer.material.mainTextureOffset = new Vector2(Time.time * _speed, 0f);

        //transform.rotation = _player.transform.rotation;

        BackgroundMovement();
    }

    void BackgroundMovement()
    {
        float playerHorizontalInput = Input.GetAxis("Horizontal");
        float playerVerticalInput = Input.GetAxis("Vertical");

        /*
        if (Input.GetKey("z")) // Player rotate CCW
        {
            _playerNoseThrusterLeft.SetActive(true);
            Vector3 playerRotateLeft = new Vector3(0, 0, 15.0f);
            transform.Rotate(_playerRotateSpeed * Time.deltaTime * playerRotateLeft);
        }
        else
        {
            _playerNoseThrusterLeft.SetActive(false);
        }

        if (Input.GetKey("c")) // Player rotate CW
        {
            _playerNoseThrusterRight.SetActive(true);
            Vector3 playerRotateRight = new Vector3(0, 0, -15.0f);
            transform.Rotate(_playerRotateSpeed * Time.deltaTime * playerRotateRight);

        }
        else
        {
            _playerNoseThrusterRight.SetActive(false);

        }
        */

        Vector2 playerMovement = new Vector2(playerHorizontalInput, playerVerticalInput);
        //transform.Translate(_speed * Time.time * playerMovement);


        _renderer.material.mainTextureOffset = new Vector2(_test, Time.time * _speed);

        if(_isParticle == true)
        {
            transform.rotation = _player.transform.rotation;

        }

        //_renderer.material.mainTextureOffset = new Vector2(playerHorizontalInput, Time.time * _speed);
        //_renderer.material.mainTextureOffset = new Vector2(Time.time * _speed, playerVerticalInput);


    }
}
