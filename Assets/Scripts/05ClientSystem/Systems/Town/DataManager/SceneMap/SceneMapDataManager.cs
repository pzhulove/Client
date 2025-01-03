using System;
using Protocol;
using Network;
using ProtoTable;
using System.Collections.Generic;
///////删除linq
using ActivityLimitTime;
using UnityEngine;


namespace GameClient
{

    //场景中寻路路径的数据
    //在一个场景中，从门1到门3的路径
    public class SceneMapPathByCacheDataModel
    {
        public int SceneId;

        public int BeginDoorId;
        public int EndDoorId;

        //从BeginDoorId 的 BeginBirthPos 到 EndDoorId 的 EndBirthPos;
        public List<Vector3> PathListFromBeginToEnd = null;

        //从EndDoorId 的 EndBirthPos 到BeginDoorId 的 BeginPos;
        public List<Vector3> PathListFromEndToBegin = null;
    }

    //具体的路径
    public class SceneMapPathDataModel
    {
        //所在的场景Id
        public int SceneId;

        //所在的场景起点和结束点
        public Vector3 BeginPos;
        public Vector3 EndPos;
        public List<Vector3> PathList;
    }

    public class SceneMapEnterSceneDataModel
    {
        public int SceneId;

        public int EnterDoorId;
        public Vector3 EnterDoorBirthPos;
    }

    public class SceneMapDataManager : DataManager<SceneMapDataManager>
    {

        public List<SceneMapPathDataModel> SceneMapPathDataModelList;

        //每个场景对应的GridInfo
        private Dictionary<int, PathFinding.GridInfo> _sceneGridInfoDic;
        private Dictionary<int, ISceneData> _sceneDataDic;

        public Dictionary<int, List<SceneMapPathByCacheDataModel>> _sceneMapPathByCacheDataModelDic;

        private Dictionary<int, CitySceneTable.eSceneType> _sceneTypeDic;

        public override void Initialize()
        {
        }

        public override void Clear()
        {
            ClearData();
        }

        private void ClearData()
        {
            ResetSceneMapPathDataModelList();
            ResetSceneMapPathByCacheDataModelList();
            ResetSceneGridInfoDic();
            ResetSceneTypeDic();
        }

        #region CacheSceneData

        public void SetSceneDataBySceneData(int sceneId, ISceneData sceneData)
        {
            if (_sceneDataDic == null)
                _sceneDataDic = new Dictionary<int, ISceneData>();

            _sceneDataDic[sceneId] = sceneData;
        }

        public ISceneData GetSceneDataBySceneId(int sceneId)
        {
            if (_sceneDataDic == null)
                _sceneDataDic = new Dictionary<int, ISceneData>();

            if (_sceneDataDic.ContainsKey(sceneId) == false)
            {
                var targetCityTable = TableManager.GetInstance().GetTableItem<CitySceneTable>(sceneId);
                if (targetCityTable != null)
                {
                    var levelData = DungeonUtility.LoadSceneData(targetCityTable.ResPath);
                    if (levelData != null)
                    {
                        _sceneDataDic[sceneId] = levelData;
                    }
                }
            }

            if (_sceneDataDic.ContainsKey(sceneId) == true)
            {
                var sceneData = _sceneDataDic[sceneId];
                return sceneData;
            }

            return null;
        }

        public void SetGridInfoByGridInfo(int sceneId, PathFinding.GridInfo gridInfo)
        {
            if (_sceneGridInfoDic == null)
                _sceneGridInfoDic = new Dictionary<int, PathFinding.GridInfo>();

            _sceneGridInfoDic[sceneId] = gridInfo;
        }

        public PathFinding.GridInfo GetGridInfoBySceneId(int sceneId)
        {
            if (_sceneGridInfoDic == null)
                _sceneGridInfoDic = new Dictionary<int, PathFinding.GridInfo>();

            if (_sceneGridInfoDic.ContainsKey(sceneId) == false)
            {
                var levelData = GetSceneDataBySceneId(sceneId);

                if (levelData != null)
                {
                    var gridInfo = new PathFinding.GridInfo
                    {
                        GridSize = levelData.GetGridSize(),
                        GridMinX = levelData.GetLogicXmin(),
                        GridMaxX = levelData.GetLogicXmax(),
                        GridMinY = levelData.GetLogicZmin(),
                        GridMaxY = levelData.GetLogicZmax(),
                        GridBlockLayer = levelData.GetRawBlockLayer(),
                    };
                    gridInfo.GridDiagonalLength = gridInfo.GridSize.magnitude;
                    _sceneGridInfoDic[sceneId] = gridInfo;
                }
            }

            if (_sceneGridInfoDic.ContainsKey(sceneId) == true)
            {
                var gridInfo = _sceneGridInfoDic[sceneId];
                return gridInfo;
            }

            return null;
        }

