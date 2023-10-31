using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

namespace Tim
{
    /* IMatchmakingCallbacks 配對(進房前) */
    public partial class MyPhotonRealtimeCallBack : IMatchmakingCallbacks
    {
        /// <summary>
        /// 已創建房間
        /// </summary>
        public override void OnCreatedRoom()
        {
            Debug.Log(string.Format("Photon.Realtime.IMatchmakingCallbacks - OnCreatedRoom()<已創建房間>"));
        }

        /// <summary>
        /// 創建房間已失敗
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="message"></param>
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log(string.Format("Photon.Realtime.IMatchmakingCallbacks - OnCreateRoomFailed()<創建房間已失敗>"));
        }

        /// <summary>
        /// 好友名單列表更新
        /// </summary>
        /// <param name="friendList"></param>
        public override void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            string strfriendList = "";
            for (int i = 0; i < friendList.Count; i++)
            {
                strfriendList += string.Format("{0} - Name(已過時請改用UserId):{1}, UserId:{2}, IsOnline:{3}, Room:{4}, IsInRoom:{5},\n" +
                    "ToString():{6} \n",
                    i,
                    friendList[i].Name,
                    friendList[i].UserId,
                    friendList[i].IsOnline,
                    friendList[i].Room,
                    friendList[i].IsInRoom,
                    friendList[i].ToString());
            }
            Debug.Log(string.Format("Photon.Realtime.IMatchmakingCallbacks - OnFriendListUpdate()<好友名單列表更新如下 : \n{0}>", strfriendList));
        }

        /// <summary>
        /// 已加入房間
        /// </summary>
        public override void OnJoinedRoom()
        {
            Debug.Log(string.Format("Photon.Realtime.IMatchmakingCallbacks - OnJoinedRoom()<已加入房間>"));
        }

        /// <summary>
        /// 隨機加入房間已失敗
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="message"></param>
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log(string.Format("Photon.Realtime.IMatchmakingCallbacks - OnJoinRandomFailed()<隨機加入房間已失敗 - returnCode:{0}, message{1}>", returnCode, message));
        }

        /// <summary>
        /// 加入房間已失敗
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="message"></param>
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log(string.Format("Photon.Realtime.IMatchmakingCallbacks - OnJoinRoomFailed()<加入房間已失敗 - returnCode:{0}, message{1}>", returnCode, message));
        }

        /// <summary>
        /// 已離開房間
        /// </summary>
        public override void OnLeftRoom()
        {
            Debug.Log(string.Format("Photon.Realtime.IMatchmakingCallbacks - OnLeftRoom()<已離開房間>"));
        }
    }

}

