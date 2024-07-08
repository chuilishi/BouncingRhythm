#if UNITY_EDITOR
using System;
using System.IO;
using Core;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Utility;

namespace ChartNamespace
{
    /// <summary>
    /// 根据Chart来绘制折线以及折线上的点
    /// </summary>
    public class ChartGenerator : MonoBehaviour
    {
        #region GeneratorPart
        [Header("左黑右白")]
        public SpriteRenderer leftSingleCircle;
        public SpriteRenderer rightSingleCirCle;
        public SpriteRenderer leftDoubleCircle;
        public SpriteRenderer rightDoubleCircle;

        [Header("动态背景")]
        public Renderer backgroundRenderer;
        //准备创建的chart 
        private Chart chart;

        public LineRenderer lineRenderer;

        public Vector3 initPos = Vector3.zero;

        public Vector2 curDir;

        public MainCharacter mainCharacter;

        public AudioClip audioClip;
        private int curIndex = 1;

        private int lastBeat = 0;

        // 理论位置是
        private Vector2 theoreticalPosition;

        public float speed = 1f;
        [Header("chart的保存路径")]

        public int leftHoldStartBeat;
        public int rightHoldStartBEat;
        
        [Button]
        public void SaveChart()
        {
            chart.Sort();
            AssetCreator.SafelyCreateAsset(chart,Path.Combine("Assets/Chart",audiosource.clip.name,audiosource.clip.name+"Chart"+".asset"));
            EditorUtility.SetDirty(chart);
            AssetDatabase.SaveAssets();
        }
        private void Awake()
        {
            chart = null;
            m_bpm = multiple * bpm;
            audiosource.clip.LoadAudioData();
            // 修改播放速度
            audiosource.pitch = (float)soundPitch;
            samplePerBeat = audiosource.clip.frequency / (m_bpm / 60.0f);
            secondPerBeat = 60f / m_bpm;
            OnBeat.AddListener((beatCount =>
            {
                sumForOriginBpm = sumForOriginBpm == multiple-1 ? 0 : sumForOriginBpm + 1;
            }));
        }

        private void Start()
        {
            CreateChart();
            Application.quitting += SaveChart;
        }
        
        private void CreateChart()
        {
            chart = ScriptableObject.CreateInstance<Chart>();
            chart.BPM = bpm;
            chart.multiple = multiple;
            chart.initSpeed = speed;
            chart.audioClip = audioClip;
            chart.keys.Add(new Tap(KeyType.LeftSingle,0,speed,Vector2.up,secondPerBeat,0));
            curDir = Vector3.up;
            lineRenderer.SetPosition(0,mainCharacter.transform.position);
            lineRenderer.SetPosition(1,mainCharacter.transform.position);
            // 计算theoreticalPosition 上一次的theoreticalPosition + curDir * 每beat走的路程 * (当前beat - lastBeat)
            void TheoreticalPositionUpdate()
            {
                theoreticalPosition += curDir * (float)secondPerBeat * speed *
                                       (curBeat - lastBeat);
                lastBeat = curBeat;
            }
            TouchManager.Instance.leftTapEvent.AddListener((keyType =>
            {
                switch (keyType)
                {
                    case KeyType.LeftSingle:
                        Instantiate(leftSingleCircle, mainCharacter.transform.position, Quaternion.identity);
                        TheoreticalPositionUpdate();
                        chart.PushBackKey(new Tap(KeyType.LeftSingle, curBeat,speed,curDir,secondPerBeat,0));
                        break;
                    case KeyType.LeftDouble:
                        if (curDir == Vector2.left+Vector2.up)
                        {
                            curDir = Vector2.right + Vector2.up;
                        }
                        else
                        {
                            curDir = Vector2.left + Vector2.up;
                        }
                        chart.PushBackKey(new Bounce(KeyType.LeftDouble, curBeat,speed,curDir,secondPerBeat,0));
                        Instantiate(leftDoubleCircle, mainCharacter.transform.position, Quaternion.identity);
                        lineRenderer.positionCount++;
                        curIndex++;
                        lineRenderer.SetPosition(curIndex,mainCharacter.transform.position);
                        break;
                }
            }));
            TouchManager.Instance.rightTapEvent.AddListener((keyType =>
            {
                switch (keyType)
                {
                    case KeyType.RightSingle:
                        chart.PushBackKey(new Tap(KeyType.RightSingle, curBeat,speed,curDir,secondPerBeat,0));
                        Instantiate(rightSingleCirCle, mainCharacter.transform.position, Quaternion.identity);
                        break;
                    case KeyType.RightDouble:
                        if (curDir == Vector2.left+Vector2.up)
                        {
                            curDir = Vector2.right + Vector2.up;
                        }
                        else
                        {
                            curDir = Vector2.left + Vector2.up;
                        }
                        chart.PushBackKey(new Bounce(KeyType.RightDouble, curBeat,speed,curDir,secondPerBeat,0));
                        Instantiate(rightDoubleCircle,mainCharacter.transform.position, Quaternion.identity);
                        lineRenderer.positionCount++;
                        curIndex++;
                        lineRenderer.SetPosition(curIndex,mainCharacter.transform.position);
                        break;
                }
            }));
            TouchManager.Instance.leftHoldBeginEvent.AddListener((keyType =>
            {
                leftHoldStartBeat = curBeat;
            }));
            TouchManager.Instance.leftHoldReleaseEvent.AddListener((keyType =>
            {
                if (curBeat == leftHoldStartBeat)return;
                chart.PushBackKey(new Hold(KeyType.LeftSimultaneous,leftHoldStartBeat,speed,curDir,curBeat * secondPerBeat,secondPerBeat,0));
            }));
            
            TouchManager.Instance.rightHoldBeginEvent.AddListener((keyType =>
            {
                rightHoldStartBEat = curBeat;
            }));
            TouchManager.Instance.rightHoldReleaseEvent.AddListener((keyType =>
            {
                if (curBeat == rightHoldStartBEat)return;
                chart.PushBackKey(new Hold(KeyType.RightSimultaneous,rightHoldStartBEat,speed,curDir,curBeat * secondPerBeat,secondPerBeat,0));
            }));
        }
        #endregion
        
