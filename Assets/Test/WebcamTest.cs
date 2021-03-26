using UnityEngine;
using UI = UnityEngine.UI;

namespace MeetBarracuda {

public sealed class WebcamTest : MonoBehaviour
{
    [SerializeField] WebcamInput _webcam = null;
    [SerializeField] Texture2D _background = null;
    [SerializeField] UI.RawImage _imageUI = null;
    [SerializeField] ResourceSet _resources = null;
    [SerializeField] Shader _shader = null;

    SegmentationFilter _filter;
    RenderTexture _composited;
    Material _material;

    void Start()
    {
        _filter = new SegmentationFilter(_resources);
        _composited = new RenderTexture(1920, 1080, 0);
        _material = new Material(_shader);
    }

    void OnDestroy()
    {
        _filter.Dispose();
        Destroy(_composited);
        Destroy(_material);
    }

    void Update()
    {
        _filter.ProcessImage(_webcam.Texture);

        _material.SetTexture("_SourceTexture", _webcam.Texture);
        _material.SetTexture("_MaskTexture", _filter.MaskTexture);
        _material.SetTexture("_BGTexture", _background);
        Graphics.Blit(null, _composited, _material, 0);

        _imageUI.texture = _composited;
    }
}

} // namespace MeetBarracuda
