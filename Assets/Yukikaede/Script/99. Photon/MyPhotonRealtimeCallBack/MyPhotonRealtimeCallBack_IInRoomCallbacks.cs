using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

namespace Tim
{
    /* IInRoomCallbacks 房間內 */
    public partial class MyPhotonRealtimeCallBack : IInRoomCallbacks
    {
        /// <summary>
        /// MasterClient已換人
        /// </summary>
        /// <param name="newMasterClient"></param>
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            Debug.Log(string.Format("Photon.Realtime.IInRoomCallbacks - OnMasterClientSwitched()<MasterClient已換人>"));
        }

        /// <summary>
        /// 玩家已進入房間
        /// </summary>
        /// <param name="newPlayer"></param>
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            string strnewPlayer = string.Format(
                "ActorNumber:{0}, \n" +
                "IsLocal:{1}, \n" +
                "HasRejoined:{2}, \n" +
                "NickName:{3}, \n" +
                "UserId:{4}, \n" +
                "IsMasterClient:{5}, \n" +
                "IsInactive:{6}, \n" +
                "CustomProperties:{7}, \n" +
                "TagObject:{8}, \n" +
                "ToStringFull():{9}",
                newPlayer.ActorNumber,
                newPlayer.IsLocal,
                newPlayer.HasRejoined,
                newPlayer.NickName,
                newPlayer.UserId,
                newPlayer.IsMasterClient,
                newPlayer.IsInactive,
                newPlayer.CustomProperties,
                newPlayer.TagObject,
                newPlayer.ToStringFull()
                );
            Debug.Log(string.Format("Photon.Realtime.IInRoomCallbacks - OnPlayerEnteredRoom()<玩家已進入房間 - newPlayer :\n{0}>", strnewPlayer));
        }

        /// <summary>
        /// 玩家已離開房間
        /// </summary>
        /// <param name="otherPlayer"></param>
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            string strotherPlayer = string.Format(
               "ActorNumber:{0}, \n" +
               "IsLocal:{1}, \n" +
               "HasRejoined:{2}, \n" +
               "NickName:{3}, \n" +
               "UserId{4}, \n" +
               "IsMasterClient:{5}, \n" +
               "IsInactive:{6}, \n" +
               "CustomProperties:{7}, \n" +
               "TagObject:{8}, \n" +
               "ToStringFull():{9}",
               otherPlayer.ActorNumber,
               otherPlayer.IsLocal,
               otherPlayer.HasRejoined,
               otherPlayer.NickName,
               otherPlayer.UserId,
               otherPlayer.IsMasterClient,
               otherPlayer.IsInactive,
               otherPlayer.CustomProperties,
               otherPlayer.TagObject,
               otherPlayer.ToStringFull()
               );
            Debug.Log(string.Format("Photon.Realtime.IInRoomCallbacks - OnPlayerLeftRoom()<玩家已離開房間 - otherPlayer :\n{0}>", strotherPlayer));
        }

        /// <summary>
        /// 玩家屬性已更新
        /// </summary>
        /// <param name="targetPlayer"></param>
        /// <param name="changedProps"></param>
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            string strtargetPlayer = string.Format(
               "ActorNumber:{0}, \n" +
               "IsLocal:{1}, \n" +
               "HasRejoined:{2}, \n" +
               "NickName:{3}, \n" +
               "UserId{4}, \n" +
               "IsMasterClient:{5}, \n" +
               "IsInactive:{6}, \n" +
               "CustomProperties:{7}, \n" +
               "TagObject:{8}, \n" +
               "ToStringFull():{9}",
               targetPlayer.ActorNumber,
               targetPlayer.IsLocal,
               targetPlayer.HasRejoined,
               targetPlayer.NickName,
               targetPlayer.UserId,
               targetPlayer.IsMasterClient,
               targetPlayer.IsInactive,
               targetPlayer.CustomProperties,
               targetPlayer.TagObject,
               targetPlayer.ToStringFull()
               );

            string strchangedProps = "";
            foreach (var item in changedProps)
            {
                strchangedProps += string.Format("Key:{0}, Value:{1},\n", item.Key, item.Value);
            }

            //Debug.Log(string.Format("Photon.Realtime.IInRoomCallbacks - OnPlayerPropertiesUpdate()<玩家屬性已更新 - \n" +
            //    "targetPlayer:{0},\n" +
            //    "changedProps:{1}>",
            //    strtargetPlayer,
            //    strchangedProps));
        }

        /// <summary>
        /// 房間屬性已更新
        /// </summary>
        /// <param name="propertiesThatChanged"></param>
        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            string strpropertiesThatChanged = "";
            foreach (var item in propertiesThatChanged)
            {
                strpropertiesThatChanged += string.Format("Key:{0}, Value:{1},\n", item.Key, item.Value);
            }

            Debug.Log(string.Format("Photon.Realtime.IInRoomCallbacks - OnRoomPropertiesUpdate()<房間屬性已更新> - \n" +
                "propertiesThatChanged:{0}",
                strpropertiesThatChanged));
        }
    }

}

