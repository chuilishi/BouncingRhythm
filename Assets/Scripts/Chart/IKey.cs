using System;
using Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace ChartNamespace
{
    /// <summary>
    /// 键型的基类
    /// </summary>
    [Serializable]
    public class IKey
    {
        public int beat;
        // 左&右 单&双
        public KeyType keyType;
        //位置
        public Vector2 position;
        //小球在当前key到下一次key之间的移动速度 (Hold忽略)
        public float speed;
        //方向
        public Vector2 direction;
        
        //从铺面来
        public float secondPerBeat;
        //同上
        public float offset;
        
        // key的结束事件
        [NonSerialized] public UnityEvent<KeyState> keyEndEvent;
        // 结束的委托,被注册给afterafterbeat 或者在
        [NonSerialized]
        public UnityAction<int> keyAction;
        [NonSerialized]
        public bool isOver = false;
        
        public float Time => _time;
        [SerializeField]
        private float _time;
        public GameObject ball;
        
        protected IKey(KeyType keyType, int beat,float speed,Vector2 direction,float secondPerBeat,float offset)
        {
            this.keyType = keyType;
            this.beat = beat;
            this.speed = speed;
            this.direction = direction;
            this.secondPerBeat = secondPerBeat;
            this.offset = offset;
            _time = secondPerBeat * beat + offset;
        }

        public void UpdateTime(float secondPerBeat,float offset)
        {
            this.secondPerBeat = secondPerBeat;
            this.offset = offset;
            _time = secondPerBeat * beat + offset;
        }

        public void UpdateBeat(int beat)
        {
            this.beat = beat;
            _time = secondPerBeat * beat + offset;
        }

        #region 判断类型的一些函数

        public bool isLeft()
        {
            return keyType is KeyType.LeftSingle or KeyType.LeftDouble;
        }

        public bool isRight()
        {
            return keyType is KeyType.RightSingle or KeyType.RightDouble;
        }
        #endregion

        public virtual KeyState TryTrigger()
        {
            var minus = Mathf.Abs(SoundManager.Instance.audioSource.time - Time);
            if (minus > BeatManager.Instance.badTolerance)
            {
                return KeyState.UnTrigger;
            }
            else if (minus > BeatManager.Instance.goodTolerance)
            {
                return KeyState.Bad;
            }
            else if (minus > BeatManager.Instance.perfectTolerance)
            {
                return KeyState.Good;
            }
            else
            {
                return KeyState.Perfect;
            }
        }
    }
}