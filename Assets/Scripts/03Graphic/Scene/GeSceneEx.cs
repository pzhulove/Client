using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using ProtoTable;

public partial class GeSceneEx
{

    public static Vector4 EntityPlane = new Vector4(0, 1, 0, 0.03f);

    #region 角色管理
    protected class GeActorManager
    {
        protected List<GeActorEx> m_ActorList = new List<GeActorEx>();
        protected bool bIsActorListDirty = false;
        public void DoBackToFront()
        {
#if !LOGIC_SERVER
            for (int i = 0; i < m_ActorList.Count; i++)
            {
                var actor = m_ActorList[i];
                if (!actor.CanRemove())
                {
                    actor.DoBackToFront();
                }
            }
#endif
        }
        public GeActorEx AddActor(int nResID,GameObject entityRoot, GeSceneEx scene,int iUnitId,bool isBattleActor = true,bool usePool = true, bool useCube=false, bool needBackCreate = true)
        {
            GeActorEx newActor = new GeActorEx();
            if (newActor.Create(nResID, entityRoot, scene, iUnitId, isBattleActor, usePool, useCube, needBackCreate))
            {
                m_ActorList.Add(newActor);
                return newActor;
            }

            return null;
        }

        public GeActorEx AddActorAync(int nResID, GameObject entityRoot, GeSceneEx scene, int iUnitId, bool isBattleActor = true, bool usePool = true, PosLoadGeActorEx load = null)
        {
            GeActorEx newActor = new GeActorEx();
            newActor.CreateAsyncForTownNPC(nResID, entityRoot, scene, iUnitId, isBattleActor, usePool, load);
            m_ActorList.Add(newActor);
            return newActor;
        }

        public GeActorEx AddActorAyncEx(int nResID, GameObject entityRoot, GeSceneEx scene, int iUnitId, bool isBattleActor = true, bool usePool = true, PosLoadGeActorEx load = null)
        {
            GeActorEx newActor = new GeActorEx();

            if (null == entityRoot)
            {
                Logger.LogError("[GeActorEx] Entity root can not be null!");
                return null;
            }

            if (null == scene)
            {
                Logger.LogError("[GeActorEx] Entity scene can not be null!");
                return null;
            }
            
            if (newActor.CreateAsync(nResID, entityRoot, scene, iUnitId, load, isBattleActor, usePool))
            {
                m_ActorList.Add(newActor);
                return newActor;
            }

            return null;
        }

        public void RemoveActor(GeActorEx actor)
        {
            actor.Remove();
            bIsActorListDirty = true;
        }
        public void ClearAll()
        {
            m_ActorList.RemoveAll(
                a =>
                {
                    a.Destroy();
                    return true;
                });
        }

        public void Update()
        {
            if (bIsActorListDirty)
            {
                m_ActorList.RemoveAll(
                    a =>
                    {
                        if (a.CanRemove())
                        {
                            a.Destroy();
                            return true;
                        }
                        else
                            return false;
                    });

                bIsActorListDirty = false;
            }
        }

        public GeActorEx[] actorList
        {
            get { return m_ActorList.ToArray(); }
        }
    }
    #endregion

#if !LOGIC_SERVER
    public void SetDoorData(List<BeRegionBase> doorData)
    {
#if UNITY_EDITOR
        if (m_SceneLogicRoot == null) return;
        proxy = m_SceneLogicRoot.GetComponent<GeBlockDataAuxDrawer>();
        if (proxy != null)
        {
        proxy.SetDoorData(doorData);
        }
#endif
    }
    #region 私有
    protected bool _InitScene(ISceneData sceneData, bool isChiji = false)
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeSceneEx._InitScene"))
        {
#endif
		if (null != sceneData)
		{
			m_LevelData = sceneData;

            _loadSceneLogicRoot();

            _loadSceneRoot();
            _loadActorRoot();
            
            _LoadSceneUIRoot(m_SceneLogicRoot);

            _updateGeCamera();
            
            _loadInstance(isChiji);

#if UNITY_EDITOR
            if (Global.Settings.debugDrawBlock)
                proxy = m_SceneLogicRoot.AddComponent<GeBlockDataAuxDrawer>();

            if (null != proxy)
            {
                proxy.RefreshBlockData(sceneData, sceneData.GetRawBlockLayer());
                proxy.RefreshGrassData(sceneData, sceneData.GetRawGrassLayer());
                proxy.RefreshEcosystemData(sceneData, sceneData.GetRawEcosystemLayer());
                proxy.RefreshEventAreaData(sceneData, sceneData.GetRawEventAreaLayer());
            }
#endif

            if (null != m_SceneRoot)
            {
                m_SceneRoot.AddComponent<GameClient.ComLifeCycleDebug>();
            }

            if (null != m_SceneActorRoot)
            {
                m_SceneActorRoot.AddComponent<GameClient.ComLifeCycleDebug>();
            }

			Logger.LogFormat("Load scene \"{0} succeed\"!", m_LevelData.GetName());

			return true;
		}
		else
		{
			Logger.LogError("levelData is nil");
		}

        return false;
#if ENABLE_PROFILER
        }
#endif
    }
    private void _loadInstance(bool isChiji = false)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeSceneEx._loadInstance"))
        {
#endif
        // load the prefab
        // TODO fix the m_levelData._prefab -> m_levelData._prefab path
        if (isChiji)
        {
            m_SceneObject = ManualPoolCollector.instance.GetGameObject(m_LevelData.GetPrefabPath());
        }
        else
        {
            m_SceneObject = AssetLoader.instance.LoadResAsGameObject(m_LevelData.GetPrefabPath());
        }

        if (m_SceneObject == null)
        {
            var dSceneData = m_LevelData as DSceneData;
            if (dSceneData != null)
                Logger.LogErrorFormat("找不到场景 请策划修改配置 name:{0}   path:{1}", dSceneData.name, m_LevelData.GetPrefabPath());
        }

        _InitColorDescList(m_SceneObject);
        _InitLut(m_LevelData.GetLutTexPath());
        _InitLightmap(m_LevelData.GetLightmapTexPath(), m_LevelData.GetLightmapPosition());

		GeGraphicSetting.instance.CheckComponent(m_SceneObject);

        m_SceneObject.AddComponent<GameClient.ComLifeCycleDebug>();

        _NormalizeSceneWaterLevel(m_SceneObject);

        Battle.GeUtility.AttachTo(m_SceneObject, m_SceneRoot);
        
        GePlaneShadowManager.GePlaneShadowSetting shadowSetting = new GePlaneShadowManager.GePlaneShadowSetting();
        shadowSetting.m_AttenuatePow = 0.2f;
        shadowSetting.m_ShadowColor.r = 0.10f;
        shadowSetting.m_ShadowColor.g = 0.15f;
        shadowSetting.m_ShadowColor.b = 0.11f;
        shadowSetting.m_ShadowColor.a = 3.5f;
        shadowSetting.m_ShadowPlane = EntityPlane;
        GePlaneShadowManager.instance.SetShadowSetting(shadowSetting);


        // 没有在用了
        //GeMeshRenderManager.GetInstance().Init(m_MainCamera.GetCamera().gameObject);
        //GeMeshRenderManager.GetInstance().AddMeshObject(m_SceneObject);

        //Test
        _InitRenderSetting();
#if ENABLE_PROFILER
        }
