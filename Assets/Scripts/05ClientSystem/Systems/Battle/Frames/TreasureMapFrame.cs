using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    /// <summary>
    /// 春节活动副本 藏宝图副本的UI界面
    /// </summary>
    public class TreasureMapFrame : ClientFrame
    {
#region ExtraUIBind
        private TreasureDungeonMap mTreasureDungeonMap;
        private TreasureMapBuff mTreasureMapBuff;
        protected override void _bindExUI()
        {
            mTreasureDungeonMap = mBind.GetCom<TreasureDungeonMap>("DungeonMapScript");
            mTreasureMapBuff = mBind.GetCom<TreasureMapBuff>("DungeonBuffScript");
        }

        protected override void _unbindExUI()
        {
            mTreasureDungeonMap = null;
            mTreasureMapBuff = null;
        }
        #endregion

        protected sealed override void _OnLoadPrefabFinish()
        {
            if (null == mComClienFrame)
            {
                mComClienFrame = frame.AddComponent<ComClientFrame>();
            }
            mComClienFrame.SetGroupTag("system");
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/TreasureMapFrame";
        }

        public TreasureDungeonMap TreasureDungeonMap
        {
            get { return mTreasureDungeonMap; }
        }

        public TreasureMapBuff TreasureMapBuff
        {
            get { return mTreasureMapBuff; }
        }
    }

}