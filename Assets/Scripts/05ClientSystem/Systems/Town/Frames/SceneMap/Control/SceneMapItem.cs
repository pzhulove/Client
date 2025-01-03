using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ProtoTable;
using System;
using System.Collections.Generic;

namespace GameClient
{

    public class SceneMapItem : MonoBehaviour
    {


        private int _pathInterval = 4;
        private bool _isInit = false;
        private int _levelLimit = 0;
        private string _sceneName = "";

        //地图缩放比例
        private Vector3 _sceneSizeRate = Vector3.zero;
        private Vector3 _sceneOffset = Vector3.zero;

        private RectTransform _rtf = null;

        private bool _isShowPath = false;
        //当前系统的场景Id
        private int _currentSystemSceneId = 0;
        //路径点相关
        private GameObject _pathPrefab = null;
        private List<GameObject> _pathPointPrefabList = new List<GameObject>();

        [Space(10)]
        [HeaderAttribute("Content")]
        [Space(10)]
        [SerializeField] private int sceneId;
        [SerializeField] private ComButtonEx sceneButtonEx;

        [Space(10)]
        [HeaderAttribute("SceneLock")]
        [Space(10)]
        [SerializeField] private GameObject sceneLockRoot;
        [SerializeField] private Text sceneLockDesLabel;

        [Space(10)]
        [HeaderAttribute("SceneMapPath")]
        [Space(10)]
        [SerializeField] private GameObject pathRoot;

        private void Awake()
        {
            if (sceneButtonEx != null)
            {
                sceneButtonEx.onMouseClick.RemoveAllListeners();
                sceneButtonEx.onMouseClick.AddListener(OnSceneButtonExClick);
            }
        }

        private void OnDestroy()
        {
            if (sceneButtonEx != null)
                sceneButtonEx.onMouseClick.RemoveAllListeners();

            ClearData();
        }

        private void ClearData()
        {
            _isInit = false;
            _levelLimit = 0;
            _sceneName = "";

            _rtf = null;

            _isShowPath = false;
            _currentSystemSceneId = 0;
            _pathPrefab = null;
            _pathPointPrefabList.Clear();
        }

