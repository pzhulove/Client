/*using System;
using System.Collections.Generic;
using GameClient;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 1）进场后，场内会出现UI狙击瞄准镜，狙击镜会在玩家当前屏幕中央出现；
/// 2）出现0.5秒后，狙击镜开始朝敌方目标进行移动。但是狙击镜无法超出召唤主的摄像机范围；
/// 3）敌方目标的优先规则为：有集火标记的单位＞场上最大生命值最多的单位
///
/// 配置：
/// A：瞄准镜攻击范围(默认270)
/// B：瞄准镜UI大小（默认1000）
/// C：子弹数
/// D：自动CD|手动CD(千)
/// E: 瞄准移动速度(默认100)
/// </summary>
public class Mechanism1123 : LockAttackMechanism
{
    /// <summary>
    /// 内部表现类
    /// </summary>
    protected class Graphic
    {
        public bool NewAnim = false;
        private SkillMonsterSniperFrame _sniperFrame;
        private SkillMonsterSniperOtherFrame _sniperOtherFrame; //其他人看到的狙击枪口页面
        private BeActor _owner;
        private int skillId = 1406;
        private int offsetZ = 10000;
        private Vector3 _sniperWorldPos;
        private Camera mainCamera;
        private int m_checkTargetUIRadius;
        private int m_sightMoveSpeed;
        protected int mSyncSightAcc = 250;                  //同步帧的时间间隔
        protected int mCurSyncSightAcc = 0;                 //当前时间间隔
        private BeActor _topActor;

        public void InitConfig(int uiRadius, int sniperMoveSpeed, int endScale, int beginScale, int scaleSpeed)
        {
#if !LOGIC_SERVER
            m_checkTargetUIRadius = uiRadius;
            m_sightMoveSpeed = sniperMoveSpeed;
            if (_sniperFrame != null)
            {
                _sniperFrame.InitSniperScale(endScale, beginScale, scaleSpeed);
            }
            if (_sniperOtherFrame != null)
            {
                _sniperOtherFrame.InitSniperScale(endScale, beginScale, scaleSpeed);
            }
#endif
        }

        /// <summary>
        /// 创建瞄准镜UI
        /// </summary>
        public void CreateUI(BeActor actor)
        {
#if !LOGIC_SERVER
            mainCamera = Camera.main;
            _owner = actor;
            _topActor = _owner.GetTopOwner(_owner) as BeActor;

            InitFrame();
#endif
        }

        /// <summary>
        /// 同步瞄准镜镜头
        /// </summary>
        public void UpdateSight(BeEntity target, bool lerpMove = true)
        {
#if !LOGIC_SERVER
            if (lerpMove)
            {
                if (target != null)
                {
                    var targetPos = target.GetPosition();
                    targetPos.z += offsetZ;
                    _sniperWorldPos = Vector3.Lerp(_sniperWorldPos, targetPos.vector3, m_sightMoveSpeed / 1000f);
                }                
            }
            Vector2 localPos = _GetTargetUIPos(_sniperWorldPos);

            if (_sniperFrame != null)
            {
                Vector3 pos = new Vector3(localPos.x, localPos.y, 0);
                _sniperFrame.SniperMove(pos);
                if (NewAnim && lerpMove)
                {
                    _sniperFrame.SniperScale();
                }
            }
#endif
        }

        public void UpdateLockSightPos()
        {
#if !LOGIC_SERVER
            if(_sniperFrame == null)
                return;
            
            _sniperWorldPos = _sniperFrame.GetSniperScenePos();
#endif
        }

        /// <summary>
        /// 获取准星对应场景坐标有没有攻击目标
        /// </summary>
        public void GetSightTarget(List<BeActor> list)
        {
#if !LOGIC_SERVER
            if(list == null)
                return;
            list.Clear();

            if (_sniperFrame == null)
                return;

            var targets = GamePool.ListPool<BeEntity>.Get();
            _owner.CurrentBeScene.GetEntitys2(targets);
            for (int i = 0; i < targets.Count; i++)
            {            
                var actor = targets[i] as BeActor;
                if (actor == null)
                    continue;
       
                if (actor.m_iCamp == _owner.m_iCamp)
                    continue;

                if ((actor as BeActor).IsSkillMonster())
                    continue;
                
                if (!actor.stateController.CanBeTargeted())
                    continue;

                if (CheckTargetInSign(actor))
                {
                    list.Add(actor);
                }
            }
            GamePool.ListPool<BeEntity>.Release(targets);
#endif
        }

        // 单位是否在枪口里
        public bool CheckTargetInSign(BeActor actor)
        {
            if (NewAnim)
                return true;
            
#if !LOGIC_SERVER
            if (_topActor == null)
                return false;

            if (!_topActor.isLocalActor)
                return false;
                
            if (actor == null)
                return false;

            if (_sniperFrame == null)
                return false;
            
            var targetPos = actor.GetPosition();
            targetPos.z += offsetZ;
            Vector2 localPos = _GetTargetUIPos(targetPos.vector3);
            return _sniperFrame.CheckTargetInSign(localPos, m_checkTargetUIRadius);
#else
            return false;
#endif
        }

        /// <summary>
        /// 发送攻击命令帧
        /// </summary>
        public void SendAttackFrame()
        {
#if !LOGIC_SERVER
            var list = GamePool.ListPool<BeActor>.Get();
            GetSightTarget(list);
            for (int i = 0; i < list.Count; i++)
            {
                if(list[i] == null)
                    continue;
                
                InputManager.CreateSkillDoattackFrameCommand(skillId, 1, list[i].GetPID());
            }
            InputManager.CreateSkillDoattackFrameCommand(skillId, 1, 0);
            GamePool.ListPool<BeActor>.Release(list);
#endif
        }

        public void OnAttack()
        {
#if !LOGIC_SERVER
            //if (ReplayServer.GetInstance().IsReplay())
            //    return;
            if (_sniperFrame == null)
                return;

            if (NewAnim)
            {
                _sniperFrame.ResetScale();
            }
            
            _sniperFrame.PlayAttackEffect();
#endif
        }
        
        public void CloseFrame()
        {
#if !LOGIC_SERVER
            //if (ReplayServer.GetInstance().IsReplay())
            //    return;
            
            if (_sniperFrame != null)
            {
                _sniperFrame.PlayCloseAni();
                _sniperFrame.Close();
                _sniperFrame = null;    
            }
            
            if (_sniperOtherFrame != null)
            {
                _sniperOtherFrame.PlayCloseAni();
                _sniperOtherFrame.Close();
                _sniperOtherFrame = null;    
            }
#endif
        }

        public void SyncSight(int deltaTime)
        {
#if !LOGIC_SERVER
            if (ReplayServer.GetInstance().IsReplay())
                return;
            if (!_topActor.isLocalActor)
    			return;
            if (_sniperFrame == null)
                return;
            if (mCurSyncSightAcc < mSyncSightAcc)
            {
                mCurSyncSightAcc += deltaTime;
            }
            else
            {
                mCurSyncSightAcc = 0;
                Vector3 centerWorldPoint = _sniperFrame.GetSniperScenePos();
                InputManager.CreateSkillSynSightFrameCommand(skillId, (int)((centerWorldPoint.x + 50) * 100), (int)((centerWorldPoint.z + 50) * 100));
            }
#endif
        }

        public void DoSyncSight(int x, int z)
        {
#if !LOGIC_SERVER
            //if (ReplayServer.GetInstance().IsReplay())
            //   return;
            if(_topActor.isLocalActor)
    			return;
            if (_sniperOtherFrame == null)
                return;
            float fx = x / 100.0f - 50;
            float fz = z / 100.0f - 50;
            Vector2 localPos = _GetTargetUIPos(new Vector3(fx, 0, fz));
            _sniperOtherFrame.SniperMove(localPos);
#endif
        }
        
#if !LOGIC_SERVER
        private void InitFrame()
        {
            //if (ReplayServer.GetInstance().IsReplay())
            //    return;
            if (_topActor != null && _topActor.isLocalActor)
            {
                if (_sniperFrame == null)
                {
                    _sniperFrame = CreateFrame<SkillMonsterSniperFrame>();
                }
            }
            else
            {
                // PVE不显示别人的枪口
                if (BattleMain.IsModePvP(_owner.battleType))
                {
                    if (_sniperOtherFrame == null)
                    {
                        _sniperOtherFrame = CreateFrame<SkillMonsterSniperOtherFrame>();
                    }
                }
            }
            InitSnipePosition();
        }
        
        private void InitSnipePosition()
        {
            _sniperWorldPos = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, _owner.GetPosition().vector3.z - mainCamera.transform.position.z));
        }

        /// <summary>
        /// 获取目标的UI坐标
        /// </summary>
        protected Vector3 _GetTargetUIPos(Vector3 worldPos)
        {

            var frame = GetFrame();

            if (frame == null)
                return Vector3.zero;
            
            var screenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPos);
            return frame.GetUIPointInFrame(screenPos);
        }

        private SkillMonsterSniperFrame GetFrame()
        {
            if (_topActor != null && _topActor.isLocalActor)
            {
                return _sniperFrame;
            }
            else
            {
                return _sniperOtherFrame;
            }
        }
        
        private T CreateFrame<T>() where T : SkillMonsterSniperFrame
        {
            var sniperFrame = ClientSystemManager.instance.OpenFrame<T>(FrameLayer.Bottom) as T;
            if (sniperFrame != null)
            {
                sniperFrame.gameObject.transform.SetAsFirstSibling();
            }

            return sniperFrame;
        }
#endif
    }

    public Mechanism1123(int mid, int lv) : base(mid, lv) { }

    protected int m_checkTargetUIRadius = 270;
    private int m_SniperEndScale = 1000;
    private int m_SniperBeginScale = 1000;
    private int m_SniperScaleSpeed = 50;
    private int m_totalBulletNum = 4;
    private int m_AutoAttackTimeCD = 2000;        // 自动CD
    private int m_ManualAttackTimeCD = 750;    // 手动CD
    private int m_sightMoveSpeed = 100;
    private bool NewAnim = false;

    private int m_delayMove = 500;
    private int m_attackDelayMove = 500;
    private BeActor _target;
    private int _hurtId = 14061;
    private int _juJiSkillId = 1415;
    private int _curBulletNum = 0;
    private bool _willDeadFlag = false;
    private Graphic _graphic;
    private BeActor _topOwner;
    private bool _noTargetFlag = false;
    private int m_curCDTime = 0;
    private Skill1406 m_Skill;
    private BeActorFilter m_filter;
    private int m_runTime = 0;
    private int m_attackDelayTimeAcc = 0;
    
    public override void OnInit()
    {
        base.OnInit();
        
        if (data.ValueA.Count == 2)
        {
            m_checkTargetUIRadius =  !BattleMain.IsModePvP(battleType) ? TableManager.GetValueFromUnionCell(data.ValueA[0], level) : TableManager.GetValueFromUnionCell(data.ValueA[1], level);
        }
        
        if (data.ValueB.Count > 0)
        {
            m_SniperBeginScale = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        }
        
        if (data.ValueC.Count == 2)
        {
            m_totalBulletNum = !BattleMain.IsModePvP(battleType) ? TableManager.GetValueFromUnionCell(data.ValueC[0], level) : TableManager.GetValueFromUnionCell(data.ValueC[1], level); 
        }
        
        if (data.ValueD.Count == 4)
        {
            m_AutoAttackTimeCD = !BattleMain.IsModePvP(battleType) ? TableManager.GetValueFromUnionCell(data.ValueD[0], level) : TableManager.GetValueFromUnionCell(data.ValueD[1], level);
            m_ManualAttackTimeCD = !BattleMain.IsModePvP(battleType) ? TableManager.GetValueFromUnionCell(data.ValueD[2], level) : TableManager.GetValueFromUnionCell(data.ValueD[3], level);
        }
        
        if (data.ValueE.Count == 2)
        {
            m_sightMoveSpeed = !BattleMain.IsModePvP(battleType) ? TableManager.GetValueFromUnionCell(data.ValueE[0], level) : TableManager.GetValueFromUnionCell(data.ValueE[1], level);
        }
        
        /*if (data.ValueF.Count > 0)
        {
            NewAnim = false;
        }#1#
        m_filter = new BeActorFilter(owner);
    }
    public override void OnStart()
    {
        base.OnStart();
        _InitEventRegister();
        _GetTopOwner();
        _InitGraphic();
        m_runTime = 0;
        m_attackDelayTimeAcc = m_attackDelayMove;
        m_Skill = GetSkill();
    }

    private void _InitGraphic()
    {
        if(_graphic != null)
            return;
        
        _graphic = new Graphic();
        _graphic.CreateUI(owner);
        _graphic.NewAnim = NewAnim;
        _graphic.InitConfig(m_checkTargetUIRadius, m_sightMoveSpeed, m_SniperEndScale, m_SniperBeginScale, m_SniperScaleSpeed);
    }

    private Skill1406 GetSkill()
    {
        if (_topOwner == null)
            return null;
        
        var skill = _topOwner.GetSkill(1406);
        if (skill != null)
        {
            return skill as Skill1406;
        }

        return null;
    }
    
    public override void OnUpdate(int deltaTime)
    {
        _UpdateCD(deltaTime);
        _UpdateFindTarget();
        _UpdateFaceToTarget();
        _SynSight(deltaTime);
    }

    private void _SynSight(int deltaTime)
    {
        if (!BattleMain.IsModePvP(battleType))
            return;
        
        if(_graphic != null)
            _graphic.SyncSight(deltaTime);
    }

    private enum SightState
    {
        Start,    // 开始时的状态
        Move,    // 瞄准状态
        Pre,    // 准备射击前的锁定状态(前摇)
        Post,    // 射击的后摇
        Max
    }

    private int[] mSightStateTime = new int[(int)SightState.Max];
    private int curStateTime = 0;
    private SightState mCurSightState;

    private void UpdateStateTime(int deltaTime)
    {
        curStateTime += deltaTime;
        int curStateTimeOut = mSightStateTime[(int) mCurSightState];
        if (curStateTime >= curStateTimeOut)
        {
            mCurSightState = ChangeToNextState(mCurSightState);
            curStateTime = 0;
        }
    }

    private void UpdateStateAction()
    {
        // 只有Move能移动
        if (mCurSightState == SightState.Move)
        {
            _graphic.UpdateSight(_target);    
        }
        else
        {
            _graphic.UpdateLockSightPos();
        }
    }

    private SightState ChangeToNextState(SightState curState)
    {
        SightState result = (curState + 1);
        if (result >= SightState.Max)
        {
            result = SightState.Move;
        }

        // 开枪前摇结束射击
        if (result == SightState.Pre)
        {
            if (m_Skill != null) 
                m_Skill.DoAttack();
        }
        return result;
    }

    public override void OnUpdateGraphic(int deltaTime)
    {
    	if(_topOwner == null || !_topOwner.isLocalActor)
    		return;

        m_runTime += deltaTime;
        m_attackDelayTimeAcc += deltaTime;
        if (m_attackDelayTimeAcc >= m_attackDelayMove)
        {
            if (_graphic != null)
                _graphic.UpdateSight(_target, CanMoveSniper());    
        }
        else
        {
            if (_graphic != null)
                _graphic.UpdateLockSightPos();
        }
    }

    /// <summary>
    /// 当刚开始与攻击后一段时间内不能移动镜头
    /// </summary>
    /// <returns></returns>
    private bool CanMoveSniper()
    {
        return m_runTime > m_delayMove;
    }

    private void _UpdateCD(int deltaTime)
    {
        m_curCDTime += deltaTime;

        if (m_Skill != null)
        {
            if (CanManualAttack())
            {
                m_Skill.LightSkillButton();
            }
        
            if (CanAutoAttack())
            {
                m_Skill.DoAttack();
            }
        }
    }

    private bool CanManualAttack()
    {
        return m_curCDTime >= m_ManualAttackTimeCD && _curBulletNum < m_totalBulletNum;
    }

    private bool CanAutoAttack()
    {
        return m_curCDTime >= (m_AutoAttackTimeCD + m_attackDelayMove) && _curBulletNum < m_totalBulletNum && 
               (_graphic != null && _graphic.CheckTargetInSign(_target));
    }
    
    public override void OnFinish()
    {
        base.OnFinish();
        if (_graphic != null)
            _graphic.CloseFrame();
        _graphic = null;
        _target = null;
        _topOwner = null;
        m_Skill = null;
        m_filter = null;
    }

    private void _InitEventRegister()
    {
        handleA = owner.RegisterEventNew(BeEventType.onSkillCancel, _OnSkillFinish);
        handleB = owner.RegisterEventNew(BeEventType.onCastSkillFinish, _OnSkillFinish);
        handleC = owner.CurrentBeScene.RegisterEvent(BeEventSceneType.onCreateMonster, _OnCreateMonster);
    }
    
    private void _GetTopOwner()
    {
        _topOwner = owner.GetTopOwner(owner) as BeActor;
    }

    /// <summary>
    /// 敌方目标的优先规则为：有集火标记的单位＞场上最大生命值最多的单位
    /// </summary>
    /// <returns></returns>
    private BeActor FindTarget()
    {
        // 集火优先
        var target = FindForceTarget();
        if (target != null)
        {
            return target;
        }
        
        // 最大血量
        target = FindMaxHpTarget();
        if (target != null)
        {
            return target;
        }

        return null;
    }
    
    /// <summary>
    /// 范围内的集火单位
    /// </summary>
    /// <returns></returns>
    private BeActor FindForceTarget()
    {
        if (ForceTarget != null && !ForceTarget.IsDeadOrRemoved())
        {
            return ForceTarget;
        }

        return null;
    }

    // 最大血量
    private BeActor FindMaxHpTarget()
    {
        if (owner.CurrentBeScene == null)
            return null;

        BeActor result = null;
        int maxHp = -1;
        var targets = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.GetFilterTarget(targets, m_filter);
        for (int i = 0; i < targets.Count; i++)
        {
            var curEntityData = targets[i].GetEntityData();
            if (curEntityData != null)
            {
                int hp = curEntityData.GetMaxHP();
                if (hp > maxHp)
                {
                    result = targets[i];
                    maxHp = hp;
                }
            }
        }
        GamePool.ListPool<BeActor>.Release(targets);

        return result;
    }

    private void _UpdateFaceToTarget()
    {
        if(_target == null)
            return;
        
        owner.SetFace((_target.GetPosition().x - owner.GetPosition().x) < 0);
    }
    
    /// <summary>
    /// 寻找目标
    /// </summary>
    protected void _UpdateFindTarget()
    {
        if (_target == null)
        {
            _target = FindTarget();
        }
        else
        {
            if (_target.IsDeadOrRemoved())
            {
                _target = FindTarget();
            }
        }
    }

    protected override void OnForceTargetChange()
    {
        _target = FindTarget();
    }

    
    private void _OnCreateMonster(object[] args)
    {
        _target = FindTarget();
    }

    /// <summary>
    /// 玩家手动点击攻击|自动攻击
    /// </summary>
    public void ClickAttack()
    {
        if (_curBulletNum >= m_totalBulletNum)
            return;
        
        if (ReplayServer.GetInstance().IsReplay())
            return;
        
        if (_topOwner != null && !_topOwner.isLocalActor)
            return;
        
        if (_graphic == null)
            return;
        
        _graphic.OnAttack();

        _graphic.SendAttackFrame();
        m_curCDTime = 0;
    }

    /// <summary>
    /// 真正造成伤害
    /// </summary>
    public void DoRealAttack(int pid)
    {
        if (_curBulletNum >= m_totalBulletNum)
            return;

        // Pid==0用于同步子弹
        if (pid == 0)
        {
            var skill = owner.GetSkill(_juJiSkillId);
            if (skill != null)
            {
                owner.CancelCurSkill();
                skill.ResetCoolDown();
                owner.UseSkill(_juJiSkillId);
            }
        
            _curBulletNum++;
            if (_curBulletNum == m_totalBulletNum)
                _willDeadFlag = true;
            m_curCDTime = 0;
            m_attackDelayTimeAcc = 0;
        }
        else
        {
            // 同步伤害逻辑
            if (owner.CurrentBeScene == null)
                return;
            var target = owner.CurrentBeScene.GetEntityByPID(pid) as BeActor;
            if (target == null || target.IsRemoved())
                return;
            var hitPos = target.GetPosition();
            hitPos.z += VInt.one.i;
            owner._onHurtEntity(target, hitPos, _hurtId);
        }
    }

    /// <summary>
    /// 监听技能结束
    /// </summary>
    private void _OnSkillFinish(BeEvent.BeEventParam args)
    {
        int skillId = args.m_Int;
        if (skillId != _juJiSkillId)
            return;
        if (!_willDeadFlag)
            return;
        if(!owner.IsDeadOrRemoved())
            owner.DoDead();
    }

    public void DoSyncSight(int x, int z)
    {
        if (_graphic == null)
            return;
        _graphic.DoSyncSight(x, z);
    }
}*/