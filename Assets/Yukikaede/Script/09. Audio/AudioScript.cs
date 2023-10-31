using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Tim.Game;
using System.IO;

public class AudioScript : MonoBehaviour
{
    //public static AudioScript inst;

    public AudioManager _audioManager;
    private SoundSettings _soundSettings;
    public float fAudioRegulateBGM; //場景切換後BGM音效校正值，Unity面板設定值

    private string sJsonFilePath;

    //private void Awake()
    //{
    //    if (inst == null)
    //    {
    //        inst = this;
    //        DontDestroyOnLoad(this.gameObject);
    //    }
    //    else
    //    {
    //        Destroy(this);
    //    }

    //    DontDestroyOnLoad(gameObject);
    //}

    // Start is called before the first frame update
    void Start()
    {
        _soundSettings = new SoundSettings();
        sJsonFilePath = Application.dataPath + "/SoundSettings.json";
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region 場景與BGM切換 ChangeSceneWithBGM()
    public void ChangeSceneWithBGM()
    {
        SoundSettingLoad();

        if (SceneManager.GetActiveScene().name == GameSceneScript.SceneName_GameLogIn)
        {
            Debug.Log("ChangeScene : " + SceneManager.GetActiveScene().name);
            _audioManager.PlayBGM("LogIn");
        }

        if (SceneManager.GetActiveScene().name == GameSceneScript.SceneName_GameScene)
        {
            Debug.Log("ChangeScene : " + SceneManager.GetActiveScene().name);
            _audioManager.PlayBGM("Gaming");
        }
    }
    #endregion

    #region 載入Json音訊資料 SoundSettingLoad()
    /// <summary>
    /// 載入Json音訊資料 SoundSettingLoad()
    /// 於遊戲啟動、開啟SettingMenu時都需要載入
    /// </summary>
    public void SoundSettingLoad()
    {
        float fBGM = 0;
        float fSFX = 0;

        //檢查檔案是否存在
        if (File.Exists(sJsonFilePath))
        {
            string json = File.ReadAllText(sJsonFilePath);
            _soundSettings = JsonUtility.FromJson<SoundSettings>(json);
            fBGM = _soundSettings.fBGMValue;
            fSFX = _soundSettings.fSFXValue;

            if (SceneManager.GetActiveScene().name == GameSceneScript.SceneName_GameLogIn)
            {
                fBGM = fBGM + fAudioRegulateBGM; //場景切換的音效大小差異修正值
                fBGM = fBGM >= 1 ? 1 : fBGM; //如果新的fBGM的值大於，則需要被設定在1
            }
        }
        else
        {
            fBGM = 0.5f;
            fSFX = 0.5f;
        }
        Debug.Log("AudioSetting : " + fBGM + " / " + fSFX);
        BGMSoundSettingUpdate(fBGM);
        SFXSoundSettingUpdate(fSFX);
    }
    #endregion

    #region SettingMenu開啟時的音訊資料顯示 SettingMenuInfoLoading()
    /// <summary>
    /// SettingMenu開啟時的音訊資料顯示 SettingMenuInfoLoading()
    /// </summary>
    public void SettingMenuInfoLoading()
    {
        SoundSettingLoad();

        if (SceneManager.GetActiveScene().name == GameSceneScript.SceneName_GameLogIn)
        {
            SettingMenuInfoUpdate(_soundSettings.fBGMValue, _soundSettings.fSFXValue);
        }
        if (SceneManager.GetActiveScene().name == GameSceneScript.SceneName_GameScene)
        {
            //還沒做
        }
    }
    #endregion

    #region SettingMenu音訊數值更新 SettingMenuInfoUpdate()
    void SettingMenuInfoUpdate(float BGMAudioValue, float SFXAudioValue)
    {
        string sGameSceneName = "";
        if (SceneManager.GetActiveScene().name == GameSceneScript.SceneName_GameLogIn)
            sGameSceneName = "SceneName_GameLogIn";
        if (SceneManager.GetActiveScene().name == GameSceneScript.SceneName_GameScene)
            sGameSceneName = "SceneName_GameScene";

        switch (sGameSceneName)
        {
            case "SceneName_GameLogIn":
                MultiPlayerSettingManagerScript.inst.txtBGMValue.text = (BGMAudioValue * 100).ToString();
                MultiPlayerSettingManagerScript.inst.txtSFXValue.text = (SFXAudioValue * 100).ToString();
                break;
            case "SceneName_GameScene":

                break;
        }
    }

    #endregion

    #region BGM音訊資料更新 BGMSoundSettingUpdate(float value)
    /// <summary>
    /// BGM音訊資料更新 BGMSoundSettingUpdate(float value)
    /// </summary>
    /// <param name="value"></param>
    public void BGMSoundSettingUpdate(float value)
    {
        float fDataSldBGM = value;
        _audioManager.BGMReset(fDataSldBGM);
        if (MultiPlayerSettingManagerScript.inst.bSettingMenu)
            SettingMenuInfoUpdate(fDataSldBGM, _soundSettings.fSFXValue);
    }
    #endregion

    #region SFX音訊資料更新 SFXSoundSettingUpdate(float value)
    /// <summary>
    /// SFX音訊資料更新 SFXSoundSettingUpdate(float value)
    /// </summary>
    /// <param name="value"></param>
    public void SFXSoundSettingUpdate(float value)
    {
        float fDataSldSFX = value;
        _audioManager.SFXReset(fDataSldSFX);
        if (MultiPlayerSettingManagerScript.inst.bSettingMenu)
            SettingMenuInfoUpdate(_soundSettings.fBGMValue, value);
    }
    #endregion

    #region 音訊資料儲存 SoundSettingSave(bool bSaving)
    public void SoundSettingSave(bool bSaving)
    {
        if (bSaving)
        {
            _soundSettings.fBGMValue = MultiPlayerSettingManagerScript.inst.sldBGM.value;
            _soundSettings.fSFXValue = MultiPlayerSettingManagerScript.inst.sldSFX.value;
            _audioManager.BGMReset(_soundSettings.fBGMValue);
            _audioManager.SFXReset(_soundSettings.fSFXValue);

            string json = JsonUtility.ToJson(_soundSettings);
            Debug.Log("Serialized JSON : " + json);
            File.WriteAllText(sJsonFilePath, json);
        }
        else
        {
            _audioManager.BGMReset(_soundSettings.fBGMValue);
            _audioManager.BGMReset(_soundSettings.fSFXValue);
        }
    }
    #endregion

    #region 音效設定Json資料存取
    [System.Serializable]
    public class SoundSettings
    {
        public float fBGMValue;
        public float fSFXValue;
    }
    #endregion
}
