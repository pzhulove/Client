using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

#region SkillSnierEffect
//用于技能表现相关
public class SkillSniperEffect
{
    protected SkillSniperFrame m_SnipertFrame;          //狙击UI界面
    protected SkillSniperOtherFrame m_SniperOtherFrame; //其他人看到的狙击枪口页面
    protected RectTransform m_Rect;                     //狙击口的父节点
    protected float m_Radius = 270;                     //狙击口半径
    protected Skill1310 skill1310 = null;
    protected BeActor owner = null;
    public int m_SkillTotalTime = 9910;                 //技能持续总时间
    protected Vector3 m_LastCenterWorldPoint = Vector3.zero;
	protected ETCButton attackButton = null;

    protected int mSyncSightAcc = 250;                  //同步帧的时间间隔
    protected int mCurSyncSightAcc = 0;                 //当前时间间隔

    public SkillSniperEffect(Skill1310 skill, BeActor skillOwner)
    {
#if !LOGIC_SERVER
        skill1310 = skill;
        owner = skillOwner;
#endif
    }

    public void InitFrame()
    {
#if !LOGIC_SERVER
        if (ReplayServer.GetInstance().IsReplay())
            return;
        if (owner.isLocalActor)
        {
            InitSniperFrame();
        }
        else
        {
            InitSniperOtherFrame();
        }
#endif
    }
	
	public void SetAttackButtonEnable(bool flag)
    {
#if !LOGIC_SERVER
        if (owner.isLocalActor)
            return;
        if (ReplayServer.GetInstance().IsReplay())
            return;
        if(attackButton == null && InputManager.instance != null
            && InputManager.instance.ButtonSlotMap!=null && InputManager.instance.ButtonSlotMap.ContainsKey(1))
            attackButton = InputManager.instance.ButtonSlotMap[1];
        if (attackButton != null)
        {
            if (!flag)
                attackButton.SetDark(true, 0.5f);
            else
                attackButton.SetDark(false);
        }
#endif
    }

    protected void InitSniperFrame()
    {
        m_SnipertFrame = ClientSystemManager.instance.OpenFrame<SkillSniperFrame>(FrameLayer.Bottom) as SkillSniperFrame;
        if (m_SnipertFrame != null)
        {
            if (BattleMain.IsModePvP(skill1310.battleType))
            {
                m_SnipertFrame.m_MoveXOffset = skill1310.m_PvpMoveXOffset / 1000.0f;
            }
            m_SnipertFrame.InitZiDan(skill1310.mCurMaxBullet);
            m_SnipertFrame.m_Owner = owner;
            m_SnipertFrame.gameObject.transform.SetAsFirstSibling();
            m_Rect = m_SnipertFrame.GetTargetParent();
        }
    }

    protected void InitSniperOtherFrame()
    {
        if (!BattleMain.IsModePvP(skill1310.battleType))
            return;
        m_SniperOtherFrame = ClientSystemManager.instance.OpenFrame<SkillSniperOtherFrame>(FrameLayer.Bottom) as SkillSniperOtherFrame;
        if (m_SniperOtherFrame != null)
        {
            m_SniperOtherFrame.gameObject.transform.SetAsFirstSibling();
            m_Rect = m_SniperOtherFrame.GetTargetParent();
        }
    }

    public void AttackPhaseStartEffect()
    {
#if !LOGIC_SERVER
        if (ReplayServer.GetInstance().IsReplay())
            return;
        if (!owner.isLocalActor)
            return;
        if (m_SnipertFrame != null)
        {
            m_SnipertFrame.InitCenterPos();
        }
        mCurSyncSightAcc = 0;
        if (InputManager.instance != null)
        {
            InputManager.instance.SetButtonStateActive(0);
        }
        InitJoystick();
        SetCameraUpdatePause(true);
#endif
    }


