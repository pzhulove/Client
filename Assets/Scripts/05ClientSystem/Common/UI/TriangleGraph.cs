using UnityEngine;
using System.Collections;

namespace UnityEngine.UI.Extensions
{
    [ExecuteAlways]
    public class TriangleGraph : Graphic
    {
        public RectTransform p0;
        public RectTransform p1;
        public RectTransform p2;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Color32 co = color;

            //vh.AddVert(rectTransform.localPosition + posA.localPosition + new Vector3(pAC.x, pAC.y, 0), co, new Vector2(0f, 0f));
            //vh.AddVert(rectTransform.localPosition + posB.localPosition + new Vector3(pBC.x, pBC.y, 0), co, new Vector2(0f, 1f));
            //vh.AddVert(rectTransform.localPosition + posC.localPosition + new Vector3(pCC.x, pCC.y, 0), co, new Vector2(1f, 1f));
            //vh.AddVert(posA.position, co, new Vector2(0f, 0f));
            //vh.AddVert(posB.position, co, new Vector2(0f, 1f));
            //vh.AddVert(posC.position, co, new Vector2(1f, 1f));

            vh.AddVert(p0.anchoredPosition3D, co, new Vector2(0f, 0f));
            vh.AddVert(p1.anchoredPosition3D, co, new Vector2(0f, 1f));
            vh.AddVert(p2.anchoredPosition3D, co, new Vector2(1f, 1f));

            vh.AddTriangle(0, 1, 2);
        }
    }
}
