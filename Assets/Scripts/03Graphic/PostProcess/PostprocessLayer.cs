using Tenmove.Runtime.Client;
using UnityEngine;
using UnityEngine.Rendering;
using GameClient;

namespace Tenmove.Runtime.Unity
{
    /// <summary>
    /// 所有受后处理影响的物体都在PostprocessCamera和PostprocessCamera上的PostprocessCmd执行渲染，
    /// PostprocessCmd最后一步会把结果渲染到屏幕上，MainCamera的ClearFlag设置为Nothing，这两步中没有切换Render Buffer，不需要Clear来优化GPU的Load指令
    /// 不受后处理影响的物体又不需要排序就可以在MainCamera渲染，通过CullingMask来设置
    /// </summary>
    //[ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class PostprocessLayer : MonoBehaviour, ITMNativePostprocess
    {
        public delegate void OnUpdate(float deltaTime);
        public delegate void OnRenderTextureSizeChanged(int rtWidth, int rtHeight);
        public delegate void OnOrthographicSizeChanged(float size);
        public OnUpdate UpdateCallback;
        public OnRenderTextureSizeChanged RenderTextureSizeChangedCallback;
        public OnOrthographicSizeChanged OrthographicSizeChangedCallback;


        /// 最终的输出目标，为空时表示直接输出到屏幕
        private RenderTexture m_Target;

        private RenderTexture m_RT1;
        private RenderTexture m_RT2;
        private RenderBuffer[] m_ColorBuffers = new RenderBuffer[2];

        private Vector2Int m_TargetResolution;

        private Camera m_MainCamera;
        private CameraClearFlags m_MainCameraClearFlags;
        private int m_MainCameraCullingMask;
        private CommandBuffer m_ClearCmd;
        private bool m_ClearCmdEnable;

        private Camera m_PostprocessCamera;
        private bool m_GeCameraEnabled = true;
        private bool m_AnyRendering;
        private PostprocessBundle[] m_Bundles = new PostprocessBundle[(int)PostProcessType.PostprocessCount];

        // 可以修改这些设置来控制这个Camera默认启用的后处理
        [SerializeField] private bool m_BloomSwitch;
        [SerializeField] private bool m_RadialBlurSwitch;
        [SerializeField] private bool m_ColrSuiteSwitch;
        [SerializeField] private bool m_DistortionSwitch;
        [SerializeField] private PostprocessEffectSettings m_BloomSettings;
        [SerializeField] private PostprocessEffectSettings m_RadialBlurSettings;
        [SerializeField] private PostprocessEffectSettings m_ColorSuiteSettings;

        Tenmove.Runtime.AssetLoadCallbacks<object> m_AssetLoadCallbacks = 
            new Tenmove.Runtime.AssetLoadCallbacks<object>(_OnLoadAssetSuccess, _OnLoadAssetUpdate);


        #region Context
        public CommandBuffer PostprocessCmd { get; private set; }
        public Camera PostprocessCamera { get { return m_PostprocessCamera; } }
        public RenderTargetIdentifier SourceRT { get; private set; }
        public RenderTargetIdentifier TargetRT { get; private set; }
        public int RenderTextureWidth { get; private set; }
        public int RenderTextureHeight { get; private set; }
        public Material CopyMaterial { private set; get; }
        public Material FinalPassMaterial { private set; get; }
        public int BloomBufferNameID { set; get; }
        public int VolumetricLightNameID { set; get; }
        #endregion

        private bool m_Inited;

        private void OnDestroy()
        {
            DeInit();
        }

        public void Init()
        {
            if (m_Inited)
                return;
            m_Inited = true;

            m_MainCamera = GetComponent<Camera>();
            m_MainCameraClearFlags = m_MainCamera.clearFlags;
            m_MainCameraCullingMask = m_MainCamera.cullingMask;
            // ClearCmd 替代 Clear Camera
            m_ClearCmd = new CommandBuffer() { name = "ClearCmd" };
            _UpdateClearCmd();
            _SetClearCmdEnable(true);


            for (int i = 0; i < (int)PostProcessType.PostprocessCount; i++)
            {
                m_Bundles[i] = new PostprocessBundle(_CreateRenderer((PostProcessType)i));
            }

            // 游戏中才有默认参数
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
            {
                // 默认设置
                _GetBundle(PostProcessType.Bloom).SetDefaultSetting(m_BloomSwitch, m_BloomSettings);
                _GetBundle(PostProcessType.RadialBlur).SetDefaultSetting(m_RadialBlurSwitch, m_RadialBlurSettings);
                _GetBundle(PostProcessType.ColrSuite).SetDefaultSetting(m_ColrSuiteSwitch, m_ColorSuiteSettings);
                _GetBundle(PostProcessType.Distortion).SetDefaultSetting(m_DistortionSwitch);
            }

            //#if UNITY_EDITOR
            //            if (!Application.isPlaying)
            //            {
            //                if (UnityEditor.SceneView.lastActiveSceneView == null)
            //                {
            //                    Debug.Log("选择Scene视图");
            //                    return;
            //                }
            //                m_MainCamera = UnityEditor.SceneView.lastActiveSceneView.camera;
            //            }
            //            if (Application.isPlaying)
            //#endif
            {
                PostprocessManager.RegisterLayer(m_MainCamera, this);
            }

            m_TargetResolution = _GetTargetResolution();
        }

        public void DeInit()
        {
            if (!m_Inited)
                return;
            m_Inited = false;

            _ReleasePipeline();
            foreach (var bundle in m_Bundles)
            {
                bundle.Release();
            }

            //#if UNITY_EDITOR
            //            if (Application.isPlaying)
            //#endif
            PostprocessManager.UnRegisterLayer(m_MainCamera);
        }

        /// <summary>
        /// 创建后处理管线，包括Camera、RenderTexture和CommandBuffer,设置Camera状态和Clear Cmd
        /// </summary>
        private void _CreatePipeline(bool needRT2, bool updateRTSize, bool anyRendering)
        {
            int rtWidth, rtHeight;
            _CaculateRenderTextureSize(out rtWidth, out rtHeight);

            // RenderTexture
            RenderTextureWidth = rtWidth;
            RenderTextureHeight = rtHeight;

            //Camera设置
            if (m_PostprocessCamera == null)
            {
                GameObject postprocessGO = new GameObject("PostprocessCamera");
                postprocessGO.transform.SetParent(m_MainCamera.transform, false);
                m_PostprocessCamera = postprocessGO.AddComponent<Camera>();
                m_PostprocessCamera.CopyFrom(m_MainCamera);
                m_PostprocessCamera.rect = new Rect(0, 0, 1, 1);
                m_PostprocessCamera.depth = m_MainCamera.depth - 0.1f;
                m_PostprocessCamera.clearFlags = CameraClearFlags.SolidColor;
            }

            var bloom = _GetBundle(PostProcessType.Bloom);
            RenderTextureFormat rtFormat = RenderTextureFormat.Default;
            if (bloom.CanInit())
            {
                rtFormat = RenderTextureFormat.ARGBHalf;
            }

            if (m_RT1 == null)
            {
                m_RT1 = new RenderTexture(rtWidth, rtHeight, 24, rtFormat);
                m_RT1.name = "PostprocessRT1";
            }
            else
            {
                bool sizeChange = updateRTSize && (m_RT1.width != rtWidth || m_RT1.height != rtHeight);
                bool formatChange = m_RT1.format != rtFormat;

                if (sizeChange || formatChange)
                {
                    // 删除RenderTexutre时不能被Camera使用： Releasing a render texture which is being used as a camera target 
                    // (via Camera.SetTargetBuffers) for camera with name 'PostprocessCamera'.
                    m_PostprocessCamera.targetTexture = null;
                    PostprocessUtilities.DestroyObject(m_RT1);
                    m_RT1 = new RenderTexture(rtWidth, rtHeight, 24, rtFormat) { name = "PostprocessRT1" };

                    //RenderTexture大小改变~~~~~~~~
                    if(sizeChange)
                        RenderTextureSizeChangedCallback?.Invoke(rtWidth, rtHeight);
                }
            }

            if (needRT2)
            {
                if (m_RT2 == null)
                {
                    m_RT2 = new RenderTexture(rtWidth, rtHeight, 0, rtFormat) { name = "PostprocessRT2" };
                }
                else
                {
                    bool sizeChange = updateRTSize && (m_RT2.width != rtWidth || m_RT2.height != rtHeight);
                    bool formatChange = m_RT2.format != rtFormat;

                    if (sizeChange || formatChange)
                    {
                        m_PostprocessCamera.targetTexture = null;
                        PostprocessUtilities.DestroyObject(m_RT2);
                        m_RT2 = new RenderTexture(rtWidth, rtHeight, 0, rtFormat) { name = "PostprocessRT2" };
                    }
                }
            }
            else
            {
                if (m_RT2 != null)
                {
                    PostprocessUtilities.DestroyObject(m_RT2);
                    m_RT2 = null;
                }
            }

            //创建CommandBuffer
            if (PostprocessCmd == null)
            {
                PostprocessCmd = new CommandBuffer() { name = "PostprocessCmd" };
                m_PostprocessCamera.AddCommandBuffer(CameraEvent.AfterEverything, PostprocessCmd);
            }

            //创建通用材质
            if (CopyMaterial == null)
                CopyMaterial = PostprocessUtilities.GetMaterial("Hidden/PostProcessing/CopyStd");
            if (FinalPassMaterial == null)
                FinalPassMaterial = PostprocessUtilities.GetMaterial("Postprocessing/Combine");

            // 设置渲染状态
            // 如果后处理在渲染：1，关闭ClearCmd 由后处理Clear整个屏幕 2，场景由PostprocessCamera渲染
            // 如果没有后处理在渲染：开启Clear Cmd，Diable PostprocessCamera，场景由MainCamera渲染
            if (anyRendering)
            {
                // 关闭Clear Cmd
                _SetClearCmdEnable(false);

                m_PostprocessCamera.enabled = m_GeCameraEnabled;  //true & m_GeCameraEnbaled
                m_PostprocessCamera.SetTargetBuffers(m_RT1.colorBuffer, m_RT1.depthBuffer);
                m_MainCamera.cullingMask = 0;
                m_MainCamera.clearFlags = CameraClearFlags.Nothing;
            }
            else
            {
                // 开启Clear Cmd
                _SetClearCmdEnable(true);

                m_PostprocessCamera.enabled = false;
                m_MainCamera.cullingMask = m_MainCameraCullingMask;
                m_MainCamera.clearFlags = m_MainCameraClearFlags;
            }
        }

        /// <summary>
        /// 后处理效果都没开启时，释放资源，重置Camera的渲染
        /// </summary>
        private void _ReleasePipeline()
        {
            // 开启Clear Cmd
            _SetClearCmdEnable(true);

            //CommandBuffer
            if (PostprocessCmd != null)
            {
                PostprocessCmd.Clear();
                if (m_PostprocessCamera != null)
                    m_PostprocessCamera.RemoveCommandBuffer(CameraEvent.AfterEverything, PostprocessCmd);
                PostprocessCmd.Dispose();
                PostprocessCmd = null;
            }

            //Camera
            m_MainCamera.cullingMask = m_MainCameraCullingMask;
            m_MainCamera.clearFlags = m_MainCameraClearFlags;
            if (m_PostprocessCamera != null)
            {
                PostprocessUtilities.DestroyObject(m_PostprocessCamera.gameObject);
                m_PostprocessCamera = null;
            }

            //RenderTexture
            if (m_RT1 != null)
            {
                PostprocessUtilities.DestroyObject(m_RT1);
                m_RT1 = null;
            }
            if (m_RT2 != null)
            {
                PostprocessUtilities.DestroyObject(m_RT2);
                m_RT2 = null;
            }

            //Material
            if (CopyMaterial != null)
            {
                PostprocessUtilities.DestroyObject(CopyMaterial);
                CopyMaterial = null;
            }
            if (FinalPassMaterial != null)
            {
                PostprocessUtilities.DestroyObject(FinalPassMaterial);
                FinalPassMaterial = null;
            }
        }

        /// <summary>
        /// 后处理RT：如果需要xBR，大小就根据Size和宽高比计算，如果不需要就用计算出的和屏幕大小中小的那一个，如果用了xBR，RT1的FilterMode就设置为Point
        /// </summary>
        private void _CaculateRenderTextureSize(out int width, out int height)
        {
            int screenWidth = m_TargetResolution.x;
            int screenHeight = m_TargetResolution.y;
            //             #if UNITY_EDITOR
            //                 screenWidth = (int)UnityEditor.Handles.GetMainGameViewSize().x;
            //                 screenHeight = (int)UnityEditor.Handles.GetMainGameViewSize().y;
            //             #else
            //#endif

            if(screenWidth == 0 || screenHeight == 0)
            {
                screenWidth = Screen.width;
                screenHeight = Screen.height;
            }

            width = screenWidth;
            height = screenHeight;
        }

        /// <summary>
        /// 更新CommandBuffer时重置参数
        /// </summary>
        private void _Reset()
        {
            // DisableKeyword，在每个PostprocessEffect的Render函数中会EnableKeyword，xBR没改
            Shader.DisableKeyword("BLOOM");
            Shader.DisableKeyword("_POST_DISTORTION");
            Shader.DisableKeyword("_LightEffect");


            if (FinalPassMaterial != null)
                FinalPassMaterial.shaderKeywords = null;

            BloomBufferNameID = -1;
            VolumetricLightNameID = -1;

            if (PostprocessCmd != null)
                PostprocessCmd.Clear();
        }

        private void _OnOrthographicSizeChanged(float orthographicSize, bool resizeRenderTexture)
        {
            if (orthographicSize > 0 && orthographicSize < 30)
            {
                if (m_PostprocessCamera != null)
                    m_PostprocessCamera.orthographicSize = orthographicSize;

                if (resizeRenderTexture)
                {
                    UpdateCommandBuffer();
                }

                if (OrthographicSizeChangedCallback != null)
                    OrthographicSizeChangedCallback(orthographicSize);
            }
        }

        private Vector2Int _GetTargetResolution()
        {
            if (m_Target != null)
            {
                return new Vector2Int(m_Target.width, m_Target.height);
            }

            GeCamera geCamera = GameFrameWork.instance.MainCamera;
            if (geCamera != null)
            {
                return new Vector2Int(geCamera.pixelWidth, geCamera.pixelHeight);
            }

            return new Vector2Int(Screen.width, Screen.height);
        }

        private void _SetClearCmdEnable(bool enable)
        {
            if(m_ClearCmdEnable != enable)
            {
                m_ClearCmdEnable = enable;

                if(m_ClearCmdEnable)
                    m_MainCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_ClearCmd);
                else
                    m_MainCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, m_ClearCmd);
            }

        }

        private void _UpdateClearCmd()
        {
            m_ClearCmd.Clear();
            m_ClearCmd.SetViewport(new Rect(0, 0, Screen.width, Screen.height));
            m_ClearCmd.ClearRenderTarget(true, true, Color.black);
            m_ClearCmd.SetViewport(m_MainCamera.pixelRect);
        }

        private void LateUpdate()
        {
            //解决Scene窗口的Camera的Viewport会错的bug，旧版本没有
            //#if UNITY_EDITOR
            //            if(!Application.isPlaying)
            //            {
            //                if (UnityEditor.SceneView.lastActiveSceneView != null)
            //                {
            //                    UnityEditor.SceneView.lastActiveSceneView.camera.rect = new Rect(0,0,1,1);
            //                }
            //                UpdateCommandBuffer();
            //            }
            //#endif

            if (!m_Inited)
                return;

            if (UpdateCallback != null)
                UpdateCallback(Time.deltaTime);

#if UNITY_EDITOR
            if (!Application.isPlaying && PostprocessCamera != null)
                PostprocessCamera.orthographicSize = m_MainCamera.orthographicSize;
#endif

            if(m_MainCamera != null && PostprocessCamera != null)
            {
                if(m_PostprocessCamera.orthographicSize != m_MainCamera.orthographicSize)
                {
                    _OnOrthographicSizeChanged(m_MainCamera.orthographicSize, true);
                }
            }


            Vector2Int currResolution = _GetTargetResolution();
            if (m_TargetResolution != currResolution)
            {
                m_TargetResolution = currResolution;

                _UpdateClearCmd();
                UpdateCommandBuffer();
            }
        }

        public void OverrideSettings(PostprocessProfile _profile)
        {
            foreach (var bundle in m_Bundles)
            {
                bundle.SetActive(false);
                bundle.SetOverrideSettings(null);
            }

            if (_profile != null)
            {
                foreach (var setting in _profile.Effects)
                {
                    var bundle = _GetBundle(setting.EffectType);
                    bundle.SetActive(true);
                    bundle.SetOverrideSettings(setting);
                }
            }

            UpdateCommandBuffer();
        }

        /// <summary>
        /// 设置新的效果集合后更新CommandBuffer
        /// </summary>
        /// <param name="updateRTSize"> 是否更新RT大小，径向模糊时Camera Size改变，不更新RT大小</param>
        public void UpdateCommandBuffer(bool updateRTSize = true)
        {
            UnityEngine.Profiling.Profiler.BeginSample("BuildCommandBuffer");


            _Reset();


            bool anyEnabled = false;
            bool anyRendering = false;
            bool needFinalPass = false;       //是否需要FinalPass
            int blitToLayerTimes = 0;
            int lastNeedBlitIndex = 0;        //最后一个需要Blit的Index
            for (int i = 0; i < m_Bundles.Length; i++)
            {
                var bundle = m_Bundles[i];
                if (bundle.CanInit())
                {
                    PostprocessEffectRenderer renderer = bundle.GetRenderer();
                    anyEnabled = true;
                    if (bundle.CanRendering())
                    {
                        anyRendering = true;
                        if (renderer.NeedFinalPass)
                        {
                            needFinalPass = true;
                        }
                        blitToLayerTimes += renderer.BlitToLayerTimes;
                        if (renderer.BlitToLayerTimes > 0)
                        {
                            lastNeedBlitIndex = i;
                        }
                    }
                }
            }
            m_AnyRendering = anyRendering;

            if (anyEnabled)
            {
                bool needRT2 = false;
                if (blitToLayerTimes == 1)
                {
                    //Blit一次，需要FinalPass，需要使用RT2
                    if (needFinalPass)
                    {
                        needRT2 = true;
                    }
                    //Blit一次，不能Blit到屏幕，需要使用RT2
                    if (!m_Bundles[lastNeedBlitIndex].GetRenderer().CanBlitToScreen)
                    {
                        needRT2 = true;
                    }
                }
                //Blit多次，需要使用RT2
                else if (blitToLayerTimes > 1)
                {
                    needRT2 = true;
                }

                _CreatePipeline(needRT2, updateRTSize, anyRendering);

                if (anyRendering)
                {
                    SourceRT = m_RT1;
                    TargetRT = m_RT2;

                    //不需要FinalPass，最后一次后处理的Blit可以直接Blit到屏幕，节省一次Blit
                    if (!needFinalPass && m_Bundles[lastNeedBlitIndex].GetRenderer().CanBlitToScreen)
                    {
                        for (int i = 0; i < m_Bundles.Length; i++)
                        {
                            var bundle = m_Bundles[i];
                            if (bundle.CanInit())
                            {
                                bundle.Init(this);

                                if (bundle.CanRendering())
                                {
                                    if (i == lastNeedBlitIndex)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        bundle.GetRenderer().OnPreRender();
                                        bundle.Render(PostprocessCmd);
                                        if (bundle.GetRenderer().NeedSwapRT)
                                        {
                                            var tempRT = SourceRT;
                                            SourceRT = TargetRT;
                                            TargetRT = tempRT;
                                        }
                                    }
                                }
                                else
                                {
                                    bundle.GetRenderer().OnPreNotRender();
                                }
                            }
                            else
                            {
                                bundle.Release();
                            }
                        }

                        if (m_Target == null)
                        {
                            TargetRT = BuiltinRenderTextureType.CameraTarget;
                        }
                        else
                        {
                            TargetRT = m_Target;
                        }
                        m_Bundles[lastNeedBlitIndex].Render(PostprocessCmd, true);
                    }
                    else
                    {
                        for (int i = 0; i < m_Bundles.Length; i++)
                        {
                            var bundle = m_Bundles[i];
                            if (bundle.CanInit())
                            {
                                bundle.Init(this);

                                if (bundle.CanRendering())
                                {
                                    bundle.GetRenderer().OnPreRender();
                                    bundle.Render(PostprocessCmd);
                                    //Swap
                                    if (bundle.GetRenderer().NeedSwapRT)
                                    {
                                        var tempRT = SourceRT;
                                        SourceRT = TargetRT;
                                        TargetRT = tempRT;
                                    }
                                }
                                else
                                {
                                    bundle.GetRenderer().OnPreNotRender();
                                }
                            }
                            else
                            {
                                bundle.Release();
                            }
                        }


                        if (m_Target == null)
                        {
                            PostprocessCmd.BlitFullscreenTriangle(SourceRT, BuiltinRenderTextureType.CameraTarget, FinalPassMaterial, 0, m_MainCamera.pixelRect, true);
                        }
                        else
                        {
                            PostprocessCmd.BlitFullscreenTriangle(SourceRT, m_Target, FinalPassMaterial, 0, true);
                        }
                    }
                    // 释放临时RT
                    if (BloomBufferNameID > -1) PostprocessCmd.ReleaseTemporaryRT(BloomBufferNameID);
                    if (VolumetricLightNameID > -1) PostprocessCmd.ReleaseTemporaryRT(VolumetricLightNameID);
                }
            }
            else
            {
                _ReleasePipeline();
                foreach (var bundle in m_Bundles)
                {
                    bundle.Release();
                }
            }

            UnityEngine.Profiling.Profiler.EndSample();
        }

        public void SetPostprocessSetting(string settingPath)
        {
            AssetLoader.LoadResAsync(settingPath, typeof(PostprocessEffectSettings), m_AssetLoadCallbacks, this);
        }

        static void _OnLoadAssetSuccess(string assetPath, object asset, int grpID, float duration, object userData)
        {
            PostprocessLayer layer = userData as PostprocessLayer;
            if(layer == null)
            {
                Debugger.LogError("User data can not be null!");
                return;
            }

            PostprocessEffectSettings setting = asset as PostprocessEffectSettings;
            if(setting == null)
            {
                Debugger.LogError("Load asset failed: {0}", assetPath);
            }
            else
            {
                layer.ActiveEffect(setting.EffectType, setting);
                if (setting.EffectType == PostProcessType.RadialBlur)
                    layer.UpdateCommandBuffer(false);
                else
                    layer.UpdateCommandBuffer(true);
            }
        }

        static void _OnLoadAssetUpdate(string path, int taskGrpID, AssetLoadErrorCode errorCode, string message, object userData)
        {

        }

        public void SetPostProcess<TParam>(TParam param) where TParam : PostProcessParam
        {
            switch (param.PostEffectType)
            {
                case PostProcessType.RadialBlur:
                    RadialBlurParam radialBlurParam = param as RadialBlurParam;
                    if (radialBlurParam != null)
                    {
                        Vector2 center = m_MainCamera.WorldToViewportPoint(new Vector2(radialBlurParam.Center.fx, radialBlurParam.Center.fy));

                        if (center.x > 10 || center.y > 10)
                            return;

                        center = new Vector2(Mathf.Clamp01(center.x), Mathf.Clamp01(center.y));

                        if (radialBlurParam.Active)
                        {
                            RadialBlur radialBlur = _GetEffect(PostProcessType.RadialBlur) as RadialBlur;
                            if (radialBlur != null)
                            {
                                radialBlur.Center = center;
                                SetPostprocessSetting(radialBlurParam.ConfigPath);
                            }
                        }
                        else
                        {
                            DeactiveEffect(PostProcessType.RadialBlur);
                            UpdateCommandBuffer();
                        }
                    }
                    break;
                default:
                    break;
            }
        }


        public void OnQualityChanged(GraphicLevel postprocessQuality)
        {
            UpdateCommandBuffer();
        }

        public void ActiveEffect(PostProcessType effectType, PostprocessEffectSettings setting = null)
        {
            var bundle = _GetBundle(effectType);
            bundle.SetActive(true);
            bundle.SetOverrideSettings(setting);
        }

        public void DeactiveEffect(PostProcessType effectType)
        {
            var bundle = _GetBundle(effectType);
            bundle.SetActive(false);
        }

        public void SetExternalSwitch(PostProcessType effectType, bool _switch)
        {
            var bundle = _GetBundle(effectType);
            bundle.SetExternalSwitch(_switch);
        }

        public PostprocessEffectRenderer _GetEffect(PostProcessType effectType)
        {
            return _GetBundle(effectType).GetRenderer();
        }

        public PostprocessBundle _GetBundle(PostProcessType effectType)
        {
            return m_Bundles[(int)effectType];
        }

        // TODO:每一帧更新一次
        public void IncreaseBloomRef()
        {
            var bloom = _GetBundle(PostProcessType.Bloom);
            if (bloom.CanInit())
            {
                var bloomRenderer = bloom.CastRenderer<BloomRenderer>();
                if (bloomRenderer != null)
                {
                    bool oldState = bloom.CanRendering();
                    bloomRenderer.IncreaseRef();
                    if (bloom.CanRendering() != oldState)
                    {
                        UpdateCommandBuffer();
                    }
                }
            }
        }

        public void DecreaseBloomRef()
        {
            var bloom = _GetBundle(PostProcessType.Bloom);
            if (bloom.CanInit())
            {
                var bloomRenderer = bloom.CastRenderer<BloomRenderer>();
                if (bloomRenderer != null)
                {
                    bool oldState = bloom.CanRendering();
                    bloomRenderer.DecreaseRef();
                    if (bloom.CanRendering() != oldState)
                    {
                        UpdateCommandBuffer();
                    }
                }
            }
        }

        public void IncreaseDistortionRef()
        {
            var distortion = _GetBundle(PostProcessType.Distortion);
            if (distortion.CanInit())
            {
                var distortionRenderer = distortion.CastRenderer<Distortion>();
                if (distortionRenderer != null)
                {
                    bool oldState = distortion.CanRendering();
                    distortionRenderer.IncreaseRef();
                    if (distortion.CanRendering() != oldState)
                    {
                        UpdateCommandBuffer();
                    }
                }
            }
        }

        public void DecreaseDistortionRef()
        {
            var distortion = _GetBundle(PostProcessType.Distortion);
            if (distortion.CanInit())
            {
                var distortionRenderer = distortion.CastRenderer<Distortion>();
                if (distortionRenderer != null)
                {
                    bool oldState = distortion.CanRendering();
                    distortionRenderer.DecreaseRef();
                    if (distortion.CanRendering() != oldState)
                    {
                        UpdateCommandBuffer();
                    }
                }
            }
        }

        private PostprocessEffectRenderer _CreateRenderer(PostProcessType effectType)
        {
            switch (effectType)
            {
                case PostProcessType.Bloom:
                    return new BloomRenderer();
                case PostProcessType.ColrSuite:
                    return new ColorSuiteRenderer();
                case PostProcessType.RadialBlur:
                    return new RadialBlur();
                case PostProcessType.Distortion:
                    return new Distortion();
                default:
                    return null;
            }
        }

        public void SetRenderTarget(object renderTarget)
        {
            m_Target = (RenderTexture)renderTarget;

            m_TargetResolution = _GetTargetResolution();

            _UpdateClearCmd();
            UpdateCommandBuffer();
        }

        public void SetCameraEnabled(bool enable)
        {
            m_GeCameraEnabled = enable;

            if(m_PostprocessCamera != null)
                m_PostprocessCamera.enabled = enable;
        }
    }
}