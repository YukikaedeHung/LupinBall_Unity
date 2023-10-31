using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class XDScript : MonoBehaviour
{
    public float XD;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonView.Get(this).IsMine && Input.GetKey(KeyCode.UpArrow))
        {
            XD+=5;
        }
    }
}
