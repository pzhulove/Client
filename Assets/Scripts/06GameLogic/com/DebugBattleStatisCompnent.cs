using System;
using System.Collections.Generic;
using System.Collections;
///////删除linq
using System.Text;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Reflection;

using DG.Tweening;
using UnityEngine.EventSystems;
namespace GameClient
{
	public class DebugBattleStatisCompnent
	{
		GameObject objBattleSta;
		Text textBattleOut;
		Text textBattleStat;
		ScrollRect scroll;
		Text textInput;

		struct BattleStatics
		{
			public int attackTotalNum;
			public int normalNum;
			public int criticalNum;
			public int missNum;
		}

		BattleStatics battleInfo;

		public void _loadBattleStatisticsUI()
		{
			objBattleSta = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/EditorHUD/BattleInfoPanel");
			Utility.AttachTo(objBattleSta, ClientSystemManager.instance.BottomLayer);

			Text[] texts = objBattleSta.GetComponentsInChildren<Text>();
			if (texts != null)
			{
				for (int i = 0; i < texts.Length; ++i)
				{
					if (texts[i].gameObject.name == "Text")
						textBattleOut = texts[i];
					else if (texts[i].gameObject.name == "BattleInfoStaticText")
						textBattleStat = texts[i];
					else if (texts[i].gameObject.name == "InputText")
						textInput = texts[i];
				}
			}

			scroll = objBattleSta.GetComponentInChildren<ScrollRect>();

			Button btnCreateMonster = objBattleSta.GetComponentInChildren<Button>();
			if (btnCreateMonster != null)
			{
				btnCreateMonster.onClick.AddListener(
					() =>
					{
						int monsterID = 0;

						try
						{
							monsterID = Int32.Parse(textInput.text);
						}
						catch (Exception e)
						{

						}

						if (monsterID == 0)
						{

							//return;
							monsterID = Global.Settings.defaultMonsterID;
						}


						var target = BattleMain.instance.GetPlayerManager().GetMainPlayer().playerActor;
						var enemy = BattleMain.instance.Main.CreateMonster(monsterID);
						if (enemy != null)
						{
							enemy.StartAI(target);
							var pos = target.GetPosition();
							Vec3 targetPos = new Vec3(pos.fx + UnityEngine.Random.Range(-2, 2), pos.fy + 1, pos.fz);
							enemy.SetPosition(new VInt3(targetPos));

							var monsterData = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(monsterID);
							if (monsterData != null)
							{
								if (monsterData.FloatValue > 0)
									enemy.SetFloating(VInt.NewVInt(monsterData.FloatValue,1000));
							}
						}
					}
				);
			}
		}

		public void BS_AddBattleInfo(string attacker, string defender, AttackResult result, int damage)
		{
			if (textBattleOut == null)
				return;

			battleInfo.attackTotalNum++;

			string content = string.Format("{0} 攻击 {1}, ", attacker, defender);
			if (result == AttackResult.MISS)
			{
				content += "<color=gray>未命中</color>";
				battleInfo.missNum++;
			}
			else if (result == AttackResult.NORMAL)
			{
				content += "<color=yellow>命中</color>";
				content += string.Format(" 并造成 <color=yellow>{0}</color> 伤害", damage);
				battleInfo.normalNum++;
			}
			else if (result == AttackResult.CRITICAL)
			{
				content += "<color=red>暴击</color>";
				content += string.Format(" 并造成 <color=red>{0}</color> 伤害", damage);
				battleInfo.criticalNum++;
			}

			textBattleOut.text += content;
			textBattleOut.text += "\n";

			if (scroll != null)
			{
				scroll.verticalNormalizedPosition = 0f;
			}

			if (textBattleStat != null)
			{
				string stat = string.Format("攻击总次数:{0} 普通:<color=yellow>{1}</color> 暴击:<color=red>{2}</color> MISS：{3}",
					battleInfo.attackTotalNum, battleInfo.normalNum, battleInfo.criticalNum, battleInfo.missNum);
				textBattleStat.text = stat;
			}
		}
	}

}

