using UnityEngine;
using Unity.Barracuda;

namespace MediaPipe.Selfie {

public sealed class SegmentationFilter : System.IDisposable
{
    #region Public members

    public Texture MaskTexture => _buffers.output;

    public SegmentationFilter(ResourceSet resources)
    {
        _resources = resources;
        AllocateObjects();
    }

    public void Dispose()
      => DeallocateObjects();

    public void ProcessImage(Texture sourceTexture)
      => RunModel(sourceTexture);

    #endregion

    #region Private objects

    ResourceSet _resources;
    (int w, int h) _size;
    IWorker _worker;

    (ComputeBuffer preprocess,
     RenderTexture inference,
     RenderTexture temp1,
     RenderTexture temp2,
     RenderTexture output) _buffers;

    void AllocateObjects()
    {
        var model = ModelLoader.Load(_resources.model);

        var shape = model.inputs[0].shape; // NHWC
        _size = (shape[6], shape[5]);      // (W, H)

        _worker = model.CreateWorker();

        _buffers = (new ComputeBuffer(_size.w * _size.h * 3, sizeof(float)),
                    RTUtil.NewSingleChannelFloat(_size.w, _size.h),
                    RTUtil.NewSingleChannelFloatUAV(_size.w, _size.h),
                    RTUtil.NewSingleChannelFloatUAV(_size.w, _size.h),
                    RTUtil.NewSingleChannelUAV(_size.w, _size.h));
    }


    void DeallocateObjects()
    {
        _worker?.Dispose();
        _worker = null;

        _buffers.preprocess?.Dispose();
        Object.Destroy(_buffers.inference);
        Object.Destroy(_buffers.temp1);
        Object.Destroy(_buffers.temp2);
        Object.Destroy(_buffers.output);
        _buffers = (null, null, null, null, null);
    }

    #endregion

    #region Main inference function

    void RunModel(Texture source)
    {
        // Preprocessing
        var pre = _resources.preprocess;
        pre.SetInts("_Dimensions", _size.w, _size.h);

        pre.SetTexture(0, "_InputTexture", source);
        pre.SetBuffer(0, "_OutputBuffer", _buffers.preprocess);
        pre.DispatchThreads(0, _size.w, _size.h, 1);

        // NN worker invocation
        using (var tensor = new Tensor(1, _size.h, _size.w, 3,
                                       _buffers.preprocess))
            _worker.Execute(tensor);

        // Postprocessing (erosion + bilateral filter)
        _worker.PeekOutput().ToRenderTexture(_buffers.inference);

        var post = _resources.postprocess;
        post.SetInts("_Dimensions", _size.w, _size.h);

        post.SetTexture(0, "_Inference", _buffers.inference);
        post.SetTexture(0, "_MaskOutput", _buffers.temp1);
        post.DispatchThreads(0, _size.w, _size.h, 1);

        post.SetTexture(1, "_MaskInput", _buffers.temp1);
        post.SetTexture(1, "_MaskOutput", _buffers.temp2);
        post.DispatchThreads(1, _size.w, _size.h, 1);

        post.SetTexture(2, "_MaskInput", _buffers.temp2);
        post.SetTexture(2, "_MaskOutput", _buffers.output);
        post.DispatchThreads(2, _size.w, _size.h, 1);
    }

    #endregion
}

} // namespace MediaPipe.Selfie