#endif
    }

    private void _loadSceneLogicRoot()
    {
        if (m_SceneLogicRoot == null)
        {
            m_SceneLogicRoot = new GameObject("SceneLogicRoot");
        }

        _updateSceneLogicRoot();
    }

    private void _updateSceneLogicRoot()
    {
		if (m_SceneLogicRoot != null && m_LevelData != null)
			m_SceneLogicRoot.transform.localPosition = m_LevelData.GetLogicPos();
	}
	private void _unloadSceneLogicRoot()
	{
		if (m_SceneLogicRoot != null)
		{
			GameObject.Destroy(m_SceneLogicRoot);
			m_SceneLogicRoot = null;
		}
	}
	private class CreateGeActorExData
	{
		public int nResID { get; private set; }
		public int iUnitId { get; private set; }
		public int height { get; private set; }
		public bool isSceneObj { get; private set; }

        public bool isNeedChangeMaterial { get; private set; }
        public bool usePool { get; private set; } 

        public CreateGeActorExData(int _nResID, int _iUintId, int _height, bool _isSceneObj, bool _isNeedChangeMaterial, bool _usePool)
        {
            nResID  = _nResID;
            iUnitId = _iUintId;
            height = _height;
            isSceneObj = _isSceneObj;
            isNeedChangeMaterial = _isNeedChangeMaterial;
            usePool = _usePool;
        }
    }
    private void _onCreateActorAsync(GeActorEx newActorEx)
    { 
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeSceneEx._onCreateActorAsync"))
        {
#endif
#if !LOGIC_SERVER

        if (null == mCreateGeActorExData)
        {
            return;
        }

        string resName = newActorEx.GetResName();
        if (resName.Contains("Hero") || resName.Contains("Monster") || resName.Contains("NPC"))
        {
            newActorEx.PushPostLoadCommand(() =>
            {
                Vector4 entityPlane = EntityPlane;
                entityPlane.w += mCreateGeActorExData.height * 0.01f;
                newActorEx.SetEntityPlane(entityPlane);
                newActorEx.AddSimpleShadow(Vector3.one,resName);
            });
        }

        newActorEx.PushPostLoadCommand(() =>
        {
            if (mCreateGeActorExData.isSceneObj)
                GeSceneObjManager.instance.AddOccludeObject(newActorEx.renderObject);

            for (int i = 0; i < newActorEx.renderObject.Length; ++i)
                GeMeshRenderManager.GetInstance().AddMeshObject(newActorEx.renderObject[i]);
        });

        if (newActorEx == null)
        {
            Logger.LogErrorFormat("CreateActor is nil {0}, {1}", mCreateGeActorExData.nResID, mCreateGeActorExData.iUnitId);
        }
        else
        {
            newActorEx.hpBarManager = mHPManager;
            newActorEx.stateBarManager = mStateManager;
        }
#endif
        //return newActorEx;
#if ENABLE_PROFILER
        }
#endif
    }

    protected void _InitRenderSetting()
    {
        RenderSettings.fog = false;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight =  new Color(0.15f,0.18f,0.15f,1.0f);
        RenderSettings.ambientIntensity = 1.0f;
    }

    protected void _InitLut(string lutTexPath)
    {
        if(string.IsNullOrEmpty(lutTexPath))
        {
            Shader.DisableKeyword("COLOR_LUT");
        }
        else
        {
            AssetLoader.LoadAsset(lutTexPath, typeof(Texture2D), null, _OnLutLoaded);
        }
    }
    protected void _InitLightmap(string lightmapTexPath, Vector4 lightmapPosition)
    {
        if(string.IsNullOrEmpty(lightmapTexPath))
        {
            Shader.DisableKeyword("LIGHT_ENV");
        }
        else
        {
            Shader.SetGlobalVector("_LigntingMapPosition", lightmapPosition);
            AssetLoader.LoadAsset(lightmapTexPath, typeof(Texture2D), null, _OnLightmapLoaded);
        }
    }

    private void _OnLutLoaded(string path, object asset, int taskGrpID, float duration, object userData)
    {
        Texture2D lut = asset as Texture2D;
        if(lut == null)
        {
            Logger.LogErrorFormat("Load asset with path '{0}' has failed!", path);
            Shader.DisableKeyword("COLOR_LUT");
            return;
        }

        Shader.EnableKeyword("COLOR_LUT");
        Shader.SetGlobalTexture("_LutTexture", lut);
    }

    private void _OnLightmapLoaded(string path, object asset, int taskGrpID, float duration, object userData)
    {
        Texture2D lightmap = asset as Texture2D;
        if (lightmap == null)
        {
            Logger.LogErrorFormat("Load asset with path '{0}' has failed!", path);
            Shader.DisableKeyword("LIGHT_ENV");
            return;
        }

        Shader.EnableKeyword("LIGHT_ENV");
        Shader.SetGlobalTexture("_SrcTex", lightmap);
    }

    public void _loadActorRoot()
    {
        if (m_ActorRoot == null)
        {
            m_ActorRoot = new GameObject();
            m_ActorRoot.name = "ActorRoot";
        }
        m_ActorRoot.transform.SetParent(m_SceneLogicRoot.transform, false);

        if (m_SceneActorRoot == null)
        {
            m_SceneActorRoot = new GameObject();
            m_SceneActorRoot.name = "SceneActorRoot";
            Battle.GeUtility.AttachTo(m_SceneActorRoot, m_ActorRoot);
        }

        _updateActorRoot();
    }

    public void _updateActorRoot()
    {

    }
    
    public void _unloadActorRoot()
    {
        if (m_ActorRoot != null)
        {
            GameObject.Destroy(m_ActorRoot);
            m_ActorRoot = null;
        }
    }



    public void _updateGeCamera()
    {
        //m_MainCamera.InitCamera(sceneData._CameraSize, sceneData._CameraNearClip, sceneData._CameraFarClip, sceneData._CameraPersp);
        //m_MainCamera.GetController().SetMotorLimit(Global.Settings.cameraInRange.x, 0, Global.Settings.cameraInRange.y);
        //m_MainCamera.GetController().SetLimitRange(sceneData._CameraXRange.x, 0 , sceneData._CameraZRange.x, sceneData._CameraXRange.y, 0, sceneData._CameraZRange.y);

		m_MainCamera.InitCamera(
			m_LevelData.GetCameraSize(),
			m_LevelData.GetCameraNearClip(),
			m_LevelData.GetCameraFarClip(),
			m_LevelData.IsCameraPersp());

		m_MainCamera.GetController().SetMotorLimit(
			Global.Settings.cameraInRange.x, 
			0, 
			Global.Settings.cameraInRange.y);

		m_MainCamera.GetController().SetLimitRange(
			m_LevelData.GetCameraXRange().x, 0,
			m_LevelData.GetCameraZRange().x, 
			m_LevelData.GetCameraXRange().y, 0,
			m_LevelData.GetCameraZRange().y);
    }

    public void _loadSceneRoot()
    {
        if (m_SceneRoot == null)
        {
            m_SceneRoot = new GameObject("SceneRoot");
        }

        m_SceneRoot.transform.SetParent(m_SceneLogicRoot.transform, false);

        _updateSceneRoot();
    }

    public void _updateSceneRoot()
    {
		if (m_SceneRoot != null && m_LevelData != null)
		{
			m_SceneRoot.transform.localPosition = m_LevelData.GetScenePostion();
			m_SceneRoot.transform.localScale = new Vector3(m_LevelData.GetSceneUScale(), m_LevelData.GetSceneUScale(),
				m_LevelData.GetSceneUScale());
        }

        //To Do: wang chong 测试调整角色光照
        RenderSettings.ambientMode = RenderSettings.ambientMode;
        RenderSettings.ambientLight = new  Color32(105,120,105,255);
    }

    public void _unloadSceneRoot()
    {
        if (m_SceneRoot != null)
        {
            Logger.Log("Unload scene root!");
            GameObject.Destroy(m_SceneRoot);
            m_SceneRoot = null;
        }
    }
    
    public void SetActive(bool flag)
    {
        if (m_SceneLogicRoot != null)
        {
            m_SceneLogicRoot.SetActive(flag);
        }
    }
    private CreateGeActorExData mCreateGeActorExData = null;
	public EffectsFrames DUMMY_HIT_EFF_FRAME = new EffectsFrames();
    #endregion

