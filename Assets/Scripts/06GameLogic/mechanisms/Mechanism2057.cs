using GameClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// 团本机制19--boss
/// </summary>
public class Mechanism2057 : BeMechanism
{
    private int skillID = 21216;
    private int buffID = 521633;//场景混沌buff
    private int blindBuffId = 521631;//失明buff
    private int weakBuffID = 521632;//虚弱BUFF
    private int endBuffID = 521636;//正常状态结束后附加的BUFF
    private bool flag = false;
    private int monsterNum = 0;
    private int monsterCnt = 0;
    private bool timeStop = false;
    GoblinKingdomFrame uiFrame = null;
    int totalTime;                              //爆炸时间
    private int[] monsterIDs = new int[] { 81110011, 81120011 };
    public Mechanism2057(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
        monsterNum = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        totalTime = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        if(data.ValueC.Count == 2)
        {
            monsterIDs[0] = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
            monsterIDs[1] = TableManager.GetValueFromUnionCell(data.ValueC[1], level);
        }

        if (data.ValueD.Count > 0)
        {
            blindBuffId = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        }
    }

    public override void OnReset()
    {
        flag = false;
        monsterCnt = 0;
        timeStop = false;
        uiFrame = null;
        monsterIDs = new int[] { 81110011, 81120011 };
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onHPChange, (args) =>
        {
            if (!flag && owner.attribute.GetHPRate() <= VFactor.NewVFactor(20, 100))
            {
                flag = true;
                owner.UseSkill(skillID,true);
                owner.buffController.TryAddBuff(buffID, -1);
                OpenFrame();
            }
        });
        handleB = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onMonsterDead, (args) => 
        {
            if (timeStop) return;
            BeActor monster = args.m_Obj as BeActor;
            if (monster != null)
            {
                if (Array.IndexOf(monsterIDs, monster.GetEntityData().monsterID) != -1)
                {
                    monsterCnt++;
                    if (monsterCnt >= monsterNum && totalTime > 0)
                    {
                        timeStop = true;
                        RestoreSceneClearMonster(true);
                        owner.buffController.TryAddBuff(weakBuffID, -1);
                    }
                    if (uiFrame != null)
                    {
                        uiFrame.SetNumText((monsterNum-monsterCnt).ToString());
                    }
                }
            }
        });
    }

    public bool IsTimeStop()
    {
        return timeStop;
    }

    private void RestoreSceneClearMonster(bool success)
    {
        //移除混沌场景
        owner.buffController.RemoveBuff(buffID);

        //清楚幻影
        List<BeActor> list = new List<BeActor>();
        for (int i = 0; i < monsterIDs.Length; i++)
        {
            owner.CurrentBeScene.FindActorById2(list, monsterIDs[i]);
            for (int j = 0; j < list.Count; j++)
            {
                list[j].DoDead();
            }
        }

        //关闭界面
#if !LOGIC_SERVER

        CreateEffect(success);
        if (uiFrame != null)
        {
            uiFrame.Close();
            uiFrame = null;
        }
#endif

        //移除角色的失明效果
        BeUtility.GetAllEnemyPlayers(owner, list);//体验服BugList反馈，之前接口只能找到不死亡的玩家对于死亡玩家则跳过，会导致玩家无法移除机制
        for (int i = 0; i < list.Count; i++)
        {
            var curActor = list[i];
            if (curActor == null) continue;
            curActor.buffController.RemoveBuff(blindBuffId);
            if (!success)
            {
                if (!curActor.IsDead()) 
                {
                    curActor.DoDead();
                    curActor.GetEntityData().SetHP(0);
                    if(curActor.m_pkGeActor != null)
                        curActor.m_pkGeActor.SyncHPBar();
                }
            }
        }
    }

    private void CreateEffect(bool success)
    {
        string path = success ?
       "Effects/Monster_TB02/Prefab/Eff_Monster_TB02_kexila_Emeng_Mengyan_06"
       : "Effects/Monster_TB02/Prefab/Eff_Monster_TB02_kexila_Emeng_Mengyan_05";
        GameObject obj = AssetLoader.instance.LoadResAsGameObject(path);
        Utility.AttachTo(obj, ClientSystemManager.instance.GetLayer(FrameLayer.TopMost));
        int time = success ? 2580 : 5600;
        ClientSystemManager.instance.delayCaller.DelayCall(time, () =>
        {
            if(obj!=null)
              GameObject.Destroy(obj);
        });
    }

    private void OpenFrame()
    {
#if !LOGIC_SERVER
        uiFrame = ClientSystemManager.instance.OpenFrame<GoblinKingdomFrame>() as GoblinKingdomFrame;
        uiFrame.SetNumText(monsterNum.ToString());
#endif
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (!flag) return;
        if (timeStop) return;
        UpdateTotalTime(deltaTime);
    }

    void UpdateTotalTime(int deltaTime)
    {
        totalTime -= deltaTime;
        if (totalTime > 0)
        {
            if (uiFrame != null)
            {
                int m = totalTime / 60000;
                int s = (totalTime % 60000) / 1000;
                int ms = (totalTime % 1000) / 10;
                string text = string.Format("{0}.{1:D2}.{2:D2}", m, s, ms);
                uiFrame.SetTimeText(text);
            }
        }
        else
        {
            timeStop = true;
            RestoreSceneClearMonster(false);
            owner.buffController.TryAddBuff(endBuffID,-1);
        }
       
    }

    public override void OnFinish()
    {
        base.OnFinish();
    }
}
