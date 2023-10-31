using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

namespace Tim
{
    /* IErrorInfoCallback 錯誤(需事先註冊)??? */
    public partial class MyPhotonRealtimeCallBack : IErrorInfoCallback
    {
        public override void OnErrorInfo(ErrorInfo errorInfo)
        {
            string strerrorInfo = string.Format(
                "Info:{0},\n" +
                "ToString():{1}",
                errorInfo.Info,
                errorInfo.ToString()
                );
            Debug.Log(string.Format("Photon.Realtime.IErrorInfoCallback - OnErrorInfo()<錯誤訊息 - \n" +
                "errorInfo:{0}>",
                strerrorInfo));
        }
    }

}

