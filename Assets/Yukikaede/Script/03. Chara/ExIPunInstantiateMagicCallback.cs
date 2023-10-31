// ----------------------------------------------------------------------------
// 這段程式碼用於將玩家創建之角色作組隊分類
// ----------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ExIPunInstantiateMagicCallback : MyIPunInstantiateMagicCallback
{
    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        base.OnPhotonInstantiate(info);

        object[] obj = info.photonView.InstantiationData;
        Transform tr = PhotonView.Find((int)obj[0]).transform;
        transform.parent = tr;
    }
}