#endif
    #region 方法
#if !LOGIC_SERVER
    public GeSceneEx()
    {
        m_MainCamera = GameFrameWork.instance.MainCamera;
    }

	public bool LoadScene(ISceneData sceneData,bool needColorDyer = true, bool isChiji = false)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeSceneEx.LoadScene"))
        {
#endif
		m_EnableColorDyer = needColorDyer;
        return _InitScene(sceneData, isChiji);
#if ENABLE_PROFILER
        }
#endif
    }

    public void UnloadScene(bool a_bNeedGC = true, bool isChiji = false)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeSceneEx.UnloadScene"))
        {
#endif
		GeEffectEx.ClearDefaultTimeMap();
		
        GeSceneObjManager.instance.ClearAll();
        GeMeshRenderManager.instance.Deinit();
        GePlaneShadowManager.instance.ClearAll();
		GeSimpleShadowManager.instance.ClearAll();
		AudioManager.instance.ClearPreloadSound();

        _ClearSceneDescList();

        if(null != m_ActorManager)
        {
            m_ActorManager.ClearAll();
            m_ActorManager = null;
        }

        if (null != m_EffectManager)
        {
            m_EffectManager.Deinit();
            m_EffectManager = null;
        }

		//放在EffectManager后面 避免特效池子清空以后又因为EffectManager的清理往里面塞东西
        GeEffectPool.GetInstance().ClearAll();

        if (specialSceneManager != null)
        {
            specialSceneManager.Deinit();
            specialSceneManager = null;
        }
        if (isChiji)
        {
            ManualPoolCollector.instance.Recycle(m_SceneObject);
        }
        else
        {
            GameObject.Destroy(m_SceneObject);
        }
        m_SceneObject = null;

        _unloadActorRoot();
        
        _UnLoadSceneUIRoot();

        _unloadSceneRoot();
        _unloadSceneLogicRoot();

        if (mHPManager != null)
        {
            mHPManager.Unload();
            mHPManager = null;
        }

        if (mStateManager != null)
        {
            mStateManager.Unload();
            mStateManager = null;
        }

        Logger.LogFormat("Unload scene \"{0}\"!", m_LevelData.GetName());
        m_LevelData = null;

        if (a_bNeedGC)
        {
            AssetGabageCollector.instance.ClearUnusedAsset();
        }

        if (m_GeSpecialSceneEx != null)
        {
            m_GeSpecialSceneEx = null;
        }
#if ENABLE_PROFILER
        }
