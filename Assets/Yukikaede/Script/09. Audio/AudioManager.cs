using UnityEngine;
using System;
using baseSys.Audio;
using baseSys.Audio.Sources;

public class AudioManager : MonoBehaviour {

    //public static AudioManager inst;
    /// <summary>
    /// 音樂資源設定
    /// </summary>
    [SerializeField]
    Source[] BGMSetting;
    /// <summary>
    /// 音效資源設定
    /// </summary>
    [SerializeField]
    Source[] SFXSetting;

    /// <summary>
    /// 音樂播放器
    /// </summary>
    AudioPlayer BGM;
    /// <summary>
    /// 音效播放器
    /// </summary>
    AudioPlayer SFX;

    /// <summary>
    /// 音樂音量較正值
    /// </summary>
    [Range(0, 1)]
    float BGMValue = 0.5f;
    /// <summary>
    /// 音效音量較正值
    /// </summary>
    [Range(0, 1)]
    float SFXValue = 0.5f;

    void Awake() {
        //inst = this;

        BGM = new AudioPlayer(gameObject, "BGMPlayer", BGMSetting, BGMValue);
        SFX = new AudioPlayer(gameObject, "SFXPlayer", SFXSetting, SFXValue);
    }

    void Start () {
        //清空省記憶體
        BGMSetting = null;
        SFXSetting = null;
    }

    #region [BGM播放]
    /// <summary>
    /// 播放設定BGM
    /// </summary>
    /// <param name="name"></param>
    public void PlayBGM(string name) {
        BGM.Play(name);
    }

    /// <summary>
    /// StopAll
    /// </summary>
    public void BGMStop() {
        BGM.StopAll();
    }

    /// <summary>
    /// Stop Name
    /// </summary>
    /// <param name="name"></param>
    public void BGMStop(string name) {
        BGM.Stop(name);
    }

    /// <summary>
    /// 重設音量
    /// </summary>
    /// <param name="value"></param>
    public void BGMReset(float value)
    {
        BGM.ResetValue(value);
    }
    #endregion

    #region [SFX播放]
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name"></param>
    public void PlaySFX(string name) {
        SFX.Play(name);
    }

    /// <summary>
    /// Stop All
    /// </summary>
    public void SFXStop() {
        SFX.StopAll();
    }

    /// <summary>
    /// Stop Name
    /// </summary>
    /// <param name="name"></param>
    public void SFXStop(string name) {
        SFX.Stop(name);
    }

    /// <summary>
    /// 重設音量
    /// </summary>
    /// <param name="value"></param>
    public void SFXReset(float value)
    {
        SFX.ResetValue(value);
    }
    #endregion

    /// <summary>
    /// 靜音選項
    /// </summary>
    /// <param name="setAct"></param>
    public void Mute(bool setAct) {        
        BGM.Mute(setAct);
        SFX.Mute(setAct);
    }
}
