using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Tim.Game;
using Tim;

namespace Tim.Game
{
    public class CharaScript : MonoBehaviour
    {
        [Header("Character")]
        public TriggerScript TriggerScript_Jab;
        private PhotonView pvChara;
        private Rigidbody rdChara;
        private Transform trChara;
        public Animator aniChara;

        [Header("Camera")]
        public CameraScript _cameraScript;
        public Transform trCamera;
        private Transform trCameraDirection;

        [Header("GameObject")]
        public GameObject objChara;
        public GameObject objPersonal;
        public GameObject objGameMenu;

        [Header("BulletSetting")]
        public Transform trBulletCreatePonit;
        private GameObject objBullet;
        public const string strPathPreBullet = "Prefab/02. preBullet/";
        public float[] fBulletSpeed = new float[4];

        [Header("UI Setting")]
        public Image imgBulletTip0;
        public Image imgBulletTip1;
        public Image imgBulletTip2;
        public Image imgBulletTip3;
        public Image imgPlayerWin;
        public Image imgPlayerLose;
        public Text txtScoreCount;
        public Text txtBulletCount0;
        public Text txtBulletCount1;
        public Text txtBulletCount2;
        public Text txtBulletCount3;
        public Text txtEndGameTip;
        public Text txtDebugInfo;
        public Sprite sprFrameSelect;
        public Sprite sprFrameNotSelect;
        public Button btnBackToLobby;
        public Button btnLeaveGame;
        private Image[] imgBulletTip;

        [Header("Prefab")]
        public GameObject preBullet1;
        public GameObject preBullet2;

        [Header("Variable")]
        public EnSupplies enSupplies; //當前槍種設定，預設為Handgun
        public float fRunSpeed; //Unity面板設定值
        public float fRotationSmoothTime; //Unity面板設定值
        public float fRotationSpeed; //Unity面板設定值
        public bool bGameMenu;
        public bool bCtrl;
        public int[] iDamage = new int[4]; //Unity面板設定值
        private int iAtkType;
        private int[] iBulletCount = new int[4];
        private int iScore;
        private float fVelocity;
        private ExitGames.Client.Photon.Hashtable htGetDead;

        private void Awake()
        {
            pvChara = PhotonView.Get(this);

            //判斷pvChara是否屬於自己，如果不是則開啟pvChara中Collider的isTrigger功能
            if (!pvChara.IsMine)
            {
                GetComponent<Collider>().isTrigger = true;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            bCtrl = true;

            if (!pvChara.IsMine)
            {
                objPersonal.SetActive(false);
                return;
            }
            else
            {
                Initial();
                BtnControl();
            }
        }

        // Update is called once per frame
        void Update()
        {
            //確認物體所有權在我方IsMine，如果不是就return
            if (!pvChara.IsMine)
            {
                return;
            }

            CharaAction();
            //TxtDebugInfo();
        }

        #region 初始化 Initial()
        void Initial()
        {
            GameManager.inst.objPlayer = gameObject;
            rdChara = GetComponent<Rigidbody>();
            htGetDead = new ExitGames.Client.Photon.Hashtable();
            htGetDead.Add("DEAD", false);
            htGetDead.Add("PLAYERID", PhotonView.Get(this).ViewID);
            MyPhotonNetwork.SetPlayerCustomProperties(htGetDead);

            bGameMenu = false;

            imgBulletTip = new Image[4] { imgBulletTip0, imgBulletTip1, imgBulletTip2, imgBulletTip3 };

            //生成一個物件取得Camera相關位置作為後續角色移動的依據
            GameObject objCameraDir = new GameObject();
            objCameraDir.transform.parent = transform;
            objCameraDir.transform.localPosition = Vector3.zero;
            objCameraDir.name = "CameraDir";
            trCameraDirection = objCameraDir.transform;

            //預設槍種
            enSupplies = EnSupplies.Handgun;
            iAtkType = 0;

            iScore = GameManager.inst.iScore;
            UpdateScoreCount(iScore);
            MouseEvent();
        }
        #endregion

        #region Button操作 BtnControl()
        void BtnControl()
        {
            btnBackToLobby.onClick.AddListener(delegate ()
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    StartCoroutine(CoMasterLeaveRoom());
                }
                else
                {
                    BackToLobby();
                }
                Debug.Log("Back To Lobby");
            });
            btnLeaveGame.onClick.AddListener(delegate ()
            {
                Application.Quit();
                Debug.Log("Leave Game");
            });
        }
        #endregion

