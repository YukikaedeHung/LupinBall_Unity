using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Tim;

public class MyIOnPhotonViewPreNetDestroy : MonoBehaviour, IOnPhotonViewPreNetDestroy
{
    /* https://doc.photonengine.com/zh-cn/pun/current/getting-started/dotnet-callbacks */
    private void Awake()
    {
        PhotonView.Get(this).AddCallbackTarget(this);
    }

    public virtual void OnPreNetDestroy(PhotonView rootView)
    {
        Debug.Log("OnPreNetDestroy - rootView : " + rootView);
    }
}