        private void ResetSceneGridInfoDic()
        {
            if (_sceneGridInfoDic != null)
            {
                _sceneGridInfoDic.Clear();
                _sceneGridInfoDic = null;
            }
        }
        #endregion


        //找到到目标场景的路径
        public List<Vector3> GetSceneMapMovePath(int targetSceneId, 
            Vector3 targetPos,
            bool isCreateAllPath = false,
            bool isShowLastScenePath = true)
        {
            ResetSceneMapPathDataModelList();

            var systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
                return null;

            if (systemTown.MainPlayer == null)
                return null;

            if (SceneMapPathDataModelList == null)
                SceneMapPathDataModelList = new List<SceneMapPathDataModel>();

            var currentSceneId = systemTown.CurrentSceneID;

            var beginPos = systemTown.MainPlayer.ActorData.MoveData.Position;

            //目标点在当前场景
            if (currentSceneId == targetSceneId)
            {
                var endPos = targetPos;
                var movePath = PathFinding.FindPath(beginPos, endPos, systemTown.GridInfo);

                var sceneMapPathDataModel = SceneMapUtility.CreateSceneMapPathDataModel(currentSceneId,
                    beginPos,
                    endPos,
                    movePath);
                SceneMapPathDataModelList.Add(sceneMapPathDataModel);
                return null;
            }
            
            //目标点不在当前场景，寻找路径
            var path = GetSceneMapMovePathCore(systemTown.m_sceneNodes,
                currentSceneId,
                beginPos,
                targetSceneId,
                targetPos,
                isCreateAllPath,
                isShowLastScenePath);

            return path;
        }

        //得到路径
        private List<Vector3> GetSceneMapMovePathCore(List<SceneNode> sceneNodeList,
            int currentSceneId,
            Vector3 beginPos,
            int targetSceneId,
            Vector3 targetPos,
            bool isCreateAllPath = false,
            bool isShowLastScenePath = true)
        {
            List<Vector3> movePath = new List<Vector3>();

            var targetIndex = SceneMapUtility.GetSceneNodeIndex(sceneNodeList, targetSceneId);
            if (targetIndex < 0)
                return movePath;

            for (var i = 0; i < sceneNodeList.Count; i++)
            {
                var currentSceneNode = sceneNodeList[i];
                if (currentSceneNode != null)
                    currentSceneNode.HasVisited = false;
            }

            var currentIndex = SceneMapUtility.GetSceneNodeIndex(sceneNodeList, currentSceneId);

            List<int> leaveSceneDoorIdList = new List<int>();
            List<SceneMapEnterSceneDataModel> enterSceneDataModelList = new List<SceneMapEnterSceneDataModel>();

            GetSceneMapMovePathByDfs(sceneNodeList,
                currentIndex,
                targetIndex,
                ref movePath,
                null,
                ref leaveSceneDoorIdList,
                ref enterSceneDataModelList);

            //得到具体的路径
            if (isCreateAllPath == true)
            {
                if (movePath.Count > 0)
                {

                    //当前场景数据
                    var firstSceneMapPathDataModel = new SceneMapPathDataModel();

                    firstSceneMapPathDataModel.SceneId = currentSceneId;
                    firstSceneMapPathDataModel.BeginPos = beginPos;
                    firstSceneMapPathDataModel.EndPos = movePath[0];
                    var firstGridInfo = GetGridInfoBySceneId(currentSceneId);

                    firstSceneMapPathDataModel.PathList = PathFinding.FindPath(beginPos, movePath[0], firstGridInfo);
                    SceneMapPathDataModelList.Add(firstSceneMapPathDataModel);

                    var i = 1;

                    for (; i < movePath.Count; i++)
                    {
                        //出场景的门和点
                        var curEndPathPos = movePath[i];
                        var curEndDoorId = leaveSceneDoorIdList[i];

                        var curEnterSceneDataModel = enterSceneDataModelList[i-1];
                        //进入场景的门
                        var curBeginDoorId = curEnterSceneDataModel.EnterDoorId;

                        var tempSceneMapPathDataModel = new SceneMapPathDataModel();

                        tempSceneMapPathDataModel.SceneId = curEnterSceneDataModel.SceneId;
                        tempSceneMapPathDataModel.BeginPos = curEnterSceneDataModel.EnterDoorBirthPos;
                        tempSceneMapPathDataModel.EndPos = curEndPathPos;
                        var tempGridInfo = GetGridInfoBySceneId(curEnterSceneDataModel.SceneId);

                        //同一个场景中，门与门之间的路径
                        tempSceneMapPathDataModel.PathList = GetScenePathFromBeginDoorToEndDoor(
                            curEnterSceneDataModel.SceneId,
                            curBeginDoorId, curEnterSceneDataModel.EnterDoorBirthPos,
                            curEndDoorId, curEndPathPos,
                            tempGridInfo
                        );
                        
                        SceneMapPathDataModelList.Add(tempSceneMapPathDataModel);
                    }


                    //最后一个场景路径
                    var lastEnterSceneDataModel = enterSceneDataModelList[i - 1];
                    var lastSceneMapPathDataModel = new SceneMapPathDataModel();

                    lastSceneMapPathDataModel.SceneId = lastEnterSceneDataModel.SceneId;
                    lastSceneMapPathDataModel.BeginPos = lastEnterSceneDataModel.EnterDoorBirthPos;
                    lastSceneMapPathDataModel.EndPos = targetPos;

                    if (isShowLastScenePath == true)
                    {
                        var lastGridInfo = GetGridInfoBySceneId(lastEnterSceneDataModel.SceneId);
                        lastSceneMapPathDataModel.PathList = PathFinding.FindPath(lastSceneMapPathDataModel.BeginPos,
                            lastSceneMapPathDataModel.EndPos,
                            lastGridInfo);
                    }
                    SceneMapPathDataModelList.Add(lastSceneMapPathDataModel);
                }
            }
            return movePath;
        }

