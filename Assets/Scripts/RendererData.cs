using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "Pipeline data", menuName = "MyPipeline/data")]
public class RendererData : ScriptableRendererData
{
    protected override ScriptableRenderer Create()
    {
        return new MyRenderer(this);
    }
}