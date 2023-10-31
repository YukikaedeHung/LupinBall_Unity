using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tim.Game;

public class WallScript : MonoBehaviour
{
    private Collider colWall;
    private Collider colBullet;

    private void Awake()
    {
        colWall = GetComponent<Collider>();
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {

        }
        if (other.tag == "Bullet")
        {
            Collider colBullet = other.GetComponent<Collider>();
            Physics.IgnoreCollision(colWall, colBullet);
        }
        else
        {
            colWall.isTrigger = true;
        }
    }
}
