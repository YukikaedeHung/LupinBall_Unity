using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Tim
{
    /*Room*/
    public static partial class MyPhotonNetwork
    {
        #region 房間內
        /// <summary>是否在房間內 (NetworkClientState == ClientState.Joined).</summary>
        public static bool InRoom
        {
            get
            {
                //在離線模式下，您也可以在一個房間中，然後NetworkClientState會像在在線模式下一樣返回Joined！
                return PhotonNetwork.InRoom;
            }
        }

        /// <summary>
        /// 獲取我們當前所在的房間。 如果我們不在任何房間，則為空。
        /// </summary>
        public static Room CurrentRoom
        {
            get
            {
                return PhotonNetwork.CurrentRoom;
            }
        }

        /// <summary>
        /// 當前房間的玩家列表的排序副本。 這是使用Linq，因此更好地緩存此值。 玩家加入/離開時更新。
        /// </summary>
        public static Player[] PlayerList
        {
            get
            {
                return PhotonNetwork.PlayerList;
            }
        }
        /// <summary>
        /// 當前房間（不包括此客戶）的玩家列表的排序副本。 這是使用Linq的模式所以當玩家加入/離開更新時，最好緩存此值。
        /// </summary>
        public static Player[] PlayerListOthers
        {
            get
            {
                return PhotonNetwork.PlayerListOthers; ;
            }
        }

        /// <summary>
        /// ???????????  
        /// 可以將脫機模式(離線模式)設置為在單人遊戲模式中重用您的多人代碼。
        /// 啟用此功能後，PhotonNetwork不會創建任何連接
        /// 沒有開銷。 對於重用RPC和PhotonNetwork最有用。
        /// </summary>
        public static bool OfflineMode
        {
            get
            {
                return PhotonNetwork.OfflineMode;
            }

            set
            {
                PhotonNetwork.OfflineMode = value;
            }
        }

        /// <summary>
        /// 自己是否為 master client?
        /// </summary>
        public static bool IsMasterClient
        {
            get
            {
                return PhotonNetwork.IsMasterClient;
            }
        }
        /// <summary>
        ///當前房間的主客戶，或者為null（房間外）。
        /// </summary>
        /// <remarks>
        ///可以用作“權威(authoritative)”客戶端/玩家進行決策，運行AI或其他操作。
        ///
        ///如果當前的主客戶端離開會議室（離開/斷開連接），則服務器將快速分配其他人。
        ///如果當前的主客戶端超時（關閉的應用程序，連接斷開等），則發送給該客戶端的消息為
        ///實際上為其他人迷失了！ 在沒有主客戶端處於活動狀態的情況下，超時可能需要10秒鐘。
        ///
        ///實現在主客戶端切換時調用的IPunCallbacks.OnMasterClientSwitched方法。
        ///
        ///使用PhotonNetwork.SetMasterClient手動切換到其他播放器/客戶端。
        ///
        ///如果使用OfflineMode == true，則始終返回PhotonNetwork.player。
        /// </remarks>
        public static Player MasterClient
        {
            get
            {
                return PhotonNetwork.MasterClient;
            }
        }

        /// <summary>
        ///要求服務器將另一位玩家分配為您當前房間的主客戶端。
        /// </summary>
        /// <remarks>
        /// RPC和RaiseEvent可以選擇僅將消息發送到房間的主客戶端。
        /// SetMasterClient影響哪個客戶端獲取這些消息。
        ///
        ///此方法在服務器上調用一個操作來設置新的Master Client，這需要往返。
        ///如果成功，則此客戶端和其他客戶端從服務器獲取新的Master Client。
        ///
        /// SetMasterClient告訴服務器應使用新的Master Client替換當前的Master Client。
        ///如果任何事情在更早的時候切換了主客戶端，它將失敗。沒有為此的回調
        /// 錯誤。無論如何，所有客戶端都應獲取服務器分配的新的主客戶端。
        ///
        ///另請參見：PhotonNetwork.MasterClient
        ///
        ///在v3服務器上：
        /// ReceiverGroup.MasterClient（可在RPC中使用）不受此影響（仍然指向房間中最低的player.ID）。
        ///避免使用此枚舉值（而是發送給特定的播放器）。
        ///
        ///如果當前的主客戶端離開，則PUN將通過“最低玩家ID”檢測到一個新的客戶端。實施OnMasterClientSwitched
        ///在這種情況下獲取回調。 PUN選擇的主客戶端可能會分配一個新的。
        ///
        ///確保您沒有創建一個無限的主分配循環！選擇自定義主客戶端時，所有客戶端
        ///應該指向同一個玩家，無論實際上是誰分配了該玩家。
        ///
        ///在本地，主客戶端會立即切換，而遠程客戶端會收到事件。這意味著遊戲
        ///暫時沒有Master Client，例如當前的Master Client離開時。
        ///
        ///手動切換Master Client時，請記住，該用戶可能會離開而無法完成工作，就像
        ///任何主客戶端。
        ///
        /// </remarks>
        /// <param name="masterClientPlayer">The player to become the next Master Client.</param>
        /// <returns>無法執行此操作時為False。 必須在一個房間中（不在OfflineMode中）。</returns>
        public static bool SetMasterClient(Player masterClientPlayer)
        {
            return PhotonNetwork.SetMasterClient(masterClientPlayer);
        }

        /// <summary>請求客戶端斷開連接（踢）。 只有主客戶端才能做到這一點</summary>
        /// <remarks>只有目標玩家獲得此事件。 該播放器將自動斷開連接，這也是其他播放器也會注意到的。</remarks>
        /// <param name="kickPlayer">The Player to kick.</param>
        public static bool CloseConnection(Player kickPlayer)
        {
            return PhotonNetwork.CloseConnection(kickPlayer);
        }

        /// <summary>離開當前會議室並返回到主服務器，您可以在其中加入或創建會議室（請參見備註）。</summary>
        /// <remarks>
        ///這將使用PhotonView清理所有（網絡）遊戲對象，除非您將autoCleanUp更改為false。
        ///返回到主服務器。
        ///
        ///在OfflineMode中，將清理本地“假”房間，並立即調用OnLeftRoom。
        ///
        ///在一個玩家TTL＆lt;的房間裡0，LeaveRoom只是使客戶端處於非活動狀態。玩家停留在房間的玩家列表中
        ///並可以稍後返回。故意將“設置為非活動”設置為“假”，這意味著“放棄”房間，儘管
        /// playerTTL允許您返回。
        ///
        ///在玩家TTL == 0的房間中，變為非活動狀態無效（客戶端會立即從房間中移出）。
        /// </remarks>
        /// <param name ="becomeInactive">???? 如果此客戶端在玩家TTL＆lt; 0。默認為true。</param>
        public static bool LeaveRoom(bool becomeInactive = true)
        {
            return PhotonNetwork.LeaveRoom(becomeInactive);
        }
        #endregion
    }
}