        [Header("音乐")] public AudioSource audiosource;
    #region 相关计算
    [Header("音乐的BPM")] public int bpm;
    // [Header("节奏点和实际节拍的偏移量")]
    // public int offset;
    [Header("已经beat了多少次")] public int curBeat = 0;

    //每beat多少sample
    public float samplePerBeat;
    //每beat多少秒
    public float secondPerBeat ;
    public float lastSample;

    [Header("BPM 乘的倍数 (节拍会快多少倍)")]
    public int multiple = 1;
    // 真正的乘了multiple之后的bpm
    private int m_bpm;

    [Header("音乐的播放速度倍数")] 
    public float soundPitch = 1f;

    public float audioTime => audiosource.time;
    
    #endregion
    #region BitMessage相关
    /// <summary>
    /// 容忍度
    /// </summary>
    [Header("可施法范围占每Beat的范围(失误容忍度)(秒数)")] [Range(0, 1)]
    //perfect 指的是在这个范围内就是perfect
    public float perfectTolerance;

    [Header("Good判定的范围")] [Range(0, 1)] // 必须要比tolerance宽
    public float goodTolerance;

    [Header("Bad判定的范围")] [Range(0, 1)] 
    public float badTolerance;
    
    //目前状态
    [HideInInspector]
    public int beatState = 0;
    [Header("乐曲开始位置")]
    public int startSample = 0;

    private int sumForOriginBpm = 0;

    /// <summary>
    /// 根据BPM*multiple的 在Beat中的位置(周期快很多)
    /// </summary>
    public float beatPosInOneByOriginBpm;
    /// <summary>
    /// 根据原来的(没有multiple参与的)BPM的 在Beat中的位置
    /// </summary>
    public float beatPosInOneByMultipleBpm;
    
    #region beat相关的事件们
    // 返回的是当次Beat所属的BeatCounter (也就是在AfterAfterBeat才会让BeatCounter++)
    [HideInInspector]
    public UnityEvent<int> BeforeBeforeBeat = new UnityEvent<int>();
    [HideInInspector]
    public UnityEvent<int> BeforeBeat = new UnityEvent<int>();
    [HideInInspector]
    public UnityEvent<int> OnBeat = new UnityEvent<int>();
    [HideInInspector]
    public UnityEvent<int> AfterBeat = new UnityEvent<int>();
    [HideInInspector]
    public UnityEvent<int> AfterAfterBeat = new UnityEvent<int>();
    
    [HideInInspector]
    public UnityEvent AudioStart = new UnityEvent();
    
    [HideInInspector]
    public UnityEvent AudioPause = new UnityEvent();

    private static readonly int TimeOffset = Shader.PropertyToID("_TimeOffset");

    #endregion
    
    #endregion

    private void OnValidate()
    {
        m_bpm = multiple * bpm;
        samplePerBeat = audiosource.clip.frequency / (m_bpm / 60.0f);
        secondPerBeat = 60f / m_bpm;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            audiosource.Play();
            AudioStart.Invoke();
        }
        beatPosInOneByMultipleBpm = 1.0f * (audiosource.timeSamples - lastSample - startSample) / samplePerBeat;
        beatPosInOneByOriginBpm =
            1.0f * (audiosource.timeSamples - lastSample - startSample + sumForOriginBpm * samplePerBeat) /
            (samplePerBeat * multiple);
        BeatInvoker();
        
        mainCharacter.transform.Translate(curDir.normalized * speed * Time.deltaTime);
        lineRenderer.SetPosition(curIndex,mainCharacter.transform.position);
        
        backgroundRenderer.material.SetFloat(TimeOffset, beatPosInOneByOriginBpm);
    }
    /// <summary>
    /// 在beat前中后发送一次消息
    /// </summary>
    private void BeatInvoker()
    {
        if (audiosource.timeSamples < startSample) return;
        
        if (beatState == 0 && audiosource.timeSamples- startSample - lastSample > (1 - goodTolerance) * samplePerBeat)
        {
            BeforeBeforeBeat.Invoke(curBeat);
            beatState = 1;
        }
        else if (beatState == 1 &&
                 audiosource.timeSamples - startSample - lastSample > (1 - perfectTolerance) * samplePerBeat)
        {
            BeforeBeat.Invoke(curBeat);
            beatState = 2;
        }
        else if (beatState == 2 & audiosource.timeSamples - startSample - lastSample > samplePerBeat)
        {
            lastSample += samplePerBeat;
            
            OnBeat.Invoke(curBeat);
            beatState = 3;
        }
        else if (beatState == 3 && audiosource.timeSamples - startSample - lastSample > samplePerBeat * perfectTolerance)
        {
            beatState = 4;
            AfterBeat.Invoke(curBeat);
        }
        else if (beatState == 4 && audiosource.timeSamples - startSample - lastSample > samplePerBeat * goodTolerance)
        {
            beatState = 0;
            AfterAfterBeat.Invoke(curBeat);
            curBeat++;
        }
    }
    [Button("开始")]
    public void BeatStart()
    {
        audiosource.Play();
    }
    [Button("暂停/开始")]
    public void BeatPauseUnPause()
    {
        if(audiosource.isPlaying) audiosource.Pause();
        else audiosource.Play();
    }
    public int CurSample()
    {
        return audiosource.timeSamples;
    }
        
    }
}
#endif