using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tenmove.Runtime.Unity
{
    public static class PostprocessUtilities
    {
        static Mesh m_FullscreenTriangle;
        public static Mesh FullscreenTriangle
        {
            get
            {
                if (m_FullscreenTriangle != null)
                    return m_FullscreenTriangle;

                m_FullscreenTriangle = new Mesh { name = "Fullscreen Triangle" };

                // Because we have to support older platforms (GLES2/3, DX9 etc) we can't do all of
                // this directly in the vertex shader using vertex ids :(
                m_FullscreenTriangle.SetVertices(new List<Vector3>
                {
                     new Vector3(0f, 0f, 0f),
                     new Vector3(0f,  2f, 0f),
                     new Vector3(2f, 0f, 0f)
            });
                m_FullscreenTriangle.SetIndices(new[] { 0, 1, 2 }, MeshTopology.Triangles, 0, false);

                List<Vector2> uvs = new List<Vector2>();
                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(0, 2));
                uvs.Add(new Vector2(2, 0));

                m_FullscreenTriangle.SetUVs(0, uvs);

                m_FullscreenTriangle.UploadMeshData(false);

                return m_FullscreenTriangle;
            }
        }

        public static void DestroyObject(Object _object)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                Object.DestroyImmediate(_object);
            else
#endif
                Object.Destroy(_object);
        }


        #region Blit
        public static void SetRenderTargetWithLoadStoreAction(this CommandBuffer cmd, RenderTargetIdentifier rt, RenderBufferLoadAction loadAction, RenderBufferStoreAction storeAction)
        {
#if UNITY_2018_2_OR_NEWER
            cmd.SetRenderTarget(rt, loadAction, storeAction);
#else
            cmd.SetRenderTarget(rt);
#endif
        }
        public static void SetRenderTargetWithLoadStoreAction(this CommandBuffer cmd,
            RenderTargetIdentifier color, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction,
            RenderTargetIdentifier depth, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction)
        {
#if UNITY_2018_2_OR_NEWER
            cmd.SetRenderTarget(color, colorLoadAction, colorStoreAction, depth, depthLoadAction, depthStoreAction);
#else
            cmd.SetRenderTarget(color, depth);
#endif
        }

        //不做任何处理的Blit
        //     public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, bool clear = false)
        //     {
        //         cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
        //         cmd.SetRenderTargetWithLoadStoreAction(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
        // 
        //         if (clear)
        //             cmd.ClearRenderTarget(true, true, Color.clear);
        // 
        //         cmd.DrawMesh(FullscreenTriangle, Matrix4x4.identity, copyMaterial, 0, 0);
        //     }

        public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination,
                                                    Material material, MaterialPropertyBlock propertyBlock, int pass, bool clear = false)
        {
            cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
            cmd.SetRenderTargetWithLoadStoreAction(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);

            if (clear)
                cmd.ClearRenderTarget(true, true, Color.clear);

            cmd.DrawMesh(FullscreenTriangle, Matrix4x4.identity, material, 0, pass, propertyBlock);
        }


        // 没有PropertyBlock
        public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination,
                                                Material material, int pass, bool clear = false)
        {
            cmd.SetGlobalTexture(ShaderIDs.MainTex, source);

            cmd.SetRenderTargetWithLoadStoreAction(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);

            if (clear)
                cmd.ClearRenderTarget(true, true, Color.clear);

            cmd.DrawMesh(FullscreenTriangle, Matrix4x4.identity, material, 0, pass);
        }

        public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination,
                                        Material material, int pass, Rect viewport, bool clear = false)
        {
            cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
            cmd.SetRenderTargetWithLoadStoreAction(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);

            // 在SetViewport之前Clear，不然如果Clear的不是RenderTexture的所有像素就会有问题
            if (clear)
                cmd.ClearRenderTarget(true, true, Color.clear);

            cmd.SetViewport(viewport);

            cmd.DrawMesh(FullscreenTriangle, Matrix4x4.identity, material, 0, pass);
        }

        public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination,
                                                    RenderTargetIdentifier depth, Material material, MaterialPropertyBlock propertyBlock, int pass,
                                                    bool clear = false)
        {
            cmd.SetGlobalTexture(ShaderIDs.MainTex, source);

            if (clear)
            {
                cmd.SetRenderTargetWithLoadStoreAction(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store,
                                                       depth, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
                cmd.ClearRenderTarget(true, true, Color.clear);
            }
            else
            {
                cmd.SetRenderTargetWithLoadStoreAction(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store,
                                                       depth, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
            }

            cmd.DrawMesh(FullscreenTriangle, Matrix4x4.identity, material, 0, pass, propertyBlock);
        }

        public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier[] destinations,
                                                    RenderTargetIdentifier depth, Material material, MaterialPropertyBlock propertyBlock, int pass,
                                                    bool clear = false)
        {
            cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
            cmd.SetRenderTarget(destinations, depth);

            if (clear)
                cmd.ClearRenderTarget(true, true, Color.clear);

            cmd.DrawMesh(FullscreenTriangle, Matrix4x4.identity, material, 0, pass, propertyBlock);
        }

        public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination,
                                                    RenderTargetIdentifier depth, Material material, int pass,
                                                    bool clearColor = false, bool clearDepth = false)
        {
            cmd.SetGlobalTexture(ShaderIDs.MainTex, source);

            RenderBufferLoadAction colorLoadAction;
            RenderBufferLoadAction depthLoadAction;

            if(clearColor)
                colorLoadAction = RenderBufferLoadAction.DontCare;
            else
                colorLoadAction = RenderBufferLoadAction.Load;
            if (clearDepth)
                depthLoadAction = RenderBufferLoadAction.DontCare;
            else
                depthLoadAction = RenderBufferLoadAction.Load;

            cmd.SetRenderTargetWithLoadStoreAction(destination, colorLoadAction, RenderBufferStoreAction.Store,
                                       depth, depthLoadAction, RenderBufferStoreAction.Store);

            if (clearColor || clearDepth)
                cmd.ClearRenderTarget(clearDepth, clearColor, Color.clear);

            cmd.DrawMesh(FullscreenTriangle, Matrix4x4.identity, material, 0, pass);
        }

        public static void BlitFullscreenTriangle(Texture source, RenderTexture destination, Material material, int pass)
        {
            var oldRt = RenderTexture.active;

            material.SetPass(pass);
            if (source != null)
                material.SetTexture(ShaderIDs.MainTex, source);

            if (destination != null)
                destination.DiscardContents(true, false);

            UnityEngine.Graphics.SetRenderTarget(destination);
            UnityEngine.Graphics.DrawMeshNow(FullscreenTriangle, Matrix4x4.identity);
            RenderTexture.active = oldRt;
        }

        public static void BuiltinBlit(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier dest)
        {
#if UNITY_2018_2_OR_NEWER
            cmd.SetRenderTarget(dest, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            dest = BuiltinRenderTextureType.CurrentActive;
#endif
            cmd.Blit(source, dest);
        }

        public static void BuiltinBlit(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier dest, Material mat, int pass = 0)
        {
#if UNITY_2018_2_OR_NEWER
            cmd.SetRenderTarget(dest, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
            dest = BuiltinRenderTextureType.CurrentActive;
#endif
            cmd.Blit(source, dest, mat, pass);
        }

        // Fast basic copy texture if available, falls back to blit copy if not
        // Assumes that both textures have the exact same type and format
        //拷贝Texture
        //     public static void CopyTexture(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination)
        //     {
        //         if (SystemInfo.copyTextureSupport > CopyTextureSupport.None)
        //         {
        //             cmd.CopyTexture(source, destination);
        //             return;
        //         }
        // 
        //         cmd.BlitFullscreenTriangle(source, destination);
        //     }
        #endregion

        #region Material
        
        public static Material GetMaterial(string shaderName)
        {
            Shader shader = AssetShaderLoader.Find(shaderName);
            if(shader != null)
            {
                return new Material(shader);
            }
            else
            {
                Debugger.LogError("Shader not found: {0}", shaderName);
                return null;
            }
        }

        #endregion
    }
}

