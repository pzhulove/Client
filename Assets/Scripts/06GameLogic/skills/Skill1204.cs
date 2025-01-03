using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using GameClient;
using ProtoTable;

public class Skill1218 : Skill1512
{
    public Skill1218(int sid, int skillLevel) : base(sid, skillLevel) { }
    public override void OnInit()
    {
#if !LOGIC_SERVER
        base.OnInit();
        canSlide = SettingManager.GetInstance().GetSlideMode("1218") == InputManager.SlideSetting.SLIDE;
#endif
    }
}
public class Skill2611 : Skill1512
{
    public Skill2611(int sid, int skillLevel) : base(sid, skillLevel) { }
    public override void OnInit()
    {
#if !LOGIC_SERVER
        base.OnInit();
        canSlide = SettingManager.GetInstance().GetSlideMode("2611") == InputManager.SlideSetting.SLIDE;
#endif
    }
}
public class Skill1216 : Skill1204
{
    public Skill1216(int sid, int skillLevel) : base(sid, skillLevel) { }
    public override void OnInit()
    {
#if !LOGIC_SERVER
        base.OnInit();
        canSlide = SettingManager.GetInstance().GetSlideMode("1216") == InputManager.SlideSetting.SLIDE;
#endif
    }
}
//女大枪 加农炮
public class Skill2612: Skill1216
{
    public Skill2612(int sid,int skillLevel) : base(sid, skillLevel) { }
}
//圣骑士 圣光球
public class Skill3713 : Skill1512
{
    public Skill3713(int sid, int skillLevel) : base(sid, skillLevel) { }

    private int[] entityIdArr = new int[2];     //圣光球实体ID(PVE|PVP)
    private List<int> pveDizzBuffInfoIdList = new List<int>();     //pve眩晕Buff信息ID列表
    private List<int> pvpDizzBuffInfoIdList = new List<int>();     //pvp眩晕Buff信息ID列表
    private VInt[] dizzRadiueArr = new VInt[2];        //眩晕半径(PVE|PVP)

    private List<IBeEventHandle> handleList = new List<IBeEventHandle>();

    private VInt curDizzRadius = 0;
    private int curEntityId = 0;

    public static void SkillPreloadRes(SkillTable tableData)
    {
#if !LOGIC_SERVER
        if (BattleMain.instance == null) return;

        if (BattleMain.IsModePvP(BattleMain.battleType))
        {
            for (int i = 0; i < tableData.ValueC.Count; i++)
            {
                PreloadManager.PreloadBuffInfoID(TableManager.GetValueFromUnionCell(tableData.ValueC[i], 1), null, null);
            }
        }
        else
        {
            for (int i = 0; i < tableData.ValueB.Count; i++)
            {
                PreloadManager.PreloadBuffInfoID(TableManager.GetValueFromUnionCell(tableData.ValueB[i], 1), null, null);
            }
        }
#endif
    }

