using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Tim.Game
{
    public class UnityChanAnimationControl : MonoBehaviour
    {
        public EnSupplies enSupplies;
        public CharaScript _charaScript;
        public Transform trShoot;
        private PhotonView pv;

        private void Awake()
        {
            pv = GetComponentInParent<PhotonView>();
        }

        public void ChangeATKTo0()
        {
            //預將Chara動作由ATK切換為Idle = 觸發Animator中的條件ATK並將值設定為0
            _charaScript.aniChara.SetInteger("ATK", 0);
        }

        public void UnityChanATK()
        {
            if (pv.IsMine)
            {
                _charaScript.PlayerATK();
            }
        }

        public void PlayerControlTure()
        {
            gameObject.GetComponentInParent<CharaScript>().bCtrl = true;
        }
    }
}

