using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    class SceneIntervalControl : MonoBehaviour
    {
        private Dictionary<string, GameObject> _sceneIntervalItemDic =
            new Dictionary<string, GameObject>();

        private List<GameObject> _showIntervalList = new List<GameObject>();

        [Space(10)] [HeaderAttribute("Item")] [Space(10)] [SerializeField]
        private GameObject sceneIntervalRoot;

        private void OnDestroy()
        {
            _sceneIntervalItemDic.Clear();
            _showIntervalList.Clear();
        }

        public void InitSceneIntervalControl()
        {
            if (sceneIntervalRoot == null)
                return;

            _sceneIntervalItemDic.Clear();

            var sceneIntervalArray = sceneIntervalRoot.GetComponentsInChildren<SceneIntervalItem>(true);
            if (sceneIntervalArray == null || sceneIntervalArray.Length <= 0)
                return;

            for (var i = 0; i < sceneIntervalArray.Length; i++)
            {
                var currentSceneIntervalItem = sceneIntervalArray[i];
                if(currentSceneIntervalItem == null)
                    continue;

                var intervalStr = SceneMapUtility.GetIntervalStr(currentSceneIntervalItem.FirstSceneId,
                    currentSceneIntervalItem.SecondSceneId);

                CommonUtility.UpdateGameObjectVisible(currentSceneIntervalItem.gameObject, false);

                _sceneIntervalItemDic[intervalStr] = currentSceneIntervalItem.gameObject;
            }
        }

        public void UpdateSceneIntervalControlByPathList(List<SceneMapPathDataModel> pathDataModelList)
        {
            ResetSceneIntervalItem();

            if (pathDataModelList == null || pathDataModelList.Count <= 1)
                return;

            //更新路径
            for (var i = 0; i < pathDataModelList.Count - 1; i++)
            {
                var firstSceneDataModel = pathDataModelList[i];
                var secondSceneDataModel = pathDataModelList[i + 1];

                var firstSceneId = firstSceneDataModel.SceneId;
                var secondSceneId = secondSceneDataModel.SceneId;

                var intervalStr = SceneMapUtility.GetIntervalStr(firstSceneId, secondSceneId);
                if (_sceneIntervalItemDic.ContainsKey(intervalStr) == true)
                {
                    var itemGo = _sceneIntervalItemDic[intervalStr];
                    CommonUtility.UpdateGameObjectVisible(itemGo, true);
                    _showIntervalList.Add(itemGo);
                    continue;
                }

                intervalStr = SceneMapUtility.GetIntervalStr(secondSceneId, firstSceneId);
                if (_sceneIntervalItemDic.ContainsKey(intervalStr) == true)
                {
                    var itemGo = _sceneIntervalItemDic[intervalStr];
                    CommonUtility.UpdateGameObjectVisible(itemGo, true);
                    _showIntervalList.Add(itemGo);
                    continue;
                }
            }
        }
        
        public void ResetSceneIntervalItem()
        {
            if (_showIntervalList == null || _showIntervalList.Count <= 0)
                return;

            for (var i = 0; i < _showIntervalList.Count; i++)
            {
                var curInterval = _showIntervalList[i];
                CommonUtility.UpdateGameObjectVisible(curInterval, false);
            }

            _showIntervalList.Clear();
        }
    }
}