    //发送攻击帧命令
    public void CreateAttackFrame(int curBullet, int curMaxBullet)
    {
#if !LOGIC_SERVER
        if (!owner.isLocalActor)
            return;
        if (ReplayServer.GetInstance().IsReplay())
            return;
        int audioId = 4327;
        List<BeActor> canAttackMonsterList = GamePool.ListPool<BeActor>.Get();
        GetInCircleMonster(canAttackMonsterList);
        for (int i = 0; i < canAttackMonsterList.Count; i++)
        {
            BeActor target = canAttackMonsterList[i];
            if (target != null && !target.IsDead())
            {
                audioId = 4326;
                InputManager.CreateSkillDoattackFrameCommand(skill1310.skillID, curBullet, target.GetPID());
            }
        }
        InputManager.CreateSkillDoattackFrameCommand(skill1310.skillID, curBullet, 0);
        GamePool.ListPool<BeActor>.Release(canAttackMonsterList);
        ShowAttackEffect(curBullet, curMaxBullet);
        PlayAudio(audioId);
        SetAttackButtonEnable(false);
#endif
    }

    public void ShowAttackEffect(int curBullet, int maxBullet)
    {
#if !LOGIC_SERVER
        if (ReplayServer.GetInstance().IsReplay())
            return;
        if (m_SnipertFrame != null)
        {
            m_SnipertFrame.Attack(curBullet - 1);
            if (curBullet >= maxBullet)
            {
                m_SnipertFrame.CloseProgress();
            }
        }
#endif
    }

    public void ShowOtherAttackEffect()
    {
#if !LOGIC_SERVER
        if (ReplayServer.GetInstance().IsReplay())
            return;
        if (owner.isLocalActor || m_SniperOtherFrame == null)
            return;
        m_SniperOtherFrame.Attack();
#endif
    }

    //刷新每次攻击的CD
    public void RefreshCd(int clickTimeAcc, int currTimeAcc)
    {
#if !LOGIC_SERVER
        if (ReplayServer.GetInstance().IsReplay())
            return;
        if (!owner.isLocalActor)
            return;
        if (m_SnipertFrame != null)
        {
            VFactor rrate = new VFactor(clickTimeAcc - currTimeAcc, clickTimeAcc);
            m_SnipertFrame.RefreshProgress(rrate.single);
        }
#endif
    }

    public void UpdateSkillTime(int iDeltime)
    {
#if !LOGIC_SERVER
        if (ReplayServer.GetInstance().IsReplay())
            return;
        if (!owner.isLocalActor)
            return;
        if (m_SnipertFrame == null)
            return;
        SyncSight(iDeltime);
        m_SkillTotalTime -= iDeltime;
        if (m_SkillTotalTime < 0)
        {
            m_SkillTotalTime = 0;
        }
        m_SnipertFrame.ShowSkillTime(m_SkillTotalTime);
#endif
    }
    
    protected void SyncSight(int iDeltime)
    {
        if (!BattleMain.IsModePvP(skill1310.battleType))
            return;
        if (mCurSyncSightAcc < mSyncSightAcc)
        {
            mCurSyncSightAcc += iDeltime;
        }
        else
        {
            mCurSyncSightAcc = 0;
            Vector3 centerWorldPoint = m_SnipertFrame.GetCenterScenePos(m_SnipertFrame.GetWorldCenterPoint());
            InputManager.CreateSkillSynSightFrameCommand(skill1310.skillID, (int)((centerWorldPoint.x + 50) * 100), (int)((centerWorldPoint.z + 50) * 100));
        }
    }

    //播放音效
    protected void PlayAudio(int audioId)
    {
        if (owner != null && owner.CurrentBeBattle != null)
			owner.CurrentBeBattle.PlaySound(audioId);

    }

    public void AttackPhaseEnd()
    {
#if !LOGIC_SERVER
        if (ReplayServer.GetInstance().IsReplay())
            return;
        if (owner.isLocalActor)
        {
            if (InputManager.instance != null)
            {
                InputManager.instance.ResetButtonState();
            }
            
            if (m_SnipertFrame != null)
            {
                m_SnipertFrame.PlayCloseAni();
            }
            RemoveJoystick();
            SetCameraUpdatePause(false);
        }
#endif
    }

    public void DoSyncSight(int x, int z)
    {
#if !LOGIC_SERVER
        if (ReplayServer.GetInstance().IsReplay())
            return;
        if (owner.isLocalActor)
            return;
        if (m_SniperOtherFrame == null)
            return;
        float xPos = x / 100.0f - 50;
        float yPos = 0.0f;
        float zPos = z / 100.0f - 50;
        Vector3 sceneScreenPos = Camera.main.WorldToScreenPoint(new Vector3(xPos, yPos, zPos));
        Vector2 localPos = GetLocalPosFromScreenPos(sceneScreenPos);
        m_SniperOtherFrame.QiangkouMove(localPos);
#endif
    }

