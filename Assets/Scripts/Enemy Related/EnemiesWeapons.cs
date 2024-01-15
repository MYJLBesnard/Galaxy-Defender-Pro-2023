using System.Collections;
using UnityEngine;

using static UnityEngine.UI.ScrollRect;

public class EnemiesWeapons : MonoBehaviour
{
//    private Player _player;
//    private Animator _animEnemyDestroyed;
    private SpawnManager _spawnManager;
    private GameManager _gameManager;
    //private AudioSource _audioSource;

    //[SerializeField] private EnemiesCoreMovement _enemiesCoreMovement;

   // [SerializeField] private GameObject _enemyPrefab;

    [SerializeField] private AudioClip _explosionSoundEffect;
    [SerializeField] private AudioClip _enemyLaserShotAudioClip;

    [SerializeField] private GameObject _laserNode1;
    [SerializeField] private GameObject _enemyBasicLaser;
    [SerializeField] private GameObject _enemyDoubleShotLaserPrefab;
    [SerializeField] private GameObject _enemyArcLaserPrefab;

    //[SerializeField] private int _enemyType;
    [SerializeField] private bool _stopUpdating = false;

    [SerializeField] private float _wpnReadyToFire;
    [SerializeField] private float _enemyRateOfFire = 1.0f;

   // [SerializeField] private bool _enemyDestroyed = false;
   // [SerializeField] public bool noSound = false;

    [SerializeField] public bool isMineLayerArmed = true;
    [SerializeField] public float releasePoint;
    [SerializeField] public GameObject enemyRoamingSpaceMines;

    enum TypeWeapon { BasicSingle, DoubleShot, ArcLaser, Mines, BossWeapons };
    [Header("Types of Weapon")]
    [SerializeField] TypeWeapon weaponType = new TypeWeapon();


    // Start is called before the first frame update
    void Start()
    {
    //    _player = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

        //_audioSource = GetComponent<AudioSource>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        releasePoint = Random.Range(-12.0f, 12.0f);

    }

    // Update is called once per frame
    void Update()
    {
     //   if(_enemyDestroyed == false)
     //   {
            switch (weaponType)
            {
                case TypeWeapon.BasicSingle:
                    {
                        FireEnemyBasicLaser();
                    }

                    break;

                case TypeWeapon.DoubleShot:
                    {

                    }

                    break;

                case TypeWeapon.ArcLaser:
                    {

                    }

                    break;

                case TypeWeapon.Mines:
                    {
                    DeployMines();
                    }

                    break;

                case TypeWeapon.BossWeapons:
                    {

                    }

                    break;

                default:
                    break;
            }
     //   }       
    }



    void FireEnemyBasicLaser()
    {
        //if (Time.time > _wpnReadyToFire && _enemyDestroyed == false)
        if (Time.time > _wpnReadyToFire)

        {
            _enemyRateOfFire = _gameManager.currentEnemyRateOfFire;
            _wpnReadyToFire = Time.time + (_enemyRateOfFire * Random.Range(1f, 3f));

            if (_gameManager.currentEnemyRateOfFire != 0)
            {

                Vector3 laserPx = _laserNode1.transform.position;
                //GameObject playerLaser = Instantiate(_playerDoubleShotLaserPrefab, laserPx, transform.rotation);

                //Vector3 laserPx = new(transform.position.x, transform.position.y - 0.35f, transform.position.z);
                //Vector3 laserPx = new(transform.position.x, transform.position.y, transform.position.z);

                GameObject enemyLaser = Instantiate(_enemyBasicLaser, laserPx, transform.rotation);
                enemyLaser.transform.parent = _spawnManager.EnemyLaserContainer.transform;
            }
        }
    }

    
    public IEnumerator LaserBurst()
    {
        if (_stopUpdating == false)
        {
            Debug.Log("Running EnemiesWeapons: LaserBurst coroutine");

            Vector3 position = new Vector3(transform.position.x, transform.position.y + 0.25f, transform.position.z);
            GameObject enemyLaser = Instantiate(_enemyDoubleShotLaserPrefab, position, Quaternion.identity);
            enemyLaser.transform.parent = _spawnManager.EnemyLaserContainer.transform;

            
            yield return new WaitForSeconds(0.1f);
        }
    }
    

    public void DeployMines()
    {
        float minesSpawnPoint = 0.88f;
        int totalMines = 0;
        int mineQty = 3 + _gameManager.difficultyLevel; // D1 = 4
                                                        // D2 = 5
                                                        // D3 = 6
                                                        // D4 = 7

        while (totalMines <= mineQty)
        {
            if (_gameManager.enemyMineLayerDirectionRight == true)
            {
                Instantiate(enemyRoamingSpaceMines, new Vector3(transform.position.x - minesSpawnPoint,
                    transform.position.y, transform.position.z), Quaternion.identity);
            }
            else
            {
                Instantiate(enemyRoamingSpaceMines, new Vector3(transform.position.x + minesSpawnPoint,
                    transform.position.y, transform.position.z), Quaternion.identity);
            }

            totalMines++;
        }
    }

}
