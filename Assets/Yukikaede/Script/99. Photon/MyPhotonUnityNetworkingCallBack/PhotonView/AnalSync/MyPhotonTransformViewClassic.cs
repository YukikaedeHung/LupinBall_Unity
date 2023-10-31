// ----------------------------------------------------------------------------
// <copyright file="PhotonTransformViewClassic.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Component to synchronize Transforms via PUN PhotonView.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------
using Photon.Pun;

namespace Tim
{
    using UnityEngine;
    using System.Collections.Generic;


    /// <summary>
    /// 這個類幫助你同步位置、旋轉和縮放
    /// 一個遊戲對象。 它還為您提供了許多不同的選擇
    /// 同步值看起來平滑，即使數據只有
    /// 每秒發送幾次。
    /// 只需將組件添加到您的遊戲對象並確保
    /// 將 PhotonTransformViewClassic 添加到觀察到的組件列表中
    /// </summary>
    [AddComponentMenu("My Photon Networking/My Photon Transform View Classic")]
    public class MyPhotonTransformViewClassic : MonoBehaviourPun, IPunObservable
    {
        //As this component is very complex, we separated it into multiple classes.
        //The PositionModel, RotationModel and ScaleMode store the data you are able to
        //configure in the inspector while the "control" objects below are actually moving
        //the object and calculating all the inter- and extrapolation

        /// <summary>
        /// ★臨時加入的玩家，過往遊戲物件產生後位置會先在原始位置，之後透過m_firstTake來判定第一次抓取資料，但拿取過程太慢所以物件在畫面會瞬移。
        /// </summary>
        public GameObject objDisplay;
        public Transform trPosition;
        public Transform trRotation;
        public Transform trScale;
        public Rigidbody rbVelocity;
        public Rigidbody rbAngularVelocity;

        [HideInInspector]
        public PhotonTransformViewPositionModel m_PositionModel = new PhotonTransformViewPositionModel();

        [HideInInspector]
        public PhotonTransformViewRotationModel m_RotationModel = new PhotonTransformViewRotationModel();

        [HideInInspector]
        public PhotonTransformViewScaleModel m_ScaleModel = new PhotonTransformViewScaleModel();

        PhotonTransformViewPositionControl m_PositionControl;
        PhotonTransformViewRotationControl m_RotationControl;
        PhotonTransformViewScaleControl m_ScaleControl;

        PhotonView m_PhotonView;

        /// <summary>
        /// 是否接收網路更新
        /// </summary>
        bool m_ReceivedNetworkUpdate = false;

        /// <summary>
        /// Flag to skip initial data when Object is instantiated and rely on the first deserialized data instead.
        /// </summary>
        bool m_firstTake = false;

        public bool isUpdate;

        void Awake()
        {
            this.m_PhotonView = GetComponent<PhotonView>();

            this.m_PositionControl = new PhotonTransformViewPositionControl(this.m_PositionModel);
            this.m_RotationControl = new PhotonTransformViewRotationControl(this.m_RotationModel);
            this.m_ScaleControl = new PhotonTransformViewScaleControl(this.m_ScaleModel);
        }

        void OnEnable()
        {
            m_firstTake = true;
        }

        private void Start()
        {
            if (m_firstTake == true && objDisplay != null)//檢查 : 因為OnPhotonSerializeView 有時執行順序比Start早，會導致obj開了又關
            {
                objDisplay.SetActive(false);
                if (MyPhotonNetwork.PlayerListOthers.Length == 0 || objDisplay.GetComponent<PhotonView>().IsMine)
                    objDisplay.SetActive(true);
            }
        }

        void Update()
        {
            if (!isUpdate || this.m_PhotonView == null || this.m_PhotonView.IsMine == true || PhotonNetwork.IsConnectedAndReady == false)
            {
                return;
            }

            this.UpdatePosition();
            this.UpdateRotation();
            this.UpdateScale();
        }

        void FixedUpdate()
        {
            if (isUpdate || this.m_PhotonView == null || this.m_PhotonView.IsMine == true || PhotonNetwork.IsConnectedAndReady == false)
            {
                return;
            }
            this.UpdatePosition();
            this.UpdateRotation();
            this.UpdateScale();
        }

