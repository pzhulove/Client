using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class NewbieDuelGuide : NewbieGuideDataUnit
	{
		public NewbieDuelGuide(int tid):base(tid){}

		public override void InitContent()
		{
// 			AddContent(new ComNewbieData(
// 				NewbieGuideComType.BUTTON,
// 				null,
// 				"ClientSystemTownFrame",
// 				MainUIIconPath.battle,
// 				"决斗场开启啦！快去挑战一下吧",
// 				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
// 				TextTipType.TextTipType_Three,
// 				new Vector3(0,0,0),
//                 eNewbieGuideAgrsName.SaveBoot
//             ));

			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION2,
				null,
				"欢迎来到决斗场\n与其他冒险者<color=#ff0000ff>决斗</color>可以\n提高自己的决斗段位\n赢取丰厚的决斗奖励"
			));
			AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION2,
				null,
				"点击左侧<color=#ff0000ff>奖励查看</color>可以显示你当前战绩\n您也可以通过右侧<color=#ff0000ff>好友挑战</color>和<color=#ff0000ff>自由练习</color>\n功能进行决斗练习"
			));

            AddContent(new ComNewbieData(
				NewbieGuideComType.INTRODUCTION,
                null,
                "PkWaitingRoom",
				"btBegin",
				"有兴趣的话，可以尝试匹配对手开始决斗。",
				ComNewbieGuideBase.eNewbieGuideAnchor.TopLeft,
				TextTipType.TextTipType_Two,
				new Vector3(250,90,0)
            ));
//
//            AddContent(new ComNewbieData(
//                NewbieGuideComType.BUTTON,
//                null,
//                "PkMenuFrame",
//                "middle/Scroll View/Viewport/Content/btSetting",
//                "选择设置",
//				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
//                TextTipType.TextTipType_Two
//            ));
//
//            AddContent(new ComNewbieData(
//                NewbieGuideComType.INTRODUCTION,
//                null,
//                "SettingFrame",
//                "Panel/ToggleRunInBattle",
//				"<color=#ff0000ff>拖拽跑动</color>易于操控，<color=#ff0000ff>双击跑动</color>可以更加精确控制，您可以根据您的情况进行选择",
//				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
//				TextTipType.TextTipType_Three,
//                new Vector3(-800,-400,0),
//                eNewbieGuideAgrsName.None, 
//                eNewbieGuideAgrsName.None,
//                "Panel/ToggleRunInBattle"
//            ));
//
////            AddContent(new ComNewbieData(
////                NewbieGuideComType.WAIT,
////                null,
////                2.0f,
////                true
////            ));
//
//            AddContent(new ComNewbieData(
//                NewbieGuideComType.BUTTON,
//                null,
//                "SettingFrame",
//                "Panel/BtnClose",
//                "如果不适应当前的模式，可随时到设置中修改~",
//				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
//				TextTipType.TextTipType_Two,
//				new Vector3(-750,-300,0)
//            ));
//
//            AddContent(new ComNewbieData(
//				NewbieGuideComType.BUTTON,
//				null,
//                "PkWaitingRoom",
//                "btSkill",
//				"再来看一下技能吧",
//				ComNewbieGuideBase.eNewbieGuideAnchor.TopRight,
//				TextTipType.TextTipType_Two,
//				new Vector3(-500,-200,0)
//			));
//
//			AddContent(new ComNewbieData(
//				NewbieGuideComType.INTRODUCTION,
//				null,
//				"SkillTreeFrame",
//				"right/midddle/Func/Func2",
//				"技能在决斗场有不同属性，更换技能时记得查看哦~",
//				ComNewbieGuideBase.eNewbieGuideAnchor.Top,
//				TextTipType.TextTipType_Two,
//				new Vector3(-300,0,0)
//			));
//
//			AddContent(new ComNewbieData(
//				NewbieGuideComType.INTRODUCTION2,
//				null,
//				"多多查看技能属性，调\n整技能配置，可以使你\n在决斗场无往不利"
//			));
		}

		public override void InitCondition()
		{
            AddCondition(new NewbieConditionData(
                eNewbieGuideCondition.Level,
                new int[] { 10 }
            ));

            AddCondition(new NewbieConditionData(
				eNewbieGuideCondition.ScenePkWaitingRoom
            ));

//             AddCondition(new NewbieConditionData(
//                 eNewbieGuideCondition.MainFrameMutex
//             ));
        }
	}
}

