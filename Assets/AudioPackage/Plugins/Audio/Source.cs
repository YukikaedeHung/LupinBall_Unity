using UnityEngine;
using UnityEngine.Audio;
using System;

namespace baseSys.Audio.Sources {
    /// <summary>
    /// 音樂播放資料型態
    /// </summary>
    [Serializable]
    public class Source {
        public Source() {
            Name = "";
            Clip = new AudioClip[1];
            Loop = false;
            ResetTime = true;
            Volume = new Vol();
            Pitch = new AudioPitch();
        }

        /// <summary>
        /// 音量控制設定件
        /// </summary>
        [Serializable]
        public class Vol {
            public Vol() {
                Volume = 1;
                IsRandom = false;                
                Max = 1;
                Min = 1;
            }

            [Range(0, 1)]
            public float Volume = 1;
            public bool IsRandom;
            [Range(0, 1)]
            public float Max = 1;
            [Range(0, 1)]
            public float Min = 1;
        }

        /// <summary>
        /// Pitch控制設定件
        /// </summary>
        [Serializable]
        public class AudioPitch {
            public AudioPitch() {
                Pitch = 1;
                IsRandom = false;
                Max = 1;
                Min = 1;
            }
            [Range(-3, 3)]
            public float Pitch = 1;
            public bool IsRandom;
            [Range(1, 3)]
            public float Max = 1;
            [Range(-3, 1)]
            public float Min = 1;            
        }

        public string Name;
        public AudioClip[] Clip;
        public bool Loop;
        public bool ResetTime;
        public Vol Volume;
        public AudioPitch Pitch;
        public AudioMixerGroup MixerGroup;
    }
}