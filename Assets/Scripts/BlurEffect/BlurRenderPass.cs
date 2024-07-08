using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace BlurEffect
{
    public class BlurRenderPass : ScriptableRenderPass
    {
        private static readonly int horizontalBlurId =
            Shader.PropertyToID("_HorizontalBlur");
        private static readonly int verticalBlurId =
            Shader.PropertyToID("_VerticalBlur");

        private Material material;

        private RenderTextureDescriptor blurTextureDescriptor;
        private RTHandle blurTextureHandle;

        public BlurRenderPass(Material material)
        {
            this.material = material;
            blurTextureDescriptor = new RenderTextureDescriptor(Screen.width,
                Screen.height, RenderTextureFormat.Default, 0);
        }

        public override void Configure(CommandBuffer cmd,
            RenderTextureDescriptor cameraTextureDescriptor)
        {
            // Set the blur texture size to be the same as the camera target size.
            blurTextureDescriptor.width = cameraTextureDescriptor.width;
            blurTextureDescriptor.height = cameraTextureDescriptor.height;

            // Check if the descriptor has changed, and reallocate the RTHandle if necessary
            RenderingUtils.ReAllocateIfNeeded(ref blurTextureHandle, blurTextureDescriptor);
        }

        private void UpdateBlurSettings()
        {
            if (material == null) return;
            // var volumeComponent =
            //     VolumeManager.instance.stack.GetComponent<BlurVolumeComponent>();
            // Debug.Log("BlurRenderPass"+VolumeManager.instance.stack.GetComponent<BlurVolumeComponent>().GetInstanceID());
            // float horizontalBlur = volumeComponent.horizontalBlur.overrideState ?
            //     volumeComponent.horizontalBlur.value : settings.horizontalBlur;
            // float verticalBlur = volumeComponent.verticalBlur.overrideState ?
            //     volumeComponent.verticalBlur.value : settings.verticalBlur;
            material.SetFloat(horizontalBlurId, BlurSettings.Instance.horizontalBlur);
            material.SetFloat(verticalBlurId, BlurSettings.Instance.verticalBlur);
        }

        public override void Execute(ScriptableRenderContext context,
            ref RenderingData renderingData)
        {
            //Get a CommandBuffer from pool.
            CommandBuffer cmd = CommandBufferPool.Get();

            RTHandle cameraTargetHandle =
                renderingData.cameraData.renderer.cameraColorTargetHandle;

            UpdateBlurSettings();

            Blit(cmd, cameraTargetHandle, blurTextureHandle, material, 0);
            Blit(cmd, blurTextureHandle, cameraTargetHandle, material, 1);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Dispose()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                Object.Destroy(material);
            }
            else
            {
                Object.DestroyImmediate(material);
            }
#else
                Object.Destroy(material);
#endif

            if (blurTextureHandle != null) blurTextureHandle.Release();
        }
    }
}