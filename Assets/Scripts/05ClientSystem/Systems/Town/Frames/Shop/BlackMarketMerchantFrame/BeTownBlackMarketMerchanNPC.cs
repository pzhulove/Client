using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;

namespace GameClient
{
    class BeTownBlackMarketMerchanNPC : BeTownNPC
    {
        private const string mFootSfxPath = "Effects/Eff_Tongyong/Prefab/Eff_Tongyong_guaiwugongcheng_quan";
        private const string mStrAnimTownWalk = "Anim_Walk";    //移动动画
        private const string mStrAnimTownIdle = "Anim_Idle";    //待机动画
        private const string mStrAnimTownIdle01 = "Anim_Town_Idle_01";   //第二种待机动画

        //移动位置相关信息
        private float mIdleTime = 0.0f;
        private ETownAttackCityMonsterMoveState mETownNpcMoveState = ETownAttackCityMonsterMoveState.Invalid;
        
        /// <summary>
        /// NPC初始位置
        /// </summary>
        private Vector3 mBornPosition = Vector3.zero;
        /// <summary>
        /// 目标位置
        /// </summary>
        private Vector3 mTargetPosition = Vector3.zero;
        /// <summary>
        /// 目标位置和初始位置的距离
        /// </summary>
        private Vector3 mTargetOffsetPosition = Vector3.zero;

        /// <summary>
        /// 动画时间段
        /// </summary>
        private float mAnimTownIdle01Interval = 0.0f;
        /// <summary>
        /// 状态时间
        /// </summary>
        private float mTownIdle01Time = 0.0f;
        /// <summary>
        /// 移动速度
        /// </summary>
        private Vector3 mMoveSpeed = Vector3.zero;

        /// <summary>
        /// 向右移动终点
        /// </summary>
        private Vector3 mEndVet3 = Vector3.zero;

        private BeTownNPCData mBeTownNPCData = null;
        private NpcTable mNpcTable = null;

        private BeTownAttackCityMonsterDialogComponentController _dialogComponentController = null;

        public BeTownBlackMarketMerchanNPC(BeTownNPCData data, ClientSystemTown systemTown) : base(data, systemTown)
        {
        }

        public sealed override void InitGeActor(GeSceneEx geScene)
        {
            if (geScene == null)
            {
                return;
            }

            try
            {
                if (_geActor == null)
                {
                    mBeTownNPCData = _data as BeTownNPCData;
                    if (mBeTownNPCData == null)
                        return;

                    mNpcTable = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(mBeTownNPCData.NpcID);
                    if (mNpcTable == null)
                        return;

                    _geScene = geScene;

                    if (EngineConfig.useTMEngine)
                        _geActor = geScene.CreateActorAsyncEx(mBeTownNPCData.ResourceID, 0, mNpcTable.Height, false, false);
                    else
                        _geActor = geScene.CreateActorAsync(mBeTownNPCData.ResourceID, 0, mNpcTable.Height, false, false);


                    if (_geActor == null)
                    {
                        return;
                    }

                    ActorData.MoveData.TransformDirty = true;
                    UpdateGeActor(0.0f);

                    _geActor.SuitAvatar();
                    //////加载NPCVoice脚本
                    //_geActor.AddNpcVoiceComponent(_townData.NpcID);
                    //////加载箭头
                    //_geActor.AddNpcArrowComponent();

                    //添加加载完成之后的回调
                    AddBlackMarketMerchanComponent();

                }
            }
            catch (System.Exception ex)
            {
                _geActor = null;
                Logger.LogError(ex.ToString());
            }
        }

        protected void AddBlackMarketMerchanComponent()
        {
            if (mBeTownNPCData == null || _geActor == null)
            {
                return;
            }

            _geActor.PushPostLoadCommand(() =>
            {
                if (_geActor == null || mBeTownNPCData == null || mNpcTable == null)
                    return;

                AddBlackMarketMerchanFunction();
                AddBlackMarketMerchanNpcDialog();
                //AddBlackMarketMerchanNpcFootShadow();
                SetBlackMarketMerchanMoveStates();
            });
        }

        /// <summary>
        /// 添加名称，title和点击效果
        /// </summary>
        private void AddBlackMarketMerchanFunction()
        {
            _geActor.AddNpcInteraction(mBeTownNPCData.NpcID);

            float defaultNamePosY = 0.0f;
            if (mNpcTable.NameLocalPosY > 0)
            {
                defaultNamePosY = (float)mNpcTable.NameLocalPosY / 1000.0f;
            }

            _geActor.AddNPCFunction(mBeTownNPCData.Name, mBeTownNPCData.NameColor, 0, null, defaultNamePosY);
        }

