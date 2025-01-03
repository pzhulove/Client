using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class NewbieBreakDownHavenotWhiteGuide : NewbieGuideDataUnit
    {
        public NewbieBreakDownHavenotWhiteGuide(int tid) : base(tid) { }

        public override void InitContent()
        {
            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "ClientSystemTownFrame",
                MainUIIconPath.packge,
                "多余的装备，可以进行一键分解获取水晶哦！",
                ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
                TextTipType.TextTipType_Two,
                new Vector3(-100, 0, 0),
                eNewbieGuideAgrsName.SaveBoot
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "PackageNewFrame",
                "Content/Bottom/Ctrl/QuickDecompose",
                "点击一键分解按钮。",
                ComNewbieGuideBase.eNewbieGuideAnchor.ButtomLeft,
                TextTipType.TextTipType_Two,
                new Vector3(-50, 80, 0)
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.INTRODUCTION2,
                null,
                "选择相应品质的装备后，就可以一键分解啦。"
            ));


        }

        public override void InitCondition()
        {

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.FinishedMissionID,
                new int[] { 2213 }
            ));
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.ClientSystemTownFrameOpen
            ));
        }
    }
}
