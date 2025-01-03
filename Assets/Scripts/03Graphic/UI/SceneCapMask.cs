using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Tenmove.Runtime.Unity;
using Tenmove.Runtime;
using GameClient;

public class SceneCapMask : MonoBehaviour
{
    private static GameObject sm_SceneCapMask;
    private static uint sm_AsyncRequest = AssetLoader.INVILID_HANDLE;
    private static int sm_RefCount = 0;

    public RawImage m_ImageComponent;

    private CommandBuffer m_Cmd;
    private RenderTexture m_RT;
    private Material m_BlitMaterial;
    private bool m_CameraEnabledBeforeCap;

    static AssetLoadCallbacks<object> m_AssetLoadCallbacks = 
        new Tenmove.Runtime.AssetLoadCallbacks<object>(OnAssetLoadSuccess, OnAssetLoadFailure);

    public static void Preload()
    {
        if (sm_SceneCapMask == null && sm_AsyncRequest == AssetLoader.INVILID_HANDLE)
        {
            sm_AsyncRequest = AssetLoader.LoadResAsync("UIFlatten/Prefabs/Common/SceneCapMask", typeof(GameObject), m_AssetLoadCallbacks, null);
        }
    }

    /// <summary>
    /// 对场景截图并显示截图
    /// </summary>
    /// <param name="windowTransform"></param>
    public static void Enable()
    {
        sm_RefCount++;


        if ((sm_SceneCapMask == null || sm_SceneCapMask.IsNull()) && sm_AsyncRequest == AssetLoader.INVILID_HANDLE)
        {
            sm_SceneCapMask = null;
            sm_AsyncRequest = AssetLoader.LoadResAsync("UIFlatten/Prefabs/Common/SceneCapMask", typeof(GameObject), m_AssetLoadCallbacks, null);
        }
        else
        {
            if(sm_SceneCapMask != null)
            {
                var script = sm_SceneCapMask.GetComponent<SceneCapMask>();
                if(script != null) 
                {
                    script?._CaptureScene();
                }
            }
        }
    }

    /// <summary>
    /// 隐藏，释放资源
    /// </summary>
    public static void Disable()
    {
        sm_RefCount--;
        // 引用计数小于0关闭截图、释放资源
        if(sm_RefCount <= 0)
        {
            sm_RefCount = 0;

            if (sm_SceneCapMask != null)
            {
                var script = sm_SceneCapMask.GetComponent<SceneCapMask>();
                if (script != null)
                {
                    script.m_ImageComponent.enabled = false;
                    script._ReleaseResource();
                }
            }
        }
    }

    static void OnAssetLoadSuccess(string path, object asset, int taskGrpID, float duration, object userData)
    {
        sm_AsyncRequest = AssetLoader.INVILID_HANDLE;
        GameObject go = asset as GameObject;
        if (go != null)
        {
            // todo: 修改显隐
            go.SetActive(true);
            GameObject backgroundLayer = ClientSystemManager.GetInstance().BackgroundLayer;
            if (backgroundLayer != null)
            {
                go.transform.SetParent(backgroundLayer.transform, false);
            }
            sm_SceneCapMask = go;
            var script = go.GetComponent<SceneCapMask>();
            if (script != null)
            {
                script._CaptureScene();
            }
        }
        else
        {
            Debugger.LogError("Load asset with path '{0}' has failed!", path);
        }
    }

    static void OnAssetLoadFailure(string path, int taskGrpID, AssetLoadErrorCode errorCode, string message, object userData)
    {
        sm_AsyncRequest = AssetLoader.INVILID_HANDLE;
        Debugger.LogError("Load asset with path '{0}' has failed!", path);
    }

    /// <summary>
    /// 防止Destroy时没有释放资源
    /// </summary>
    private void OnDestroy()
    {
        _ReleaseResource();
    }

    private void _CaptureScene()
    {
        // 引用计数为1才截图，也就是第一次请求截图时截图
        if (sm_RefCount != 1)
            return;

        GeCamera geCamera = GameFrameWork.instance.MainCamera;
        if(geCamera == null)
        {
            Debugger.LogError("MainCamera is null!");
            return;
        }


        if(m_BlitMaterial == null)
        {
            Shader copyShader = AssetShaderLoader.Find("Hidden/PostProcessing/CopyStd");
            m_BlitMaterial = new Material(copyShader);
        }

        if(m_RT == null)
        {
            m_RT = RenderTexture.GetTemporary(geCamera.pixelWidth / 4, geCamera.pixelHeight / 4, 24, RenderTextureFormat.Default);
            m_RT.name = "CaptureSceneRT";
            m_RT.filterMode = FilterMode.Bilinear;
        }

        m_CameraEnabledBeforeCap = geCamera.enabled;
        geCamera.enabled = true;
        geCamera.SetTargetBuffer(m_RT);

        // 如果截图时MainCamera开启，就把场景截图Blit到BackBuffer，否则不Blit
        if(m_CameraEnabledBeforeCap)
        {
            m_Cmd = new CommandBuffer() { name = "CaptureCmd" };
            // 优化,不使用forceIntoRenderTexture，这样会创建两个屏幕大小的RT
            // 优化后只创建一张RT，把MainCamera的Target设为这张RT，然后在Cmd中把RT Blit到backbuffer
            geCamera.AddCommandBuffer(CameraEvent.AfterEverything, m_Cmd);
            //chanm_Cmd.SetViewport(new Rect(0,0,Screen.width, Screen.height));
            m_Cmd.BlitFullscreenTriangle(m_RT, BuiltinRenderTextureType.CameraTarget, m_BlitMaterial, 0);
        }

        StartCoroutine(_FinishCapture());
    }

    private IEnumerator _FinishCapture()
    {
        yield return Yielders.EndOfFrame;

        GeCamera geCamera = GameFrameWork.instance.MainCamera;
        if (geCamera == null)
        {
            Debugger.LogError("MainCamera is null!");
            yield break;
        }

        m_ImageComponent.enabled = true;
        m_ImageComponent.texture = m_RT;
        geCamera.SetRenderTarget(null);
        geCamera.enabled = m_CameraEnabledBeforeCap;


        if (m_Cmd != null)
        {
            m_Cmd.Clear();
            geCamera.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, m_Cmd);
            m_Cmd = null;
        }
    }

    private void _ReleaseResource()
    {
        if(m_BlitMaterial != null)
            Object.Destroy(m_BlitMaterial);

        if (m_RT != null)
        {
            m_ImageComponent.texture = null;
            RenderTexture.ReleaseTemporary(m_RT);
            m_RT = null;
        }
    }
}
