using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoTable;

namespace GameClient
{
    public enum ETownAttackCityMonsterMoveState
    {
        Invalid = -1,
        BornIdle = 0,           //出生的Idle状态
        ForwardMove = 1,        //前进到目的地的状态
        MoveIdle = 2,           //在目的地的Idle状态
        BackMove = 3,           //回退到出生点的状态
        TownIdle01 = 4,         //特殊的Idle
    }

    class BeTownAttackCityMonster : BeTownNPC
    {
        private const string FootSfxPath = "Effects/Eff_Tongyong/Prefab/Eff_Tongyong_guaiwugongcheng_quan";
        private const string AnimTownWalkStr = "Anim_Town_Walk";    //移动动画
        private const string AnimTownIdleStr = "Anim_Town_Idle";    //待机动画
        private const string AnimTownIdle01Str = "Anim_Town_Idle_01";   //第二种待机动画

        //移动位置相关信息
        private float _idleTime = 0.0f;
        private ETownAttackCityMonsterMoveState _eTownNpcMoveState = ETownAttackCityMonsterMoveState.Invalid;
        private Vector3 _bornPosition = Vector3.zero;       // 初始位置
        private Vector3 _targetPosition = Vector3.zero;     //目标位置
        private Vector3 _targetOffsetPosition = Vector3.zero;   //目标位置和初始位置的差距

        private float _animTownIdle01Interval = 0.0f;           //动画的时间段
        private float _townIdleO1Time = 0.0f;                   //状态时间

        private Vector3 _moveSpeed = Vector3.zero;              //移动的速度

        private BeTownNPCData _townData = null;
        private NpcTable _npcItem = null; 

        private BeTownAttackCityMonsterDialogComponentController _dialogComponentController = null;

        public BeTownAttackCityMonster(BeTownNPCData data, ClientSystemTown systemTown)
            : base(data, systemTown)
        {

        }

        public override void InitGeActor(GeSceneEx geScene)
        {
            if (geScene == null)
            {
                return;
            }

            try
            {
                if (_geActor == null)
                {
                    _townData = _data as BeTownNPCData;
                    if(_townData == null)
                        return;

                    _npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(_townData.NpcID);
                    if(_npcItem == null)
                        return;

                    _geScene = geScene;

                    if (EngineConfig.useTMEngine)
                        _geActor = geScene.CreateActorAsyncEx(_townData.ResourceID, 0, _npcItem.Height, false, false);
                    else
                        _geActor = geScene.CreateActorAsync(_townData.ResourceID, 0, _npcItem.Height, false, false);


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
                    AddAttackCityMonsterComponent();

                }
            }
            catch (System.Exception e)
            {
                _geActor = null;
                Logger.LogError(e.ToString());
            }
        }

        #region AttackCityMonsterComponent
        //怪物模型加载完成之后，添加组件
        protected void AddAttackCityMonsterComponent()
        {
            if(_townData == null || _geActor == null)
                return;

            _geActor.PushPostLoadCommand(
                () =>
                {
                    if(_geActor == null || _townData == null || _npcItem == null)
                        return;

                    AddAttackCityMonsterFunction();
                    AddAttackCityMonsterDialog();
                    AddAttackCityMonsterFootShadow();
                    SetAttackCityMonsterMoveStates();

                });
        }

        //添加名称，title和点击效果
        private void AddAttackCityMonsterFunction()
        {
            _geActor.AddNpcInteraction(_townData.NpcID, _townData.GUID);

            float defaultNamePosY = 0.0f;
            if (_npcItem.NameLocalPosY > 0)
                defaultNamePosY = (float)_npcItem.NameLocalPosY / 1000.0f;

            _geActor.AddNPCFunction(_townData.Name, _townData.NameColor, 0, null, defaultNamePosY);
        }

