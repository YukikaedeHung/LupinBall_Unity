using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

namespace Tim
{
    public static partial class MyPhotonNetwork
    {
        #region RaiseEvent(註冊使用)
        /// <summary>
        ///在一個房間中發送完全可定制的事件。事件至少包含一個EventCode（0..199），並且可以包含內容。
        /// </summary>
        /// <remarks>
        ///要接收事件，請在任何類中實現IOnEventCallback並通過PhotonNetwork.AddCallbackTarget對其進行註冊。
        ///參見IOnEventCallback.OnEvent。
        ///
        /// eventContent是可選的。如果設置，則eventContent必須為“可序列化類型”，
        ///客戶端基本上可以變成一個byte []。支持大多數基本類型和數組，包括
        /// Unity的Vector2，Vector3，四元數。不支持轉換。
        ///
        ///您可以按照CustomTypes.cs中的示例將類轉換為“可序列化的類型”。
        ///
        /// RaiseEventOptions具有一些（不太直觀）的組合規則：
        ///如果設置了targetActors（Player.ID值的數組），那麼接收器參數將被忽略。
        ///使用事件緩存時，不能使用targetActors，receiver和interestGroup。緩衝事件歸所有人所有。
        ///當使用cachingOption removeFromRoomCache時，eventCode和內容實際上不發送，而是用作過濾器。
        /// </remarks>
        /// <param name ="eventCode">(訊息內容傳遞編號)標識事件類型的字節。您可能希望對每個操作使用代碼，或者表示可以預期的內容。允許：0..199。</param>
        /// <param name ="eventContent">(範例為 帶入資料 + 執行單位編號(有創意點可自訂))( 例如 : new object[] { new Vector3(10.0f, 2.0f, 5.0f), 1, 2, 5,300 };)一些可序列化的對象，例如字符串，字節，整數，浮點數（等）及其數組。帶有字節鍵的哈希表很適合發送可變內容。</param>
        /// <param name ="raiseEventOptions">允許更複雜地使用事件。如果為null，將使用RaiseEventOptions.Default。
        /// Photon.Realtime.RaiseEventOptions
        /// - Others：所有其他活躍演員都加入了同一個房間。
        /// - All：所有活躍演員都加入了同一個房間，包括發件人。
        /// - MasterClient：當前指定的主客戶端在房間內。</param>
        /// <param name ="sendOptions">發送可靠，加密等選項。</param>
        /// <returns>如果無法發送事件，則為false。</returns>
        public static bool RaiseEvent(byte eventCode, object eventContent, RaiseEventOptions raiseEventOptions, ExitGames.Client.Photon.SendOptions sendOptions)
        {
            return PhotonNetwork.RaiseEvent(eventCode, eventContent, raiseEventOptions, sendOptions);
        }
        /// <summary>
        ///為已實現的回調接口註冊用於回調的對象。(★PhotonView.AddCallbackTarget(個別)與PhotonNerworking.AddCallbackTarget(全局)用的地方不同，切記。)
        /// </summary>
        /// <remarks>
        ///涵蓋的回調接口是：IConnectionCallbacks，IMatchmakingCallbacks，
        /// ILobbyCallbacks，IInRoomCallbacks，IOnEventCallback和IWebRpcCallback。
        ///
        ///參見："https://doc.photonengine.com/en-us/pun/v2/getting-started/dotnet-callbacks .Net回調
        /// </remarks>
        /// <param name ="target">註冊以從PUN的LoadBalancingClient獲取回調的對象。</param>
        public static void AddCallbackTarget(object target)
        {
            PhotonNetwork.AddCallbackTarget(target);
        }
        /// <summary>
        ///從已實現的回調接口的回調中刪除目標對象。
        /// </summary>
        /// <remarks>
        ///涵蓋的回調接口是：IConnectionCallbacks，IMatchmakingCallbacks，
        /// ILobbyCallbacks，IInRoomCallbacks，IOnEventCallback和IWebRpcCallback。
        ///
        ///參見："https://doc.photonengine.com/en-us/pun/v2/getting-started/dotnet-callbacks" .Net回調
        /// </remarks>
        /// <param name ="target">從獲取回調中取消註冊的對象。</param>
        public static void RemoveCallbackTarget(object target)
        {
            PhotonNetwork.RemoveCallbackTarget(target);
        }
        #endregion

