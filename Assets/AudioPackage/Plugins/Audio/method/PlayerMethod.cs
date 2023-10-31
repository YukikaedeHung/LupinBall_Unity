using UnityEngine;
using System.Collections.Generic;
using baseSys.Audio.Sources;
using DG.Tweening;

namespace baseSys.Audio.Method {
    public class PlayMethod {
        /// <summary>
        /// 播放清單
        /// </summary>
        Dictionary<string, Source> _list = new Dictionary<string, Source>();

        /// <summary>
        /// 現在播放中
        /// </summary>
        List<GameObject> _nowPlayer = new List<GameObject>();

        /// <summary>
        /// 音量較正值
        /// </summary>
        [Range(0, 1)]
        float FixValue = 0.5f;

        /// <summary>
        /// 播放父物件
        /// </summary>
        GameObject playerObject;
        /// <summary>
        /// 物件池
        /// </summary>
        GameObject ObjectPool;

        bool _mute = false;

        /// <summary>
        /// 初始化播放清單
        /// </summary>
        /// <param name="thisObject"></param>
        /// <param name="type">播放器類型名稱</param>
        /// <param name="playlist">播放清單</param>
        /// <param name="fixValue">初始音量較正值</param>
        public PlayMethod(GameObject thisObject, string type, Source[] playlist, float fixValue)
        {
            for (int i = 0; i < playlist.Length; ++i)
            {
                var pl = playlist[i];
                _list.Add(pl.Name, pl);
            }

            FixValue = fixValue;

            #region [產生物件]
            playerObject = new GameObject();
            playerObject.transform.SetParent(thisObject.transform, false);
            playerObject.name = type;

            ObjectPool = new GameObject();
            ObjectPool.transform.SetParent(playerObject.transform, false);
            ObjectPool.name = "Pool";
            #endregion
        }

        /// <summary>
        /// 重設音量
        /// </summary>
        /// <param name="newValue">新較正值</param>
        public void ResetValue(float newValue)
        {
            for (int i = 0; i < _nowPlayer.Count; ++i)
            {
                var playlist = _nowPlayer[i];
                float value = playlist.GetComponent<AudioSource>().volume;
                //重設音量
                playlist.GetComponent<AudioSource>().volume = (value / FixValue) * newValue;
            }

            //替換新音量較正值
            FixValue = newValue;
        }

        /// <summary>
        /// 靜音
        /// </summary>
        /// <param name="setAct"></param>
        public void OnMute(bool setAct) {
            _mute = setAct;
            for(int i=0; i< _nowPlayer.Count; ++i) {
                var playlist = _nowPlayer[i];
                playlist.GetComponent<AudioSource>().mute = _mute;
            }
        }

        /// <summary>
        /// 一般播放
        /// </summary>
        /// <param name="name"></param>
        public void NextPlay(string name) {
            //如果播放清單有
            if (_list.ContainsKey(name)) {
                nextPlay(name);
            }
            else {
                Debug.LogError("Not Find Audio");
            }
        }

        /// <summary>
        /// 產生播放器
        /// </summary>
        /// <param name="name"></param>
        public void ADDPlay(string name) {
            //如果播放清單有
            if (_list.ContainsKey(name)) {
                play(name);
            }
            else {
                Debug.LogError("Not Find Audio");
            }
        }

        /// <summary>
        /// 停止所有播放
        /// </summary>
        public void StopAll() {
            for(int i=0; i< _nowPlayer.Count; ++i) {
                var stop = _nowPlayer[i];
                recover(stop);
            }
        }

        /// <summary>
        /// 停止特定聲音
        /// </summary>
        /// <param name="name"></param>
        public void Stop(string name) {
            for (int i = 0; i < _nowPlayer.Count; ++i)
            {
                var stop = _nowPlayer[i];
                if (stop.name == name)
                    recover(stop);
            }
        }

        /// <summary>
        /// 檢查物件池是否有可用物件，無則產生一個，並返回物件(減少創物件)。
        /// </summary>
        /// <returns></returns>
        GameObject create() {
            Transform tsf = ObjectPool.transform;
            GameObject obj;

            if (tsf.childCount > 0) {
                obj =
                tsf.GetChild(0).gameObject;                
            }
            else {
                obj =
                    new GameObject();

                obj.AddComponent<AudioSource>();
            }

            if(obj.GetComponent<AudioSource>() == null)
                obj.AddComponent<AudioSource>();

            obj.transform.SetParent(playerObject.transform, false);

            return obj;
        }

        /// <summary>
        /// 回收該物件(減少創物件)
        /// </summary>
        /// <param name="obj"></param>
        void recover(GameObject obj) {
            //從使用中移除
            _nowPlayer.Remove(obj);
            //丟進物件池並關閉
            obj.transform.SetParent(ObjectPool.transform, false);
            obj.SetActive(false);
        }

