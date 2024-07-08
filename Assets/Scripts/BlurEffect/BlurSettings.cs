using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace BlurEffect
{
    [Serializable]
    public class BlurSettings
    {
        private static BlurSettings _instance;
        public static BlurSettings Instance => _instance ??= new BlurSettings();
        public float BlurSize
        {
            get => horizontalBlur;
            set
            {
                horizontalBlur = value;
                verticalBlur = value;
            }
        }
        public float horizontalBlur;
        public float verticalBlur;

        public UniTask Blur(float duration,float blurSize)
        {
            return DOTween.To((() => BlurSize), x => BlurSize = x, blurSize, duration).ToUniTask();
        }
        public UniTask UnBlur(float duration)
        {
            return DOTween.To((() => BlurSize), x => BlurSize = x, 0, duration).ToUniTask();
        }
    }
}