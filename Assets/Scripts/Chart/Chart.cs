using System;
using System.Collections.Generic;
using System.IO;
using RhythmTool;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ChartNamespace
{
    [Serializable]
    public class Chart : SerializedScriptableObject
    {
        public AudioClip audioClip;
        public int BPM;
        // BPM要乘的值 (决定了一个标准beat要分成几个小beat)
        public int multiple = 1;

        //初始速度, 之后的速度交给之后的key决定
        public float initSpeed;
        
        // 可能需要(?) 秒为单位的偏移量
        public float offset;

        public float SecondPerBeat => 60f / (BPM * multiple);

        private void OnValidate()
        {
            UpdateKeys(SecondPerBeat,offset);
        }

        #region ChartToRhythmDataConverter

        [Space]
        [SerializeField]
        public List<Track> tracks;
        [SerializeField]
        private RhythmData rhythmData;
        public float keyWidth = 0.02f;

#if UNITY_EDITOR
        [Button]
        public void ConvertToRhythmData()
        {
            tracks = new List<Track>();

            var leftTrack = BeatTrack.CreateInstance(LeftOrRight.Left);
            var rightTrack = BeatTrack.CreateInstance(LeftOrRight.Right);
            leftTrack.chart = this;
            rightTrack.chart = this;
            foreach (var key in keys)
            {
                switch (key.keyType)
                {
                    case KeyType.LeftSingle or KeyType.LeftDouble:
                        leftTrack.Add(new Beat
                        {
                            length = keyWidth,
                            timestamp = key.Time,
                            // keyIndex = leftTrack.count,
                            key = key,
                            // chart = this
                        });
                        break;
                    case KeyType.RightSingle or KeyType.RightDouble:
                        rightTrack.Add(new Beat
                        {
                            length = keyWidth,
                            timestamp = key.Time,
                            // keyIndex = rightTrack.count,
                            key = key,
                            // chart = this
                        });
                        break;
                    case KeyType.LeftSimultaneous:
                        leftTrack.Add(new Beat
                        {
                            length = ((Hold)key).endTime - ((Hold)key).Time,
                            timestamp = key.Time,
                            // keyIndex = leftTrack.count,
                            key = key,
                            // chart = this
                        });
                        break;
                    case KeyType.RightSimultaneous:
                        rightTrack.Add(new Beat
                        {
                            length = ((Hold)key).endTime - ((Hold)key).Time,
                            timestamp = key.Time,
                            // keyIndex = rightTrack.count,
                            key = key,
                            // chart = this
                        });
                        break;
                }
            }
            tracks.Add(leftTrack);
            tracks.Add(rightTrack);
            rhythmData = RhythmData.Create(audioClip,tracks);
            rhythmData.chart = this;
            var curPath = AssetDatabase.GetAssetPath(this);
            curPath = Path.GetDirectoryName(curPath);
            curPath = curPath.Replace("\\","/");
            Utility.AssetCreator.SafelyCreateAsset(leftTrack,Path.Combine(curPath,"leftTrack.asset"));
            Utility.AssetCreator.SafelyCreateAsset(rightTrack,Path.Combine(curPath,"rightTrack.asset"));
            Utility.AssetCreator.SafelyCreateAsset(rhythmData, Path.Combine(curPath,"rhythmData.asset"));
            AssetDatabase.SaveAssets();
        }
#endif
        
        #endregion
        
        // 所有键
        public List<IKey> keys = new List<IKey>();
        
        public void PushBackKey(IKey key)
        {
            keys.Add(key);
        }

        public void UpdateKeys(float secondPerBeat,float offset)
        {
            foreach (var key in keys)
            {
                key.UpdateTime(secondPerBeat,offset);
            }
        }

        public void Sort()
        {
            keys.Sort(Comparer<IKey>.Create((key0, key1) => key0.beat.CompareTo(key1.beat)));
        }
    }
}