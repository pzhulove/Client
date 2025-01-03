using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class SceneMapView : MonoBehaviour
    {
        private ClientSystemTown _systemTown;
        private int _currentSceneId;

        private List<SceneMapPathDataModel> _currentSceneMapPathDataModelList = null;

        [Space(10)] [HeaderAttribute("Close")] [Space(10)] [SerializeField]
        private Button closeButton;

        [Space(10)] [HeaderAttribute("SceneMainPlayerControl")] [Space(10)] [SerializeField]
        private SceneMainPlayerControl sceneMainPlayerControl;

        [Space(10)] [HeaderAttribute("SceneMapItemControl")] [Space(10)] [SerializeField]
        private SceneMapItemControl sceneMapItemControl;

        [Space(10)] [HeaderAttribute("SceneIntervalControl")] [Space(10)] [SerializeField]
        private SceneIntervalControl sceneIntervalControl;

        private void Awake()
        {
            BindUiMessages();
            BindUiEvents();
        }

        private void OnDestroy()
        {
            UnBindUiMessages();
            UnBindUiEvents();

            ClearData();
        }

        private void ClearData()
        {
            _currentSceneMapPathDataModelList = null;
            _systemTown = null;
            _currentSceneId = 0;

            SceneMapDataManager.GetInstance().ResetSceneTypeDic();
        }

        private void BindUiMessages()
        {
            if (BeTownPlayerMain.OnAutoMoveSuccess != null)
                BeTownPlayerMain.OnAutoMoveSuccess.AddListener(OnAutoMoveEnd);

            if (BeTownPlayerMain.OnAutoMoveFail != null)
                BeTownPlayerMain.OnAutoMoveFail.AddListener(OnAutoMoveEnd);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SceneChangedFinish, OnSceneChangeFinished);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSceneMapAutoMoveBeginMessage,
                OnSceneMapAutoMoveBeginMessage);
        }

        private void UnBindUiMessages()
        {
            if (BeTownPlayerMain.OnAutoMoveSuccess != null)
                BeTownPlayerMain.OnAutoMoveSuccess.RemoveListener(OnAutoMoveEnd);

            if (BeTownPlayerMain.OnAutoMoveFail != null)
                BeTownPlayerMain.OnAutoMoveFail.RemoveListener(OnAutoMoveEnd);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SceneChangedFinish, OnSceneChangeFinished);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSceneMapAutoMoveBeginMessage,
                OnSceneMapAutoMoveBeginMessage);
        }

        private void BindUiEvents()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseButtonClick);
            }
        }

        private void UnBindUiEvents()
        {
            if(closeButton != null)
                closeButton.onClick.RemoveAllListeners();
        }

        public void Init()
        {
            _systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (_systemTown != null)
            {
                _currentSceneId = _systemTown.CurrentSceneID;
                _currentSceneMapPathDataModelList =
                    SceneMapUtility.GetCurrentSceneMapPathDataModelBySceneId(_currentSceneId);
            }
            else
            {
                _currentSceneId = 0;
                _currentSceneMapPathDataModelList = null;
                return;
            }

            //场景中的地图Item
            InitSceneMapItemControl();

            //场景间隔
            InitSceneIntervalControl();

            //主角特效
            UpdateSceneMainPlayerItem();
            //初始化目标点的位置
            InitSceneMainPlayerTargetPos();
        }

        //场景的地图Item
        private void InitSceneMapItemControl()
        {
            if (sceneMapItemControl != null)
            {
                sceneMapItemControl.InitControl();
                UpdateSceneMapItemControl();
            }
        }

        //人物在地图上的特效
        private void UpdateSceneMainPlayerItem()
        {
            if (sceneMainPlayerControl == null)
                return;

            if (_systemTown == null)
                return;

            var sceneMapItem = GetSceneMapItemBySceneId(_currentSceneId);
            sceneMainPlayerControl.UpdateMainPlayer(_systemTown.MainPlayer, sceneMapItem);
        }

        //场景间隔
        private void InitSceneIntervalControl()
        {
            if (sceneIntervalControl == null)
                return;
            
            sceneIntervalControl.InitSceneIntervalControl();

            UpdateSceneIntervalControl();
        }

        //目标点
        private void InitSceneMainPlayerTargetPos()
        {
            var autoMoveTargetSceneId = BeTownPlayerMain.OnAutoMoveTargetSceneId;
            if (autoMoveTargetSceneId <= 0)
                return;

            var autoMoveTargetScenePos = BeTownPlayerMain.OnAutoMoveTargetScenePos;
            UpdateSceneMainPlayerTargetPos(autoMoveTargetSceneId, autoMoveTargetScenePos);
        }

        //点击的目标点
        private void UpdateSceneMainPlayerTargetPos(int sceneId,Vector3 targetPos)
        {
            if (sceneMainPlayerControl == null)
                return;

            var targetSceneMapItem = GetSceneMapItemBySceneId(sceneId);
            
            sceneMainPlayerControl.UpdateTargetPositionItem(targetSceneMapItem, targetPos);
        }
        
        #region UIEvent

        //寻路结束
        private void OnAutoMoveEnd()
        {
            if(sceneMainPlayerControl != null)
                sceneMainPlayerControl.ResetTargetPositionItemVisible();

            if(sceneIntervalControl != null)
                sceneIntervalControl.ResetSceneIntervalItem();

            if(sceneMapItemControl != null)
                sceneMapItemControl.ResetSceneMapItemControl();
        }

        //场景切换
        private void OnSceneChangeFinished(UIEvent uiEvent)
        {
            _systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (_systemTown == null)
                return;
            _currentSceneId = _systemTown.CurrentSceneID;

            _currentSceneMapPathDataModelList =
                SceneMapUtility.GetCurrentSceneMapPathDataModelBySceneId(_currentSceneId);

            //更新人物角色
            UpdateSceneMainPlayerItem();

            //场景间隔
            UpdateSceneIntervalControl();

            //地图中的路线点
            UpdateSceneMapItemControl();
        }

        //点击地图，开始寻路
        private void OnSceneMapAutoMoveBeginMessage(UIEvent uiEvent)
        {
            if (uiEvent == null
                || uiEvent.Param1 == null
                || uiEvent.Param2 == null)
                return;

            var targetSceneId = (int)uiEvent.Param1;
            var mapLogicPos = (Vector3)uiEvent.Param2;

            UpdateSceneMainPlayerTargetPos(targetSceneId, mapLogicPos);

            _currentSceneMapPathDataModelList =
                SceneMapUtility.GetCurrentSceneMapPathDataModelBySceneId(_currentSceneId);
            
            UpdateSceneIntervalControl();

            UpdateSceneMapItemControl();
        }

        #endregion

        #region Helper

        //根据场景Id获得对应的场景地图控制器
        private SceneMapItem GetSceneMapItemBySceneId(int sceneId)
        {
            if (sceneMapItemControl != null)
            {
                return sceneMapItemControl.GetSceneMapItemBySceneId(sceneId);
            }

            return null;
        }

        private void UpdateSceneIntervalControl()
        {
            if (sceneIntervalControl == null)
                return;

            sceneIntervalControl.UpdateSceneIntervalControlByPathList(_currentSceneMapPathDataModelList);
        }

        private void UpdateSceneMapItemControl()
        {
            if (sceneMapItemControl == null)
                return;

            sceneMapItemControl.UpdateControl(_currentSceneId,
                _currentSceneMapPathDataModelList);
        }

        #endregion


        private void OnCloseButtonClick()
        {
            SceneMapUtility.CloseSceneMapFrame();
        }

    }
}
