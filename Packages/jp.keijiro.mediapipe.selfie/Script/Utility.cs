using UnityEngine;

namespace MediaPipe.Selfie {

static class RTUtil
{
    public const RenderTextureFormat SingleChannelFormat
      = RenderTextureFormat.R8;

    public const RenderTextureFormat SingleChannelFloatFormat
      = RenderTextureFormat.RHalf;

    public static RenderTexture NewSingleChannelUAV(int width, int height)
    {
        var rt = new RenderTexture(width, height, 0, SingleChannelFormat);
        rt.enableRandomWrite = true;
        rt.Create();
        return rt;
    }

    public static RenderTexture NewSingleChannelFloatUAV(int width, int height)
    {
        var rt = new RenderTexture(width, height, 0, SingleChannelFloatFormat);
        rt.enableRandomWrite = true;
        rt.Create();
        return rt;
    }

    public static RenderTexture NewSingleChannelFloat(int width, int height)
      => new RenderTexture(width, height, 0, SingleChannelFloatFormat);

    public static void ReleaseTemp(RenderTexture rt)
      => RenderTexture.ReleaseTemporary(rt);
}

static class ComputeShaderExtensions
{
    public static void DispatchThreads
      (this ComputeShader compute, int kernel, int x, int y, int z)
    {
        uint xc, yc, zc;
        compute.GetKernelThreadGroupSizes(kernel, out xc, out yc, out zc);

        x = (x + (int)xc - 1) / (int)xc;
        y = (y + (int)yc - 1) / (int)yc;
        z = (z + (int)zc - 1) / (int)zc;

        compute.Dispatch(kernel, x, y, z);
    }
}

} // namespace MediaPipe.Selfie