        //添加对话框
        private void AddAttackCityMonsterDialog()
        {
#if !LOGIC_SERVER
            _geActor.AddComponentDialog(_townData.NpcID, NpcDialogComponent.IdBelong2.IdBelong2_NpcTable);

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

            _dialogComponentController.InitController(_townData.NpcID);


            //调整怪物冒泡的位置，在名字位置上面0.5的位置
            //以前的位置有问题
            var parentRoot = _geActor.NpcDialogComponent.transform.parent;
            if (parentRoot == null)
                return;

            if (_npcItem.NameLocalPosY > 0)
            {
                var parentRootLocalPos = parentRoot.transform.localPosition;
                parentRoot.transform.localPosition = new Vector3(
                    parentRootLocalPos.x,
                    (float)_npcItem.NameLocalPosY / 1000.0f + 0.5f,
                    parentRootLocalPos.z);

                var dialogComponentLocalPos = _dialogComponentController.transform.localPosition;
                _dialogComponentController.transform.localPosition = new Vector3(
                    0,
                    0,
                    dialogComponentLocalPos.z);
            }
#endif
        }

        //添加脚底阴影
        private void AddAttackCityMonsterFootShadow()
        {
            _geActor.CreateFootIndicator(FootSfxPath);
        }

        //设置状态
        private void SetAttackCityMonsterMoveStates()
        {
            InitAttackCityMonsterMoveStates();

            //获得Idle01的时间间隔
            _animTownIdle01Interval = _geActor.GetActionTimeLen(AnimTownIdle01Str);
        }

        public override void Update(float deltaTime)
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
        #endregion

        #region MoveState
        //初始化攻城怪物的移动状态，分为：不移动，左右移动，右左移动
        private void InitAttackCityMonsterMoveStates()
        {
            if (_townData == null)
                return;

            //非怪物攻城
            if (_townData.TownNpcType != ESceneActorType.AttackCityMonster)
                return;


            //设置移动速度和阴影
            _moveSpeed = new Vector3(1.0f, 0f, 0f);
            _geActor.AddSimpleShadow(Vector3.one);

            _eTownNpcMoveState = ETownAttackCityMonsterMoveState.BornIdle;
            _idleTime = UnityEngine.Random.Range(4f, 8f); ;
            _bornPosition = _townData.MoveData.Position;

            SetNpcMoveTarget();
        }

        private void SetNpcMoveTarget()
        {
            var index = UnityEngine.Random.Range(1, 7);
            if (index <= 3)
            {
                // 1/2的概率向左右来回移动, 设置初始的目标点（出生点），最左边位置
                _targetPosition = _bornPosition + new Vector3(-1 * UnityEngine.Random.Range(1.1f, 1.3f), 0, 0);
                _targetOffsetPosition = _targetPosition - _bornPosition;
            }
            else
            {
                // 1/2的概率向右左来回移动, 设置初始的目标点（出生点），最右边的位置
                _targetPosition = _bornPosition + new Vector3(1 * UnityEngine.Random.Range(1.1f, 1.3f), 0, 0);
                _targetOffsetPosition = _targetPosition - _bornPosition;
            }

        }

        public override void UpdateMove(float timeElapsed)
        {
            if (_eTownNpcMoveState == ETownAttackCityMonsterMoveState.Invalid)
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
            if (_eTownNpcMoveState == ETownAttackCityMonsterMoveState.Invalid)
                return;

            switch (_eTownNpcMoveState)
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
            if (_idleTime > 0)
            {
                _idleTime -= timeElapsed;
                return;
            }

            SetNextStateOfBornIdle();
        }

        private void SetNextStateOfBornIdle()
        {

            var index = UnityEngine.Random.Range(0, 10);

            //townIdle01动画存在，并且概率为20%，执行特殊的Idle；其他情况，执行walk
            if (_animTownIdle01Interval > 0 && index >= 6)
            {
                _eTownNpcMoveState = ETownAttackCityMonsterMoveState.TownIdle01;
                _townIdleO1Time = _animTownIdle01Interval;
                SetAnimationAction(AnimTownIdle01Str);
            }
            else
            {
                _eTownNpcMoveState = ETownAttackCityMonsterMoveState.ForwardMove;
                if (_targetOffsetPosition.x < 0)
                {
                    _townData.MoveData.FaceRight = false;
                }
                else
                {
                    _townData.MoveData.FaceRight = true;
                }

                SetAnimationAction(AnimTownWalkStr);
            }
        }

