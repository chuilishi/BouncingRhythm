using System;
using Core;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class StartBackground : MonoBehaviour
    {
        public SpriteRenderer background;
        [Header("开场等待时间")]
        public float waitDuration = 1f;

        [Header("动画时间")] public float tweenDuration = 1f;
        private async void Start()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(waitDuration));
            await background.DOColor(Color.clear, tweenDuration).ToUniTask();
        }

        private void Update()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SoundManager.Instance.audioStart.Invoke();
                SoundManager.Instance.audioSource.Play();
            }
#endif
#if UNITY_ANDROID
            foreach (var touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Ended)
                {
                    SoundManager.Instance.audioSource.Play();
                    SoundManager.Instance.audioStart.Invoke();
                    Destroy(gameObject);
                }
            }
#endif
        }
    }
}