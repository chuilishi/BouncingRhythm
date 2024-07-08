using ChartNamespace;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class TouchManager : MonoBehaviour
    {
        public static TouchManager Instance;
    
        //int 是touch的数量
        [HideInInspector] public UnityEvent<KeyType> leftTapEvent = new UnityEvent<KeyType>();
        [HideInInspector] public UnityEvent<KeyType> rightTapEvent = new UnityEvent<KeyType>();
        [HideInInspector] public UnityEvent<KeyType> leftHoldBeginEvent = new UnityEvent<KeyType>(); 
        [HideInInspector] public UnityEvent<KeyType> rightHoldBeginEvent = new UnityEvent<KeyType>();
        [HideInInspector] public UnityEvent<KeyType> leftHoldReleaseEvent = new UnityEvent<KeyType>();
        [HideInInspector] public UnityEvent<KeyType> rightHoldReleaseEvent = new UnityEvent<KeyType>();

#if UNITY_ANDROID
        [Header("在几帧之内算作双击")]
        public int doubleTouchToleranceFrame = 3;
        // 一个用于检测的计数器 每帧减1
        private int _leftDoubleTouchCounter;
        private int _rightDoubleTouchCounter;
#endif

        private void Awake()
        {
            Instance = this;
        }
        private void Update()
        {
#if UNITY_ANDROID
            void QueueUpdate(ref int counter)
            {
                if(counter != 0)counter--;
            }
            QueueUpdate(ref _leftDoubleTouchCounter);
            QueueUpdate(ref _rightDoubleTouchCounter);
#endif
        
            TouchEventTrigger();
        }

        private void TouchEventTrigger()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                if (Input.anyKeyDown)
                {
                    if (Input.GetKeyDown(KeyCode.J))
                    {
                        rightTapEvent.Invoke(KeyType.RightSingle);
                        rightHoldBeginEvent.Invoke(KeyType.RightSingle);
                    }
                    if (Input.GetKeyDown(KeyCode.K))
                    {
                        rightTapEvent.Invoke(KeyType.RightDouble);
                        rightHoldBeginEvent.Invoke(KeyType.RightDouble);
                    }
                    if (Input.GetKeyDown(KeyCode.D))
                    {
                        leftTapEvent.Invoke(KeyType.LeftDouble);
                        leftHoldBeginEvent.Invoke(KeyType.LeftDouble);
                    }
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        leftTapEvent.Invoke(KeyType.LeftSingle);
                        leftHoldBeginEvent.Invoke(KeyType.LeftSingle);
                    }
                    if (Input.GetKeyUp(KeyCode.J))
                    {
                        rightHoldReleaseEvent.Invoke(KeyType.RightSingle);
                    }
                    if (Input.GetKeyUp(KeyCode.K))
                    {
                        rightHoldReleaseEvent.Invoke(KeyType.RightDouble);
                    }
                    if (Input.GetKeyUp(KeyCode.D))
                    {
                        rightHoldReleaseEvent.Invoke(KeyType.LeftDouble);
                    }
                    if (Input.GetKeyUp(KeyCode.F))
                    {
                        leftHoldReleaseEvent.Invoke(KeyType.LeftSingle);
                    }
                }
            }
#endif
#if UNITY_ANDROID
            int count = Input.touchCount;
            if (count == 0) return;
            var touches = Input.touches;
            int leftBeganTouchCount = 0;
            int rightBeganTouchCount = 0;
            int leftStationaryTouchCount = 0;
            int rightStationaryTouchCount = 0;
            bool isHoldLeft = false;
            bool isHoldRight = false;
            bool isReleasedLeft = false;
            bool isReleasedRight = false;
            foreach (var touch in touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    if (touch.position.x < Screen.currentResolution.width / 2f)
                    {
                        leftBeganTouchCount++;
                        leftStationaryTouchCount++;
                        isHoldLeft = true;
                    }
                    else
                    {
                        rightBeganTouchCount ++;
                        rightStationaryTouchCount ++;
                        isHoldRight = true;
                    }
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    if (touch.position.x < Screen.currentResolution.width / 2f)
                    {
                        isReleasedLeft = true;
                    }
                    else
                    {
                        isReleasedRight = true;
                    }
                }
                else if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    if (touch.position.x < Screen.currentResolution.width / 2f)
                    {
                        leftStationaryTouchCount++;
                    }
                    else
                    {
                        rightStationaryTouchCount++;
                    }
                }
            }

            if (leftBeganTouchCount != 0)
            {
                if (leftBeganTouchCount == 1)
                {
                    leftTapEvent.Invoke(KeyType.LeftSingle);
                    if (_leftDoubleTouchCounter == 0) _leftDoubleTouchCounter = doubleTouchToleranceFrame;
                    else
                    {
                        _leftDoubleTouchCounter = 0;
                        leftTapEvent.Invoke(KeyType.LeftDouble);
                    }
                }
                else
                {
                    leftTapEvent.Invoke(KeyType.LeftDouble);
                }
            }

            if (rightBeganTouchCount != 0)
            {
                if (rightBeganTouchCount == 1)
                {
                    rightTapEvent.Invoke(KeyType.RightSingle);
                    if (_rightDoubleTouchCounter == 0) _rightDoubleTouchCounter = doubleTouchToleranceFrame;
                    else
                    {
                        _rightDoubleTouchCounter = 0;
                        rightTapEvent.Invoke(KeyType.RightDouble);
                    }
                }
                else
                {
                    rightTapEvent.Invoke(KeyType.RightDouble);
                }
            }
            if (isHoldLeft) leftHoldBeginEvent.Invoke(leftStationaryTouchCount == 1? KeyType.LeftSingle : KeyType.LeftDouble);
            if (isHoldRight)rightHoldBeginEvent.Invoke(rightStationaryTouchCount == 1? KeyType.RightSingle : KeyType.RightDouble);
            if (isReleasedLeft)leftHoldReleaseEvent.Invoke(leftStationaryTouchCount == 0? KeyType.LeftSingle : KeyType.LeftDouble);
            if (isReleasedRight)rightHoldReleaseEvent.Invoke(rightStationaryTouchCount == 0? KeyType.RightSingle : KeyType.RightDouble);
#endif
        }
    }
}
