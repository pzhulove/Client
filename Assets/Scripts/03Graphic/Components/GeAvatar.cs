using UnityEngine;
using System.Collections.Generic;

public enum GeAvatarChannel
{
    Head, /// 头部
    UpperPart, /// 上半身
    LowerPart, /// 下半身
    Bracelet, /// 手链
    Headwear, /// 头饰
    Weapon, /// 武器
    Wings, /// 翅膀

    WholeBody,
    MaxChannelNum,

    AvatarRoot,
    Simple
}

public delegate void OnAvatarLoaded(GeAvatar avatar);

public class GeAvatar
{
    public enum EAvatarRes
    {
        Action    = 0x01,
        Attach    = 0x02,

        All       = Action | Attach,
    }
    
    Tenmove.Runtime.AssetLoadCallbacks<object> m_AssetLoadCallbacks = new Tenmove.Runtime.AssetLoadCallbacks<object>(_OnLoadAssetSuccess, _OnLoadAssetFailure);

    static void _OnLoadAssetSuccess(string assetPath, object asset,int grpID, float duration, object userData)
    {
        if (null == userData)
        {
            Tenmove.Runtime.Debugger.LogError("User data can not be null!");
            return;
        }

        GeAvatar _this = userData as GeAvatar;
        if (null == _this)
        {
            Tenmove.Runtime.Debugger.LogError("User data type '{0}' is NOT GeAvatar!");
            return;
        }

        if(null == asset)
        {
            Tenmove.Runtime.Debugger.LogError("Asset '{0}' load error!", assetPath);
            return;
        }

        GameObject go = asset as GameObject;
        if (null == go)
        {
            Tenmove.Runtime.Debugger.LogError("Asset '{0}' is nil or type '{1}' error!", assetPath, asset.GetType());
            return;
        }
        if (AssetLoader.INVILID_HANDLE != _this.m_AvatarRootDesc.m_AsyncRequest)
        {
            if ((uint)grpID == _this.m_AvatarRootDesc.m_AsyncRequest)
            {
                --_this.m_LoadingCnt;
                //go.SetActive(false);

                _this._OnAvatarRootLoaded(go);
                _this.m_AvatarRootDesc.m_AsyncRequest = AssetLoader.INVILID_HANDLE;
                return;
            }
        }
        
        for (int i = 0, icnt = _this.m_AvatarDescList.Count; i < icnt; ++i)
        {
            GeAvatarDesc curAvatarDesc = _this.m_AvatarDescList[i];
            if (null == curAvatarDesc) continue;

            if (AssetLoader.INVILID_HANDLE == curAvatarDesc.m_AsyncRequest) continue;
            
            if ((uint)grpID == curAvatarDesc.m_AsyncRequest)
            {
                --_this.m_LoadingCnt;
                go.SetActive(false);
                curAvatarDesc.m_MeshObject = go;
                _this._OnAvatarPartLoaded(curAvatarDesc);
                curAvatarDesc.m_AsyncRequest = AssetLoader.INVILID_HANDLE;

                return;
            }
        }

        // 小白人
        if(AssetLoader.INVILID_HANDLE != _this.m_SimpleAvatarDesc.m_AsyncRequest)
        {
            if((uint)grpID == _this.m_SimpleAvatarDesc.m_AsyncRequest)
            {
                --_this.m_LoadingCnt;
                go.SetActive(false);
                _this.m_SimpleAvatarDesc.m_MeshObject = go;
                _this._OnAvatarPartLoaded(_this.m_SimpleAvatarDesc);
                _this.m_SimpleAvatarDesc.m_AsyncRequest = AssetLoader.INVILID_HANDLE;

                return;
            }
        }
        
        CGameObjectPool.RecycleGameObjectEx(go);
    }

    static void _OnLoadAssetFailure(string assetPath, int taskID, Tenmove.Runtime.AssetLoadErrorCode errorCode, string errorMessage, object userData)
    {
        //Logger.LogErrorFormat("[GeAvatar]Load game object '{0}' has failed![Error:{1}]", assetPath, errorMessage);
    }

    public GeAvatar(bool enableAvatarPartFallback = false)
    {
        _ResetAsyncCommads();
        m_AvatarDescList = new List<GeAvatarDesc>((int)GeAvatarChannel.MaxChannelNum);
        for (int i = 0; i < (int)GeAvatarChannel.MaxChannelNum; ++i)
        {
            m_AvatarDescList.Add(new GeAvatarDesc((GeAvatarChannel)i));
        }

        m_AvatarRootDesc = new GeAvatarDesc(GeAvatarChannel.AvatarRoot);
        m_SimpleAvatarDesc = new GeAvatarDesc(GeAvatarChannel.Simple);
        m_EnableAvatarPartFallback = enableAvatarPartFallback;
    }
    public void SetProfessionalId(int proId)
    {
        m_professionalId = proId;
    }

    public bool Init(string avatarRes, int layer, bool usePool = true, bool asyncLoad = false, bool highPriority = false)
    {
        _ResetAsyncCommads();
        m_AvatarBBox.extents = Vector3.zero;
        m_AvatarBBox.center = Vector3.zero;

        if (!string.IsNullOrEmpty(avatarRes))
        {
            if (null != m_AvatarRootDesc.m_MeshObject)
            {
                if (null != m_AvatarRootDesc.m_MeshRendererList)
                {
                    for (int i = 0, icnt = m_AvatarRootDesc.m_MeshRendererList.Length; i < icnt; ++i)
                    {
                        SkinnedMeshRenderer cur = m_AvatarRootDesc.m_MeshRendererList[i];
                        if (null == cur) continue;

                        cur.gameObject.layer = m_AvatarRootDesc.m_OriginLayer;
                    }
                }

                if (EngineConfig.useTMEngine)
                    CGameObjectPool.RecycleGameObjectEx(m_AvatarRootDesc.m_MeshObject);
                else
                    AssetManager.RecycleAsset(m_AvatarRootDesc.m_MeshObject);
                m_AvatarRootDesc.m_MeshObject = null;
            }

            m_bUsePool = usePool;
            m_AvatarRootDesc.m_Asset = new DAssetObject(null, avatarRes);
            m_RenderLayer = layer;

            m_IsAsyncLoad = asyncLoad;
            if (asyncLoad)
            {
                m_AvatarRootDesc.m_MeshObject = null;
                m_AvatarRootDesc.m_HasRemapSkeleton = false;
                m_AvatarRootDesc.m_MeshRendererList = null;
                m_AvatarRootDesc.m_SkeletonRoot = null;
                m_AvatarRootDesc.m_RendObjRoot = null;
                m_AvatarRootDesc.m_OriginLayer = 0;

                uint loadFlag = highPriority ? ((uint)GameObjectPoolFlag.HideAfterLoad | (uint)GameObjectPoolFlag.HighPriority) : (uint)GameObjectPoolFlag.HideAfterLoad;
                if (EngineConfig.useTMEngine)
                {
                    if (AssetLoader.INVILID_HANDLE != m_AvatarRootDesc.m_AsyncRequest)
                    {
                        //AssetLoader.AbortLoadRequest(m_AvatarRootDesc.m_AsyncRequest);
                        CGameObjectPool.AbortAcquireRequest(m_AvatarRootDesc.m_AsyncRequest);
                        --m_LoadingCnt;
                    }

                    //m_AvatarRootDesc.m_AsyncRequest = (uint)AssetLoader.LoadResAsGameObjectAsync(avatarRes, m_AssetLoadCallbacks, this, loadFlag, 0x3324cdcd);
                    try
                    {
                        m_AvatarRootDesc.m_AsyncRequest = (uint)CGameObjectPool.GetGameObjectAsync(avatarRes, m_AssetLoadCallbacks, this, loadFlag, 0x3324cdcd);
                    }
                    catch (System.Exception e)
                    {
                        Logger.LogErrorFormat("Init avatar Async res {0} is error reason {1}", avatarRes, e.Message);
                    }
                }
                else
                {
                    if (AssetLoader.INVILID_HANDLE != m_AvatarRootDesc.m_AsyncRequest)
                    {
                        --m_LoadingCnt;
                        AssetManager.AbortRequest(m_AvatarRootDesc.m_AsyncRequest);
                    }
                    m_AvatarRootDesc.m_AsyncRequest = AssetManager.LoadAssetRequest(avatarRes, typeof(GameObject), true, (uint)GameObjectPoolFlag.HideAfterLoad, false, 0x3324cdcd);
                }

                if (AssetLoader.INVILID_HANDLE != m_AvatarRootDesc.m_AsyncRequest)
                {
                    if (-1 == m_LoadingCnt)
                        m_LoadingCnt = 0;
                    ++m_LoadingCnt;
                    return true;
                }
            }
            else
            {
                if (EngineConfig.useTMEngine)
                {
                    try
                    {
                        m_AvatarRootDesc.m_MeshObject = CGameObjectPool.GetGameObject(avatarRes, 0) as GameObject;
                    }
                    catch (System.Exception e)
                    {
                        Logger.LogErrorFormat("Init avatar MeshObject res {0} is error reason {1}", avatarRes, e.Message);
                    }
                }
                else
                    m_AvatarRootDesc.m_MeshObject = AssetManager.LoadAsset(avatarRes, typeof(GameObject), true, (uint)GameObjectPoolFlag.None) as GameObject;
                if (null != m_AvatarRootDesc.m_MeshObject)
                {
                    _OnAvatarRootLoaded(m_AvatarRootDesc.m_MeshObject);
                    return true;

                    // /// 后面需要预处理
                    // SkinnedMeshRenderer avatarmr = m_AvatarRootDesc.m_MeshObject.GetComponentInChildren<SkinnedMeshRenderer>();
                    // if (null != avatarmr)
                    // {
                    //     m_AvatarRootMesh = avatarmr.gameObject;
                    //     m_OriginLayer = m_AvatarRootMesh.layer;
                    // }
                    // 
                    // m_AvatarRootDesc.m_MeshObject.layer = layer;
                    // 
                    // /// 重新初始化角色本事的挂点列表
                    // m_SkeletonRoot = _FindSkeletonRoot(m_AvatarRootDesc.m_MeshObject);
                    // if (null != m_SkeletonRoot)
                    // {
                    //     m_RenderLayer = layer;
                    // 
                    //     m_AvatarSkeleton = m_AvatarRootDesc.m_MeshObject.transform.GetChild(0).GetComponentsInChildren<Transform>();
                    //     return true;
                    // }
                    // else
                    //     Logger.LogWarningFormat("Root bone must be named with \"BoneAll\" res:{0}!", avatarRes);
                    // 
                    // AssetManager.RecycleAsset(m_AvatarRootDesc.m_MeshObject);
                }
                else
                    Logger.LogWarningFormat("Create avatar node has failed! Res:{0}", avatarRes);

            }

        }

        m_AvatarRootDesc.m_MeshObject = new GameObject("AvatarRoot");
        if (null != m_AvatarRootDesc.m_MeshObject)
        {
            m_SkeletonRoot = m_AvatarRootDesc.m_MeshObject;
            m_RenderLayer = layer;
            return true;
        }
        return false;
    }

