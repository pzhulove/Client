using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieForgeGuide : NewbieGuideDataUnit
	{
		public NewbieForgeGuide(int tid):base(tid){}

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

			List<NewbieModifyData> ModifyDataTypeList3 = new List<NewbieModifyData>();
			NewbieModifyData data3 = new NewbieModifyData();
			data3.iIndex = 1;
			data3.ModifyDataType = NewBieModifyDataType.ChangedEquipInPackagePos;
			ModifyDataTypeList3.Add(data3);

            List<NewbieModifyData> ModifyDataTypeList4 = new List<NewbieModifyData>();
            NewbieModifyData data4 = new NewbieModifyData();
            data4.iIndex = 1;
            data4.ModifyDataType = NewBieModifyDataType.ActorShowEquipPos;
            ModifyDataTypeList4.Add(data4);

            AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"ClientSystemTownFrame",
				MainUIIconPath.packge,
				"赶快来穿上武器，点击打开背包界面",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
				TextTipType.TextTipType_Three,
				new Vector3(),
				eNewbieGuideAgrsName.SaveBoot
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.WAIT,
				null,
				0.6f
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				ModifyDataTypeList,
				"PackageFrame",
                MainUIIconPath.PackageNewFrameItemPath + "/ItemRoot_{0}/{1}/ItemGroup/Icon",
				"选择武器",
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
				new Vector3(-100, 20, 0)
			));

            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                0.30f
            ));

            AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				ModifyDataTypeList3,
				"PackageFrame",
                MainUIIconPath.PackageNewFrameItemPath + "/ItemRoot_{0}/{1}/ItemGroup/Icon",
				"然后我们来<color=#ff0000ff>分解</color>掉废旧装备",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(-400, 0, 0)
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				ModifyDataTypeList2,
				"tipItemFrame{0}",
				"Func/Decompose",
				"点击分解",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(-100, 20, 0)
			));

            AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"CommonMsgBoxOKCancel",
				"Back/Panel/btOK",
				"分解获得的材料还可以进行<color=#ff0000ff>强化</color>等操作",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(-120, 30, 0)
			));

            AddContent(new ComNewbieData(
                NewbieGuideComType.WAIT,
                null,
                2.8f
            ));

            AddContent(new ComNewbieData(
                NewbieGuideComType.BUTTON,
                null,
                "DecomposeResultFrame",
                "Result/Title/Close",
                "点击关闭",
                ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
                TextTipType.TextTipType_Two,
                new Vector3(0, 0, 0)
            ));

            AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
                ModifyDataTypeList4,
				"ActorShowFrame",
                MainUIIconPath.EquipItemPath + "/{0}/{1}/ItemGroup/Icon",
				"选择刚获得的武器",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(0, 0, 0)
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				ModifyDataTypeList2,
				"tipItemFrame0",
				"Func/Strengthen",
				"点击强化",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(-200, 20, 0)
			));
				
			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
                "SmithShopFrame",
                "Strengthen/Matchine/CostContent/BtnStrength",
				"开始强化~",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(0, 20, 0)
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.WAIT,
				null,
				2.30f
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION2,
				null,
				"锻冶屋可以强化装备，品阶调整，\n再次封装等功能，是提升<color=#ff0000ff>装备属性</color>\n的主要场所"
			));


			AddContent(new ComNewbieData(
				NewbieGuideComType.PASS_THROUGH,
				null,
				"StrengthenResultFrame",
				"ok_10down/close"
			));

			AddContent(new ComNewbieData(
				NewbieGuideComType.BUTTON,
				null,
				"SmithShopFrame",
				"ComWnd/Title/Close",
				"",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
				TextTipType.TextTipType_Two,
				new Vector3(0, 0, 0)
			));

        }

		public override void InitCondition()
		{
			AddCondition(new NewbieConditionData(
				eNewbieGuideCondition.Level,
				new int[] { 9 }
			));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.FinishedMissionID,
                new int[] { 2207 }
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

