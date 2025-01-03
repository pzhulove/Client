
namespace UnityEngine.UI{    /// <summary>
    /// 只会作为raycast target，不会参与渲染。
    /// </summary>    [AddComponentMenu("UI/NullImage", 12)]
    public class NullImage : MaskableGraphic    {        protected NullImage()        {            useLegacyMeshGeneration = false;        }

        protected override void OnPopulateMesh(VertexHelper toFill)        {            toFill.Clear();        }    }}