#endif
    }





    public  void initScrollScript()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeSceneEx.initScrollScript"))
        {
#endif
        if(m_SceneObject == null)
        {
            return;
        }

        XCameraScrollScript[] list = m_SceneObject.GetComponentsInChildren<XCameraScrollScript>();
    
        for(int i = 0; i < list.Length; ++i)
        {
            list[i].ForceUpdate();
        }
#if ENABLE_PROFILER
        }
#endif
    }



    public void ReloadSceneWithSameScene(ISceneData sceneData)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeSceneEx.ReloadSceneWithSameScene"))
        {
#endif
        if (null != sceneData)
        {
            bool flag = false;

            if (m_LevelData.GetPrefabPath() != sceneData.GetPrefabPath())
            {
                _unloadSceneRoot();
                _loadSceneRoot();

                flag = true;
            }

            m_LevelData = sceneData;

#if UNITY_EDITOR
            if (null != proxy)
                proxy.RefreshBlockData(sceneData, sceneData.GetRawBlockLayer());
#endif

            _updateSceneLogicRoot();
            _updateSceneRoot();

            //_unloadActorRoot();
            //_loadActorRoot();

            _updateGeCamera();

            if (flag)
            {
                _loadInstance();
            }
            else
            {
                _InitRenderSetting();
            }
        }

        else 
        {
            Logger.LogError("level data is nil");
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public GeEffectEx CreateEffect(int effectInfoId, Vec3 initPos, bool faceLeft = false, float fTime = 0, float fSpeed = 1f, EffectTimeType timeType = EffectTimeType.SYNC_ANIMATION)
    {
        var data = TableManager.GetInstance().GetTableItem<EffectInfoTable>(effectInfoId);
        if (data != null)
        {
            EffectsFrames effectInfo = DUMMY_HIT_EFF_FRAME;
            effectInfo.localPosition = new Vector3(0, 0, 0);
            effectInfo.localRotation = Quaternion.identity;
            effectInfo.timetype = timeType;

            DAssetObject asset;

            asset.m_AssetObj = null;
            asset.m_AssetPath = data.Path;

            var time = data.Duration / 1000f;
            if (fTime > 0)
                time = fTime;
            var scale = data.Scale / 1000f;
            if (data.Scale == 0)
                scale = 1f;

            var effect = CreateEffect(asset, effectInfo, time, initPos, scale, fSpeed, data.Loop, faceLeft);
            return effect;
        }
        return null;
    }

    public GeEffectEx CreateEffect(string effectPath, float fTime, Vec3 initPos, float initScale = 1.0f, float fSpeed = 1.0f, bool isLoop = false, bool faceLeft = false)
    {
		EffectsFrames effectInfo = DUMMY_HIT_EFF_FRAME;
        effectInfo.localPosition = new Vector3(0, 0, 0);
		effectInfo.localRotation = Quaternion.identity;

        DAssetObject asset;

        asset.m_AssetObj = null;
        asset.m_AssetPath = effectPath;

        return CreateEffect(asset, effectInfo, fTime, initPos, initScale, fSpeed, isLoop, faceLeft);
    }

    public GeEffectEx CreateEffect(DAssetObject effectRes,EffectsFrames info,float fTime,Vec3 initPos,float initScale = 1.0f,float fSpeed = 1.0f,bool isLoop = false,bool faceLeft = false)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeSceneEx.CreateEffect"))
        {
#endif
            if (null != effectRes.m_AssetObj || (null != effectRes.m_AssetPath && "" != effectRes.m_AssetPath))
            {
                Vector3 graphicPos = new Vector3(initPos.x, initPos.z, initPos.y);
                GeEffectEx newEffect = m_EffectManager.AddEffect(effectRes, info, fTime, graphicPos, null, faceLeft);
                if (null != newEffect)
                {    
                    //newEffect.SetPosition(initPos, faceLeft);

                    bool bLoop = false;
                    if (info.timetype == EffectTimeType.GLOBAL_ANIMATION)
                        bLoop = true;
                    else
                    {
                        if (newEffect.GetDefaultTimeLen() < fTime)
                            bLoop = true;
                    }

                    newEffect.SetSpeed(fSpeed);
                    newEffect.Play(bLoop || isLoop);
                    return newEffect;
                }
                else
                    Logger.LogWarningFormat("Create effect [{0}] has failed!", effectRes.m_AssetPath);
            }
            else
                Logger.LogWarning("Effect resource path can not be null!");

            return null;

#if ENABLE_PROFILER
        }
#endif
    }

    public void DestroyEffect(GeEffectEx effect)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeSceneEx.DestroyEffect"))
        {
#endif
        GeEffectType effType = GeEffectType.EffectMultiple;
        if (EffectTimeType.GLOBAL_ANIMATION == effect.GetTimeType())
            effType = GeEffectType.EffectUnique;
        m_EffectManager.RemoveEffect(effect, effType);
#if ENABLE_PROFILER
        }
#endif
    }

    public GeActorEx CreateActorAsync(int nResID, int iUnitId = 0, int height = 0, bool isSceneObj = false, bool isBattleActor = true, bool usePool = true)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeSceneEx.CreateActorAsync"))
        {
#endif
        mCreateGeActorExData = new CreateGeActorExData(nResID, iUnitId, height, isSceneObj, isBattleActor, usePool);
        return m_ActorManager.AddActorAync(nResID, isSceneObj ? m_SceneActorRoot : m_ActorRoot, this, iUnitId, isBattleActor, usePool, _onCreateActorAsync);

#if ENABLE_PROFILER
        }
