using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Prefab的CallBack必須掛載在Prefab上
/// </summary>
public class MyIPunInstantiateMagicCallback : MonoBehaviour, IPunInstantiateMagicCallback
{
    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        string str = "";
        if (info.photonView.InstantiationData.Length > 0)
        {
            for (int i=0; i < info.photonView.InstantiationData.Length; i++) {
                str += string.Format("Index:{0} - Group:{1}, InstantiationData:{2}", i, info.photonView.Group, info.photonView.InstantiationData[i]) + "\n";
            }
        }
        //Debug.Log(string.Format("MyPhotonUnityNetworkingCallBack-OnPhotonInstantiate(info:{0}, InstantiationData:{1})", info, str));
    }
}
