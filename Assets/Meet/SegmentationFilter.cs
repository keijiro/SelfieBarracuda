using UnityEngine;
using Unity.Barracuda;

namespace MeetBarracuda {

public sealed class SegmentationFilter : System.IDisposable
{
    #region Public constructor

    public SegmentationFilter(ResourceSet resources)
    {
        _resources = resources;
        _worker = ModelLoader.Load(_resources.model).CreateWorker();

        var dim = (x: Config.ImageWidth, y: Config.ImageHeight);
        _buffers = (new ComputeBuffer(dim.x * dim.y * 3, sizeof(float)),
                    RTUtil.NewDoubleChannelFloat(dim.x, dim.y),
                    RTUtil.NewSingleChannelFloatUAV(dim.x, dim.y),
                    RTUtil.NewSingleChannelFloatUAV(dim.x, dim.y),
                    RTUtil.NewSingleChannelUAV(dim.x, dim.y));
    }

    #endregion

    #region IDisposable implementation

    public void Dispose()
    {
        _worker.Dispose();
        _worker = null;

        _buffers.preprocess.Dispose();
        Object.Destroy(_buffers.inference);
        Object.Destroy(_buffers.temp1);
        Object.Destroy(_buffers.temp2);
        Object.Destroy(_buffers.output);
        _buffers = (null, null, null, null, null);
    }

    #endregion

    #region Public accessors

    public Texture MaskTexture => _buffers.output;

    #endregion

    #region Internal objects

    ResourceSet _resources;
    IWorker _worker;

    (ComputeBuffer preprocess,
     RenderTexture inference,
     RenderTexture temp1,
     RenderTexture temp2,
     RenderTexture output) _buffers;

    #endregion

    #region Main image processing function

    public void ProcessImage(Texture sourceTexture)
    {
        var (width, height) = (Config.ImageWidth, Config.ImageHeight);

        // Preprocessing
        var pre = _resources.preprocess;
        pre.SetInts("_Dimensions", width, height);

        pre.SetTexture(0, "_InputTexture", sourceTexture);
        pre.SetBuffer(0, "_OutputBuffer", _buffers.preprocess);
        pre.Dispatch(0, width / 8, height / 8, 1);

        // NN worker invocation
        using (var tensor = new Tensor(1, height, width, 3, _buffers.preprocess))
            _worker.Execute(tensor);

        // Postprocessing
        _worker.PeekOutput().ToRenderTexture(_buffers.inference);

        var post = _resources.postprocess;
        post.SetInts("_Dimensions", width, height);

        post.SetTexture(0, "_Inference", _buffers.inference);
        post.SetTexture(0, "_MaskOutput", _buffers.temp1);
        post.Dispatch(0, width / 8, height / 8, 1);

        post.SetTexture(1, "_MaskInput", _buffers.temp1);
        post.SetTexture(1, "_MaskOutput", _buffers.temp2);
        post.Dispatch(1, width / 8, height / 8, 1);

        post.SetTexture(2, "_MaskInput", _buffers.temp2);
        post.SetTexture(2, "_MaskOutput", _buffers.output);
        post.Dispatch(2, width / 8, height / 8, 1);
    }

    #endregion
}

} // namespace MeetBarracuda