    public void Deinit()
    {
        ClearAll();

        if (null != m_Animation)
        {
            m_Animation.Deinit();
            m_Animation = null;
        }

        if (null != m_Attachment)
        {
            m_Attachment.Deinit();
            m_Attachment = null;
        }
    }

    public bool IsLoadFinished()
    {
        return false == m_IsAsyncLoad || 0 == m_LoadingCnt;
    }

    public GameObject GetSkeletonRoot()
    {
        return m_SkeletonRoot;
    }

    public GameObject GetAvatarRoot()
    {
        return m_AvatarRootDesc.m_MeshObject;
    }
    public void DoBackToFront()
    {
        m_Attachment.DoBackToFront();
    }
    public void ClearAll()
    {
        m_Attachment.ClearAll();

        if (null != m_AvatarDescList)
        {
            for (int i = 0; i < m_AvatarDescList.Count; ++i)
            {
                GeAvatarDesc curAvatarDesc = m_AvatarDescList[i];
                if(curAvatarDesc != null)
                    _ClearAvatarMeshObject(curAvatarDesc);
            }
        }

        if (AssetLoader.INVILID_HANDLE != m_AvatarRootDesc.m_AsyncRequest)
        {
            if (EngineConfig.useTMEngine)
                CGameObjectPool.AbortAcquireRequest(m_AvatarRootDesc.m_AsyncRequest);
            else
                AssetManager.AbortRequest(m_AvatarRootDesc.m_AsyncRequest);

            --m_LoadingCnt;
            m_AvatarRootDesc.m_AsyncRequest = AssetLoader.INVILID_HANDLE;
        }

        if (null != m_AvatarRootDesc.m_MeshObject)
        {
            m_AvatarRootDesc.m_MeshObject.layer = m_AvatarRootDesc.m_OriginLayer;

            if (null != m_AvatarRootDesc.m_MeshRendererList)
            {
                for (int i = 0, icnt = m_AvatarRootDesc.m_MeshRendererList.Length; i < icnt; ++i)
                {
                    SkinnedMeshRenderer cur = m_AvatarRootDesc.m_MeshRendererList[i];
                    if (null == cur) continue;

                    cur.gameObject.layer = m_AvatarRootDesc.m_OriginLayer;
                }
            }

            if (EngineConfig.useTMEngine)
                CGameObjectPool.RecycleGameObjectEx(m_AvatarRootDesc.m_MeshObject);
            else
                AssetManager.RecycleAsset(m_AvatarRootDesc.m_MeshObject);

            m_AvatarRootDesc.m_MeshObject = null;
        }

        _SetSimpleAvatarAlpha(0.0f);
        _ClearAvatarMeshObject(m_SimpleAvatarDesc);

        m_AvatarSkeleton = null;
        m_OnLoad = null;

        _ResetAsyncCommads();
    }

    public void ChangeLayer(int layer)
    {
        if (null != m_AvatarRootDesc.m_MeshObject)
            _ChangeLayer(layer);
        else
            changeLayerCommand = new ChangeLayerCommand(layer);
    }
    protected void _ChangeLayer(int layer)
    {
        if (layer < 0 || layer > 31)
            Debug.LogErrorFormat("Inivalid layer ID:{0}", layer);

        m_RenderLayer = layer;

        m_AvatarRootDesc.m_MeshObject.layer = m_RenderLayer;

        SkinnedMeshRenderer[] asmr = m_AvatarRootDesc.m_MeshObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0, icnt = asmr.Length; i < icnt; ++i)
            if(null != asmr[i])
                asmr[i].gameObject.SetLayer(layer);
			
		MeshRenderer[] meshRenderer = m_AvatarRootDesc.m_MeshObject.GetComponentsInChildren<MeshRenderer>();
        if (null != meshRenderer)
        {
            for (int i = 0, icnt = meshRenderer.Length; i < icnt; ++i)
            {
                MeshRenderer cur = meshRenderer[i];
                if (null == cur) continue;

                cur.gameObject.layer = m_RenderLayer;
            }
        }

