using System;
using ChartNamespace;
using UnityEngine;
using UnityEngine.Serialization;

namespace RhythmTool
{
    /// <summary>
    /// A Track that contains Beat Features.
    /// </summary>
    public class BeatTrack : Track<Beat>
    {
        public LeftOrRight leftOrRight;
        public static BeatTrack CreateInstance(LeftOrRight leftOrRight)
        {
            var beatTrack = ScriptableObject.CreateInstance<BeatTrack>();
            beatTrack.leftOrRight = leftOrRight;
            return beatTrack;
        }
    }

    /// <summary>
    /// A Beat represents the rhythm of the song.
    /// </summary>
    [Serializable]
    public class Beat : Feature
    {
        public float bpm;
    }
}