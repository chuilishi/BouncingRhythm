using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlurEffect
{
    public class BlurTransitionController : MonoBehaviour
    {
        public float duration = 1;
        [Range(0,0.4f)][SerializeField]
        private float horizontalBlur = 0;
        [Range(0,0.4f)][SerializeField]
        private float verticalBlur = 0;
        [SerializeField] private float blurEndValue = 0.4f;
        /// <summary>
        /// 转场效果的函数
        /// </summary>
        /// <param name="duration">转场持续时间(秒)</param>
        /// <param name="endValue"></param>
        /// <param name="interval">平滑程度 (暂时还没用)</param>
        /// <returns></returns>
        public async void Execute(float duration,bool isBlur)
        {
            float startTime = Time.time;
            while ((Time.time - startTime) <duration)
            {
                var res = isBlur
                    ? blurEndValue * (Time.time - startTime) / duration
                    : blurEndValue * (duration - (Time.time - startTime)) / duration;
                BlurSettings.Instance.verticalBlur = res;
                BlurSettings.Instance.horizontalBlur = res;
                await UniTask.DelayFrame(1);
            }
        }
    }
}