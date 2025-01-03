using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 场外击飞机制
/// </summary>
public class Mechanism1049 : BeMechanism
{
    public Mechanism1049(int id, int level) : base(id, level) { }

    private VInt[] speedArr = new VInt[3];  //三个方向的速度 

    public override void OnInit()
    {
        base.OnInit();
        speedArr = new VInt[3];
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            speedArr[i] = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[i], level), GlobalLogic.VALUE_1000);
        }
    }


    public override void OnStart()
    {
        base.OnStart();
        owner.SetRestrainPosition(false);
        owner.stateController.SetAbilityEnable(BeAbilityType.BLOCK, false);
        owner.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE_NO_ALPHA, GlobalLogic.VALUE_10000);


        owner.FrozenDisMax = GlobalLogic.VALUE_100000;

        owner.ClearMoveSpeed();

        //owner.Locomote(new BeStateData((int)ActionState.AS_HURT, 0, 0, 0, 0, 0, GlobalLogic.VALUE_10000, true));
        owner.Locomote(new BeStateData((int)ActionState.AS_HURT) { _timeout = GlobalLogic.VALUE_10000, _timeoutForce = true });

        owner.SetMoveSpeedX(owner.GetFace() ? speedArr[0] : -speedArr[0]);
        owner.SetMoveSpeedY(speedArr[1]);
        owner.SetMoveSpeedZ(speedArr[2]);
    }

    public override void OnFinish()
    {
        base.OnFinish();

        owner.Locomote(new BeStateData((int)ActionState.AS_FALL));

        owner.stateController.SetAbilityEnable(BeAbilityType.BLOCK, true);
        owner.ClearMoveSpeed();
        
        owner.SetRestrainPosition(true);
        VInt3 targetPos = owner.CurrentBeScene.GetSceneCenterPosition();
        targetPos.z = 35000;
        owner.SetPosition(targetPos);

        owner.buffController.RemoveBuff((int)GlobalBuff.INVINCIBLE_NO_ALPHA);
    }
}
