using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using ExitGames.Client.Photon;

namespace Tim
{
    /*Game相關*/
    public static partial class MyPhotonNetwork
    {
        #region PhotonView 光子視圖
        /// <summary>
        ///獲取光子視圖(photon views)。
        /// </summary>
        /// <remarks>
        ///這是一個昂貴的操作，因為它返回內部列表的副本。
        /// </remarks>
        /// <value>光子視圖(photon views)。</value>
        [System.Obsolete(
            "Use PhotonViewCollection instead for an iterable collection of current photonViews." +
            "(使用PhotonViewCollection代替當前photonViews的可迭代集合。)"
        )]
        public static PhotonView[] PhotonViews
        {
            get
            {
                return PhotonNetwork.PhotonViews;
            }
        }
        /// <summary>
        ///返回當前光子視圖的新的可迭代集合。
        /// </summary>
        /// <remarks>
        ///您可以在一個簡單的foreach循環中遍歷所有PhotonView。
        ///要在while循環中使用此函數，請將新的迭代器分配給變量，然後在該變量上調用MoveNext。
        /// </remarks>
        public static NonAllocDictionary<int, PhotonView>.ValueIterator PhotonViewCollection
        {
            get
            {
                return PhotonNetwork.PhotonViewCollection;
            }
        }
        public static int ViewCount
        {
            get { return PhotonNetwork.ViewCount; }
        }

        /// <summary>
        /// ???
        /// 為當前/本地播放器分配一個viewID。
        /// </summary>
        /// <returns>如果已分配viewId，則為true。 如果PhotonView已經具有非零的viewID，則為False。</returns>
        public static bool AllocateViewID(PhotonView view)
        {
            return AllocateViewID(view);
        }

        [System.Obsolete("已過時 需要Renamed時. Use PhotonNetwork.AllocateRoomViewID instead")]
        public static bool AllocateSceneViewID(PhotonView view)
        {
            return AllocateRoomViewID(view);
        }
        /// <summary>
        /// ???
        /// 允許主客戶端為房間對象分配一個viewID。
        /// </summary>
        /// <returns>如果已分配viewId，則為true。 如果PhotonView已經具有非零的viewID或此客戶端不是主客戶端，則為False。</returns>
        public static bool AllocateRoomViewID(PhotonView view)
        {
            return PhotonNetwork.AllocateRoomViewID(view);
        }

        /// <summary>??? 為當前/本地玩家或房間分配一個viewID。</summary>
        /// <param name="roomObject">使用true來分配房間viewID，使用false來為本地播放器分配viewID。</param>
        /// <returns>返回一個可以分配為PhotonView.ViewID的viewID（所有者和序列號的組合）。</returns>
        public static int AllocateViewID(bool roomObject)
        {
            return PhotonNetwork.AllocateViewID(roomObject);
        }

        /// <summary>??? 為當前/本地玩家或房間分配一個viewID。</summary>
        /// <param name="ownerId">要為其分配viewID的ActorNumber(每位玩家的進房編號)。</param>
        /// <returns>返回一個可以分配為PhotonView.ViewID的viewID（所有者和序列號的組合）。</returns>
        public static int AllocateViewID(int ownerId)
        {
            return PhotonNetwork.AllocateViewID(ownerId);
        }

        /// <summary>
        /// ??? 為稍後實例化的PhotonView設置級別前綴。 如果只需要一個就不要設置！
        /// </summary>
        /// <remarks>
        ///重要提示：如果您不使用多個級別前綴，則只需不設置此值。 這
        ///從流量中優化了默認值。
        ///
        ///這不會影響現有的PhotonView（對於現有的PhotonView，還不能更改它們）。
        ///
        ///將接收但不執行以不同級別前綴發送的消息。 這影響
        /// RPC，實例化和同步。
        ///
        ///請注意，PUN永遠不會重置此值，您必須自己重置。
        /// </remarks>
        /// <param name ="prefix">最大值很短。MaxValue= 255 </param>
        public static void SetLevelPrefix(byte prefix)
        {
            PhotonNetwork.SetLevelPrefix(prefix);
        }

