using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Tim
{
    /*MasterServer & Lobby*/
    public static partial class MyPhotonNetwork
    {
        #region 時間相關
        /// <summary>
        ///光子網絡時間，與服務器同步。(從4294967.295“環繞”到0)
        /// </summary>
        /// <remarks>
        /// v1.55 <br/>
        ///此時間值取決於服務器的Environment.TickCount。 每個服務器不同
        ///但在一個Room中，所有客戶端應具有相同的值（Room僅在一台服務器上）。<br/>
        ///這不是DateTime！<br/>
        ///
        ///謹慎使用此值：<br/>
        ///可以從任何正值開始。<br/>
        ///它將從4294967.295“環繞”到0！
        /// </remarks>
        public static double Time
        {
            get
            {
                return PhotonNetwork.Time;
            }
        }
        /// <summary>
        ///當前服務器的毫秒時間戳。(從4294967 . 295“環繞”到0)=>(從4294967295“環繞”到0)
        /// </summary>
        /// <remarks>
        ///這對於在一個房間中同步所有客戶端上的動作和事件很有用。
        ///時間戳基於服務器的Environment.TickCount。
        ///
        ///它會經常從正值溢出到負值，所以
        ///注意事項發生時，請僅使用時差檢查時間增量
        ///發生。
        ///
        ///這是PhotonNetwork.Time的基礎。
        /// </remarks>
        public static int ServerTimestamp
        {
            get
            {
                return PhotonNetwork.ServerTimestamp; ;
            }
        }
        #endregion

        #region 玩家相關
        /// <summary>
        /// 除非應用程序關閉，否則此客戶端的Player實例始終可用。
        /// </summary>
        /// <remarks>
        /// 有用（例如）隨時為該客戶端設置自定義播放器屬性或暱稱。
        /// 當客戶端加入會議室時，將同步“自定義屬性”和其他值。
        /// </remarks>
        public static Player LocalPlayer
        {
            get
            {
                return PhotonNetwork.LocalPlayer;
            }
        }

        /// <summary>
        ///設置此（本地）播放器的屬性，並將它們與其他播放器同步（不要直接修改它們）(★★★=new才會更新，不要拿原資料Add)。
        /// </summary>
        /// <remarks>
        ///在房間裡時，您的媒體資源會與其他玩家同步。
        ///當您進入房間時，CreateRoom，JoinRoom和JoinRandomRoom將全部應用播放器的自定義屬性。
        ///整個Heshtable將被發送。通過僅設置更新的鍵/值來最大程度地減少流量。
        ///
        ///如果Hashtable為null，則將清除自定義屬性。
        ///自定義屬性永遠不會自動清除，因此，如果您不進行更改，它們將轉移到下一個房間。
        ///
        ///不要通過修改PhotonNetwork.player.customProperties來設置屬性！
        /// </remarks>
        /// <param name ="customProperties">此Heshtable將僅使用字符串類型的鍵。如果為null，則將刪除所有自定義屬性。</param>
        /// <returns>
        ///如果customProperties為空或具有零個字符串鍵，則為False。
        ///在離線模式下為True。
        ///如果不在房間內，則為True，這是本地播放器
        ///（使用此屬性緩存加入會議室時要發送的屬性）。
        ///否則，返回是否可以將該操作發送到服務器。
        /// </returns>
        public static bool SetPlayerCustomProperties(ExitGames.Client.Photon.Hashtable customProperties)
        {
            return PhotonNetwork.SetPlayerCustomProperties(customProperties);
        }

        /// <summary>
        ///在本地刪除“此”播放器的自定義屬性。重要提示：這不會同步更改！切換房間時很有用。
        /// </summary>
        /// <remarks>
        ///請謹慎使用此方法。它會在玩家之間造成狀態不一致！
        ///這只會在本地更改player.customProperties。這有助於清除您的
        ///遊戲之間的自定義屬性（假設它們存儲了您做出的回合，擊殺等）。
        ///
        /// SetPlayerCustomProperties（）同步，可用於在房間中將值設置為null。
        ///在房間中可以認為是“已移除”。
        ///
        ///如果customPropertiesToDelete為null或具有0個條目，則將刪除所有“自定義屬性”（替換為新的Hashtable）。
        ///如果您指定要刪除的鍵，則這些鍵將從哈希表中刪除，但其他鍵不受影響。
        /// </remarks>
        /// <param name ="customPropertiesToDelete">要刪除的自定義屬性鍵(key)的列表。請參閱備註。</param>
        public static void RemovePlayerCustomProperties(string[] customPropertiesToDelete)
        {
            PhotonNetwork.RemovePlayerCustomProperties(customPropertiesToDelete);
        }

        /// <summary>
        ///請求房間和在線狀態以獲取朋友列表，並將結果保存在PhotonNetwork.Friends中。
        /// </summary>
        /// <remarks>
        ///僅在主服務器上起作用，以查找選定用戶列表所播放的房間。
        ///
        ///結果將存儲在PhotonNetwork.Friends中。
        ///該列表在首次使用OpFindFriends時初始化（在此之前為null）。
        ///要刷新列表，請再次致電FindFriends（5秒或10或20）。
        ///
        ///用戶通過在PhotonNetwork.AuthValues中設置唯一的userId來標識自己。
        ///有關如何設置和使用此信息，請參見AuthenticationValues的說明。
        ///
        ///朋友列表必須從其他來源獲取（Photon不提供）。
        ///
        ///
        /// 內部的：
        ///服務器響應包括2個信息數組（每個索引與請求中的一個朋友匹配）：
        /// ParameterCode.FindFriendsResponseOnlineList = bool []在線狀態
        /// ParameterCode.FindFriendsResponseRoomIdList =房間名稱的字符串[]（如果不在房間中，則為空字符串）
        /// </remarks>
        /// <param name ="friendsToFind">朋友數組（請確保使用唯一的暱稱或AuthValues）。</param>
        /// <returns>如果可以發送操作（需要連接，則在任何時候都只允許一個請求）。在離線模式下始終為false。</returns>
        public static bool FindFriends(string[] friendsToFind)
        {
            return PhotonNetwork.FindFriends(friendsToFind);
        }

        /// <summary>
        /// 此雲區域玩家"不在"房間的人數（每隔5秒在MasterServer上可用）。
        /// </summary>
        public static int CountOfPlayersOnMaster
        {
            get
            {
                return PhotonNetwork.CountOfPlayersOnMaster;
            }
        }

        /// <summary>
        /// 此雲區域玩家"在"房間的人數（每隔5秒在MasterServer上可用）。
        /// </summary>
        public static int CountOfPlayersInRooms
        {
            get
            {
                return PhotonNetwork.CountOfPlayersInRooms;
            }
        }

        /// <summary>
        /// 此雲區域玩家總人數（每隔5秒在MasterServer上可用）。
        /// </summary>
        public static int CountOfPlayers
        {
            get
            {
                return PhotonNetwork.CountOfPlayers;
            }
        }

        /// <summary>
        /// 此雲區域房間總數（每隔5秒在MasterServer上可用）。
        /// </summary>
        public static int CountOfRooms
        {
            get
            {
                return PhotonNetwork.CountOfRooms;
            }
        }

        /// <summary>
        ///???
        ///啟用或禁用有關此客戶端流量的統計信息收集。
        /// </summary>
        /// <remarks>
        ///如果您遇到與客戶有關的問題，則流量統計信息是查找解決方案的一個很好的起點。
        ///只有啟用了統計信息，您才能使用GetVitalStats
        /// </remarks>
        public static bool NetworkStatisticsEnabled
        {
            get
            {
                return PhotonNetwork.NetworkStatisticsEnabled;
            }

            set
            {
                PhotonNetwork.NetworkStatisticsEnabled = value;
            }
        }
        #endregion

        #region 大廳相關
        /// <summary>從服務器獲取一個自定義的遊戲列表，並與（非空的）類似SQL的過濾器匹配。觸發OnRoomListUpdate回調。</summary>
        /// <remarks>
        ///該操作僅適用於SqlLobby類型的大廳，並且過濾器不能為空。
        ///它會檢查這些條件並在本地失敗，並返回false。
        ///這是一個異步請求。
        ///
        ///★★★注意：您不必加入大廳即可對其進行查詢。需要將房間“附加”到大廳，這可以完成
        ///通過CreateRoom，JoinOrCreateRoom等中的typedLobby參數。
        ///
        ///完成後，將調用OnRoomListUpdate。
        /// </remarks>
        /// 請參閱 https://doc.photonengine.com/en-us/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby/#sql_lobby_type 
        /// <param name ="typedLobby">要查詢的大廳。必須是SqlLobby類型。</param>
        /// <param name ="sqlLobbyFilter"> sql查詢語句。</param>
        /// <returns>是否可以發送操作（必須連接）。</returns>
        public static bool GetCustomRoomList(TypedLobby typedLobby, string sqlLobbyFilter)
        {
            return PhotonNetwork.GetCustomRoomList(typedLobby, sqlLobbyFilter);
        }

        /// <summary>
        /// ?????
        ///如果啟用，則客戶端將從主服務器獲取可用大廳的列表。
        /// </summary>
        /// <remarks>
        ///在客戶端連接到主服務器之前設置此值。 與主人連接時
        ///服務器，更改無效。
        ///
        ///實現OptionalInfoCallbacks.OnLobbyStatisticsUpdate，以獲取已使用大廳的列表。
        ///
        ///如果您的標題動態使用大廳（取決於（例如）），則大廳統計信息可能會很有用
        ///當前玩家的活動或類似情況。
        ///在這種情況下，獲取可用大廳的列表，它們的房間數和玩家數可以
        ///是有用的信息。
        ///
        /// ConnectUsingSettings將此設置為PhotonServerSettings值。
        /// </remarks>
        public static bool EnableLobbyStatistics
        {
            get
            {
                return PhotonNetwork.EnableLobbyStatistics;
            }
        }

        /// <summary>此客戶在大廳中時為真。</summary>
        /// <remarks>
        ///實現IPunCallbacks.OnRoomListUpdate（）以獲得房間列表時的通知
        ///變為可用或已更新。
        ///
        ///加入房間後，您將自動離開任何大廳！
        ///大廳僅存在於主服務器上（而房間由遊戲服務器處理）。
        /// </remarks>
        public static bool InLobby
        {
            get
            {
                return PhotonNetwork.InLobby;
            }
        }

        /// <summary>
        /// 當PUN加入大廳或創建遊戲時將使用的大廳。
        /// 這是在加入大廳或創建房間時定義的
        /// </summary>
        /// <remarks>
        /// 默認的大廳使用一個空字符串作為名稱。
        /// 因此，當您連接或離開房間時，PUN會自動使您再次進入大廳。
        /// 如果客戶端在大廳中，請檢查PhotonNetwork.InLobby。
        /// masterServerAndLobby
        /// </remarks>
        public static TypedLobby CurrentLobby
        {
            get { return PhotonNetwork.CurrentLobby; }
        }

        /// <summary>在MasterServer上，它加入了默認大廳，該大廳列出了當前正在使用的房間。</summary>
        /// <remarks>
        ///房間列表由服務器使用ILobbyCallbacks.OnRoomListUpdate發送和刷新。
        ///
        ///每個房間都應在加入之前檢查房間是否已滿。光子還列出了
        ///已滿，除非您關閉並隱藏它們（room.open = false和room.visible = false）。
        ///
        ///在最佳情況下，您可以讓客戶加入隨機遊戲，如下所述：
        /// https://doc.photonengine.com/zh-CN/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby
        ///
        ///
        ///您可以在不加入大廳的情況下顯示當前的球員和房間數（但是您必須
        ///在主服務器上）。使用：CountOfPlayers，CountOfPlayersOnMaster，CountOfPlayersInRooms和
        /// CountOfRooms。
        ///
        ///您可以使用多個大廳來縮短房間列表。請參閱JoinLobby（TypedLobby大廳）。
        ///創建新房間時，它們將“附加”到當前使用的大廳或默認大廳。
        ///
        ///★您可以在沒有大廳的情況下使用JoinRandomRoom！
        /// </remarks>
        public static bool JoinLobby()
        {
            return PhotonNetwork.JoinLobby();
        }

        /// <summary>??? 在主服務器上，您可以加入大廳以獲取可用房間的列表。</summary>
        /// <remarks>
        ///房間列表由服務器使用ILobbyCallbacks.OnRoomListUpdate發送和刷新。
        ///
        ///任何客戶都可以即時“組成”任何大廳。將房間分為多個大廳
        ///縮短每個列表。但是，列表過多可能會破壞配對的經驗。
        ///
        ///在最佳情況下，您將創建數量有限的大廳。例如，為每個大廳創建一個大廳
        ///遊戲模式：“ koth”代表山丘之王，“ ffa”代表所有人免費，依此類推。
        ///
        ///目前沒有大廳的清單。
        ///
        /// Sql類型的大廳為隨機配對提供了不同的過濾模型。這可能會更多
        ///適合基於技能的遊戲。但是，您還需要遵循命名約定
        /// sql-lobbies中的可過濾屬性！兩者都在下面的配對文件中進行了說明。
        ///
        ///在最佳情況下，您可以讓客戶加入隨機遊戲，如下所述：
        /// https://doc.photonengine.com/zh-CN/realtime/current/reference/matchmaking-and-lobby
        ///
        ///
        ///每個房間都應在加入之前檢查房間是否已滿。光子確實列出了
        ///已滿，除非您關閉並隱藏它們（room.open = false和room.visible = false）。
        ///
        ///您可以在不加入大廳的情況下顯示遊戲的當前玩家和房間數（但您必須
        ///在主服務器上）。使用：CountOfPlayers，CountOfPlayersOnMaster，CountOfPlayersInRooms和
        /// CountOfRooms。
        ///
        ///創建新房間時，它們將“附加”到當前使用的大廳或默認大廳。
        ///
        ///您可以在沒有大廳的情況下使用JoinRandomRoom！
        /// </remarks>
        /// <param name ="typedLobby">要加入的類型化大廳（必須具有名稱和類型）。</param>
        public static bool JoinLobby(TypedLobby typedLobby)
        {
            return PhotonNetwork.JoinLobby(typedLobby);
        }

        /// <summary>離開大廳以停止獲取有關可用房間的最新信息。</summary>
        /// <remarks>
        /// ??? 這不會重置PhotonNetwork.lobby！ 這使您以後可以加入這個特定的大廳
        /// 容易地。
        ///
        ///值CountOfPlayers，CountOfPlayersOnMaster，CountOfPlayersInRooms和CountOfRooms
        ///即使沒有在大廳也能收到。
        ///
        ///您可以在沒有大廳的情況下使用JoinRandomRoom。
        /// </remarks>
        public static bool LeaveLobby()
        {
            return PhotonNetwork.LeaveLobby();
        }
        #endregion

        #region 進房間前相關
        /// <summary>
        ///加入與過濾器匹配的隨機房間。將回調：OnJoinedRoom或OnJoinRandomFailed。(★只會隨機加入目前所待的大廳的房間)
        /// </summary>
        /// <remarks>
        ///用於隨機配對。您可以加入任何房間，也可以加入在opJoinRandomRoomParams中定義的特定屬性的房間。
        ///
        ///如果沒有適合的房間或沒有可用的房間（所有房間都已滿，已關閉，在另一個大廳中或不可見），此操作將失敗。
        ///實際加入找到的房間時，它也可能會失敗。房間可能隨時關閉，變滿或空著。
        ///
        ///僅當客戶端連接到主服務器時才能調用此方法，因此您應該
        ///實現回調OnConnectedToMaster。
        ///檢查返回值，以確保將在服務器上調用該操作。
        ///注意：如果此方法返回false，則不會有回調。
        ///
        ///有關PUN對接會的更多信息：
        /// https://doc.photonengine.com/zh-CN/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby
        /// </remarks>
        public static bool JoinRandomRoom()
        {
            return PhotonNetwork.JoinRandomRoom();
        }

        /// <summary>
        ///加入與過濾器匹配的隨機房間。將回調：OnJoinedRoom或OnJoinRandomFailed。(★只會隨機加入目前所待的大廳的房間，條件也要達到)
        /// </summary>
        /// <remarks>
        ///用於隨機配對。您可以加入任何房間，也可以加入在opJoinRandomRoomParams中定義的特定屬性的房間。
        ///
        ///如果沒有適合的房間或沒有可用的房間（所有房間都已滿，已關閉，在另一個大廳中或不可見），此操作將失敗。
        ///實際加入找到的房間時，它也可能會失敗。房間可能隨時關閉，變滿或空著。
        ///
        ///僅當客戶端連接到主服務器時才能調用此方法，因此您應該
        ///實現回調OnConnectedToMaster。
        ///檢查返回值，以確保將在服務器上調用該操作。
        ///注意：如果此方法返回false，則不會有回調。
        ///
        ///有關PUN對接會的更多信息：
        /// https://doc.photonengine.com/zh-CN/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby
        /// </remarks>
        /// <param name="expectedCustomRoomProperties">篩選與這些自定義屬性（字符串鍵和值）匹配的房間。 要忽略，請傳遞null。</param>
        /// <param name="expectedMaxPlayers">過濾特定的maxplayer設置。 使用0接受任何maxPlayer值。(>=指定人數才行)</param>
        /// <returns>如果操作已排隊(queued)，將被發送。</returns>
        public static bool JoinRandomRoom(ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers)
        {
            return PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers);
        }

        /// <summary>
        ///加入與過濾器匹配的隨機房間。將回調：OnJoinedRoom或OnJoinRandomFailed。(★只會隨機加入目前所待的大廳的房間，條件也要達到)
        /// </summary>
        /// <remarks>
        ///用於隨機配對。您可以加入任何房間，也可以加入在opJoinRandomRoomParams中定義的特定屬性的房間。
        ///
        ///如果沒有適合的房間或沒有可用的房間（所有房間都已滿，已關閉，在另一個大廳中或不可見），此操作將失敗。
        ///實際加入找到的房間時，它也可能會失敗。房間可能隨時關閉，變滿或空著。
        ///
        ///僅當客戶端連接到主服務器時才能調用此方法，因此您應該
        ///實現回調OnConnectedToMaster。
        ///檢查返回值，以確保將在服務器上調用該操作。
        ///注意：如果此方法返回false，則不會有回調。
        ///
        ///有關PUN對接會的更多信息：
        /// https://doc.photonengine.com/zh-CN/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby
        /// </remarks>
        /// <param name =“ expectedCustomRoomProperties”>針對與這些自定義屬性（字符串鍵和值）匹配的房間的過濾器。要忽略，請傳遞null。</param>
        /// <param name =“ expectedMaxPlayers”>用於特定maxplayer設置的過濾器。使用0接受任何maxPlayer值。</param>
        /// <param name =“ matchingType”>選擇一種可用的配對算法。有關選項，MatchmakingMode:FillRoom(填滿房間（最舊的房間），以盡可能快地將玩家召集在一起。 默認。),SerialMatching(平均分配玩家至房間),RandomMatching(條件達到但隨機分配)。</param>
        /// <param name =“ typedLobby”>您要在其中查找房間的大廳。傳遞null，以使用默認大廳。這不會加入該大廳，也不會設置該大廳的屬性。</param>
        /// <param name =“ sqlLobbyFilter”> SQL類型的大廳的過濾字符串。</ param>
        /// <param name =“ expectedUsers”>要加入此遊戲且要為其屏蔽位置的用戶的可選列表（按UserId列出）(必定要有的玩家)。</param>
        /// <returns>如果操作已排隊並且將被發送。</returns>
        public static bool JoinRandomRoom(ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, MatchmakingMode matchingType, TypedLobby typedLobby, string sqlLobbyFilter, string[] expectedUsers = null)
        {
            return PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, matchingType, typedLobby, sqlLobbyFilter, expectedUsers);
        }


        /// <summary>
        ///創建一個新房間。將回調：OnCreatedRoom和OnJoinedRoom或OnCreateRoomFailed。(★所有大廳的房間名稱是唯一)
        /// </summary>
        /// <remarks>
        ///成功後，將調用回調OnCreatedRoom和OnJoinedRoom（後者會導致您以第一個玩家的身份加入）。
        ///在所有錯誤情況下，都會調用OnCreateRoomFailed。
        ///
        ///如果會議室名稱已被使用或RoomOptions衝突，則創建會議室將失敗
        ///彼此。檢查EnterRoomParams參考以獲取各種房間創建選項。
        ///
        ///如果不想創建唯一的房間名稱，請傳遞null或“”作為名稱，服務器將分配一個roomName（GUID作為字符串）。
        ///
        ///僅當客戶端連接到主服務器時才能調用此方法，因此您應該
        ///實現回調OnConnectedToMaster。
        ///檢查返回值，以確保將在服務器上調用該操作。
        ///注意：如果此方法返回false，則不會有回調。
        ///
        ///有關PUN對接會的更多信息：
        /// https://doc.photonengine.com/zh-CN/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby
        /// </remarks>
        /// <param name = "roomName">要創建的房間的唯一名稱。傳遞null或以使服務器生成名稱。(★相同PhotonCloud雲區域與PhotonNetwork.GameVersion裡的唯一房間名)</param>
        ///  /// <param name ="roomOptions">房間的通用選項，例如MaxPlayers，初始自定義房間屬性等。請參閱RoomOptions類型。.</param>
        /// <param name ="typedLobby">如果為null，則會在當前使用的大廳中自動創建房間（如果您未明確加入，則為“默認”）。</param>
        /// <param name ="expectedUsers">要加入此遊戲且要為其屏蔽位置的用戶的可選列表（按UserId列出）(幫人保留位置，無法重複ID進入)(★★★指定加入房間，其餘方式會失敗)。</param>
        /// <returns>如果操作已排隊並且將被發送。</returns>
        public static bool CreateRoom(string roomName, RoomOptions roomOptions = null, TypedLobby typedLobby = null, string[] expectedUsers = null)
        {
            return PhotonNetwork.CreateRoom(roomName, roomOptions, typedLobby, expectedUsers);
        }



        /// <summary>
        ///通過名稱加入特定的房間並根據需要創建它。將會回調：OnJoinedRoom或OnJoinRoomFailed。(★不同大廳也可以，名稱是唯一，相同即加入)
        /// </summary>
        /// <remarks>
        ///當玩家組成一個房間名稱來開會時很有用：
        ///所有涉及的客戶都調用相同的方法，無論誰先行，都會創建會議室。
        ///
        ///成功後，客戶端將進入指定的房間。
        ///創建房間的客戶端將同時回調OnCreatedRoom和OnJoinedRoom。
        ///加入現有會議室的客戶端將僅回調OnJoinedRoom。
        ///在所有錯誤情況下，都會調用OnJoinRoomFailed。
        ///
        ///如果會議室已滿，關閉或用戶進入會議室，都會失敗
        ///已經存在於房間中（由userId選中）。
        ///
        ///要返回房間，請使用OpRejoinRoom。
        ///
        ///僅當客戶端連接到主服務器時才能調用此方法，因此您應該
        ///實現回調OnConnectedToMaster。
        ///檢查返回值，以確保將在服務器上調用該操作。
        ///注意：如果此方法返回false，則不會有回調。
        ///
        ///
        ///如果您在roomOptions中設置房間屬性，則在房間已經存在時將被忽略。
        ///這避免了由於後期加入玩家而改變房間的屬性。
        ///
        ///您可以定義一個ExpectedUsers數組，以阻止這些用戶在房間中的播放器插槽。
        /// Photon中的相應功能稱為“插槽預留”，可以在文檔頁面中找到。
        ///
        ///
        ///有關PUN對接會的更多信息：
        /// https://doc.photonengine.com/zh-CN/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby
        /// </remarks>
        /// <param name ="roomName">要加入的會議室的名稱。必須為非null。</param>
        /// <param name ="roomOptions">房間的選項（如果尚不存在）。否則，這些值將被忽略。</param>
        /// <param name ="typedLobby">您要在其中列出新房間的大廳。忽略該房間是否存在並已加入。</param>
        /// <param name ="expectedUsers">要加入此遊戲且要為其屏蔽位置的用戶的可選列表（按UserId列出）(必定要有的玩家)。</param>
        /// <returns>如果操作已排隊並且將被發送。</returns>
        public static bool JoinOrCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby, string[] expectedUsers = null)
        {
            return PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, typedLobby, expectedUsers);
        }

        /// <summary>
        ///通過名稱加入一個房間。將會回調：OnJoinedRoom或OnJoinRoomFailed。(★不同大廳也可以，名稱是唯一，相同即加入)
        /// </summary>
        /// <remarks>
        ///在使用大廳或玩家跟隨朋友或互相邀請時很有用。
        ///
        ///成功後，客戶端將進入指定的房間並通過OnJoinedRoom進行回調。
        ///在所有錯誤情況下，都會調用OnJoinRoomFailed。
        ///
        ///如果會議室已滿，關閉，不存在或用戶進入會議室，則將失敗
        ///已經存在於房間中（由userId選中）。
        ///
        ///要返回房間，請使用OpRejoinRoom。
        ///當玩家互相邀請並且不清楚誰先回應時，請改用OpJoinOrCreateRoom。
        ///
        ///僅當客戶端連接到主服務器時才能調用此方法，因此您應該
        ///實現回調OnConnectedToMaster。
        ///檢查返回值，以確保將在服務器上調用該操作。
        ///注意：如果此方法返回false，則不會有回調。
        ///
        ///
        ///有關PUN對接會的更多信息：
        /// https://doc.photonengine.com/zh-CN/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby
        /// </remarks>
        /// 請參閱OnJoinRoomFailed
        /// OnJoinedRoom
        /// <param name ="roomName">要加入的會議室的唯一名稱。</param>
        /// <param name ="expectedUsers">要加入此遊戲且要為其屏蔽位置的用戶的可選列表（按UserId列出）(必定要有的玩家)。</param>
        /// <returns>如果操作已排隊並且將被發送。</returns>
        public static bool JoinRoom(string roomName, string[] expectedUsers = null)
        {
            return PhotonNetwork.JoinRoom(roomName, expectedUsers);
        }


        /// <summary>
        ///通過roomName重新加入房間（內部使用userID返回）。將會回調：OnJoinedRoom或OnJoinRoomFailed。
        /// </summary>
        /// <remarks>
        ///斷開連接後，您也許可以返回房間繼續玩遊戲，
        ///如果客戶端重新連接的速度足夠快。使用Reconnect（）和此方法。
        ///緩存您所在的房間名稱，然後使用RejoinRoom（roomname）返回游戲。
        ///
        ///注意：要能夠重新加入任何房間，您需要使用UserID！
        ///您還需要設置RoomOptions.PlayerTtl(玩家離開後保留時間)。
        ///
        ///重要提示：尚不支持Instantiate（）和RPC的使用。
        ///如果使用PhotonViews，則PhotonViews的所有權規則會阻止無縫返回游戲。
        ///使用自定義屬性和RaiseEvent進行事件緩存。
        ///
        ///常見用例：按下iOS設備上的“鎖定”按鈕，您將立即斷開連接。
        ///
        ///重新加入房間不會發送任何玩家屬性。相反，客戶端將從服務器接收最新消息。
        ///如果要設置新的播放器屬性，請在重新加入後進行設置。
        /// </remarks>
        public static bool RejoinRoom(string roomName)
        {
            return PhotonNetwork.RejoinRoom(roomName);
        }


        /// <summary>當客戶端在玩遊戲時失去連接時，此方法會嘗試重新連接並重新加入房間。</summary>
        /// <remarks>
        ///此方法直接重新連接到託管PUN之前所在房間的遊戲服務器。
        ///如果同時關閉了會議室，則PUN將調用OnJoinRoomFailed並將此客戶端返回到主服務器。
        ///
        ///檢查返回值，如果此客戶端將嘗試重新連接並重新加入（如果滿足條件）。
        ///如果ReconnectAndRejoin返回false，您仍然可以嘗試重新連接並重新加入。
        ///
        ///與PhotonNetwork.RejoinRoom相似，這要求您為每個播放器使用唯一的ID（UserID）。
        ///
        ///重新加入房間不會發送任何玩家屬性。相反，客戶端將從服務器接收最新消息。
        ///如果要設置新的播放器屬性，請在重新加入後進行設置。
        /// </remarks>
        /// <returns> False，如果沒有已知的房間或遊戲服務器要返回。然後，此客戶端不會嘗試ReconnectAndRejoin。</returns>
        public static bool ReconnectAndRejoin()
        {
            return PhotonNetwork.ReconnectAndRejoin();
        }
        #endregion

        #region 其他
        /// <summary>
        /// ????
        ///定義PhotonHandler每秒發送多少次數據（如果有排隊）。默認值：30
        /// </summary>
        /// <remarks>
        ///此值定義PUN多久調用一次低級PhotonPeer來放入排隊的傳出消息
        ///放入要發送的數據報中。這是在PhotonHandler組件中實現的，該組件集成了PUN
        ///進入Unity遊戲循環。
        /// PhotonHandler.MaxDatagrams值定義在一次迭代中可以發送多少個數據報。
        ///
        ///此值不影響PhotonViews寫入更新的頻率。那是由
        /// SerializationRate。為避免PhotonView更新的發送延遲，PUN還將在最後發送數據
        ///在OnPhotonSerializeView中寫入數據的幀，因此發送實際上可能比
        /// SendRate。
        ///
        ///由於RPC和RaiseEvent而排隊的消息將至少以SendRate頻率發送。他們
        ///包含在內，當OnPhotonSerialize編寫更新並觸發提前發送時。
        ///
        ///設置此值不會再調整SerializationRate（從PUN 2.24開始）。
        ///
        ///減少發送頻率將把消息聚合為數據報，從而避免了網絡開銷。
        ///同樣重要的是，每幀不要推送太多數據報。三到五個似乎是最佳選擇。
        ///
        ///請記住您的目標平台：移動網絡通常速度較慢。
        /// WiFi速度較慢，變化較大，且突發性突發。
        ///
        ///低幀率（如在Update調用中）會影響消息的發送。
        /// </remarks>
        public static int SendRate
        {
            get
            {
                return PhotonNetwork.SendRate;
            }

            set
            {
                PhotonNetwork.SendRate = value;
            }
        }
        /// <summary>
        ///定義每秒在PhotonViews上為受控對象調用OnPhotonSerialize多少次。
        /// </summary>
        /// <remarks>
        ///此值定義PUN在受控網絡對像上調用OnPhotonSerialize的頻率。
        ///這是在PhotonHandler組件中實現的，該組件將PUN集成到Unity遊戲循環中。
        ///
        ///用OnPhotonSerialize編寫的更新將暫時排隊，並在下一個LateUpdate中發送，
        ///因此，較高的SerializationRate也將導致更多發送。這樣做的目的是使延遲時間短
        ///哪些書面更新已排隊。
        ///
        ///調用RPC不會觸發發送。
        ///
        ///低幀速率將影響寫入更新的頻率以及更新的“準時”程度。
        ///
        ///較低的速率佔用較少的性能，但接收端需要插補較長的時間
        ///更新之間。
        /// </remarks>
        public static int SerializationRate
        {
            get
            {
                return PhotonNetwork.SerializationRate;
            }

            set
            {
                PhotonNetwork.SerializationRate = value;
            }
        }

        /// <summary>
        /// ????
        ///可用於暫停傳入事件（RPC，實例化和其他傳入事件）的分派。
        /// </summary>
        /// <remarks>
        ///當IsMessageQueueRunning == false時，OnPhotonSerializeView調用不會完成，並且不會發送任何內容
        /// 一個客戶。 同樣，傳入消息將排隊，直到您重新激活消息隊列。
        ///
        ///如果您首先要加載一個關卡，然後繼續接收PhotonViews和RPC的數據，則這很有用。
        ///客戶端將繼續接收和發送傳入包和RPC /事件的確認。
        ///這會增加“滯後”，並且在暫停時間較長時會引起問題，因為所有傳入消息都已排隊。
        /// </remarks>
        public static bool IsMessageQueueRunning
        {
            get
            {
                return PhotonNetwork.IsMessageQueueRunning;
            }

            set
            {
                PhotonNetwork.IsMessageQueueRunning = value;
            }
        }

        /// <summary>
        /// ??????
        ///重複的命令計數（由於在收到ACK之前的本地重複計時）。
        /// </summary>
        /// <remarks>
        ///如果此值增加很多，則很可能由於情況不好而發生超時斷開連接。
        /// </remarks>
        public static int ResentReliableCommands
        {
            get { return PhotonNetwork.ResentReliableCommands; }
        }

        /// <summary>????? Crc檢查對於檢測和避免數據報損壞的問題很有用。 未連接時可以啟用。</summary>
        public static bool CrcCheckEnabled
        {
            get { return PhotonNetwork.CrcCheckEnabled; }
            set
            {
                PhotonNetwork.CrcCheckEnabled = value;
            }
        }

        /// <summary>???? 如果為CrcCheckEnabled，則對沒有有效CRC校驗和並被拒絕的傳入數據包進行計數。</summary>
        public static int PacketLossByCrcCheck
        {
            get { return PhotonNetwork.PacketLossByCrcCheck; }
        }

        /// <summary>???? 定義在未獲得ACK之前可以重新發送可靠消息的次數，因為它會觸發斷開連接。 默認值：5。</summary>
        /// <remarks>重發次數少意味著斷開連接更快，而更多的事務會導致更大的延遲而無濟於事。 最低：3。最高：10。</remarks>
        public static int MaxResendsBeforeDisconnect
        {
            get { return PhotonNetwork.MaxResendsBeforeDisconnect; }
            set
            {
                if (value < 3) value = 3;
                if (value > 10) value = 10;
                PhotonNetwork.MaxResendsBeforeDisconnect = value;
            }
        }

        /// <summary>???? 在網絡中斷的情況下，可靠的消息最多可以快速重複3次。</summary>
        /// <remarks>
        ///當可靠的消息丟失不止一次時，後續的重複操作會延遲一點
        ///以便網絡恢復。
        ///使用此選項，可以加快重複2和3的速度。 這可以幫助避免超時，但是
        ///也會提高縫隙閉合的速度。<br/>
        ///設置此項時，將PhotonNetwork.MaxResendsBeforeDisconnect增加到6或7。
        /// </remarks>
        public static int QuickResends
        {
            get { return PhotonNetwork.QuickResends; }
            set
            {
                PhotonNetwork.QuickResends = value;
            }
        }

        /// <summary>???? 定義服務器端口的替代。 如果> 0，則按服務器類型使用。重要說明：如果更改傳輸協議，也請調整替代。</summary>
        /// 請參閱LoadBalancingClient.ServerPortOverrides
        public static PhotonPortDefinition ServerPortOverrides
        {
            get { return PhotonNetwork.ServerPortOverrides; }
            set { PhotonNetwork.ServerPortOverrides = value; }
        }

        /// <summary>
        /// ????
        ///定義在Unity的OnApplicationPause（true）調用之後PUN保持連接多少秒。默認值：60秒。
        /// </summary>
        /// <remarks>
        ///最好的做法是在一段時間後斷開不活動的應用程序/連接，但也允許用戶接聽電話等。
        ///我們認為合理的後台超時為60秒。
        ///
        ///要處理超時，請照常實現：OnDisconnected（）。
        ///您的應用程序將在再次變為活動狀態（運行Update（）循環）時“通知”後台斷開連接。
        ///
        ///如果您需要將此案例與其他案例分開，則需要跟踪該應用是否在後台
        ///（PUN沒有特殊的回調）。
        ///
        ///
        ///信息：
        ///即使Unity不定期調用Update（），PUN仍在運行“後備線程”以將ACK發送到服務器。
        ///這有助於在加載場景和資產時以及應用程序在後台時保持連接。
        ///
        /// 筆記：
        ///某些平台（例如iOS）不允許在後台運行應用程序時保持連接。
        ///在這種情況下，此值不會更改任何內容，應用程序會立即在後台斷開連接。
        ///
        ///在某些Unity版本的某些導出（Android）中，Unity的OnApplicationPause（）回調已損壞。
        ///確保OnApplicationPause（）在目標平台上獲得了您期望的回調！
        ///檢查PhotonHandler.OnApplicationPause（布爾暫停(bool pause)）以查看實現。
        /// </ remarks>
        public static float KeepAliveInBackground
        {
            set
            {
                PhotonNetwork.KeepAliveInBackground = value;
            }

            get { return PhotonNetwork.KeepAliveInBackground; }
        }

        /// <summary>???? 影響PhotonHandler是在LateUpdate還是FixedUpdate（默認）中調度傳入消息。</summary>
        /// <remarks>
        ///默認情況下，PhotonHandler組件在FixedUpdate中分派傳入的消息。
        ///
        ///當Time.timeScale較低時，FixedUpdate的調用頻率會降低，直到更新可能被暫停為止。
        /// PUN可以在LateUpdate中自動為低timeScale值（當Time.timeScale低於此值時）分派消息。
        ///
        /// PUN將使用FixedUpdate或LateUpdate，但不能同時使用（從v2.23開始）。
        ///
        ///當您使用此值時，請注意，實例化和RPC在幀內以更改的時序執行。
        ///如果從FixedUpdate調用了Instantiate，則物理引擎似乎在實例化對象運行Start（）之前先對實例化對象運行。
        ///
        ///默認情況下，此值為-1f，因此不會回退到LateUpdate。
        /// </remarks>
        public static float MinimalTimeScaleToDispatchInFixedUpdate
        {
            set
            {
                PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = value;
            }

            get { return PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate; }
        }
        #endregion
    }
}
