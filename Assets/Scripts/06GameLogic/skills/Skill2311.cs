using UnityEngine;
using System.Collections.Generic;
using GameClient;

public class Skill2311 : BeSkill
{
    public Skill2311(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    public override void OnPostInit()
    {
        canPressJumpBackCancel = false;
        startJumpBackCnacelFlag = "231100";
        endJumpBackCnacelFlag = "231101";
    }


}
