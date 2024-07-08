using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UI;
using UnityEngine;

namespace Core
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        private int levelSum;
        [Header("放手完之后回去的移动速度,相当于移动18花多长时间")][SerializeField] public float speed = 1;
        [Header("blur程度，越大越模糊")] public float blurSize = 0.2f;
        [Header("缓动曲线")] public Ease easeType;
        
        public List<IBackground> backgrounds = new List<IBackground>();
        [HideInInspector]
        public int distanceBetweenLevel = 18;
        
        private void Awake()
        {
            Instance = this;
            levelSum = backgrounds.Count;
        }
    }
}