    public override void OnInit()
    {
        base.OnInit();
        pveDizzBuffInfoIdList.Clear();
        pvpDizzBuffInfoIdList.Clear();

#if !LOGIC_SERVER
        canSlide = SettingManager.GetInstance().GetSlideMode("3713") == InputManager.SlideSetting.SLIDE;
#endif
        entityIdArr[0] = TableManager.GetValueFromUnionCell(skillData.ValueA[0],level);
        entityIdArr[1] = TableManager.GetValueFromUnionCell(skillData.ValueA[1], level);

        for (int i=0;i< skillData.ValueB.Count; i++)
        {
            pveDizzBuffInfoIdList.Add(TableManager.GetValueFromUnionCell(skillData.ValueB[i], level));
        }

        for (int i = 0; i < skillData.ValueC.Count; i++)
        {
            pvpDizzBuffInfoIdList.Add(TableManager.GetValueFromUnionCell(skillData.ValueC[i], level));
        }

        dizzRadiueArr[0] = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueD[0],level),GlobalLogic.VALUE_1000);
        dizzRadiueArr[1] = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueD[1], level), GlobalLogic.VALUE_1000);
    }

    public override void OnStart()
    {
        base.OnStart();
        RemoveHandleList();
        curEntityId = BattleMain.IsModePvP(battleType) ? entityIdArr[1] : entityIdArr[0];
        curDizzRadius = BattleMain.IsModePvP(battleType) ? dizzRadiueArr[1] : dizzRadiueArr[0];
        AddListenerEntity();
    }

    private void RemoveHandleList()
    {
        for(int i=0;i< handleList.Count; i++)
        {
            if (handleList[i] != null)
            {
                handleList[i].Remove();
                handleList[i] = null;
            }
            
        }
    }

    //监听实体移除
    private void AddListenerEntity()
    {
        if (owner == null)
            return;
        var handle1 = owner.RegisterEventNew(BeEventType.onAfterGenBullet, (args)=> 
        {
            var entity= args.m_Obj as BeProjectile;
            if(entity != null && entity.m_iResID == curEntityId)
            {
                var handle2 = entity.RegisterEventNew(BeEventType.onDead, eventParam => 
                {
                    List<BeActor> list = GamePool.ListPool<BeActor>.Get();
                    if (owner!=null && owner.CurrentBeScene != null)
                    {
                        owner.CurrentBeScene.FindTargetsByEntity(list, entity, curDizzRadius);
                        for(int i = 0; i < list.Count; i++)
                        {
                            bool projectileFace = entity.GetFace();
                            int dis =  entity.GetPosition().x - list[i].GetPosition().x;
                            if (projectileFace != list[i].GetFace() && ((dis > 0 && projectileFace) || (!projectileFace && dis < 0)))
                            {
                                List<int> buffInfoList = BattleMain.IsModePvP(battleType) ? pvpDizzBuffInfoIdList : pveDizzBuffInfoIdList;
                                for(int j=0;j< buffInfoList.Count; j++)
                                {
                                    BuffInfoData data = new BuffInfoData(buffInfoList[j],level);
                                    list[i].buffController.TryAddBuff(data, null, false, new VRate(),owner);
                                }
                            }  
                        }
                    }
                    GamePool.ListPool<BeActor>.Release(list);
                });
                handleList.Add(handle2);
            }
        });
        handleList.Add(handle1);
    }
}

//星落打
public class Skill3608 : Skill1512
{
    public Skill3608(int sid, int skillLevel) : base(sid, skillLevel) { }
    public override void OnInit()
    {
#if !LOGIC_SERVER
        base.OnInit();
        canSlide = SettingManager.GetInstance().GetSlideMode("3608") == InputManager.SlideSetting.SLIDE;             //设置驱魔 星落打
#endif
    }
}

//破魔符
public class Skill3600 : Skill1512
{
    public Skill3600(int sid, int skillLevel) : base(sid, skillLevel) { }
    public override void OnInit()
    {
#if !LOGIC_SERVER
        base.OnInit();
        canSlide = SettingManager.GetInstance().GetSlideMode("3600") == InputManager.SlideSetting.SLIDE;             //设置驱魔 破魔符
#endif
    }
}

//冰冻手雷
public class Skill1307 : Skill1204
{
    public Skill1307(int sid, int skillLevel) : base(sid, skillLevel) { }
}

//感电手雷
public class Skill1304 : Skill1204
{
    public Skill1304(int sid, int skillLevel) : base(sid, skillLevel) { }
}

public class Skill2507 : Skill1204
{
    public Skill2507(int sid, int skillLevel) : base(sid, skillLevel) { }
}

public class Skill1512 : Skill1204
{
    public Skill1512(int sid, int skillLevel) : base(sid, skillLevel) { }
    public override void OnInit()
    {
#if !LOGIC_SERVER
        base.OnInit();
        canSlide = SettingManager.GetInstance().GetSlideMode("1512") == InputManager.SlideSetting.SLIDE;             //设置狂战崩山击
#endif
    }

    public override void OnPostInit()
    {
#if !LOGIC_SERVER
        base.OnPostInit();
        strValue = new string[] { "远", "中", "近" };
#endif
    }
}

public class Skill1716 : Skill1512
{
    public Skill1716(int sid, int skillLevel) : base(sid, skillLevel) { }                           //阵鬼崩山击设置
}

public class Skill2010 : Skill1204
{
    public Skill2010(int sid, int skillLevel) : base(sid, skillLevel) { }
    public override void OnInit()
    {
#if !LOGIC_SERVER
        base.OnInit();
        canSlide = SettingManager.GetInstance().GetSlideMode("2010") == InputManager.SlideSetting.SLIDE;         //设置天击
#endif
    }
    public override void OnPostInit()
    {
#if !LOGIC_SERVER
        base.OnPostInit();
        strValue = new string[] { "上", "前", "下" };
#endif
    }
}

