using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Tim;

namespace Tim.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager inst;

        [HideInInspector]
        public PhotonView pv; //存取資訊用的PhotnView
        [HideInInspector]
        public GameObject objControl;
        [HideInInspector]
        public GameObject objPlayer; //宣告給CharaScript與ProjectileScript使用

        [Header("PhotonView Pool")]
        public PhotonView pvParentPlayer;
        public PhotonView pvParentSupplies;
        public PhotonView pvPareantAudio;

        [Header("Prefab")]
        public GameObject preUnityChan;
        public GameObject preSupplies;
        public GameObject preAudioSFXGunShot;
        public GameObject preAudioSFXGunReload;

        [Header("Variable")]
        public AudioScript _audioScript;
        public int iScore; //Unity面板設定值
        public bool bPlayerDead;
        public float[] fSuppliesCreatTime = new float[2];  //生成preSupplies的間隔，Unity面板設定值
        public float fSuppliesCreatTimeInitial;
        [HideInInspector]
        public bool bInittailFinish;
        [HideInInspector]
        public bool bEndGame;

        [Header("Prefab Path")]
        public const string strPathPreChara = "Prefab/00. preChara/";
        public const string strPathPreAudio = "Prefab/03. preAudio/";
        public const string strPathPreSupplies = "Prefab/01. preSupplies/";

        [Header("Players Create Place")]
        public GameObject[] objArrayPlayerSpawnPoint = new GameObject[4];
        private bool[] bPlayerSpawnPoint = new bool[4];

        [Header("Supplies Create Place")]
        public GameObject[] objArrSuppliesPlaceTeamA = new GameObject[6];//[]內的數量為畫面上生成點的對應數量
        public GameObject[] objArrSuppliesPlaceTeamB = new GameObject[6];
        //計算用參數
        private GameObject[,] objArrSuppliesPlace = new GameObject[2, 6];//[A, B]其中A為隊伍數量，B為畫面上生成點的對應數量
        private PhotonView[,] pvSupplies = new PhotonView[2, 6];
        private int[] iSuppliesPlace = new int[2] { 0, 0 };
        private bool[,] bSuppliesInPlace = new bool[2, 6];
        private Player[,] arrPlayer = new Player[2, 2];//[A, B]其中A為隊伍數量，B為玩家數量/2 (ex：4名玩家，則2名在A隊[0,0]與[0,1]，2名在B隊[1,0]與[1,1]

        private void Awake()
        {            
            inst = this;
            pv = PhotonView.Get(this);
        }

        void OnEnable()
        {
            Initial();
        }

        // Start is called before the first frame update
        void Start()
        {
            GameStartSetting();
        }

        // Update is called once per frame
        void Update()
        {
            if (!bEndGame && MyPhotonNetwork.IsMasterClient)
            {
                SuppliesCeate(0);
                SuppliesCeate(1);
            }
        }

        #region 初始化 Initial()
        void Initial()
        {
            //生成Supplies的時間重置
            fSuppliesCreatTime[0] = fSuppliesCreatTimeInitial;
            fSuppliesCreatTime[1] = fSuppliesCreatTimeInitial;
            bEndGame = false;
        }
        #endregion

        #region GameStartSetting()
        void GameStartSetting()
        {
            //音效置入
            _audioScript.ChangeSceneWithBGM();

            //GameObject置入
            for (int i = 0; i < objArrSuppliesPlaceTeamA.Length; i++)
            {
                objArrSuppliesPlace[0, i] = objArrSuppliesPlaceTeamA[i];
                objArrSuppliesPlace[1, i] = objArrSuppliesPlaceTeamB[i];
            }

            //玩家進入遊戲場景後，自動生成角色
            GameObject obj;
            CreateCharaEvent(preUnityChan, out obj, pvParentPlayer.ViewID);
        }
        #endregion

        #region Supplies隨機生成 SuppliesCeate(int iTeam)
        void SuppliesCeate(int iTeam)
        {
            iSuppliesPlace[iTeam] = Random.Range(0, objArrSuppliesPlaceTeamA.Length);
            if (!bSuppliesInPlace[iTeam, iSuppliesPlace[iTeam]])
            {
                fSuppliesCreatTime[iTeam] -= Time.deltaTime;
                if (fSuppliesCreatTime[iTeam] <= 0)
                {
                    //計時器更新
                    fSuppliesCreatTime[iTeam] = fSuppliesCreatTimeInitial;

                    //隨機指定固定的區域objArrayPlace[iSuppliesPlace]生成物件
                    bSuppliesInPlace[iTeam, iSuppliesPlace[iTeam]] = true;
                    Debug.Log(iTeam + " , " + iSuppliesPlace[iTeam]);
                    Transform tr = objArrSuppliesPlace[iTeam, iSuppliesPlace[iTeam]].transform; //tr為生成點
                    GameObject objPv;
                    CreateSuppliesEvent(preSupplies, out objPv, new Vector3(tr.position.x, tr.position.y - 1.5f, tr.position.z), tr.rotation, pvParentSupplies.ViewID);
                    pvSupplies[iTeam, iSuppliesPlace[iTeam]] = objPv.GetComponent<PhotonView>();
                }
            }
        }
        #endregion

        #region 生成物件相關事件
        #region 角色生成事件 CreateCharaEvent(GameObject pre, out GameObject obj, int parentViewID)
        public void CreateCharaEvent(GameObject pre, out GameObject obj, int parentViewID)
        {
            Player[] playerList = MyPhotonNetwork.PlayerList;
            int iPoint;
            //string s = string.Format("playerList.Length : {0}, MyPhotonNetwork.LocalPlayer.ActorNumber : {1}", playerList.Length, MyPhotonNetwork.LocalPlayer.ActorNumber);
            //Debug.Log(s);
            //取得房間內的玩家的 MyPhotonNetwork.LocalPlayer.ActorNumber 其值是由1開始排序，即 MasterClient = 1、玩家2 = 2、etc...
            if (MyPhotonNetwork.LocalPlayer.ActorNumber % 2 == 1)
            {
                //Team A 生成點
                iPoint = Random.Range(0, 2);
            }
            else
            {
                //Team B 生成點
                iPoint = Random.Range(2, 4);
            }

            Transform tr = objArrayPlayerSpawnPoint[iPoint].transform;
            objControl = MyPhotonNetwork.Instantiate(strPathPreChara + pre.name, new Vector3(tr.position.x, tr.position.y, tr.position.z), tr.rotation, 0, new object[] { pvParentPlayer.ViewID });
            obj = objControl;
            bPlayerSpawnPoint[iPoint] = true;
        }
        #endregion

        #region 場景物件生成事件 CreateSuppliesEvent(GameObject pre, Vector3 preV3, Quaternion preQ, int parentViewID)
        public void CreateSuppliesEvent(GameObject pre, out GameObject obj, Vector3 preV3, Quaternion preQ, int parentViewID)
        {
            GameObject objNew = MyPhotonNetwork.Instantiate(strPathPreSupplies + pre.name, preV3, preQ, 0, new object[] { pvParentSupplies.ViewID });
            obj = objNew;
        }
        #endregion

        #region Audio生成事件 CreateAudioEvent(GameObject pre, out GameObject obj, int parentViewID)
        public void CreateAudioEvent(GameObject pre, int parentViewID)
        {
            Debug.Log("Create Audio GunShot");

            MyPhotonNetwork.Instantiate(strPathPreAudio + pre.name, pre.transform.position, pre.transform.rotation, 0, new object[] { pvPareantAudio.ViewID });
        }
        #endregion
        #endregion

        #region 摧毀物件相關事件
        #region 場景物件摧毀事件 ObjDestroyEvent(int playerID, int viewID)
        public void ObjDestroyEvent(int playerID, int viewID)
        {
            Player[] playerList = MyPhotonNetwork.PlayerList;
            Player player = null;
            foreach (Player i in playerList)
            {
                if (i.ActorNumber == playerID)
                {
                    player = i;
                }
            }
            Debug.Log("ObjDestroyEvent");
            pv.RPC("RPCObjDestroy", player, viewID);
        }
        #endregion

        #region [PunRPC]觸發事件
        [PunRPC]
        public void RPCObjDestroy(int playerViewID)
        {
            Debug.Log("ObjDestroy");
            PhotonView pvTarget = PhotonView.Find(playerViewID);

            SuppliesInPlace(pvTarget);
            MyPhotonNetwork.Destroy(pvTarget);
        }
        #endregion
        #endregion

        #region Supplies生成區域是否已經有物體 SuppliesInPlace(PhotonView pvTarget,)
        public void SuppliesInPlace(PhotonView pvTarget)
        {
            for (int iTeam = 0; iTeam < 2; iTeam++)
            {
                for (int i = 0; i < objArrSuppliesPlaceTeamA.Length; i++)
                {
                    if (pvSupplies[iTeam, i] != null)
                    {
                        Vector3 objV3 = pvSupplies[iTeam, i].transform.position;
                        Vector3 targetV3 = pvTarget.transform.position;
                        if (objV3 == targetV3)
                        {
                            bSuppliesInPlace[iTeam, i] = false;
                        }
                    }
                }
            }
        }
        #endregion

        #region 遊戲結束事件
        public void GameOVerEvent()
        {
            bEndGame = true;

            Player[] ps = MyPhotonNetwork.PlayerList;
            Debug.Log("ps.Length : " + ps.Length);
            for (int i = 0; i < ps.Length; i++)
            {
                if (ps[i].CustomProperties["DEAD"] != null)
                {
                    Debug.Log(ps[i]);

                    if ((bool)ps[i].CustomProperties["DEAD"] == false)
                    {
                        Debug.Log("Win");
                        int PLAYERID = (int)ps[i].CustomProperties["PLAYERID"];

                        PhotonView.Find(PLAYERID).transform.Find("Personal").Find("Camera").Find("CanvasUI").Find("Img_PlayerWin").gameObject.SetActive(true);
                        PhotonView.Find(PLAYERID).transform.Find("Personal").Find("Camera").Find("CanvasUI").Find("txt_EndGameTip").gameObject.SetActive(true);

                        ps[i].CustomProperties["DEAD"] = false;
                    }
                    else
                    {
                        Debug.Log("Lose");
                        int PLAYERID = (int)ps[i].CustomProperties["PLAYERID"];

                        PhotonView.Find(PLAYERID).transform.Find("Personal").Find("Camera").Find("CanvasUI").Find("Img_PlayerLose").gameObject.SetActive(true);
                        PhotonView.Find(PLAYERID).transform.Find("Personal").Find("Camera").Find("CanvasUI").Find("txt_EndGameTip").gameObject.SetActive(true);

                        ps[i].CustomProperties["DEAD"] = false;
                    }
                }
                else
                {
                    Debug.Log(ps[i] + " is null");
                }
            }
        }
        #endregion
    }
}