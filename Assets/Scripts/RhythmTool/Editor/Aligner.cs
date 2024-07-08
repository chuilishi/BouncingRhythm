using System;
using UnityEngine;

namespace RhythmTool
{
    public class Aligner
    {
        private const int Frequency = 44100;
        private static Aligner _instance;
        public static Aligner Instance
        {
            get { return _instance ??= new Aligner(); }
        }
        /// <summary>
        /// 根据BPM对齐
        /// </summary>
        /// <param name="timeStamp">现在播放到的秒数</param>
        /// <returns></returns>
        public float Align(float timeStamp, out int beatCount)
        {
            timeStamp -= RhythmToolWindow.instance.offset;
            var secondPerBeat = (60.0f/RhythmToolWindow.instance.BPM);
            if (timeStamp % secondPerBeat >= secondPerBeat/2.0f)
            {
                beatCount = Mathf.CeilToInt(timeStamp / secondPerBeat);
                return Mathf.CeilToInt(timeStamp/secondPerBeat)*secondPerBeat + RhythmToolWindow.instance.offset;
            }
            else
            {
                beatCount = Mathf.FloorToInt(timeStamp / secondPerBeat);
                return Mathf.FloorToInt(timeStamp/secondPerBeat)*secondPerBeat + RhythmToolWindow.instance.offset;
            }
        }
        public float Align(float timeStamp)
        {
            timeStamp -= RhythmToolWindow.instance.offset;
            var secondPerBeat = (60.0f/RhythmToolWindow.instance.BPM);
            if (timeStamp % secondPerBeat >= secondPerBeat/2.0f)
            {
                return Mathf.CeilToInt(timeStamp/secondPerBeat)*secondPerBeat + RhythmToolWindow.instance.offset;
            }
            else
            {
                return Mathf.FloorToInt(timeStamp/secondPerBeat)*secondPerBeat + RhythmToolWindow.instance.offset;
            }
        }
    }
}