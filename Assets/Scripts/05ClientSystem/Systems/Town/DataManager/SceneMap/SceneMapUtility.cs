using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{
    //场景地图
    public static class SceneMapUtility
    {
        public static void OpenSceneMapFrame()
        {
            CloseSceneMapFrame();

            ClientSystemManager.GetInstance().OpenFrame<SceneMapFrame>(FrameLayer.Middle);
        }

        public static void CloseSceneMapFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<SceneMapFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<SceneMapFrame>();
        }


        public static SceneMapPathDataModel CreateSceneMapPathDataModel(int sceneId, Vector3 beginPos, Vector3 endPos,
            List<Vector3> pathList)
        {
            var sceneMapPathDataModel = new SceneMapPathDataModel();
            sceneMapPathDataModel.SceneId = sceneId;
            sceneMapPathDataModel.BeginPos = beginPos;
            sceneMapPathDataModel.EndPos = endPos;
            sceneMapPathDataModel.PathList = pathList;

            return sceneMapPathDataModel;
        }

        public static int GetSceneNodeIndex(List<SceneNode> nodes, int sceneId)
        {
            for (var i = 0; i < nodes.Count; i++)
            {
                var curNode = nodes[i];
                if(curNode == null)
                    continue;

                if (curNode.SceneID == sceneId)
                    return i;
            }

            return -1;
        }

        public static SceneMapEnterSceneDataModel CreateEnterSceneDataModel(DoorNode door, SceneNode sceneNode)
        {
            var enterSceneDoorId = door.Door.GetTargetDoorID();
            var sceneId = sceneNode.SceneID;

            var enterSceneDataModel = new SceneMapEnterSceneDataModel();
            enterSceneDataModel.SceneId = sceneId;
            enterSceneDataModel.EnterDoorId = enterSceneDoorId;

            for (var i = 0; i < sceneNode.DoorNodes.Count; i++)
            {
                var nextDoorNode = sceneNode.DoorNodes[i];
                if (nextDoorNode.Door.GetDoorID() == enterSceneDoorId)
                {
                    enterSceneDataModel.EnterDoorBirthPos = nextDoorNode.Door.GetBirthPos();
                    break;
                }
            }

            return enterSceneDataModel;
        }

        public  static void TestPrintSceneMapPath()
        {
            var sceneMapPathDataModelList = SceneMapDataManager.GetInstance().SceneMapPathDataModelList;

            for (var i = 0; i < sceneMapPathDataModelList.Count; i++)
            {
                var curPathDataModel = sceneMapPathDataModelList[i];
                Logger.LogErrorFormat("111111111111 the sceneId is {0}",
                    curPathDataModel.SceneId);

                var pathCount = 0;
                if (curPathDataModel.PathList != null)
                    pathCount = curPathDataModel.PathList.Count;

                var beginPos = curPathDataModel.BeginPos;
                var endPos = curPathDataModel.EndPos;

                Logger.LogErrorFormat(
                    "111111111111 the pathCount is {0}, beginPos is ({1},{2},{3}) to  to to endPos is ({4},{5},{6}) \n",
                    pathCount,
                    beginPos.x,
                    beginPos.y,
                    beginPos.z,
                    endPos.x,
                    endPos.y,
                    endPos.z);

                //Logger.LogErrorFormat(
                //    "the pathCount is {0}, beginPos is ({1},{2},{3}) \n",
                //    pathCount,
                //    beginPos.x,
                //    beginPos.y,
                //    beginPos.z);
            }
        }

        public static string GetIntervalStr(int firstSceneId, int secondSceneId)
        {
            return firstSceneId.ToString() + "_" + secondSceneId.ToString();
        }

        public static List<SceneMapPathDataModel> GetCurrentSceneMapPathDataModelBySceneId(int currentSceneId)
        {
            var sceneMapPathDataModelList = SceneMapDataManager.GetInstance().SceneMapPathDataModelList;
            if (sceneMapPathDataModelList == null || sceneMapPathDataModelList.Count <= 0)
                return null;

            List<SceneMapPathDataModel> curSceneMapPathDataModelList = new List<SceneMapPathDataModel>();

            var isFind = false;
            for (var i = 0; i < sceneMapPathDataModelList.Count; i++)
            {
                var mapPathDataModel = sceneMapPathDataModelList[i];
                if(mapPathDataModel == null)
                    continue;

                if (mapPathDataModel.SceneId == currentSceneId)
                {
                    isFind = true;
                    curSceneMapPathDataModelList.Add(mapPathDataModel);
                }
                else
                {
                    if (isFind == true)
                    {
                        curSceneMapPathDataModelList.Add(mapPathDataModel);
                    }
                }
            }

            return curSceneMapPathDataModelList;
        }

        //得到某个场景中的npc数据
        public static ISceneNPCInfoData GetNpcInfoByNpcId(int npcId, ISceneData sceneData)
        {
            if (sceneData == null)
                return null;

            if (sceneData.GetNpcInfoLength() <= 0)
                return null;

            for (var i = 0; i < sceneData.GetNpcInfoLength(); i++)
            {
                var npcInfo = sceneData.GetNpcInfo(i);
                if(npcInfo == null)
                    continue;

                if (npcInfo.GetEntityInfo().GetResid() == npcId)
                    return npcInfo;
            }

            return null;
        }

        //得到合法点
        public static Vector3 GetValidTargetPosition(PathFinding.GridInfo gridInfo,Vector3 a_targetPos, Vector2 a_minRange, Vector2 a_maxRange)
        {
            PathFinding.Grid targetGrid = new PathFinding.Grid(gridInfo, a_targetPos);
            int validTargetGridX = targetGrid.X > gridInfo.GridMaxX ?
                gridInfo.GridMaxX : (targetGrid.X < gridInfo.GridMinX ? gridInfo.GridMinX : targetGrid.X);
            int validTargetGridY = targetGrid.Y > gridInfo.GridMaxY ?
                gridInfo.GridMaxX : (targetGrid.Y < gridInfo.GridMinY ? gridInfo.GridMinY : targetGrid.Y);

            PathFinding.Grid validTargetGrid = new PathFinding.Grid(gridInfo, validTargetGridX, validTargetGridY);

            Vector3 vecMinHalf = new Vector3(a_minRange.x * 0.5f, 0.0f, a_minRange.y * 0.5f);
            Vector3 vecMaxHalf = new Vector3(a_maxRange.x * 0.5f, 0.0f, a_maxRange.y * 0.5f);

            PathFinding.Grid minGridA = new PathFinding.Grid(gridInfo, validTargetGrid.RealPos + vecMinHalf);
            PathFinding.Grid minGridB = new PathFinding.Grid(gridInfo, validTargetGrid.RealPos - vecMinHalf);
            PathFinding.Grid maxGridA = new PathFinding.Grid(gridInfo, validTargetGrid.RealPos + vecMaxHalf);
            PathFinding.Grid maxGridB = new PathFinding.Grid(gridInfo, validTargetGrid.RealPos - vecMaxHalf);

            List<PathFinding.Grid> validGrids = new List<PathFinding.Grid>();

            int outMinX = maxGridA.X < maxGridB.X ? maxGridA.X : maxGridB.X;
            int outMaxX = maxGridA.X < maxGridB.X ? maxGridB.X : maxGridA.X;
            int outMinY = maxGridA.Y < maxGridB.Y ? maxGridA.Y : maxGridB.Y;
            int outMaxY = maxGridA.Y < maxGridB.Y ? maxGridB.Y : maxGridA.Y;

            int inMinX = minGridA.X < minGridB.X ? minGridA.X : minGridB.X;
            int inMaxX = minGridA.X < minGridB.X ? minGridB.X : minGridA.X;
            int inMinY = minGridA.Y < minGridB.Y ? minGridA.Y : minGridB.Y;
            int inMaxY = minGridA.Y < minGridB.Y ? minGridB.Y : minGridA.Y;

            for (int i = outMinX; i <= outMaxX; ++i)
            {
                for (int j = outMinY; j <= outMaxY; ++j)
                {
                    if (
                        (i >= inMinX && i <= inMaxX && j >= inMinY && j <= inMaxY) ||
                        i < gridInfo.GridMinX || i >= gridInfo.GridMaxX ||
                        j < gridInfo.GridMinY || j >= gridInfo.GridMaxY
                        )
                    {
                        continue;
                    }

                    int x = i - gridInfo.GridMinX;
                    int y = j - gridInfo.GridMinY;
                    int index = (gridInfo.GridMaxX - gridInfo.GridMinX) * y + x;
                    if (index >= 0 && index < gridInfo.GridBlockLayer.Length)
                    {
                        if (gridInfo.GridBlockLayer[index] != 0)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    validGrids.Add(new PathFinding.Grid(gridInfo, i, j));
                }
            }

            if (validGrids.Count > 0)
            {
                Logger.LogProcessFormat("获取随机位置 >>> 有效格子数{0}", validGrids.Count);
                int index = UnityEngine.Random.Range(0, validGrids.Count - 1);
                return validGrids[index].RealPos;
            }
            else
            {
                Logger.LogWarningFormat("获取随机位置 >>> 错误!");
                return a_targetPos;
            }
        }


        //找到当前点周围合法的点，如果当前点合法，直接返回当前点
        public static Vector3 FindValidPosition(int sceneId, Vector3 curPos,int findStep = 16)
        {
            var gridInfo = SceneMapDataManager.GetInstance().GetGridInfoBySceneId(sceneId);

            if (gridInfo == null)
                return curPos;

            var blockData = gridInfo.GridBlockLayer;
            var col = gridInfo.GridMaxX - gridInfo.GridMinX;
            var row = gridInfo.GridMaxY - gridInfo.GridMinY;

            var curGridX = (int) (Mathf.Floor(curPos.x / gridInfo.GridSize.x)) - gridInfo.GridMinX;
            var curGridY = (int) (Mathf.Floor(curPos.z / gridInfo.GridSize.y)) - gridInfo.GridMinY;

            //判断自己是不是合法的点
            var isValidPosition = IsValidPosition(blockData, col, row, curGridX, curGridY);
            if (isValidPosition == true)
                return curPos;

            //在周围寻找Step圈，知道找到对应的点
            int index = 0;
            while (findStep > 0)
            {
                //步数减1
                findStep -= 1;

                //圈数增加1
                index += 1;

                var minXLimit = -index;
                var maxXLimit = index;

                var minYLimit = -index;
                var maxYLimit = index;

                //寻找每圈四周的数据
                //最底层
                for (var i = minXLimit; i <= maxXLimit; i++)
                {
                    var tempGridX = curGridX + i;
                    var tempGridY = curGridY + minYLimit;
                    isValidPosition = IsValidPosition(blockData, col, row, tempGridX, tempGridY);
                    if (isValidPosition == true)
                    {
                        var validPos = GetValidPos(gridInfo, curPos, curGridX, curGridY,
                            tempGridX, tempGridY);
                        return validPos;
                    }
                }

                //最顶层
                for (var i = minXLimit; i <= maxXLimit; i++)
                {
                    var tempGridX = curGridX + i;
                    var tempGridY = curGridY + maxYLimit;
                    isValidPosition = IsValidPosition(blockData, col, row, tempGridX, tempGridY);
                    if (isValidPosition == true)
                    {
                        var validPos = GetValidPos(gridInfo, curPos, curGridX, curGridY,
                            tempGridX,
                            tempGridY);
                        return validPos;
                    }
                }

                //最左边
                for (var i = minYLimit + 1; i <= maxYLimit - 1; i++)
                {
                    var tempGridX = curGridX + minXLimit;
                    var tempGridY = curGridY + i;
                    isValidPosition = IsValidPosition(blockData, col, row, tempGridX, tempGridY);
                    if (isValidPosition == true)
                    {
                        var validPos = GetValidPos(gridInfo, curPos, curGridX, curGridY,
                            tempGridX,
                            tempGridY);

                        return validPos;
                    }
                }

                //最右边
                for (var i = minYLimit + 1; i <= maxYLimit + 1; i++)
                {
                    var tempGridX = curGridX + maxXLimit;
                    var tempGridY = curGridY + i;
                    isValidPosition = IsValidPosition(blockData, col, row, tempGridX, tempGridY);
                    if (isValidPosition == true)
                    {
                        var validPos = GetValidPos(gridInfo, curPos, curGridX, curGridY,
                            tempGridX,
                            tempGridY);
                        return validPos;
                    }
                }
            }
            return curPos;
        }

        //得到合法点的具体坐标
        private static Vector3 GetValidPos(PathFinding.GridInfo gridInfo, Vector3 startPos, int startGridX, int startGridY,
            int endGridX, int endGridY)
        {
            var gridXInterval = endGridX - startGridX;
            var gridYInterval = endGridY - startGridY;

            var targetPos = new Vector3(
                startPos.x + gridXInterval * gridInfo.GridSize.x,
                startPos.y,
                startPos.z + gridYInterval * gridInfo.GridSize.y);

            return targetPos;
        }

        //判断点是否合法
        private static bool IsValidPosition(byte[] blockData, int width, int height, int targetPosX, int targetPosY)
        {
            //是否超过边界
            if (targetPosX < 0 || targetPosX >= width)
                return false;

            if (targetPosY < 0 || targetPosY >= height)
                return false;

            //当前的点在blockData中的索引
            var index = width * targetPosY + targetPosX;
            if (index < 0 || index >= blockData.Length)
                return false;

            //对应的blockData中数值非法
            if (blockData[index] != 0)
                return false;

            return true;
        }

    }
}
