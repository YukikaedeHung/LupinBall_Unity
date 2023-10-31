using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

namespace Tim
{
    /* IWebRpcCallback WebRPC(使用WebRPC，必須事先安裝好以HTTP基礎的伺服器端應用程式。此處製作的伺服器端應用程式，將以Photon Turnbased 儀表板進行設定。) */
    public partial class MyPhotonRealtimeCallBack : IWebRpcCallback
    {
        /// <summary>
        /// WebRpc響應
        /// </summary>
        /// <param name="response"></param>
        public override void OnWebRpcResponse(OperationResponse response)
        {
            string strresponse = string.Format("OperationCode:{0},\n" +
                "ReturnCode:{1},\n" +
                "DebugMessage:{2},\n" +
                "Parameters:{3},\n" +
                "ToStringFull():{4}",
                response.ReturnCode,
                response.DebugMessage,
                response.Parameters,
                response.ToStringFull()
                );
            Debug.Log(string.Format("Photon.Realtime.IWebRpcCallback - OnWebRpcResponse()<WebRpc響應 - \n" +
                "response:{0}>",
                strresponse));
        }
    }

}

