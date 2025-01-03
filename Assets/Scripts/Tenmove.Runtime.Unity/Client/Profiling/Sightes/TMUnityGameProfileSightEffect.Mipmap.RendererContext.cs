using UnityEngine;

namespace Tenmove.Runtime.Unity
{
    public partial class UnityGameProfileSightEffect
    {
        public/*private*/ partial class Mipmap
        {
            private abstract class RendererContext
            {
                protected readonly Mipmap m_Mipmap;
                private bool m_IsEnableMipmap;

                public RendererContext(Mipmap mipmap)
                {
                    Debugger.Assert(null != mipmap, "Parameter 'mipmap' can not be null!");

                    m_Mipmap = mipmap;
                    m_IsEnableMipmap = false;
                }

                public void EnableMipmap()
                {
                    if (!m_IsEnableMipmap)
                    {
                        m_IsEnableMipmap = true;
                        _OnEnableMipmap();
                    }
                    else
                        Debugger.LogWarning("Already enable mipmap view mode!");
                }

                public void DisableMipmap()
                {
                    if (m_IsEnableMipmap)
                    {
                        m_IsEnableMipmap = false;
                        _OnDisableMipmap();
                    }
                    else
                        Debugger.LogWarning("Already disable mipmap view mode!");
                }

                protected abstract void _OnEnableMipmap();
                protected abstract void _OnDisableMipmap();
            }

            private class RendererContext<T> : RendererContext where T : Renderer
            {
                private readonly Renderer m_Renderer;
                private readonly Material[] m_OriginMaterials;
                private readonly Material[] m_MipmapMaterials;

                public RendererContext(Mipmap mipmap,T renderer)
                    : base(mipmap)
                {
                    Debugger.Assert(null != renderer, "Parameter 'renderer' can not be null!");

                    m_Renderer = renderer;
                    m_OriginMaterials = new Material[m_Renderer.sharedMaterials.Length];
                    for (int i = 0, icnt = m_Renderer.sharedMaterials.Length; i < icnt; ++i)
                        m_OriginMaterials[i] = m_Renderer.sharedMaterials[i];

                    m_MipmapMaterials = new Material[m_Renderer.sharedMaterials.Length];
                    for (int i = 0, icnt = m_MipmapMaterials.Length; i < icnt; ++i)
                        m_MipmapMaterials[i] = m_Mipmap.m_MipMapMaterial;
                }

                protected override void _OnEnableMipmap()
                {
                    m_Renderer.materials = m_MipmapMaterials;
                }

                protected override void _OnDisableMipmap()
                {
                    m_Renderer.materials = m_OriginMaterials;
                }
            }
        }
    }
}