        #region 其他
        /// <summary>
        /// 使用須小心 localOnly-true:只有自己畫面自己的物件消失，別人畫面沒有，之後會發生ViewID重複的錯誤，false:自己與其他人畫面所有東西全部刪除，不是Master也能使用。
        /// </summary>
        /// <param name="localOnly"></param>
        public static void DestroyAll(bool localOnly)
        {
            PhotonNetwork.DestroyAll(localOnly);
        }

        /// <summary>使用特定類型的組件查找遊戲對象（使用FindObjectsOfType）例如 : FindGameObjectsWithComponent(typeof( Photon.Pun.PhotonView));。</summary>
        /// <param name ="type">類型必須為組件</param>
        /// <returns>帶有具有特定類型組件的GameObject的哈希集。</returns>
        public static HashSet<GameObject> FindGameObjectsWithComponent(Type type)
        {
            return PhotonNetwork.FindGameObjectsWithComponent(type); ;
        }

        public static void LoadOrCreateSettings()
        {
            PhotonNetwork.LoadOrCreateSettings();
        }

        /// <summary>
        /// ??? 定義將OnPhotonSerialize（）產生的更新數量批處理為一條消息。
        /// </summary>
        /// <remarks>
        ///數值低會增加開銷，數值高可能會導致消息碎片化。
        /// </remarks>
        public static int ObjectsInOneUpdate
        {
            get { return PhotonNetwork.ObjectsInOneUpdate; }
            set { PhotonNetwork.ObjectsInOneUpdate = value; }
        }

        // compresses currentContent by using NULL as value if currentContent equals previousContent
        // skips initial indexes, as defined by SyncFirstValue
        // to conserve memory, the previousContent is re-used as buffer for the result! duplicate the values before using this, if needed
        // returns null, if nothing must be sent (current content might be null, which also returns null)
        // SyncFirstValue should be the index of the first actual data-value (3 in PUN's case, as 0=viewId, 1=(bool)compressed, 2=(int[])values that are now null)
        //如果currentContent等於previousContent，則使用NULL作為值壓縮currentContent
        //跳過由SyncFirstValue定義的初始索引
        //為了節省內存，將previousContent再次用作結果緩衝區！ 如果需要，請在使用此值之前複製這些值
        //如果不發送任何內容，則返回null（當前內容可能為null，也將返回null）
        // SyncFirstValue應該是第一個實際數據值的索引（在PUN的情況下為3，因為0 = viewId，1 =（bool）壓縮，2 =（int []）現在為空的值）
        public const int SyncViewId = 0;
        public const int SyncCompressed = 1;
        public const int SyncNullValues = 2;
        public const int SyncFirstValue = 3;
        #endregion

        #region UNITY_EDITOR(Unity編輯器底下才有作用)

#if UNITY_EDITOR

        /// <summary>
        /// 根據名稱或搜索查詢查找資產路徑：https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html
        /// </summary>
        /// <returns>資產Assets路徑。</returns>
        /// <param name ="asset">資產(Assets)。</param>
        public static string FindAssetPath(string asset)
        {
            return PhotonNetwork.FindAssetPath(asset);
        }

        /// <summary>
        ///查找雙關語(pun)資產(asset)文件夾。 Assets/Photon Unity Networking/Resources/
        /// </summary>
        /// <returns>雙關語(pun)資產(asset)文件夾。</returns>
        public static string FindPunAssetFolder()
        {
            return PhotonNetwork.FindPunAssetFolder();
        }

        /// <summary>
        /// ??? 由編輯器腳本內部使用，在更改層次結構（包括場景保存）時調用，以刪除多餘的隱藏PhotonHandlers。
        /// </summary>
        /// <remarks>這是在此類中完成的，因為Editor程序集無法訪問PhotonHandler。</remarks>
        public static void InternalCleanPhotonMonoFromSceneIfStuck()
        {
            PhotonNetwork.InternalCleanPhotonMonoFromSceneIfStuck();
        }

#endif

        #endregion
    }
}
