using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tenmove.Runtime.Unity
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class TMEffectLightComponent : MonoBehaviour
    {
        [SerializeField] private Renderer[] m_LightRenderers;
        [SerializeField] private Vector3 m_LightLocalPos;
        private static readonly int m_LightPosID = Shader.PropertyToID("_EffectPointLightPos");

        void Update()
        {
            if (transform.hasChanged)
            {
                transform.hasChanged = false;

                for (int i = 0; i < m_LightRenderers.Length; ++i)
                {
                    if(m_LightRenderers[i] != null)
                    {
                        Material material = m_LightRenderers[i].sharedMaterial;
                        if (material != null)
                            material.SetVector(m_LightPosID, transform.localToWorldMatrix.MultiplyPoint(m_LightLocalPos));
                    }
                }
            }
        }
    }
}

