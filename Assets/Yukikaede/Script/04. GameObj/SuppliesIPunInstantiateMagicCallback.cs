// ----------------------------------------------------------------------------
// 這段程式碼用於將玩家創建之角色作子彈顏色分配
// ----------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Tim.Game;

public class SuppliesIPunInstantiateMagicCallback : MyIPunInstantiateMagicCallback
{
    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);
        object[] obj = info.photonView.InstantiationData;
        Transform tr = PhotonView.Find((int)obj[0]).transform;
        transform.parent = tr;
    }
}
