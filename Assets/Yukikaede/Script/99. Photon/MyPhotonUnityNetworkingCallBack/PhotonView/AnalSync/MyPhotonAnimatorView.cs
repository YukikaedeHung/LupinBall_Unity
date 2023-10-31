// ----------------------------------------------------------------------------
// <copyright file="PhotonAnimatorView.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Component to synchronize Mecanim animations via PUN.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Tim
{
    using System.Collections.Generic;
    using UnityEngine;
    using Photon.Pun;


    /// <summary>
    /// This class helps you to synchronize Mecanim animations
    /// Simply add the component to your GameObject and make sure that
    /// the PhotonAnimatorView is added to the list of observed components
    /// </summary>
    /// <remarks>
    /// When Using Trigger Parameters, make sure the component that sets the trigger is higher in the stack of Components on the GameObject than 'PhotonAnimatorView'
    /// Triggers are raised true during one frame only.
    /// </remarks>
    [AddComponentMenu("My Photon Networking/My Photon Animator View")]
    public class MyPhotonAnimatorView : MonoBehaviourPun, IPunObservable
    {
        #region Enums

        /// <summary>
        /// 動畫參數類型
        /// </summary>
        public enum ParameterType
        {
            Float = 1,
            Int = 3,
            Bool = 4,
            Trigger = 9,
        }

        /// <summary>
        /// 同步類型
        /// </summary>
        public enum SynchronizeType
        {
            Disabled = 0,//已禁用
            Discrete = 1,//離散(寫資料時機-IPunObservable-OnPhotonSerializeView實作讀寫)
            Continuous = 2,//連續(寫資料時機-Update裡將大料的更新資料放入PhotonStreamQueue，之後PhotonStream一次性送出，PhotonStream讀寫頻率受PhotonNetwork.SendRate影響)
        }


        /// <summary>
        /// 同步動畫參數類別
        /// </summary>
        [System.Serializable]
        public class SynchronizedParameter
        {
            /// <summary>
            /// 動畫參數類型
            /// </summary>
            public ParameterType Type;
            /// <summary>
            /// 同步類型
            /// </summary>
            public SynchronizeType SynchronizeType;
            /// <summary>
            /// 參數名稱
            /// </summary>
            public string Name;
        }

        /// <summary>
        /// 同步動畫Layer
        /// </summary>
        [System.Serializable]
        public class SynchronizedLayer
        {
            /// <summary>
            /// 同步類型
            /// </summary>
            public SynchronizeType SynchronizeType;
            /// <summary>
            /// Layer索引值
            /// </summary>
            public int LayerIndex;
        }

        #endregion


        #region Properties

#if PHOTON_DEVELOP
        public MyPhotonAnimatorView ReceivingSender;
#endif

        #endregion


        #region Members

        /// <summary>
        /// 觸發使用警告完成
        /// </summary>
        private bool TriggerUsageWarningDone;
        
        private Animator m_Animator;

        /// <summary>
        /// PhotonStreamQueue 可幫助您以更高的頻率輪詢對象狀態(PhotonStreamQueue集中資料後透過PhotonStream一次性送出)
        /// PhotonNetwork.SendRate 指示並在以下情況下立即發送所有這些狀態
        /// Serialize() 被調用。
        /// 在接收端，您可以調用 Deserialize()，然後流將推出
        /// 接收到的對象狀態與它們記錄的順序和時間步長相同。
        /// </summary>
        private PhotonStreamQueue m_StreamQueue = new PhotonStreamQueue(120);

        //這些字段僅在此腳本的自定義編輯器中使用，並且會觸發
        //“這個變量從未使用過”警告，我在這裡壓制 ???
        #pragma warning disable 0414

        /// <summary>
        /// ???
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private bool ShowLayerWeightsInspector = true;

        /// <summary>
        /// ???
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private bool ShowParameterInspector = true;

#pragma warning restore 0414

        /// <summary>
        /// 同步動畫參數類別-列表
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private List<SynchronizedParameter> m_SynchronizeParameters = new List<SynchronizedParameter>();

        /// <summary>
        /// 同步動畫Layer-列表
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private List<SynchronizedLayer> m_SynchronizeLayers = new List<SynchronizedLayer>();

        /// <summary>
        /// 位置???
        /// </summary>
        private Vector3 m_ReceiverPosition;
        /// <summary>
        /// 最後一次反序列化時間
        /// </summary>
        private float m_LastDeserializeTime;
        /// <summary>
        /// 同步類型是否有更改
        /// </summary>
        private bool m_WasSynchronizeTypeChanged = true;

        /// <summary>
        /// 在離散模式下設置為同步的緩存引發的觸發器(Trigger)。 因為觸發器(Trigger)只停留不到一幀，
        /// 我們需要緩存它直到下一個離散的序列化調用。
        /// </summary>
        List<string> m_raisedDiscreteTriggersCache = new List<string>();

        #endregion


        #region Unity

        private void Awake()
        {
            this.m_Animator = GetComponent<Animator>();
            //Debug.Log("Animation is Running : " + this.m_Animator.GetBool("XD"));
            ///*設定動畫"XD"為非執行中(false)的狀態*/
            //this.m_Animator.SetBool("XD", false);
            //Debug.Log("Animation is Running : " + this.m_Animator.GetBool("XD"));
        }

        private void Update()
        {
            /*本來動畫移動物件，但這個不是自己的，所以關閉動畫移動物件，免得雙重移動*/
            if (this.m_Animator.applyRootMotion && this.photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                /*
                Animator.ApplyRootMotion
                这个属性是用来控制物体在播放骨骼动画的时候是否应用骨骼根节点的运动参数。

                一、当没有骨骼根节点的情况时，比如只是一个Cube立方体，如果勾选了ApplyRootMotion，运行后不会播放动画，因为应用了骨骼根节点的运动参数，而没有骨骼根节点，就没有动画了。即便是在代码中强行调用Animator.Play("rotation")方法也不会播放动画。

                二、当有骨骼根节点的情况时，一旦设置了这个变量为true，那么请一定注意，这个会对物理引擎在模拟对象的运动轨迹时产生直接的影响，例如在某个动画A中，对象只向Y轴方向进行了移动，在X和Z轴是静止的，那么我们在播放A动画的时候，如果使用Rigidbody设置速度或者施加外力，还是不会让物体在X和Z轴上发生位移的。这是因为，在整个动画播放的过程中（例如0.5秒），Animator会根据动画中物体的位移信息对物体的速度进行赋值，这样达到使用骨骼根节点的位移的效果，也就是说，我在播放动画过程中的任意时间给物体设置了X或者Z轴方向上的运动速度，后续的动画播放帧中，速度又会被Animator强制赋值为跟动画文件中的位移信息一致。
                ————————————————
                版权声明：本文为CSDN博主「鹅厂程序小哥」的原创文章，遵循CC 4.0 BY-SA版权协议，转载请附上原文出处链接及本声明。
                原文链接：https://blog.csdn.net/qq826364410/article/details/80463080
                //https://zhuanlan.zhihu.com/p/105029905
                */
                this.m_Animator.applyRootMotion = false;
            }

            if (PhotonNetwork.InRoom == false || PhotonNetwork.CurrentRoom.PlayerCount <= 1)
            {
                /*重置 PhotonStreamQueue。 每當您觀察的對像數量發生變化時，您都需要執行此操作*/
                this.m_StreamQueue.Reset();
                return;
            }

            if (this.photonView.IsMine == true)//寫
            {
                this.SerializeDataContinuously();

                this.CacheDiscreteTriggers();
            }
            else//讀
            {
                this.DeserializeDataContinuously();
            }
        }

        #endregion


        #region Setup Synchronizing Methods 設置同步方法(幾乎都在MyPhotonAnimatorViewEditor執行)

        /// <summary>
        /// (Discrete)緩存離散觸發器值以跟踪引發的觸發器，並將在同步例程執行後重置
        /// </summary>
        public void CacheDiscreteTriggers()
        {
            for (int i = 0; i < this.m_SynchronizeParameters.Count; ++i)
            {
                SynchronizedParameter parameter = this.m_SynchronizeParameters[i];

                if (parameter.SynchronizeType == SynchronizeType.Discrete && parameter.Type == ParameterType.Trigger && this.m_Animator.GetBool(parameter.Name))
                {
                    if (parameter.Type == ParameterType.Trigger)
                    {
                        this.m_raisedDiscreteTriggersCache.Add(parameter.Name);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 檢查特定層是否配置為同步(Editor執行)
        /// </summary>
        /// <param name="layerIndex">Index of the layer.</param>
        /// <returns>如果圖層同步，則為真</returns>
        public bool DoesLayerSynchronizeTypeExist(int layerIndex)
        {
            return this.m_SynchronizeLayers.FindIndex(item => item.LayerIndex == layerIndex) != -1;
        }

        /// <summary>
        /// 檢查指定參數是否配置為同步(Editor執行)
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>如果參數同步則為真</returns>
        public bool DoesParameterSynchronizeTypeExist(string name)
        {
            return this.m_SynchronizeParameters.FindIndex(item => item.Name == name) != -1;
        }

        /// <summary>
        /// 獲取所有同步層的列表(Editor執行)
        /// </summary>
        /// <returns>List of SynchronizedLayer objects</returns>
        public List<SynchronizedLayer> GetSynchronizedLayers()
        {
            return this.m_SynchronizeLayers;
        }

        /// <summary>
        /// 獲取所有同步參數的列表(Editor執行)
        /// </summary>
        /// <returns>List of SynchronizedParameter objects</returns>
        public List<SynchronizedParameter> GetSynchronizedParameters()
        {
            return this.m_SynchronizeParameters;
        }

        /// <summary>
        /// 獲取層如何同步的類型(Editor執行)
        /// </summary>
        /// <param name="layerIndex">Index of the layer.</param>
        /// <returns>Disabled/Discrete/Continuous</returns>
        public SynchronizeType GetLayerSynchronizeType(int layerIndex)
        {
            int index = this.m_SynchronizeLayers.FindIndex(item => item.LayerIndex == layerIndex);

            if (index == -1)
            {
                return SynchronizeType.Disabled;
            }

            return this.m_SynchronizeLayers[index].SynchronizeType;
        }

        /// <summary>
        /// 獲取參數如何同步的類型(Editor執行)
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>Disabled/Discrete/Continuous</returns>
        public SynchronizeType GetParameterSynchronizeType(string name)
        {
            int index = this.m_SynchronizeParameters.FindIndex(item => item.Name == name);

            if (index == -1)
            {
                return SynchronizeType.Disabled;
            }

            return this.m_SynchronizeParameters[index].SynchronizeType;
        }

        /// <summary>
        /// 設置圖層應該如何同步(Editor執行)
        /// </summary>
        /// <param name="layerIndex">Index of the layer.</param>
        /// <param name="synchronizeType">Disabled/Discrete/Continuous</param>
        public void SetLayerSynchronized(int layerIndex, SynchronizeType synchronizeType)
        {
            if (Application.isPlaying == true)
            {
                this.m_WasSynchronizeTypeChanged = true;
            }

            int index = this.m_SynchronizeLayers.FindIndex(item => item.LayerIndex == layerIndex);

            if (index == -1)
            {
                this.m_SynchronizeLayers.Add(new SynchronizedLayer {LayerIndex = layerIndex, SynchronizeType = synchronizeType});
            }
            else
            {
                this.m_SynchronizeLayers[index].SynchronizeType = synchronizeType;
            }
        }

        /// <summary>
        /// 設置參數應該如何同步(Editor執行)
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="type">The type of the parameter.</param>
        /// <param name="synchronizeType">Disabled/Discrete/Continuous</param>
        public void SetParameterSynchronized(string name, ParameterType type, SynchronizeType synchronizeType)
        {
            if (Application.isPlaying == true)
            {
                this.m_WasSynchronizeTypeChanged = true;
            }

            int index = this.m_SynchronizeParameters.FindIndex(item => item.Name == name);

            if (index == -1)
            {
                this.m_SynchronizeParameters.Add(new SynchronizedParameter {Name = name, Type = type, SynchronizeType = synchronizeType});
            }
            else
            {
                this.m_SynchronizeParameters[index].SynchronizeType = synchronizeType;
            }
        }

        #endregion


        #region Serialization 序列化

        /// <summary>
        /// (Continuously)連續序列化數據
        /// </summary>
        private void SerializeDataContinuously()
        {
            if (this.m_Animator == null)
            {
                return;
            }

            /*運作時更改內容才會看到作用*/
            for (int i = 0; i < this.m_SynchronizeLayers.Count; ++i)
            {
                if (this.m_SynchronizeLayers[i].SynchronizeType == SynchronizeType.Continuous)
                {
                    this.m_StreamQueue.SendNext(this.m_Animator.GetLayerWeight(this.m_SynchronizeLayers[i].LayerIndex));
                }
            }

            for (int i = 0; i < this.m_SynchronizeParameters.Count; ++i)
            {
                SynchronizedParameter parameter = this.m_SynchronizeParameters[i];

                if (parameter.SynchronizeType == SynchronizeType.Continuous)
                {
                    switch (parameter.Type)
                    {
                        case ParameterType.Bool:
                            this.m_StreamQueue.SendNext(this.m_Animator.GetBool(parameter.Name));
                            break;
                        case ParameterType.Float:
                            this.m_StreamQueue.SendNext(this.m_Animator.GetFloat(parameter.Name));
                            break;
                        case ParameterType.Int:
                            this.m_StreamQueue.SendNext(this.m_Animator.GetInteger(parameter.Name));
                            break;
                        case ParameterType.Trigger:
                            if (!TriggerUsageWarningDone)
                            {
                                TriggerUsageWarningDone = true;
                                Debug.Log("PhotonAnimatorView: When using triggers, make sure this component is last in the stack.\n" +
                                          "If you still experience issues, implement triggers as a regular RPC \n" +
                                          "or in custom IPunObservable component instead", this);
                                /*
                                ??????
                                使用觸發器(Trigger)時，請確保此組件在堆棧中的最後。\n" +
                                "如果您仍然遇到問題，請將觸發器實現為常規 RPC \n" +
                                "或在自定義 IPunObservable 組件中"
                                */

                            }
                            this.m_StreamQueue.SendNext(this.m_Animator.GetBool(parameter.Name));
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// (Continuously)連續反序列化數據
        /// </summary>
        private void DeserializeDataContinuously()
        {
            /*確定隊列是否存儲了任何對象*/
            if (this.m_StreamQueue.HasQueuedObjects() == false)
            {
                return;
            }

            /*運作時更改內容才會看到作用*/
            for (int i = 0; i < this.m_SynchronizeLayers.Count; ++i)
            {
                if (this.m_SynchronizeLayers[i].SynchronizeType == SynchronizeType.Continuous)
                {
                    this.m_Animator.SetLayerWeight(this.m_SynchronizeLayers[i].LayerIndex, (float)this.m_StreamQueue.ReceiveNext());
                }
            }

            for (int i = 0; i < this.m_SynchronizeParameters.Count; ++i)
            {
                SynchronizedParameter parameter = this.m_SynchronizeParameters[i];

                if (parameter.SynchronizeType == SynchronizeType.Continuous)
                {
                    switch (parameter.Type)
                    {
                        case ParameterType.Bool:
                            this.m_Animator.SetBool(parameter.Name, (bool) this.m_StreamQueue.ReceiveNext());
                            break;
                        case ParameterType.Float:
                            this.m_Animator.SetFloat(parameter.Name, (float) this.m_StreamQueue.ReceiveNext());
                            break;
                        case ParameterType.Int:
                            this.m_Animator.SetInteger(parameter.Name, (int) this.m_StreamQueue.ReceiveNext());
                            break;
                        case ParameterType.Trigger:
                            this.m_Animator.SetBool(parameter.Name, (bool) this.m_StreamQueue.ReceiveNext());
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// (Discretly)離散序列化數據(會寫資料stream.SendNext)
        /// </summary>
        /// <param name="stream"></param>
        private void SerializeDataDiscretly(PhotonStream stream)
        {
            for (int i = 0; i < this.m_SynchronizeLayers.Count; ++i)
            {
                if (this.m_SynchronizeLayers[i].SynchronizeType == SynchronizeType.Discrete)
                {
                    stream.SendNext(this.m_Animator.GetLayerWeight(this.m_SynchronizeLayers[i].LayerIndex));
                }
            }

            for (int i = 0; i < this.m_SynchronizeParameters.Count; ++i)
            {
               
                SynchronizedParameter parameter = this.m_SynchronizeParameters[i];
       
                if (parameter.SynchronizeType == SynchronizeType.Discrete)
                {
                    switch (parameter.Type)
                    {
                        case ParameterType.Bool:
                            stream.SendNext(this.m_Animator.GetBool(parameter.Name));
                            break;
                        case ParameterType.Float:
                            stream.SendNext(this.m_Animator.GetFloat(parameter.Name));
                            break;
                        case ParameterType.Int:
                            stream.SendNext(this.m_Animator.GetInteger(parameter.Name));
                            break;
                        case ParameterType.Trigger:
                            if (!TriggerUsageWarningDone)
                            {
                                TriggerUsageWarningDone = true;
                                Debug.Log("PhotonAnimatorView: When using triggers, make sure this component is last in the stack.\n" +
                                          "If you still experience issues, implement triggers as a regular RPC \n" +
                                          "or in custom IPunObservable component instead",this);
                            
                            }
                            // here we can't rely on the current real state of the trigger, we might have missed its raise
                            /*在這裡我們不能依賴觸發器的當前真實狀態，我們可能錯過了它的加註*/
                            stream.SendNext(this.m_raisedDiscreteTriggersCache.Contains(parameter.Name));
                            break;
                    }
                }
            }

            // reset the cache, we've synchronized.
            /*重置緩存，我們已經同步了。*/
            this.m_raisedDiscreteTriggersCache.Clear();
        }

        /// <summary>
        /// (Discretly)離散反序列化數據
        /// </summary>
        /// <param name="stream"></param>
        private void DeserializeDataDiscretly(PhotonStream stream)
        {
            for (int i = 0; i < this.m_SynchronizeLayers.Count; ++i)
            {
                if (this.m_SynchronizeLayers[i].SynchronizeType == SynchronizeType.Discrete)
                {
                    this.m_Animator.SetLayerWeight(this.m_SynchronizeLayers[i].LayerIndex, (float) stream.ReceiveNext());
                }
            }

            for (int i = 0; i < this.m_SynchronizeParameters.Count; ++i)
            {
                SynchronizedParameter parameter = this.m_SynchronizeParameters[i];

                if (parameter.SynchronizeType == SynchronizeType.Discrete)
                {
                    switch (parameter.Type)
                    {
                        case ParameterType.Bool:
                            if (stream.PeekNext() is bool == false)
                            {
                                return;
                            }
                            this.m_Animator.SetBool(parameter.Name, (bool) stream.ReceiveNext());
                            break;
                        case ParameterType.Float:
                            if (stream.PeekNext() is float == false)
                            {
                                return;
                            }

                            this.m_Animator.SetFloat(parameter.Name, (float) stream.ReceiveNext());
                            break;
                        case ParameterType.Int:
                            if (stream.PeekNext() is int == false)
                            {
                                return;
                            }

                            this.m_Animator.SetInteger(parameter.Name, (int) stream.ReceiveNext());
                            break;
                        case ParameterType.Trigger:
                            if (stream.PeekNext() is bool == false)
                            {
                                return;
                            }

                            if ((bool) stream.ReceiveNext())
                            {
                                this.m_Animator.SetTrigger(parameter.Name);
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 序列化同步類型狀態(會寫資料stream.SendNext)
        /// </summary>
        /// <param name="stream"></param>
        private void SerializeSynchronizationTypeState(PhotonStream stream)
        {
            byte[] states = new byte[this.m_SynchronizeLayers.Count + this.m_SynchronizeParameters.Count];

            for (int i = 0; i < this.m_SynchronizeLayers.Count; ++i)
            {
                states[i] = (byte) this.m_SynchronizeLayers[i].SynchronizeType;
            }

            for (int i = 0; i < this.m_SynchronizeParameters.Count; ++i)
            {
                states[this.m_SynchronizeLayers.Count + i] = (byte) this.m_SynchronizeParameters[i].SynchronizeType;
            }

            stream.SendNext(states);
        }

        /// <summary>
        /// 反序列化同步類型狀態
        /// </summary>
        /// <param name="stream"></param>
        private void DeserializeSynchronizationTypeState(PhotonStream stream)
        {
            byte[] state = (byte[]) stream.ReceiveNext();

            for (int i = 0; i < this.m_SynchronizeLayers.Count; ++i)
            {
                this.m_SynchronizeLayers[i].SynchronizeType = (SynchronizeType) state[i];
            }

            for (int i = 0; i < this.m_SynchronizeParameters.Count; ++i)
            {
                this.m_SynchronizeParameters[i].SynchronizeType = (SynchronizeType) state[this.m_SynchronizeLayers.Count + i];
            }
        }

        /// <summary>
        /// IPunObservable-OnPhotonSerializeView實作讀寫
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (this.m_Animator == null)
            {
                return;
            }

            if (stream.IsWriting == true)
            {
                if (this.m_WasSynchronizeTypeChanged == true)
                {
                    this.m_StreamQueue.Reset();
                    this.SerializeSynchronizationTypeState(stream);

                    this.m_WasSynchronizeTypeChanged = false;
                }

                /*m_StreamQueue.Serialize 序列化指定的流。 在您的 OnPhotonSerializeView 方法中調用它以發送整個錄製的流。*/
                this.m_StreamQueue.Serialize(stream);
                this.SerializeDataDiscretly(stream);
                /*
                1s 60次資料
                1s 10次資料
                */
            }
            else
            {
                #if PHOTON_DEVELOP
                if( ReceivingSender != null )
                {
                    ReceivingSender.OnPhotonSerializeView( stream, info );
                }
                else
                #endif
                {
                    if (stream.PeekNext() is byte[])//讀到的第一份內容是否為byte[]類型，對應寫時的m_WasSynchronizeTypeChanged=true,this.SerializeSynchronizationTypeState(stream)做讀取;
                    {
                        this.DeserializeSynchronizationTypeState(stream);
                    }

                    /*m_StreamQueue.Deserialize 反序列化指定的流。 在您的 OnPhotonSerializeView 方法中調用它以接收整個錄製的流。*/
                    this.m_StreamQueue.Deserialize(stream);
                    this.DeserializeDataDiscretly(stream);
                }
            }
        }
        #endregion
    }
}