using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Tim;

public class MyIOnPhotonViewControllerChange : MonoBehaviour, IOnPhotonViewControllerChange
{
    /* https://doc.photonengine.com/zh-cn/pun/current/getting-started/dotnet-callbacks */
    private void Awake()
    {
        PhotonView.Get(this).AddCallbackTarget(this);
    }

    public virtual void OnControllerChange(Player newController, Player previousController)
    {
        Debug.Log(string.Format("OnControllerChange - Player newController:{0}, Player previousController:{1} ", newController, previousController));
    }
}
