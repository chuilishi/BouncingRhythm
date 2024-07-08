using System;
using Core;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class OffsetAdjuster : MonoBehaviour
{
    public TMP_Text text;

    private float offsetSum;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            AdjustOffset();
            text.text = m_offset.ToString();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetOffset();
        }
    }
    [Button("重置调延迟")]
    public void ResetOffset()
    {
        offsetSum = 0;
    }
    private float m_offset = 0;
    private int startCounter = 8;
    private int count;
    [Button("调延迟")]
    public void AdjustOffset()
    {
        if (SoundManager.Instance.audioSource.timeSamples < BeatManager.Instance.startSample) return;
        if (SoundManager.Instance.audioSource.timeSamples - (BeatManager.Instance.startSample) - BeatManager.Instance.lastSample < (BeatManager.Instance.samplePerBeat*BeatManager.Instance.multiple) / 2)
        {
            if (startCounter > 0)
            {
                startCounter--;
                return;
            }
            offsetSum += (float)(SoundManager.Instance.audioSource.timeSamples - (BeatManager.Instance.startSample) - BeatManager.Instance.lastSample);
            count++;
            m_offset = offsetSum / count;
        }
    }
}