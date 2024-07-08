using System;
using UnityEngine;

namespace UI
{
    public abstract class IBackground : MonoBehaviour
    {
        public bool enable = false;
        public void Update()
        {
            if(!enable)return;
            OnUpdate();
        }
        public abstract void OnUpdate();
    }
}