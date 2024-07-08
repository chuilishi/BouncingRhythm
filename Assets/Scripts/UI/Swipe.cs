using System;
using BlurEffect;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI
{
    public class Swipe : MonoBehaviour
    {
        private Vector3 mouseOriginPos;
        private float mainCharacterX;
        private bool isDown = false;
        public bool enable = true;
        [Header("更换歌曲需要在X轴上移动多远")]
        public float distanceX;
        [Header("花多少时间变模糊")] [SerializeField] private float blurDuration = 0.1f;
        
        [Header("手指移动1,画面移动多少")] public float speed = 0.5f;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        private void Update()
        {
            SwipeController();
        }

        private void SwipeController()
        {
            if(!enable)return;
            if (!isDown)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    mainCharacterX = MainCharacter.Instance.transform.position.x;
                    mouseOriginPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    isDown = true;
                    BlurSettings.Instance.Blur(blurDuration,UIManager.Instance.blurSize);
                    SoundManager.Instance.Mute();
                }
            }
            else
            {
                var minus = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - mouseOriginPos.x;
                Debug.Log(minus);
                if (Input.GetMouseButtonUp(0))
                {
                    isDown = false;
                    BlurSettings.Instance.UnBlur(blurDuration);
                    if (Mathf.Abs(minus)>distanceX)
                    {
                        enable = false;
                        if (minus < 0)
                        {
                            var task = LevelManager.Instance.SwitchLevel(LevelManager.Instance.CurLevel + 1);
                            task.GetAwaiter().OnCompleted((() =>
                            {
                                enable = true;
                                SoundManager.Instance.Restart();
                                SoundManager.Instance.UnMute();
                            }));
                        }
                        else
                        {
                            var task = LevelManager.Instance.SwitchLevel(LevelManager.Instance.CurLevel - 1);
                            task.GetAwaiter().OnCompleted((() =>
                            {
                                enable = true;
                                SoundManager.Instance.Restart();
                                SoundManager.Instance.UnMute();
                            }));
                        }
                    }
                }
                //  MainCharacter 跟随手指移动
                else
                {
                    MainCharacter.Instance.transform.position = new Vector3((mainCharacterX - speed*minus),0,0);
                }
            }
        }
#endif
    }
}