        void UpdatePosition()
        {
            if (this.m_PositionModel.SynchronizeEnabled == false || this.m_ReceivedNetworkUpdate == false)
            {
                return;
            }
            //除了第一次讀取資料的更新位置，其餘都在這更新
                trPosition.localPosition = this.m_PositionControl.UpdatePosition(trPosition.localPosition, rbVelocity);
        }

        void UpdateRotation()
        {
            if (this.m_RotationModel.SynchronizeEnabled == false || this.m_ReceivedNetworkUpdate == false)
            {
                return;
            }

            trRotation.localRotation = this.m_RotationControl.GetRotation(trRotation.localRotation, rbAngularVelocity);
        }

        void UpdateScale()
        {
            if (this.m_ScaleModel.SynchronizeEnabled == false || this.m_ReceivedNetworkUpdate == false)
            {
                return;
            }

            trScale.localScale = this.m_ScaleControl.GetScale(trScale.localScale);
        }

        ///// <summary>
        ///// 如果插值模式，這些值同步到遠程對象
        ///// 或使用外推模式 SynchronizeValues。 你的運動腳本應該傳遞
        ///// 當前速度（單位/秒）和轉彎速度（角度/秒）所以遙控器
        ///// 對象可以使用它們來預測對象的運動。
        ///// </summary>
        ///// <param name="speed">The current movement vector of the object in units/second.</param>
        ///// <param name="turnSpeed">The current turn speed of the object in angles/second.</param>
        //public void SetSynchronizedValues(Vector3 speed, float turnSpeed)
        //{
        //    this.m_PositionControl.SetSynchronizedValues(speed, turnSpeed);
        //}

        /// <summary>
        /// IPunObservable
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            //讀寫資料
            this.m_PositionControl.OnPhotonSerializeView(trPosition.localPosition, stream, info);
            this.m_RotationControl.OnPhotonSerializeView(trRotation.localRotation, stream, info);
            this.m_ScaleControl.OnPhotonSerializeView(trScale.localScale, stream, info);

