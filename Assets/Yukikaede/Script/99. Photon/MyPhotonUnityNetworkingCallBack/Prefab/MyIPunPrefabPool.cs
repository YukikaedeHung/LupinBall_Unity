using Tim;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自建Instantiate與Destroy執行內容並更新至PhotonNetwork.PrefabPool(目前無法使用)
/// </summary>
public class MyIPunPrefabPool : MonoBehaviour, IPunPrefabPool
{
    public GameObject obj;

    //private void Awake()
    //{
    //    MyPhotonNetwork.AddCallbackTarget(this);
    //    PhotonView.Get(this).AddCallbackTarget(this);
    //}

    public virtual void Destroy(GameObject gameObject)
    {
        Debug.Log(string.Format("MyPhotonUnityNetworkingCallBack-Destroy(gameObject:{0})", gameObject));
    }

    public virtual GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        Debug.Log(string.Format("MyPhotonUnityNetworkingCallBack-Instantiate(prefabId:{0}, position:{1}, rotation:{2})", prefabId, position, rotation));

        //InstantiateParameters netParams = new InstantiateParameters(prefabId, position, rotation, 0, null, 0, null, null, 0);
        //return MyPhotonNetwork.NetworkInstantiate(netParams, false);//??? .............................
        return null;
    }
}
