using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

namespace Tim
{
    /* ILobbyCallbacks 大廳 */
    public partial class MyPhotonRealtimeCallBack : ILobbyCallbacks
    {
        /// <summary>
        /// 已連線至大廳
        /// </summary>
        public override void OnJoinedLobby()
        {
            Debug.Log(string.Format("Photon.Realtime.ILobbyCallbacks - OnJoinedLobby()<已連線至大廳>"));
        }

        /// <summary>
        /// 已離開於大廳
        /// </summary>
        public override void OnLeftLobby()
        {
            Debug.Log(string.Format("Photon.Realtime.ILobbyCallbacks - OnLeftLobby()<已離開於大廳>"));
        }

        /// <summary>
        /// 更新大廳列表(TypedLobbyInfo : 服務器上大廳的信息。 當LoadBalancingClient.EnableLobbyStatistics為true時使用)???
        /// </summary>
        /// <param name="lobbyStatistics"></param>
        public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            string strlobbyStatistics = "";
            for (int i = 0; i < lobbyStatistics.Count; i++)
            {
                string strLobbyType = "";
                switch (lobbyStatistics[i].Type)
                {
                    case LobbyType.Default:
                        strLobbyType = "標準類型和行為：加入此大廳後，客戶會獲得房間列表，JoinRandomRoom可以使用簡單的過濾器來匹配屬性（完全匹配）。";
                        break;
                    case LobbyType.SqlLobby:
                        strLobbyType = "此大廳類型列出了諸如Default之類的房間，但是JoinRandom具有用於類似SQL的“ where”子句的參數以進行過濾。 這允許更大，更少，或和和組合";
                        break;
                    case LobbyType.AsyncRandomLobby:
                        strLobbyType = "該大廳不發送遊戲列表。 它僅用於OpJoinRandomRoom(意無法指定)。 當只剩下不活動的用戶時，它可以使房間保持一段時間。";
                        break;
                }

                strlobbyStatistics += string.Format("{0} - PlayerCount大廳人數:{1}, RoomCount大廳房間:{2}\n" +
                    "LobbyName:{3}, \n" +
                    "LobbyType:{4}({5}), \n" +
                    "IsDefault返回此實例是否指向“默認大廳”:{6}, \n" +
                    "ToString():{7}\n",
                    i, lobbyStatistics[i].PlayerCount, lobbyStatistics[i].RoomCount,
                    lobbyStatistics[i].Name,
                    lobbyStatistics[i].Type,
                    strLobbyType,
                    lobbyStatistics[i].IsDefault,
                    lobbyStatistics[i].ToString());
            }
            Debug.Log(string.Format("Photon.Realtime.ILobbyCallbacks - OnLobbyStatisticsUpdate()<更新大廳列表如下 : \n" +
                "{0}>",
                strlobbyStatistics));
        }

        /// <summary>
        /// 更新房間列表
        /// </summary>
        /// <param name="roomList"></param>
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            string strroomList = "";
            for (int i = 0; i < roomList.Count; i++)
            {
                strroomList += string.Format("" +
                    "{0} - \n" +
                    "RemovedFromList滿、關閉、隱藏是否顯示於列表:{1}, \n" +
                    "masterClientId:{2}, \n" +
                    "Name:{3}, \n" +
                    "PlayerCount:{4}, \n" +
                    "MaxPlayers:{5}, \n" +
                    "IsOpen:{6}, \n" +
                    "IsVisible:{7}, \n" +
                    "ToStringFull():{8}\n",
                    i,
                    roomList[i].RemovedFromList,
                    roomList[i].masterClientId,
                    roomList[i].Name,
                    roomList[i].PlayerCount,
                    roomList[i].MaxPlayers,
                    roomList[i].IsOpen,
                    roomList[i].IsVisible,
                    roomList[i].ToStringFull()
                    );
            }

            Debug.Log(string.Format("Photon.Realtime.ILobbyCallbacks - OnRoomListUpdate()<更新房間列表如下 : \n" +
            "{0}>", strroomList));
        }
    }

}

