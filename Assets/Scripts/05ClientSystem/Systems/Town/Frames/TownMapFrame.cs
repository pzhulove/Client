using System;
using System.Collections.Generic;
using System.Collections;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class TownMapFrame : ClientFrame
    {
        #region ExtraUIBind
        private TouchMoveCamera2D mTouchMoveCamera2D = null;

        protected sealed override void _bindExUI()
        {
            mTouchMoveCamera2D = mBind.GetCom<TouchMoveCamera2D>("TouchMoveCamera2D");
        }

        protected sealed override void _unbindExUI()
        {
            mTouchMoveCamera2D = null;
        }
        #endregion

        [UIObject("root/targetPos")]
        GameObject m_objTargetPos;

        ComMapPlayer m_comPlayer;

        class MapSceneUnit
        {
            public int TownID
            {
                get; set;
            }

            public Dictionary<int, ComMapScene> mapScenes
            {
                get
                {
                    return mMapScene;
                }
            }

            private Dictionary<int, ComMapScene> mMapScene = new Dictionary<int, ComMapScene>();
        }

        List<MapSceneUnit> mMapSceneUnits = new List<MapSceneUnit>();


        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TownMap/Map";
        }

        protected sealed override void _OnLoadPrefabFinish()
        {
            if (GetFrameName() != "full_map")
            {
                if (null == mComClienFrame)
                {
                    mComClienFrame = frame.AddComponent<ComClientFrame>();
                }
                mComClienFrame.SetGroupTag("system");
            }

            if (mTouchMoveCamera2D != null)
            {
                //mTouchMoveCamera2D.bEnabled = (GetFrameName() == "full_map");
            }
        }

        protected sealed override void _OnOpenFrame()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                systemTown = ClientSystemManager.GetInstance().TargetSystem as ClientSystemTown;
                if (systemTown == null)
                {
                    Logger.LogError("TownMapFrame must open in town system!!");
                    return;
                }
            }

            if(BeTownPlayerMain.OnAutoMoveSuccess != null)
            {
                BeTownPlayerMain.OnAutoMoveSuccess.RemoveAllListeners();
                BeTownPlayerMain.OnAutoMoveSuccess.AddListener(_OnAutoMoveEnd);
            }

            if (BeTownPlayerMain.OnAutoMoveFail != null)
            {
                BeTownPlayerMain.OnAutoMoveFail.RemoveAllListeners();
                BeTownPlayerMain.OnAutoMoveFail.AddListener(_OnAutoMoveEnd);
            }
            
            GameObject sceneRoot = Utility.FindGameObject(frame, "root/scenes");
            if (sceneRoot == null)
            {
                return;
            }

            ComMapScene[] scenes = sceneRoot.GetComponentsInChildren<ComMapScene>(true);
            if(scenes == null)
            {
                Logger.LogError("TownMapFrame Get scenes is null");
                return;
            }

            for (int i = 0; i < scenes.Length; ++i)
            {
                ComMapScene mapScene = scenes[i];
                if (mapScene == null)
                {
                    continue;
                }

                mapScene.Initialize();

                if(mapScene.btnScene != null)
                {
                    mapScene.btnScene.onMouseClick.RemoveAllListeners();
                    mapScene.btnScene.onMouseClick.AddListener((var) =>
                    {
                        if (systemTown.MainPlayer != null)
                        {
                            if (PlayerBaseData.GetInstance().Level < mapScene.levelLimit)
                            {
                                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("town_lock_desc", mapScene.sceneName, mapScene.levelLimit));
                            }
                            else
                            {
                                Vector2 pos = Vector2.zero;
                                RectTransform rect = mapScene.GetComponent<RectTransform>();

                                if(rect != null)
                                {
                                    RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, var.pressPosition, var.enterEventCamera, out pos);
                                }

                                Vector3 vecPos = new Vector3(pos.x / mapScene.XRate, 0.0f, pos.y / mapScene.ZRate);
                                Vector3 vecTargetPos = vecPos + mapScene.offset;

                                systemTown.MainPlayer.CommandAutoMoveToTargetPos(mapScene.SceneID, vecTargetPos);
                                if (m_objTargetPos != null)
                                {
                                    m_objTargetPos.transform.SetParent(mapScene.transform, false); ;
                                    m_objTargetPos.transform.localPosition = new Vector3(pos.x, pos.y, 0);
                                    m_objTargetPos.SetActive(true);
                                }
                            }
                        }
                    });
                }

                _addMapScenes(mapScene);
            }

            m_comPlayer = Utility.FindGameObject(frame, "root/player_main").GetComponent<ComMapPlayer>();
            if (m_comPlayer != null)
            {
                m_comPlayer.Initialize();
                if (_containsIdInCurrentTown(systemTown.CurrentSceneID))
                {
                    m_comPlayer.Setup(systemTown.MainPlayer, _getMapSceneInCurrentTown(systemTown.CurrentSceneID));
                }
            }
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SceneChangedFinish, _OnSceneChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged, _OnLevelChanged);

            _onTownAreaChange(systemTown.CurrentSceneID);

            if (mTouchMoveCamera2D != null && m_comPlayer != null)
            {
                mTouchMoveCamera2D.PlayerTransform = m_comPlayer.gameObject.transform;
            }     
        }

        


        #region MapScenesOps
        private MapSceneUnit _getMapSceneUnitByTownId(int townId)
        {
            for (int i = 0; i < mMapSceneUnits.Count; ++i)
            {
                if (townId == mMapSceneUnits[i].TownID)
                {
                    return mMapSceneUnits[i];
                }
            }

            return null;
        }

        private MapSceneUnit _getMapSceneUnitBySceneId(int sceneId)
        {
            int townId = _getTownID(sceneId);
            return _getMapSceneUnitByTownId(townId);
        }

        private MapSceneUnit _addMapSceneUnitByTownId(int townId)
        {
            MapSceneUnit unit = new MapSceneUnit();
            unit.TownID = townId;
            mMapSceneUnits.Add(unit);
            return unit;
        }

        private MapSceneUnit _addMapSceneUnitBySceneId(int sceneId)
        {
            int townId = _getTownID(sceneId);
            return _addMapSceneUnitByTownId(townId);
        }

        protected void _addMapScenes(ComMapScene mapScene)
        {
            if (null == mapScene)
            {
                return;
            }

            MapSceneUnit unit = null;

            if (mapScene.JumpTownId != -1)
            {
                unit = _getMapSceneUnitByTownId(mapScene.JumpTownId);
                if (null == unit)
                {
                    unit = _addMapSceneUnitByTownId(mapScene.JumpTownId);
                }
            }
            else
            {
                unit = _getMapSceneUnitBySceneId(mapScene.SceneID);

                if (null == unit)
                {
                    unit = _addMapSceneUnitBySceneId(mapScene.SceneID);
                }
            }

            if (null == unit)
            {
                return;
            }

            unit.mapScenes.Add(mapScene.SceneID, mapScene);
        }

        protected bool _containsIdInCurrentTown(int sceneId)
        {
            Dictionary<int, ComMapScene> maps = _getCurrentTownMapScenesDics();
            if (null == maps)
            {
                return false;
            }

            return maps.ContainsKey(sceneId);
        }

        protected ComMapScene _getMapSceneInCurrentTown(int sceneId)
        {
            if (!_containsIdInCurrentTown(sceneId))
            {
                return null;
            }

            Dictionary<int, ComMapScene> maps = _getCurrentTownMapScenesDics();

            if (null == maps || !maps.ContainsKey(sceneId))
            {
                return null;
            }
            
            return maps[sceneId];
        }

        protected Dictionary<int, ComMapScene> _getCurrentTownMapScenesDics()
        {
            int townId = _getCurrentTownID();

            MapSceneUnit unit = _getMapSceneUnitByTownId(townId);
            if (null == unit)
            {
                return null;
            }

            return unit.mapScenes;
        }

        protected void _clearMapScenes()
        {
            if(mMapSceneUnits != null)
            {
                for (int i = 0; i < mMapSceneUnits.Count; ++i)
                {
                    if (mMapSceneUnits[i] != null && mMapSceneUnits[i].mapScenes != null)
                    {
                        mMapSceneUnits[i].mapScenes.Clear();
                    } 
                }

                mMapSceneUnits.Clear();
            }
        }
        #endregion

        protected sealed override void _OnCloseFrame()
        {
            if (BeTownPlayerMain.OnAutoMoveSuccess != null)
            {
                BeTownPlayerMain.OnAutoMoveSuccess.RemoveListener(_OnAutoMoveEnd);
            }

            if(BeTownPlayerMain.OnAutoMoveFail != null)
            {
                BeTownPlayerMain.OnAutoMoveFail.RemoveListener(_OnAutoMoveEnd);
            }

            if(m_comPlayer != null)
            {
                m_comPlayer = null;
            }
            
            _clearMapScenes();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SceneChangedFinish, _OnSceneChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged, _OnLevelChanged);
        }

        public void SetScale(Vector2 scale)
        {
            frame.GetComponent<RectTransform>().localScale = new Vector3(scale.x, scale.y, 1.0f);
        }

        public Vector2 GetSize()
        {
            Vector2 size = frame.GetComponent<RectTransform>().rect.size;
            Vector3 scale = frame.GetComponent<RectTransform>().localScale;

            return new Vector2(size.x * scale.x, size.y * scale.y);
        }

        public Vector2 GetPlayerMainPos()
        {
            if (m_comPlayer == null)
            {
                return Vector2.zero;
            }
            else
            {
                Vector3 pos = m_comPlayer.transform.localPosition + m_comPlayer.transform.parent.localPosition;
                Vector3 scale = frame.GetComponent<RectTransform>().localScale;
                return new Vector2(pos.x * scale.x, pos.y * scale.y);
            }
        }

        public string GetCurrentSceneName()
        {
            if (m_comPlayer != null && m_comPlayer.isValid)
            {
                return m_comPlayer.scene.sceneName;
            }
            return string.Empty;
        }

        void _OnAutoMoveEnd()
        {
            m_objTargetPos.SetActive(false);
        }

        protected void _OnSceneChanged(UIEvent uiEvent)
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                systemTown = ClientSystemManager.GetInstance().TargetSystem as ClientSystemTown;
                if (systemTown == null)
                {
                    Logger.LogError("TownMapFrame must open in town system!!");
                    return;
                }
            }

            m_comPlayer.Initialize();
            if (_containsIdInCurrentTown(systemTown.CurrentSceneID))
            {
                m_comPlayer.Setup(systemTown.MainPlayer, _getMapSceneInCurrentTown(systemTown.CurrentSceneID));
            }

            if (_hasTownIDChanged(systemTown.CurrentSceneID, systemTown.FromSceneID))
            {
                _onTownAreaChange(systemTown.CurrentSceneID);
            }

            mTouchMoveCamera2D.UpdateMapPos();
        }

        protected void _OnLevelChanged(UIEvent a_event)
        {
            var maps = _getCurrentTownMapScenesDics();
            if (null == maps)
            {
                return;
            }
            
            var iter = maps.GetEnumerator();
            while (iter.MoveNext())
            {
                ComMapScene comMapScene = iter.Current.Value;
                comMapScene.SetLock(PlayerBaseData.GetInstance().Level < comMapScene.levelLimit);
            }
        }

        /// <summary>
        /// 切换城镇
        /// </summary>
        private void _onTownAreaChange(int currentSceneId)
        {
            ComMapScene mapScene = null;
            int townId = _getCurrentTownID();
            for (int i = 0; i < mMapSceneUnits.Count; ++i)
            {
                bool isShow = (townId == mMapSceneUnits[i].TownID);

                if (null == mMapSceneUnits[i].mapScenes)
                {
                    continue;
                }

                var iter = mMapSceneUnits[i].mapScenes.GetEnumerator();

                while (iter.MoveNext())
                {
                    ComMapScene comMapScene = iter.Current.Value;
                    if(isShow && mapScene == null)
                    {
                        mapScene = comMapScene;
                    }
                    comMapScene.CustomActive(isShow);
                }
            }
            if(mapScene != null)
            {
                mapScene.LoadBackgroundImg();
            }
        }

        private bool _hasTownIDChanged(int currentSceneID, int fromSceneID)
        {
            int currentTownID = _getTownID(currentSceneID);
            if (-1 == currentSceneID)
            {
                return false;
            }

            int fromTownID = _getTownID(fromSceneID);
            if (-1 == fromTownID)
            {
                return false;
            }

            return fromTownID != currentSceneID;
        }

        private int _getCurrentTownID()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                systemTown = ClientSystemManager.GetInstance().TargetSystem as ClientSystemTown;
                if (systemTown == null)
                {
                    Logger.LogError("TownMapFrame must open in town system!!");
                    return -1;
                }
            }

            return _getTownID(systemTown.CurrentSceneID);
        }

        private int _getTownID(int sceneID)
        {
            ProtoTable.CitySceneTable cityTable = TableManager.instance.GetTableItem<ProtoTable.CitySceneTable>(sceneID);
            if (null == cityTable)
            {
                return -1;
            }

            return cityTable.TownID;
        }
    }
}