    public void CloseFrame()
    {
#if !LOGIC_SERVER
        if (ReplayServer.GetInstance().IsReplay())
            return;
        if (owner.isLocalActor)
        {
            if (m_SnipertFrame != null)
            {
                m_SnipertFrame.Close();
                m_SnipertFrame = null;
            }
        }
        else
        {
            if (m_SniperOtherFrame != null)
            {
                m_SniperOtherFrame.PlayCloseAni();
                m_SniperOtherFrame.Close();
                m_SniperOtherFrame = null;
            }
        }
#endif
    }

    //设置摄像机是否主动更新位置
    protected void SetCameraUpdatePause(bool flag)
    {
        owner.CurrentBeScene.currentGeScene.GetCamera().GetController().SetPause(flag);
    }

    //获取狙击口能打到的怪物
    protected void GetInCircleMonster(List<BeActor> targetList)
    {
        if (targetList == null)
            return;
        targetList.Clear();
        List<BeActor> targets = GamePool.ListPool<BeActor>.Get();
        //获取全部的怪物
        skill1310.owner.CurrentBeScene.FindTargets(targets, owner, VInt.Float2VIntValue(100f));

        for (int i = 0; i < targets.Count; i++)
        {
            Vector3 targetPos = targets[i].GetPosition().vector3;
            Vector3 screenStartPos = Camera.main.WorldToScreenPoint(targetPos);
            Vector3 screenEndPos = Camera.main.WorldToScreenPoint(new Vector3(targetPos.x, targetPos.y + 1.5f, targetPos.z));
            Vector3 screenMiddlePos = (screenStartPos + screenEndPos) / 2;
            //狙击口父节点
            Vector2 localStartPos = GetLocalPosFromScreenPos(screenStartPos);
            Vector2 localEndPos = GetLocalPosFromScreenPos(screenEndPos);
            Vector2 localMiddlePos = GetLocalPosFromScreenPos(screenMiddlePos);
            if (LineInterCircle(localStartPos, localEndPos, localMiddlePos,m_SnipertFrame.GetCenterPoint()))
            {
                if (!targetList.Contains(targets[i]))
                    targetList.Add(targets[i]);
            }
        }
        GamePool.ListPool<BeActor>.Release(targets);
    }

    //获取Canvas下的本地坐标
    protected Vector2 GetLocalPosFromScreenPos(Vector2 screenPos)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Rect, screenPos, ClientSystemManager.GetInstance().UICamera, out localPos);
        return localPos;
    }

    /// <summary>
    /// 判断怪物与狙击口是否有交点
    /// </summary>
    /// <param name="start">脚底点</param>
    /// <param name="end">头顶点</param>
    /// <param name="center">中心点</param>
    /// <param name="radius"></param>
    /// <returns></returns>
    protected bool LineInterCircle(Vector2 start, Vector2 end, Vector2 center, Vector2 point)
    {
        float dis01 = Vector2.Distance(start, point);
        float dis02 = Vector2.Distance(end, point);
        float dis03 = Vector2.Distance(center, point);

        return dis01 <= m_Radius || dis02 <= m_Radius || dis03 <= m_Radius;
    }

    protected void InitJoystick()
    {
        if (InputManager.instance == null)
            return;
        InputManager.instance.SetJoyStickMoveCallback(OnJoyStickMove);
    }

    protected void RemoveJoystick()
    {
        if (InputManager.instance == null)
            return;
        InputManager.instance.ReleaseJoyStickMoveCallback(OnJoyStickMove);
    }
    
    protected void OnJoyStickMove(Vector2 offset)
    {
        if (m_SnipertFrame == null || !owner.isLocalActor)
            return;
        m_SnipertFrame._OnJoyStickMove(offset);
    }
}
#endregion

