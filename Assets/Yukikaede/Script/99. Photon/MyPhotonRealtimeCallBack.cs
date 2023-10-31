using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

namespace Tim
{
    /// <summary>
    /// Photon.Realtime全部回呼函式(CallBack)
    /// </summary>
    public partial class MyPhotonRealtimeCallBack : MonoBehaviourPunCallbacks
    {
        void Awake() {
            //DontDestroyOnLoad(this);
        }
    }
}