        //得到一个场景中从一个门到另一个门的路径
        public List<Vector3> GetScenePathFromBeginDoorToEndDoor(int sceneId, int curBeginDoorId, Vector3 beginDoorPos,
            int curEndDoorId, Vector3 endDoorPos,
            PathFinding.GridInfo gridInfo)
        {
            if (_sceneMapPathByCacheDataModelDic == null)
                _sceneMapPathByCacheDataModelDic = new Dictionary<int, List<SceneMapPathByCacheDataModel>>();

            if (_sceneMapPathByCacheDataModelDic.ContainsKey(sceneId) == false)
            {
                var dataModelList = new List<SceneMapPathByCacheDataModel>();
                _sceneMapPathByCacheDataModelDic[sceneId] = dataModelList;
            }

            var curDataModelList = _sceneMapPathByCacheDataModelDic[sceneId];
            SceneMapPathByCacheDataModel curDataModel = null;
            //检测场景中是否缓存了门之间的寻路
            for (var i = 0; i < curDataModelList.Count; i++)
            {
                var tempDataModel = curDataModelList[i];
                if(tempDataModel == null)
                    continue;

                if(tempDataModel.SceneId != sceneId)
                    continue;

                if (tempDataModel.BeginDoorId == curBeginDoorId && tempDataModel.EndDoorId == curEndDoorId)
                {
                    curDataModel = tempDataModel;
                    break;
                }

                if (tempDataModel.BeginDoorId == curEndDoorId && tempDataModel.EndDoorId == curBeginDoorId)
                {
                    curDataModel = tempDataModel;
                    break;
                }
            }

            //当前场景缓存不存在，直接创建，并缓存
            if (curDataModel == null)
            {
                curDataModel = new SceneMapPathByCacheDataModel();
                curDataModel.SceneId = sceneId;
                curDataModel.BeginDoorId = curBeginDoorId;
                curDataModel.EndDoorId = curEndDoorId;

                curDataModel.PathListFromBeginToEnd = PathFinding.FindPath(beginDoorPos, endDoorPos,
                    gridInfo);
                curDataModelList.Add(curDataModel);

                return curDataModel.PathListFromBeginToEnd;
            }

            if (curDataModel.BeginDoorId == curBeginDoorId && curDataModel.EndDoorId == curEndDoorId)
            {
                //从前到后
                if (curDataModel.PathListFromBeginToEnd != null && curDataModel.PathListFromBeginToEnd.Count > 0)
                    return curDataModel.PathListFromBeginToEnd;
                else
                {
                    var movePath = PathFinding.FindPath(beginDoorPos, endDoorPos, gridInfo);
                    curDataModel.PathListFromBeginToEnd = movePath;
                    return movePath;
                }
            }
            else
            {
                //从后到钱
                if (curDataModel.PathListFromEndToBegin != null && curDataModel.PathListFromEndToBegin.Count > 0)
                {
                    return curDataModel.PathListFromEndToBegin;
                }
                else
                {
                    var movePath = PathFinding.FindPath(beginDoorPos, endDoorPos, gridInfo);
                    curDataModel.PathListFromEndToBegin = movePath;
                    return movePath;
                }
            }

        }