/// <summary>
/// 尼尔狙击
/// </summary>
public class Skill1310 : BeSkill
{
    protected int m_ClickTimeAccPve = 800;              //狙击枪手动点击释放时间间隔(Pve)
    protected int m_ClickTimeAccPvp = 1500;             //狙击枪手动点击释放时间间隔(Pvp)
    protected int mMaxBulletPve = 5;                    //可以使用的最大子弹数量(Pve)
    protected int mMaxBulletPvp = 5;                    //可以使用的最大子弹数量(Pvp)
    protected int m_HurtIdPve = 13100;                  //造成伤害的触发效果ID(PVE)
    protected int m_HurtIdPvp = 13101;                  //造成伤害的触发效果ID(PVP)
    public int m_PvpMoveXOffset = 500;                  //Pvp中镜头可以移动的距离

    protected int m_CurrClickTimeAcc = 0;               //当前手动释放时间间隔
    protected bool m_CreateAttackFlag = false;          //能否攻击标志
    protected int mCurBullet = 0;                       //当前子弹数量
    public int mCurMaxBullet = 5;                       //可以使用的最大子弹数量
    protected int m_ClickTimeAcc = 800;                 //当前攻击间隔
    protected int m_FlagBatiBuffId = 131001;            //标记进入霸体状态的Buff
    protected int m_FlagGunMoveBuffId = 131003;         //标记枪口可以移动
    protected int m_ShanbaiBuffInfoId = 131001;         //开枪时闪白Buff信息ID
    protected bool mEnterNextPhaseFlag = false;         //进入到下一阶段的标志
    protected int mAddTime = 0;

    protected SkillSniperEffect m_SkillSniperEffect;    //用于技能表现
    protected IBeEventHandle m_PassDoorHandle = null;    //监听过门
    protected IBeEventHandle m_AddBuffHandle = null;     //监听Buff添加
    protected IBeEventHandle mDeadTowerEnterNextLayerHandle = null;  //监听过塔

