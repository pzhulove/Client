using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AdventureTeamContentIntroducationView : AdventureTeamContentBaseView
    {

        [Space(10)] [HeaderAttribute("LevelIncome")] [SerializeField]
        private AdventureTeamContentLevelIncomeControl incomeControl;

        public override void InitData()
        {
            if (incomeControl != null)
            {
                incomeControl.TryInitBaseInfoView();
            }
            _TryRefreshView();
        }

        public override void OnEnableView()
        {
            _TryRefreshView();
        }

        private void _TryRefreshView()
        {
            AdventureTeamDataManager.GetInstance().ReqAdventureTeamExtraInfo();
            AdventureTeamDataManager.GetInstance().ReqBlessCrystalInfo();

            //提前刷新角色收藏数据
            var baseJobIds = AdventureTeamDataManager.GetInstance().GetTotalBaseJobTabIds();
            AdventureTeamDataManager.GetInstance().ReqOwnJobInfo(baseJobIds);

            //打开界面需要//清空标记的页签类型
            AdventureTeamDataManager.GetInstance().OnAdventureTeamLevelChangedFlag = false;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamBaseInfoFrameOpen);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.AdventureTeam);
        }
    }
}