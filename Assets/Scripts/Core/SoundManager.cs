using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Core
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;
        public AudioSource audioSource;
        public bool isMakingChart = false; 

        [Header("从开始到静音要多久")] public float duration = 0.2f;
        
        [HideInInspector]
        public UnityEvent audioStart = new UnityEvent();
    
        [HideInInspector]
        public UnityEvent audioPause = new UnityEvent();
        
        private void Awake()
        {
            Instance = this;
            if (isMakingChart) return;
            foreach (var level in LevelManager.Instance.levels)
            {
                level.audioClip.LoadAudioData();
            }
            audioSource.clip = LevelManager.Instance.levels[0].audioClip;
        }

        private void OnEnable()
        {
            Instance = this;
        }
        /// <summary>
        /// 无参数的话就是重新播放当前关卡
        /// </summary>
        public void Restart()
        {
            audioSource.clip = LevelManager.Instance.levels[LevelManager.Instance.CurLevel].audioClip;
            audioSource.Play();
        }
        public void Restart(AudioClip audioClip)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        public void Resume()
        {
            audioSource.Play();
        }
        
        private void Update()
        {
            // if (Input.GetKeyDown(KeyCode.Space))
            // {
            //     audioSource.Play();
            //     audioStart.Invoke();
            // }
        }

        //缓慢静音
        public UniTask Mute()
        {
            var tcs = new UniTaskCompletionSource();
            DOTween.To(() => audioSource.volume, (v) => audioSource.volume = v, 0, duration).onComplete +=
                () => { tcs.TrySetResult();};
            return tcs.Task;
        }

        public UniTask UnMute()
        {
            var tcs = new UniTaskCompletionSource();
            DOTween.To(() => audioSource.volume, (v) => audioSource.volume = v, 1, duration).onComplete +=
                () => { tcs.TrySetResult();};
            return tcs.Task;
        }
    }
}