        //DFS遍历路径
        private bool GetSceneMapMovePathByDfs(List<SceneNode> nodes,
            int current,
            int target,
            ref List<Vector3> movePath,
            DoorNode door,
            ref List<int> leaveSceneDoorIdList,
            ref List<SceneMapEnterSceneDataModel> enterSceneDataModelList)
        {
            var currentSceneNode = nodes[current];
            currentSceneNode.HasVisited = true;

            if (current == target)
            {
                if (door != null)
                {
                    var enterSceneDataModel = SceneMapUtility.CreateEnterSceneDataModel(door,
                        currentSceneNode);
                    enterSceneDataModelList.Add(enterSceneDataModel);

                    movePath.Add(door.Door.GetRegionInfo().GetEntityInfo().GetPosition());
                    leaveSceneDoorIdList.Add(door.Door.GetDoorID());
                }

                return true;
            }

            if (door != null)
            {
                var enterSceneDataModel = SceneMapUtility.CreateEnterSceneDataModel(door,
                    currentSceneNode);
                enterSceneDataModelList.Add(enterSceneDataModel);

                movePath.Add(door.Door.GetRegionInfo().GetEntityInfo().GetPosition());
                leaveSceneDoorIdList.Add(door.Door.GetDoorID());
            }

            //DFS查找
            for (var i = 0; i < currentSceneNode.DoorNodes.Count; i++)
            {
                var doorNode = currentSceneNode.DoorNodes[i];
                var targetNode = nodes[doorNode.TargetSceneIndex];
                if (targetNode.HasVisited == false)
                {
                    if (GetSceneMapMovePathByDfs(nodes,
                        doorNode.TargetSceneIndex,
                        target, 
                        ref movePath,
                        doorNode,
                        ref leaveSceneDoorIdList,
                        ref enterSceneDataModelList))
                    {
                        return true;
                    }
                }
            }

            if (door != null)
            {
                enterSceneDataModelList.RemoveAt(enterSceneDataModelList.Count - 1);

                movePath.RemoveAt(movePath.Count - 1);
                leaveSceneDoorIdList.RemoveAt(leaveSceneDoorIdList.Count - 1);
            }

            return false;
        }

        public void ResetSceneMapPathDataModelList()
        {
            if (SceneMapPathDataModelList != null)
            {
                SceneMapPathDataModelList.Clear();
                SceneMapPathDataModelList = null;
            }
        }

        private void ResetSceneMapPathByCacheDataModelList()
        {
            if (_sceneMapPathByCacheDataModelDic != null)
            {
                var iter = _sceneMapPathByCacheDataModelDic.GetEnumerator();
                while (iter.MoveNext())
                {
                    var dataModelList = iter.Current.Value;
                    if(dataModelList != null)
                        dataModelList.Clear();
                }
                _sceneMapPathByCacheDataModelDic.Clear();
            }

        }

        public void ResetSceneTypeDic()
        {
            if (_sceneTypeDic != null)
            {
                _sceneTypeDic.Clear();
                _sceneTypeDic = null;
            }
        }

        //得到Scene的类型
        public CitySceneTable.eSceneType GetSceneType(int sceneId)
        {
            if (_sceneTypeDic == null)
                _sceneTypeDic = new Dictionary<int, CitySceneTable.eSceneType>();

            if (_sceneTypeDic.ContainsKey(sceneId) == true)
            {
                var sceneType = _sceneTypeDic[sceneId];
                return sceneType;
            }

            var citySceneTable = TableManager.GetInstance().GetTableItem<CitySceneTable>(sceneId);
            if (citySceneTable != null)
            {
                _sceneTypeDic[sceneId] = citySceneTable.SceneType;
                return citySceneTable.SceneType;
            }

            return CitySceneTable.eSceneType.NORMAL;
        }


    }
}
