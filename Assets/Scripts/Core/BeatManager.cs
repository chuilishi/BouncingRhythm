using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance;
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

    public float audioTime => SoundManager.Instance.audioSource.time;
    
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
    
    #endregion
    
    #endregion

    private void OnValidate()
    {
        Instance = this;
        m_bpm = multiple * bpm;
        // samplePerBeat = SoundManager.Instance.audioSource.clip.frequency / (m_bpm / 60.0f);
        secondPerBeat = 60f / m_bpm;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        m_bpm = multiple * bpm;
        SoundManager.Instance.audioSource.clip.LoadAudioData();
        // 修改播放速度
        SoundManager.Instance.audioSource.pitch = (float)soundPitch;
        samplePerBeat = SoundManager.Instance.audioSource.clip.frequency / (m_bpm / 60.0f);
        secondPerBeat = 60f / m_bpm;
        OnBeat.AddListener((beatCount =>
        {
            sumForOriginBpm = sumForOriginBpm == multiple-1 ? 0 : sumForOriginBpm + 1;
        }));
    }

    private void Update()
    {
        beatPosInOneByMultipleBpm = 1.0f * (SoundManager.Instance.audioSource.timeSamples - lastSample - startSample) / samplePerBeat;
        beatPosInOneByOriginBpm =
            1.0f * (SoundManager.Instance.audioSource.timeSamples - lastSample - startSample + sumForOriginBpm * samplePerBeat) /
            (samplePerBeat * multiple);
        BeatInvoker();
    }
    /// <summary>
    /// 在beat前中后发送一次消息
    /// </summary>
    private void BeatInvoker()
    {
        if (SoundManager.Instance.audioSource.timeSamples < startSample) return;
        
        if (beatState == 0 && SoundManager.Instance.audioSource.timeSamples- startSample - lastSample > (1 - goodTolerance) * samplePerBeat)
        {
            BeforeBeforeBeat.Invoke(curBeat);
            beatState = 1;
        }
        else if (beatState == 1 &&
                 SoundManager.Instance.audioSource.timeSamples - startSample - lastSample > (1 - perfectTolerance) * samplePerBeat)
        {
            BeforeBeat.Invoke(curBeat);
            beatState = 2;
        }
        else if (beatState == 2 & SoundManager.Instance.audioSource.timeSamples - startSample - lastSample > samplePerBeat)
        {
            lastSample += samplePerBeat;
            
            OnBeat.Invoke(curBeat);
            beatState = 3;
        }
        else if (beatState == 3 && SoundManager.Instance.audioSource.timeSamples - startSample - lastSample > samplePerBeat * perfectTolerance)
        {
            beatState = 4;
            AfterBeat.Invoke(curBeat);
        }
        else if (beatState == 4 && SoundManager.Instance.audioSource.timeSamples - startSample - lastSample > samplePerBeat * goodTolerance)
        {
            beatState = 0;
            AfterAfterBeat.Invoke(curBeat);
            curBeat++;
        }
    }
    [Button("开始")]
    public void BeatStart()
    {
        SoundManager.Instance.audioSource.Play();
    }
    [Button("暂停/开始")]
    public void BeatPauseUnPause()
    {
        if(SoundManager.Instance.audioSource.isPlaying) SoundManager.Instance.audioSource.Pause();
        else SoundManager.Instance.audioSource.Play();
    }
    public int CurSample()
    {
        return SoundManager.Instance.audioSource.timeSamples;
    }
}