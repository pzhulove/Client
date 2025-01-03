using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 瞬移
*/
public enum TeleportType
{
	TARGET = 0, 		//攻击对象的位置
	MONSTERID,			//指定怪物ID的位置
	MONSTERID_TARGET,	//指定怪物ID的攻击对象的位置
    SCENE_CENTER,        //场景中心点
    ENTITYID,            //指定实体ID的位置
    RANDOM_SCENE        //场景随机点

}

public class Mechanism12 : BeMechanism {

	TeleportType telType;
	int monsterID;
	VInt offset = 0;
    bool isGoBack = false;
    int delay = 0;

	public Mechanism12(int mid, int lv):base(mid, lv)
	{
	}

    public override void OnReset()
    {
        telType = TeleportType.TARGET;
        monsterID = 0;
        offset = 0;
        isGoBack = false;
        delay = 0;
    }
	public override void OnInit ()
	{	
		telType = (TeleportType)TableManager.GetValueFromUnionCell(data.ValueA[0], level);
		if (telType > TeleportType.TARGET)
			monsterID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
		offset = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueC[0], level),GlobalLogic.VALUE_1000);
        if (data.ValueD.Count > 0)
            isGoBack = TableManager.GetValueFromUnionCell(data.ValueD[0], level) > 0;
        if (data.ValueE.Count > 0)
            delay = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
	}

	public override void OnStart ()
	{
		var pos = owner.GetPosition();
		VInt3 targetPos = pos;
		if (telType == TeleportType.TARGET)
		{
			if (owner.aiManager.aiTarget != null)
			{
				targetPos = owner.aiManager.aiTarget.GetPosition();
                if (isGoBack)
                {
                    owner.SaveCurrentPosition();
                }
			}
		}
		else if (telType == TeleportType.MONSTERID)
		{
			List<BeActor> targets = GamePool.ListPool<BeActor>.Get();
			owner.CurrentBeScene.FindMonsterByID(targets, monsterID);
			if (targets.Count > 0 && targets[0] != null)
				targetPos = targets[0].GetPosition();
			GamePool.ListPool<BeActor>.Release(targets);
		}
		else if (telType == TeleportType.MONSTERID_TARGET)
		{
			List<BeActor> targets = GamePool.ListPool<BeActor>.Get();
			owner.CurrentBeScene.FindMonsterByID(targets, monsterID);
			if (targets.Count > 0 && targets[0] != null && targets[0].aiManager.aiTarget!= null)
			{
				targetPos = targets[0].aiManager.aiTarget.GetPosition();
			}
			GamePool.ListPool<BeActor>.Release(targets);
        }
        else if (telType == TeleportType.SCENE_CENTER)
        {
            if (owner.aiManager.aiTarget != null)
            {
                targetPos = owner.CurrentBeScene.GetSceneCenterPosition();
            }
        }
        else if (telType == TeleportType.ENTITYID)
        {
            BeEntity entity = owner.CurrentBeScene.GetEntityByResId(monsterID, owner);
            if (entity != null && !entity.IsDead())
            {
                targetPos = entity.GetPosition();
            }
        }
        else if (telType == TeleportType.RANDOM_SCENE)
        {
            targetPos = owner.CurrentBeScene.GetRandomPos(20);
            if (owner.CurrentBeScene.IsInBlockPlayer(targetPos))
                targetPos = owner.CurrentBeScene.GetSceneCenterPosition();
        }

        if (pos.x > targetPos.x)
		{
			targetPos.x += offset.i;
			owner.SetFace(true);
		}
		else if (pos.x < targetPos.x)
		{
			targetPos.x -= offset.i;
			owner.SetFace(false);
		}

        if (delay > 0)
        {
            owner.delayCaller.DelayCall(delay, () =>
            {
                _moveToTargetPos(targetPos);
            });
        }
        else
        {
            _moveToTargetPos(targetPos);
        }
	}

    void _moveToTargetPos(VInt3 targetPos)
    {
        if (owner != null && !owner.IsDead())
        {
            if (telType == TeleportType.SCENE_CENTER)
            {
                owner.SetPosition(targetPos);
            }
            else
            {
                VInt3 newPos = BeAIManager.FindStandPositionNew(targetPos, owner.CurrentBeScene, owner.GetFace(), false, 30);          //防止瞬移在阻挡内
                owner.SetPosition(newPos);
            }
        }
    }

    public override void OnFinish()
    {
        if (isGoBack)
        {
            if (owner != null && !owner.IsDead())
            {
                owner.SetPosition(owner.savedPosition);
            }
            isGoBack = false;
        }
    }

}