        /// <summary>
        /// 添加气泡对话框
        /// </summary>
        private void AddBlackMarketMerchanNpcDialog()
        {
#if !LOGIC_SERVER
            _geActor.AddComponentDialog(mBeTownNPCData.NpcID, NpcDialogComponent.IdBelong2.IdBelong2_NpcTable);

            if (_geActor.NpcDialogComponent == null)
                return;

            //设置对话框内容
            _geActor.NpcDialogComponent.SetContentText();

            //添加对话框显示控制器
            _dialogComponentController = _geActor.NpcDialogComponent.transform.gameObject
                .GetComponent<BeTownAttackCityMonsterDialogComponentController>();
            if (_dialogComponentController == null)
            {
                _dialogComponentController = _geActor.NpcDialogComponent.transform.gameObject
                    .AddComponent<BeTownAttackCityMonsterDialogComponentController>();
            }

            if (_dialogComponentController == null)
                return;

            _dialogComponentController.InitController(mBeTownNPCData.NpcID);


            //调整npc冒泡的位置，在名字位置上面0.5的位置
            //以前的位置有问题
            var parentRoot = _geActor.NpcDialogComponent.transform.parent;
            if (parentRoot == null)
                return;

            if (mNpcTable.NameLocalPosY > 0)
            {
                var parentRootLocalPos = parentRoot.transform.localPosition;
                parentRoot.transform.localPosition = new Vector3(
                    parentRootLocalPos.x,
                    (float)mNpcTable.NameLocalPosY / 1000.0f + 0.5f,
                    parentRootLocalPos.z);

                var dialogComponentLocalPos = _dialogComponentController.transform.localPosition;
                _dialogComponentController.transform.localPosition = new Vector3(
                    0,
                    0,
                    dialogComponentLocalPos.z);
            }

            var mGoInfoBar = _geActor.goInfoBar;
            if (mGoInfoBar == null)
                return;

            var goInfoBarLocalPos = mGoInfoBar.transform.localPosition;
            mGoInfoBar.transform.localPosition =
                new UnityEngine.Vector3(1.3f, goInfoBarLocalPos.y, goInfoBarLocalPos.z);
#endif
        }

        /// <summary>
        /// 调整NPC名字位置
        /// </summary>
        /// <param name="direction">方向</param>
        private void AdjustNameNpcDialogPostion(bool direction)
        {
#if !LOGIC_SERVER
            var mGoInfoBar = _geActor.goInfoBar;
            if (mGoInfoBar == null)
                return;

            var parentRoot = _geActor.NpcDialogComponent.transform.parent;
            if (parentRoot == null)
                return;

            if (direction)
            {
                var goInfoBarLocalPos = mGoInfoBar.transform.localPosition;
                mGoInfoBar.transform.localPosition = 
                    new UnityEngine.Vector3(1.3f, goInfoBarLocalPos.y, goInfoBarLocalPos.z);

                var parentRootLocalPos = parentRoot.transform.localPosition;
                parentRoot.transform.localPosition = new Vector3(
                    1.1f,
                    parentRootLocalPos.y,
                    parentRootLocalPos.z);
            }
            else
            {
                var goInfoBarLocalPos = mGoInfoBar.transform.localPosition;
                mGoInfoBar.transform.localPosition =
                    new UnityEngine.Vector3(-1.4f, goInfoBarLocalPos.y, goInfoBarLocalPos.z);

                var parentRootLocalPos = parentRoot.transform.localPosition;
                parentRoot.transform.localPosition = new Vector3(
                    -1.6f,
                    parentRootLocalPos.y,
                    parentRootLocalPos.z);
            }
#endif
        }

        /// <summary>
        /// 添加脚底阴影
        /// </summary>
        private void AddBlackMarketMerchanNpcFootShadow()
        {
            _geActor.CreateFootIndicator(mFootSfxPath);
        }
        
        public sealed override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            UpdateDialogComponentController(deltaTime);
        }

        private void UpdateDialogComponentController(float deltaTime)
        {
            if (_dialogComponentController != null)
            {
                _dialogComponentController.OnUpdate(deltaTime);
            }
        }

        //设置状态
        private void SetBlackMarketMerchanMoveStates()
        {
            InitBlackMarketMerchanMoveStates();

            //获得Idle01的时间间隔
            mAnimTownIdle01Interval = _geActor.GetActionTimeLen(mStrAnimTownIdle01);
        }

