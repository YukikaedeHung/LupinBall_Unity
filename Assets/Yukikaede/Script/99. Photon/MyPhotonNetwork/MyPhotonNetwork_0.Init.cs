using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Tim
{
    /*Init*/
    public static partial class MyPhotonNetwork
    {
        #region "建議"連線前設定
        /// <summary>序列化的服务器设置，由安装向导编写，用于ConnectUsingSettings。</summary>
        public static ServerSettings PhotonServerSettings
        {
            get
            {
                return PhotonNetwork.PhotonServerSettings;
            }
        }

        /// <summary>
        /// Pun版號
        /// </summary>
        public static string PunVersion
        {
            get { return PhotonNetwork.PunVersion; }
        }
        /// <summary>
        /// Pun版號 -> AppVersion版號
        /// </summary>
        public static string AppVersion
        {
            get { return PhotonNetwork.NetworkingClient.AppVersion; }
        }
        /// <summary>
        /// Pun版號 -> AppVersion版號 -> GameVersion版號(不同版號不能互通)
        /// </summary>
        public static string GameVersion
        {
            get { return PhotonNetwork.GameVersion; }
            set
            {
                PhotonNetwork.GameVersion = value;
                PhotonNetwork.NetworkingClient.AppVersion = string.Format("{0}_{1}", value, PhotonNetwork.PunVersion);
            }
        }

        public static LoadBalancingClient NetworkingClient
        {
            get { return PhotonNetwork.NetworkingClient; }
        }

        /// <summary>
        /// 連接期間使用的用戶身份驗證值。 
        /// </summary>
        /// <remake>
        /// 如果要自定義身份驗證，請在調用Connect之前進行設置。
        /// PhotonNetwork.AuthValues = new AuthenticationValues();
        /// PhotonNetwork.AuthValues.UserId = iptUserID.text;
        /// 這些值設置userId，是否以及如何驗證該userId（服務器端）等。
        /// 如果任何值的身份驗證失敗，PUN將調用OnCustomAuthenticationFailed（string debugMessage）的實現。
        /// 請參見Photon.Realtime.IConnectionCallbacks.OnCustomAuthenticationFailed
        /// </remake>
        public static AuthenticationValues AuthValues
        {
            get { return PhotonNetwork.AuthValues; }
            set { PhotonNetwork.AuthValues = value; }
        }

        /// <summary>
        /// (建議連線前設定)設置為將玩家的暱稱與您輸入房間中的所有人同步。 這將設置PhotonNetwork.player.NickName。
        /// </summary>
        /// <remarks>
        /// NickName只是一個暱稱，不必唯一，也不必使用某些帳戶進行備份。<br/>
        /// 隨時設置該值（例如，在連接之前），並且您所玩的每個人都可以使用它。<br/>
        /// 通過以下方式訪問播放器的名稱：Player.NickName。 <br/>
        /// PhotonNetwork.PlayerListOthers是其他播放器的列表-每個播放器都包含遠程播放器設置的暱稱。
        /// </remarks>
        public static string NickName
        {
            get
            {
                return PhotonNetwork.NickName;
            }

            set
            {
                PhotonNetwork.NickName = value;
            }
        }

        /// <summary>定義會議室中的所有客戶端是否應自動加載與主客戶端相同的級別。</summary>
        /// <remarks>
        ///啟用後，客戶端將加載與主客戶端上活動的場景相同的場景。
        ///當客戶端加入房間時，即使在調用回調OnJoinedRoom之前，場景也會被加載。
        ///
        ///要同步加載的級別，主客戶端應使用PhotonNetwork.LoadLevel，
        ///在開始加載場景之前通知其他客戶端。
        ///如果主客戶端直接通過Unity的API加載關卡，PUN將在之後通知其他玩家
        ///場景加載完成（使用SceneManager.sceneLoaded）。
        ///
        ///在內部，為加載的場景設置了“自定義房間”屬性。更改時，客戶端使用LoadLevel
        ///如果它們不在同一場景中。
        ///
        ///請注意，這僅適用於單個活動場景，並且不支持重新加載場景。
        ///主客戶端實際上將重新加載場景，而其他客戶端則不會。
        ///為了讓所有人重新加載遊戲，遊戲可以發送RPC或事件來觸發加載。
        /// </remarks>
        public static bool AutomaticallySyncScene
        {
            get
            {
                return PhotonNetwork.AutomaticallySyncScene;
            }
            set
            {
                PhotonNetwork.AutomaticallySyncScene = value;
            }
        }
        #endregion

        #region 連線方法
        /// <summary>按照PhotonServerSettings文件中的配置連接到Photon。</summary>
        /// <remarks>
        ///實現IConnectionCallbacks，以使您的遊戲邏輯了解狀態變化。
        ///特別是，IConnectionCallbacks.ConnectedToMasterServer對於在以下情況下做出反應非常有用
        ///客戶可以進行配對。
        ///
        ///此方法將禁用OfflineMode（它不會破壞任何實例化的GO），並且它
        ///將IsMessageQueueRunning設置為true。
        ///
        ///您的光子配置是由PUN嚮導創建的，其中包含AppId，
        /// Photon Cloud遊戲的區域，服務器地址等。
        ///
        ///要忽略設置文件，請設置相關值並通過調用進行連接
        /// ConnectToMaster，ConnectToRegion。
        ///
        ///要連接到Photon Cloud，必須在設置文件中包含有效的AppId
        ///（顯示在 https://dashboard.photonengine.com Photon Cloud Dashboard中）。
        ///
        ///由於以下原因，連接到光子云可能會失敗：
        ///-無效的AppId
        ///-網絡問題
        ///-無效的區域
        ///-達到訂閱CCU限制
        /// - 等等。
        ///
        ///通常，從IConnectionCallbacks.OnDisconnected回調中籤出DisconnectCause。
        /// </remarks>
        public static bool ConnectUsingSettings()
        {
            return PhotonNetwork.ConnectUsingSettings();
        }
        /// <summary>
        /// ???????
        /// </summary>
        /// <param name="appSettings"></param>
        /// <param name="startInOfflineMode"></param>
        /// <returns></returns>
        public static bool ConnectUsingSettings(AppSettings appSettings, bool startInOfflineMode = false) // parameter name hides static class member
        {
            return PhotonNetwork.ConnectUsingSettings(appSettings, startInOfflineMode);
        }


        /// <summary>通過地址，端口，appID連接到Photon Master服務器。</summary>
        /// <remarks>
        ///要連接到Photon Cloud，必須在設置文件中顯示有效的AppId（顯示在Photon Cloud儀表板中）。
        /// https://dashboard.photonengine.com
        ///
        ///由於以下原因，連接到光子云可能會失敗：
        ///-無效的AppId
        ///-網絡問題
        ///-無效的區域
        ///-達到訂閱CCU限制
        /// - 等等。
        ///
        ///通常，從IConnectionCallbacks.OnDisconnected回調中籤出DisconnectCause。
        /// </remarks>
        /// <param name="masterServerAddress">The server's address (either your own or Photon Cloud address).</param>
        /// <param name="port">The server's port to connect to.</param>
        /// <param name="appID">Your application ID (Photon Cloud provides you with a GUID for your game).</param>
        public static bool ConnectToMaster(string masterServerAddress, int port, string appID)
        {
            return PhotonNetwork.ConnectToMaster(masterServerAddress, port, appID);
        }


        /// <summary>
        ///以最低的ping連接到Photon Cloud區域（在支持Unity的Ping的平台上）。
        /// </summary>
        /// <remarks>
        ///將保存對PlayerPrefs中的所有云服務器執行ping操作的結果。第一次撥打此電話可能需要+ -2秒的時間。
        ///可以通過PhotonNetwork.OverrideBestCloudServer（..）覆蓋ping結果
        ///如果您是第一次使用此呼叫，則此過程最多可能需要2秒鐘，所有的雲服務器都將被ping以檢查最佳區域。
        ///
        /// PUN設置嚮導將您的appID存儲在設置文件中，並應用服務器地址/端口。
        ///要連接到Photon Cloud，必須在設置文件中顯示有效的AppId（顯示在Photon Cloud儀表板中）。
        /// https://dashboard.photonengine.com
        ///
        ///由於以下原因，連接到光子云可能會失敗：
        ///-無效的AppId
        ///-網絡問題
        ///-無效的區域
        ///-達到訂閱CCU限制
        /// - 等等。
        ///
        ///通常，從IConnectionCallbacks.OnDisconnected回調中籤出DisconnectCause。
        /// </remarks>
        /// <returns>如果此客戶端將基於ping連接到雲服務器。即使為true，也不能保證建立連接，但會嘗試進行連接。</returns>
        public static bool ConnectToBestCloudServer()
        {
            return PhotonNetwork.ConnectToBestCloudServer();
        }

        /// <summary>
        ///連接到所選的“光子云”(PhotonCloud)區域。
        /// </summary>
        /// <remarks>
        ///通常，足以定義區域代碼（“ eu”，“ us”等）。
        ///當區域被分割並且您支持朋友/邀請時，可能需要連接到特定的群集。
        ///
        ///在所有其他情況下，不應定義群集，因為這將允許名稱服務器進行分發
        ///根據需要提供客戶端。 將選擇一個隨機的，負載均衡的群集。
        ///
        ///名稱服務器擁有分配可用群集的最終決定權。
        ///如果請求的群集不可用，則會分配另一個群集。
        ///
        ///一旦連接，請檢查CurrentCluster的值。
        /// </remarks>
        public static bool ConnectToRegion(string region)
        {
            return PhotonNetwork.ConnectToRegion(region);
        }
        #endregion

        #region 連線相關
        /// <summary>
        ///使該客戶端與光子服務器斷開連接，該過程將保留所有空間並在完成時調用OnDisconnected。
        /// </summary>
        /// <remarks>
        ///當您斷開連接時，客戶端將向服務器發送“正在斷開連接”消息。 這加快了離開/斷開連接的速度
        ///與您在同一個房間中的玩家的消息（否則服務器將使該客戶端的連接超時）。
        ///在離線模式下使用時，狀態更改和事件調用OnDisconnected是立即的。
        ///脫機模式也設置為false。
        ///一旦斷開連接，客戶端即可再次連接。 使用ConnectUsingSettings。
        /// </remarks>
        public static void Disconnect()
        {
            PhotonNetwork.Disconnect();
        }

        /// <summary>可用於在斷開連接後重新連接到主服務器。</summary>
        /// <remarks>
        ///斷開連接後，您可以使用它再次將客戶端連接到區域主服務器。
        ///緩存您所在的房間名稱，然後使用RejoinRoom（roomname）返回游戲。
        ///常見用例：按下iOS設備上的“鎖定”按鈕，您將立即斷開連接。
        /// </remarks>
        public static bool Reconnect()
        {
            return PhotonNetwork.Reconnect();
        }


        /// <summary>
        /// ??? 重置流量統計信息並重新啟用它們。
        /// </summary>
        public static void NetworkStatisticsReset()
        {
            PhotonNetwork.NetworkStatisticsReset();
        }


        /// <summary>
        /// ??? 僅在使用NetworkStatisticsEnabled收集某些統計信息時可用。
        /// </summary>
        /// <returns>包含重要網絡統計信息的字符串。</returns>
        public static string NetworkStatisticsToString()
        {
            return PhotonNetwork.NetworkStatisticsToString();
        }

        /// <summary>
        /// 當前到光子服務器的往返時間。
        /// </summary>
        /// <returns>往返時間（往返服務器）。</returns>
        public static int GetPing()
        {
            return PhotonNetwork.GetPing();
        }

        /// <summary>??? 刷新服務器時間戳（異步操作，需要往返）。</summary>
        /// <remarks>如果連接不良導致時間戳無法使用或不精確，則很有用。</remarks>
        public static void FetchServerTimestamp()
        {
            PhotonNetwork.FetchServerTimestamp();
        }

        /// <summary>
        /// ??? 可以用於立即發送剛剛調用的RPC和實例化，因此它們正在傳遞給其他播放器。
        /// </summary>
        /// <remarks>
        ///如果您執行RPC加載級別然後自己加載它，這可能會很有用。
        ///加載時，沒有RPC發送給其他人，因此這將延遲“加載” RPC。
        ///您可以將RPC發送給“其他”，使用此方法，禁用消息隊列
        ///（通過IsMessageQueueRunning），然後加載。
        /// </remarks>
        public static void SendAllOutgoingCommands()
        {
            PhotonNetwork.SendAllOutgoingCommands();
        }

        /// <summary>
        /// 當前使用的服務器地址（無論是主服務器還是遊戲服務器）。
        /// </summary>
        public static string ServerAddress
        {
            get { return (PhotonNetwork.NetworkingClient != null) ? PhotonNetwork.NetworkingClient.CurrentServerAddress : "<not connected>"; }
        }
        /// <summary>
        /// 當前使用的雲區域(區域伺服器)
        /// </summary>
        public static string CloudRegion
        {
            get { return (PhotonNetwork.NetworkingClient != null && PhotonNetwork.IsConnected && PhotonNetwork.Server != ServerConnection.NameServer) ? PhotonNetwork.NetworkingClient.CloudRegion : null; }
        }
        /// <summary>
        /// ??? 名稱服務器提供的群集名稱。該值由OpResponse提供，用於OpAuthenticate / OpAuthenticateOnce。 請參閱ConnectToRegion。請注意，如果請求的集群未配置或不可用，則名稱服務器可能會分配另一個集群。
        /// </summary>
        public static string CurrentCluster
        {
            get { return (PhotonNetwork.NetworkingClient != null) ? PhotonNetwork.NetworkingClient.CurrentCluster : null; }
        }
        /// <summary>
        /// ??? 用於存儲和訪問“播放器首選項”中的“最佳區域摘要”。在連接之前將此值設置為null，以放棄先前為客戶端選擇的最佳區域。
        /// </summary>
        public static string BestRegionSummaryInPreferences
        {
            get
            {
                return PhotonNetwork.BestRegionSummaryInPreferences;
            }
        }
        /// <summary>
        /// 是否連上MasterServer(PhotonServer) 雲區域伺服器
        /// </summary>
        public static bool IsConnected
        {
            get
            {
                return PhotonNetwork.IsConnected;
            }
        }
        /// <summary>
        /// ??? 精簡版的connected，僅當您與服務器的連接已準備好接受加入，離開等操作時才是正確的。
        /// </summary>
        public static bool IsConnectedAndReady
        {
            get
            {
                return PhotonNetwork.IsConnectedAndReady;
            }
        }
        /// <summary>
        /// 直接提供網絡級客戶端狀態，除非處於OfflineMode下。
        /// </summary>
        /// <remarks>
        /// 在PUN上下文中，通常應使用IsConnected或IsConnectedAndReady。
        /// 這是較低級別的連接狀態。 請記住，PUN使用不止一台服務器，
        /// 因此，即使只是切換服務器，客戶端也可能會斷開連接。
        /// 當OfflineMode為true時，在所有其他情況下為ClientState.Joined（在創建/加入之後）或ConnectedToMasterServer。
        /// </remarks>
        public static ClientState NetworkClientState
        {
            get
            {
                return PhotonNetwork.NetworkClientState;
            }
        }
        /// <summary>
        /// 跟踪，最後一次調用哪個Connect方法。
        /// </summary>
        /// <remarks>
        /// ConnectToMaster(根據物件PhotonServerSettings)、ConnectToRegion(自定義連結雲區域)、ConnectToBestCloudServer(自動判斷最好區域)
        /// </remarks>
        public static ConnectMethod ConnectMethod
        {
            get { return PhotonNetwork.ConnectMethod; }
        }
        /// <summary>
        /// ??? 此客戶端當前已連接或正在連接的服務器（類型）。
        /// </summary>
        /// <remarks>Photon使用3種不同的服務器角色：名稱服務器，主服務器和遊戲服務器。</remarks>
        public static ServerConnection Server
        {
            get
            {
                return PhotonNetwork.Server;
            }
        }
        #endregion

    }
}