#endif
    }

    public GeActorEx CreateActorAsyncEx(int nResID, int iUnitId = 0, int height = 0, bool isSceneObj = false, bool isBattleActor = true, bool usePool = true, PosLoadGeActorEx load = null)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeSceneEx.CreateActorAsyncEx"))
        {
#endif
        GeActorEx newActorEx = m_ActorManager.AddActorAyncEx(nResID, isSceneObj ? m_SceneActorRoot : m_ActorRoot, this, iUnitId, isBattleActor, usePool, load);
        if(null != newActorEx)
        {
            Vector4 entityPlane = EntityPlane;
            entityPlane.w += height * 0.01f;
            newActorEx.SetEntityPlane(entityPlane);
            newActorEx.AddSimpleShadow(Vector3.one, newActorEx.GetResName());

            newActorEx.PushPostLoadCommand(() =>
            {
                if (isSceneObj)
                    GeSceneObjManager.instance.AddOccludeObject(newActorEx.renderObject);

                for (int i = 0; i < newActorEx.renderObject.Length; ++i)
                    GeMeshRenderManager.GetInstance().AddMeshObject(newActorEx.renderObject[i]);
            });
            newActorEx.hpBarManager = mHPManager;
            newActorEx.stateBarManager = mStateManager;
        }

        return newActorEx;
#if ENABLE_PROFILER
        }
#endif
    }

    public GeActorEx CreateActor(int nResID, int iUnitId = 0, int height = 0, bool isSceneObj = false, bool isBattleActor = true, bool usePool = true, bool useCube = false,bool needBackCreate = true)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeSceneEx.CreateActor"))
        {
#endif
		GeActorEx newActorEx = m_ActorManager.AddActor(nResID, isSceneObj? m_SceneActorRoot : m_ActorRoot, this,iUnitId, isBattleActor, usePool, useCube, needBackCreate);

        if (newActorEx == null)
        {
            var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(nResID);
            if(null == resData)
                Logger.LogErrorFormat("Create actor has failed with resID:{0} [Can not find table data with this ID]!", nResID);
            else
                Logger.LogErrorFormat("Create actor has failed with res path:'{0}'!", resData.ModelPath);

            return null;
        }

        string resName = newActorEx.GetResName();
		if (resName.Contains("Hero") || resName.Contains("Monster") || resName.Contains("NPC")/* || resName.Contains("pet", System.StringComparison.OrdinalIgnoreCase)*/)
        {
            newActorEx.PushPostLoadCommand(() =>
            {
                Vector4 entityPlane = EntityPlane;
                entityPlane.w += height * 0.01f;
                newActorEx.SetEntityPlane(entityPlane);
                newActorEx.AddSimpleShadow(Vector3.one,resName);
            });
        }

        newActorEx.PushPostLoadCommand(() =>
        {
            if (isSceneObj)
                GeSceneObjManager.instance.AddOccludeObject(newActorEx.renderObject);

            for (int i = 0; i < newActorEx.renderObject.Length; ++i)
                GeMeshRenderManager.GetInstance().AddMeshObject(newActorEx.renderObject[i]);
        });

        if (newActorEx == null)
        {
            Logger.LogErrorFormat("CreateActor is nil {0}, {1}", nResID, iUnitId);
        }
        else
        {
            newActorEx.hpBarManager = mHPManager;
            newActorEx.stateBarManager = mStateManager;
        }

        return newActorEx;
#if ENABLE_PROFILER
        }
#endif
    }

    public void DestroyActor(GeActorEx actor)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeSceneEx.DestroyActor"))
        {
#endif
        if(null != actor)
        {
            GePlaneShadowManager.instance.RemoveShadowObject(actor.renderObject);
			GeSimpleShadowManager.instance.RemoveShadowObject(actor.renderObject);
            if(null != actor.renderObject)
            {
                for (int i = 0; i < actor.renderObject.Length; ++i)
                    GeSceneObjManager.instance.RemoveOccludeObject(actor.renderObject[i]);
            }
        }

        m_ActorManager.RemoveActor(actor);
#if ENABLE_PROFILER
        }
#endif
    }

	public void RecycleActor(GeActorEx actor)
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeSceneEx.RecycleActor"))
        {
#endif
		actor.Clear();
		actor.ReleaseForProjectile();

		//m_reusedActorList.Enqueue(actor);
#if ENABLE_PROFILER
        }
