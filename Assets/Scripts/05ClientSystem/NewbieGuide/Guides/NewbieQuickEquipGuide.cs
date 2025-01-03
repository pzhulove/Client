using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieQuickEquipGuide : NewbieGuideDataUnit
	{
		public NewbieQuickEquipGuide(int tid):base(tid){}

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
                "又有新装备了，快去换上吧",
                ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
                TextTipType.TextTipType_Three,
                new Vector3(0, 0, 0),
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
                "ItemListView/Viewport/Content/ItemRoot_{0}/{1}/ItemGroup/Icon",
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
                NewbieGuideComType.BUTTON,
                null,
                "ItemGroupFrame",
                "BG/Title/Close",
                "",
                ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
                TextTipType.TextTipType_Two,
                new Vector3(0, 0, 0)
            ));

            //             AddContent(new ComNewbieData(
            // 				NewbieGuideComType.BUTTON,
            // 				null,
            //                 "EquipmentChangedFrame",
            //                 "back/BtnWearImmediately",
            // 				"获取更好的装备时，可以通过\n<color=#fcff21>快速穿戴</color>便捷换装~",
            // 				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
            // 				TextTipType.TextTipType_Three,
            // 				new Vector3(-183,17.5f,0),
            //                 eNewbieGuideAgrsName.SaveBoot,
            //                 eNewbieGuideAgrsName.PauseBattle,
            //                 "back"
            //             ));

            //             AddContent(new ComNewbieData(
            // 				NewbieGuideComType.TALK_DIALOG,
            // 				null,
            // 				22040
            // 			));
        }

		public override void InitCondition()
		{
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.Level,
                new int[] { 3 }
            ));

            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.FinishedMissionID,
                new int[] { 2202 }
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

//             AddCondition(new NewbieConditionData(
//                 eNewbieGuideCondition.QuickEquipFrameOpen
//             ));
        }
	}
}

