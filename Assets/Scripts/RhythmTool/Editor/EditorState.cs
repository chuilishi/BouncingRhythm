using System;
using UnityEngine;

namespace RhythmTool
{
    [Serializable]
    public class EditorState
    {
        public float start;

        public float length;

        public float pixelsPerSecond;

        public bool featureClicked;
        public bool trackClicked;
        public bool manipulatorClicked;

        public bool dragging;

        public bool refresh;

        public Track curTrack;

        public Vector2 mouseDownPosition;

        public float TimestampToPixels(float timestamp)
        {
            return (timestamp - start) * pixelsPerSecond;
        }

        public float PixelsToTimestamp(float pixels)
        {
            return (pixels / pixelsPerSecond) + start;
        }
    }
}