#endif
	}

    public GeCamera GetCamera()
    {
        return m_MainCamera;
    } 

    public void AttachCameraTo(GeEntity actor)
    {
        GetCamera().GetController().AttachTo(actor, 
            m_LevelData.GetCameraLookHeight(), 
            m_LevelData.GetCameraAngle(),
            m_LevelData.GetCameraDistance());
        GeSceneObjManager.instance.SetRefCamera(m_MainCamera);
        GeSceneObjManager.instance.SetFocusEntity(actor.GetEntityNode(GeEntity.GeEntityNodeType.Actor));
        initScrollScript();
    }

    public void Update(int delta)
    {
        _UpdateBlendSceneColor(delta / 1000.0f);
        m_MainCamera.Update(delta);
        m_EffectManager.Update(delta,GeEffectType.EffectMultiple);
        m_EffectManager.Update(delta, GeEffectType.EffectUnique);
        specialSceneManager.Update(delta);
        GePlaneShadowManager.instance.Update();
		GeSimpleShadowManager.instance.Update();
        GeSceneObjManager.instance.Update();
        m_ActorManager.Update();

        if (null != mHPManager)
        {
            mHPManager.Update(delta);
        }

        if (null != mStateManager)
        {
            if (!isPauseLogic)
            {
                mStateManager.Update(delta);
            }
        }

        if (m_GeSpecialSceneEx != null)
        {
            m_GeSpecialSceneEx.Update(delta);
        }
    }
   
    public void PauseLogic()
    {
        isPauseLogic = true;
    }

    public void ResumeLogic()
    {
        isPauseLogic = false;
    }

    public Vector2 WorldPosToScreenPos(Vector3 pos)
    {
        return m_MainCamera.WorldToScreenPoint(pos);
    }

    public Color GetObjectDyeColor()
    {
        if (null != m_LevelData)
            return m_LevelData.GetObjectDyeColor();

        return Color.white;
    }
    public void DoBackToFront()
    {
        if (m_ActorManager != null)
            m_ActorManager.DoBackToFront();
        if (m_EffectManager != null)
            m_EffectManager.DoBackToFront();
    }

    public void SetBlockData(ISceneData sceneData, byte[] blockData)
	{
#if UNITY_EDITOR
        if (proxy != null)
			proxy.RefreshBlockData(sceneData, blockData);
#endif
    }
		
	public GeEffectManager GetEffectManager()
	{
		return m_EffectManager;
	}
    // 加载魔幻场景
    public void LoadMagicScene(string _scenePath, int _time, int _reversTime, float maxValue)
    {
        specialSceneManager.LoadMagicScene(_scenePath,_time,_reversTime, maxValue,this);
    }
    
    public void ReverseMaterialSpecialScene()
    {
        specialSceneManager.ReverseScene();
    }

	public GameObject GetSceneRoot()
	{
		return m_SceneRoot;
	}

    public GameObject GetActorRoot()
    {
        return m_ActorRoot;
    }

	public GameObject GetSceneActorRoot()
	{
		return m_SceneActorRoot;
	}

    public GameObject GetSceneObject()
    {
        return m_SceneObject;
    }

    private struct SceneColorDesc
    {
        public SceneColorDesc(Renderer render,int instanceID,Color col)
        {
            m_RenderInstanceID = instanceID;
            m_OriginColor = col;
            m_Renderer = render;
        }

        public Color m_OriginColor;
        public readonly int m_RenderInstanceID;
        public readonly Renderer m_Renderer;
    }

    private struct SceneColorTimeline
    {
        public Color m_BlendColor ;
        public float m_TimeTotal;
        public float m_TimeRemain;
        public bool m_IsRecover;
        public bool m_IsPlaying;
        public bool m_IsAlpha;
        public bool m_IsUnique;
        public SceneColorTimeline(float timeTotal, float timeRemain, bool isRecover, bool isPlaying, bool isAlpha,Color blendColor,bool IsUnique)
        {
            m_TimeTotal = timeTotal;
            m_TimeRemain = timeRemain;
            m_IsRecover = isRecover;
            m_IsPlaying = isPlaying;
            m_IsAlpha = isAlpha;
            m_BlendColor = blendColor;
            m_IsUnique = IsUnique;
        }
    }

    MaterialPropertyBlock m_ColorBlendBlock = new MaterialPropertyBlock();
    List<SceneColorDesc> m_ColorDescList = new List<SceneColorDesc>();
    List<SceneColorTimeline> m_ColorTimelines = new List<SceneColorTimeline>();

    public int BlendSceneSceneColor(Color blendColor,float time=0.0F, bool alphaEffect = false, bool IsUnique = false)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeSceneEx.BlendSceneSceneColor"))
        {
#endif
		if (!m_EnableColorDyer)
			return -1;
		
        if(time < 0)
        {
            Logger.LogWarning("Parameter 'time' can not less or equal to zero!");
            return -1;
        }

        if (IsUnique)
        {
            for (int i = 0; i < m_ColorTimelines.Count; ++i)
            {
                if (m_ColorTimelines[i].m_IsUnique)
                    return -1;
            }
        }

        float totalTime=time;
        if (time == 0)
        {
            totalTime = 0.0001f;
        }
        for (int i = 0;i < m_ColorTimelines.Count;++i)
        {
            if(!m_ColorTimelines[i].m_IsPlaying)
            {
                SceneColorTimeline tempTimeline = m_ColorTimelines[i];
                tempTimeline.m_BlendColor = blendColor;
                tempTimeline.m_IsUnique = IsUnique;
                tempTimeline.m_TimeRemain = time;
                tempTimeline.m_TimeTotal = totalTime;
                tempTimeline.m_IsRecover = false;
                tempTimeline.m_IsPlaying = true;
                tempTimeline.m_IsAlpha = alphaEffect;
                m_ColorTimelines[i] = tempTimeline;
                return i;
            }
        }

        SceneColorTimeline sceneColorTimeline = new SceneColorTimeline(totalTime, time, false, true, alphaEffect,blendColor, IsUnique);
        m_ColorTimelines.Add(sceneColorTimeline);

        return m_ColorTimelines.Count - 1;
#if ENABLE_PROFILER
        }
