using System.Collections.Generic;
using ProtoTable;
using UnityEngine;

//圣光沁盾 技能机制
public class Mechanism2018 : BeMechanism
{
    public Mechanism2018(int mid, int lv) : base(mid, lv) { }

    protected VInt[] boxXY = new VInt[2];  //盾的碰撞框XY轴大小(X|Y) 
    private VInt xSpeed = 0;  //添加X轴推力大小
    private List<int> monsterTypeList = new List<int>();    //可以推开的怪物类型

    public override void OnInit()
    {
        base.OnInit();
        boxXY[0] = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000);
        boxXY[1] = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[1], level), GlobalLogic.VALUE_1000);

        xSpeed = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueB[0], level), GlobalLogic.VALUE_1000);

        for (int i = 0; i < data.ValueD.Count; i++)
        {
            monsterTypeList.Add(TableManager.GetValueFromUnionCell(data.ValueD[i], level));
        }
    }

    public override void OnReset()
    {
        monsterTypeList.Clear();
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        CheckTarget(deltaTime);
    }

    //检查符合条件的怪物
    private void CheckTarget(int deltaTime)
    {
        if (owner == null || owner.CurrentBeScene == null)
            return;
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindFaceTargetsX(list, owner, boxXY[0]);
        for (int i = 0; i < list.Count; i++)
        {
            if (Mathf.Abs(list[i].GetPosition().y - owner.GetPosition().y) <= boxXY[1])
                ChangeEnemySpeed(list[i]);
        }
        GamePool.ListPool<BeActor>.Release(list);
    }

    //对满足条件的怪物造成一次触发效果伤害
    private void ChangeEnemySpeed(BeActor actor)
    {
        if (actor == null || actor.IsDead())
            return;
        if (actor.IsMonster() && !monsterTypeList.Contains(actor.GetEntityData().type))
            return;
        if (actor.stateController == null)
            return;
        if (actor.stateController != null && (!actor.stateController.CanMove() || !actor.stateController.CanMoveX()))
            return;
        if (actor.stateController.CanNotAbsorbByBlockHole())
            return;
        if (actor.moveXSpeed.i * owner.moveXSpeed.i > 0)
            return;
        actor.ClearMoveSpeed((int)SpeedCear.SPEEDCEAR_X);
        VInt speed = owner.GetFace() ? -xSpeed : xSpeed;
        actor.extraSpeed.x = speed.i;
    }
}
