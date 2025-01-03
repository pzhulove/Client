using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tenmove.Editor.Unity
{
    public class SceneEffectiveAreaObject
    {
        // id is index in list.
        public int id = -1;
        public string name;
        public Renderer renderer;
        public Material originMaterial;
        public int totalArea = 0;
        public int effectiveArea = 0;
        public float effectiveRate = 0f;

        private Material m_ObjMarkMaterial;
        private MaterialPropertyBlock block;

        public SceneEffectiveAreaObject(int id, Renderer renderer)
        {
            this.id = id;
            this.renderer = renderer;

            originMaterial = renderer.sharedMaterial;
            m_ObjMarkMaterial = new Material(Shader.Find("Hidden/Debug/ObjectID"));
            m_ObjMarkMaterial.renderQueue = originMaterial.renderQueue;
            renderer.sharedMaterial = m_ObjMarkMaterial;


            if(renderer.gameObject.name == "render")
            {
                name = renderer.transform.parent.name;
            }
            else
            {
                name = renderer.gameObject.name;
            }
        }

        public void EnableRenderer(bool enable)
        {
            Assert.IsNotNull(renderer, "Renderer is null");
            renderer.enabled = enable;
        }

        public Bounds GetBounds()
        {
            Assert.IsNotNull(renderer, "Renderer is null");
            return renderer.bounds;
        }

        public void SetObjectID2Material()
        {
            Assert.IsNotNull(renderer, "Renderer is null");
            if (block == null)
                block = new MaterialPropertyBlock();

            renderer.GetPropertyBlock(block);

            Color objectIDColor = SceneEffectiveAreaUtility.EncodeObjectID(id);
            block.SetColor("_ObjectID", objectIDColor);

            renderer.SetPropertyBlock(block);
        }

        public void ResetMaterial()
        {
            Assert.IsNotNull(renderer, "Renderer is null");
            renderer.sharedMaterial = originMaterial;
            Object.DestroyImmediate(m_ObjMarkMaterial);
        }
    }
}
