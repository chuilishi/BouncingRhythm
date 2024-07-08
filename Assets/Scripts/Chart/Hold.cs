using UnityEngine;
using UnityEngine.Events;

namespace ChartNamespace
{
    /// <summary>
    /// 长按
    /// </summary>
    public class Hold : IKey
    {
        //长按的结束sample位置
        public float endTime;
        // 是否开始点击了
        public bool isStarted = false;
        // 开始事件
        public UnityEvent<KeyState> holdStartEvent = new UnityEvent<KeyState>();

        public Hold(KeyType keyType, int beat, float speed, Vector2 direction,float endTime,float secondPerBeat,float offset) : base(keyType, beat, speed, direction,secondPerBeat,offset)
        {
            this.endTime = endTime;
        }
        
        public override KeyState TryTrigger()
        {
            var time = Time;
            if (isStarted)//尾判(?)
            {
                if (Mathf.Abs(BeatManager.Instance.audioTime - endTime) < BeatManager.Instance.perfectTolerance)
                {
                    return KeyState.Perfect;
                }
                else if(Mathf.Abs(BeatManager.Instance.audioTime - endTime) < BeatManager.Instance.goodTolerance)
                {
                    return KeyState.Good;
                }
                else if (Mathf.Abs(BeatManager.Instance.audioTime - endTime) < BeatManager.Instance.badTolerance)
                {
                    return KeyState.Bad;
                }
                // 太晚放手是Bad
                else if (BeatManager.Instance.audioTime - endTime > BeatManager.Instance.badTolerance)
                {
                    return KeyState.Bad;
                }
                // 太早放手是Miss
                else return KeyState.Miss;
            }
            else
            {
                // 先判断perfect，因为大部分玩家打出的最多情况就是perfect
                if (Mathf.Abs(BeatManager.Instance.audioTime - time) < BeatManager.Instance.perfectTolerance)
                {
                    return KeyState.Perfect;
                }
                else if(Mathf.Abs(BeatManager.Instance.audioTime - time) < BeatManager.Instance.goodTolerance)
                {
                    return KeyState.Good;
                }
                else if (Mathf.Abs(BeatManager.Instance.audioTime - time) < BeatManager.Instance.badTolerance)
                {
                    return KeyState.Bad;
                }
                else if(time-BeatManager.Instance.audioTime > BeatManager.Instance.badTolerance)
                {
                    return KeyState.UnTrigger;
                }
                else return KeyState.Miss;
            }
        }

    }
}