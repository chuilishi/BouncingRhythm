using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Core
{
    public class LevelManager : MonoBehaviour
    {
        private int backgroundSum;
        public static LevelManager Instance;
        private int curLevel;
        private readonly List<float> levelXs = new List<float>();
        public List<Level> levels = new List<Level>();
    
        public int CurLevel
        {
            get => curLevel;
            private set => curLevel = (value + backgroundSum) % backgroundSum;
        }

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            backgroundSum = UIManager.Instance.backgrounds.Count;
            for (int i = 0; i < backgroundSum; i++)
            {
                levelXs.Add(i * UIManager.Instance.distanceBetweenLevel);
            }
        }
        public UniTask SwitchLevel(int levelIndex)
        {
            var backgrounds = UIManager.Instance.backgrounds;
            //停止当前动画
            backgrounds[CurLevel].enable = false;
            // 先用curLevel的属性过滤一遍
            CurLevel = levelIndex;
            float targetX = levelXs[CurLevel];
            var characterTransform = MainCharacter.Instance.transform;
            var originX = characterTransform.position.x;
            var tcs = new UniTaskCompletionSource();
            characterTransform.DOMoveX(targetX,
                UIManager.Instance.speed * Mathf.Abs(targetX - originX) / UIManager.Instance.distanceBetweenLevel).SetEase(UIManager.Instance.easeType).onComplete += () =>
            {
                tcs.TrySetResult();
                backgrounds[CurLevel].enable = true;
            };
            return tcs.Task;
        }
    }
}