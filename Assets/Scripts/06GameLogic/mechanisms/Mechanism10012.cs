using System.Collections;
using System.Collections.Generic;

//监听玩家放技能时，释放某技能
public class Mechanism10012 : BeMechanism
{
    int skillId;

    public Mechanism10012(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        skillId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }

    public override void OnStart()
    {
        var targets = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        for (int i = 0; i < targets.Count; i++)
        {
            var target = targets[i];
            if (target != null && target.playerActor != null && !target.playerActor.IsDead())
            {
                var handle = target.playerActor.RegisterEventNew(BeEventType.onCastSkill, param =>
                {
                    var actor = param.m_Obj as BeActor;
                    if (actor != null && owner.aiManager != null)
                    {
                        owner.aiManager.aiTarget = actor;
                        owner.UseSkill(skillId);
                    }
                });
                AddHandle(handle);
            }
        }
    }
}
