using UnityEngine;
using UI = UnityEngine.UI;

namespace MeetBarracuda {

public sealed class WebcamTest : MonoBehaviour
{
    [SerializeField] WebcamInput _webcam = null;
    [SerializeField] UI.RawImage _sourceUI = null;
    [SerializeField] UI.RawImage _maskUI = null;
    [SerializeField] ResourceSet _resources = null;

    SegmentationFilter _filter;

    void Start()
      => _filter = new SegmentationFilter(_resources);

    void OnDestroy()
      => _filter.Dispose();

    void Update()
    {
        _filter.ProcessImage(_webcam.Texture);

        _sourceUI.texture = _webcam.Texture;
        _maskUI.texture = _filter.MaskTexture;
    }
}

} // namespace MeetBarracuda
