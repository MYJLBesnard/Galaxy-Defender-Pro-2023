using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionBoxManager : MonoBehaviour
{
    //[SerializeField] private EnemyCore _enemyCore;
    public GameObject forwardDetectionBox2;
    public GameObject rearDetectionBox2;

    // Start is called before the first frame update
    void Start()
    {
        //_enemyMovement = GameObject.Find("EnemyMovement").GetComponent<EnemyMovement>();
        //_enemyCore = GameObject.Find("EnemyTemplate").GetComponent<EnemyCore>();
        //_enemyCore = GameObject.Find("TestEnemyUno").GetComponent<EnemyCore>();
      //  _enemyCore = GetComponent<EnemyCore>();


    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = _enemyCore.transform.position;
        //transform.localScale = new Vector3(0.5f, 3.5f, 0.5f);
    }

}
