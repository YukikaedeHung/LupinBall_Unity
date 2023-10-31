using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTestIPhotonViewCallback : MonoBehaviour, ITestPhotonViewCallback
{
    private void Awake()
    {
        PhotonView.Get(this).AddCallbackTarget(this);
    }

    public void Test()
    {
        Debug.Log("MyTestIPhotonViewCallback : ITestPhotonViewCallback");
    }
}
