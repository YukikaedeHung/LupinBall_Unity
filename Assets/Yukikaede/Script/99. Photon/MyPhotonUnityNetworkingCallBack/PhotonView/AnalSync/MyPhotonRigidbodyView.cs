// ----------------------------------------------------------------------------
// <copyright file="PhotonRigidbodyView.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Component to synchronize rigidbodies via PUN.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Tim
{
    using UnityEngine;
    using Photon.Pun;

    [RequireComponent(typeof(Rigidbody))]
    [AddComponentMenu("My Photon Networking/My Photon Rigidbody View")]
    public class MyPhotonRigidbodyView : MonoBehaviourPun, IPunObservable
    {
        /// <summary>
        /// 當前位置與網路位置(預測位置)的距離
        /// </summary>
        private float m_Distance;
        /// <summary>
        /// 當前角值與網路角值(預測值)的角差距
        /// </summary>
        private float m_Angle;

        private Vector3 m_NetworkPosition;

        private Quaternion m_NetworkRotation;

        [HideInInspector]
        public bool m_SynchronizeVelocity = true;
        [HideInInspector]
        public bool m_SynchronizeAngularVelocity = true;

        [HideInInspector]
        public bool m_TeleportEnabled = false;
        [HideInInspector]
        public float m_TeleportIfDistanceGreaterThan = 3.0f;

        public bool SynchronizePosition;
        public bool SynchronizeRotation;

        public Rigidbody rbVelocity;
        public Rigidbody rbAngularVelocity;

        public void Awake()
        {
            this.m_NetworkPosition = new Vector3();
            this.m_NetworkRotation = new Quaternion();
        }

        public void FixedUpdate()
        {
            if (!this.photonView.IsMine)//不是自己的
            {
                if(SynchronizePosition && rbVelocity != null)
                    rbVelocity.position = Vector3.MoveTowards(rbVelocity.position, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * 10);
                if(SynchronizeRotation && rbAngularVelocity!=null)
                    rbAngularVelocity.rotation = Quaternion.RotateTowards(rbAngularVelocity.rotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * 10);
            }
        }

        /// <summary>
        /// IPunObservable-OnPhotonSerializeView實作讀寫
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if(rbVelocity!=null)
                    stream.SendNext(rbVelocity.position);
                if (rbAngularVelocity != null)
                    stream.SendNext(rbAngularVelocity.rotation);

                if (this.m_SynchronizeVelocity && rbVelocity != null)
                {
                    stream.SendNext(rbVelocity.velocity);
                }

                if (this.m_SynchronizeAngularVelocity && rbAngularVelocity != null)
                {
                    stream.SendNext(rbAngularVelocity.angularVelocity);
                }
            }
            else
            {
                if (rbVelocity != null)
                    this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
                if (rbAngularVelocity != null)
                    this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();

                if (this.m_TeleportEnabled && rbVelocity != null)
                {
                    if (Vector3.Distance(rbVelocity.position, this.m_NetworkPosition) > this.m_TeleportIfDistanceGreaterThan)
                    {
                        rbVelocity.position = this.m_NetworkPosition;
                    }
                }
                
                if (this.m_SynchronizeVelocity || this.m_SynchronizeAngularVelocity)
                {
                    float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

                    if (this.m_SynchronizeVelocity)
                    {
                        rbVelocity.velocity = (Vector3)stream.ReceiveNext();

                        this.m_NetworkPosition += rbVelocity.velocity * lag;//預測位置

                        this.m_Distance = Vector3.Distance(rbVelocity.position, this.m_NetworkPosition);
                    }

                    if (this.m_SynchronizeAngularVelocity)
                    {
                        rbAngularVelocity.angularVelocity = (Vector3)stream.ReceiveNext();

                        this.m_NetworkRotation = Quaternion.Euler(rbAngularVelocity.angularVelocity * lag) * this.m_NetworkRotation;

                        this.m_Angle = Quaternion.Angle(rbAngularVelocity.rotation, this.m_NetworkRotation);
                    }
                }
            }
        }
    }
}