using System.Collections.Generic;
using UnityEngine;

namespace Tenmove.Runtime.Unity
{
    public partial class UnityGameProfileSightEffect
    {
        public/*private*/ partial class Mipmap
        {
            private const int MIPMAP_TEXTURE_SIZE = 1024;
            private readonly Color[] MipMapColors = new Color[]
            {
                new Color(1,1,1),/// 第0层 白色
                new Color(1,1,0),/// 第1层 黄色
                new Color(1,0,1),/// 第2层 洋红
                new Color(1,0,0),/// 第3层 红色
                new Color(0,1,1),/// 第4层 青色
                new Color(0,1,0),/// 第5层 绿色
                new Color(0,0,1),/// 第6层 蓝色
                new Color(0,0,0),/// 第7层及以上 黑色
            };

            private readonly Texture2D m_MipMapViewTexture;
            private readonly Material m_MipMapMaterial;

            private class GameObjectContext
            {
                private readonly GameObject m_Effect;
                private readonly List<RendererContext> m_Renderers;
                private bool m_EnableMipmap; 
                
                public GameObjectContext(Mipmap mipmap,GameObject effect)
                {
                    Debugger.Assert(null != mipmap, "Parameter 'mipmap' can not be null!");
                    Debugger.Assert(null != effect, "Parameter 'effect' can not be null!");

                    List<Renderer> renderer = FrameStackList<Renderer>.Acquire();
                    effect.GetComponentsInChildren(renderer);

                    m_Renderers = new List<RendererContext>(renderer.Count);
                    for (int i = 0, icnt = renderer.Count; i < icnt; ++i)
                    {
                        Renderer cur = renderer[i];
                        if(null != cur)
                            m_Renderers.Add(new RendererContext<Renderer>(mipmap, cur));
                    }
                    FrameStackList<Renderer>.Recycle(renderer);

                    m_Effect = effect;
                    m_EnableMipmap = false;
                }

                public int InstanceID
                {
                    get { return m_Effect.GetInstanceID(); }
                }

                public void EnableMipmap()
                {
                    if (!m_EnableMipmap)
                    {
                        for (int i = 0, icnt = m_Renderers.Count; i < icnt; ++i)
                            m_Renderers[i].EnableMipmap();

                        m_EnableMipmap = true;
                    }
                }

                public void DisableMipmap()
                {
                    if (m_EnableMipmap)
                    {
                        for (int i = 0, icnt = m_Renderers.Count; i < icnt; ++i)
                            m_Renderers[i].DisableMipmap();

                        m_EnableMipmap = false;
                    }
                }

                public void Destroy()
                {
                    DisableMipmap();
                }
            }

            private readonly LinkedList<GameObjectContext> m_ObjectList;
            private bool m_EnableMipmapView;

            public Mipmap()
            {
                m_MipMapViewTexture = new Texture2D(MIPMAP_TEXTURE_SIZE, MIPMAP_TEXTURE_SIZE, TextureFormat.RGBA32, true);
                for(int i = 0,icnt = m_MipMapViewTexture.mipmapCount;i<icnt;++i)
                {
                    Color[] pixels = m_MipMapViewTexture.GetPixels(i);
                    int layer = Utility.Math.Min(i, MipMapColors.Length - 1);
                    Color color = MipMapColors[layer];
                    for (int p = 0, pcnt = pixels.Length; p < pcnt; ++p)
                        pixels[p] = color;

                    m_MipMapViewTexture.SetPixels(pixels, i);
                }
                m_MipMapViewTexture.Apply(false);

                m_MipMapMaterial = new Material(AssetShaderLoader.Find("Debug/Particles/Mipmap"));
                m_MipMapMaterial.mainTexture = m_MipMapViewTexture;

                m_ObjectList = new LinkedList<GameObjectContext>();
                m_EnableMipmapView = false;
            }

            public void AddMipmapViewObject(GameObject target)
            {
                if (null != target)
                {
                    GameObjectContext newObject = _GetCurrentContext(target);
                    if (null == newObject)
                    {
                        newObject = new GameObjectContext(this, target);
                        m_ObjectList.AddLast(newObject);
                    }
                }
                else
                    Debugger.LogWarning("Parameter 'target' can not be null!");
            }

            public void RemoveMipmapViewObject(GameObject target)
            {
                if (null != target)
                    _RemoveContext(target);
                else
                    Debugger.LogWarning("Parameter 'target' can not be null!");
            }

            public void EnableMipmapView()
            {
                if(!m_EnableMipmapView)
                {
                    m_EnableMipmapView = true;
                    _SetMipampViewState(m_EnableMipmapView);
                }
            }

            public void DisableMipmapView()
            {
                if (m_EnableMipmapView)
                {
                    m_EnableMipmapView = false;
                    _SetMipampViewState(m_EnableMipmapView);
                }
            }

            private GameObjectContext _GetCurrentContext(GameObject obj)
            {
                LinkedListNode<GameObjectContext> cur = m_ObjectList.First;
                int targetInstanceID = obj.GetInstanceID();
                while (null != cur)
                {
                    GameObjectContext curObj = cur.Value;
                    if (targetInstanceID == curObj.InstanceID)
                        return curObj;

                    cur = cur.Next;
                }

                return null;
            }

            private void _RemoveContext(GameObject obj)
            {
                LinkedListNode<GameObjectContext> cur = m_ObjectList.First;
                int targetInstanceID = obj.GetInstanceID();
                while (null != cur)
                {
                    GameObjectContext curObj = cur.Value;
                    if (targetInstanceID == curObj.InstanceID)
                    {
                        m_ObjectList.Remove(cur);
                        return;
                    }

                    cur = cur.Next;
                }
            }

            private void _SetMipampViewState(bool enable)
            {
                LinkedListNode<GameObjectContext> cur = m_ObjectList.First;
                while (null != cur)
                {
                    if (enable)
                        cur.Value.EnableMipmap();
                    else
                        cur.Value.DisableMipmap();

                    cur = cur.Next;
                }
            }
        }
    }
}