            //接收讀取
            if (stream.IsReading == true)
            {
                //Debug.Log("Read");
                this.m_ReceivedNetworkUpdate = true;

                // 強制最新數據以避免在玩家實例化時出現初始漂移。
                if (m_firstTake)//第一次取資料
                {
                    m_firstTake = false;

                    if (this.m_PositionModel.SynchronizeEnabled)
                    {
                        trPosition.localPosition = this.m_PositionControl.GetNetworkPosition();
                    }

                    if (this.m_RotationModel.SynchronizeEnabled)
                    {
                        trRotation.localRotation = this.m_RotationControl.GetNetworkRotation();
                    }

                    if (this.m_ScaleModel.SynchronizeEnabled)
                    {
                        trScale.localScale = this.m_ScaleControl.GetNetworkScale();
                    }

                    if (objDisplay != null)/*★自加*/
                    {
                        objDisplay.SetActive(true);
                    }
                }
            }
            else
            {
                //Debug.Log("Write");
                if (objDisplay != null)/*★自加*/
                    objDisplay.SetActive(true);
            }
        }
    }


    [System.Serializable]
    public class PhotonTransformViewPositionModel
    {
        /// <summary>
        /// 插值選項
        /// </summary>
        public enum InterpolateOptions
        {
            Disabled,//已禁用
            FixedSpeed,//固定速度
            EstimatedSpeed,//估計速度
            SynchronizeValues, //同步值
            Lerp//插值
        }

        /// <summary>
        /// 外推選項
        /// </summary>
        public enum ExtrapolateOptions
        {
            Disabled,//已禁用
            SynchronizeValues,//同步值
            EstimateSpeedAndTurn,//估計速度和轉彎
            FixedSpeed,//固定速度
        }

        /// <summary>
        /// 同步已啟用
        /// </summary>
        public bool SynchronizeEnabled;

        /// <summary>
        /// 傳送啟用
        /// </summary>
        public bool TeleportEnabled = true;
        /// <summary>
        /// 傳送如果距離大於
        /// </summary>
        public float TeleportIfDistanceGreaterThan = 3f;

        /// <summary>
        /// 插值選項
        /// </summary>
        public InterpolateOptions InterpolateOption = InterpolateOptions.EstimatedSpeed;
        /// <summary>
        /// 插值邁向速度
        /// </summary>
        public float InterpolateMoveTowardsSpeed = 1f;
        /// <summary>
        /// 插值速度
        /// </summary>
        public float InterpolateLerpSpeed = 1f;

        /// <summary>
        /// 外推選項
        /// </summary>
        public ExtrapolateOptions ExtrapolateOption = ExtrapolateOptions.Disabled;
        /// <summary>
        /// 外推速度
        /// </summary>
        public float ExtrapolateSpeed = 1f;
        /// <summary>
        /// 外推包括往返時間
        /// </summary>
        public bool ExtrapolateIncludingRoundTripTime = true;
        /// <summary>
        /// 外推存儲位置的數量
        /// </summary>
        public int ExtrapolateNumberOfStoredPositions = 1;
    }

    public class PhotonTransformViewPositionControl
    {
        PhotonTransformViewPositionModel m_Model;
        /// <summary>
        /// 當前速度
        /// </summary>
        float m_CurrentSpeed;
        /// <summary>
        /// 上次序列化時間
        /// </summary>
        double m_LastSerializeTime;
        /// <summary>
        /// 同步速度
        /// </summary>
        Vector3 m_SynchronizedSpeed = Vector3.zero;
        /// <summary>
        /// 同步轉彎速度
        /// </summary>
        float m_SynchronizedTurnSpeed = 0;

        /// <summary>
        /// 網路位置
        /// </summary>
        Vector3 m_NetworkPosition;
        /// <summary>
        /// 列表-舊的網路位置
        /// </summary>
        Queue<Vector3> m_OldNetworkPositions = new Queue<Vector3>();

        /// <summary>
        /// 序列化後是否已經更新位置
        /// </summary>
        bool m_UpdatedPositionAfterOnSerialize = true;

        public PhotonTransformViewPositionControl(PhotonTransformViewPositionModel model)
        {
            m_Model = model;
        }

        /// <summary>
        /// 獲取最舊的存儲網絡位置
        /// </summary>
        /// <returns></returns>
        Vector3 GetOldestStoredNetworkPosition()
        {
            Vector3 oldPosition = m_NetworkPosition;

            if (m_OldNetworkPositions.Count > 0)
            {
                oldPosition = m_OldNetworkPositions.Peek();
            }

            return oldPosition;
        }

        /// <summary>
        /// ★★★★ 根本沒用
        /// 如果插值模式，這些值同步到遠程對象 
        /// 或使用外推模式 SynchronizeValues。 你的運動腳本應該傳遞
        /// 當前速度（單位/秒）和轉彎速度（角度/秒）所以遙控器
        /// 對象可以使用它們來預測對象的運動。
        /// </summary>
        /// <param name="speed">對象的當前移動向量，單位/秒。</param>
        /// <param name="turnSpeed">以角度/秒為單位的對象當前轉動速度。</param>
        public void SetSynchronizedValues(Vector3 speed, float turnSpeed)
        {
            Debug.Log("★★★★ 根本沒用");
            m_SynchronizedSpeed = speed;
            m_SynchronizedTurnSpeed = turnSpeed;
        }

        /// <summary>
        /// "InterpolateOption" 根據檢查器中設置的值計算新位置
        /// </summary>
        /// <param name="currentPosition">當前位置。</param>
        /// <returns>位置</returns>
        public Vector3 UpdatePosition(Vector3 currentPosition, Rigidbody rbVelocity)
        {
            if(rbVelocity!=null &&  Vector3.Distance(Vector3.zero, rbVelocity.velocity) != 0)
            {
                float m_Distance = Vector3.Distance(currentPosition, m_NetworkPosition);
                currentPosition = Vector3.MoveTowards(currentPosition, this.m_NetworkPosition, m_Distance * Time.deltaTime * 10);
                return currentPosition;
            }

            //Debug.Log(string.Format("targetPosition{0} = GetNetworkPosition(){1} + GetExtrapolatedPositionOffset(){2}",
            //    GetNetworkPosition() + GetExtrapolatedPositionOffset(),
            //    GetNetworkPosition(),
            //    GetExtrapolatedPositionOffset()
            //    ));
            Vector3 targetPosition = GetNetworkPosition() + GetExtrapolatedPositionOffset();
            //Debug.Log("targetPosition.x : " + targetPosition.x);
            if(Vector3.Distance(currentPosition, GetNetworkPosition()) < Vector3.Distance(new Vector3(0,0,0), GetExtrapolatedPositionOffset()))
            {
                Debug.Log("★★★外推取消★★★");
                targetPosition = GetNetworkPosition();
            }

            switch (m_Model.InterpolateOption)
            {
                case PhotonTransformViewPositionModel.InterpolateOptions.Disabled:
                    if (m_UpdatedPositionAfterOnSerialize == false)
                    {
                        currentPosition = targetPosition;
                        m_UpdatedPositionAfterOnSerialize = true;
                    }

                    break;

                case PhotonTransformViewPositionModel.InterpolateOptions.FixedSpeed://固定速度 currentPosition更新位置 = Vector3.MoveTowards(currentPosition當前位置, targetPosition外推預測目標位置, Time.deltaTime * m_Model.InterpolateMoveTowardsSpeed(插值邁向速度，外部設定));
                    Debug.Log(string.Format("currentPosition{0} = Vector3.MoveTowards(currentPosition{1}, targetPosition{2}, Time.deltaTime * m_Model.InterpolateMoveTowardsSpeed{3}:{4})",
                        Vector3.MoveTowards(currentPosition, targetPosition, Time.deltaTime * m_Model.InterpolateMoveTowardsSpeed),
                        currentPosition,
                        targetPosition,
                        m_Model.InterpolateMoveTowardsSpeed,
                        Time.deltaTime * m_Model.InterpolateMoveTowardsSpeed
                        ));
                    currentPosition = Vector3.MoveTowards(currentPosition, targetPosition, Time.deltaTime * m_Model.InterpolateMoveTowardsSpeed);
                    break;

                case PhotonTransformViewPositionModel.InterpolateOptions.EstimatedSpeed://估計速度 currentPosition更新位置 = Vector3.MoveTowards(currentPosition當前位置, targetPosition外推預測目標位置, Time.deltaTime * (Vector3.Distance(m_NetworkPosition現在網路位置, GetOldestStoredNetworkPosition()上次網路位置) / m_OldNetworkPositions.Count儲存舊網路位置資料筆數) * PhotonNetwork.SerializationRate傳輸頻率);
                    if (m_OldNetworkPositions.Count == 0)
                    {
                        // 特殊情況：我們在內存中沒有以前的更新，所以我們無法猜測速度！
                        break;
                    }

                    // 知道最後（傳入）位置和之前的位置，我們可以猜測速度。
                    // 注意速度是sendRateOnSerialize的倍數！ 我們發送 X 個更新/秒，因此我們的估計必須將其考慮在內。
                    //Debug.Log(string.Format("estimatedSpeed{0} = (Vector3.Distance(m_NetworkPosition{1}, GetOldestStoredNetworkPosition(){2}) / m_OldNetworkPositions.Count{3}) * PhotonNetwork.SerializationRate{4}",
                    //    Vector3.Distance(m_NetworkPosition, GetOldestStoredNetworkPosition()) / m_OldNetworkPositions.Count * PhotonNetwork.SerializationRate,
                    //    m_NetworkPosition,
                    //    GetOldestStoredNetworkPosition(),
                    //    m_OldNetworkPositions.Count,
                    //    PhotonNetwork.SerializationRate
                    //    ));
                    float estimatedSpeed = 0;
                    estimatedSpeed = Vector3.Distance(m_NetworkPosition, GetOldestStoredNetworkPosition()) * PhotonNetwork.SerializationRate * Time.deltaTime;
                    if (m_Model.ExtrapolateOption != PhotonTransformViewPositionModel.ExtrapolateOptions.Disabled && GetExtrapolatedPositionOffset() == new Vector3(0, 0, 0))
                    {
                        estimatedSpeed = Vector3.Distance(currentPosition, targetPosition) * Time.deltaTime;
                        Debug.Log("改estimatedSpeed : " + estimatedSpeed);
                    }
                    //Debug.Log(string.Format("currentPosition{0} = Vector3.MoveTowards(currentPosition{1}, targetPosition{2}, estimatedSpeed:{3});",
                    //    Vector3.MoveTowards(currentPosition, targetPosition, estimatedSpeed),
                    //    currentPosition,
                    //    targetPosition,
                    //    estimatedSpeed
                    //    ));
                    // 以根據上次更新計算的速度向 targetPosition（包括估計值，如果它處於活動狀態）移動。
                    currentPosition = Vector3.MoveTowards(currentPosition, targetPosition, estimatedSpeed);
                    //Debug.Log("currentPosition.x : " + currentPosition.x);
                    break;

                case PhotonTransformViewPositionModel.InterpolateOptions.SynchronizeValues://同步值
                    if (m_SynchronizedSpeed.magnitude == 0)//magnitude 矢量??? 
                    {
                        Debug.Log("m_SynchronizedSpeed:"+ m_SynchronizedSpeed);
                        currentPosition = targetPosition;
                    }
                    else //(★★ m_SynchronizedSpeed一值都是0阿，這裡進不來)
                    {
                        Debug.Log("★★★★ 根本沒用");
                        Debug.Log(string.Format("currentPosition{0} = Vector3.MoveTowards(currentPosition{1}, targetPosition{2}, Time.deltaTime * m_SynchronizedSpeed.magnitude{3}:{4})",
                            Vector3.MoveTowards(currentPosition, targetPosition, Time.deltaTime * m_SynchronizedSpeed.magnitude),
                            currentPosition,
                            targetPosition,
                             m_SynchronizedSpeed.magnitude,
                             Time.deltaTime * m_SynchronizedSpeed.magnitude
                            ));
                        currentPosition = Vector3.MoveTowards(currentPosition, targetPosition, Time.deltaTime * m_SynchronizedSpeed.magnitude);
                    }
                    break;

                case PhotonTransformViewPositionModel.InterpolateOptions.Lerp://插值 currentPosition更新位置 = Vector3.Lerp(currentPosition當前位置, targetPosition外推預測目標位置, Time.deltaTime * m_Model.InterpolateLerpSpeed插值速度外部設定)
                    Debug.Log(string.Format("currentPosition{0} = Vector3.Lerp(currentPosition{1}, targetPosition{2}, Time.deltaTime * m_Model.InterpolateLerpSpeed{3}:{4})",
                        Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * m_Model.InterpolateLerpSpeed),
                        currentPosition,
                        targetPosition,
                        m_Model.InterpolateLerpSpeed,
                        Time.deltaTime * m_Model.InterpolateLerpSpeed
                        ));
                    currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * m_Model.InterpolateLerpSpeed);
                    break;
            }

            if (m_Model.TeleportEnabled == true)
            {
                if (Vector3.Distance(currentPosition, GetNetworkPosition()) > m_Model.TeleportIfDistanceGreaterThan)//(當前位置 - 上次網路位置) > m_Model.TeleportIfDistanceGreaterThan 大於指定傳送距離，執行瞬移。
                {
                    currentPosition = GetNetworkPosition();
                }
            }

            return currentPosition;
        }

        /// <summary>
        /// 獲取通過網絡收到的最後一個位置
        /// </summary>
        /// <returns></returns>
        public Vector3 GetNetworkPosition()
        {
            return m_NetworkPosition;
        }

        /// <summary>
        /// "ExtrapolateOption" 根據上次同步位置計算估計位置，
        /// 接收到最後一個位置的時間和物體的移動速度(移動量)
        /// </summary>
        /// <returns>遠程物體的估計位置(移動量)</returns>
        public Vector3 GetExtrapolatedPositionOffset()
        {
            float timePassed = (float)(PhotonNetwork.Time - m_LastSerializeTime);//現在時間-上次收資料(序列化)時間=上次接收資料所花時間

            if (m_Model.ExtrapolateIncludingRoundTripTime == true)
            {
                timePassed += (float)PhotonNetwork.GetPing() / 1000f;//上次接收資料所花時間 += 資料往返ping時間
            }

            Vector3 extrapolatePosition = Vector3.zero;//外推位置(預測位置)

            if (m_SynchronizedSpeed != new Vector3(0,0,0) || m_SynchronizedTurnSpeed != 0)//★★這兩值一值都是0，選了ExtrapolateOptions.SynchronizeValues的extrapolatePosition也只會是0
            {
                Debug.Log("★★★★ 根本沒用");
                Debug.Log(string.Format("m_SynchronizedTurnSpeed{0}, m_SynchronizedTurnSpeed:{1}", m_SynchronizedTurnSpeed, m_SynchronizedTurnSpeed));
            }

            switch (m_Model.ExtrapolateOption)
            {
                //case PhotonTransformViewPositionModel.ExtrapolateOptions.SynchronizeValues://預測轉彎角度與行進速度(★★★★完全沒屁用.....一直都是0(m_SynchronizedSpeed、m_SynchronizedTurnSpeed))
                //    Debug.Log(string.Format("turnRotation{0} = Quaternion.Euler(0, m_SynchronizedTurnSpeed{1} * timePassed{2}:{3}, 0)",
                //        Quaternion.Euler(0, m_SynchronizedTurnSpeed * timePassed, 0),
                //        m_SynchronizedTurnSpeed,
                //        timePassed,
                //        m_SynchronizedTurnSpeed * timePassed
                //        ));
                //    Quaternion turnRotation = Quaternion.Euler(0, m_SynchronizedTurnSpeed * timePassed, 0);
                //    Debug.Log(string.Format("extrapolatePosition{0} = turnRotation{1} * (m_SynchronizedSpeed{2} * timePassed{3})",
                //        turnRotation * (m_SynchronizedSpeed * timePassed),
                //        turnRotation,
                //        m_SynchronizedSpeed,
                //        timePassed
                //        ));
                //    extrapolatePosition = turnRotation * (m_SynchronizedSpeed * timePassed);
                    //break;
                case PhotonTransformViewPositionModel.ExtrapolateOptions.FixedSpeed://extrapolatePosition預測移動量 = (moveDirection(現在網路位置-上次網路位置) * m_Model.ExtrapolateSpeed外推速度(預測速度，外部設定) * timePassed(上次接收資料所花時間))
                    //Debug.Log(string.Format("moveDirection{0} = (m_NetworkPosition{1} - GetOldestStoredNetworkPosition(){2}).normalized",
                    //    (m_NetworkPosition - GetOldestStoredNetworkPosition()).normalized,
                    //    m_NetworkPosition,
                    //    GetOldestStoredNetworkPosition()
                    //    ));
                    Vector3 moveDirection = (m_NetworkPosition - GetOldestStoredNetworkPosition()).normalized;/*(30,5,0)-(10,0,0)=(20,5,0)->(1, 0.25f, 0)*/
                    //Debug.Log(string.Format("extrapolatePosition{0} = moveDirection{1} * m_Model.ExtrapolateSpeed{2} * timePassed{3}",
                    //    moveDirection * m_Model.ExtrapolateSpeed * timePassed,
                    //    moveDirection,
                    //    m_Model.ExtrapolateSpeed,
                    //    timePassed
                    //    ));
                    extrapolatePosition = moveDirection * m_Model.ExtrapolateSpeed * timePassed;
                    break;
                case PhotonTransformViewPositionModel.ExtrapolateOptions.EstimateSpeedAndTurn://extrapolatePosition預測移動量 = ((現在網路位置 - 上次網路位置) * PhotonNetwork.SerializationRate傳輸頻率) * timePassed(上次接收資料所花時間);
                    Debug.Log(string.Format("moveDelta{0} = (m_NetworkPosition{1} - GetOldestStoredNetworkPosition(){2}) * PhotonNetwork.SerializationRate{3}",
                        (m_NetworkPosition - GetOldestStoredNetworkPosition()) * PhotonNetwork.SerializationRate,
                        m_NetworkPosition,
                        GetOldestStoredNetworkPosition(),
                        PhotonNetwork.SerializationRate
                        ));
                    Vector3 moveDelta = (m_NetworkPosition - GetOldestStoredNetworkPosition()) * PhotonNetwork.SerializationRate;
                    Debug.Log(string.Format("extrapolatePosition{0} = moveDelta{1} * timePassed{2}",
                        moveDelta * timePassed,
                        moveDelta,
                        timePassed
                        ));
                    extrapolatePosition = moveDelta * timePassed;
                    break;
            }

            return extrapolatePosition;
        }

        /// <summary>
        /// 配合MyPhotonTransformViewClassic:IPunObservable
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        public void OnPhotonSerializeView(Vector3 currentPosition, PhotonStream stream, PhotonMessageInfo info)
        {
            if (m_Model.SynchronizeEnabled == false)
            {
                return;
            }

            if (stream.IsWriting == true)
            {
                SerializeData(currentPosition, stream, info);
            }
            else
            {
                DeserializeData(stream, info);
            }

            m_LastSerializeTime = PhotonNetwork.Time;
            m_UpdatedPositionAfterOnSerialize = false;
        }

        /// <summary>
        /// 序列化數據
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        void SerializeData(Vector3 currentPosition, PhotonStream stream, PhotonMessageInfo info)
        {
            stream.SendNext(currentPosition);
            m_NetworkPosition = currentPosition;

            if (m_Model.ExtrapolateOption == PhotonTransformViewPositionModel.ExtrapolateOptions.SynchronizeValues ||
                m_Model.InterpolateOption == PhotonTransformViewPositionModel.InterpolateOptions.SynchronizeValues)
            {
                stream.SendNext(m_SynchronizedSpeed);
                stream.SendNext(m_SynchronizedTurnSpeed);
            }
        }

        /// <summary>
        /// 反序列化數據
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        void DeserializeData(PhotonStream stream, PhotonMessageInfo info)
        {
            Vector3 readPosition = (Vector3)stream.ReceiveNext();
            if (m_Model.ExtrapolateOption == PhotonTransformViewPositionModel.ExtrapolateOptions.SynchronizeValues ||
                m_Model.InterpolateOption == PhotonTransformViewPositionModel.InterpolateOptions.SynchronizeValues)
            {
                m_SynchronizedSpeed = (Vector3)stream.ReceiveNext();
                m_SynchronizedTurnSpeed = (float)stream.ReceiveNext();
            }

            if (m_OldNetworkPositions.Count == 0)
            {
                // 如果我們還沒有舊職位，這是該客戶讀取的第一個更新。 讓我們將此用作當前和舊位置。
                m_NetworkPosition = readPosition;
            }

            // 先前收到的位置成為舊的（er）位置並排隊。 新的是 m_NetworkPosition
            m_OldNetworkPositions.Enqueue(m_NetworkPosition);
            m_NetworkPosition = readPosition;

            // 將隊列中的項目減少到定義數量的存儲位置。
            while (m_OldNetworkPositions.Count > m_Model.ExtrapolateNumberOfStoredPositions)
            {
                m_OldNetworkPositions.Dequeue();
            }
        }
    }


    [System.Serializable]
    public class PhotonTransformViewRotationModel
    {
        public enum InterpolateOptions
        {
            Disabled,
            RotateTowards,
            Lerp,
        }


        public bool SynchronizeEnabled;

        public InterpolateOptions InterpolateOption = InterpolateOptions.RotateTowards;
        public float InterpolateRotateTowardsSpeed = 180;
        public float InterpolateLerpSpeed = 5;
    }

    public class PhotonTransformViewRotationControl
    {
        PhotonTransformViewRotationModel m_Model;
        Quaternion m_NetworkRotation;

        public PhotonTransformViewRotationControl(PhotonTransformViewRotationModel model)
        {
            m_Model = model;
        }

        /// <summary>
        /// Gets the last rotation that was received through the network
        /// </summary>
        /// <returns></returns>
        public Quaternion GetNetworkRotation()
        {
            return m_NetworkRotation;
        }

        public Quaternion GetRotation(Quaternion currentRotation, Rigidbody rbAngularVelocity)
        {
            if (rbAngularVelocity!=null && Vector3.Distance(Vector3.zero, rbAngularVelocity.angularVelocity) != 0)
            {
                float m_Angle = Quaternion.Angle(currentRotation, m_NetworkRotation);
                currentRotation = Quaternion.RotateTowards(currentRotation, this.m_NetworkRotation, m_Angle * Time.deltaTime * 10);
                return currentRotation;
            }

            switch (m_Model.InterpolateOption)
            {
                default:
                case PhotonTransformViewRotationModel.InterpolateOptions.Disabled:
                    return m_NetworkRotation;
                case PhotonTransformViewRotationModel.InterpolateOptions.RotateTowards:
                    return Quaternion.RotateTowards(currentRotation, m_NetworkRotation, m_Model.InterpolateRotateTowardsSpeed * Time.deltaTime);
                case PhotonTransformViewRotationModel.InterpolateOptions.Lerp:
                    return Quaternion.Lerp(currentRotation, m_NetworkRotation, m_Model.InterpolateLerpSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// 配合MyPhotonTransformViewClassic:IPunObservable
        /// </summary>
        /// <param name="currentRotation"></param>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        public void OnPhotonSerializeView(Quaternion currentRotation, PhotonStream stream, PhotonMessageInfo info)
        {
            if (m_Model.SynchronizeEnabled == false)
            {
                return;
            }

            if (stream.IsWriting == true)
            {
                stream.SendNext(currentRotation);
                m_NetworkRotation = currentRotation;
            }
            else
            {
                m_NetworkRotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }


    [System.Serializable]
    public class PhotonTransformViewScaleModel
    {
        public enum InterpolateOptions
        {
            Disabled,
            MoveTowards,
            Lerp,
        }


        public bool SynchronizeEnabled;

        public InterpolateOptions InterpolateOption = InterpolateOptions.Disabled;
        public float InterpolateMoveTowardsSpeed = 1f;
        public float InterpolateLerpSpeed;
    }

    public class PhotonTransformViewScaleControl
    {
        PhotonTransformViewScaleModel m_Model;
        Vector3 m_NetworkScale = Vector3.one;

        public PhotonTransformViewScaleControl(PhotonTransformViewScaleModel model)
        {
            m_Model = model;
        }

        /// <summary>
        /// Gets the last scale that was received through the network
        /// </summary>
        /// <returns></returns>
        public Vector3 GetNetworkScale()
        {
            return m_NetworkScale;
        }

        public Vector3 GetScale(Vector3 currentScale)
        {
            switch (m_Model.InterpolateOption)
            {
                default:
                case PhotonTransformViewScaleModel.InterpolateOptions.Disabled:
                    return m_NetworkScale;
                case PhotonTransformViewScaleModel.InterpolateOptions.MoveTowards:
                    return Vector3.MoveTowards(currentScale, m_NetworkScale, m_Model.InterpolateMoveTowardsSpeed * Time.deltaTime);
                case PhotonTransformViewScaleModel.InterpolateOptions.Lerp:
                    return Vector3.Lerp(currentScale, m_NetworkScale, m_Model.InterpolateLerpSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// 配合MyPhotonTransformViewClassic:IPunObservable
        /// </summary>
        /// <param name="currentScale"></param>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        public void OnPhotonSerializeView(Vector3 currentScale, PhotonStream stream, PhotonMessageInfo info)
        {
            if (m_Model.SynchronizeEnabled == false)
            {
                return;
            }

            if (stream.IsWriting == true)
            {
                stream.SendNext(currentScale);
                m_NetworkScale = currentScale;
            }
            else
            {
                m_NetworkScale = (Vector3)stream.ReceiveNext();
            }
        }
    }
}