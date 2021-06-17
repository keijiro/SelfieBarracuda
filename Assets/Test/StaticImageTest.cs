using UnityEngine;
using UI = UnityEngine.UI;
using MediaPipe.Selfie;

public sealed class StaticImageTest : MonoBehaviour
{
    [SerializeField] Texture2D _sourceImage = null;
    [SerializeField] UI.RawImage _imageUI = null;
    [SerializeField] ResourceSet _resources = null;

    SegmentationFilter _filter;

    void Start()
    {
        _filter = new SegmentationFilter(_resources);
        _filter.ProcessImage(_sourceImage);
        _imageUI.texture = _filter.MaskTexture;
    }

    void OnDestroy()
      => _filter.Dispose();

    // Enable for benchmarking
    // void Update() => _filter.ProcessImage(_sourceImage);
}
