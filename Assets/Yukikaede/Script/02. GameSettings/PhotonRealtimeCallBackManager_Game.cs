using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Tim;
using Tim.Game;

public class PhotonRealtimeCallBackManager_Game : MyPhotonRealtimeCallBack
{
    /// <summary>
    /// 玩家屬性已更新
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <param name="changedProps"></param>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        //當玩家屬性變更時，來檢查存活人數
        Player[] ps = MyPhotonNetwork.PlayerList;
        int surviver = 0;
        for (int i = 0; i < ps.Length; i++)
        {
            //預設玩家為存活者
            surviver++;
            //若玩家已死亡再扣掉
            if (ps[i].CustomProperties.ContainsKey("DEAD") && (bool)ps[i].CustomProperties["DEAD"] == true)
            {
                surviver--;
            }
        }

        //獲勝者：如果存活者剩一位，則設定其為獲勝者
        if (surviver == 1)
        {
            //結束遊戲
            Debug.Log("Only One Player");
            GameManager.inst.GameOVerEvent();
        }
    }

    /// <summary>
    /// 玩家已離開房間
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Player[] ps = MyPhotonNetwork.PlayerList;

        //設定剩下的玩家為獲勝者
        Debug.Log("Player Leave the Room : " + otherPlayer.NickName);

        MyPhotonNetwork.CurrentRoom.IsOpen = false;
        GameManager.inst.GameOVerEvent();
    }
}