        //前进
        private void UpdateForwardMoveState(float timeElapsed)
        {
            if (_CheckPosEqual(_targetPosition, _townData.MoveData.Position) == true
                || ((_targetOffsetPosition.x < 0 && (_townData.MoveData.Position.x < _targetPosition.x)))
                || ((_targetOffsetPosition.x > 0 && (_townData.MoveData.Position.x > _targetPosition.x))))
            {
                //移动结束
                _townData.MoveData.Position = _targetPosition;
            }
            else
            {
                //移动中，
                _townData.MoveData.Position =
                    _townData.MoveData.Position + (_targetOffsetPosition.x < 0 ? -1 : 1) * timeElapsed * _moveSpeed;
                return;
            }


            //当前状态结束，设置新的状态
            _eTownNpcMoveState = ETownAttackCityMonsterMoveState.MoveIdle;
            //设置下一个状态Idle时间     
            _idleTime = UnityEngine.Random.Range(4f, 8f);
            //设置人物动画状态（idle状态）
            SetAnimationAction(AnimTownIdleStr);
        }

        //移动到一定距离的Idle
        private void UpdateMoveIdleState(float timeElapsed)
        {
            if (_idleTime > 0)
            {
                _idleTime -= timeElapsed;
                return;
            }

            //当前状态结束，设置新的状态
            _eTownNpcMoveState = ETownAttackCityMonsterMoveState.BackMove;
            //人物的朝向
            _townData.MoveData.FaceRight = !_townData.MoveData.FaceRight;
            //设置人物的动画状态,(walk状态)
            SetAnimationAction(AnimTownWalkStr);

        }

        //后退
        private void UpdateBackMoveState(float timeElapsed)
        {

            if (_CheckPosEqual(_bornPosition, _townData.MoveData.Position) == true
                || ((_targetOffsetPosition.x < 0 && (_townData.MoveData.Position.x > _bornPosition.x)))
                || ((_targetOffsetPosition.x > 0 && (_townData.MoveData.Position.x < _bornPosition.x))))
            {
                //移动结束
                _townData.MoveData.Position = _bornPosition;
            }
            else
            {
                //移动中，
                _townData.MoveData.Position =
                    _townData.MoveData.Position + (_targetOffsetPosition.x < 0 ? 1 : -1) * timeElapsed * _moveSpeed;
                return;
            }

            //当前状态结束，设置新的状态
            _eTownNpcMoveState = ETownAttackCityMonsterMoveState.BornIdle;
            //设置Idle的时间
            _idleTime = UnityEngine.Random.Range(4f, 8f);
            //设置人物的动画状态（idle状态）
            SetAnimationAction(AnimTownIdleStr);
            //重置AI
            SetNpcMoveTarget();
        }

        //特殊的Idle
        private void UpdateTownIdle01State(float timeElapsed)
        {
            if (_townIdleO1Time > 0.0f)
            {
                _townIdleO1Time -= timeElapsed;
                return;
            }

            _eTownNpcMoveState = ETownAttackCityMonsterMoveState.BornIdle;
            _idleTime = UnityEngine.Random.Range(4f, 8f);
            SetAnimationAction(AnimTownIdleStr);
        }

        

        //设置攻城怪物的自身动画，Anim_Town_Walk, Anim_Town_Idle
        private void SetAnimationAction(string animationAction)
        {
            _townData.ActionData.ActionName = animationAction;
        }

        #endregion

        public sealed override void Dispose()
        {
            base.Dispose();
            _townData = null;
            _npcItem = null;
            _eTownNpcMoveState = ETownAttackCityMonsterMoveState.Invalid;
            _dialogComponentController = null;
        }

    }
}