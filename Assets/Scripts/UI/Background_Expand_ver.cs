using UnityEngine;

namespace UI
{
    public class Background_Expand_ver : IBackground
    {
        private Renderer _renderer;
        private static readonly int TimeOffset = Shader.PropertyToID("_TimeOffset");
        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }
        public override void OnUpdate()
        {
            _renderer.material.SetFloat(TimeOffset, BeatManager.Instance.beatPosInOneByOriginBpm);
        }
    }
}