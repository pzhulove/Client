using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    class SceneMapItemControl : MonoBehaviour
    {
        private Dictionary<int, SceneMapItem> _sceneMapItemDic = new Dictionary<int, SceneMapItem>();

        private List<SceneMapItem> _showSceneMapItemList = new List<SceneMapItem>();

        [Space(10)] [HeaderAttribute("PathIntervalPoint")] [Space(10)] [SerializeField]
        private int pathInterval = 4;

        [Space(10)]
        [HeaderAttribute("SceneRoot")]
        [Space(10)]
        [SerializeField] private GameObject sceneItemRoot;

        [Space(10)]
        [HeaderAttribute("PathPrefab")]
        [Space(10)]
        [SerializeField] private GameObject pathPrefab;


        private void OnDestroy()
        {
            ClearData();
        }

        private void ClearData()
        {
            _sceneMapItemDic.Clear();
        }

        public void InitControl()
        {
            if (sceneItemRoot == null)
                return;

            var sceneMapItemArray = sceneItemRoot.GetComponentsInChildren<SceneMapItem>(true);
            if (sceneMapItemArray == null || sceneMapItemArray.Length <= 0)
                return;

            for (var i = 0; i < sceneMapItemArray.Length; i++)
            {
                var curSceneMapItem = sceneMapItemArray[i];

                if (curSceneMapItem == null)
                    continue;

                curSceneMapItem.InitItem(pathPrefab, pathInterval);
                var sceneId = curSceneMapItem.GetSceneId();

                _sceneMapItemDic[sceneId] = curSceneMapItem;
            }
        }

        public void UpdateControl(int currentSystemSceneId, 
            List<SceneMapPathDataModel> pathDataModelList)
        {
            ResetSceneMapItemControl();

            if (pathDataModelList == null || pathDataModelList.Count <= 0)
                return;

            if (_sceneMapItemDic.Count <= 0)
                return;

            for (var i = 0; i < pathDataModelList.Count; i++)
            {
                var curSceneMapPathDataModel = pathDataModelList[i];
                if(curSceneMapPathDataModel == null)
                    continue;

                var sceneId = curSceneMapPathDataModel.SceneId;
                var pathList = curSceneMapPathDataModel.PathList;

                if(_sceneMapItemDic.ContainsKey(sceneId) == false)
                    continue;

                var sceneMapItem = _sceneMapItemDic[sceneId];
                if(sceneMapItem == null)
                    continue;

                sceneMapItem.UpdateSceneMapPath(currentSystemSceneId, pathList);
                _showSceneMapItemList.Add(sceneMapItem);
            }
        }


        //根据场景Id获得对应的场景地图控制器
        public SceneMapItem GetSceneMapItemBySceneId(int sceneId)
        {
            if (_sceneMapItemDic == null || _sceneMapItemDic.Count <= 0)
                return null;

            if (_sceneMapItemDic.ContainsKey(sceneId) == true)
            {
                var sceneMapItem = _sceneMapItemDic[sceneId];
                return sceneMapItem;
            }

            return null;
        }


        public void ResetSceneMapItemControl()
        {
            if (_showSceneMapItemList.Count <= 0)
                return;

            for (var i = 0; i < _showSceneMapItemList.Count; i++)
            {
                var sceneMapItem = _showSceneMapItemList[i];
                if(sceneMapItem == null)
                    continue;

                sceneMapItem.ResetSceneMapItemPath();
            }

            _showSceneMapItemList.Clear();
        }

    }
}
