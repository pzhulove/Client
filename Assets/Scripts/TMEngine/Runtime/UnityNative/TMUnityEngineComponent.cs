using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Tenmove.Runtime.Unity
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Tenmove/EngineRoot")]
    public class TMUnityEngineComponent : TMModuleComponent
    {
        protected sealed override void Awake()
        {
            base.Awake();
            GameObject.DontDestroyOnLoad(gameObject);

            FileArchiveAccessor.SetFileArchivePath(Application.dataPath,
                Application.streamingAssetsPath, Application.persistentDataPath);

            Debugger.SetLogLevel((DebugLevel)EngineConfig.logLevel);
        }

        private void Start()
        {
            int assetLoadAgentCount = 8;
            int currentGraphicLevel = GeGraphicSetting.instance.GetGraphicLevel();
            assetLoadAgentCount = EngineConfig.GetAgentCountByGraphicLevel(currentGraphicLevel);

            TMAssetModuleComponent assetModule = gameObject.GetComponentInChildren<TMAssetModuleComponent>();
            if (null != assetModule)
                assetModule.SetAssetLoadAgentCount(assetLoadAgentCount, assetLoadAgentCount >> 1);

            Log2File.Path = Utility.Path.Combine(Application.streamingAssetsPath, "BattleAssetLoad.txt");
        }

        private void Update()
        {
            ModuleManager.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }


    }
}

