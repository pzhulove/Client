using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Tenmove.Runtime.Unity;

public class PostprocessDebugNaN : MonoBehaviour
{
    Camera m_Camera;
    RenderTexture m_RT;
    CommandBuffer m_Cmd;
    Material m_CopyMaterial;

    void Start()
    {
        m_Camera = GetComponent<Camera>();

        m_RT = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGBHalf) { name = "DebugNaNRT" };
        m_Camera.SetTargetBuffers(m_RT.colorBuffer, m_RT.depthBuffer);

        Shader shader = AssetShaderLoader.Find("Hidden/PostProcessing/CopyStd");
        if (shader != null)
        {
            m_CopyMaterial = new Material(shader);
        }

        m_Cmd = new CommandBuffer() { name = "DebugNaNCmd" };
        m_Camera.AddCommandBuffer(CameraEvent.AfterEverything, m_Cmd);

        m_CopyMaterial.SetTexture("_MainTex", m_RT);
        m_Cmd.BlitFullscreenTriangle(m_RT, BuiltinRenderTextureType.CameraTarget, m_CopyMaterial, 1);
    }
}