        /// <summary>
        /// 播放功能
        /// </summary>
        /// <param name="name"></param>
        void play(string name) {
            //取得物件
            GameObject obj =
                create();
            AudioSource aos =
                obj.GetComponent<AudioSource>();
            obj.name = name;

            bool retrigger = 
            set(_list[name], ref aos);
            obj.SetActive(true);
            aos.Play();

            _nowPlayer.Add(obj);
            float life =
                aos.clip.length;

            //判斷是否循環播放，或者重複觸發
            if (!retrigger) {                
                if (!_list[name].Loop) {
                    Sequence _delayCallback;
                    _delayCallback = DOTween.Sequence();
                    _delayCallback.InsertCallback(life, delegate {
                        recover(obj);
                    });
                }
            }
            else {
                Sequence _delayCallback;
                _delayCallback = DOTween.Sequence();
                _delayCallback.InsertCallback(life, delegate {
                    play(name);
                });
            }

        }
        Sequence _delayNextPlay;
        /// <summary>
        /// 同個播放器，播放下一首
        /// </summary>
        /// <param name="name"></param>
        void nextPlay(string name) {
            int playerCount =
                _nowPlayer.Count;
            //取得物件
            GameObject obj;
            AudioSource aos;

            //如果有正在播放
            if (playerCount > 0) {
                obj = _nowPlayer[0];
                aos = obj.GetComponent<AudioSource>();
            }
            else {
                obj = create();
                _nowPlayer.Add(obj);
                aos = _nowPlayer[0].GetComponent<AudioSource>();
            }

            //移除Delay
            if (_delayNextPlay != null) {
                _delayNextPlay.Kill();
            }
            if (obj.name != name) {
                obj.name = name;
            }

            
            //是否重置播放時間
            float time;
            if (!_list[name].ResetTime) {
                time = aos.time + 0.01f;
            }
            else {
                time = 0;
            };

            //載入設定檔&播放
            bool retrigger;
            retrigger =
            set(_list[name], ref aos);

            aos.time = time;
            aos.Play();
            obj.SetActive(true);

            if (retrigger && _list[name].Loop) {
                float life =
                aos.clip.length;
                life -= aos.time;

                _delayNextPlay = DOTween.Sequence();
                _delayNextPlay.InsertCallback(life, delegate {
                    nextPlay(name);
                });
            }
        }

        /// <summary>
        /// return reTrigger
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        bool set(Source setting, ref AudioSource player) {
            bool reTrigger;

            if (setting.Clip.Length > 1 && setting.Loop)
                reTrigger = true;
            else
                reTrigger = false;

            //取得播放歌曲
            AudioClip clip =
                getClip(setting.Clip);
            float volume =
                getVol(setting.Volume);
            float pitch =
                getPitch(setting.Pitch);

            player.clip = clip;
            player.loop = setting.Loop;
            player.volume = volume;
            player.pitch = pitch;
            if (setting.MixerGroup)
                player.outputAudioMixerGroup = setting.MixerGroup;
            else
                player.outputAudioMixerGroup = null;

            if (_mute)
                player.mute = _mute;

            return reTrigger;
        }

        /// <summary>
        /// 取得播放音源
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        AudioClip getClip(AudioClip[] data) {
            AudioClip clip;
            int rang =
                data.Length;
            //陣列範圍0
            if(rang == 1) {
                clip = data[0];
            }
            //若陣列大於，則亂數選取播放
            else if(rang > 1) {
                System.Random ptr =
                new System.Random(System.Guid.NewGuid().GetHashCode());
                int num =
                    ptr.Next(rang);
                clip = data[num];
            }
            else {
                clip = null;
                Debug.LogError("out of rang! AudioClip...");
            }            

            return clip;
        }

        /// <summary>
        /// 取得音量
        /// </summary>
        /// <param name="vol"></param>
        /// <returns></returns>
        float getVol(Source.Vol vol) {
            float volume;

            if (!vol.IsRandom) {
                volume = vol.Volume;
            }
            else {
                volume = Random.Range(vol.Max, vol.Min);
            }

            //音量較正
            volume *= FixValue;

            return volume;
        }

        /// <summary>
        /// 取得pitch
        /// </summary>
        /// <param name="pit"></param>
        /// <returns></returns>
        float getPitch(Source.AudioPitch pit) {
            float pitch;

            if (!pit.IsRandom) {
                pitch = pit.Pitch;
            }
            else {
                pitch = Random.Range(pit.Max, pit.Min);
            }

            return pitch;
        }
    }
}
    
