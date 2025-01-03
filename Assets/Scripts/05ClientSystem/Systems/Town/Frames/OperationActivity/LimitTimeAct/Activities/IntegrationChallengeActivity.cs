using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameClient
{
    /// <summary>
    /// 挑战者活动任务类型
    /// </summary>
    public enum OpActivityChallengeType
    {
        OACT_MONSTER_ATTACK = 0,  //怪物攻城（通关）
        OACT_ELITE_DUNGEON = 1,    //精英地下城（需消耗精力）（通关）
        OACT_ABYESS_DUNGEON = 2,  //深渊地下城（参与）
        OACT_ASCENT_DUNGEON = 3,  //远古地下城（通关）
        OACT_TEAM_DUNGEON = 4,    //团队地下城（通关）
        OACT_3V3_PK = 5,      //3V3积分赛（参与）
        OACT_GUILD_BATTLE = 6,    //公会战（包括攻城战）（参与）
        OACT_GUILD_DUNGEON = 7,    //公会地下城（参与）
        OACT_2V2_PK = 8,           //2v2pk
    };
    public class IntegrationChallengeActivity : LimitTimeCommonActivity
    {
        private readonly LimitTimeActivityCheckComponent mCheckComponent = new LimitTimeActivityCheckComponent();
        public override bool IsHaveRedPoint()
        {
            return !mCheckComponent.IsChecked();
        }
        public override void Show(Transform root)
        {
            base.Show(root);
            mCheckComponent.Checked(this);
        }
        protected override string _GetPrefabPath()
        {
            string prefabPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/IntegrationChallengeActivity";
            if (mDataModel != null && !string.IsNullOrEmpty(mDataModel.ActivityPrefafPath))
            {
                prefabPath = mDataModel.ActivityPrefafPath;
                return prefabPath;
            }
            return prefabPath;
        }

        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/IntegrationChallengeItem";
        }
    }
}
