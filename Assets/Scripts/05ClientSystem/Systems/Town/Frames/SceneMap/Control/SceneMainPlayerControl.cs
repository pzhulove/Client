using UnityEngine;

namespace GameClient
{
    class SceneMainPlayerControl : MonoBehaviour
    {

        private BeTownPlayerMain _beTownPlayerMain;
        private SceneMapItem _sceneMapItem;

        [Space(10)] [HeaderAttribute("Item")] [Space(10)]
        [SerializeField] private GameObject mainPlayerItem;
        [SerializeField] private GameObject targetPosItem;

        private void Awake()
        {

        }

        private void OnDestroy()
        {
            Clear();
        }

        private void Clear()
        {
            _beTownPlayerMain = null;
            _sceneMapItem = null;
        }


        #region MainPlayer
        public void UpdateMainPlayer(BeTownPlayerMain beTownPlayerMain, SceneMapItem sceneMapItem)
        {
            _beTownPlayerMain = beTownPlayerMain;
            _sceneMapItem = sceneMapItem;

            if (_beTownPlayerMain == null)
                return;

            if (_sceneMapItem == null)
                return;

            UpdateMainPlayerItem();
        }

        private void UpdateMainPlayerItem()
        {
            if (mainPlayerItem == null)
                return;

            CommonUtility.UpdateGameObjectVisible(mainPlayerItem, true);
            mainPlayerItem.transform.SetParent(_sceneMapItem.gameObject.transform, false);

            UpdateMainPlayerItemPosition();

            UpdateMainPlayerAutoMovePathList();
        }

        private void Update()
        {
            if (_sceneMapItem == null)
                return;

            if (mainPlayerItem == null)
                return;

            if (_beTownPlayerMain == null)
                return;

            //不在移动中，不更新位置
            if (_beTownPlayerMain.MoveState == BeTownPlayerMain.EMoveState.Idle
                || _beTownPlayerMain.MoveState == BeTownPlayerMain.EMoveState.Invalid)
                return;

            UpdateMainPlayerItemPosition();

            //当前在寻路中，更新寻路的路径
            UpdateMainPlayerAutoMovePathList();
        }

        private void UpdateMainPlayerAutoMovePathList()
        {
            if (_beTownPlayerMain.MoveState == BeTownPlayerMain.EMoveState.AutoMoving)
            {
                var movePathNumber = _beTownPlayerMain.ActorData.MoveData.MovePath.Count;
                _sceneMapItem.ShowPathPointPrefabList(movePathNumber);
            }
        }


        //更新人物在地图上的位置
        private void UpdateMainPlayerItemPosition()
        {
            var actorData = _beTownPlayerMain.ActorData;
            if (actorData == null)
                return;

            var moveData = actorData.MoveData;
            if (moveData == null)
                return;
            //人物场景中的位置
            var scenePos = moveData.ServerPosition;

            var sceneMapXRate = _sceneMapItem.GetXRate();
            var sceneMapZRate = _sceneMapItem.GetZRate();

            //人物在地图上的位置
            mainPlayerItem.transform.localPosition =
                new Vector3(scenePos.x * sceneMapXRate, scenePos.z * sceneMapZRate, 0.0f);
        }
        #endregion


        #region TargetPosition
        //目标点展示
        public void UpdateTargetPositionItem(SceneMapItem sceneMapItem, Vector3 logicPos)
        {
            if (targetPosItem == null)
                return;

            if (sceneMapItem == null)
            {
                ResetTargetPositionItemVisible();
                return;
            }

            CommonUtility.UpdateGameObjectVisible(targetPosItem, true);
            targetPosItem.transform.SetParent(sceneMapItem.gameObject.transform, false);
            targetPosItem.transform.localPosition = new Vector3(logicPos.x, logicPos.y, 0);
        }

        //目标点隐藏
        public void ResetTargetPositionItemVisible()
        {
            CommonUtility.UpdateGameObjectVisible(targetPosItem, false);
        }
        #endregion

    }
}