public class Skill1204 : BeSkill
{

    protected enum DIR
    {
        TOP = 0,
        FORWARD = 1,
        DOWN = 2,
        COUNT = 3,
    }

    protected string strIndicator = "UIFlatten/Prefabs/Battle_Digit/Indicator_ShouLei";
    
    protected GameObject objIndicator = null;
    protected Image[] sprites = new Image[3];
    protected Text[] texts = new Text[3];
    protected IBeEventHandle mChangeFaceHandle = null;
    protected string[] strValue = new string[3];  

    public Skill1204(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnPostInit()
    {
#if !LOGIC_SERVER
        if (inTown)
            return;

        if (owner == null)
            return;

        if (!owner.isLocalActor)
            return;

        if (canSlide)
        {
            ComCommonBind bind = owner.m_pkGeActor.GetArrowBind(strIndicator);
            if (bind == null)
                return;
            for (int i = 0; i < (int)DIR.COUNT; ++i)
            {
                sprites[i] = bind.GetCom<Image>(string.Format("spr{0}", i));
                texts[i] = bind.GetCom<Text>(string.Format("txt{0}", i));
            }
            ChangeText();                       //默认情况先设置一下
            if (owner != null)
            {
                RemoveHandle();
                mChangeFaceHandle = owner.RegisterEventNew(BeEventType.onChangeFace, (args) =>
                {
                    ChangeText();
                });
            }
            strValue = new string[] {"上","前","下"};
            HideAllArrow();
        }
        else
        {
            joystickMode = SkillJoystickMode.NONE;                  //设置技能摇杆不显示  不创建手雷箭头图标
        }
#endif
    }

    public override void OnInit()
    {
#if !LOGIC_SERVER
        canSlide = SettingManager.GetInstance().GetSlideMode("1204") == InputManager.SlideSetting.SLIDE;
#endif
    }

    public override void OnStart()
    {
#if !LOGIC_SERVER
        HideAllArrow();
#endif
    }

    public override void OnCancel()
    {
        HideAllArrow();
    }

    public override void OnFinish()
    {
        HideAllArrow();
    }

    public override void OnReleaseJoystick()
    {
        //Logger.LogErrorFormat("OnReleaseJoystick");
        HideAllArrow();
    }

    public override void OnUpdateJoystick(int degree)
    {
#if !LOGIC_SERVER
        if (!owner.isLocalActor)
            return;
        if (!canSlide)
            return;

        var dir = InputManager.GetDir(degree);
        if (dir == InputManager.PressDir.TOP)
            ShowArrow(DIR.TOP);
        else if (dir == InputManager.PressDir.DOWN)
            ShowArrow(DIR.DOWN);
        else
            ShowArrow(DIR.FORWARD);
#endif
    }

    protected void ShowArrow(DIR dir)
    {
#if !LOGIC_SERVER
        if (!CanUseSkill())
            return;
        if (dir >= DIR.COUNT)
            return;
        HideAllArrow();

        var color = sprites[(int)dir].color;
        color.a = 255;
        sprites[(int)dir].color = color;
        int index = (int)dir;
        var textColor = texts[index].color;
        textColor.a = 255;
        texts[index].text = strValue[index];
        texts[index].color = textColor;
#endif
    }

    protected void HideAllArrow()
    {
#if !LOGIC_SERVER
        if (!owner.isLocalActor)
            return;
        for (int i = 0; i < (int)DIR.COUNT; ++i)
        {
            if (sprites[i] == null)
                continue;

            var color = sprites[i].color;
            color.a = 0;
            sprites[i].color = color;
            var textColor = texts[i].color;
            textColor.a = 0;
            texts[i].color = textColor;
        }
#endif
    }

    protected void ChangeText()
    {
#if !LOGIC_SERVER
        if (owner == null)
            return;
        bool faceLeft = owner.GetFace();
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] != null)
            {
                int xScale = faceLeft ? -1 : 1;
                texts[i].rectTransform.localScale = new Vector3(xScale, 1, 1);
            }
        }
#endif
    }

    protected void RemoveHandle()
    {
        if (mChangeFaceHandle != null)
        {
            mChangeFaceHandle.Remove();
            mChangeFaceHandle = null;
        }
    }
}
