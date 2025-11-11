using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Ps2LookFeature : ScriptableRendererFeature
{
    class Ps2Pass : ScriptableRenderPass
    {
        static readonly string kTag = "Ps2Look";
        Material _mat;
        RenderTargetIdentifier _source;

        public Ps2Pass(Material mat)
        {
            _mat = mat;
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            _source = renderingData.cameraData.renderer.cameraColorTarget;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_mat == null) return;

            CommandBuffer cmd = CommandBufferPool.Get(kTag);
            RenderTargetIdentifier destination = _source;

            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            int tempID = Shader.PropertyToID("_TempPs2LookTex");
            cmd.GetTemporaryRT(tempID, desc);

            Blit(cmd, _source, tempID, _mat);
            Blit(cmd, tempID, destination);

            cmd.ReleaseTemporaryRT(tempID);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    [System.Serializable]
    public class Settings
    {
        public Material material;
    }

    public Settings settings = new Settings();
    Ps2Pass _pass;

    public override void Create()
    {
        if (settings.material != null)
            _pass = new Ps2Pass(settings.material);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_pass != null)
            renderer.EnqueuePass(_pass);
    }
}