        /// <summary>啟用/禁用來自給定興趣組的事件。</summary>
        /// <remarks>
        ///客戶端可以告訴服務器它感興趣的興趣組。
        ///服務器將僅將那些興趣組的事件轉發到該客戶端（節省帶寬和性能）。
        ///
        ///參見：https：//doc.photonengine.com/en-us/pun/v2/gameplay/interestgroups
        ///
        ///請參閱：https://doc.photonengine.com/zh-CN/pun/v2/demos-and-tutorials/package-demos/culling-demo
        /// </remarks>
        /// <param name ="group">要影響的興趣組。</param>
        /// <param name ="enabled">設置是否從組接收為啟用（或不啟用）。</param>
        public static void SetInterestGroups(byte group, bool enabled)
        {
            PhotonNetwork.SetInterestGroups(group, enabled);
        }
        /// <summary>啟用/禁用給定興趣組的接收（適用於PhotonViews）。</summary>
        /// <remarks>
        ///客戶端可以告訴服務器它感興趣的興趣組。
        ///服務器將僅將那些興趣組的事件轉發到該客戶端（節省帶寬和性能）。
        ///
        ///參見：https：//doc.photonengine.com/en-us/pun/v2/gameplay/interestgroups
        ///
        ///請參閱：https://doc.photonengine.com/zh-CN/pun/v2/demos-and-tutorials/package-demos/culling-demo
        /// </remarks>
        /// <param name ="disableGroups">要禁用的興趣組（或為null）。</param>
        /// <param name ="enableGroups">要啟用的興趣組（或為null）。</param>
        public static void SetInterestGroups(byte[] disableGroups, byte[] enableGroups)
        {
            PhotonNetwork.SetInterestGroups(disableGroups, enableGroups);
        }
        /// <summary>啟用/禁用在給定組上的發送（適用於PhotonViews）</summary>
        /// <remarks>
        ///這不會與Photon服務器端進行交互。
        ///這只是禁止更新的客戶端設置，如果更新被發送到其中一個被阻止的組中。
        ///
        ///??? 此設置並不是特別有用，因為它意味著更新實際上不會到達服務器或其他任何人。
        ///小心使用。
        /// </remarks>
        /// <param name ="group">要影響的興趣組。</param>
        /// <param name ="enabled">設置是否啟用“發送到組”。</param>
        public static void SetSendingEnabled(byte group, bool enabled)
        {
            PhotonNetwork.SetSendingEnabled(group, enabled);
        }

        /// <summary>啟用/禁用在給定組上的發送（適用於PhotonViews）</summary>
        /// <remarks>
        ///這不會與Photon服務器端進行交互。
        ///這只是禁止更新的客戶端設置，如果更新被發送到其中一個被阻止的組中。
        ///
        ///??? 此設置並不是特別有用，因為它意味著更新實際上不會到達服務器或其他任何人。
        ///小心使用。
        ///</remarks>
        /// <param name ="enableGroups">啟用發送功能的興趣組（或為null）。</param>
        /// <param name ="disableGroups">要禁止繼續發送（或為null）的興趣組。</param>
        public static void SetSendingEnabled(byte[] disableGroups, byte[] enableGroups)
        {
            PhotonNetwork.SetSendingEnabled(disableGroups, enableGroups);
        }

        /// <summary>
        /// ???
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static bool LocalCleanPhotonView(PhotonView view)
        {
            return PhotonNetwork.LocalCleanPhotonView(view);
        }

        /// <summary>
        /// 給予viewID獲取PhotonView
        /// </summary>
        /// <param name="viewID"></param>
        /// <returns></returns>
        public static PhotonView GetPhotonView(int viewID)
        {
            return PhotonNetwork.GetPhotonView(viewID);
        }

        /// <summary>
        /// ???
        /// </summary>
        /// <param name="netView"></param>
        public static void RegisterPhotonView(PhotonView netView)
        {
            PhotonNetwork.RegisterPhotonView(netView);
        }
        #endregion

