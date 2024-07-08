//一次点击
using UnityEngine;
using UnityEngine.Events;
using ChartNamespace;

namespace ChartNamespace
{
    /// <summary>
    /// 路径上的点击 Tap (非反弹)
    /// </summary>
    public class Tap : IKey
    {
        public Tap(KeyType keyType, int beat, float speed, Vector2 direction,float secondPerBeat,float offset) : base(keyType, beat, speed, direction,secondPerBeat,offset)
        {
        }
        public override KeyState TryTrigger()
        {
            var time = Time;
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