        private void InitBlackMarketMerchanMoveStates()
        {
            if(mBeTownNPCData == null)
                return;

            //非黑市商人
            if (mBeTownNPCData.TownNpcType != ESceneActorType.BlackMarketMerchanNpc)
                return;


            //设置移动速度和阴影
            mMoveSpeed = new Vector3(0.4f, 0f, 0f);
            _geActor.AddSimpleShadow(Vector3.one);
            mBeTownNPCData.MoveData.FaceRight = true;
            mEndVet3 = new Vector3(BlackMarketMerchantDataManager.BlackMarketMerchantEndPos, 0f, 0f);

            mETownNpcMoveState = ETownAttackCityMonsterMoveState.BornIdle;
            mIdleTime = RandomIdleTime();
            mBornPosition = mBeTownNPCData.MoveData.Position;

            SetAnimationAction(mStrAnimTownIdle);
            SetNpcMoveTarget();
        }

        float RandomIdleTime()
        {
            return UnityEngine.Random.Range(BlackMarketMerchantDataManager.BlackMarketMerchantRandomPlayerNextAniamtionMinTime, BlackMarketMerchantDataManager.BlackMarketMerchantRandomPlayerNextAniamtionMaxTime);
        }

        private void SetNpcMoveTarget()
        {
            //设置向左移动的目标位置
            if (!mBeTownNPCData.MoveData.FaceRight)
            {
                mTargetPosition = mBeTownNPCData.MoveData.Position + new Vector3(-1 * UnityEngine.Random.Range(4.1f, 4.3f), 0, 0);
                mTargetOffsetPosition = mTargetPosition - mBeTownNPCData.MoveData.Position;
            }
            else
            {
                //设置向左移动的目标位置
                mTargetPosition = mBeTownNPCData.MoveData.Position + new Vector3(1 * UnityEngine.Random.Range(4.1f, 4.3f), 0, 0);
                mTargetOffsetPosition = mTargetPosition - mBeTownNPCData.MoveData.Position;
            }


            AdjustNameNpcDialogPostion(mBeTownNPCData.MoveData.FaceRight);
        }

        public override void UpdateMove(float timeElapsed)
        {
            if (mETownNpcMoveState == ETownAttackCityMonsterMoveState.Invalid)
            {
                base.UpdateMove(timeElapsed);
            }
            else
            {
                UpdateNpcMoveAi(timeElapsed);
            }
        }

        //进行状态的切换，可以优化为状态机模式
        private void UpdateNpcMoveAi(float timeElapsed)
        {
            if (mETownNpcMoveState == ETownAttackCityMonsterMoveState.Invalid)
                return;

            switch (mETownNpcMoveState)
            {
                case ETownAttackCityMonsterMoveState.BornIdle:
                    UpdateBornIdleState(timeElapsed);
                    break;
                case ETownAttackCityMonsterMoveState.ForwardMove:
                    UpdateForwardMoveState(timeElapsed);
                    break;
                case ETownAttackCityMonsterMoveState.MoveIdle:
                    UpdateMoveIdleState(timeElapsed);
                    break;
                case ETownAttackCityMonsterMoveState.BackMove:
                    UpdateBackMoveState(timeElapsed);
                    break;
                case ETownAttackCityMonsterMoveState.TownIdle01:
                    UpdateTownIdle01State(timeElapsed);
                    break;
            }
        }

        //出生点的Idle状态
        private void UpdateBornIdleState(float timeElapsed)
        {
            if (mIdleTime > 0)
            {
                mIdleTime -= timeElapsed;
                return;
            }

            SetNextStateOfBornIdle();
        }

        private void SetNextStateOfBornIdle()
        {

            //var index = UnityEngine.Random.Range(0, 10);

            ////townIdle01动画存在，并且概率为20%，执行特殊的Idle；其他情况，执行walk
            //if (mAnimTownIdle01Interval > 0 && index >= 6)
            //{
            //    mETownNpcMoveState = ETownAttackCityMonsterMoveState.TownIdle01;
            //    mTownIdle01Time = mAnimTownIdle01Interval;
            //    SetAnimationAction(mStrAnimTownIdle01);
            //}
            //else
            {
                if (mBeTownNPCData.MoveData.FaceRight)
                {
                    mETownNpcMoveState = ETownAttackCityMonsterMoveState.ForwardMove;
                }
                else
                {
                    mETownNpcMoveState = ETownAttackCityMonsterMoveState.BackMove;
                }
               
                SetAnimationAction(mStrAnimTownWalk);
            }
        }

