using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    /// <summary>
    /// 死亡之塔
    /// </summary>
    public class BattleUIDeadTower : BattleUIBase
    {
        #region ExtraUIBind
        private Text mTextDeadTowerLevel = null;

        protected override void _bindExUI()
        {
            mTextDeadTowerLevel = mBind.GetCom<Text>("TextDeadTowerLevel");
        }

        protected override void _unbindExUI()
        {
            mTextDeadTowerLevel = null;
        }
        #endregion

        protected override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleUIComponent/BattleUIDeadTower";
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            InitForDeadTower();
        }

        public void InitForDeadTower()
        {
            if (mTextDeadTowerLevel != null)
            {
                mTextDeadTowerLevel.transform.parent.gameObject.SetActive(true);
            }
        }

        public void SetFloor(int floor)
        {
            if (mTextDeadTowerLevel != null)
                mTextDeadTowerLevel.text = floor.ToString();
        }
    }
}