using GameClient;

public class Skill1422 : BeSkill
{
    private int m_CloneMonsterId = 64018;
    private bool m_IsTest = false;
    public Skill1422(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnInit()
    {
        base.OnInit();
        m_IsTest = skillData.ValueA.Count > 0;
    }

    public override void OnStart()
    {
        base.OnStart();
        
        handleA = owner.RegisterEventNew(BeEventType.onSummon, OnSummon);
        handleB = owner.RegisterEventNew(BeEventType.OnBeforeInitData, OnCreateMonster);
    }

    private void OnSummon(BeEvent.BeEventParam args)
    {
        BeActor clone = args.m_Obj as BeActor;
        if(clone == null)
            return;
        
        if(clone.m_iResID != m_CloneMonsterId)
            return;
        
#if !LOGIC_SERVER
        if (clone.m_pkGeActor == null)
            return;

        if (owner.m_pkGeActor == null)
            return;
        
        clone.m_pkGeActor.EquipFashions(owner.m_pkGeActor.FashionsData);
        clone.SetFace(owner.GetFace(), false, true);
        var titleData = owner.m_pkGeActor.CurTittleComponentData;
        clone.m_pkGeActor.AddTittleComponent(titleData.iTittleID, titleData.name,titleData.guildDuty, titleData.bangName, titleData.iRoleLv, titleData.a_nPKRank, titleData.color);
        var data = owner.m_pkGeActor.CurPlayerInfoBarData;
        clone.m_pkGeActor.CreateInfoBar(data.name, data.infoColor, data.RoleLevel, data.namecolors, data.NameLocalPosY);
        
        if (string.IsNullOrEmpty(owner.attachmentproxy.fashionWeaponPath) ||
            string.IsNullOrEmpty(owner.attachmentproxy.roleDefaultWeaponName))
        {
            clone.attribute.currentWeapon = owner.attribute.currentWeapon;
            int level = owner.attribute.currentWeapon == null ? 0 : owner.attribute.currentWeapon.strengthen; 
            clone.ChangeWeaponModle(level);
        }
        else
        {
            clone.attachmentproxy.SetShowFashionWeapon(owner.attachmentproxy.fashionWeaponPath, owner.attachmentproxy.roleDefaultWeaponName);
        }
#endif
    }

    private void OnCreateMonster(BeEvent.BeEventParam param)
    {
        BeActor clone = (BeActor) param.m_Obj;
        if(clone == null)
            return;
        
        if(clone.m_iResID != m_CloneMonsterId)
            return;

        clone.SetDefaultWeapenTag(owner.attachmentproxy.tag);
    }
    
    public override bool CanUseSkill()
    {
        if (m_IsTest)
            return base.CanUseSkill();
        
        return base.CanUseSkill() && CheckSpellCondition((ActionState)owner.sgGetCurrentState());
    }
    
    public override bool CheckSpellCondition(ActionState state)
    {
        if (m_IsTest)
            return base.CheckSpellCondition(state);
        
        bool flag =
            owner.stateController.HasBuffState(BeBuffStateType.FROZEN) ||
            owner.stateController.HasBuffState(BeBuffStateType.STUN) ||
            owner.stateController.HasBuffState(BeBuffStateType.STONE) ||
            owner.stateController.HasBuffState(BeBuffStateType.SLEEP) ||
            owner.stateController.HasBuffState(BeBuffStateType.STRAIN);

        bool flag2 =
            owner.sgGetCurrentState() == (int)ActionState.AS_GRABBED ||
            owner.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL ||
            owner.sgGetCurrentState() == (int)ActionState.AS_BIRTH ||
            owner.sgGetCurrentState() == (int)ActionState.AS_JUMPBACK ||
            owner.sgGetCurrentState() == (int)ActionState.AS_WIN;

        return !flag2 && !flag && owner.IsInPassiveState() && !owner.stateController.WillBeGrab();
    }
}