        //前进
        private void UpdateForwardMoveState(float timeElapsed)
        {
            if (_CheckPosEqual(mTargetPosition, mBeTownNPCData.MoveData.Position) == true
                || ((mTargetOffsetPosition.x < 0 && (mBeTownNPCData.MoveData.Position.x < mTargetPosition.x)))
                || ((mTargetOffsetPosition.x > 0 && (mBeTownNPCData.MoveData.Position.x > mTargetPosition.x))))
            {
                //移动结束
                mBeTownNPCData.MoveData.Position = mTargetPosition;
            }
            else
            {
                //移动中，
                mBeTownNPCData.MoveData.Position =
                    mBeTownNPCData.MoveData.Position + (mTargetOffsetPosition.x < 0 ? -4 : 4) * timeElapsed * mMoveSpeed;
                return;
            }
            
            if ((mEndVet3 - mBeTownNPCData.MoveData.Position).x <= 0.6f)
            {
                //当前状态结束，设置新的状态
                mETownNpcMoveState = ETownAttackCityMonsterMoveState.MoveIdle;
                //设置下一个状态Idle时间     
                mIdleTime = RandomIdleTime();
                //设置人物动画状态（idle状态）
                SetAnimationAction(mStrAnimTownIdle);
            }
            else
            {
                //当前状态结束，设置新的状态
                mETownNpcMoveState = ETownAttackCityMonsterMoveState.BornIdle;
                //设置下一个状态Idle时间     
                mIdleTime = RandomIdleTime();
                //设置人物动画状态（idle状态）
                SetAnimationAction(mStrAnimTownIdle);

                SetNpcMoveTarget();
            }
           
        }

        //移动到一定距离的Idle
        private void UpdateMoveIdleState(float timeElapsed)
        {
            if (mIdleTime > 0)
            {
                mIdleTime -= timeElapsed;
                return;
            }

            if (mBeTownNPCData.MoveData.FaceRight)
            { //当前状态结束，设置新的状态
                mETownNpcMoveState = ETownAttackCityMonsterMoveState.BackMove;
            }
            else
            {
                //当前状态结束，设置新的状态
                mETownNpcMoveState = ETownAttackCityMonsterMoveState.ForwardMove;
            }
            
            //人物的朝向
            mBeTownNPCData.MoveData.FaceRight = !mBeTownNPCData.MoveData.FaceRight;
            //设置人物的动画状态,(walk状态)
            SetAnimationAction(mStrAnimTownWalk);

            //重置AI
            SetNpcMoveTarget();
        }

        //后退
        private void UpdateBackMoveState(float timeElapsed)
        {

            if (_CheckPosEqual(mBeTownNPCData.MoveData.Position, mTargetPosition) == true 
                || ((mTargetOffsetPosition.x < 0 && (mBeTownNPCData.MoveData.Position.x < mTargetPosition.x)))
                || ((mTargetOffsetPosition.x > 0 && (mBeTownNPCData.MoveData.Position.x > mTargetPosition.x))))
            {
                //移动结束
                mBeTownNPCData.MoveData.Position = mTargetPosition;
            }
            else
            {
                //移动中，
                mBeTownNPCData.MoveData.Position =
                    mBeTownNPCData.MoveData.Position + (mTargetOffsetPosition.x < 0 ? -4 : 4) * timeElapsed * mMoveSpeed;
                return;
            }

            if ((mBeTownNPCData.MoveData.Position - mBornPosition).x <= 0.6f)
            {
                //当前状态结束，设置新的状态
                mETownNpcMoveState = ETownAttackCityMonsterMoveState.MoveIdle;
                //设置下一个状态Idle时间     
                mIdleTime = RandomIdleTime();
                //设置人物动画状态（idle状态）
                SetAnimationAction(mStrAnimTownIdle);
            }
            else
            {
                //当前状态结束，设置新的状态
                mETownNpcMoveState = ETownAttackCityMonsterMoveState.BornIdle;
                //设置Idle的时间
                mIdleTime = RandomIdleTime();
                //设置人物的动画状态（idle状态）
                SetAnimationAction(mStrAnimTownIdle);

                SetNpcMoveTarget();
            }
           
        }

        //特殊的Idle
        private void UpdateTownIdle01State(float timeElapsed)
        {
            if (mTownIdle01Time > 0.0f)
            {
                mTownIdle01Time -= timeElapsed;
                return;
            }

            mETownNpcMoveState = ETownAttackCityMonsterMoveState.BornIdle;
            mIdleTime = RandomIdleTime();
            SetAnimationAction(mStrAnimTownIdle);
        }



        //设置攻城怪物的自身动画，Anim_Town_Walk, Anim_Town_Idle
        private void SetAnimationAction(string animationAction)
        {
            mBeTownNPCData.ActionData.ActionName = animationAction;
        }

        public sealed override void Dispose()
        {
            base.Dispose();
            mBeTownNPCData = null;
            mNpcTable = null;
            mETownNpcMoveState = ETownAttackCityMonsterMoveState.Invalid;
            _dialogComponentController = null;
        }
    }
}