    public Skill1310(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnInit()
    {
        m_ClickTimeAccPve = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        m_ClickTimeAccPvp = TableManager.GetValueFromUnionCell(skillData.ValueA[1], level);
        mMaxBulletPve = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
        mMaxBulletPvp = TableManager.GetValueFromUnionCell(skillData.ValueB[1], level);
        m_HurtIdPve = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
        m_HurtIdPvp = TableManager.GetValueFromUnionCell(skillData.ValueC[1], level);
        m_PvpMoveXOffset = TableManager.GetValueFromUnionCell(skillData.ValueD[0], level);
    }

    public override void OnStart()
    {
        mCurMaxBullet = BattleMain.IsModePvP(battleType) ? mMaxBulletPvp : mMaxBulletPve;
        m_ClickTimeAcc = BattleMain.IsModePvP(battleType) ? m_ClickTimeAccPvp : m_ClickTimeAccPve;
        m_CurrClickTimeAcc = 0;
        mCurBullet = 0;
        mAddTime = 0;
        mEnterNextPhaseFlag = false;
        RemoveHandle();
        m_AddBuffHandle = owner.RegisterEventNew(BeEventType.onAddBuff, (args) =>
        {
            BeBuff buff = (BeBuff) args.m_Obj;
            if (null != buff && buff.buffID == m_FlagBatiBuffId)
            {
                InitLogicData();
                InitEffectData();
            }
            else if (null != buff && buff.buffID == m_FlagGunMoveBuffId)
            {
                AttackPhaseStart();
            }
        });

        //修改子弹数量和技能总时间（UI显示）
        for (int i = 0; i < owner.MechanismList.Count; i++)
        {
            var m = owner.MechanismList[i] as Mechanism124;
            if (m != null)
            {
                mCurMaxBullet += m.bulletNum;
                mAddTime += m.addTime;
            }
        }
    }

    public override void OnUpdate(int iDeltime)
    {
        if (!m_CreateAttackFlag || m_SkillSniperEffect == null)
            return;
        UpdateAttackCD(iDeltime);
        CheckAttack();
        m_SkillSniperEffect.UpdateSkillTime(iDeltime);
    }

    public override void OnEnterPhase(int phase)
    {
        base.OnEnterPhase(phase);
        if (phase == 2)
        {
            AttackPhaseEnd();
        }
    }

    //更新攻击CD
    protected void UpdateAttackCD(int iDeltaTime)
    {
        if (!m_CreateAttackFlag)
            return;
        if (mCurBullet >= mCurMaxBullet)
            return;
        //手动释放CD
        if (m_CurrClickTimeAcc > 0)
        {
            m_CurrClickTimeAcc -= iDeltaTime;
        }
        else
        {
            m_SkillSniperEffect.SetAttackButtonEnable(true);
            m_CurrClickTimeAcc = 0;
        }
        m_SkillSniperEffect.RefreshCd(m_ClickTimeAcc, m_CurrClickTimeAcc);
    }

    //检测攻击
    protected void CheckAttack()
    {
        if (!m_CreateAttackFlag)
            return;
        if (mCurBullet >= mCurMaxBullet)
            return;
        if (owner.GetCurrentBtnState() == ButtonState.PRESS && m_CurrClickTimeAcc == 0)
        {
            CreateAttackFrameCommand();
        }
    }

    public override void OnCancel()
    {
        AttackPhaseEnd();
        CloseFrame();
    }

    public override void OnFinish()
    {
        CloseFrame();
    }

    protected void InitLogicData()
    {
        m_PassDoorHandle = owner.RegisterEventNew(BeEventType.onPassedDoor, (args) =>
        {
            LocomoteToIdle();
        });

        mDeadTowerEnterNextLayerHandle = owner.RegisterEventNew(BeEventType.onDeadTowerEnterNextLayer, (args) =>
        {
            Cancel();
        });
    }

    protected void InitEffectData()
    {
#if !LOGIC_SERVER
        m_SkillSniperEffect = new SkillSniperEffect(this, owner);
        m_SkillSniperEffect.m_SkillTotalTime += mAddTime;
        m_SkillSniperEffect.InitFrame();
#endif
    }

    //攻击阶段开始
    protected void AttackPhaseStart()
    {
        if (m_SkillSniperEffect == null)
            return;
        m_CreateAttackFlag = true;
        m_SkillSniperEffect.AttackPhaseStartEffect();
    }

    //发送攻击帧
    protected void CreateAttackFrameCommand()
    {
        mCurBullet++;
        m_SkillSniperEffect.CreateAttackFrame(mCurBullet, mCurMaxBullet);
        m_CurrClickTimeAcc = m_ClickTimeAcc;
    }

    //真正进行攻击
    public void DoRealAttack(int bulletNum, int pid)
    {
        if (bulletNum > mCurMaxBullet)
            return;

        if (owner.CurrentBeScene != null)
        {
            var target = owner.CurrentBeScene.GetEntityByPID(pid);
            if (target != null && (target as BeActor) != null)
            {
                var hitPos = target.GetPosition();
                hitPos.z += VInt.one.i;
                int hurtId = BattleMain.IsModePvP(battleType) ? m_HurtIdPvp : m_HurtIdPve;
                owner._onHurtEntity(target, hitPos, hurtId);
            }
        }

        if (pid == 0)
        {
            owner.buffController.TryAddBuff(131001);
            if (m_SkillSniperEffect != null)
            {
                m_SkillSniperEffect.ShowOtherAttackEffect();
            }
        }
        
        if (bulletNum >= mCurMaxBullet && !mEnterNextPhaseFlag)
        {
            mEnterNextPhaseFlag = true;
            ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
        }
    }

    //攻击阶段结束
    protected void AttackPhaseEnd()
    {
        m_CreateAttackFlag = false;
        RemoveHandle();
        if (m_SkillSniperEffect != null)
        {
            m_SkillSniperEffect.AttackPhaseEnd();
        }
    }

    //同步枪口位置
    public void DoSyncSight(int x, int z)
    {
        if (m_SkillSniperEffect == null)
            return;
        m_SkillSniperEffect.DoSyncSight(x, z);
    }

    //关闭页面
    protected void CloseFrame()
    {
        if (m_SkillSniperEffect == null)
            return;
        m_SkillSniperEffect.CloseFrame();
    }

    protected void RemoveHandle()
    {
        if (m_PassDoorHandle != null)
        {
            m_PassDoorHandle.Remove();
            m_PassDoorHandle = null;
        }

        if (m_AddBuffHandle != null)
        {
            m_AddBuffHandle.Remove();
            m_AddBuffHandle = null;
        }

        if (mDeadTowerEnterNextLayerHandle != null)
        {
            mDeadTowerEnterNextLayerHandle.Remove();
            mDeadTowerEnterNextLayerHandle = null;
        }
    }

    //强制切换到Idle状态
    protected void LocomoteToIdle()
    {
        owner.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
    }
}
