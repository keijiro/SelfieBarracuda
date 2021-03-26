using UnityEngine;

namespace MeetBarracuda {

static class RTUtil
{
    public const RenderTextureFormat SingleChannelFormat
      = RenderTextureFormat.R8;

    public const RenderTextureFormat SingleChannelFloatFormat
      = RenderTextureFormat.RHalf;

    public const RenderTextureFormat DoubleChannelFloatFormat
      = RenderTextureFormat.RGHalf;

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

    public static RenderTexture NewDoubleChannelFloat(int width, int height)
      => new RenderTexture(width, height, 0, DoubleChannelFloatFormat);

    public static RenderTexture NewDoubleChannelFloatUAV(int width, int height)
    {
        var rt = NewDoubleChannelFloat(width, height);
        rt.enableRandomWrite = true;
        rt.Create();
        return rt;
    }

    public static RenderTexture TempSingleChannelFloat(int width, int height)
      => RenderTexture.GetTemporary(width, height, 0, SingleChannelFloatFormat);

    public static RenderTexture TempDoubleChannelFloat(int width, int height)
      => RenderTexture.GetTemporary(width, height, 0, DoubleChannelFloatFormat);

    public static void ReleaseTemp(RenderTexture rt)
      => RenderTexture.ReleaseTemporary(rt);
}

static class ComputeShaderExtensions
{
    static int[] _int2 = new int[2];

    public static void SetInts
      (this ComputeShader compute, string name, int x, int y)
    {
        _int2[0] = x; _int2[1] = y;
        compute.SetInts(name, _int2);
    }
}

} // namespace MeetBarracuda
