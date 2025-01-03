using GameClient;
using System.Collections.Generic;
public class Skill1723 : BeSkill
{
    protected IBeEventHandle doAttackhandle = null;
    protected IBeEventHandle curFrameHandle = null;
    protected IBeEventHandle summonHandle = null;
    protected int suckDurtime = 0;
    protected VFactor suckTimeFactor = VFactor.zero;
    protected bool isStartSuck = false;
    protected bool frameFlag = false;
    protected int firsthurtId = 0;
    protected int explosiveHurtId = 0;
    protected VInt baseAttackRange = 15000;   //��Ҫ�߻��������ֵ
    protected int specialMonsterTypeBuffID = 0;
    string effectPath = "Effects/Hero_Axiuluo/wushuangbo/prefeb/Eff_wushuangbo_xuligongji_02"; //��Ч·��
    string skillFrame = "1723";
    string attackEndFrame = "attackEnd";
    protected int explosiveOffset = 0;
    protected bool isStartExplosive = false;
    protected bool isSpecailAttack = false;
    protected bool isHitOther = false;
    protected int suckEnemyCount = 0;
    protected List<VFactor> mAddDamage = new List<VFactor>();
    protected VFactor mSpecialMonsterAddDamage = VFactor.zero;
    protected VInt mSearchEnemyRadius = VInt.zero;
    protected BeAbilityEnable mFilter = null;
    protected int summonMonsterPid = 0;

    BeEvent.BeEventHandleNew _useSkillHandle = null;
    BeEvent.BeEventHandleNew _canCancelSkillHandle = null;
    private bool _needUseBuDongMingWang = false;
    private int _buDongMingWangSkillId = 1717;
    private string _cancelSkillFlag = "CancelSkill";
    private Mechanism22 _runeManager = null;
    private bool _haveShowBuDongMingWangState = false;

