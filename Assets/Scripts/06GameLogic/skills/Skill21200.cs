using UnityEngine;
using System.Collections;
using GameClient;
using ProtoTable;

//1.背景动画：贝希摩斯嘴开合
public enum MouthState
{
    Mouth_Open,
    Mouth_Ready_Open,
    Mouth_Close,
    Mouth_Ready_Close,
};
public class Skill21200 : BeSkill
{
    private bool bInitClose = false;
    private int totalTime = 0;
    private VFactor mouseOpenSpeed = VFactor.zero;
    private int durTime = 0;
    private VInt height = 0;
    private int curheightAdd = 0;
    private MouthState curState = 0;
    public Skill21200(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }
    public MouthState stat { get { return curState; } }
    public override void OnInit()
    {
        bInitClose = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level) == 0 ? true : false;
        height = new VInt(TableManager.GetValueFromUnionCell(skillData.ValueB[0], level) / 1000.0f);
        totalTime = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
        mouseOpenSpeed = VFactor.NewVFactor(height.i, totalTime);

        if (bInitClose)
        {
            curState = MouthState.Mouth_Close;
        }
        else
        {
            curState = MouthState.Mouth_Open;
            curheightAdd = height.i;
        }

        if (owner == null || owner.CurrentBeScene == null) return;
        owner.CurrentBeScene.SetDayTime(!bInitClose);
    }
    public override void OnStart()
    {
        switch(curState)
        {
            case MouthState.Mouth_Close:
                {
                    curState = MouthState.Mouth_Ready_Open;
                    if (owner != null && owner.CurrentBeScene != null)
                    {
                        owner.CurrentBeScene.SetDayTime(true);
                        owner.CurrentBeScene.TriggerEventNew(BeEventSceneType.onMouthClose, new EventParam(){m_Bool = false});
                        //owner.CurrentBeScene.TriggerEvent(BeEventSceneType.onMouthClose, new object[] { false });
                    }
                }
                break;
            case MouthState.Mouth_Open:
                {
                    curState = MouthState.Mouth_Ready_Close;
                    if (owner != null && owner.CurrentBeScene != null)
                    {
                        owner.CurrentBeScene.SetDayTime(false);
                        owner.CurrentBeScene.TriggerEventNew(BeEventSceneType.onMouthClose, new EventParam(){m_Bool = true});
                        //owner.CurrentBeScene.TriggerEvent(BeEventSceneType.onMouthClose, new object[] { true });
                    }
                }
                break;
            case MouthState.Mouth_Ready_Open:
                {
                    curState = MouthState.Mouth_Ready_Close;
                    if (owner != null && owner.CurrentBeScene != null)
                    {
                        owner.CurrentBeScene.SetDayTime(false);
                        owner.CurrentBeScene.TriggerEventNew(BeEventSceneType.onMouthClose, new EventParam(){m_Bool = true});
                        //owner.CurrentBeScene.TriggerEvent(BeEventSceneType.onMouthClose, new object[] { true });
                    }
                }
                break;
            case MouthState.Mouth_Ready_Close:
                {
                    curState = MouthState.Mouth_Ready_Open;
                    if (owner != null && owner.CurrentBeScene != null)
                    {
                        owner.CurrentBeScene.SetDayTime(true);
                        owner.CurrentBeScene.TriggerEventNew(BeEventSceneType.onMouthClose, new EventParam(){m_Bool = false});
                        //owner.CurrentBeScene.TriggerEvent(BeEventSceneType.onMouthClose, new object[] { false });
                    }
                }
                break;
        }
    }
    public override void OnUpdate(int iDeltime)
    {
        base.OnUpdate(iDeltime);
        switch(curState)
        {
            case MouthState.Mouth_Ready_Close:
                {
                    var offset = mouseOpenSpeed * iDeltime;
                    int intOffset = offset.integer;
                    var heightAdd = curheightAdd - intOffset;
                    if(heightAdd < 0)
                    {
                        intOffset = curheightAdd;
                    }
                    var pos = owner.GetPosition();
                    curheightAdd -= intOffset;
                    pos.z = pos.z - intOffset;
                    owner.SetPosition(pos);
                    if(curheightAdd <= 0)
                    {
                        curState = MouthState.Mouth_Close;
                    }
                }
                break;
            case MouthState.Mouth_Ready_Open:
                {
                    var offset = mouseOpenSpeed * iDeltime;
                    int intOffset = offset.integer;
                    var heightAdd = curheightAdd + intOffset;
                    if(heightAdd > height)
                    {
                        intOffset = height.i - curheightAdd;
                       
                    }
                    var pos = owner.GetPosition();
                    pos.z = pos.z + intOffset;
                    curheightAdd += intOffset;
                    owner.SetPosition(pos);
                    if(heightAdd >= height)
                    {
                        curState = MouthState.Mouth_Open;
                    }
                }
                break;
        }
    }
}


public class Skill21564 : BeSkill
{
    public Skill21564(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }
}