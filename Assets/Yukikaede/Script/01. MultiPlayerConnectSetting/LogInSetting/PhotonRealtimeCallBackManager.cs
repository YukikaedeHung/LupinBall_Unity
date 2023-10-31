using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Realtime;
using ExitGames.Client.Photon;

namespace Tim.Game
{
    public class PhotonRealtimeCallBackManager : MyPhotonRealtimeCallBack
    {
        public Dictionary<string, bool> dictPlayerReadys = new Dictionary<string, bool>();

        public static PhotonRealtimeCallBackManager inst;

        #region PhotonServer連線設定
        //連線設定Server
        #region 連線設定-連線確認 OnConnected()
        /// <summary>
        /// 連線設定-連線確認 OnConnected()
        /// </summary>
        public override void OnConnected()
        {
            base.OnConnected();
            Debug.Log("連線設定-連線確認的CallBack功能 OnConnected()");
        }
        #endregion

        #region 連線設定-連線到MasterServer的CallBack功能 OnConnectedToMaster()
        /// <summary>
        /// 連線設定-連線到MasterServer的CallBack功能 OnConnectedToMaster()
        /// </summary>
        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            Debug.Log("連線設定-連線到MasterServer的CallBack功能 OnConnectedToMaster()");
            MultiPlayerSettingManagerScript.inst.EnterMasterServer();
        }
        #endregion

