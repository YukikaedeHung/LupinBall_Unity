using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTestIPhotonViewRegister : MonoBehaviour, ITestPhotonViewCallback
{

    private void Awake()
    {
        PhotonView.Get(this).AddCallbackTarget(this);

        PhotonView pv = PhotonView.Get(this);
        pv.AddCallback<ITestPhotonViewCallback>(this);
    }

    public void Test()
    {
        Debug.Log("MyTestIPhotonViewRegister - Test()");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Test();
        }
    }
}