        private void OnEnable()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged, OnLevelChanged);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged, OnLevelChanged);

        }

        public void InitItem(GameObject pathPrefab, int pathInterval = 4)
        {
            if (_isInit == true)
                return;

            if (pathRoot != null)
                _pathPrefab = pathPrefab;

            _pathInterval = pathInterval;
            if (_pathInterval <= 0)
                _pathInterval = 4;
            
            var sceneTable = TableManager.GetInstance().GetTableItem<CitySceneTable>(sceneId);
            if (sceneTable == null)
                return;

            var sceneData = DungeonUtility.LoadSceneData(sceneTable.ResPath);
            if (sceneData == null)
                return;

            _sceneName = sceneTable.Name;
            _levelLimit = sceneTable.LevelLimit;


            _rtf = GetComponent<RectTransform>();
            var rect = _rtf.rect;
            //场景逻辑大小
            var logicXSize = sceneData.GetLogicXSize();
            var logicZSize = sceneData.GetLogicZSize();

            //缩放比例
            var logicWidth = logicXSize.y - logicXSize.x;
            if (logicWidth > 0)
                _sceneSizeRate.x = rect.width / logicWidth;
            else
            {
                _sceneSizeRate.x = 1.0f;
            }

            var logicHeight = logicZSize.y - logicZSize.x;
            if (logicHeight > 0)
                _sceneSizeRate.z = rect.height / logicHeight;
            else
            {
                _sceneSizeRate.z = 1.0f;
            }

            //偏移量
            _sceneOffset = new Vector3(logicXSize.x, 0.0f, logicZSize.x);

            //等级
            if (sceneLockDesLabel != null)
            {
                var tipStr = TR.Value("town_map_lock_desc", _levelLimit);
                sceneLockDesLabel.text = tipStr;
            }
            UpdateLock();

            _isInit = true;
        }

        #region UIEvent

        private void OnLevelChanged(UIEvent uiEvent)
        {
            if (_isInit == false)
                return;

            UpdateLock();
        }
        private void UpdateLock()
        {
            //未解锁
            if (PlayerBaseData.GetInstance().Level < _levelLimit)
            {
                CommonUtility.UpdateGameObjectVisible(sceneLockRoot, true);
            }
            else
            {
                CommonUtility.UpdateGameObjectVisible(sceneLockRoot, false);
            }
        }

        #endregion

        #region Path

        //创建路径
        public void UpdateSceneMapPath(int currentSystemSceneId, List<Vector3> pathList)
        {
            if (pathRoot == null)
                return;

            if (_pathPrefab == null)
                return;

            if (pathList == null || pathList.Count <= 0)
                return;

            var pathCount = pathList.Count;

            //重置点
            _isShowPath = true;
            ResetPathPointPrefabList();
            CommonUtility.UpdateGameObjectVisible(pathRoot, true);

            //当前系统所处的场景Id
            _currentSystemSceneId = currentSystemSceneId;

            //检测当前的路径点是否足够，不足创建
            var showPathPointNumber = pathCount / _pathInterval + 1;
            var pathPointPrefabNumber = _pathPointPrefabList.Count;
            if (pathPointPrefabNumber < showPathPointNumber)
            {
                for (var i = pathPointPrefabNumber + 1; i <= showPathPointNumber; i++)
                {
                    var onePathPrefab = CreateOnePathPrefab();
                    _pathPointPrefabList.Add(onePathPrefab);
                }
            }

            var pathPointPrefabIndex = 0;
            for (var index = pathCount - 1; index >= 0; index -= _pathInterval)
            {
                //路径点的位置坐标
                var pathPointPosition = pathList[index];
                if(pathPointPrefabIndex >= _pathPointPrefabList.Count)
                    continue;

                //路径点的预制体
                var pathPointPrefab = _pathPointPrefabList[pathPointPrefabIndex];
                pathPointPrefabIndex += 1;

                if (pathPointPrefab == null)
                    continue;

                SetPathPointMapPosition(pathPointPrefab, pathPointPosition);
            }
        }

        //设置地图上点的位置
        private void SetPathPointMapPosition(GameObject pathPointPrefab, Vector3 scenePointPosition)
        {
            //转化为地图中的位置
            var mapLogicPointPosition = GetMapLogicPointPosition(scenePointPosition);

            CommonUtility.UpdateGameObjectVisible(pathPointPrefab, true);
            pathPointPrefab.transform.localPosition = mapLogicPointPosition;

        }

        //根据场景中实际的坐标点，找到对应地图上的点
        private Vector3 GetMapLogicPointPosition(Vector3 scenePointPosition)
        {
            var sceneLogicPointPosition = scenePointPosition - _sceneOffset;
            var mapLogicPointPosition = new Vector3(sceneLogicPointPosition.x * GetXRate(),
                sceneLogicPointPosition.z * GetZRate(),
                0.0f);

            return mapLogicPointPosition;
        }

        //显示路径
        public void ShowPathPointPrefabList(int movePathNumber)
        {
            if (pathRoot == null)
                return;
            
            if (_isShowPath == false)
                return;

            var pathPointNumber = movePathNumber / _pathInterval + 1;

            UpdatePathPointPrefabList(0, pathPointNumber - 1, true);
            UpdatePathPointPrefabList(pathPointNumber, _pathPointPrefabList.Count - 1, false);
        }

        private GameObject CreateOnePathPrefab()
        {
            var pathGo = Instantiate(_pathPrefab) as GameObject;
            CommonUtility.UpdateGameObjectVisible(pathGo, false);
            pathGo.transform.SetParent(pathRoot.transform, false);

            return pathGo;
        }

        private void ResetPathPointPrefabList()
        {
            UpdatePathPointPrefabList(0, _pathPointPrefabList.Count - 1, false);
        }

        private void UpdatePathPointPrefabList(int startIndex, int endIndex,bool isShow)
        {
            for (var i = startIndex; i <= endIndex; i++)
            {
                if (i >= _pathPointPrefabList.Count)
                    break;
                else
                {
                    var pathPointPrefab = _pathPointPrefabList[i];
                    CommonUtility.UpdateGameObjectVisible(pathPointPrefab, isShow);
                }
            }
        }

        public void ResetSceneMapItemPath()
        {
            CommonUtility.UpdateGameObjectVisible(pathRoot, false);
            _currentSystemSceneId = 0;
            _isShowPath = false;
        }

        #endregion

        #region ButtonClick
        private void OnSceneButtonExClick(PointerEventData pointerEventData)
        {
            //未初始化
            if (_isInit == false)
                return;

            //等级不足
            if (PlayerBaseData.GetInstance().Level < _levelLimit)
            {
                var tipStr = TR.Value("town_lock_desc", _sceneName, _levelLimit);
                SystemNotifyManager.SysNotifyFloatingEffect(tipStr);
                return;
            }
            else
            {
                Vector2 pos = Vector2.zero;
                if (_rtf == null)
                    return;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(_rtf,
                    pointerEventData.pressPosition,
                    pointerEventData.enterEventCamera,
                    out pos);

                //点击点在场景中的位置
                var baseTargetPos = new Vector3(pos.x / _sceneSizeRate.x, 0.0f, pos.y / _sceneSizeRate.z);
                var targetScenePos = baseTargetPos + _sceneOffset;

                OnSceneMapClickAction(sceneId, targetScenePos);
            }
        }

        private void OnSceneMapClickAction(int targetSceneId, Vector3 targetScenePos)
        {
            var systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
                return;

            if (systemTown.MainPlayer == null)
                return;

            var sceneType = SceneMapDataManager.GetInstance().GetSceneType(targetSceneId);

            //正常可以移动的场景
            var isNormalSceneType = sceneType != CitySceneTable.eSceneType.DUNGEON_ENTRY;
            if (isNormalSceneType)
            {
                targetScenePos = SceneMapUtility.FindValidPosition(targetSceneId, targetScenePos);
            }
            
            var mapLogicPos = GetMapLogicPointPosition(targetScenePos);

            systemTown.MainPlayer.CommandAutoMoveToTargetPos(targetSceneId, targetScenePos, isNormalSceneType);
            BeTownPlayerMain.SetOnAutoMoveTargetData(targetSceneId, mapLogicPos);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSceneMapAutoMoveBeginMessage,
                targetSceneId,
                mapLogicPos);

        }
        #endregion

        #region Helper
        public int GetSceneId()
        {
            return sceneId;
        }

        public float GetXRate()
        {
            return _sceneSizeRate.x;
        }

        public float GetZRate()
        {
            return _sceneSizeRate.z;
        }
        #endregion

    }
}
