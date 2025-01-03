using System.Collections.Generic;
using System.ComponentModel;

public enum BeEventType
{
    //battle

    [Description("进入战斗")]
    onEnterBattle = 1,
    [Description("场景清空")]
    onSceneClear,

    [Description("连击")]
    onBattleCombo,

    [Description("连击中断")]
    onBattleComboStop,

    [Description("进入场景")]
    onEnterScene,
    OnBeforePassDoor,
    onStartPassDoor,    //开始场景切换
    onPassedDoor,		//过门结束
    onDeadTowerEnterNextLayer,           //死亡之塔进入到下一层
    [Description("hp改变")]
    onHPChanged,
    [Description("受伤")]
    onHurt,
    [Description("死亡")]
    onDead,
    onAfterDead,
    onBeforeAfterDead,

    [Description("出生")]
    onBirth,
    /// <summary>
    /// 复活 
    /// </summary>
    onReborn,
    [Description("释放技能")]
    onCastSkill,
    onCastSkillFinish,
    [Description("走到边界处")]
    onWalkToAreaLimit,
    [Description("空中最高处")]
    onReachTop,
    [Description("触底")]
    onTouchGround,
    [Description("状态切换")]
    onStateChange,
    onStateChangeEnd,   //切换状态完成 已经切换到新的状态
    OnBuffHpChange,
    onChangeFace,
    OnHurtEnter,    // 攻击判定入口
    OnAttackResult,
    OnChangeEffectTime,//修改触发效果时间
    onCastNormalAttack, //释放普攻

    onBeforeGenBullet,	//产生弹道之前
    onAfterGenBullet,   //弹道产生之后
    onOwnerAfterGenBullet,//弹道产生之后(自身)
    onChangeLaunchProNum,   //改变发射实体的数量
    onChangeModifySpeed,//改变属性操作力
    onChangeSkillTime,  //改变技能阶段时间
    // Entity
    onCollide,          //碰撞到别人
    onCollideOther,     //别人碰撞到自己
    onBeforeHit,		//击中后计算伤害前
    onBeforeOtherHit,   //被别人击中后计算伤害前
    onAfterCalFirstDamage,//产生第一次标准伤害
	onAfterFinalDamage, //计算出最终伤害后
    onAfterFinalDamageNew, //计算出最终伤害后(新的考虑到实体攻击到对象 触发召唤者的事件)
    onBeHitAfterFinalDamage, //被别人攻击计算出最终伤害后
    onHit,				//被击
    onHitAfterAddBuff,
    onHitOtherAfterAddBuff,
    BlockSuccess,       //成功格挡
    ConfigCommand,      //收到操作同步帧
    onHitOther,         //击中别的单位
    onHitOtherAfterHurt,//在计算伤害以后触发该事件
	onGrabbed,			//被抓取
	onKill,				//杀死敌人
	onSelfKill,			//自身杀敌敌人
	onBeKilled,			//被杀死
	onMarkRemove,       //被标记删除时
	onRemove,			//被删除
    onHitCritical,      //产生暴击
    onHitCriticalBeforDamage,//触发暴击 产生伤害之前
	onHPChange,			//HP发生变化
	onMPChange,			//MP发生变化
    onAttrChange,       //目前只应用于四维属性
    OnChangeWeapon,     //切换武器
    OnChangeWeaponEnd,  //更换武器操作处理完
    onChangeEquipEnd,   //切换装备完成
    onBreakAction,      //破招
    onBackHit,          //被背击
    OnBeforeInitData,    //创建怪物
    onBeforeSummon,		//召唤前
    onSummon,           //召唤
    onChangeSummonScale,//改变召唤兽大小
    onChangeSummonWeight,//改变召唤兽重量
    onChangeSummonLifeTime, //改变召唤兽存在时间
    onAddBuff,          //添加buff
    onRemoveBuff,       //删除buff
    OnAddBuffToOthers,//给别人添加眩晕BUFF
    onChangeHurtValue,     //最终伤害数值调整
    onSpecialDead,      //特殊的死亡事件(例如斩杀机制直接弄死)
    onReplaceHurtTableDamageData,     //改变触发效果表读取到的初始伤害数值
    onReplaceHurtTableCiriticalData,     //改变触发效果表读取到的初始暴击数值
    onGetUp,            //起身
    //BeProjectile
    onCollideByProjectile,  //被实体碰撞到
    onXInBlock,             //在X轴方向碰到阻挡
    onYInBlock,             //在Y轴方向碰到阻挡
    onEnterEventArea,   //进入自定义事件区域
    onExitEventArea,    //离开自定义事件区域
    OnChangeSpeed,
    OnUseCrystal,
    onPetSkill,			//宠物可以放技能
    OnReleaseButtonTrigger,
	//技能相关
	onAddRune,			//产生波动刻印
	onClearRune,		//清空刻印
    OnConsumeRune,      //消耗刻印
    onPreSetSkillAction,//确定技能配置之前
    onActionLoop,	    //技能配置文件循环
    onJumpBackAttack,   //监听后跳技能释放
    onReplaceSkill,     //替换技能
    onSkillCurFrame,    //监听到技能某一帧的触发
	onChangeAttackDBox, //改变攻击框XY值
    onChangeAttackZDim, //改变攻击框ZDim值
    onSkillPostInit,    //技能PostInit完成
    onSkillCoolDownStart,   //技能开始冷却
    onSkillCoolDownFinish,  //技能冷却完成
    OnSkillCoolDown,    //技能CD结束
    OnSkillChargeComplete, //技能蓄力完成
    CanUseSkill,            // 技能能否释放（额外判定）
    onClickAgain,			// 再次点击技能
    onReleaseJoystick,		// 释放摇杆

