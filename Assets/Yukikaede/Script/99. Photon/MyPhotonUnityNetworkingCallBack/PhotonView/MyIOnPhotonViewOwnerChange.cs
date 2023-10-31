using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Tim;

public class MyIOnPhotonViewOwnerChange : MonoBehaviour, IOnPhotonViewOwnerChange
{
    /* https://doc.photonengine.com/zh-cn/pun/current/getting-started/dotnet-callbacks */
    private void Awake()
    {
        PhotonView.Get(this).AddCallbackTarget(this);
    }

    public virtual void OnOwnerChange(Player newOwner, Player previousOwner)
    {
        Debug.Log(string.Format("OnOwnerChange - Player newOwner:{0}, Player previousOwner:{1} ", newOwner, previousOwner));
    }
}
