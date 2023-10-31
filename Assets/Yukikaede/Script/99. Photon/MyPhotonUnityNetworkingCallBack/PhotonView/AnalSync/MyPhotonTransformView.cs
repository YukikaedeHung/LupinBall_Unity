// ----------------------------------------------------------------------------
// <copyright file="PhotonTransformView.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Component to synchronize Transforms via PUN PhotonView.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Tim
{
    using UnityEngine;
    using Photon.Pun;

    [AddComponentMenu("My Photon Networking/My Photon Transform View")]
    [HelpURL("https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state")]
    public class MyPhotonTransformView : MonoBehaviourPun, IPunObservable
    {
        private float m_Distance;//Read-距離
        private float m_Angle;//Read-角度

        private Vector3 m_Direction;//Read-方向
        private Vector3 m_NetworkPosition;//Read-網路最新位置
        private Vector3 m_StoredPosition;//Read-上一偵的資料(位置)

        private Quaternion m_NetworkRotation;

        public bool m_SynchronizePosition = true;
        public bool m_SynchronizeRotation = true;
        public bool m_SynchronizeScale = false;

        [Tooltip("Indicates if localPosition and localRotation should be used. Scale ignores this setting, and always uses localScale to avoid issues with lossyScale.")]
        public bool m_UseLocal;

        bool m_firstTake = false;

        /// <summary>
        /// 臨時加入者，對其而言過往遊戲物件產生後位置會先生成在原始位置
        /// 透過m_firstTake來判定第一次抓取資料，但可能因拿取資料的過程太慢而導致物件在畫面中有瞬移的情況。
        /// </summary>
        public GameObject objTemporaryJoiner;

        public void Awake()
        {
            m_StoredPosition = transform.localPosition;//讀取當前物件的localPosition(不含父物件的值)
            m_NetworkPosition = Vector3.zero;
            m_NetworkRotation = Quaternion.identity;//讀取當前物件的旋轉值
        }

        #region Reset()
        /// <summary>
        /// Only default to true with new instances. useLocal will remain false for old projects that are updating PUN.
        /// 僅對新實例對象的默認值設定為True，對於就項目且有在更新的對象則會維持為False
        /// </summary>
        private void Reset()
        {
            m_UseLocal = true;
        }
        #endregion

        #region OnEnable()
        /// <summary>
        /// 當此腳本被啟用，則會執行一次；若關掉再打開則會執行第二次
        /// ※※※Start只會再第一次被啟用的時候執行※※※
        /// </summary>
        void OnEnable()
        {
            m_firstTake = true;
        }
        #endregion

        /*★自加*/
        void Start() 
        {
            if (m_firstTake == true && objTemporaryJoiner != null)//檢查 : 因為OnPhotonSerializeView 有時執行順序比Start早，會導致obj開了又關
            {
                objTemporaryJoiner.SetActive(false);
                if (MyPhotonNetwork.PlayerListOthers.Length == 0)
                    objTemporaryJoiner.SetActive(true);
            }
            Debug.Log("Start()-End");
        }

        public void Update()
        {
            var tr = transform;

            if (!this.photonView.IsMine)//當PhotonView不是自己擁有
            {
                if (m_UseLocal)
                {
                    ///Vector3 顯示值為小數點無條件進位，所以實際值非顯示值
                    ///PhotonNetwork.SerializationRate平均約為60(幀率)
                    ///透過this.m_Distance * (1.0f / PhotonNetwork.SerializationRate)的計算來決定下一次移動的速度要快還是慢
                    ///若上述的結果越大，則需要越快的速度來拉近物件自身在每幀的距離差；反之則是越慢，來避免突然跑過頭的情況(有點類似馬達的damping)
                    ///***最後一份資料則需要花100幀***
                    tr.localPosition = Vector3.MoveTowards(tr.localPosition, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * 10);
                    tr.localRotation = Quaternion.RotateTowards(tr.localRotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * 10);
                }
                else
                {
                    tr.position = Vector3.MoveTowards(tr.position, this.m_NetworkPosition, this.m_Distance * (1.0f / PhotonNetwork.SerializationRate));
                    tr.rotation = Quaternion.RotateTowards(tr.rotation, this.m_NetworkRotation, this.m_Angle * (1.0f / PhotonNetwork.SerializationRate));
                }
            }
        }
        #region OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        /// <summary>
        /// 用於Photon Unity Networking (PUN)的函數，用於在網絡遊戲中同步物件的狀態
        /// 此函數為回調函數，由PUN自動調用，用於將物件的狀態序列化和反序列化，以便在網絡上進行同步
        /// stream：用於序列化和反序列化的PhotonStream對象，可以使用來讀取和寫入物件的狀態
        /// info：包含相關消息的附加信息(例如：發送該消息的玩家的ID等)
        /// 如果只有一位玩家則不會執行此方法，因為不需要寫讀資料
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            var tr = transform;
            Debug.Log("stream.IsWriting:" + stream.IsWriting);//確認資料是讀取/寫入的狀態
            Debug.Log("m_firstTake:" + m_firstTake);//若資料屬於自己的因為沒有讀取的必要則永遠為True，而別人資訊則第一次拿取後會將其設為False

            #region 寫入 Write
            if (stream.IsWriting)
            {
                if (objTemporaryJoiner != null)/*★自加*/
                    objTemporaryJoiner.SetActive(true);

                if (this.m_SynchronizePosition)
                {
                    if (m_UseLocal)
                    {
                        this.m_Direction = tr.localPosition - this.m_StoredPosition;
                        this.m_StoredPosition = tr.localPosition;
                        stream.SendNext(tr.localPosition);
                        stream.SendNext(this.m_Direction);
                        //***使用範例：加入HP資訊 stream.SendNext(healthPoints);
                    }
                    else
                    {
                        this.m_Direction = tr.position - this.m_StoredPosition;
                        this.m_StoredPosition = tr.position;
                        stream.SendNext(tr.position);
                        stream.SendNext(this.m_Direction);
                    }
                }

                if (this.m_SynchronizeRotation)
                {
                    if (m_UseLocal)
                    {
                        stream.SendNext(tr.localRotation);
                    }
                    else
                    {
                        stream.SendNext(tr.rotation);
                    }
                }

                if (this.m_SynchronizeScale)
                {
                    stream.SendNext(tr.localScale);
                }
            }
            #endregion

            #region 讀取 Read
            else
            {
                if (this.m_SynchronizePosition)
                {
                    this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
                    this.m_Direction = (Vector3)stream.ReceiveNext();

                    //m_firstTabke預設為True
                    if (m_firstTake)
                    {
                        if (m_UseLocal)
                            tr.localPosition = this.m_NetworkPosition;
                        else
                            tr.position = this.m_NetworkPosition;

                        this.m_Distance = 0f;
                    }
                    else
                    {
                        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                        this.m_NetworkPosition += this.m_Direction * lag;//lag時間 * 方向 = 等於額外偷跑距離

                        if (m_UseLocal)
                        {
                            this.m_Distance = Vector3.Distance(tr.localPosition, this.m_NetworkPosition);
                        }
                        else
                        {
                            this.m_Distance = Vector3.Distance(tr.position, this.m_NetworkPosition);
                        }
                    }

                }

                if (this.m_SynchronizeRotation)
                {
                    this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();

                    //m_firstTabke預設為True
                    if (m_firstTake)
                    {
                        this.m_Angle = 0f;

                        if (m_UseLocal)
                        {
                            tr.localRotation = this.m_NetworkRotation;
                        }
                        else
                        {
                            tr.rotation = this.m_NetworkRotation;
                        }
                    }
                    else
                    {
                        if (m_UseLocal)
                        {
                            this.m_Angle = Quaternion.Angle(tr.localRotation, this.m_NetworkRotation);
                        }
                        else
                        {
                            this.m_Angle = Quaternion.Angle(tr.rotation, this.m_NetworkRotation);
                        }
                    }
                }

                if (this.m_SynchronizeScale)
                {
                    tr.localScale = (Vector3)stream.ReceiveNext();
                }

                if (m_firstTake)
                {
                    m_firstTake = false;

                    if (objTemporaryJoiner != null)/*★自加*/
                    {
                        Debug.Log("objTemporaryJoiner.name:" + objTemporaryJoiner.name);
                        objTemporaryJoiner.SetActive(true);
                        Debug.Log("objTemporaryJoiner.activeInHierarchy:" + objTemporaryJoiner.activeInHierarchy);
                    }
                }

                //***使用範例：這邊則要加上讀取的方式 healthPoints = (int)stream.ReceiveNext();
            }
            #endregion
        }
        #endregion
    }
}