    public Skill1723(int sid, int skillLevel) : base(sid, skillLevel)
    {
        mFilter = new BeAbilityEnable
        {
            abType = (int)BeAbilityType.BEHIT
        };

    }
    public override void OnInit()
    {
        firsthurtId = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        suckTimeFactor = new VFactor(TableManager.GetValueFromUnionCell(skillData.ValueB[0], level), GlobalLogic.VALUE_1000);
        explosiveHurtId = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
        mAddDamage.Clear();
        if (BattleMain.IsModePvP(battleType))
        {
            for (int i = 1; i <= 5; i++)
            {
                int damageAdd = TableManager.GetValueFromUnionCell(skillData.ValueF[1], i);
                mAddDamage.Add(VFactor.NewVFactor(damageAdd, GlobalLogic.VALUE_1000));
            }
        }
        else
        {
            for (int i = 1; i <= 5; i++)
            {
                int damageAdd = TableManager.GetValueFromUnionCell(skillData.ValueF[0], i);
                mAddDamage.Add(VFactor.NewVFactor(damageAdd, GlobalLogic.VALUE_1000));
            }
        }
        InitValue();
    }
    private void InitValue()
    {
        mSpecialMonsterAddDamage = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(skillData.ValueE[0], level), GlobalLogic.VALUE_1000);
        mSearchEnemyRadius = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueG[0], level), GlobalLogic.VALUE_1000);
        baseAttackRange = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueD[0], level), GlobalLogic.VALUE_1000);
    }
    public override void OnPostInit()
    {
        InitValue();
    }
    public override bool CanUseSkill()
    {
        if (owner == null) return false;
        if (!base.CanUseSkill()) return false;
        if (owner.buffController == null) return false;
        if (HaveBuff())
            return false;
        return owner.buffController.HasBuffByID(171901) != null && owner.buffController.HasBuffByID(171101) != null;
    }

    /// <summary>
    /// ʤ��֮ì��buff
    /// </summary>
    /// <returns></returns>
    private bool HaveBuff()
    {
        return owner.buffController.HasBuffByID(370500) != null || owner.buffController.HasBuffByID(370501) != null;
    }
    // private void onSummonMonster(object[] args)
    private void onSummonMonster(BeEvent.BeEventParam args)
    {
        // var summonActor = args[0] as BeActor;
        var summonActor = args.m_Obj as BeActor;
        if (summonActor != null)
        {
            if (owner.GetCastSkillID() == this.skillID)
            {
                summonMonsterPid = summonActor.GetPID();
            }
        }
    }
    public override void OnStart()
    {
        _SetRuneManager();
        _haveShowBuDongMingWangState = false;
        CleanUp();
        isStartSuck = true;
        suckDurtime = 0;
        summonHandle = owner.RegisterEventNew(BeEventType.onSummon, onSummonMonster);
        curFrameHandle = owner.RegisterEventNew(BeEventType.onSkillCurFrame, args =>
         {
            //  string flag = (string)args[0];
            string flag = args.m_String;
             if (flag == skillFrame)
             {
                 frameFlag = true;
                 //_ShowBuDongMingWangButton(true);
#if !LOGIC_SERVER
                 if (button != null)
                     button.AddEffect(ETCButton.eEffectType.onContinue);
#endif
            }
             else if (flag == attackEndFrame)
             {
                 isStartExplosive = true;
             }
             else if (flag == _cancelSkillFlag)
             {
                 if(_needUseBuDongMingWang)
                    owner.Locomote(new BeStateData((int)ActionState.AS_IDLE));
             }
         });
        doAttackhandle = owner.RegisterEventNew(BeEventType.onAfterFinalDamageNew, args =>
        {
            // BeActor target = args[1] as BeActor;
            BeActor target = args.m_Obj as BeActor;
            // int curHurtId = (int)args[2];
            int curHurtId = args.m_Int2;
            if (curHurtId == firsthurtId)
            {
                if (owner.CurrentBeScene == null)
                {
                    return;
                }
                isStartSuck = false;
                if (target != null)
                {
                    isHitOther = true;
                }
            }
            else if (curHurtId == explosiveHurtId)
            {
                if (suckEnemyCount <= 0) return;
                // int hurtid = (int)args[2];
                int hurtid = args.m_Int2;
                if (hurtid != explosiveHurtId) return;
                // var vals = (int[])args[0];
                // int damage = vals[0];
                int damage = args.m_Int;
                int enmeyCount = suckEnemyCount - 1;
                VFactor addDamagePercent = VFactor.zero;

                if (enmeyCount >= mAddDamage.Count)
                {
                    addDamagePercent = mAddDamage[mAddDamage.Count - 1];
                }
                else
                {
                    addDamagePercent = mAddDamage[enmeyCount];
                }

                int addDamage = damage * addDamagePercent;
                if (!BattleMain.IsModePvP(this.battleType) && target != null && target.GetEntityData() != null)
                {
                    if (target.GetEntityData().type == (int)ProtoTable.UnitTable.eType.ELITE ||
                        target.GetEntityData().type == (int)ProtoTable.UnitTable.eType.BOSS ||
                        target.GetEntityData().type == (int)ProtoTable.UnitTable.eType.HELL ||
                        target.GetEntityData().type == (int)ProtoTable.UnitTable.eType.ACTIVITYMONSTER)
                    {
                        addDamage = addDamage + damage * mSpecialMonsterAddDamage;
                    }
                }
                damage = damage + addDamage;

                // vals[0] = damage;
                args.m_Int = damage;
            }
        });

        if (!BattleMain.IsModePvP(battleType))
        {
            _useSkillHandle = owner.RegisterEventNew(BeEventType.onWillUseSkill, _OnWillUseSkill);
        }
    }

    private void _SetRuneManager()
    {
        if (owner != null)
        {
            var skill = owner.GetSkill(Global.BODONGKEYIN_SKILL_ID) as Skill1710;
            if (skill != null)
            {
                _runeManager = skill.runeManager;
            }
        }
    }

    private void _OnWillUseSkill(BeEvent.BeEventParam param)
    {
        if (BattleMain.IsModePvP(battleType)) return;
        if (!frameFlag) return;
        if (_needUseBuDongMingWang) return;
        if (param.m_Int != _buDongMingWangSkillId)return;
        if(_runeManager != null && _runeManager.GetRuneCount() <=0 )return;
        param.m_Int = -1;
        OnClickAgain();
        _needUseBuDongMingWang = true;
    }

    public override void OnUpdate(int iDeltime)
    {
        _CheckBuDongMingWangButtonState();
        if (isStartSuck)
        {
            suckDurtime += iDeltime;
        }
        if (isStartExplosive && isHitOther)
        {
            var list = GamePool.ListPool<BeActor>.Get();
            owner.CurrentBeScene.FindTargets(list, owner, mSearchEnemyRadius, false, mFilter);
            suckEnemyCount = list.Count;
            GamePool.ListPool<BeActor>.Release(list);
            var actorpos = owner.GetPosition();
            actorpos.x = owner.GetFace() ? actorpos.x - explosiveOffset : actorpos.x + explosiveOffset;
            list = GamePool.ListPool<BeActor>.Get();
            //   4.��ʽ��������Χ = ����������Χ���ɵ�����+����ÿ֡�ӳɷ�Χ���ɵ�����*����ʱ����֡��λ����
            owner.CurrentBeScene.FindActorInRange(list, actorpos, baseAttackRange + suckDurtime * suckTimeFactor, owner.GetCamp() == 0 ? 1 : 0);
            //�����˺� =[���������˺����ɵ����������ٷֱ��˺���+��ȡ��λ�����ӳ��˺����ɵ����������ٷֱ��˺���*��ȡ��λ���������޿ɵ���] * ��λ���ͼӳɣ����ⵥλ�ӳɲ����ɵ��������ⵥλ�̶�Ϊ1��

            for (int i = 0; i < list.Count; i++)
            {
                var curActor = list[i];
                if (curActor != null && !curActor.IsDeadOrRemoved())
                {
                    owner._onHurtEntity(curActor, curActor.GetPosition(), explosiveHurtId);
                }
            }
#if !LOGIC_SERVER
            if (owner.CurrentBeScene.currentGeScene != null)
            {
                owner.m_pkGeActor.CreateEffect(effectPath, "[actor]Orign", 0, new Vec3(0, 0, 0));
                //  /*var effect = */owner.CurrentBeScene.currentGeScene.CreateEffect(effectPath, 0.0f, actorpos.vec3);
                //if (effect != null)
                //{
                //    effect.SetScale(0.5f + suckDurtime * suckTimeFactor / 1000.0f);
                //}
            }
#endif
            GamePool.ListPool<BeActor>.Release(list);
            isStartExplosive = false;
            isHitOther = false;
        }
    }
    public override void OnFinish()
    {
        _UseBuDongMingWang();
        CleanUp();
    }
    public override void OnEnterPhase(int phase)
    {
        base.OnEnterPhase(phase);
        if (phase == 3)
        {
            isStartSuck = false;
            var list = GamePool.ListPool<BeActor>.Get();
            owner.CurrentBeScene.FindActorById(list, 93000032);
            for (int i = 0; i < list.Count; i++)
            {
                var monster = list[i];
                if (monster == null) continue;
                var summonOwner = list[i].GetOwner();
                if (summonOwner != null)
                {
                    if (summonOwner.GetPID() == owner.GetPID())
                    {
                        monster.DoDead();
                        break;
                    }
                }
            }
            GamePool.ListPool<BeActor>.Release(list);
        }
    }
    public override void OnClickAgain()
    {
        if (curPhase == 2)
        {
            if (frameFlag)
            {
#if !LOGIC_SERVER
                if (button != null)
                    button.RemoveEffect(ETCButton.eEffectType.onContinue);
#endif
                ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();

                frameFlag = false;
                skillButtonState = SkillState.NORMAL;
            }
        }
    }
    public override void OnCancel()
    {
        _UseBuDongMingWang();
        CleanUp();
    }
    void CleanUp()
    {
        frameFlag = false;
        isStartSuck = false;
        isStartExplosive = false;
        suckDurtime = 0;
        suckEnemyCount = 0;
        isHitOther = false;
        _needUseBuDongMingWang = false;

        if (owner.CurrentBeScene != null)
        {
            var entity = owner.CurrentBeScene.GetEntityByPID(summonMonsterPid);
            if (entity != null && !entity.IsDeadOrRemoved())
            {
                entity.DoDead();
            }
        }
        summonMonsterPid = 0;
        if (summonHandle != null)
        {
            summonHandle.Remove();
            summonHandle = null;
        }
        if (curFrameHandle != null)
        {
            curFrameHandle.Remove();
            curFrameHandle = null;
        }
        if (doAttackhandle != null)
        {
            doAttackhandle.Remove();
            doAttackhandle = null;
        }

        if (_useSkillHandle != null)
        {
            _useSkillHandle.Remove();
            _useSkillHandle = null;
        }

        if (_canCancelSkillHandle != null)
        {
            _canCancelSkillHandle.Remove();
            _canCancelSkillHandle = null;
        }
        
        skillButtonState = SkillState.NORMAL;

        _ShowBuDongMingWangButton(false);

#if !LOGIC_SERVER
        if (button != null)
            button.RemoveEffect(ETCButton.eEffectType.onContinue);
#endif
    }

    private void _CheckBuDongMingWangButtonState()
    {
        if (BattleMain.IsModePvP(battleType)) return;
        if (_haveShowBuDongMingWangState) return;
        if (!frameFlag) return;
        if (_runeManager != null && _runeManager.GetRuneCount() <= 0) return;
        _haveShowBuDongMingWangState = true;
        _ShowBuDongMingWangButton(true);
    }

    private void _ShowBuDongMingWangButton(bool flag)
    {
        if (BattleMain.IsModePvP(battleType)) return;
        var skill = owner.GetSkill(_buDongMingWangSkillId);
        if (skill == null) return;
        skill.ForceShowButtonImage = flag;
    }

    private void _UseBuDongMingWang()
    {
        if (BattleMain.IsModePvP(battleType)) return;
        if (!_needUseBuDongMingWang) return;
        owner.delayCaller.DelayCall((30), _RealUseSkill);
    }

    private void _RealUseSkill()
    {
        owner.UseSkill(_buDongMingWangSkillId);
    }
}