        #region 離開遊戲房間回到大廳 BackToLobby();
        void BackToLobby()
        {
            GameSceneScript.PreviousScene = GameSceneScript.SceneName_GameScene;
            PhotonNetwork.LoadLevel(GameSceneScript.SceneName_GameLogIn);
            objGameMenu.SetActive(false);
            MyPhotonNetwork.LeaveRoom();
        }
        #endregion

        #region 操作者的角色動作
        #region 基本移動 CharaAction()
        void CharaAction()
        {
            #region 遊戲進行中
            if (!GameManager.inst.bEndGame && trCameraDirection != null && trCamera != null)
            {
                #region  多方位移動
                //基本物體移動
                //獲取Camera的Y方向旋轉值讓角色的正面朝向該方向
                if (trCamera.transform)
                {
                    trCameraDirection.eulerAngles = new Vector3(0, trCamera.transform.eulerAngles.y, 0);
                }

                float fY = trCameraDirection.eulerAngles.y;
                bool bCharaMoving = false;
                Vector3 moveV3 = new Vector3();

                if (Input.GetKey(KeyCode.W))
                {
                    moveV3 = trCameraDirection.forward;
                    bCharaMoving = true;
                    objChara.transform.localEulerAngles = new Vector3(0, fY + 0, 0);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    moveV3 = -trCameraDirection.forward;
                    bCharaMoving = true;
                    objChara.transform.localEulerAngles = new Vector3(0, fY + 180, 0);
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    moveV3 = -trCameraDirection.right;
                    bCharaMoving = true;
                    objChara.transform.localEulerAngles = new Vector3(0, fY + 270, 0);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    moveV3 = trCameraDirection.right;
                    bCharaMoving = true;
                    objChara.transform.localEulerAngles = new Vector3(0, fY + 90, 0);
                }
                else
                {
                    moveV3 = Vector3.zero;
                    bCharaMoving = false;
                }
                CharaMovingEven(bCharaMoving, moveV3);

                #endregion

                #region 攻擊
                if (Input.GetMouseButtonDown(0) && aniChara.GetInteger("ATK") == 0 && !bGameMenu)
                {
                    //滑鼠左鍵攻擊Kick
                    bCtrl = false;
                    aniChara.SetInteger("ATK", 1);
                }
                #endregion

                #region 武器選擇事件
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    int iAtkTypeBefore = iAtkType;
                    //按下數字鍵1之後將武器類型設成 Handgun
                    enSupplies = EnSupplies.Handgun;
                    iAtkType = 0;
                    //圖示提示框切換到第1格
                    ImgBulletTypeSelectReset(iAtkTypeBefore, iAtkType);
                }

                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    int iAtkTypeBefore = iAtkType;
                    //按下數字鍵1之後將武器類型設成 Shotgun
                    enSupplies = EnSupplies.Shotgun;
                    iAtkType = 1;
                    //圖示提示框切換到第2格
                    ImgBulletTypeSelectReset(iAtkTypeBefore, iAtkType);
                }

                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    int iAtkTypeBefore = iAtkType;
                    //按下數字鍵1之後將武器類型設成 Rilfe
                    enSupplies = EnSupplies.Rifle;
                    iAtkType = 2;
                    //圖示提示框切換到第3格
                    ImgBulletTypeSelectReset(iAtkTypeBefore, iAtkType);
                }

                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    int iAtkTypeBefore = iAtkType;
                    //按下數字鍵1之後將武器類型設成 RPG
                    enSupplies = EnSupplies.RPG;
                    iAtkType = 3;
                    //圖示提示框切換到第4格
                    ImgBulletTypeSelectReset(iAtkTypeBefore, iAtkType);
                }
                #endregion

                #region GameMenu開啟
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    bGameMenu = !bGameMenu;
                    MouseEvent();

                    objGameMenu.SetActive(bGameMenu);
                    _cameraScript.bActive = !bGameMenu;
                }
                #endregion
            }
            #endregion

            #region 遊戲結束畫面開啟後，按下Enter鍵返回Lobby
            if (GameManager.inst.bEndGame)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        StartCoroutine(CoMasterLeaveRoom());
                    }
                    else
                    {
                        //切換場景回到登入的Lobby選單介面
                        BackToLobby();
                        Debug.Log("Back To Lobby");
                    }
                }
            }
            #endregion
        }
        #endregion

        #region 角色移動事件 CharaMovingEven(Vector3 moveDirection)
        void CharaMovingEven(bool bCharaMoving, Vector3 moveDirection)
        {
            if (bCharaMoving)
            {
                transform.position += moveDirection * fRunSpeed * Time.deltaTime;
                aniChara.SetBool("RUN", true);
            }
            else
            {
                aniChara.SetBool("RUN", false);
            }
        }
        #endregion

        #region 滑鼠鎖定/隱藏/顯示事件 MouseEvent()
        public void MouseEvent()
        {
            if (bGameMenu)
            {
                //若GameMenu被開啟，則需要開啟滑鼠顯示並可以自由移動
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                //若GameMenu未被開啟，則需隱藏滑鼠並鎖定其位置在畫面中心
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        #endregion
        #endregion

        #region 玩家攻擊事件 PlayerATK()
        public void PlayerATK()
        {
            if (pvChara.IsMine)
            {
                switch (enSupplies)
                {
                    case EnSupplies.Handgun:
                        if (iBulletCount[0] >= 1)
                        {
                            objBullet = MyPhotonNetwork.Instantiate(strPathPreBullet + preBullet1.name, trBulletCreatePonit.position, trBulletCreatePonit.rotation, 0, new object[] { 3 });
                            SetBulletInfo(objBullet, enSupplies);
                            iBulletCount[iAtkType]--;
                            UpdateBulletCount(iBulletCount[iAtkType], iAtkType);
                        }
                        break;

                    case EnSupplies.Shotgun:
                        if (iBulletCount[1] >= 1)
                        {
                            StartCoroutine("ShotGunBullet");
                        }
                        break;

                    case EnSupplies.Rifle:
                        if (iBulletCount[2] >= 1)
                        {
                            objBullet = MyPhotonNetwork.Instantiate(strPathPreBullet + preBullet1.name, trBulletCreatePonit.position, trBulletCreatePonit.rotation, 0, new object[] { 3 });
                            SetBulletInfo(objBullet, enSupplies);
                            iBulletCount[iAtkType]--;
                            UpdateBulletCount(iBulletCount[iAtkType], iAtkType);
                        }
                        break;

                    case EnSupplies.RPG:
                        if (iBulletCount[3] >= 1)
                        {
                            objBullet = MyPhotonNetwork.Instantiate(strPathPreBullet + preBullet2.name, trBulletCreatePonit.position, trBulletCreatePonit.rotation, 0, new object[] { 3 });
                            SetBulletInfo(objBullet, enSupplies);
                            iBulletCount[iAtkType]--;
                            UpdateBulletCount(iBulletCount[iAtkType], iAtkType);
                        }
                        break;

                    default:
                        break;
                }
            }
        }
        #endregion

        #region 霰彈槍子彈生成 IEnumerator ShotGunBullet()
        IEnumerator ShotGunBullet()
        {
            Quaternion quaRotation = Quaternion.LookRotation(trBulletCreatePonit.forward);
            objBullet = MyPhotonNetwork.Instantiate(strPathPreBullet + preBullet1.name, trBulletCreatePonit.position, trBulletCreatePonit.rotation, 0, new object[] { 3 });
            SetBulletInfo(objBullet, enSupplies);
            yield return new WaitForSeconds(0.05f);

            float fR1 = Random.Range(5f, 15f);
            Quaternion Rotation1 = Quaternion.Euler(0, fR1, 0);
            objBullet = MyPhotonNetwork.Instantiate(strPathPreBullet + preBullet1.name, trBulletCreatePonit.position, quaRotation * Rotation1, 0, new object[] { 3 });
            SetBulletInfo(objBullet, enSupplies);
            yield return new WaitForSeconds(0.05f);

            float fR2 = Random.Range(-5f, -15f);
            Quaternion Rotation2 = Quaternion.Euler(0, fR2, 0);
            objBullet = MyPhotonNetwork.Instantiate(strPathPreBullet + preBullet1.name, trBulletCreatePonit.position, quaRotation * Rotation2, 0, new object[] { 3 });
            SetBulletInfo(objBullet, enSupplies);

            iBulletCount[iAtkType]--;
            UpdateBulletCount(iBulletCount[iAtkType], iAtkType);
        }
        #endregion

        #region 將未被選擇的武器框架重設ImgBulletTypeSelectReset(int iAtkTypeBefore, int iAtkType)
        void ImgBulletTypeSelectReset(int iAtkTypeBefore, int iAtkType)
        {
            imgBulletTip[iAtkTypeBefore].GetComponent<Image>().sprite = sprFrameNotSelect;
            imgBulletTip[iAtkType].GetComponent<Image>().sprite = sprFrameSelect;
        }
        #endregion

        #region 設定槍種對應的子彈資訊 SetBulletInfo(EnSupplies.Hnadgun)
        void SetBulletInfo(GameObject objTarget, EnSupplies enNewSupplies)
        {
            BulletScript _bulletScript = objTarget.GetComponent<BulletScript>();

            switch (enNewSupplies)
            {
                case EnSupplies.Handgun:
                    _bulletScript.SetBulletState(fBulletSpeed[0]);
                    break;
                case EnSupplies.Shotgun:
                    _bulletScript.SetBulletState(fBulletSpeed[1]);
                    break;
                case EnSupplies.Rifle:
                    _bulletScript.SetBulletState(fBulletSpeed[2]);
                    break;
                case EnSupplies.RPG:
                    _bulletScript.SetBulletState(fBulletSpeed[3]);
                    break;
            }
        }
        #endregion

        #region 角色觸發相關事件 OnTriggerEnter(Collider other)
        /// <summary>
        /// 角色觸發相關事件 OnTriggerEnter(Collider other)
        /// 包含傷害事件、武器更換事件
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if (!pvChara.IsMine)
            {
                return;
            }

            if (other.tag == "Supplies")
            {
                //取得補給品
                Debug.Log("GetSupplies");

                SuppliesScript _suppliesScript = other.GetComponent<SuppliesScript>();
                EnSupplies enNewSupplies = _suppliesScript.GetSuppliesType();
                Debug.Log("GetSupplies : " + enNewSupplies);
                GetSupplies(enNewSupplies);

                PhotonView pvOther = other.GetComponent<PhotonView>(); //取得當前角色碰觸到的物件(other)的PhotonView資訊
                other.gameObject.SetActive(false); //關閉當前角色碰觸到的物件(other)
                GameManager.inst.ObjDestroyEvent(pvOther.OwnerActorNr, pvOther.ViewID); //通知GameManager來刪除當前角色碰觸到的物件(other)

                PlayerGetDead();
            }

            if (other.tag == "Bullet" && !other.GetComponent<PhotonView>().IsMine)
            {
                if (iScore >= 0)
                {
                    Debug.Log("Player Get Hit");

                    //將角色碰觸到的物件(other)刪除
                    PhotonView pvOther = other.GetComponent<PhotonView>(); //取得當前角色碰觸到的物件(other)的PhotonView資訊
                    BulletDestroyEven(pvOther.OwnerActorNr, pvOther.ViewID); //通知刪除當前角色碰觸到的物件(other)

                    //角色扣分事件
                    iScore = iScore - 10;
                    CharaScoreChange(iScore);

                    PlayerGetDead();
                }
            }
        }
        #endregion

        #region 玩家受傷事件 PlayerGetDead()
        void PlayerGetDead()
        {
            if (iScore <= 0)
            {
                //Hashtable建立
                htGetDead["DEAD"] = true;
                MyPhotonNetwork.SetPlayerCustomProperties(htGetDead);

                //我方角色死亡事件觸發
                StartCoroutine(CoDead());
                pvChara.RPC("CoDead", RpcTarget.Others);
            }
        }
        #endregion

        #region 取得物資 GetSupplies()
        /// <summary>
        /// 取得物資 GetSupplies()
        /// </summary>
        /// <param name="supplies"></param>
        /// <param name="other"></param>
        void GetSupplies(EnSupplies enNewSupplies)
        {
            switch (enNewSupplies)
            {
                case EnSupplies.Null:
                    break;
                case EnSupplies.Handgun:
                    iBulletCount[0] += 1;
                    //_audioCharaScript.bBulletReload = true;
                    UpdateBulletCount(iBulletCount[0], 0);
                    break;
                case EnSupplies.Shotgun:
                    iBulletCount[1] += 1;
                    //_audioCharaScript.bBulletReload = true;
                    UpdateBulletCount(iBulletCount[1], 1);
                    break;
                case EnSupplies.Rifle:
                    iBulletCount[2] += 1;
                    //_audioCharaScript.bBulletReload = true;
                    UpdateBulletCount(iBulletCount[2], 2);
                    break;
                case EnSupplies.RPG:
                    iBulletCount[3] += 1;
                    //_audioCharaScript.bBulletReload = true;
                    UpdateBulletCount(iBulletCount[3], 3);
                    break;
                case EnSupplies.Health:
                    iScore = iScore + 10;
                    CharaScoreChange(iScore);
                    break;
                case EnSupplies.Trap:
                    iScore = iScore - 10;
                    CharaScoreChange(iScore);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region 角色分數異動事件 CharaScoreChange()
        void CharaScoreChange(int iNewScore)
        {
            UpdateScoreCount(iNewScore);
            pvChara.RPC("DamageEvent", RpcTarget.Others, iNewScore);

            //Hashtable建立
            ExitGames.Client.Photon.Hashtable htGetDamage = new ExitGames.Client.Photon.Hashtable();
            htGetDamage.Add("GetHit", true);
            htGetDamage.Add("PlayerID", PhotonView.Get(this).ViewID);
            MyPhotonNetwork.SetPlayerCustomProperties(htGetDamage);
        }
        #endregion

        #region BulletCount數量更新 UpdateBulletCount()
        /// <summary>
        /// BulletCount數量更新 UpdateBulletCount()
        /// iNewBulletCount：子彈數量
        /// iNewBulletType：子彈類型，0為Handgun，1為Shootgun，2為Rifle，3為RPG
        /// </summary>
        /// <param name="iNewBulletCount"></param>
        /// <param name="iNewBulletType"></param>
        public void UpdateBulletCount(int iNewBulletCount, int iNewBulletType)
        {
            switch (iNewBulletType)
            {
                case 0:
                    txtBulletCount0.text = iNewBulletCount.ToString();
                    break;
                case 1:
                    txtBulletCount1.text = iNewBulletCount.ToString();
                    break;
                case 2:
                    txtBulletCount2.text = iNewBulletCount.ToString();
                    break;
                case 3:
                    txtBulletCount3.text = iNewBulletCount.ToString();
                    break;
            }
        }
        #endregion

        #region ScoreCount數量更新 UpdateScoreCount()
        public void UpdateScoreCount(int iNewScoreCount)
        {
            txtScoreCount.text = iNewScoreCount.ToString();
        }
        #endregion

        #region EnumToIntEven()
        public int EnumToIntEven(EnSupplies enNewSupplies)
        {
            switch (enNewSupplies)
            {
                case EnSupplies.Null:
                    return 0;
                case EnSupplies.Handgun:
                    return 1;
                case EnSupplies.Shotgun:
                    return 2;
                case EnSupplies.Rifle:
                    return 3;
                case EnSupplies.RPG:
                    return 4;
                case EnSupplies.Health:
                    return 5;
                case EnSupplies.Trap:
                    return 6;
                default:
                    return 0;
            }
        }
        #endregion

        #region 子彈摧毀事件 BulletDestroyEven(int playerID, int viewID)
        public void BulletDestroyEven(int playerID, int viewID)
        {
            if (!pvChara.IsMine)
            {
                return;
            }
            Debug.Log("BulletDestroy" + playerID + " , " + viewID);
            Player[] playerList = MyPhotonNetwork.PlayerList;
            Player player = null;
            foreach (Player i in playerList)
            {
                if (i.ActorNumber == playerID)
                {
                    player = i;
                }
            }
            Debug.Log("BulletDestroy" + player + " , " + viewID);
            pvChara.RPC("RPCBulletDestroy", player, viewID);
        }

        [PunRPC]
        void RPCBulletDestroy(int playerViewID)
        {
            Debug.Log("Destroy");
            PhotonView pvTarget = PhotonView.Find(playerViewID);
            MyPhotonNetwork.Destroy(pvTarget);
        }
        #endregion

        #region RPC事件
        #region [PunRPC] Damage事件
        [PunRPC]
        void DamageEvent(int iNewScore)
        {
            if (pvChara.IsMine && iNewScore > 0)
            {
                Player[] ps = MyPhotonNetwork.PlayerList;
                for (int i = 0; i < ps.Length; i++)
                {
                    if (ps[i].CustomProperties.ContainsKey("GetHit") && (bool)ps[i].CustomProperties["GetHit"] == true)
                    {
                        Debug.Log("Player Get Hit : " + ps[i].NickName);
                        GameManager.inst.iScore = iNewScore;
                        int PLAYERID = (int)ps[i].CustomProperties["PLAYERID"];
                        break;
                    }
                }
            }
        }
        #endregion

        #region 遊戲結束室長離開房間事件 CoMasterLeaveRoom
        public IEnumerator CoMasterLeaveRoom()
        {
            pvChara.RPC("LeaveRoomEvent", RpcTarget.Others);
            yield return new WaitForSeconds(0.5f);
            BackToLobby();
        }
        #endregion

        #region [PunRPC] IEnumerator CoDead事件
        [PunRPC]
        public IEnumerator CoDead()
        {
            aniChara.SetBool("DEAD", true);
            yield return new WaitForSeconds(4);

            if (pvChara.IsMine)
            {
                //失敗者：玩家死亡
                Debug.Log("Player Dead");
                GameManager.inst.GameOVerEvent();
            }
        }
        #endregion

        #region [PunRPC] LeaveRoom事件
        [PunRPC]
        void LeaveRoomEvent()
        {
            Player[] ps = MyPhotonNetwork.PlayerList;
            if (ps.Length != 1)
            {
                for (int i = 0; i < ps.Length; i++)
                {
                    if (!ps[i].IsMasterClient)
                    {
                        BackToLobby();
                    }
                }
            }
        }
        #endregion
        #endregion

        void TxtDebugInfo()
        {
            string s = string.Format("GameManager PV : {0} \n" + "bEndGame : {1} \n", GameManager.inst.pv, GameManager.inst.bEndGame);
            txtDebugInfo.text = s;
        }
    }
}
