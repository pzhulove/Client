using UnityEngine;
using System.Collections;
using GameClient;

public class Skill2113 : BeSkill
{

    int buffID;
	IBeEventHandle handle = null;

    public Skill2113(int sid, int skillLevel):base(sid, skillLevel)
    {

    }

    public static void SkillPreloadRes(ProtoTable.SkillTable tableData)
    {
#if !LOGIC_SERVER
        PreloadManager.PreloadResID(TableManager.GetValueFromUnionCell(tableData.ValueA[0], 1), null, null);
#endif
    }

    public override void OnInit ()
	{
		buffID = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
	}

    public override void OnStart()
    {
		RemoveHandle();
		handle = owner.RegisterEventNew(BeEventType.onRemoveBuff, (args)=>{
			int bid = (int)args.m_Int;
			if (bid == buffID)
			{
				PressAgainCancel();
			}
		});
    }

	public override void OnCancel ()
	{
		RemoveHandle();
		owner.buffController.RemoveBuff(buffID);
	}

	public void RemoveHandle()
	{
		if (handle != null)
		{
			handle.Remove();
			handle = null;
		}
	}
}
