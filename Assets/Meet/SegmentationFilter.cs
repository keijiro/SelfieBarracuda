using UnityEngine;
using Unity.Barracuda;

namespace MeetBarracuda {

public sealed class SegmentationFilter : System.IDisposable
{
    #region Public constructor

    public SegmentationFilter(ResourceSet resources)
    {
        _resources = resources;
        _preprocessed = new ComputeBuffer(Config.InputSize, sizeof(float));
        _postprocessed = RTUtil.NewSingleChannelUAV(Config.ImageWidth, Config.ImageHeight);
        _worker = ModelLoader.Load(_resources.model).CreateWorker();
    }

    #endregion

    #region IDisposable implementation

    public void Dispose()
    {
        Object.Destroy(_postprocessed);

        _preprocessed?.Dispose();
        _preprocessed = null;

        _worker?.Dispose();
        _worker = null;
    }

    #endregion

    #region Public accessors

    public Texture MaskTexture => _postprocessed;

    #endregion

    #region Internal objects

    ResourceSet _resources;
    ComputeBuffer _preprocessed;
    RenderTexture _postprocessed;
    IWorker _worker;

    #endregion

    #region Main image processing function

    public void ProcessImage(Texture sourceTexture)
    {
        var (width, height) = (Config.ImageWidth, Config.ImageHeight);

        // Preprocessing
        var pre = _resources.preprocess;
        pre.SetTexture(0, "_InputTexture", sourceTexture);
        pre.SetBuffer(0, "_OutputBuffer", _preprocessed);
        pre.SetInts("_Dimensions", width, height);
        pre.Dispatch(0, width / 8, height / 8, 1);

        // NN worker invocation
        using (var tensor = new Tensor(1, height, width, 3, _preprocessed))
            _worker.Execute(tensor);

        // Output retrieval
        var temp = RTUtil.TempDoubleChannelFloat(width, height);
        _worker.PeekOutput().ToRenderTexture(temp);

        // Postprocess
        var post = _resources.postprocess;
        post.SetTexture(0, "_InputTexture", temp);
        post.SetTexture(0, "_OutputTexture", _postprocessed);
        post.SetInts("_Dimensions", width, height);
        post.Dispatch(0, width / 8, height / 8, 1);

        RTUtil.ReleaseTemp(temp);
    }

    #endregion
}

} // namespace MeetBarracuda
