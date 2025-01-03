using System;
using System.Collections.Generic;

//对场上所有拥有指定BUFF的玩家  以及场上所有指定ID的实体  以上所有目标的位置创建实体机制，并且释放指定技能ID
public class Mechanism10018 : BeMechanism
{
    private int _targetPlayerBuffId;
    private int _targetProjectileId;
    private int _createProjectileId;
    private VInt3 _createProjectileOffset;
    private int _createProjectileDelay;
    private int _castSkillIdInCondition;
    private int _castSkillIdOutCondition;

    public Mechanism10018(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        _targetPlayerBuffId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        _targetProjectileId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        _createProjectileId = TableManager.GetValueFromUnionCell(data.ValueC[0], level);

        int[] offset = new int[3];
        for (int i = 0; i < 3; i++)
        {
            int value = i < data.ValueD.Length ? TableManager.GetValueFromUnionCell(data.ValueD[i], level) : 0;
            offset[i] = VInt.NewVInt(value, GlobalLogic.VALUE_1000).i;
        }
        _createProjectileOffset = new VInt3(offset[0], offset[1], offset[2]);
        
        _createProjectileDelay = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        _castSkillIdInCondition = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
        _castSkillIdOutCondition = TableManager.GetValueFromUnionCell(data.ValueG[0], level);
    }

    public override void OnStart()
    {
        bool hasInCondition = false;
        
        List<BeEntity> list = GamePool.ListPool<BeEntity>.Get();
        owner.CurrentBeScene.GetEntitys2(list);
        foreach (var entity in list)
        {
            bool bInCondition = false;
            BeActor actor = entity as BeActor;
            if (actor != null && actor.isMainActor && actor.buffController.HasBuffByID(_targetPlayerBuffId) != null)
            {
                bInCondition = true;
            }

            BeProjectile projectile = entity as BeProjectile;
            if (projectile != null && projectile.m_iID == _targetProjectileId)
            {
                bInCondition = true;
            }

            if (bInCondition)
            {
                owner.delayCaller.DelayCall(_createProjectileDelay, () =>
                {
                    if (!owner.IsDead() && !entity.IsDead())
                    {
                        var projectilePos = entity.GetPosition() + _createProjectileOffset;
                        owner.AddEntity(_createProjectileId, projectilePos, level);
                    }
                });
               
                hasInCondition = true;
            }
        }
        GamePool.ListPool<BeEntity>.Release(list);

        int castSkillId = hasInCondition ? _castSkillIdInCondition : _castSkillIdOutCondition;
        owner.UseSkill(castSkillId, true);
    }
    
    
}
