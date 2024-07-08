using System;
using UnityEngine.Rendering;

namespace BlurEffect
{
    [Serializable]
    public class BlurVolumeComponent : VolumeComponent
    {
        public BoolParameter isActive = new BoolParameter(true);
    }
}