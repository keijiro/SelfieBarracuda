using UnityEngine;
using Unity.Barracuda;

namespace MediaPipe.Selfie {

[CreateAssetMenu(fileName = "SelfieBarracuda",
                 menuName = "ScriptableObjects/SelfieBarracuda Resource Set")]
public sealed class ResourceSet : ScriptableObject
{
    public NNModel model;
    public ComputeShader preprocess;
    public ComputeShader postprocess;
}

} // namespace MediaPipe.Selfie