        #region Instantiate創建、Destroy銷毀
        /// <summary>
        /// 建立物件於場景，類似原Unity的Instantiate，但每個玩家皆會收到建立通知並且同步創建。(必須Asset/.../Resources內)(有再分層則Resources/Prefab/Cube => Prefab/Cube)
        /// </summary>
        /// <param name="prefabName">預製物名稱與位置路徑</param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="group">填0都能創建，其餘號碼除非玩家有SetInterestGroups開啟訂閱不然不會創建</param>
        /// <param name="data">資料存至物件PhotonView</param>
        /// <returns></returns>
        public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
        {
            return PhotonNetwork.Instantiate(prefabName, position, rotation, group, data);
        }

        [System.Obsolete("Renamed. Use InstantiateRoomObject instead")]
        public static GameObject InstantiateSceneObject(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
        {
            return InstantiateRoomObject(prefabName, position, rotation, group, data);
        }
        /// <summary>
        /// ??? 創建房間物件(創建後建立者的值為空)
        /// </summary>
        /// <param name="prefabName"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="group">填0都能創建，其餘號碼除非玩家有SetInterestGroups開啟訂閱不然不會創建</param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static GameObject InstantiateRoomObject(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
        {
            return PhotonNetwork.InstantiateRoomObject(prefabName, position, rotation, group, data);
        }

        /// <summary>
        /// 銷毀與PhotonView關聯的GameObject，除非PhotonView是靜態的或不在此客戶端的控制之下。
        /// </summary>
        /// <remarks>
        ///在房間中銷毀聯網的GameObject包括：
        ///-從服務器的房間緩衝區中刪除實例化調用。
        ///-刪除為通過PhotonNetwork.Instantiate調用間接創建的PhotonViews緩衝的RPC。
        ///-向其他客戶端發送消息以也刪除GameObject（受網絡延遲的影響）。
        ///
        ///通常，當您離開房間時，GO會自動銷毀。
        ///如果您必須在不在房間內時銷毀GO，則銷毀僅在本地進行。
        ///
        ///僅當使用PhotonNetwork.Instantiate（）創建網絡對象時，才能銷毀它們。
        ///加載場景的對象將被忽略，無論它們是否具有PhotonView組件。
        ///
        /// GameObject必須在此客戶端的控制下：
        ///-由該客戶端實例化並擁有。
        ///-離開房間的玩家的實例化對象由主客戶端控制。
        ///-房間擁有的遊戲對象由主客戶端控制。
        ///-客戶端不在房間內時，可以銷毀GameObject。
        /// </remarks>
        /// <returns>什麼都沒有。檢查錯誤調試日誌中是否有任何問題。</returns>
        public static void Destroy(PhotonView targetView)
        {
            PhotonNetwork.Destroy(targetView);
        }
        /// <summary>
        /// 網絡銷毀GameObject，除非它是靜態的或不在此客戶端的控制之下。
        /// </summary>
        /// <remarks>
        ///銷毀聯網的GameObject包括：
        ///-從服務器的房間緩衝區中刪除實例化調用。
        ///-刪除為通過PhotonNetwork.Instantiate調用間接創建的PhotonViews緩衝的RPC。
        ///-向其他客戶端發送消息以也刪除GameObject（受網絡延遲的影響）。
        ///
        ///通常，當您離開房間時，GO會自動銷毀。
        ///如果您必須在不在房間內時銷毀GO，則銷毀僅在本地進行。
        ///
        ///僅當使用PhotonNetwork.Instantiate（）創建網絡對象時，才能銷毀它們。
        ///加載場景的對象將被忽略，無論它們是否具有PhotonView組件。
        ///
        /// GameObject必須在此客戶端的控制下：
        ///-由該客戶端實例化並擁有。
        ///-離開房間的玩家的實例化對象由主客戶端控制。
        ///-房間擁有的遊戲對象由主客戶端控制。
        ///-客戶端不在房間內時，可以銷毀GameObject。
        /// </remarks>
        /// <returns>什麼都沒有。檢查錯誤調試日誌中是否有任何問題。</ returns>
        public static void Destroy(GameObject targetGo)
        {
            PhotonNetwork.Destroy(targetGo);
        }
        /// <summary>
        /// 網絡銷毀targetPlayer的所有GameObjects，PhotonViews及其RPC。 只能在本地播放器（“自我”）或主客戶端（對任何人）上調用。
        /// </summary>
        /// <remarks>
        ///銷毀聯網的GameObject包括：
        ///-從服務器的房間緩衝區中刪除實例化調用。
        ///-刪除為通過PhotonNetwork.Instantiate調用間接創建的PhotonViews緩衝的RPC。
        ///-向其他客戶端發送消息以也刪除GameObject（受網絡延遲的影響）。
        ///
        ///僅當使用PhotonNetwork.Instantiate（）創建網絡對象時，才能銷毀它們。
        ///加載場景的對象將被忽略，無論它們是否具有PhotonView組件。
        /// </remarks>
        /// <returns>什麼都沒有。 檢查錯誤調試日誌中是否有任何問題。</returns>
        public static void DestroyPlayerObjects(Player targetPlayer)
        {
            if (targetPlayer == null)
            {
                Debug.LogError("DestroyPlayerObjects() failed, cause parameter 'targetPlayer' was null.");
            }

            DestroyPlayerObjects(targetPlayer.ActorNumber);
        }
        /// <summary>
        ///網絡銷毀該玩家的所有GameObject，PhotonViews及其RPC（按ID）。 只能在本地播放器（“自我”）或主客戶端（對任何人）上調用。
        /// </summary>
        /// <remarks>
        ///銷毀聯網的GameObject包括：
        ///-從服務器的房間緩衝區中刪除實例化調用。
        ///-刪除為通過PhotonNetwork.Instantiate調用間接創建的PhotonViews緩衝的RPC。
        ///-向其他客戶端發送消息以也刪除GameObject（受網絡延遲的影響）。
        ///
        ///僅當使用PhotonNetwork.Instantiate（）創建網絡對象時，才能銷毀它們。
        ///加載場景的對象將被忽略，無論它們是否具有PhotonView組件。
        /// </remarks>
        /// <returns>什麼都沒有。 檢查錯誤調試日誌中是否有任何問題。</returns>
        public static void DestroyPlayerObjects(int targetPlayerId)
        {
            PhotonNetwork.DestroyPlayerObjects(targetPlayerId);
        }
        /// <summary>
        ///網絡銷毀房間中的所有GameObject，PhotonViews及其RPC。從服務器中刪除所有緩衝的內容。只能由主客戶（對於任何人）調用。
        /// </summary>
        /// <remarks>
        ///只能由Master Client調用（適用於任何人）。
        ///與Destroy方法不同，這將從服務器的房間緩衝區中刪除所有內容。如果你的遊戲
        ///緩衝除Instantiate和RPC調用之外的所有內容，這些內容也將從服務器中清除。
        ///
        ///銷毀所有內容包括：
        ///-從服務器的房間緩衝區中刪除所有內容（實例化，RPC，任何緩衝區中的內容）。
        ///-向其他客戶端發送消息，也要在本地銷毀所有內容（受網絡延遲的影響）。
        ///
        ///僅當使用PhotonNetwork.Instantiate（）創建網絡對象時，才能銷毀它們。
        ///加載場景的對象將被忽略，無論它們是否具有PhotonView組件。
        /// </remarks>
        /// <returns>什麼都沒有。檢查錯誤調試日誌中是否有任何問題。</returns>
        public static void DestroyAll()
        {
            PhotonNetwork.DestroyAll();
        }

        /// <summary>
        /// ??? (PhotonNetwork.NetworkInstantiate是private這怎操作...)對像池可用於保留和重用實例化的對象實例。 替換Unity的默認“實例化”和“銷毀”方法。
        /// </summary>
        /// <remarks>
        ///默認為DefaultPool類型。
        ///要使用GameObject池，請實現IPunPrefabPool並在此處進行分配。
        ///預製件按名稱標識。
        /// </remarks>
        public static IPunPrefabPool PrefabPool
        {
            get
            {
                return PhotonNetwork.PrefabPool;
            }
            set
            {
                PhotonNetwork.PrefabPool = value;
            }
        }
        #endregion

        #region LoadLevel讀取場景
        /// <summary>此方法包裝"異步"加載級別並在此過程中暫停網絡消息。</summary>
        /// <remarks>
        ///在網絡遊戲中加載關卡時，不要分發其他玩家收到的消息是有意義的。
        /// LoadLevel通過設置PhotonNetwork.IsMessageQueueRunning = false直到加載場景來解決這一問題。
        ///
        ///要同步房間中的已加載級別，請將PhotonNetwork.AutomaticallySyncScene設置為true。
        ///然後，房間的主客戶端將與房間中的其他所有播放器同步加載的級別。
        ///請注意，這僅適用於單個活動場景，並且不支持重新加載場景。
        ///主客戶端實際上將重新加載場景，而其他客戶端則不會。
        ///
        ///您應該確保在加載另一個場景（其中不包含該場景）之前不觸發RPC
        ///相同的GameObjects和PhotonViews）。
        ///
        /// LoadLevel使用SceneManager.LoadSceneAsync（）。
        ///
        ///使用PhotonNetwork.LevelLoadingProgress檢查LevelLoading的進度。
        ///
        ///不建議在上一個場景完成加載之前調用LoadLevel。
        ///如果啟用了AutomaticallySyncScene，則PUN會取消先前的加載（並防止該加載
        ///成為活動場景）。如果關閉AutomaticallySyncScene，則先前的場景加載可以完成。
        ///在這兩種情況下，都會在本地加載新場景。
        /// </remarks>
        /// <param name ="levelNumber">
        ///要加載的級別的構建索引號。使用級別號時，請確保所有客戶端上的級別號相同。
        /// </param>
        public static void LoadLevel(int levelNumber)
        {
            PhotonNetwork.LoadLevel(levelNumber);
        }

        /// <summary>此方法包裝初始化"異步"加載級別並在此過程中暫停網絡消息。</summary>
        /// <remarks>
        ///在網絡遊戲中加載關卡時，不要分發其他玩家收到的消息是累積的。
        /// LoadLevel通過設置PhotonNetwork.IsMessageQueueRunning = false直到加載場景來解決這一問題。
        ///
        ///要同步房間中的已加載等級，分配PhotonNetwork.AutomaticallySyncScene設置為true。
        ///然後，房間的主客戶端將與房間中的其他所有播放器同步加載的等級。
        ///請注意，這僅適用於各個活動場景，並且不支持重新加載場景。
        ///主客戶端實際上將重新加載場景，而其他客戶端則不會。
        ///
        ///您應該確保在加載另一個場景（其中不包含該場景）之前不觸發RPC
        ///相同的GameObjects和PhotonViews）。
        ///
        /// LoadLevel使用SceneManager.LoadSceneAsync（）。
        ///
        ///使用PhotonNetwork.LevelLoadingProgress檢查LevelLoading的進度。
        ///
        ///不建議在上一個場景完成加載之前調用LoadLevel。
        ///如果啟用了AutomaticallySyncScene，則PUN會取消先前的加載（並防止該加載
        ///成為活動場景）。如果自動關閉SyncScene，則先前的場景加載可以完成。
        ///在這兩種情況下，都會在本地加載新場景。
        /// </remarks>
        /// <param name ="levelName">
        ///要加載的等級的名稱。確保同一房間的所有客戶都可以使用。
        /// </param>
        public static void LoadLevel(string levelName)
        {
            PhotonNetwork.LoadLevel(levelName);
        }
        /// <summary>
        ///表示使用LoadLevel（）時的場景加載進度。
        /// </summary>
        /// <remarks>
        ///如果應用程序從未使用LoadLevel（）加載場景，則值為0。
        ///在異步場景加載期間，該值介於0和1之間。
        ///一旦任何場景完成加載，它將停留在1（信號“完成”）。
        /// </remarks>
        /// <value>關卡加載進度。 範圍從0到1。</value>
        public static float LevelLoadingProgress
        {
            get
            {
                return PhotonNetwork.LevelLoadingProgress;
            }
        }
        #endregion

        #region RPC(遠程調用)
        /// <summary>
        ///從targetPlayer發送的服務器中刪除所有緩衝的RPC。只能在本地播放器（“自我”）或主客戶端（對任何人）上調用。
        /// </summary>
        /// <remarks>
        ///此方法要求：
        ///-這是targetPlayer的客戶端。
        ///-此客戶端是主客戶端（可以刪除任何播放器的RPC）。
        ///
        ///如果targetPlayer在調用的同時調用RPC，
        ///網絡滯後將決定是否像其餘一樣緩衝或清除那些滯後。
        /// </remarks>
        /// <param name ="targetPlayer">此播放器的RPC RPC已從服務器機架中刪除。</param>
        public static void RemoveRPCs(Player targetPlayer)
        {
            PhotonNetwork.RemoveRPCs(targetPlayer);
        }
        /// <summary>
        /// 從服務器刪除所有通過targetPhotonView發送的緩衝的RPC。 主客戶端和targetPhotonView的所有者可以調用此方法。
        /// </summary>
        /// <remake>
        ///此方法要求：
        ///-targetPhotonView由該客戶端擁有（由其實例化）。
        ///-此客戶端是主客戶端（可以刪除任何PhotonView的RPC）。
        /// </remake>
        /// <param name="targetPhotonView">為此PhotonView緩衝的RPC從服務器緩衝區中刪除。</param>
        public static void RemoveRPCs(PhotonView targetPhotonView)
        {
            PhotonNetwork.RemoveRPCs(targetPhotonView);
        }
        /// <summary>
        /// ??? 此操作使Photon使用給定的參數通過名稱（路徑）調用您的自定義Web服務。
        /// </summary>
        /// <remarks>
        ///這是服務器端功能，使用前必須在Photon Cloud Dashboard中對其進行設置。
        /// 請參閱"https://doc.photonengine.com/en-us/pun/v2/gameplay/web-extensions/webrpc"
        ///參數將轉換為JSon格式，因此請確保您的參數兼容。
        ///
        ///有關如何獲取響應的信息，請參見"Photon.Realtime.IWebRpcCallback.OnWebRpcResponse"。
        ///
        ///重要的是要了解OperationResponse僅告訴是否可以調用WebRPC。
        ///響應的內容包含您的Web服務發送的所有值以及錯誤/成功代碼。
        ///如果Web服務失敗，通常會在其中包含錯誤代碼和調試消息
        /// OperationResponse。
        ///
        /// WebRpcResponse類是一個幫助程序類，可從WebRPC中提取最有價值的內容
        /// 回复。
        /// </remarks>
        /// <example>
        /// Example callback implementation:<pre>
        ///
        /// public void OnWebRpcResponse(OperationResponse response)
        /// {
        ///     WebRpcResponse webResponse = new WebRpcResponse(operationResponse);
        ///     if (webResponse.ReturnCode != 0) { //...
        ///     }
        ///
        ///     switch (webResponse.Name) { //...
        ///     }
        ///     // and so on
        /// }</pre>
        /// </example>
        public static bool WebRpc(string name, object parameters, bool sendAuthCookie = false)
        {
            return NetworkingClient.OpWebRpc(name, parameters, sendAuthCookie);
        }
        /// <summary>
        /// ??? 啟用後，我們稱為RPC的MonoBehaviours將被緩存，從而避免了昂貴的GetComponents＆lt; MonoBehaviour＆gt;（）調用。
        /// </summary>
        /// <remarks>
        ///在目標PhotonView的MonoBehaviours上調用RPC。 這些必須通過GetComponents找到。
        ///
        ///將此設置為true時，MonoBehaviours的列表將緩存在每個PhotonView中。
        ///您可以使用photonView.RefreshRpcMonoBehaviourCache（）手動刷新PhotonView的
        ///按需列出的MonoBehaviours列表（例如，當將新的MonoBehaviour添加到聯網的GameObject時）。
        /// </remarks>
        public static bool UseRpcMonoBehaviourCache
        {
            get
            {
                return PhotonNetwork.UseRpcMonoBehaviourCache;
            }
            set
            {
                PhotonNetwork.UseRpcMonoBehaviourCache = value;
            }
        }

        /// <summary>
        ///如果將RPC方法實現為協程，則它將啟動，除非該值為false。(true : IEnumerator RPC才能使用，預設為true)
        /// </summary>
        /// <remarks>
        ///由於啟動協程會造成一些內存垃圾，因此您可能要禁用此選項，但是它是
        ///也足以不從帶有attribite PunRPC的方法返回IEnumerable。
        /// </remarks>
        public static bool RunRpcCoroutines
        {
            get
            {
                return PhotonNetwork.RunRpcCoroutines;
            }
            set
            {
                PhotonNetwork.RunRpcCoroutines = value;
            }
        }
        /// <summary>
        ///刪除其他人的RPC（用作主服務器）(只有MasterClient可以呼叫)。
        ///這不會清除任何本地緩存。 它只是告訴服務器忘記播放器(指定的玩家)的RPC和實例化。
        /// </summary>
        /// <param name ="actorNumber"> </param>
        public static void OpCleanActorRpcBuffer(int actorNumber)
        {
            PhotonNetwork.OpCleanActorRpcBuffer(actorNumber);
        }

        /// <summary>
        /// 取而代之的是刪除 RPC 或實例化，這刪除了參與者緩存的所有內容。
        /// </summary>
        /// <param name ="actorNumber"> </param>
        public static void OpRemoveCompleteCacheOfPlayer(int actorNumber)
        {
            PhotonNetwork.OpRemoveCompleteCacheOfPlayer(actorNumber);
        }

        /// <summary>
        /// 刪除完整的緩存
        /// </summary>
        public static void OpRemoveCompleteCache()
        {
            PhotonNetwork.OpRemoveCompleteCache();
        }

        // Remove RPCs of view (if they are local player's RPCs)
        /// <summary>
        /// 刪除視圖的RPC（如果它們是本地播放器的RPC）(Remove RPCs of view (if they are local player's RPCs))
        /// </summary>
        /// <param name="view"></param>
        public static void CleanRpcBufferIfMine(PhotonView view)
        {
            PhotonNetwork.CleanRpcBufferIfMine(view);
        }

        /// <summary>清理PhotonView的服務器RPC（無需任何進一步檢查）。(Cleans server RPCs for PhotonView (without any further checks).)</summary>
        public static void OpCleanRpcBuffer(PhotonView view)
        {
            PhotonNetwork.OpCleanRpcBuffer(view);
        }

        /// <summary>
        ///如果這是Master Client或控制單個PhotonView，則從targetGroup中發送的服務器中刪除所有緩衝的RPC。
        /// </summary>
        /// <remarks>
        ///此方法要求：
        ///-此客戶端是主客戶端（可以刪除每個組中的所有RPC）。
        ///-其他任何客戶端：檢查每個PhotonView是否在此客戶端的控制之下。 僅刪除那些RPC。
        /// </remarks>
        /// <param name ="group">刪除所有RPC的興趣組。</param>
        public static void RemoveRPCsInGroup(int group)
        {
            PhotonNetwork.RemoveRPCsInGroup(group);
        }

        /// <summary>
        ///根據過濾器參數清除緩衝的RPC。
        /// </summary>
        /// <param name ="viewId">已在其中調用RPC的PhotonView的viewID。 我們實際上需要它的ViewID。 如果提供0（默認值），則會考慮所有PhotonViews / ViewID。</param>
        /// <param name ="methodName"> RPC方法名稱，如果可能的話，我們將使用其哈希快捷方式來提高效率。 如果未提供任何值（空字符串或空字符串），則會考慮所有RPC方法名稱。</param>
        /// <param name ="callersActorNumbers">調用/緩衝RPC的玩家的演員編號。 例如，如果兩個播放器緩衝了同一個RPC，則可以清除一個緩衝的RPC，並保留另一個。 如果未提供任何值（空數組或空數組），則將考慮所有發送者。</param>
        /// <returns>如果該操作可以發送到服務器。</returns>
        public static bool RemoveBufferedRPCs(int viewId = 0, string methodName = null, int[] callersActorNumbers = null)
        {
            return RemoveBufferedRPCs(viewId, methodName, callersActorNumbers);
        }

        /// <summary>
        ///在本地銷毀所有實例化和RPC，並且（如果不是localOnly）發送EvDestroy（player）並清除服務器緩衝區中的相關事件。
        /// </summary>
        public static void DestroyPlayerObjects(int playerId, bool localOnly)
        {
            PhotonNetwork.DestroyPlayerObjects(playerId, localOnly);
        }
        #endregion
    }
}