    onEnterPhase,

    //具体技能
    onBoomerangHit,     //双鹰回旋回到身上
    onChangeBoomerangStayDuration,//修改双鹰回旋停留时间
    onReplaceComboSkill,//替换Combo技能阶段
    onChangeModelFinish,//变身结束
    OnJudgeGrab,    //判定目标能否被抓取
    OnGrab,              //抓取
    onActorAdd,
    onActorRemove,
    OnChangeAttributeDefence,
    onChangeClickForce, //改变落地弹跳时的Z轴速度
    OnChangeMoveDir,
    OnFakeReborn,       //圣骑士复活

    onMoveJoystick,     //摇杆移动
    onStopMoveJoystick, //摇杆停止

    onStartMove,
    onStopMove,
    OnPlayDeadEffect,

    //Skill
    onExcuteGrab,    //监听抓取 
    onEndGrab,
    onSkillCancel,  //技能被中断
    onSkillStart,   //技能开始
	onBeExcuteGrab, //被执行抓取
    onNextPhaseBeforeExecute,       //技能跳阶段执行技能前
    onGrabPressCountAdd,            //被抓去可摇动，pressCount增加后
    onSkillCanBeInterrupt, //当前技能是否可以被其他技能打断
    onSkillEventChangeAnimation,    //技能事件改变动作
    onExecSkillFrame,

    onWillUseSkill,  //准备释放技能
    // onSpecialSkillCombo,    //特殊技能Combo机制

    onChangeYinluoFlag, //改变银光落刃技能是否可以释放的标志

    // Range
    onRangeIn,
    onRangeOut,
    onRangeInside,
    
    //AI
    onAIStart,
    onExecuteAICmd,
    onAIMoveEnd,

    //buff
    onBuffStart ,
    onBuffRefresh,
    onBuffUpdate,
    onBuffFinish,
    onBuffDispose,
    onBuffCancel,
    onBuffReachLimit,
    onBuffCreateEffect,
    onBuffRemoveEffect,
    onBuffReplaceEffect,
    onBuffBeforePostInit,       //在BuffPosInit之前
    onChangeBuffAttackRate,     //改变Buff的附加概率
    onChangeBuffLevel,          //改变Buff等级
    onChangeBuffAttack,         //改变异常Buff攻击力
    OnBuffAddSkillAttr,
    OnBuffRemoveSkillAttr,
    OnChangeAbnormalBuffLevel,
    OnBuffDamage,               //buff直接扣血
    OnBuffHeal,               //buff直接治疗
    BuffCanAdd,
    //Effect
    onRepeatAttackInterval,     //改变重复攻击间隔
    onChangeHitThrough,         //改变穿刺率
    onChangeScreenShakeID,      //改变震屏效果ID
    onChangeHitEffect,          //改变被击特效
    onChangeFloatingRate,       //改变浮空力
    onChangeFloatYForce,        //改变空中浮空力
    onChangeSummonNumLimit,     //改变召唤怪物数量上限
    onChangeDamage,             //改变触发效果攻击力（千分比）
    onBeHitChangeDamage,        //被击改变触发效果攻击力（千分比）
    onChangeBuffTargetRadius,   //改变BUFF目标选择范围（千分比）
    onChangeBuffRangeRadius,    //改变BUFF范围半径（千分比）
    onChangeXRate,              //改变X轴推力
    onChangeFloatXForce,        //改变空中X轴推力
    onChangeMagicElement,       //改变攻击属性
    onChangeMagicElementList,   //改变属性攻击类型列表(用于火破冰等效果)
    onAddTriggerBuff,       //增加触发buff时的事件
    onHitEffect,            //改变中招反应

