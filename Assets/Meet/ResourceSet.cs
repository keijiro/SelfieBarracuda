using UnityEngine;
using Unity.Barracuda;

namespace MeetBarracuda {

[CreateAssetMenu(fileName = "MeetBarracuda",
                 menuName = "ScriptableObjects/MeetBarracuda Resource Set")]
public sealed class ResourceSet : ScriptableObject
{
    public NNModel model;
    public ComputeShader preprocess;
    public ComputeShader postprocess;
}

} // namespace MeetBarracuda
