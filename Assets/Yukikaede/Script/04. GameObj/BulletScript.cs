using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tim.Game;
using Photon.Pun;
using Photon.Realtime;

namespace Tim.Game
{
    public class BulletScript : MonoBehaviour
    {
        public CharaScript _charaScript;

        private PhotonView pv;

        public Rigidbody rbBullet;
        private Collider col; //玩家的Collider
        private Collider colBullet; //子彈的Collider

        private float fBulletSpeed;

        private void Awake()
        {
            pv = PhotonView.Get(this);
        }

        // Start is called before the first frame update
        void Start()
        {
            colBullet = GetComponent<Collider>();
            if (PhotonView.Get(this).IsMine)
            {
                col = GameManager.inst.objPlayer.GetComponent<Collider>();
                Physics.IgnoreCollision(col, colBullet);
                rbBullet.velocity = transform.TransformDirection(Vector3.forward) * (fBulletSpeed * 2);
            }
            else
            {
                colBullet.isTrigger = true;
            }
        }

        void Update()
        {

        }

        #region 設定子彈的速度 BulletState(float fNewBulletSpeed)
        /// <summary>
        /// 依照GunType來設定子彈的速度 BulletState(float fNewBulletSpeed)
        /// </summary>
        /// <param name="fNewLifeTime"></param>
        /// <param name="fNewBulletSpeed"></param>
        public void SetBulletState(float fNewBulletSpeed)
        {
            fBulletSpeed = fNewBulletSpeed;
        }
        #endregion
    }
}