    onChangeSummonMonsterAttach,    //改变召唤兽的Buff附加伤害
    onChangeSummonMonsterAddDamage, //改变召唤兽的增加伤害
    onChangeSummonMonsterAddCritiDamage, //改变召唤兽的暴击伤害加成

	//Mechanism
    onAddMechanism,             //添加机制
    //Projectile
    onChangeProjectileSpeed,    //实体速度设置之前先改变技能配置文件里面的速度参数
    onChangeHardValue,          //改变僵直值
    onDeadProtectEnd,           //死亡保护结束
    onBackModeEnd,              //追帧结束

    onReachMaxEnergy,           //能量值满了
    onEnergyStatChange,         //能量值状态变化

    onTrainingPveResetSkillCD,  //修炼场点击重置技能冷却按钮
	onMagicGirlMonsterChange,   //召唤师觉醒
    onMagicGirlMonsterRestore,  //召唤师觉醒恢复

    //玩家在副本操作相关
    onSyncDungeonOperation,
	onChangeBeHitEffectPos, //改变被击特效的位置
    onChangeBeHitNumberPos, //改变受击文字的位置
    onChangeHitNumberType,//改变攻击文字的字体

    //团本空战变身
    RaidBattleChangeMonster,    //团本空战变身

    AbnormalBuffHurt,        // 异常buff造成伤害

    onAddChaser,            // 炫纹生成时
    onRemoveChaser,          // 炫纹死亡时(主动使用时，不算自然消亡) 
    
    // 明王
    onMingWangSetEnergy,    // 明王增加剑气
    onMingWangUseEnergy,    // 明王使用剑气

    onPlayAction,           // 播放动作
    onOpen2ndState,         // 开启第二形态
    onClose2ndState,        // 关闭第二形态

    onCalcRuneAddDamage,
    onMechanism2050RestoreBtn,
    onChangeShock,  //禁止抖动
}

public enum BeEventSceneType
{
    onCreate,
    onClear,

    /// <summary>
    /// 怪物死亡的时候
    /// </summary>
    onMonsterDead,
    onAfterGenBullet,
    onEggDead,//彩蛋死亡

    /// <summary>
    /// 怪物被移除的时候
    ///
    /// 这里的被移除是BeEntity.m_iRemoveTime计时到0的时候
    /// 不是指表现上的移除
    /// </summary>
    onMonsterRemoved,
    onCreateMonster,
    onDoorStateChange,
	onBossDead,//boss死亡的瞬间
    onKill,
    onEnter,
    onExit,
    onPlayerDead,
    onPlayerLevelUp,
    onDailog,
    onSummon,
	onStartPK,
    onDoHurt,           //对别人造成伤害
    onHurtByAbnormalBuff,       //异常Buff造成伤害
    OnEndAirBattle,
    OnStartAirBattle,
    // Buff
    OnChangeBuff,
    OnAddBuff,
    OnRemoveBuff,
    //死亡之塔相关
    onDeadTowerPassFiveLayer,   //死亡之塔五层创关成功
    on3v3SwitchNext,            //3v3切换到下一场
    //实体相关
    onAddEntity,
    onEntityRemove,
    onEntityDead,
    onMouthClose, //贝希摩斯嘴开合 
    onChangeSummonMonsterAttr,    //改变召唤兽的属性
    
    onHurtEntity, 
    onBattleExit,   //退出战斗

	onZhihuiguanSelectTarget,   //指挥官集火目标
	onZhihuiguanUnSelectTarget, //指挥官集火目标移除
	onChangeStartPos,   //改变实体初始位置
	onHitEffect,        //被击特效
    onGroupAction,      //小组作战

    AfterOnReady,       //场景离开OnReady状态

    onPickGold,          //捡起金币
}

public interface IBeEventHandle
{
    void Remove();
}