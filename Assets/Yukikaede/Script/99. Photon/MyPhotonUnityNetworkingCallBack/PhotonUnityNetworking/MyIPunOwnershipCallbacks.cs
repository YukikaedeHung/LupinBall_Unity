using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tim;

/// <summary>
/// PhotonView.Owner轉讓相關 ★全局只需要一個
/// </summary>
public class MyIPunOwnershipCallbacks : MonoBehaviour, IPunOwnershipCallbacks
{
    /* https://www.youtube.com/watch?v=W0FnBDODAjI */

    private void Awake()
    {
        MyPhotonNetwork.AddCallbackTarget(this);
    }

    public virtual void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        Debug.Log(string.Format("OnOwnershipRequest - Player targetView:{0}, Player requestingPlayer:{1} ", targetView, requestingPlayer));
    }

    public virtual void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        Debug.Log(string.Format("OnOwnershipTransfered - Player targetView:{0}, Player previousOwner:{1} ", targetView, previousOwner));
    }

    public virtual void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        Debug.Log(string.Format("OnOwnershipTransferFailed - Player targetView:{0}, Player senderOfFailedRequest:{1} ", targetView, senderOfFailedRequest));
    }
}