        #region 連線設定-已中斷連線 OnDisconnected(DisconnectCause cause)
        /// <summary>
        /// 連線設定-已中斷連線 OnDisconnected(DisconnectCause cause)
        /// </summary>
        /// <param name="cause"></param>
        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            MultiPlayerSettingManagerScript.inst.Initial();
            Debug.Log("連線設定-已中斷連線 OnDisconnected(DisconnectCause cause)");
        }
        #endregion

        #region 連線設定-各區域相關資訊，當名稱服務器提供標題區域列表時調用此功能來檢查RegionHandler類的描述，以使用提供的值 OnRegionListReceived(RegionHandler regionHandler)
        /// <summary>
        /// 連線設定-各區域相關資訊，當名稱服務器提供標題區域列表時調用此功能來檢查RegionHandler類的描述，以使用提供的值 OnRegionListReceived(RegionHandler regionHandler)
        /// </summary>
        /// <param name="regionHandler"></param>
        public override void OnRegionListReceived(RegionHandler regionHandler)
        {
            base.OnRegionListReceived(regionHandler);
            Debug.Log("連線設定-各區域相關資訊 OnRegionListReceived(RegionHandler regionHandler)");
        }
        #endregion

        //大廳設定Lobby
        #region 連線設定-已連線至大廳 OnJoinedLobby()
        /// <summary>
        /// 連線設定-已連線至大廳 OnJoinedLobby()
        /// </summary>
        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            TypedLobby _TypedLobby = MyPhotonNetwork.CurrentLobby;
            Debug.Log(string.Format("連線設定-已連線至大廳 OnJoinedLobby(): Lobbyname = {0}, Lobbytype = {1}", _TypedLobby.Name, _TypedLobby.Type));
            MultiPlayerSettingManagerScript.inst.EnetrLobby();
        }
        #endregion

        #region 連線設定-已離開於大廳 OnLeftLobby()
        /// <summary>
        /// 連線設定-已離開於大廳 OnLeftLobby()
        /// </summary>
        public override void OnLeftLobby()
        {
            base.OnLeftLobby();
            Debug.Log("連線設定-已離開於大廳 OnLeftLobby()");
            MultiPlayerSettingManagerScript.inst.EnterMasterServer();
        }
        #endregion

        #region 連線設定-更新服務器上的大廳列表TypedLobbyInfo的信息 OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        /// <summary>
        /// 連線設定-更新服務器上的大廳列表TypedLobbyInfo的信息 OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        /// 此功能需在LoadBalancingClient.EnableLobbyStatistics為true時才可使用
        /// </summary>
        /// <param name="lobbyStatistics"></param>
        public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            base.OnLobbyStatisticsUpdate(lobbyStatistics);
            Debug.Log("連線設定-更新服務器上的大廳列表TypedLobbyInfo的信息 OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)");
        }
        #endregion

        //房間設定Room
        #region 連線設定-已創建房間 OnCreatedRoom()
        /// <summary>
        /// 連線設定-已創建房間 OnCreatedRoom()
        /// </summary>
        public override void OnCreatedRoom()
        {
            TypedLobby _TypedLobby = MyPhotonNetwork.CurrentLobby;
            Debug.Log(string.Format("連線設定-已創建房間 OnCreatedRoom(): Lobbyname = {0}, Lobbytype = {1}", _TypedLobby.Name, _TypedLobby.Type));
            base.OnCreatedRoom();
        }
        #endregion

        #region 連線設定-創建房間失敗 OnCreateRoomFailed(short returnCode, string message)
        /// <summary>
        /// 連線設定-創建房間失敗 OnCreateRoomFailed(short returnCode, string message)
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="message"></param>
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
        }
        #endregion

        #region 連線設定-已加入房間 OnJoinedRoom()
        /// <summary>
        /// 連線設定-當前使用者已加入房間 OnJoinedRoom()
        /// 內部相關的Callback均是以當前得使用者來作回應，而非房間內的其他成員
        /// 範例 Room中的聊天室窗刷新：玩家1進入房間，此時當事人本身的聊天室窗會被刷新，但房間內的其他人仍會擁有原本的聊天資訊
        /// </summary>
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            MultiPlayerSettingManagerScript.inst.objRoom.SetActive(true);

            RoomInfoUpdate();
            MultiPlayerSettingManagerScript.inst.bGameReady = false;
            MultiPlayerSettingManagerScript.inst.imgGameReady.sprite = null;
            dictPlayerReadys.Remove(MyPhotonNetwork.LocalPlayer.NickName);
            dictPlayerReadys.Add(MyPhotonNetwork.LocalPlayer.NickName, false);
        }
        #endregion

        #region 連線設定-離開房間 OnLeftRoom()
        /// <summary>
        /// 連線設定-離開房間 OnLeftRoom()
        /// 內部相關的Callback均是以當前離開的這位使用者來作回應，而非房間內的其他成員
        /// 範例 Room中的聊天室窗刷新：玩家1離開房間，此時玩家1本身的聊天室窗會被刷新，但留在房間內的其他人仍會擁有原本的聊天資訊
        /// </summary>
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            MessageClear();
            string s = string.Format("Leave Room");
            MultiPlayerSettingManagerScript.inst.TipsEven(s);
        }
        #endregion

        #region 連線設定-加入房間失敗 OnJoinRoomFailed(short returnCode, string message)
        /// <summary>
        /// 連線設定-加入房間失敗 OnJoinRoomFailed(short returnCode, string message)
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="message"></param>
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
            MultiPlayerSettingManagerScript.inst.EnetrLobby();
            string s = string.Format("Join Room Fail");
            MultiPlayerSettingManagerScript.inst.TipsEven(s);
        }
        #endregion

        #region 連線設定-隨機加入房間失敗 OnJoinRandomFailed(short returnCode, string message)
        /// <summary>
        /// 連線設定-隨機加入房間失敗 OnJoinRandomFailed(short returnCode, string message)
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="message"></param>
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);
            MultiPlayerSettingManagerScript.inst.EnetrLobby();
            string s = string.Format("Join Random Room Fail");
            MultiPlayerSettingManagerScript.inst.TipsEven(s);
        }
        #endregion

        #region 連線設定-玩家已進入房間 OnPlayerEnteredRoom(Player newPlayer)
        /// <summary>
        /// 連線設定-玩家已進入房間，扣除新加入的人NewPlayer，房間內的其他人可以取得相關CallBack回應 OnPlayerEnteredRoom(Player newPlayer)
        /// 內部的相關CallBack均是以房間內的其他人來取得新加入者newPlayer的訊息
        /// 會刷新當前使用者的相關應用，而非當事人newPlayer的應用
        /// 範例 Room中的聊天室窗刷新：玩家1進入房間，此時房間內的其他人的聊天室窗會被刷新，但newPlayer本身還是會帶著原本其他地方的聊天資訊來到這房間
        /// </summary>
        /// <param name="newPlayer"></param>
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log("連線設定-玩家已進入房間 OnPlayerEnteredRoom");
            base.OnPlayerEnteredRoom(newPlayer);
            dictPlayerReadys.Remove(newPlayer.NickName);
            dictPlayerReadys.Add(newPlayer.NickName, false);
            MultiPlayerSettingManagerScript.inst.txtMessage.text += newPlayer.NickName + " Join The Room" +  "\n";
            Debug.Log("PlayerIn : " + newPlayer.NickName);
            RoomInfoUpdate();
            CheckReadyForStartGame();
        }
        #endregion

        #region 連線設定-玩家已離開房間 OnPlayerLeftRoom(Player otherPlayer)
        /// <summary>
        /// 連線設定-玩家(otherPlayer)已離開房間，而房間內的其他人可以取得相關CallBack回應 OnPlayerLeftRoom(Player otherPlayer)
        /// 內部的相關CallBack均是以房間內的其他人來取得離開者otherPlayer的訊息
        /// 會刷新當前使用者的相關應用，而非當事人otherPlayer的應用
        /// 範例 Room中的聊天室窗刷新：玩家1離開房間，此時房間內的其他人的聊天室窗會被刷新
        /// </summary>
        /// <param name="otherPlayer"></param>
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            if (!otherPlayer.IsInactive)
            {
                dictPlayerReadys.Remove(otherPlayer.NickName);
                MultiPlayerSettingManagerScript.inst.txtMessage.text += otherPlayer.NickName + " Leave The Room" + "\n";
                Debug.Log("PlayerLeave : " + otherPlayer.NickName);
                RoomInfoUpdate();
                CheckReadyForStartGame();
            }
        }
        #endregion

        #region 連線設定-更新房間列表 OnRoomListUpdate(List<RoomInfo> roomList)
        /// <summary>
        /// 連線設定-更新房間列表 OnRoomListUpdate(List<RoomInfo> roomList)
        /// </summary>
        /// <param name="roomList"></param>
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("OnRoomListUpdate - roomList.Count : " + roomList.Count);

            base.OnRoomListUpdate(roomList);

            //清空dictRoomListItems原有的RoomList資料
            MultiPlayerSettingManagerScript.inst.RoomListClear();

            //填入新的RoomList資料
            for (int i = 0; i < roomList.Count; i++)
            {
                RoomInfo room = roomList[i];

                /*創建房間列表項目*/
                float fPreCreatedRoomY = 220 - (i * 60);
                /*將預置物objBtnPreCreatedRoom，生成在imgRoomListArea的區域內*/
                GameObject roomListItem = Instantiate(MultiPlayerSettingManagerScript.inst.objBtnPreCreatedRoom, MultiPlayerSettingManagerScript.inst.imgRoomListArea.transform);
                roomListItem.transform.localPosition = Vector2.zero; //將預置物的本地位置設置為Image物件的中心位置
                roomListItem.transform.localPosition = new Vector2(-250, fPreCreatedRoomY);//將預置物放置到預擺置的位置
                roomListItem.GetComponentInChildren<Text>().text = room.Name;

                /*設定按鈕點擊事件*/
                Button btnRoomList = roomListItem.GetComponent<Button>();
                string roomName = room.Name;
                btnRoomList.onClick.AddListener(delegate(){
                    MultiPlayerSettingManagerScript.inst.bRoomSelect = !MultiPlayerSettingManagerScript.inst.bRoomSelect;
                    Image btnRoomListImage = btnRoomList.GetComponent<Image>();
                    btnRoomListImage.color = MultiPlayerSettingManagerScript.inst.bRoomSelect ? new Color32 (195, 195, 195, 255) : new Color32 (255, 255, 255, 255);

                    MultiPlayerSettingManagerScript.inst.btnJoinRoom.interactable = MultiPlayerSettingManagerScript.inst.bRoomSelect;
                    MultiPlayerSettingManagerScript.inst.sSelectedRoomNameForJoin = roomName;                    
                });

                /*將房間列表項目加入dictRoomListItems中供後續使用*/
                MultiPlayerSettingManagerScript.inst.dictRoomListItems.Add(room.Name, roomListItem);
            }

            /*將RoomList的清單重新排序*/
            MultiPlayerSettingManagerScript.inst.RoomListSorting();
        }
        #endregion

        #region 連線設定-房間屬性已更新 OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        /// <summary>
        /// 連線設定-房間屬性已更新 OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        /// </summary>
        /// <param name="propertiesThatChanged"></param>
        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            Debug.Log("房間屬性已更新");
            base.OnRoomPropertiesUpdate(propertiesThatChanged);
        }
        #endregion

        #region 連線設定-MasterClient已換人 OnMasterClientSwitched(Player newMasterClient)
        /// <summary>
        /// 連線設定-MasterClient已換人 OnMasterClientSwitched(Player newMasterClient)
        /// </summary>
        /// <param name="newMasterClient"></param>
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
            string s = string.Format("Change Host :\n {0}", newMasterClient.NickName);
            MultiPlayerSettingManagerScript.inst.TipsEven(s);
        }
        #endregion

        #region 連線設定-玩家狀態已更新 OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        /// <summary>
        /// 連線設定-玩家狀態已更新 OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        /// </summary>
        /// <param name="targetPlayer"></param>
        /// <param name="changedProps"></param>
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

            if (SceneManager.GetActiveScene().name == GameSceneScript.SceneName_GameLogIn)
            {
                #region 聊天Chat的設定
                if (changedProps.ContainsKey("Chat"))
                    MultiPlayerSettingManagerScript.inst.txtMessage.text += targetPlayer.NickName + " : " + (changedProps["Chat"]).ToString() + "\n";
                #endregion

                #region 遊戲準備Ready的設定
                if (changedProps.ContainsKey("Ready"))
                {
                    string sReady = (bool)changedProps["Ready"] ? "YES" : "NO";

                    MultiPlayerSettingManagerScript.inst.txtMessage.text += targetPlayer.NickName + "(Ready)" + " : " + sReady + "\n";

                    if (dictPlayerReadys.ContainsKey(targetPlayer.NickName))
                    {
                        dictPlayerReadys[targetPlayer.NickName] = (bool)changedProps["Ready"];
                    }
                    else
                    {
                        dictPlayerReadys.Add(targetPlayer.NickName, (bool)changedProps["Ready"]);
                    }
                    CheckReadyForStartGame();
                }
                #endregion
                RoomInfoUpdate();
            }
        }
        #endregion
        #endregion

        #region Player身分認證設定
        #region 身分認證-自定義身份驗證失敗，需在Dashboard中，設置了自定義身份驗證，否則將不會觸發此功能 OnCustomAuthenticationFailed(string debugMessage)
        /// <summary>
        /// 身分認證-自定義身份驗證失敗，需在Dashboard中，設置了自定義身份驗證，否則將不會觸發此功能 OnCustomAuthenticationFailed(string debugMessage)
        /// </summary>
        /// <param name="debugMessage"></param>
        public override void OnCustomAuthenticationFailed(string debugMessage)
        {
            base.OnCustomAuthenticationFailed(debugMessage);
            Debug.Log("身分認證-自定義身份驗證失敗 OnCustomAuthenticationFailed(string debugMessage)");
        }
        #endregion

        #region 身分認證-自定義身份驗證成功，需在Dashboard中，設置了自定義身份驗證，否則將不會觸發此功能 OnCustomAuthenticationResponse(Dictionary<string, object> data)
        /// <summary>
        /// 身分認證-自定義身份驗證成功，需在Dashboard中，設置了自定義身份驗證，否則將不會觸發此功能 OnCustomAuthenticationResponse(Dictionary<string, object> data)
        /// </summary>
        /// <param name="data"></param>
        public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
            base.OnCustomAuthenticationResponse(data);
            Debug.Log("身分認證-自定義身份驗證成功 OnCustomAuthenticationResponse(Dictionary<string, object> data)");
        }
        #endregion
        #endregion

        #region 使用者功能
        #region 使用者功能-好友列表 OnFriendListUpdate(List<FriendInfo> friendList)
        /// <summary>
        /// 使用者功能-好友列表 OnFriendListUpdate(List<FriendInfo> friendList)
        /// </summary>
        /// <param name="friendList"></param>
        public override void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            base.OnFriendListUpdate(friendList);

            string strFriendList = "";
            for (int i = 0; i < friendList.Count; i++)
            {
                strFriendList += string.Format("{0} - Name(已過時請改用UserId):{1}, UserId:{2}, IsOnline:{3}, Room:{4}, IsInRoom:{5},\n" +
                    "ToString():{6} \n",
                    i,
                    friendList[i].Name,
                    friendList[i].UserId,
                    friendList[i].IsOnline,
                    friendList[i].Room,
                    friendList[i].IsInRoom,
                    friendList[i].ToString());
            }
        }
        #endregion

        #region 使用者功能-IErrorInfoCallback錯誤(需事先註冊)
        public override void OnErrorInfo(ErrorInfo errorInfo)
        {
            base.OnErrorInfo(errorInfo);
        }
        #endregion

        #region 使用者功能-WebRpc響應 OnWebRpcResponse(OperationResponse response)
        /// <summary>
        /// 使用者功能-WebRpc響應 OnWebRpcResponse(OperationResponse response)
        /// 使用WebRPC，必須事先安裝好以HTTP基礎的伺服器端應用程式。此處製作的伺服器端應用程式，將以Photon Turnbased 儀表板進行設定。
        /// </summary>
        /// <param name="response"></param>
        public override void OnWebRpcResponse(OperationResponse response)
        {
            base.OnWebRpcResponse(response);
        }
        #endregion
        #endregion

        #region Yukikaede額外追加的功能
        #region 確認房間內玩家是否都準備就緒後，開始進行遊戲 CheckReadyForStartGame()
        /// <summary>
        /// 確認房間內玩家是否都準備就緒後，開始進行遊戲 CheckReadyForStartGame()
        /// </summary>
        void CheckReadyForStartGame()
        {        
            if (MultiPlayerSettingManagerScript.inst.bYukiPass)
            {
                MultiPlayerSettingManagerScript.inst.btnGameStart.gameObject.SetActive(true);
            }
            else if (MyPhotonNetwork.IsMasterClient && MyPhotonNetwork.CurrentRoom.MaxPlayers == MyPhotonNetwork.CurrentRoom.PlayerCount)
            {
                bool bAllReady = true;
                foreach (var b in dictPlayerReadys)
                {
                    if (b.Value == false)
                    {
                        bAllReady = false;
                        break;
                    }
                }                
                MultiPlayerSettingManagerScript.inst.btnGameStart.gameObject.SetActive(bAllReady);
            }
            else if (MyPhotonNetwork.CurrentRoom.MaxPlayers != MyPhotonNetwork.CurrentRoom.PlayerCount)
            {
                Debug.Log("Somebody Out");
                MultiPlayerSettingManagerScript.inst.btnGameStart.gameObject.SetActive(false);
            }
        }
        #endregion

        #region 玩家加入/離開房間自動清空Room中的txtMessage.text資訊 MessageClear()
        /// <summary>
        /// 玩家加入/離開房間自動清空Room中的txtMessage.text資訊 MessageClear()
        /// </summary>
        void MessageClear()
        {
            Debug.Log("MessageClear");
            MultiPlayerSettingManagerScript.inst.txtMessage.text = "";
        }
        #endregion

        #region 刷新RoomInfo資訊 RoomInfoUpdate()
        void RoomInfoUpdate()
        {
            Debug.Log("RoomInfoUpdate()");
            Room _Room = MyPhotonNetwork.CurrentRoom;
            List<string> sNickNameList = new List<string>();
            Player[] _Players = MyPhotonNetwork.PlayerList;
            foreach (Player targetPlayer in _Players)
            {
                sNickNameList.Add(targetPlayer.NickName);
            }
            /*房間資訊顯示*/
            string sAllPlayerNickName = string.Join("\n", sNickNameList);//將名子逐個換行
            string sRoomInfo = string.Format(
                "{0} \n" +
                "IsVisible : {1} \n" +
                "Players :{2} / {3} \n" +
                "Members : \n{4}",
                _Room.Name,
                _Room.IsVisible,
                _Room.PlayerCount,
                _Room.MaxPlayers,
                sAllPlayerNickName
                );
            MultiPlayerSettingManagerScript.inst.txtRoomInfo.text = sRoomInfo;
        }
        #endregion
        #endregion
    }
}


