using UnityEngine;
using System.Collections.Generic;

namespace GameClient
{
	public class NewbieEquipmentGuide : NewbieGuideDataUnit
    {
		public NewbieEquipmentGuide(int tid):base(tid){}

		public override void InitContent()
		{
            List<NewbieModifyData> ModifyDataTypeList = new List<NewbieModifyData>();
            NewbieModifyData data = new NewbieModifyData();
            data.iIndex = 1;
            data.ModifyDataType = NewBieModifyDataType.EquipInPackagePos;
            ModifyDataTypeList.Add(data);

            List<NewbieModifyData> ModifyDataTypeList2 = new List<NewbieModifyData>();
            NewbieModifyData data2 = new NewbieModifyData();
            data2.iIndex = 0;
            data2.ModifyDataType = NewBieModifyDataType.PackageEquipTipsGuidePos;
            ModifyDataTypeList2.Add(data2);

            AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ClientSystemTownFrame",
				MainUIIconPath.packge,
				"打开背包，查看\n获得的<color=#fcff21>新装备</color>吧",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
				TextTipType.TextTipType_Three,
                new Vector3(0,0,0),
                eNewbieGuideAgrsName.SaveBoot
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.50f
            ));

            AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
                ModifyDataTypeList,
                "PackageFrame",
                MainUIIconPath.PackageNewFrameItemPath + "/ItemRoot_{0}/{1}/ItemGroup/Icon",
				"选择装备",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(0, 0, 0)
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
                ModifyDataTypeList2,
                "tipItemFrame{0}",
                "Func/Special",
				"点击穿戴",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(-100, 0, 0)
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.INTRODUCTION2,
                null,
				"冒险家大人，及时<color=#fcff21>更换装备</color>可以\n有效提升战力！获得新装备时请\n务必及时处理喔！"
            ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ItemGroupFrame",
				"BG/Title/Close",
				"",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(0, 0, 0)
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.TALK_DIALOG,
				null,
				22030
			));

        }

		public override void InitCondition()
		{
			AddCondition(new NewbieConditionData(
				eNewbieGuideCondition.Level,
				new int[] { 2 }
			));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.FinishedMissionID,
                new int[] { 2302 }
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.SceneTown
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.ClientSystemTownFrameOpen
            ));

//             AddCondition(new NewbieConditionData(
//                 eNewbieGuideCondition.MainFrameMutex
//             ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.EquipmentInPackage
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.MagicBoxGuide
            ));

        }
	}
}