        ParticleSystem[] particles = m_AvatarRootDesc.m_MeshObject.GetComponentsInChildren<ParticleSystem>();
        if (null != particles)
        {
            for (int i = 0, icnt = particles.Length; i < icnt; ++i)
            {
                ParticleSystem cur = particles[i];
                if (null == cur) continue;

                cur.gameObject.layer = m_RenderLayer;
            }
        }
    }

    public void ChangeDisplayMode(GeActorEx.DisplayMode displayMode)
    {
        if(m_DisplayMode != displayMode)
        {
            m_DisplayMode = displayMode;

            // Root加载完成才能切换至Normal状态
            if (null != m_AvatarRootDesc.m_MeshObject)
                _ChangeDisplayMode(m_DisplayMode);
            else
                displayModeChangeCmd = new DisplayModeChangeCmd(m_DisplayMode, true);
        }
    }

    protected void _ExecuteDisplayModeChangeCmd()
    {
        if(displayModeChangeCmd.valid)
        {
            displayModeChangeCmd.valid = false;

            _ChangeDisplayMode(displayModeChangeCmd.targetDisplayMode);
        }
    }

    protected void _ChangeDisplayMode(GeActorEx.DisplayMode displayMode)
    {
        if(displayMode == GeActorEx.DisplayMode.Normal)
        {
            // 加载有效Command中的时装
            for (int i = 0, icnt = (int)GeAvatarChannel.MaxChannelNum; i < icnt; ++i)
            {
                if (changeAvatarCommand[i].bVilad)
                {
                    _ChangeAvatarObject(changeAvatarCommand[i].eChannel, changeAvatarCommand[i].sAsset, changeAvatarCommand[i].bIsAsyncLoad, changeAvatarCommand[i].bHighPriority, changeAvatarCommand[i].prodid, changeAvatarCommand[i].m_RecolorDataPath);
                    changeAvatarCommand[i].bVilad = false;
                }
            }


            // 显示武器
            m_Attachment.SetDisplayMode(displayMode);
            // 隐藏小白人
            m_IsFading = true;
            m_FadeTimer = Mathf.Clamp(m_FadeTimer, 0f, m_FadingTimeLength);
        }
        else if(displayMode == GeActorEx.DisplayMode.Simple)
        {
            if (m_SimpleAvatarDesc.m_MeshObject == null)
                _LoadSimpleAvatar();

            _SetSimpleAvatarAlpha(0f);
            // 隐藏时装
            _SetNormalVisible(false);
            // 隐藏武器
            m_Attachment.SetDisplayMode(displayMode);
            // 显示小白人
            m_IsFading = true;
            m_FadeTimer = Mathf.Clamp(m_FadeTimer, 0f, m_FadingTimeLength);
            if (m_SimpleAvatarDesc != null && m_SimpleAvatarDesc.m_MeshObject != null)
                m_SimpleAvatarDesc.m_MeshObject.CustomActive(true);
        }
    }

    protected void _LoadSimpleAvatar()
    {
        _ClearAvatarMeshObject(m_SimpleAvatarDesc);

        if(string.IsNullOrEmpty(m_SimpleAvatarDesc.m_Asset.m_AssetPath))
        {
            if(!string.IsNullOrEmpty(m_AvatarRootDesc.m_Asset.m_AssetPath))
                m_SimpleAvatarDesc.m_Asset = new DAssetObject(null, m_AvatarRootDesc.m_Asset.m_AssetPath + "_Simple");
        }

        string assetPath = m_SimpleAvatarDesc.m_Asset.m_AssetPath;
        if(!string.IsNullOrEmpty(assetPath))
        {
            m_SimpleAvatarDesc.m_MeshObject = null;
            m_SimpleAvatarDesc.m_HasRemapSkeleton = false;
            m_SimpleAvatarDesc.m_AvatarAttachment = null;
            m_SimpleAvatarDesc.m_MeshRendererList = null;
            m_SimpleAvatarDesc.m_SkeletonRoot = null;
            m_SimpleAvatarDesc.m_RendObjRoot = null;
            m_SimpleAvatarDesc.m_OriginLayer = 0;

            if (-1 == m_LoadingCnt)
                m_LoadingCnt = 0;
            m_IsAsyncLoad = true;

            ++m_LoadingCnt;

            if (EngineConfig.useTMEngine)
            {
                if (AssetLoader.INVILID_HANDLE != m_SimpleAvatarDesc.m_AsyncRequest)
                {
                    CGameObjectPool.AbortAcquireRequest(m_SimpleAvatarDesc.m_AsyncRequest);
                }
                m_SimpleAvatarDesc.m_AsyncRequest = CGameObjectPool.GetGameObjectAsync(assetPath, m_AssetLoadCallbacks, this);
            }
            else
            {

                if (AssetLoader.INVILID_HANDLE != m_SimpleAvatarDesc.m_AsyncRequest)
                {
                    AssetManager.AbortRequest(m_SimpleAvatarDesc.m_AsyncRequest);
                    --m_LoadingCnt;
                }

                uint flag = (uint)AssetLoadFlag.HideAfterLoad;

                m_SimpleAvatarDesc.m_AsyncRequest = AssetManager.LoadAssetRequest(assetPath, typeof(GameObject), m_bUsePool, flag, false, 0xaddad213);
            }
        }
        else
        {
            Logger.LogError("Simple avatar path is empty!");
        }
    }

    protected void _SetSimpleAvatarAlpha(float alpha)
    {
        if (m_SimpleAvatarDesc != null && m_SimpleAvatarDesc.m_MeshRendererList != null)
        {
            foreach (SkinnedMeshRenderer renderer in m_SimpleAvatarDesc.m_MeshRendererList)
            {
                renderer.GetPropertyBlock(MatBlock);
                block.SetFloat("_Alpha", alpha);
                renderer.SetPropertyBlock(MatBlock);
            }
        }
    }

    public void ChangeAvatarObject(GeAvatarChannel eChannel, DAssetObject asset, bool isAsyncLoad = true, bool highPriority = false, int prodId = 0, string recolorDataPath = "")
    {
        // Normal模式加载部件
        if (null != m_AvatarRootDesc.m_MeshObject && m_DisplayMode == GeActorEx.DisplayMode.Normal)
            _ChangeAvatarObject(eChannel, asset, isAsyncLoad,highPriority, prodId, recolorDataPath);
        else
            changeAvatarCommand[(int)eChannel] = new ChangeAvatarCommand(eChannel, asset,isAsyncLoad, highPriority, prodId, recolorDataPath);
    }

    protected void _ChangeAvatarObject(GeAvatarChannel eChannel, DAssetObject asset, bool isAsyncLoad = true, bool highPriority = false,int prodId = 0, string recolorDataPath = "")
    {
        //只有吃鸡环境下m_professionalId 才有可能id不为0 
        //目的，防止因协议流 无序的情况导致蒙皮错误
        if (m_professionalId != 0 && prodId != m_professionalId)
        {
            Logger.LogErrorFormat("try change avatar object channel {0} asset {1} curprodid {2} needprodid {3}", 
                eChannel,
                asset.m_AssetPath,
                m_professionalId, prodId);
            return;
        }
        if (!m_AvatarRootDesc.m_MeshObject)
            return;

        if (m_IsAsyncLoad)
            isAsyncLoad = true;

        int idx = (int)eChannel;

        if (idx < m_AvatarDescList.Count)
        {
            GeAvatarDesc curAvatarDesc = m_AvatarDescList[idx];

            _ClearAvatarMeshObject(curAvatarDesc);
            if (!string.IsNullOrEmpty(asset.m_AssetPath))
            {
                string assetPath = asset.m_AssetPath;
                curAvatarDesc.m_IsRecolor = !string.IsNullOrEmpty(recolorDataPath);

                // 如果启用了AvatarFallback，先检查资源是否存在，不存在改成加载默认资源。
                if(IsAvatarPartFallbackEnabled() && !GeAvatarFallback.IsAssetDependentAvaliable(assetPath))
                {
                    assetPath = GeAvatarFallback.GetFallbackAvatar(m_ActorOccupation, eChannel, assetPath);
                    if(string.IsNullOrEmpty(assetPath))
                    {
                        assetPath = asset.m_AssetPath;
                    }
                }

                if(isAsyncLoad)
                {
                    curAvatarDesc.m_Asset = asset;
                    curAvatarDesc.m_MeshObject = null;
                    curAvatarDesc.m_HasRemapSkeleton = false;
                    curAvatarDesc.m_AvatarAttachment = null;
                    curAvatarDesc.m_MeshRendererList = null;
                    curAvatarDesc.m_SkeletonRoot = null;
                    curAvatarDesc.m_RendObjRoot = null;
                    curAvatarDesc.m_OriginLayer = 0;

                    if (-1 == m_LoadingCnt)
                        m_LoadingCnt = 0;
                    m_IsAsyncLoad = true;

                    ++m_LoadingCnt;

                    uint flag = 0u;
                    if (m_bUsePool)
                    {
                        flag = highPriority ? ((uint)GameObjectPoolFlag.HideAfterLoad | (uint)GameObjectPoolFlag.HighPriority) : (uint)GameObjectPoolFlag.HideAfterLoad;
                    }
                    else
                    {
                        flag = highPriority ? ((uint)AssetLoadFlag.HideAfterLoad | (uint)AssetLoadFlag.HighPriority) : (uint)AssetLoadFlag.HideAfterLoad;
                    }

                    //异步加载染色数据
                    AssetLoader.LoadAsset(recolorDataPath, typeof(DRecolorData), curAvatarDesc, _OnRecolorAssetLoadSuccess);

                    if (EngineConfig.useTMEngine)
                    {
                        if (AssetLoader.INVILID_HANDLE != curAvatarDesc.m_AsyncRequest)
                        {
                            --m_LoadingCnt;
                            //AssetLoader.AbortLoadRequest(curAvatarDesc.m_AsyncRequest);
                            CGameObjectPool.AbortAcquireRequest(curAvatarDesc.m_AsyncRequest);
                        }
                        //curAvatarDesc.m_AsyncRequest = AssetLoader.LoadResAsGameObjectAsync(asset.m_AssetPath, m_AssetLoadCallbacks, this);
                        curAvatarDesc.m_AsyncRequest = CGameObjectPool.GetGameObjectAsync(assetPath, m_AssetLoadCallbacks, this);
                    }
                    else
                    {

                        if (AssetLoader.INVILID_HANDLE != curAvatarDesc.m_AsyncRequest)
                        {
                            AssetManager.AbortRequest(curAvatarDesc.m_AsyncRequest);
                            --m_LoadingCnt;
                        }

                        curAvatarDesc.m_AsyncRequest = AssetManager.LoadAssetRequest(assetPath, typeof(GameObject), m_bUsePool, flag, false, 0xaddad213);
                    }

                    Logger.LogFormat("Async load avatar game object \"{0}\"!", assetPath);
                }
                else
                {
                    m_IsAsyncLoad = false;
                    uint flag = m_bUsePool ? (uint)GameObjectPoolFlag.HideAfterLoad : (uint)AssetLoadFlag.HideAfterLoad;

                    //同步加载染色数据
                    AssetLoader.LoadAsset(recolorDataPath, typeof(DRecolorData), curAvatarDesc, _OnRecolorAssetLoadSuccess, false);

                    if (EngineConfig.useTMEngine)
                        curAvatarDesc.m_MeshObject = CGameObjectPool.GetGameObject(assetPath,0) as GameObject;
                    else
                        curAvatarDesc.m_MeshObject = AssetManager.LoadAsset(assetPath, typeof(GameObject), m_bUsePool, flag) as GameObject;
                    if (null != curAvatarDesc && null != curAvatarDesc.m_MeshObject)
                    {
                        _OnAvatarPartLoaded(curAvatarDesc);
                    }
                    else
                        Logger.LogWarningFormat("Create avatar mesh has failed![Res:{0}]", assetPath);

                    // curAvatarDesc.m_MeshObject = AssetManager.LoadAsset(asset.m_AssetPath, typeof(GameObject), m_bUsePool, false) as GameObject;
                    // if (null != curAvatarDesc && null != curAvatarDesc.m_MeshObject)
                    // {
                    //     curAvatarDesc.m_MeshObject.layer = m_RenderLayer;
                    //     curAvatarDesc.m_Asset = asset;
                    //     curAvatarDesc.m_MeshObject.transform.SetParent(m_AvatarRootDesc.m_MeshObject.transform, false);
                    // 
                    //     curAvatarDesc.m_MeshRendererList = curAvatarDesc.m_MeshObject.GetComponents<SkinnedMeshRenderer>();
                    //     for (int i = 0; i < curAvatarDesc.m_MeshRendererList.Length; ++i)
                    //     {
                    //         curAvatarDesc.m_OriginLayer = curAvatarDesc.m_MeshRendererList[i].gameObject.layer;
                    //         curAvatarDesc.m_MeshRendererList[i].gameObject.layer = m_RenderLayer;
                    //     }
                    // 
                    //     /// 骨骼处理
                    //     // Animation[] animation = curAvatarDesc.m_MeshObject.GetComponentsInChildren<Animation>();
                    //     // for (int j = 0; j < animation.Length; ++j)
                    //     //     Object.Destroy(animation[j]);
                    //     // 
                    //     // string boneAllName = null;
                    //     // int nChildNum = curAvatarDesc.m_MeshObject.transform.childCount;
                    //     // for (int j = 0; j < nChildNum; ++j)
                    //     // {
                    //     //     GameObject boneAll = curAvatarDesc.m_MeshObject.transform.GetChild(j).gameObject;
                    //     //     boneAllName = boneAll.name;//.ToLower();
                    //     //     if (null != boneAll && boneAllName.Contains("boneall", System.StringComparison.OrdinalIgnoreCase))
                    //     //     {
                    //     //         curAvatarDesc.m_SkeletonRoot = boneAll;
                    //     //         boneAll.SetActive(false);
                    //     //         break;
                    //     //     }
                    //     // }
                    // 
                    //     if (null == curAvatarDesc.m_SkeletonRoot)
                    //         curAvatarDesc.m_SkeletonRoot = m_SkeletonRoot;
                    // 
                    //     /// 重定位骨骼
                    //     _RemapSkeleton(curAvatarDesc);
                    // 
                    //     m_RenderMeshList.Clear();
                    //     for (int i = 0; i < m_AvatarDescList.Count; ++ i)
                    //     {
                    //         curAvatarDesc = m_AvatarDescList[i];
                    //         if (curAvatarDesc.m_MeshObject)
                    //         {
                    //             m_RenderMeshList.Add(curAvatarDesc.m_MeshObject);
                    // 
                    //             Renderer curRend = curAvatarDesc.m_MeshObject.GetComponent<Renderer>();
                    //             if (null != curRend)
                    //             {
                    //                 m_AvatarBBox.min = Vector3.Min(curRend.bounds.min, m_AvatarBBox.min);
                    //                 m_AvatarBBox.max = Vector3.Min(curRend.bounds.max, m_AvatarBBox.max);
                    //             }
                    //         }
                    //     }
                    // 
                    //     curAvatarDesc.m_HasRemapSkeleton = true;
                    //     return;
                    // }
                    //  else
                    //      Logger.LogWarningFormat("Create avatar mesh has failed![Res:{0}]", asset.m_AssetPath);
                }
                
            }
            else {
            
                //卸下，但是穿上初始的
                Logger.LogWarningFormat("Avatar mesh res path is invalid string![Res:{0}]", asset.m_AssetPath);
                if (savedInitAvatar.ContainsKey((int)eChannel))
                {
                    if(!string.IsNullOrEmpty(savedInitAvatar[(int)eChannel].m_AssetPath))
                        ChangeAvatarObject(eChannel, savedInitAvatar[(int)eChannel], isAsyncLoad, highPriority, prodId);
                }
            }
                
        }
        else
            Logger.LogWarningFormat("Invalid channel enumination {0}!", eChannel.ToString());
    }

    public void SuitAvatar(bool isAsyncLoad = true,bool highPriority = false,int prodId = 0)
    {
        for (int idx = 0; idx < (int)GeAvatarChannel.MaxChannelNum; ++idx)
        {
            GeAvatarDesc curAvatarDesc = m_AvatarDescList[idx];
            if (null == curAvatarDesc.m_MeshObject && AssetLoader.INVILID_HANDLE == curAvatarDesc.m_AsyncRequest)
            {
                if (savedInitAvatar.ContainsKey(idx) && !string.IsNullOrEmpty(savedInitAvatar[idx].m_AssetPath))
                {
                    ChangeAvatarObject((GeAvatarChannel)idx, savedInitAvatar[idx], isAsyncLoad,highPriority, prodId);
                }
            }
        }
    }

    public void AddDefaultAvatar(GeAvatarChannel eChannel, DAssetObject asset)
    {
        if (eChannel == GeAvatarChannel.Headwear || eChannel == GeAvatarChannel.Bracelet)
            return;

        if (!savedInitAvatar.ContainsKey((int)eChannel))
        {
            savedInitAvatar.Add((int)eChannel, asset);
        }
    }

    public GeAttach CreateAttachment(string attachmentName, string attachRes, string attachNode, bool cached = false, bool asyncLoad = false,bool highPriority = false)
    {
        GeAttach attachment = m_Attachment.AddAttachment(attachmentName, attachRes, attachNode, true, asyncLoad, highPriority);

        if (attachment == null)
            return null;

        attachment.geAvatar = this;

        m_Attachment.RefreshAttachNode(m_SkeletonRoot);

        if (cached)
            attachment.cached = true;

        return attachment;
    }

    public void DestroyAttachment(GeAttach att)
    {
        m_Attachment.RemoveAttachment(att);
    }

    public GeAttach GetAttachment(string attach, string nodeName = null)
    {
        return m_Attachment.GetAttachment(attach, nodeName);
    }

    public GameObject GetAttachNode(string attachNode)
    {
        GeAttachManager.GeAttachNodeDesc attachNodeDesc = m_Attachment.GetAttchNodeDesc(attachNode);
        return attachNodeDesc.attachNode;
    }

    public void ClearAttachmentOnNode(string attachNode)
    {
        if (null != m_Attachment)
            m_Attachment.ClearAttachmentOnNode(attachNode);
    }

    public bool ChangeAction(string action, float speed, bool loop = false, float offset = 0.0f)
    {
        if (null != m_AvatarRootDesc.m_MeshObject)
            return _ChangeAction(action, speed, loop, offset);
        else
        {
            changeActionCommand = new ChangeActionCommand(action, speed, loop, offset);
            return true;
        }
    }
    protected bool _ChangeAction(string action, float speed, bool loop = false, float offset = 0.0f)
    {
        if (null != m_Attachment)
            m_Attachment.ChangeAction(action, speed, loop, offset);
        return m_Animation.PlayAction(action, speed, loop, offset);
    }

    public string GetCurActionName()
    {
        if(null != m_Animation)
            return m_Animation.GetCurActionName();
        return "";
    }
    public float GetCurActionOffset()
    {
        if (null != m_Animation)
            return m_Animation.GetCurActionOffset();
        return 0.0f;
    }

    public bool GetCurActionLoop()
    {
        if (null != m_Animation)
            return m_Animation.IsCurActionLoop();
        return false;
    }

    public float GetActionSpeed()
    {
        if (null != m_Animation)
            return m_Animation.GetCurrentAnimationSpeed();
        return 1.0f;
    }

    public void StopAction()
    {
        if (null != m_Animation)
            m_Animation.Stop();

        if (null != m_Attachment)
            m_Attachment.StopAction();
    }

    public bool IsActionFinished()
    {
        if (null != m_Animation)
            return m_Animation.IsCurAnimFinished();

        return true;
    }

    public float GetActionTimeLen(string action)
    {
        if (null != m_Animation)
        {
            GeAnimDesc clipDesc = m_Animation.GetAnimClipDesc(action);
            if (null != clipDesc)
                return clipDesc.timeLen;
        }
        return 0.0f;
    }

	public bool IsActionLoop(string action)
	{
		if (null != m_Animation)
		{
			GeAnimDesc clipDesc = m_Animation.GetAnimClipDesc(action);
			if (null != clipDesc)
				return clipDesc.animPlayMode == GeAnimClipPlayMode.AnimPlayLoop;
		}
		return false;
	}

	public float GetCurActionSpeed()
	{
		if (null != m_Animation) {
			return m_Animation.GetCurrentAnimationSpeed ();
		}
		return 1.0f;
	}

    public void PreloadAction(string anim)
    {
        if (null != m_Animation)
            m_Animation.PreloadAction(anim);
    }

    public void PreloadAction(string[] animList)
    {
        if (null != m_Animation)
            m_Animation.PreloadAction(animList);
    }

    public string GetResPath()
    {
        return m_AvatarRootDesc.m_Asset.m_AssetPath;
    }

    public void Update(int delta,uint mask = uint.MaxValue)
    {
        if (null != m_Attachment)
        {
            if (0 != (mask & (uint)EAvatarRes.Attach))
                m_Attachment.Update();
        }

        if(m_IsFading)
        {
            float deltaMS = delta / (float)(GlobalLogic.VALUE_1000);

            if(m_DisplayMode == GeActorEx.DisplayMode.Simple)
                m_FadeTimer += deltaMS;
            else if(m_DisplayMode == GeActorEx.DisplayMode.Normal)
                m_FadeTimer -= deltaMS;

            float alpha = Mathf.Lerp(0, 1, m_FadeTimer / m_FadingTimeLength);
            _SetSimpleAvatarAlpha(alpha);

            if (m_FadeTimer > m_FadingTimeLength || m_FadeTimer < 0)
            {
                m_IsFading = false;

                if(m_DisplayMode == GeActorEx.DisplayMode.Normal)
                {
                    if (m_SimpleAvatarDesc != null && m_SimpleAvatarDesc.m_MeshObject != null)
                        m_SimpleAvatarDesc.m_MeshObject.CustomActive(false);
                    // 显示时装
                    _SetNormalVisible(true);
                }
            }
        }
    }

    public void Pause(uint mask = uint.MaxValue)
    {
        if (0 != (mask & (uint)EAvatarRes.Action))
            m_Animation.Pause();

        if (0 != (mask & (uint)EAvatarRes.Attach))
            m_Attachment.Pause();
    }

    public void Resume(uint mask = uint.MaxValue)
    {
        if (0 != (mask & (uint)EAvatarRes.Action))
            m_Animation.Resume();

        if (0 != (mask & (uint)EAvatarRes.Attach))
            m_Attachment.Resume();
    }

    public void Clear(uint mask = (uint)EAvatarRes.All)
    {
        if (0 != (mask & (uint)EAvatarRes.Attach))
            m_Attachment.ClearAll();
    }

    public void UpdateAsyncLoading()
    {
        if (EngineConfig.useTMEngine)
            return;

        //if (!_IsRootOK())
        //{
            if(AssetLoader.INVILID_HANDLE != m_AvatarRootDesc.m_AsyncRequest)
            {
                if (AssetManager.IsRequestDone(m_AvatarRootDesc.m_AsyncRequest))
                {
                    --m_LoadingCnt;
                    GameObject go = AssetManager.ExtractAsset(m_AvatarRootDesc.m_AsyncRequest) as GameObject;
                    m_AvatarRootDesc.m_AsyncRequest = AssetLoader.INVILID_HANDLE;
                    if (null != go)
                    {
                        _OnAvatarRootLoaded(go);

                        // m_AvatarRootDesc.m_MeshObject = go;
                        // 
                        // /// 后面需要预处理
                        // SkinnedMeshRenderer avatarmr = m_AvatarRootDesc.m_MeshObject.GetComponentInChildren<SkinnedMeshRenderer>();
                        // if (null != avatarmr)
                        // {
                        //     m_AvatarRootMesh = avatarmr.gameObject;
                        //     m_OriginLayer = m_AvatarRootMesh.layer;
                        // }
                        // 
                        // m_AvatarRootDesc.m_MeshObject.layer = m_RenderLayer;
                        // 
                        // /// 重新初始化角色本事的挂点列表
                        // m_SkeletonRoot = _FindSkeletonRoot(m_AvatarRootDesc.m_MeshObject);
                        // if (null != m_SkeletonRoot)
                        //     m_AvatarSkeleton = m_AvatarRootDesc.m_MeshObject.transform.GetChild(0).GetComponentsInChildren<Transform>();
                    }
                }
            }
        //}

        for(int i = 0,icnt = m_AvatarDescList.Count; i<icnt;++i)
        {
            GeAvatarDesc curAvatarDesc = m_AvatarDescList[i];
            if(null == curAvatarDesc) continue;

            if (AssetLoader.INVILID_HANDLE == curAvatarDesc.m_AsyncRequest) continue;

            if (AssetManager.IsRequestDone(curAvatarDesc.m_AsyncRequest))
            {
                --m_LoadingCnt;
                
                GameObject go = AssetManager.ExtractAsset(curAvatarDesc.m_AsyncRequest) as GameObject;
                curAvatarDesc.m_AsyncRequest = AssetLoader.INVILID_HANDLE;
                if (null != go)
                {
                    curAvatarDesc.m_MeshObject = go;
                    _OnAvatarPartLoaded(curAvatarDesc);

                    /// Debug.LogFormat("### Finish load avatar game object \"{0}\"(Time:{1})!", curAvatarDesc.m_Asset.m_AssetPath, Time.realtimeSinceStartup);

                    /// go.transform.localPosition = Vector3.zero;
                    /// curAvatarDesc.m_MeshObject.layer = m_RenderLayer;
                    /// curAvatarDesc.m_MeshObject.transform.SetParent(m_AvatarRootDesc.m_MeshObject.transform, false);
                    /// 
                    /// /// 骨骼处理
                    /// curAvatarDesc.m_MeshRendererList = curAvatarDesc.m_MeshObject.GetComponentsInChildren<SkinnedMeshRenderer>();
                    /// for (int j = 0, jcnt = curAvatarDesc.m_MeshRendererList.Length; j < jcnt; ++j)
                    /// {
                    ///     curAvatarDesc.m_OriginLayer = curAvatarDesc.m_MeshRendererList[j].gameObject.layer;
                    ///     curAvatarDesc.m_MeshRendererList[j].gameObject.layer = m_RenderLayer;
                    /// }
                    /// 
                    /// // Animation[] animation = curAvatarDesc.m_MeshObject.GetComponentsInChildren<Animation>();
                    /// // for (int j = 0; j < animation.Length; ++j)
                    /// //     Object.Destroy(animation[j]);
                    /// // 
                    /// // string boneAllName = null;
                    /// // int nChildNum = curAvatarDesc.m_MeshObject.transform.childCount;
                    /// // for (int j = 0; j < nChildNum; ++j)
                    /// // {
                    /// //     GameObject boneAll = curAvatarDesc.m_MeshObject.transform.GetChild(j).gameObject;
					/// // 	boneAllName = boneAll.name;//.ToLower();
					/// // 	if (null != boneAll && boneAllName.Contains("boneall", System.StringComparison.OrdinalIgnoreCase))
                    /// //     {
                    /// //         curAvatarDesc.m_SkeletonRoot = boneAll;
                    /// //         boneAll.SetActive(false);
                    /// //         break;
                    /// //     }
                    /// // }
                    /// 
                    /// if (null == curAvatarDesc.m_SkeletonRoot)
                    ///     curAvatarDesc.m_SkeletonRoot = m_SkeletonRoot;
                    /// 
                    /// /// 重定位骨骼
                    /// _RemapSkeleton(curAvatarDesc);
                    /// 
                    /// m_RenderMeshList.Clear();
                    /// m_AvatarBBox.extents = Vector3.zero;
                    /// m_AvatarBBox.center = Vector3.zero;
                    /// for (int j = 0,jcnt = m_AvatarDescList.Count; j < jcnt; ++j)
                    /// {
                    ///     curAvatarDesc = m_AvatarDescList[j];
                    ///     if (curAvatarDesc.m_MeshObject)
                    ///     {
                    ///         m_RenderMeshList.Add(curAvatarDesc.m_MeshObject);
                    /// 
                    ///         Renderer curRend = curAvatarDesc.m_MeshObject.GetComponent<Renderer>();
                    ///         if (null != curRend)
                    ///         {
                    ///             m_AvatarBBox.min = Vector3.Min(curRend.bounds.min, m_AvatarBBox.min);
                    ///             m_AvatarBBox.max = Vector3.Min(curRend.bounds.max, m_AvatarBBox.max);
                    ///         }
                    ///     }
                    /// }
                    /// 
                    /// curAvatarDesc.m_HasRemapSkeleton = true;
                }

                Logger.LogFormat("Finish async load avatar game object \"{0}\"!", curAvatarDesc.m_Asset.m_AssetPath);
            }
        }

        // 小白人
        if (AssetLoader.INVILID_HANDLE == m_SimpleAvatarDesc.m_AsyncRequest)
        {
            if (AssetManager.IsRequestDone(m_SimpleAvatarDesc.m_AsyncRequest))
            {
                --m_LoadingCnt;

                GameObject go = AssetManager.ExtractAsset(m_SimpleAvatarDesc.m_AsyncRequest) as GameObject;
                m_SimpleAvatarDesc.m_AsyncRequest = AssetLoader.INVILID_HANDLE;
                if (null != go)
                {
                    m_SimpleAvatarDesc.m_MeshObject = go;
                    _OnAvatarPartLoaded(m_SimpleAvatarDesc);
                }
            }
        }
    }

    public void SetAvatarVisible(bool visible)
    {
        _SetNormalVisible(visible & (m_DisplayMode == GeActorEx.DisplayMode.Normal));
        _SetSimpleVisible(visible & (m_DisplayMode == GeActorEx.DisplayMode.Simple));
    }

    private void _SetNormalVisible(bool visible)
    {
        for (int i = 0, icnt = m_RenderMeshList.Count; i < icnt; ++i)
        {
            GameObject curObj = m_RenderMeshList[i];
            if (null == curObj)
                continue;

            curObj.SetActive(visible);
        }
    }

    private void _SetSimpleVisible(bool visible)
    {
        if (m_SimpleAvatarDesc.m_MeshObject != null)
            m_SimpleAvatarDesc.m_MeshObject.SetActive(visible);
    }

    void _OnAvatarRootLoaded(GameObject go)
    {
        go.SetActive(true);
        m_AvatarRootDesc.m_MeshObject = go;
        m_AvatarRootDesc.m_MeshObject.layer = m_RenderLayer;

        /// 后面需要预处理
        SkinnedMeshRenderer[] avatarmr = m_AvatarRootDesc.m_MeshObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (null != avatarmr)
        {
            m_AvatarRootDesc.m_MeshRendererList = avatarmr;
            m_AvatarRootDesc.m_OriginLayer = go.layer;

            for(int i = 0,icnt = avatarmr.Length;i<icnt;++i)
            {
                SkinnedMeshRenderer cur = avatarmr[i];
                if(null == cur) continue;

                cur.gameObject.layer = m_RenderLayer;
            }
        }
		
		MeshRenderer[] meshRenderer = m_AvatarRootDesc.m_MeshObject.GetComponentsInChildren<MeshRenderer>();
        if (null != meshRenderer)
        {
            for (int i = 0, icnt = meshRenderer.Length; i < icnt; ++i)
            {
                MeshRenderer cur = meshRenderer[i];
                if (null == cur) continue;

                cur.gameObject.layer = m_RenderLayer;
            }
        }

        ParticleSystem[] particles = m_AvatarRootDesc.m_MeshObject.GetComponentsInChildren<ParticleSystem>();
        if (null != particles)
        {
            for (int i = 0, icnt = particles.Length; i < icnt; ++i)
            {
                ParticleSystem cur = particles[i];
                if (null == cur) continue;

                cur.gameObject.layer = m_RenderLayer;
            }
        }

        /// 重新初始化角色本事的挂点列表
        m_SkeletonRoot = _FindSkeletonRoot(m_AvatarRootDesc.m_MeshObject);
        if (null != m_SkeletonRoot)
            m_AvatarSkeleton = m_AvatarRootDesc.m_MeshObject.transform.GetChild(0).GetComponentsInChildren<Transform>();

        m_Animation.Init(m_AvatarRootDesc.m_MeshObject);
        m_Attachment.RefreshAttachNode(m_AvatarRootDesc.m_MeshObject);

        if(changeActionCommand.bVilad)
        {
            _ChangeAction(changeActionCommand.strAction, changeActionCommand.fSpeed, changeActionCommand.bLoop, changeActionCommand.fOffset);
            changeActionCommand.bVilad = false;
        }

        // Normal模式加载部件
        if(m_DisplayMode == GeActorEx.DisplayMode.Normal)
        {
            for (int i = 0, icnt = (int)GeAvatarChannel.MaxChannelNum; i < icnt; ++i)
            {
                if (changeAvatarCommand[i].bVilad)
                {
                    _ChangeAvatarObject(changeAvatarCommand[i].eChannel, changeAvatarCommand[i].sAsset, changeAvatarCommand[i].bIsAsyncLoad, changeAvatarCommand[i].bHighPriority, changeAvatarCommand[i].prodid, changeAvatarCommand[i].m_RecolorDataPath);
                    changeAvatarCommand[i].bVilad = false;
                }
            }
        }

        if (changeLayerCommand.bVilad)
        {
            _ChangeLayer(changeLayerCommand.nLayer);
            changeLayerCommand.bVilad = false;
        }

        _ExecuteDisplayModeChangeCmd();
    }

    void _OnAvatarPartLoaded(GeAvatarDesc newAvatarDesc)
    {
        if (null != newAvatarDesc && null != newAvatarDesc.m_MeshObject)
        {
            newAvatarDesc.m_MeshObject.transform.localPosition = new Vector3(0,0, (int)newAvatarDesc.m_Channel * 0.0001f);
            newAvatarDesc.m_MeshObject.layer = m_RenderLayer;

            if (null != m_AvatarRootDesc.m_MeshObject)
                newAvatarDesc.m_MeshObject.transform.SetParent(m_AvatarRootDesc.m_MeshObject.transform, false);
            else
                Debug.LogErrorFormat("Avatar Root is null when avatar part '{0}' loaded.", newAvatarDesc.m_MeshObject.name);

            /// 骨骼处理
            newAvatarDesc.m_MeshRendererList = newAvatarDesc.m_MeshObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (null != newAvatarDesc.m_MeshRendererList)
            {
                for (int j = 0, jcnt = newAvatarDesc.m_MeshRendererList.Length; j < jcnt; ++j)
                {

                    SkinnedMeshRenderer smr = newAvatarDesc.m_MeshRendererList[j];
                    if (smr == null) continue;
                    smr.localBounds = new Bounds(new Vector3(0, 0, 0), new Vector3(2, 2, 2));

                    GameObject meshRendObj = smr.gameObject;
                    if (null == meshRendObj)
                        continue;

                    newAvatarDesc.m_OriginLayer = meshRendObj.layer;
                    meshRendObj.layer = m_RenderLayer;

                    //设置染色数据
                    _SetRecolorData(smr, newAvatarDesc);
                }
            }

            // 根据当前DisplayMode显示或隐藏
            if (newAvatarDesc.m_Channel == GeAvatarChannel.Simple)
            {
                newAvatarDesc.m_MeshObject.SetActive(m_DisplayMode == GeActorEx.DisplayMode.Simple);
                _SetSimpleAvatarAlpha(0f);
            }
            else
            {
                newAvatarDesc.m_MeshObject.SetActive(m_DisplayMode == GeActorEx.DisplayMode.Normal);
            }

            // Animation[] animation = curAvatarDesc.m_MeshObject.GetComponentsInChildren<Animation>();
            // for (int j = 0; j < animation.Length; ++j)
            //     Object.Destroy(animation[j]);
            // 
            // string boneAllName = null;
            // int nChildNum = curAvatarDesc.m_MeshObject.transform.childCount;
            // for (int j = 0; j < nChildNum; ++j)
            // {
            //     GameObject boneAll = curAvatarDesc.m_MeshObject.transform.GetChild(j).gameObject;
            // 	boneAllName = boneAll.name;//.ToLower();
            // 	if (null != boneAll && boneAllName.Contains("boneall", System.StringComparison.OrdinalIgnoreCase))
            //     {
            //         curAvatarDesc.m_SkeletonRoot = boneAll;
            //         boneAll.SetActive(false);
            //         break;
            //     }
            // }

            if (null == newAvatarDesc.m_SkeletonRoot)
                newAvatarDesc.m_SkeletonRoot = m_SkeletonRoot;

            /// 重定位骨骼
            _RemapSkeleton(newAvatarDesc);

            m_RenderMeshList.Clear();
            m_AvatarBBox.extents = Vector3.zero;
            m_AvatarBBox.center = Vector3.zero;
            for (int j = 0, jcnt = m_AvatarDescList.Count; j < jcnt; ++j)
            {
                GeAvatarDesc curAvatarDesc = m_AvatarDescList[j];
                if (curAvatarDesc != null && curAvatarDesc.m_MeshObject != null)
                {
                    m_RenderMeshList.Add(curAvatarDesc.m_MeshObject);

                    //Renderer curRend = curAvatarDesc.m_MeshObject.GetComponent<Renderer>();
                    //if (null != curRend)
                    //{
                    //    m_AvatarBBox.min = Vector3.Min(curRend.bounds.min, m_AvatarBBox.min);
                    //    m_AvatarBBox.max = Vector3.Min(curRend.bounds.max, m_AvatarBBox.max);
                    //}
                }
            }

            newAvatarDesc.m_HasRemapSkeleton = true;

            //if(null != newAvatarDesc.m_MeshObject)
            //{
            //    GeAvatarAttachment avatarAttach = newAvatarDesc.m_MeshObject.GetComponent<GeAvatarAttachment>();
            //    if(null != avatarAttach && null != avatarAttach.attachDescArray)
            //    {
            //        for(int j = 0,jcnt = avatarAttach.attachDescArray.Length;j<jcnt;++j)
            //        {
            //            GeAttachDesc cur = avatarAttach.attachDescArray[j];
            //            if(null == cur) continue;
            //
            //            m_Attachment.AddAttachment("avatar_attach", cur.m_AttachmentRes, cur.m_AttachNode, false);
            //        }
            //    }
            //}

            if (null != m_OnLoad)
                m_OnLoad(this);
        }
    }

    void _ClearAvatarMeshObject(GeAvatarDesc avatarDesc)
    {
        if (null != avatarDesc.m_MeshObject)
        {
            SkinnedMeshRenderer[] asmr = avatarDesc.m_MeshRendererList;
            if (null != asmr)
            {
                for (int j = 0; j < asmr.Length; ++j)
                {
                    if (null != asmr[j])
                    {
                        asmr[j].gameObject.SetActive(true);
                        asmr[j].gameObject.layer = avatarDesc.m_OriginLayer;

                        if (asmr[j].material.IsKeywordEnabled("_DYECOLOR_ON"))
                            asmr[j].material.DisableKeyword("_DYECOLOR_ON");
                    }
                }
            }

            if (null != avatarDesc.m_SkeletonRoot)
                avatarDesc.m_SkeletonRoot.SetActive(true);

            avatarDesc.m_MeshObject.transform.SetParent(null, false);

            if (EngineConfig.useTMEngine)
                CGameObjectPool.RecycleGameObjectEx(avatarDesc.m_MeshObject);
            else
                AssetManager.RecycleAsset(avatarDesc.m_MeshObject);

            avatarDesc.m_MeshObject = null;
            avatarDesc.m_MeshRendererList = null;
            avatarDesc.m_SkeletonRoot = null;
            avatarDesc.m_HasRemapSkeleton = false;
            avatarDesc.m_RendObjRoot = null;
            avatarDesc.m_OriginLayer = 0;
            avatarDesc.m_DRecolorData = null;
        }

        if(AssetLoader.INVILID_HANDLE != avatarDesc.m_AsyncRequest)
        {
            if (EngineConfig.useTMEngine)
            {
                //AssetLoader.AbortLoadRequest(curAvatarDesc.m_AsyncRequest);
                CGameObjectPool.AbortAcquireRequest(avatarDesc.m_AsyncRequest);
            }
            else
            {
                AssetManager.AbortRequest(avatarDesc.m_AsyncRequest);
            }

            --m_LoadingCnt;
            avatarDesc.m_AsyncRequest = AssetLoader.INVILID_HANDLE;
        }
    }

    Transform _FindSkeletonBone(Transform[] skeleton, Transform bone)
    {
        if (null != skeleton && null != bone)
        {
            for (int i = 0; i < skeleton.Length; ++i)
            {
                if (null == skeleton[i])
                    continue;

                if (skeleton[i].name == bone.name)
                    return skeleton[i];
            }
        }

        return null;
    }

    void _RemapSkeleton(GeAvatarDesc avatarDesc)
    {
        /// 重定位骨骼
        if (null == m_AvatarSkeleton)
        {
            if (null != m_AvatarRootDesc && null != m_AvatarRootDesc.m_MeshObject && null != m_AvatarRootDesc.m_MeshObject.transform)
            {
                Transform skelRoot = m_AvatarRootDesc.m_MeshObject.transform.GetChild(0);
                if(null != skelRoot)
                    m_AvatarSkeleton = skelRoot.GetComponentsInChildren<Transform>();
            }
        }

        if (null == m_AvatarSkeleton)
        {
            Logger.LogError("Remap skeleton has failed cause avatar root is null!");
            return;
        }

        int skelNodeCnt = m_AvatarSkeleton.Length;

        GeAvatarProxy avatarProxy = avatarDesc.m_MeshObject.GetComponent<GeAvatarProxy>();
        if (null != avatarProxy)
        {
            for (int s = 0; s < avatarDesc.m_MeshRendererList.Length; s++)
            {
                SkinnedMeshRenderer smr = avatarDesc.m_MeshRendererList[s];
                if (null == smr) continue;

                Transform[] meshBones = smr.bones;
                if (null == meshBones)
                    continue;

                Transform[] bones = new Transform[meshBones.Length];

                for (int k = 0; k < meshBones.Length; ++k)
                {
                    if (s < avatarProxy.skelRemapOffset.Length)
                    {
                        if (avatarProxy.skelRemapOffset[s] + k < avatarProxy.skelRemapTable.Length)
                        {
                            int dstIdx = avatarProxy.skelRemapTable[avatarProxy.skelRemapOffset[s] + k];
                            if (0 <= dstIdx && dstIdx < skelNodeCnt)
                            {
                                bones[k] = m_AvatarSkeleton[dstIdx];
                                continue;
                            }
                        }
                    }
                    else
                        Logger.LogErrorFormat("Avatar [{0}] mesh render [{1}] index is out of range!(current index:{2} map size:{3})", m_AvatarRootDesc.m_Asset.m_AssetPath, avatarDesc.m_MeshObject.name, s, avatarProxy.skelRemapOffset.Length);

                    bones[k] = null;
                }

                smr.bones = bones;
            }
        }
        else
        {
            for (int s = 0; s < avatarDesc.m_MeshRendererList.Length; s++)
            {
                SkinnedMeshRenderer smr = avatarDesc.m_MeshRendererList[s];

                if(smr == null || smr.sharedMesh == null)
                {
                    continue;
                }

                for (int sbm = 0; sbm < smr.sharedMesh.subMeshCount; ++sbm)
                {
                    Transform[] meshBones = smr.bones;                    
                    Transform[] bones = new Transform[meshBones.Length];

                    for (int k = 0; k < meshBones.Length; ++k)
                    {
                        bones[k] = _FindSkeletonBone(m_AvatarSkeleton, meshBones[k]);
                    }

                    smr.bones = bones;
                }
            }
        }
    }

    GameObject _FindSkeletonRoot(GameObject parent)
    {
        if (null != parent && parent.name.ToLower().Contains("boneall"))
            return parent;

        GameObject skeleton = null;
        int nChildNum = parent.transform.childCount;
        for (int j = 0; j < nChildNum; ++j)
        {
            GameObject child = parent.transform.GetChild(j).gameObject;
            skeleton = _FindSkeletonRoot(child);
            if (null != skeleton)
                return skeleton;
        }

        return null;
    }

    public void SetAmbientColor(Color color)
    {
        for(int i = 0,icnt = m_RenderMeshList.Count;i<icnt;++i)
        {
            GeAvatarDesc cur = m_AvatarDescList[i];
            if(null == cur || null == cur.m_MeshRendererList) continue;

            for(int j = 0,jcnt = cur.m_MeshRendererList.Length;j<jcnt;++j)
            {
                SkinnedMeshRenderer smr = cur.m_MeshRendererList[j];
                if(null == smr) continue;

                Material[] am = smr.materials;
                if(null == am)
                    continue;

                for (int k = 0,kcnt = am.Length;k<kcnt;++k)
                {
                    Material curMat = am[k];
                    if(null == curMat) continue;

                    if (curMat.HasProperty("_AmbientColor"))
                        curMat.SetColor("_AmbientColor", color);
                }
            }
        }
    }

    public void SetLoadCallback(OnAvatarLoaded onLoaded)
    {
        m_OnLoad = onLoaded;
    }

    //加载染色数据的回调
    private void _OnRecolorAssetLoadSuccess(string recolorDataPath, object recolorAsset, int taskID, float duration, object userData)
    {
        if (null == recolorAsset)
        {
            Debugger.LogError("Recolor Asset '{0}' load error!", recolorDataPath);
            return;
        }

        DRecolorData recolorData = recolorAsset as DRecolorData;

        if (null == recolorData)
        {
            Debugger.LogError("Recolor Asset '{0}' is nil or type '{1}' error!", recolorDataPath, recolorAsset.GetType());
            return;
        }

        GeAvatarDesc newAvatarDesc = userData as GeAvatarDesc;

        if( null == newAvatarDesc)
        {
            Debugger.LogError("GeAvatarDesc '{0}' is null", newAvatarDesc);
            return;
        }

        if(!string.IsNullOrEmpty(recolorDataPath))
        {
            newAvatarDesc.m_DRecolorData = recolorData;

            SkinnedMeshRenderer[] meshRendererList = newAvatarDesc.m_MeshRendererList;

            if (null != meshRendererList)
            {
                for (int i = 0, icnt = meshRendererList.Length; i < icnt; ++i)
                {
                    SkinnedMeshRenderer smr = meshRendererList[i];
                    MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();

                    Material mat = smr.material;
                    mat.EnableKeyword("_DYECOLOR_ON");
                    smr.GetPropertyBlock(materialPropertyBlock);
                    materialPropertyBlock.SetMatrixArray("_MixingMatrices", recolorData.m_MatrixArray);
                    smr.SetPropertyBlock(materialPropertyBlock);
                }

                newAvatarDesc.m_DRecolorData = null;
            }
        }
    }

    //设置染色数据
    private void _SetRecolorData(SkinnedMeshRenderer meshRenderer, GeAvatarDesc curAvatarDesc)
    {
        Material mat = meshRenderer.material;
        DRecolorData data = curAvatarDesc.m_DRecolorData;
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();

        if (null != data)
        {
            mat.EnableKeyword("_DYECOLOR_ON");
            meshRenderer.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetMatrixArray("_MixingMatrices", data.m_MatrixArray);
            meshRenderer.SetPropertyBlock(materialPropertyBlock);
        }
        else
        {
            if (!curAvatarDesc.m_IsRecolor)
                if (mat.IsKeywordEnabled("_DYECOLOR_ON"))
                    mat.DisableKeyword("_DYECOLOR_ON");
        }
    }

    //////////////////////////////////////////////////////////////////////////
    /// 临时的接口 后面干掉
    public void Rebind(GameObject skeletonRoot)
    {
        if (null != m_Attachment)
            m_Attachment.RefreshAttachNode(skeletonRoot,true);

        if (null != m_Animation)
            m_Animation.Init(skeletonRoot);
    }

    public void ReplayAction()
    {
        if (null != m_Animation)
            m_Animation.Replay();
    }

    public GameObject GetAttchNodeDescWithRareName(string nodeRareName)
    {
        if (null != m_Attachment)
            return m_Attachment.GetAttchNodeDescWithRareName(nodeRareName);

        return null;
    }


    /// 临时的接口 后面干掉
    //////////////////////////////////////////////////////////////////////////

    bool _IsRootOK()
    {
        if(null != m_AvatarRootDesc)
        {
            return null != m_AvatarRootDesc.m_MeshObject;
        }

        return false;
    }

    public Bounds boundingBox
    {
        get { return m_AvatarBBox; }
    }

    public GameObject[] suitPartModel
    {
        get
        {
            return m_RenderMeshList.ToArray();
        }
    }

    public bool airMode
    {
        get {return m_AirMode; }
        set
        {
            m_AirMode = value;
            if (null != m_Attachment)
                m_Attachment.airMode = m_AirMode;
        }
    }

    public void SetSkeletonRoot(GameObject root)
    {
        m_SkeletonRoot = root;
    }

    bool m_AirMode = false;

    protected class GeAvatarDesc
    {
        public GeAvatarDesc(GeAvatarChannel channel)
        {
            m_Asset.m_AssetObj = null;
            m_Asset.m_AssetPath = "";
            m_Channel = channel;
        }
        
        public uint m_AsyncRequest = AssetLoader.INVILID_HANDLE;
        public DAssetObject m_Asset;
        public GameObject m_MeshObject = null;
        public GameObject m_SkeletonRoot = null;
        public SkinnedMeshRenderer[] m_MeshRendererList = null;
        public GeAvatarAttachment m_AvatarAttachment = null;
        public bool m_HasRemapSkeleton = false;
        public int m_OriginLayer = 0;
        public Transform m_RendObjRoot = null;
        public GeAvatarChannel m_Channel;
        public DRecolorData m_DRecolorData = null;
        public bool m_IsRecolor = false;
    }

    Dictionary<int, DAssetObject> savedInitAvatar = new Dictionary<int, DAssetObject>();

    bool m_IsAsyncLoad = false;
    bool m_bUsePool = true;
    int m_LoadingCnt = -1;

    /// GeAvatar
    protected readonly GeAvatarDesc m_AvatarRootDesc = null;
    protected readonly List<GeAvatarDesc> m_AvatarDescList;
    protected readonly GeAvatarDesc m_SimpleAvatarDesc = null;
    protected readonly List<GameObject> m_RenderMeshList = new List<GameObject>();
    
    protected GameObject m_SkeletonRoot = null;
    protected Transform[] m_AvatarSkeleton = null;

    protected int m_RenderLayer = 0;
    protected GeActorEx.DisplayMode m_DisplayMode = GeActorEx.DisplayMode.Normal;
    protected bool m_IsFading = false;
    protected float m_FadeTimer = 0.0f;
    protected const float m_FadingTimeLength = 0.5f;
    protected MaterialPropertyBlock block;
    protected MaterialPropertyBlock MatBlock 
    { 
        get 
        { 
            if (block == null) 
                block = new MaterialPropertyBlock(); 
            return block; 
        } 
    }

    protected GeAnimationManager m_Animation = new GeAnimationManager();
    protected GeAttachManager m_Attachment = new GeAttachManager();

    protected Bounds m_AvatarBBox;
    protected int m_professionalId = 0;

    // 是否启用AvatarPart在小包中没有时，用默认时装。GeActor开启，UI不开。
    protected readonly bool m_EnableAvatarPartFallback;  

    public bool EnableAvatarPartFallback
    {
        get { return m_EnableAvatarPartFallback; }
    }

    protected int m_ActorOccupation = -1;
    public int ActorOccupation
    {
        get { return m_ActorOccupation; }
        set { m_ActorOccupation = value; }
    }

    public bool IsAvatarPartFallbackEnabled()
    {
        return EnableAvatarPartFallback && GeAvatarFallback.IsAvatarPartFallbackEnabled();
    }

    private OnAvatarLoaded m_OnLoad = null;
    #region AsyncCommand

    struct ChangeActionCommand
    {
        public ChangeActionCommand(string action, float speed, bool loop = false, float offset = 0.0f)
        {
            strAction = action;
            fSpeed = speed;
            bLoop = loop;
            bVilad = true;
            fOffset = offset;
        }

        public string strAction;
        public float fSpeed;
        public bool bLoop;
        public bool bVilad;
        public float fOffset;
    }
    ChangeActionCommand changeActionCommand = new ChangeActionCommand();

    struct ChangeAvatarCommand
    {
        public ChangeAvatarCommand(GeAvatarChannel channel,DAssetObject asset,bool asyncLoad,bool highPriority,int prodId, string recolorDataPath)
        {
            eChannel = channel;
            sAsset = asset;
            bIsAsyncLoad = asyncLoad;
            bHighPriority = highPriority;
            bVilad = true;
            prodid = prodId;
            m_RecolorDataPath = recolorDataPath;

        }

        public GeAvatarChannel eChannel;
        public DAssetObject sAsset;
        public bool bIsAsyncLoad;
        public bool bHighPriority;
        public bool bVilad;
        public int prodid;
        public string m_RecolorDataPath;
    }
    ChangeAvatarCommand[] changeAvatarCommand  = new ChangeAvatarCommand[(int)GeAvatarChannel.MaxChannelNum];

    struct DisplayModeChangeCmd
    {
        public DisplayModeChangeCmd(GeActorEx.DisplayMode targetDisplayMode, bool bValid)
        {
            this.targetDisplayMode = targetDisplayMode;
            this.valid = bValid;
        }

        public GeActorEx.DisplayMode targetDisplayMode;
        public bool valid;
    }
    DisplayModeChangeCmd displayModeChangeCmd = new DisplayModeChangeCmd(GeActorEx.DisplayMode.Normal, false);

    struct ChangeLayerCommand
    {
        public ChangeLayerCommand(int layer)
        {
            nLayer = layer;
            bVilad = true;
        }

        public int nLayer;
        public bool bVilad;
    }
    ChangeLayerCommand changeLayerCommand = new ChangeLayerCommand();

    protected void _ResetAsyncCommads()
    {
        changeActionCommand.bVilad = false;
        for (int i = 0, icnt = (int)GeAvatarChannel.MaxChannelNum; i < icnt; ++i)
            changeAvatarCommand[i].bVilad = false;
        changeLayerCommand.bVilad = false;
    }

    #endregion

}
