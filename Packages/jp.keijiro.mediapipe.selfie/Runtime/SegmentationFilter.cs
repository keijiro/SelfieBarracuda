using UnityEngine;
using Unity.Barracuda;
using Klak.NNUtils;
using Klak.NNUtils.Extensions;

namespace MediaPipe.Selfie {

public sealed class SegmentationFilter : System.IDisposable
{
    #region Public methods/properties

    public SegmentationFilter(ResourceSet resources)
      => AllocateObjects(resources);

    public void Dispose()
      => DeallocateObjects();

    public void ProcessImage(Texture sourceTexture)
      => RunModel(sourceTexture);

    public RenderTexture MaskTexture
      => _output;

    #endregion

    #region Private objects

    ResourceSet _resources;
    (int w, int h) _size;
    IWorker _worker;
    ImagePreprocess _preprocess;
    (GraphicsBuffer temp1, GraphicsBuffer temp2) _buffers;
    RenderTexture _output;

    void AllocateObjects(ResourceSet resources)
    {
        _resources = resources;

        // NN model
        var model = ModelLoader.Load(_resources.model);

        // Input shape
        var shape = model.inputs[0].GetTensorShape();
        _size = (shape.GetWidth(), shape.GetHeight());

        // GPU worker
        _worker = model.CreateWorker(WorkerFactory.Device.GPU);

        // Preprocessing buffers
        _preprocess = new ImagePreprocess(_size.w, _size.h)
          { ColorCoeffs = new Vector4(0, 0, 0, 1) };

        // Working buffers
        _buffers = (BufferUtil.NewStructured<float>(_size.w * _size.h),
                    BufferUtil.NewStructured<float>(_size.w * _size.h));

        // Output texture
        _output = RTUtil.NewSingleChannelUAV(_size.w, _size.h);
    }

    void DeallocateObjects()
    {
        _worker?.Dispose();
        _worker = null;

        _preprocess?.Dispose();
        _preprocess = null;

        _buffers.temp1?.Dispose();
        _buffers.temp2?.Dispose();
        _buffers = (null, null);

        RTUtil.Destroy(_output);
        _output = null;
    }

    #endregion

    #region Main inference function

    void RunModel(Texture source)
    {
        // Preprocessing
        _preprocess.Dispatch(source, _resources.preprocess);

        // NN worker invocation
        _worker.Execute(_preprocess.Tensor);

        // Postprocessing (erosion + bilateral filter)
        var post = _resources.postprocess;
        post.SetInts("_Dimensions", _size.w, _size.h);

        post.SetBuffer(0, "_Input", _worker.PeekOutputBuffer());
        post.SetBuffer(0, "_Output", _buffers.temp1);
        post.DispatchThreads(0, _size.w, _size.h, 1);

        post.SetBuffer(1, "_Input", _buffers.temp1);
        post.SetBuffer(1, "_Output", _buffers.temp2);
        post.DispatchThreads(1, _size.w, _size.h, 1);

        post.SetBuffer(2, "_Input", _buffers.temp2);
        post.SetTexture(2, "_OutputTexture", _output);
        post.DispatchThreads(2, _size.w, _size.h, 1);
    }

    #endregion
}

} // namespace MediaPipe.Selfie
