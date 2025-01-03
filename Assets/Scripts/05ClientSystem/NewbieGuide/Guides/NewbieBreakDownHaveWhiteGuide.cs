using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class NewbieBreakDownHaveWhiteGuide : NewbieGuideDataUnit
    {
        public NewbieBreakDownHaveWhiteGuide(int tid) : base(tid) { }

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
                new Vector3(0, 0, 0),
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
                NewbieGuideComType.TOGGLE,
                null,
                "PackageNewFrame",
                "Content/DecomposeRoot/DecomposeGroup(Clone)/SelectGroup/Toggle1",
                "选择白色页签。",
                ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
                TextTipType.TextTipType_Two,
                new Vector3(-50, 30, 0)
            ));
            

            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "PackageNewFrame",
                "Content/DecomposeRoot/DecomposeGroup(Clone)/Confirm",
                "点击开始分解吧！",
                ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
                TextTipType.TextTipType_Two,
                new Vector3(-50, 30, 0)
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "CommonMsgBoxOKCancel",
                "normal/Back/Panel/btOK",
                "点击确定按钮吧！",
                ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
                TextTipType.TextTipType_Two,
                new Vector3(-50, 30, 0)
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
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.HaveWhiteEquipment
            ));
        }
    }
}