#endif
    }

    public void RecoverSceneColor(float time, int id)
	{
		if (!m_EnableColorDyer)
			return;
		
        if (time < 0)
        {
            Logger.LogWarning("Parameter 'time' can not less or equal to zero!");
            return;
        }
        if(id >= 0 && id < m_ColorTimelines.Count)
        {
            if(m_ColorTimelines[id].m_IsPlaying)
            {
                SceneColorTimeline tempTimeline = m_ColorTimelines[id];

                tempTimeline.m_IsRecover = true;
                tempTimeline.m_TimeRemain = time;
                tempTimeline.m_TimeTotal = time;

                m_ColorTimelines[id] = tempTimeline;
            }
        }
        else
        {
            Logger.LogWarning("Parameter 'id' does not exist!");
        }
    }
    
    public void AddToColorDescList(GameObject go)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeSceneEx.AddToColorDescList"))
        {
#endif
		if (null != go && m_EnableColorDyer)
        {
            Renderer[] ars = go.GetComponentsInChildren<Renderer>();
            for (int i = 0, icnt = ars.Length; i < icnt; ++i)
            {
                Renderer cur = ars[i];
                if (null == cur)
                    continue;

                    if (null != (cur as MeshRenderer) || null != (cur as SkinnedMeshRenderer) || null != (cur as SpriteRenderer))
                    {
                        bool alreadyExist = false;
                    int instID = cur.GetInstanceID();
                    for (int k = 0, kcnt = m_ColorDescList.Count; k < kcnt; ++k)
                    {
                        if (m_ColorDescList[k].m_RenderInstanceID == instID)
                        {
                            alreadyExist = true;
                            break;
                        }
                    }

                    if (!alreadyExist)
                    {
                        Color originColor = Color.white;
                        if (null != cur.sharedMaterial && cur.sharedMaterial.HasProperty("_DyeColor"))
                            originColor = cur.sharedMaterial.GetColor("_DyeColor");
                        
                        m_ColorDescList.Add(new SceneColorDesc(cur, instID, originColor));
                    }
                    }
                }
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public void ClearColorDesc(GameObject go)
	{		
		if (!m_EnableColorDyer)
			return;
        if (null != go)
        {
            Renderer[] ars = go.GetComponentsInChildren<Renderer>();
            for (int i = 0, icnt = ars.Length; i < icnt; ++i)
            {
                Renderer cur = ars[i];
                if (null == cur)
                    continue;

                if (null != (cur as MeshRenderer) || null != (cur as SkinnedMeshRenderer))
                {
                    m_ColorDescList.RemoveAll(x =>
                    {
                        return x.m_Renderer.GetInstanceID() == cur.GetInstanceID();
                    });
                }
            }
        }
    }

    private void _InitColorDescList(GameObject sceneObj)
    {
        _ClearSceneDescList();
        AddToColorDescList(sceneObj);
    }

    private void _ClearSceneDescList()
	{
		if (!m_EnableColorDyer)
			return;
//         for (int k = 0, kcnt = m_ColorDescList.Count; k < kcnt; ++k)
//         {
//             if (null != m_ColorDescList[k].m_Renderer)
//             {
//                 m_ColorDescList[k].m_Renderer.SetPropertyBlock(m_ColorDescList[k].m_PropertyBlock);
//             }
//         }

        m_ColorDescList.Clear();

        m_ColorTimelines.Clear();
    }

    
    private void _UpdateBlendSceneColor(float timeElapsed)
    {
        if (!m_EnableColorDyer || (m_ColorTimelines.Count == 0))
        {
            return;
        }
        Color colorWeightResult = Color.white;
        bool anyAlive = false;
        float alphaWeightResult = 1f;


        for (int i = 0; i < m_ColorTimelines.Count; ++i)
        {
            SceneColorTimeline sceneColorTimeline = m_ColorTimelines[i];
            //效果没在播放
            if (!sceneColorTimeline.m_IsPlaying)
                continue;
            anyAlive = true;
            if (sceneColorTimeline.m_TimeRemain <0)
            {
                if (sceneColorTimeline.m_IsRecover)
                {
                    //效果结束
                    sceneColorTimeline.m_IsPlaying = false;
                }
            }
            float timeRemain = sceneColorTimeline.m_TimeRemain - timeElapsed;
            timeRemain = Mathf.Clamp(timeRemain, 0, sceneColorTimeline.m_TimeTotal);
            float factor = 1 - timeRemain / sceneColorTimeline.m_TimeTotal;

            Color colorWeight = Color.white;
                //白->blendColor
            if (!sceneColorTimeline.m_IsRecover)
            {
                colorWeight = Color.Lerp(Color.white, sceneColorTimeline.m_BlendColor, factor);
            }
            //blendColor->白
            else
            {
                
                colorWeight = Color.Lerp(sceneColorTimeline.m_BlendColor, Color.white, factor);
            }

            if (sceneColorTimeline.m_IsAlpha)
            {
                float alpha = sceneColorTimeline.m_BlendColor.a;
                alphaWeightResult *= alpha;
            }
            colorWeightResult *= colorWeight;
            sceneColorTimeline.m_TimeRemain = timeRemain;
            m_ColorTimelines[i] = sceneColorTimeline;
        }

        //没有正在播放的效果
        if (!anyAlive)
            return;
        for (int i = 0, icnt = m_ColorDescList.Count; i < icnt; ++i)
        {
            if (m_ColorDescList[i].m_Renderer != null)
            {
                Color dstColor = m_ColorDescList[i].m_OriginColor * colorWeightResult;
                dstColor.a = m_ColorDescList[i].m_OriginColor.a * alphaWeightResult;
                m_ColorDescList[i].m_Renderer.GetPropertyBlock(m_ColorBlendBlock);
                m_ColorBlendBlock.SetColor("_DyeColor", dstColor);
                m_ColorDescList[i].m_Renderer.SetPropertyBlock(m_ColorBlendBlock);
            }
        }
    }

    public GameObject _GetSceneGroundObj(GameObject sceneRoot)
    {
        if (null == sceneRoot)
            return null;

        if (sceneRoot.CompareTag("Ground") /*|| (sceneRoot.name.Contains("ground") && null != sceneRoot.GetComponents<MeshRenderer>())*/)
            return sceneRoot;

        GameObject groundObj = null;
        int childNum = sceneRoot.transform.childCount;
        for (int i = 0; i < childNum; ++i)
        {
            Transform curChild = sceneRoot.transform.GetChild(i);

            groundObj = _GetSceneGroundObj(curChild.gameObject);
            if (null != groundObj)
                return groundObj;
        }

        return groundObj;
    }

    public void _NormalizeSceneWaterLevel(GameObject sceneRoot)
    {
        /// Auto adjust scene ground to zero-plane.
        GameObject ground = _GetSceneGroundObj(sceneRoot);
        if (null != ground)
        {
            Vector3 sceneRootPos = sceneRoot.transform.position;
            sceneRootPos.y -= ground.transform.position.y + 0.05f;
            sceneRoot.transform.position = sceneRootPos;


            if (Global.Settings.isDebug && Global.Settings.sceneDark)
            {
                Renderer rnd = ground.GetComponent<Renderer>();
                if (rnd != null)
                {
                    var goPlane = AssetLoader.GetInstance().LoadResAsGameObject("Scene/Plane");
                    //                 var position = ground.transform.position;
                    //                 var offset = 0.5f;
                    //                 position.z += Mathf.Abs(rnd.bounds.max.z - rnd.bounds.min.z)/2.0f + offset;
                    //                 goPlane.transform.position = position;

                    Battle.GeUtility.AttachTo(goPlane, sceneRoot, false);

                    var position = rnd.bounds.center;
                    position.z += rnd.bounds.size.z / 2.0f + 2f;
                    goPlane.transform.position = position;

                    //                 Battle.GeUtility.AttachTo(goPlane, ground, true);
                    //                 var offset = Global.Settings.startVel.x;
                    //                 var position = goPlane.transform.position;
                    //                 position.z += Mathf.Abs(rnd.bounds.max.z - rnd.bounds.min.z) / 2.0f + offset;
                    //                 goPlane.transform.position = position;


                    Logger.LogErrorFormat("gound,bound:{0} {1} ground:{2} goPlane:{3}",
                        rnd.bounds.min, rnd.bounds.max, ground.transform.localPosition, goPlane.transform.localPosition);
                }
            }

            

            //Logger.LogErrorFormat("Ground:{0}", ground.transform.position.y);
        }
    }



	public GeActorEx[] GetActorList()
	{
		return m_ActorManager.actorList;
	}

    public void DestroyAllEffect()
    {
        if (m_EffectManager != null)
        {
            m_EffectManager.ClearAll(GeEffectType.EffectUnique);
            m_EffectManager.ClearAll(GeEffectType.EffectMultiple);
            m_EffectManager.ClearAll(GeEffectType.EffectGlobal);
        }
    }
#else

    public GeSceneEx(){}
	public bool LoadScene(ISceneData sceneData,bool needColorDyer = true ,bool isChiji = false){return false;}
	public void UnloadScene(bool a_bNeedGC = true,bool isChiji = false){}
	public  void initScrollScript(){}
	public void ReloadSceneWithSameScene(ISceneData sceneData){}
	public GeEffectEx CreateEffect(string effectPath, float fTime, Vec3 initPos, float initScale = 1.0f, float fSpeed = 1.0f, bool isLoop = false, bool faceLeft = false){return null;}
	public GeEffectEx CreateEffect(DAssetObject effectRes,EffectsFrames info,float fTime,Vec3 initPos,float initScale = 1.0f,float fSpeed = 1.0f,bool isLoop = false,bool faceLeft = false){return null;}

    public GeEffectEx CreateEffect(int effectInfoId, Vec3 initPos, bool faceLeft = false, float fTime = 0,
        float fSpeed = 1f, EffectTimeType timeType = EffectTimeType.SYNC_ANIMATION)
    {
        return null;
    }

    public void DestroyEffect(GeEffectEx effect){}
    public GeActorEx AddActorAyncEx(int nResID, GameObject entityRoot, GeSceneEx scene, int iUnitId, bool isBattleActor = true, bool usePool = true, PosLoadGeActorEx load = null){return null;}
	public GeActorEx CreateActorAsync(int nResID, int iUnitId = 0, int height = 0, bool isSceneObj = false, bool isNeedChangeMaterial = true, bool usePool = true){return null; }
    public GeActorEx CreateActorAsyncEx(int nResID, int iUnitId = 0, int height = 0, bool isSceneObj = false, bool isBattleActor = true, bool usePool = true, PosLoadGeActorEx load = null) { return null; }

    public GeActorEx CreateActor(int nResID, int iUnitId = 0, int height = 0, bool isSceneObj = false, bool isNeedChangeMaterial = true, bool usePool = true, bool useCube = false)
	{
		var geactor = new GeActorEx();
		
		geactor.Create(nResID, null, this, 0);
		return geactor;
	}
	public void DestroyActor(GeActorEx actor){}
    public void RecycleActor(GeActorEx actor){}
    public GeCamera GetCamera(){return null;}
    public void AttachCameraTo(GeEntity actor){}
    public void Update(int delta){}
    public void PauseLogic(){}
    public void ResumeLogic(){}
    public Vector2 WorldPosToScreenPos(Vector3 pos){return Vector2.zero;}
    public Color GetObjectDyeColor(){return Color.white;}
    public void SetBlockData(ISceneData sceneData, byte[] blockData){}
    public GeEffectManager GetEffectManager(){return null;}
    public GameObject GetSceneRoot(){return null;}
    public GameObject GetSceneActorRoot(){return null;}
    public GameObject _GetSceneGroundObj(GameObject sceneRoot){return null;}
    public void _NormalizeSceneWaterLevel(GameObject sceneRoot){} 
    public int BlendSceneSceneColor(float darkFactor,float time) {return 0;}
    public void RecoverSceneColor(float time,int id) {}
    public void AddToColorDescList(GameObject go) {}
    public void SetActive(bool flag)
    {
    }
    public void _updateGeCamera()
    {
    }
#endif


        #endregion

        #region 成员

#if !LOGIC_SERVER
    private GeCamera m_MainCamera;

    private GameObject m_SceneLogicRoot = null;
    private GameObject m_SceneRoot = null;
    private GameObject m_ActorRoot = null;
    private GameObject m_SceneActorRoot = null;
    private GameObject m_SceneObject = null;


    private GeSpecialSceneManager specialSceneManager = new GeSpecialSceneManager();
    private GeEffectManager m_EffectManager = new GeEffectManager();
    private GeActorManager m_ActorManager = new GeActorManager();

	private Queue<GeActorEx> m_reusedActorList = new Queue<GeActorEx>();

    private GeSpecialSceneEx m_GeSpecialSceneEx;
    public GeSpecialSceneEx GeSpecialSceneEx { set { m_GeSpecialSceneEx = value; } }

	private bool m_EnableColorDyer = true;

        #region 杂项
    protected HPBarManager mHPManager = new HPBarManager();
    protected StateBarManager mStateManager = new StateBarManager();
    private bool isPauseLogic = false;

#if UNITY_EDITOR
    GeBlockDataAuxDrawer proxy = null;
#endif
        #endregion

#endif

    private ISceneData m_LevelData = null;

#endregion

    
}


