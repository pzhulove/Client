using UnityEngine.UI;
using Protocol;
using UnityEngine;

namespace GameClient
{
    /// <summary>
    /// 地下城小地图
    /// </summary>
    public class BattleUIDungeonMap : BattleUIBase
    {
        #region ExtraUIBind
        private ComDungeonMap mDungeonMapCom = null;
        private GameObject mGoTextRoot = null;

        protected override void _bindExUI()
        {
            mDungeonMapCom = mBind.GetCom<ComDungeonMap>("DungeonMapCom");
            mGoTextRoot = mBind.GetGameObject("GoTextRoot");
        }

        protected override void _unbindExUI()
        {
            mDungeonMapCom = null;
            mGoTextRoot = null;
        }
        #endregion

        protected override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleUIComponent/BattleUIDungeonMap";
        }

        public ComDungeonMap dungeonMapCom { get { return mDungeonMapCom; } }

        protected override void OnEnter()
        {
            base.OnEnter();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TreasureMapSizeChange, _OnTreasureMapSizeChanged);
        }

        protected override void OnExit()
        {
            base.OnExit();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TreasureMapSizeChange, _OnTreasureMapSizeChanged);
        }

        public void SetDungeonMapActive(bool active)
        {
            if (mDungeonMapCom != null)
            {
                mDungeonMapCom.gameObject.SetActive(active);
            }
        }

        private void _OnTreasureMapSizeChanged(UIEvent uiEvent)
        {
            var param = uiEvent.Param1 as TreasureDungeonMap.UITreasureEventParam;
            if (param != null && !dungeonMapCom.IsNull())
            {
                dungeonMapCom.ResizeMap(param.width, param.height);
            }
        }
    }
}
