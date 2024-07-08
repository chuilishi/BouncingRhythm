using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BlurEffect
{
    public class BlurRendererFeature : ScriptableRendererFeature
    {
        [SerializeField] private BlurSettings settings;
        [SerializeField] private Shader shader;
        private Material material;
        private BlurRenderPass blurRenderPass;

        public override void Create()
        {
            if (shader == null) return;
            if (material == null) material = new Material(shader);
            blurRenderPass = new BlurRenderPass(material);
            blurRenderPass.renderPassEvent = RenderPassEvent.AfterRendering;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer,
            ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Game)
            {
                renderer.EnqueuePass(blurRenderPass);
            }
        }
        protected override void Dispose(bool disposing)
        {
            blurRenderPass.Dispose();
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                Destroy(material);
            }
            else
            {
                DestroyImmediate(material);
            }
#else
                Destroy(material);
#endif
        }
    }
}