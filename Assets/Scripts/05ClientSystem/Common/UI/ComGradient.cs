using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace GameClient
{
    [AddComponentMenu("UI/Effects/Gradient")]
    class ComGradient : BaseMeshEffect
    {

        [SerializeField]
        private Color32 topColor = Color.white;
        [SerializeField]
        private Color32 bottomColor = Color.black;

        public override void ModifyMesh(VertexHelper vertexHelper)
        {
            if (!IsActive())
            {
                return;
            }

            if (topColor.Equals(bottomColor))
            {
                UIVertex vertex = new UIVertex();
                for (int i = 0; i < vertexHelper.currentVertCount; i++)
                {
                    vertexHelper.PopulateUIVertex(ref vertex, i);
                    vertex.color = topColor;
                    vertexHelper.SetUIVertex(vertex, i);
                }
            }
            else
            {
                if (vertexHelper.currentVertCount > 0 )
                {
                    UIVertex vertex = new UIVertex();
                    vertexHelper.PopulateUIVertex(ref vertex, 0);
                    float bottomY = vertex.position.y;
                    float topY = vertex.position.y;

                    for (int i = 1; i < vertexHelper.currentVertCount; i++)
                    {
                        vertexHelper.PopulateUIVertex(ref vertex, i);
                        float y = vertex.position.y;
                        if (y > topY)
                        {
                            topY = y;
                        }
                        else if (y < bottomY)
                        {
                            bottomY = y;
                        }
                    }

                    float uiElementHeight = topY - bottomY;

                    for (int i = 0; i < vertexHelper.currentVertCount; i++)
                    {
                        vertexHelper.PopulateUIVertex(ref vertex, i);
                        vertex.color = Color32.Lerp(bottomColor, topColor, (vertex.position.y - bottomY) / uiElementHeight);
                        vertexHelper.SetUIVertex(vertex, i);
                    }
                }
            }

        }
    }
}
