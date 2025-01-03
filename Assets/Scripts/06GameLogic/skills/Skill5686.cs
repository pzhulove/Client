using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill5686 : BeSkill {

    protected int monsterID = 30300011;
    
    List<BeActor> list = new List<BeActor>();
    int maxCnt = 3;
    public Skill5686(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override bool CanUseSkill()
    {
        return base.CanUseSkill() && _canUseSkill();
    }

    private bool _canUseSkill()
    {
        int cnt = 0;
        owner.CurrentBeScene.FindMonsterByID(list, monsterID);
        cnt += list.Count;
        return cnt < maxCnt;
    }
}

public class Skill5688 : Skill5686
{
    
    public Skill5688(int sid, int skillLevel) : base(sid, skillLevel)
    {
        monsterID = 30310013;
    }

}
