using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComModifyColor : BaseMeshEffect
    {
        Color m_colAddColor = Color.white;
        public Color colAddColor
        {
            get
            {
                return m_colAddColor;
            }
            set
            {
                m_colAddColor = value;
                graphic.SetVerticesDirty();
            }
        }

        public override void ModifyMesh(VertexHelper vertexHelper)
        {
            if (!IsActive())
            {
                return;
            }

            UIVertex vertex = new UIVertex();
            for (int i = 0; i < vertexHelper.currentVertCount; i++)
            {
                try
                {
                    vertexHelper.PopulateUIVertex(ref vertex, i);
                    vertex.color *= colAddColor;
                    vertexHelper.SetUIVertex(vertex, i);
                }
                catch (Exception e)
                {
                    //Logger.LogErrorFormat("i = {0} count = {1}", i, vertexHelper.currentVertCount);
                }
            }
        }
    }
}
