using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

namespace Tim
{
    /* IConnectionCallbacks 連線 */
    public partial class MyPhotonRealtimeCallBack : IConnectionCallbacks
    {
        /// <summary>
        /// 已連線
        /// </summary>
        public override void OnConnected()
        {
            Debug.Log(string.Format("Photon.Realtime.IConnectionCallbacks - OnConnected()<已連線>"));
        }

        /// <summary>
        /// Master伺服器已連線
        /// </summary>
        public override void OnConnectedToMaster()
        {
            Debug.Log(string.Format("Photon.Realtime.IConnectionCallbacks - OnConnectedToMaster()<已連線至Master伺服器>") + ", " + "PhotonNetwork.CloudRegion : " + MyPhotonNetwork.CloudRegion);
        }

        /// <summary>
        /// 自定義身份已驗證失敗(除非您為您的應用程序（在Dashboard中）設置了自定義身份驗證服務，否則將不會調用該服務！)
        /// </summary>
        /// <param name="debugMessage"></param>
        public override void OnCustomAuthenticationFailed(string debugMessage)
        {
            Debug.Log(string.Format("Photon.Realtime.IConnectionCallbacks - OnCustomAuthenticationFailed()<自定義身份驗證已失敗, debugMessage : {0}", debugMessage));
        }

        /// <summary>
        /// 自定義身份已響應(除非您為您的應用程序（在Dashboard中）設置了自定義身份驗證服務，否則將不會調用該服務！)
        /// </summary>
        /// <param name="data"></param>
        public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
            string mes = "";
            foreach (var item in data) {
                mes += string.Format("Key:{0}, Value:{1}", item.Key, item.Value) + "\n";
            }
            Debug.Log(string.Format("Photon.Realtime.IConnectionCallbacks - OnCustomAuthenticationResponse()<自定義身份已響應, Dictionary data : \n{0}>", mes));
        }

        /// <summary>
        /// 已中斷連線
        /// </summary>
        /// <param name="cause"></param>
        public override void OnDisconnected(DisconnectCause cause)
        {
            string mes = "";
            switch (cause) {
                case DisconnectCause.None:
                    mes = "沒有追踪到錯誤";
                    break;
                case DisconnectCause.ExceptionOnConnect:
                    mes = "OnStatusChanged：服務器不可用或地址錯誤。 確保提供了端口並且服務器已啟動。";
                    break;
                case DisconnectCause.DnsExceptionOnConnect:
                    mes = "OnStatusChanged：主機名的Dns解析失敗。 捕獲此異常並記錄為錯誤級別。";
                    break;
                case DisconnectCause.ServerAddressInvalid:
                    mes = "OnStatusChanged：服務器地址被非法解析為IPv4。 非法地址將是例如 192.168.1.300。 IPAddress.TryParse（）可以通過，但我們的檢查無法通過。";
                    break;
                case DisconnectCause.Exception:
                    mes = "OnStatusChanged：一些內部異常導致套接字代碼失敗。 如果您嘗試在本地連接但服務器不可用，則可能會發生這種情況。 如有疑問，請聯繫退出遊戲。";
                    break;
                case DisconnectCause.ServerTimeout:
                    mes = "OnStatusChanged：由於超時（缺少客戶端的確認），服務器斷開了此客戶端的連接。";
                    break;
                case DisconnectCause.ClientTimeout:
                    mes = "OnStatusChanged：此客戶端檢測到未及時收到服務器的響應。";
                    break;
                case DisconnectCause.DisconnectByServerLogic:
                    mes = "OnStatusChanged：服務器已斷開該客戶端與會議室邏輯（C＃代碼）內的連接。";
                    break;
                case DisconnectCause.DisconnectByServerReasonUnknown:
                    mes = "OnStatusChanged：服務器出於未知原因斷開了此客戶端的連接。";
                    break;
                case DisconnectCause.InvalidAuthentication:
                    mes = "OnOperationResponse：在光子云中使用無效的AppId進行身份驗證。 更新您的訂閱或聯繫退出遊戲。";
                    break;
                case DisconnectCause.CustomAuthenticationFailed:
                    mes = "OnOperationResponse：在Photon Cloud中使用無效的客戶端值或Cloud Dashboard中的自定義身份驗證設置進行身份驗證。";
                    break;
                case DisconnectCause.AuthenticationTicketExpired:
                    mes = "身份驗證票證應提供對任何Photon Cloud服務器的訪問權限，而無需進行其他身份驗證服務調用。 但是，票證已過期。";
                    break;
                case DisconnectCause.MaxCcuReached:
                    mes = "OnOperationResponse：使用不帶CCU突發的Photon Cloud訂閱時，身份驗證（臨時）失敗。 更新您的訂閱。";
                    break;
                case DisconnectCause.InvalidRegion:
                    mes = "OnOperationResponse：在應用程序的Photon Cloud訂閱鎖定到某個（其他）區域時進行身份驗證。 更新您的訂閱或主服務器地址。";
                    break;
                case DisconnectCause.OperationNotAllowedInCurrentState:
                    mes = "OnOperationResponse：該客戶端當前不可用的操作（通常未經授權）。 僅針對op Authenticate進行跟踪。";
                    break;
                case DisconnectCause.DisconnectByClientLogic:
                    mes = "OnStatusChanged：客戶端從邏輯內斷開連接（C＃代碼）。";
                    break;
                case DisconnectCause.DisconnectByOperationLimit:
                    mes = "客戶端調用操作過於頻繁，並且由於點擊了OperationLimit而斷開了連接。 這也會觸發客戶端斷開連接。\n(為了保護服務器，某些操作有限制。 當OperationResponse因ErrorCode.OperationLimitReached失敗時，客戶端將斷開連接。)";
                    break;
                case DisconnectCause.DisconnectByDisconnectMessage:
                    mes = "客戶端從服務器收到“斷開連接消息”。 檢查調試日誌以獲取詳細信息。";
                    break;
            }
            Debug.Log(string.Format("Photon.Realtime.IConnectionCallbacks - OnDisconnected()<已中斷連線,  enum DisconnectCause斷線原因 : {0} - \n{1}>", cause, mes));
        }

        /// <summary>
        /// 各區域相關資訊(當名稱服務器提供標題區域列表時調用。(檢查RegionHandler類描述，以使用提供的值。))
        /// </summary>
        /// <param name="regionHandler"></param>
        public override void OnRegionListReceived(RegionHandler regionHandler)
        {
            string strEnabledRegions = "";
            for (int i=0; i< regionHandler.EnabledRegions.Count; i++) {
                strEnabledRegions += string.Format(
                    "{0} - Code:{1},\n " +
                    "Cluster:{1},\n " +
                    "HostAndPort:{2},\n " +
                    "Ping:{3},\n " +
                    "WasPinged:{4}\n", 
                    i,
                    regionHandler.EnabledRegions[i].Code,
                    regionHandler.EnabledRegions[i].Cluster,
                    regionHandler.EnabledRegions[i].HostAndPort,
                    regionHandler.EnabledRegions[i].Ping,
                    regionHandler.EnabledRegions[i].WasPinged
                    );
            }
            Debug.Log(string.Format("Photon.Realtime.IConnectionCallbacks - OnRegionListReceived()各區域相關資訊如下 : \n" +
                "PingImplementation:{0},\n" +
                "EnabledRegions:{1},\n" +
                "bestRegionCache:{2}", 
                RegionHandler.PingImplementation,
                strEnabledRegions,
                regionHandler.BestRegion));
        }
    }
}

