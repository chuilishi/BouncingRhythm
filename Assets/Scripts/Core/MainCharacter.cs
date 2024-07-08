using System.Collections.Generic;
using System.Linq;
using ChartNamespace;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class MainCharacter : MonoBehaviour
    {
        public Chart chart;
        // 长条尾判?
        public bool endHoldJudge = false;
        private bool isStart = false;

        public static MainCharacter Instance;

        public Transform ballTransform;
        public LineRenderer lineRenderer;
        public Transform ballsParent;

        #region 生命周期函数

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            if (chart == null) return;
            // BeatEventInit();
            PreLoadBalls();
            SoundManager.Instance.audioStart.AddListener(()=>isStart = true);
            SoundManager.Instance.audioPause.AddListener(()=>isStart = false);
            TouchEventInit(chart);

            positions = chart.keys.Select((key => key.position)).ToList();
            times = chart.keys.Select((key => key.Time)).ToList();
        
            SoundManager.Instance.audioStart.AddListener((() => isStart = true));
        }
    
        private void Update()
        {
            if (!isStart) return;
            QueueUpdate();
            Move();
        }

        #region 预加载场景

        private void PreLoadBalls()
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0,chart.keys[0].position);

            // 第一个key是工具key用于方便初始化
            var firstKey = chart.keys[0];
            firstKey.keyType = KeyType.LeftSingle;
            firstKey.position = chart.keys[0].position;
            firstKey.direction = Vector2.up;
            firstKey.ball = Instantiate(GameSettings.Instance.KeyTypeToPrefab[firstKey.keyType], firstKey.position, Quaternion.identity,ballsParent.transform);
            firstKey.ball.GetComponent<SpriteRenderer>().color = Color.clear;
        
            for (int i = 1; i < chart.keys.Count-1; i++)
            {
                var key = chart.keys[i];
                var lastKey = chart.keys[i-1];
                key.position = lastKey.position + lastKey.direction * (key.Time - lastKey.Time) * lastKey.speed;
                key.ball = Instantiate(GameSettings.Instance.KeyTypeToPrefab[key.keyType], key.position, Quaternion.identity,ballsParent.transform);
                if (chart.keys[i-1].keyType is KeyType.LeftDouble or KeyType.RightDouble or KeyType.RightSimultaneous)
                {
                    lineRenderer.positionCount++;
                    lineRenderer.SetPosition(lineRenderer.positionCount-1,key.position);
                }
                else
                {
                    lineRenderer.SetPosition(lineRenderer.positionCount-1,key.position);
                }
            }
        }

        #endregion
    
        //第0个key不用管
        private int queueUpdateIndex = 1;
        private void QueueUpdate()
        {
            while (leftTapsQueue.Count != 0)
            {
                var key = leftTapsQueue.Peek();
                if (BeatManager.Instance.audioTime - key.Time> BeatManager.Instance.badTolerance)
                {
                    key.keyEndEvent.Invoke(KeyState.Miss);
                    leftTapsQueue.Dequeue();
                }
                else break;
            }
            while (rightTapsQueue.Count != 0)
            {
                var key = rightTapsQueue.Peek();
                if (BeatManager.Instance.audioTime - key.Time> BeatManager.Instance.badTolerance)
                {
                    key.keyEndEvent.Invoke(KeyState.Miss);
                    rightTapsQueue.Dequeue();
                }
                else break;
            }
            while (leftHoldsQueue.Count != 0)
            {
                var hold = leftHoldsQueue.Peek();
                if (hold.isOver)
                {
                    leftHoldsQueue.Dequeue();
                }
                // 已经开始了但还没结束的hold *** QueueUpdate()只负责去除那些超时的hold ***
                else if (hold.isStarted)
                {
                    if (BeatManager.Instance.audioTime - hold.Time > BeatManager.Instance.badTolerance)
                    {
                        hold.keyEndEvent.Invoke(KeyState.Bad);
                        hold.isOver = true;
                    }
                    break;
                }
            }
            while (rightHoldsQueue.Count != 0)
            {
                var hold = rightHoldsQueue.Peek();
                if (hold.isOver)
                {
                    rightHoldsQueue.Dequeue();
                }
                // 已经开始了但还没结束的hold *** QueueUpdate()只负责去除那些超时的hold ***
                else if (hold.isStarted)
                {
                    if (BeatManager.Instance.audioTime - hold.Time > BeatManager.Instance.badTolerance)
                    {
                        hold.keyEndEvent.Invoke(KeyState.Bad);
                        hold.isOver = true;
                    }
                    break;
                }
            }
        
            // key enqueue 进去，并且给其加上endEvent
            while (true)
            {
                var key = chart.keys[queueUpdateIndex];
                // 到了good的临界点再将其enqueue进去这样 就不会在不该生效的区域内触发
                if (key.Time - BeatManager.Instance.audioTime <
                    BeatManager.Instance.badTolerance)
                {
                    key.keyEndEvent = new UnityEvent<KeyState>();
                    if (key is Hold hold)
                    {
                        (key.isLeft() ? leftHoldsQueue : rightHoldsQueue).Enqueue(hold);
                    }
                    else
                    {
                        (key.isLeft() ? leftTapsQueue : rightTapsQueue).Enqueue(key);
                    }
                    key.keyEndEvent.AddListener((keyState =>
                    {
                        key.ball.GetComponent<SpriteRenderer>().color = Color.clear;
                        Debug.Log("End");
                    }));
                    queueUpdateIndex++;
                }
                else break;
            }
        }
        #endregion
    
        #region 移动

        // private float speed;
        // private Vector2 curDir = Vector2.up;
        private List<Vector2> positions = new List<Vector2>();
        private List<float> times = new List<float>();
        private float initTime;
        private int curIndex = 0;
        private void Move()
        {
            var audioTime = BeatManager.Instance.audioTime;
            while (BeatManager.Instance.audioTime >= times[curIndex])
            {
                curIndex++;
            }
            //打印出 audioTime , times[curIndex-1], tirmes[curIndex], Mathf.InverseLerp(audioTime,times[curIndex-1],times[curIndex])
            // Debug.Log(audioTime + " " + times[cuIndex-1] + " " + times[curIndex] + " " + Mathf.InverseLerp(times[curIndex-1],times[curIndex],audioTime));
            ballTransform.position = Vector2.Lerp(positions[curIndex-1],positions[curIndex],Mathf.InverseLerp(times[curIndex-1],times[curIndex],audioTime));
        }
    
        #endregion

        private Queue<IKey> leftTapsQueue = new Queue<IKey>();
        private Queue<IKey> rightTapsQueue = new Queue<IKey>();
        private Queue<Hold> leftHoldsQueue = new Queue<Hold>();
        private Queue<Hold> rightHoldsQueue = new Queue<Hold>();
    
        #region TouchEventHandler
        // 在 beforebefore的时候 添加key
        // 短键: enqueue Tap,添加事件:过了时间就dequeue 
        // 长键: 过了时间/就 dequeue 
        public void TouchEventInit(Chart chart)
        {
            void TapHandler(UnityEvent<KeyType> unityEvent,Queue<IKey> queue)
            {
                unityEvent.AddListener((keyType =>
                {
                    //先吧已经超时的垃圾key给dequeue出去
                    while (queue.Count != 0 && queue.Peek().isOver)queue.Dequeue();

                    if (queue.Count == 0)
                    {
                        Debug.Log("没有Key");
                        return;
                    }
                    var key = queue.Peek();
                    var keyState = key.TryTrigger();
                    if (keyState != KeyState.UnTrigger)
                    {
                        key.keyEndEvent.Invoke(keyState);
                        key.isOver = true;
                    }
                }));
            }
        
            TapHandler(TouchManager.Instance.leftTapEvent,leftTapsQueue);
            TapHandler(TouchManager.Instance.rightTapEvent,rightTapsQueue);
        
            TouchManager.Instance.leftHoldBeginEvent.AddListener((keyType =>
            {
                // isOver的出队
                while (leftHoldsQueue.Count != 0 && leftHoldsQueue.Peek().isOver)leftHoldsQueue.Dequeue();
                if(leftHoldsQueue.Count == 0)return;
                var hold = leftHoldsQueue.Peek();
                // 这行代码导致了后面的Hold如果没有被执行完是不能点击更新的hold的
                if (hold.isStarted)return;
            
                if (hold.keyType == keyType)
                {
                    var keyState = hold.TryTrigger();
                    if (keyState != KeyState.UnTrigger)
                    {
                        hold.keyEndEvent.Invoke(keyState);
                        hold.isOver = true;
                    }
                }
            }));
            TouchManager.Instance.rightHoldBeginEvent.AddListener((keyType =>
            {
                // isOver的出队
                while (rightHoldsQueue.Count != 0 && rightHoldsQueue.Peek().isOver)rightHoldsQueue.Dequeue();
                if(rightHoldsQueue.Count == 0)return;
                var hold = rightHoldsQueue.Peek();
                // 这行代码导致了后面的Hold如果没有被执行完是不能点击更新的hold的
                if (hold.isStarted)return;
                if (hold.keyType == keyType)
                {
                    var keyState = hold.TryTrigger();
                    if (keyState != KeyState.UnTrigger)
                    {
                        hold.keyEndEvent.Invoke(keyState);
                        hold.isOver = true;
                    }
                }
            }));
            TouchManager.Instance.leftHoldReleaseEvent.AddListener((keyType =>
            {
                while (leftHoldsQueue.Count != 0 && leftHoldsQueue.Peek().isOver)leftHoldsQueue.Dequeue();
                if(leftHoldsQueue.Count == 0)return;
                var hold = (Hold)leftHoldsQueue.Peek();
                //TODO 释放Hold的时候暂时没有条件限制 (就算是双长按,释放一个手指也能perfect)
                if (hold.isStarted)
                {
                    var keyState = hold.TryTrigger();
                    hold.keyEndEvent.Invoke(keyState);
                    hold.isOver = true;
                }
            }));
            TouchManager.Instance.rightHoldReleaseEvent.AddListener((keyType =>
            {
                while (rightHoldsQueue.Count != 0 && rightHoldsQueue.Peek().isOver)rightHoldsQueue.Dequeue();
                if(rightHoldsQueue.Count == 0)return;
                var hold = (Hold)leftHoldsQueue.Peek();
                //TODO 释放Hold的时候暂时没有条件限制 (就算是双长按,释放一个手指也能perfect)
                if (hold.isStarted)
                {
                    var keyState = hold.TryTrigger();
                    hold.keyEndEvent.Invoke(keyState);
                    hold.isOver = true;
                }
            }));
        }

        #endregion
    
    
        private void Miss()
        {
            Debug.Log("Miss");
        }
    
        private void Good()
        {
            Debug.Log("Good");
        }

        private void Perfect()
        {
            Debug.Log("Perfect");
        }

        private void MissVFX()
        {
        }

        private void GoodVFX()
        {
        }

        private void PerfectVFX()
        {
        }
    }
}