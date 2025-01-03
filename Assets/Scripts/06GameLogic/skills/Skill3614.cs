using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//狂乱锤击
public class Skill3616 : Skill3614
{
    public Skill3616(int sid, int skillLevel) : base(sid, skillLevel) { }
}

//疾风旋风破
public class Skill3614 : BeSkill
{

    public Skill3614(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnInit()
    {
        base.OnInit();
    }

    public override void OnStart()
    {
        SetJumpBack();
    }

    protected void SetJumpBack()
    {
        if (BattleMain.IsModePvP(battleType))
            return;
        canPressJumpBackCancel = true;
    }
}
