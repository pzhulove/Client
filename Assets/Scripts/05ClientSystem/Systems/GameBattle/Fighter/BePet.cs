using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace GameClient
{
    public sealed class BePetData : BeBaseActorData
    {
        public int PetID { get; set; }
        public int FollowId { get; set; }
    }


    public sealed class BePet : BeBaseFighter
    {
        BeFighter m_owner = null;
        Vector3 m_vecPosOffset = new Vector3(1, 0, 0);
        Vector3 m_vecCoefficient = new Vector3(3, 1, 3);
        float m_fRunDist = 1f;

        float m_fa = -5.0f;
        Vector3 m_vecMinSpeed = new Vector3(1.5f, 0.0f, 1.5f);
        float m_fSpecialIdleRemainTime = 100.0f;
        bool m_bPlaySpecialIdle = false;

        bool m_bDialogEnable = false;
        float m_fRemainTime = 5.0f;
        ProtoTable.PetTable m_petTable = null;
        protected ProtoTable.UnitTable followData = null;
        GameObject m_objDialogRoot = null;
        Text m_labDialog = null;

        RectTransform m_rectDialogParent = null;
        RectTransform m_rectDialog = null;
        GeCamera m_cameraScene = null;
        Camera m_cameraUI = null;
        Vector2 m_vecDialogOffset = Vector2.zero;

        protected IBeTownActionPlay _townPetActionPlay = null;

        public BePet(BePetData data, ClientSystemGameBattle systemTown)
            : base(data, systemTown)
        {
            data.MoveData.MoveSpeed = Vector3.zero;

            BePetData townData = ActorData as BePetData;
            m_petTable = TableManager.GetInstance().GetTableItem<ProtoTable.PetTable>(townData.PetID);
            if (m_petTable == null)
            {
                if (townData.FollowId == 0)
                {
                    Logger.LogErrorFormat("宠物表 没有ID为 {0} 的条目", townData.PetID);
                }
                else
                {
                    followData = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(townData.FollowId);
                    if (followData == null)
                    {
                        Logger.LogErrorFormat("怪物表 没有ID为 {0} 的条目", townData.FollowId);
                    }
                }
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PlayActiveFeedPetAction, _OnFeedSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FollowPetSatietyChanged, _OnSatietyChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TownSceneInited, _OnSceneChanged);

            m_fRemainTime = Global.Settings.petDialogShowInterval;
            m_fSpecialIdleRemainTime = Global.Settings.petSpecialIdleInterval;

            _ResetIdleAction();
        }

        public override void Dispose()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayActiveFeedPetAction, _OnFeedSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FollowPetSatietyChanged, _OnSatietyChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TownSceneInited, _OnSceneChanged);

            if (m_objDialogRoot != null)
            {
                GameObject.Destroy(m_objDialogRoot);
                m_objDialogRoot = null;
            }

            base.Dispose();
            if (_townPetActionPlay != null)
            {
                _townPetActionPlay.DeInit();
                _townPetActionPlay = null;
            }
        }

        public void SetDialogEnable(bool a_bDialogEnable)
        {
            m_bDialogEnable = a_bDialogEnable;
            if (m_bDialogEnable == false)
            {
                _HideDialog();
            }
            else
            {
                _HideDialog();
                m_fRemainTime = Global.Settings.petDialogShowInterval;
            }
        }

        public void SetOwner(BeFighter a_owner)
        {
            m_owner = a_owner;

            if (m_owner != null && m_owner.IsValid())
            {
                ActorMoveData ownerMoveData = m_owner.ActorData.MoveData;
                Vector3 vecTargetPos;
                if (ownerMoveData.FaceRight)
                {
                    vecTargetPos = ownerMoveData.Position - m_vecPosOffset;
                }
                else
                {
                    vecTargetPos = ownerMoveData.Position + m_vecPosOffset;
                }

                //vecTargetPos += new Vector3(UnityEngine.Random.Range(0.0f, 1.0f), 0.0f, UnityEngine.Random.Range(0.0f, 1.0f));

                ActorData.MoveData.Position = vecTargetPos;

                ActorData.MoveData.FaceRight = ownerMoveData.FaceRight;
            }
        }

        public override void Update(float timeElapsed)
        {
            if (m_petTable != null && m_bDialogEnable && m_objDialogRoot != null)
            {
                if (m_objDialogRoot.activeSelf == false)
                {
                    m_fRemainTime -= timeElapsed;
                    if (m_fRemainTime <= 0.0f)
                    {
                        if (_IsHungry())
                        {
                            _ShowDialog(m_petTable.HungryDialogID);
                        }
                        else
                        {
                            if (PlayerBaseData.GetInstance().Level > 20)
                            {
                                _ShowDialog(m_petTable.HighLevelDialogID);
                            }
                            else
                            {
                                _ShowDialog(m_petTable.LowLevelDialogID);
                            }
                        }
                        m_fRemainTime = Global.Settings.petDialogLife;
                    }
                }
                else
                {
                    m_fRemainTime -= timeElapsed;
                    if (m_fRemainTime <= 0.0f)
                    {
                        _HideDialog();
                        m_fRemainTime = Global.Settings.petDialogShowInterval;
                    }
                }
            }


            base.Update(timeElapsed);
        }

        public override void UpdateMove(float timeElapsed)
        {
            //TODO 这里最好做成handle机制
            if (m_owner != null && m_owner.IsValid())
            {
                ActorMoveData ownerMoveData = m_owner.ActorData.MoveData;
                ActorMoveData moveData = ActorData.MoveData;

                // 计算目标位置
                if (ownerMoveData.FaceRight)
                {
                    moveData.TargetPosition = ownerMoveData.Position - m_vecPosOffset;
                }
                else
                {
                    moveData.TargetPosition = ownerMoveData.Position + m_vecPosOffset;
                }

                // 检查是否需要移动
                if (_CheckPosEqual(moveData.TargetPosition, moveData.Position))
                {
                    if (moveData.MoveType != EActorMoveType.Invalid)
                    {
                        moveData.Position = moveData.TargetPosition;
                        moveData.TargetPosition = Vector3.zero;
                        moveData.MoveSpeed = Vector3.zero;
                        moveData.FaceRight = ownerMoveData.FaceRight;
                        moveData.MoveType = EActorMoveType.Invalid;

                        if (_IsHungry())
                        {
                            DoActionHungryIdle();
                        }
                        else
                        {
                            _ResetIdleAction();
                        }
                    }
                    else
                    {
                        if (_IsHungry())
                        {
                            DoActionHungryIdle();
                        }
                        else
                        {
                            _UpdateIdleAction(timeElapsed);
                        }
                    }
                    return;
                }
                else
                {
                    moveData.MoveType = EActorMoveType.TargetPos;
                }

                // 修正朝向
                Vector3 vecOffset = moveData.TargetPosition - moveData.Position;
                if (vecOffset.x > 0)
                {
                    moveData.FaceRight = true;
                }
                else if (vecOffset.x < 0)
                {
                    moveData.FaceRight = false;
                }

                if (ownerMoveData.MoveType == EActorMoveType.Invalid)
                {
                    float fv0 = Mathf.Sqrt(moveData.MoveSpeed.sqrMagnitude);
                    if (fv0 > 0)
                    {
                        float fv = fv0 + m_fa * timeElapsed;
                        float fRate = fv / fv0;
                        moveData.MoveSpeed = _GetSuitableSpeed(
                            moveData.MoveSpeed * fRate, m_vecMinSpeed, ownerMoveData.MoveSpeed);
                    }
                    else
                    {
                        moveData.MoveSpeed = _GetSuitableSpeed(moveData.MoveSpeed, m_vecMinSpeed, ownerMoveData.MoveSpeed);
                    }
                }
                else
                {
                    float fSpeedX = moveData.MoveSpeed.x + Mathf.Abs(vecOffset.x) * m_vecCoefficient.x * timeElapsed;
                    float fSpeedZ = moveData.MoveSpeed.z + Mathf.Abs(vecOffset.z) * m_vecCoefficient.z * timeElapsed;
                    moveData.MoveSpeed = _GetSuitableSpeed(new Vector3(fSpeedX, 0.0f, fSpeedZ), Vector3.zero, ownerMoveData.MoveSpeed);
                }

                // 修正动作
                if (vecOffset.sqrMagnitude > m_fRunDist * m_fRunDist)
                {
                    if (_IsHungry())
                    {
                        DoActionHungryRun();
                    }
                    else
                    {
                        DoActionRun();
                    }
                }
                else
                {
                    moveData.MoveSpeed = m_vecMinSpeed;
                    if (_IsHungry())
                    {
                        DoActionHungryWalk();
                    }
                    else
                    {
                        DoActionWalk();
                    }
                }

                // 执行移动
                {
                    // 更新位置
                    Vector3 offset = moveData.TargetPosition - moveData.Position;
                    Vector3 speed = offset.normalized;
                    speed.x *= moveData.MoveSpeed.x;
                    speed.y *= moveData.MoveSpeed.y;
                    speed.z *= moveData.MoveSpeed.z;
                    Vector3 newPosition = moveData.Position + speed * timeElapsed;

                    // 检测是否越过目标点，如果是，则表示已到达目标点
                    Vector3 offsetNew = moveData.TargetPosition - newPosition;
                    if (offset.x * offsetNew.x <= 0.0f)
                    {
                        newPosition.x = moveData.TargetPosition.x;
                    }

                    newPosition.y = 0.0f;

                    if (offset.z * offsetNew.z <= 0.0f)
                    {
                        newPosition.z = moveData.TargetPosition.z;
                    }

                    // 设置位置
                    moveData.Position = newPosition;
                }

            }
        }

        public override void InitGeActor(GeSceneEx geScene)
        {
            _geScene = geScene;
            if (geScene == null)
            {
                return;
            }

            if (_geActor == null)
            {
                if (m_petTable == null)
                {
                    return;
                }
                _geActor = geScene.CreateActorAsyncEx(m_petTable.ModeID, 0, 0, false, false, true, _PostLoad);
                _geActor.AddSimpleShadow(Vector3.one);

            }

            base.InitGeActor(geScene);

        }

        private void _PostLoad(GeActorEx pet)
        {
#if !LOGIC_SERVER
            if (m_petTable.UseNewFunction)
            {
                var resTable = TableManager.instance.GetTableItem<ProtoTable.ResTable>(m_petTable.ModeID);
                if (resTable != null && !string.IsNullOrEmpty(resTable.ActionConfigPath2) &&
                    !resTable.ActionConfigPath2.Equals("-"))
                {
                    _townPetActionPlay = new BeTownPetActionPlay();
                    _townPetActionPlay.Init(_geActor, resTable.ActionConfigPath2);
                } 
            }

            _InitGeDialog();
            DoActionSpecialIdle();
            m_fSpecialIdleRemainTime = 0.01f;
            UpdateMove(0);
#endif
        }

        public override void UpdateGeActor(float timeElapsed)
        {
            _UpdateGeDialog();
            if (_townPetActionPlay != null)
                _townPetActionPlay.Update(timeElapsed);
            base.UpdateGeActor(timeElapsed);
        }

        public override void DoActionIdle()
        {
            if (_townPetActionPlay != null && _townPetActionPlay.PlayAction(BePetActionSwitchHelper.GetActionNameByType(PetActionNameType.Idle)))
            {
                return;
            }
            ActorData.ActionData.ActionName = "Anim_Idle01";
            ActorData.ActionData.ActionSpeed = 1.0f;
            ActorData.ActionData.ActionLoop = true;
        }

        public void DoActionHungryIdle()
        {
            if (_townPetActionPlay != null && _townPetActionPlay.PlayAction(BePetActionSwitchHelper.GetActionNameByType(PetActionNameType.HungerIdle)))
            {
                return;
            }
            ActorData.ActionData.ActionName = "Anim_eIdle01";
            ActorData.ActionData.ActionSpeed = 1.0f;
        }

        public void DoActionSpecialIdle()
        {
            if (_townPetActionPlay != null && _townPetActionPlay.PlayAction(BePetActionSwitchHelper.GetActionNameByType(PetActionNameType.SpecialIdle)))
            {
                return;
            }
            ActorData.ActionData.ActionName = "Anim_Idle02";
            ActorData.ActionData.ActionSpeed = 1.0f;
        }

        public override void DoActionWalk()
        {
            if (_townPetActionPlay != null && _townPetActionPlay.PlayAction(BePetActionSwitchHelper.GetActionNameByType(PetActionNameType.Walk)))
            {
                return;
            }
            ActorData.ActionData.ActionName = "Anim_Walk";
            ActorData.ActionData.ActionSpeed = Global.Settings.townActionSpeed;
            ActorData.ActionData.isDirty = true;

        }

        public void DoActionHungryWalk()
        {
            if (_townPetActionPlay != null && _townPetActionPlay.PlayAction(BePetActionSwitchHelper.GetActionNameByType(PetActionNameType.HungerWalk)))
            {
                return;
            }
            ActorData.ActionData.ActionName = "Anim_eWalk";
            ActorData.ActionData.ActionSpeed = Global.Settings.townActionSpeed;
            ActorData.ActionData.isDirty = true;

        }

        public void DoActionRun()
        {
            ActorData.ActionData.ActionName = "Anim_Run";
            ActorData.ActionData.ActionSpeed = Global.Settings.townActionSpeed;
            ActorData.ActionData.isDirty = true;

        }

        public void DoActionHungryRun()
        {
            ActorData.ActionData.ActionName = "Anim_eRun";
            ActorData.ActionData.ActionSpeed = Global.Settings.townActionSpeed;
            ActorData.ActionData.isDirty = true;

        }



        Vector3 _GetSuitableSpeed(Vector3 a_vecSourceSpeed, Vector3 a_vecMinSpeed, Vector3 a_vecMaxSpeed)
        {
            if (a_vecSourceSpeed.x > a_vecMaxSpeed.x)
            {
                a_vecSourceSpeed.x = a_vecMaxSpeed.x;
            }
            if (a_vecSourceSpeed.x < a_vecMinSpeed.x)
            {
                a_vecSourceSpeed.x = a_vecMinSpeed.x;
            }

            if (a_vecSourceSpeed.z > a_vecMaxSpeed.z)
            {
                a_vecSourceSpeed.z = a_vecMaxSpeed.z;
            }
            if (a_vecSourceSpeed.z < a_vecMinSpeed.z)
            {
                a_vecSourceSpeed.z = a_vecMinSpeed.z;
            }

            return new Vector3(a_vecSourceSpeed.x, 0.0f, a_vecSourceSpeed.z);
        }

        void _InitGeDialog()
        {
#if !LOGIC_SERVER
            if (_geActor != null)
            {

                m_objDialogRoot = CGameObjectPool.instance.GetGameObject("UIFlatten/Prefabs/Pet/PetDialog", enResourceType.UIPrefab, (uint)GameObjectPoolFlag.ReserveLast);

                if (ClientSystemManager.GetInstance() != null)
                {
                    if (ClientSystemManager.GetInstance().SceneUILayer != null)
                    {
                        m_rectDialogParent = ClientSystemManager.GetInstance().SceneUILayer.GetComponent<RectTransform>();
                    }
                    m_cameraUI = ClientSystemManager.GetInstance().UICamera;
                }

                if (m_objDialogRoot != null)
                {
                    m_rectDialog = m_objDialogRoot.GetComponent<RectTransform>();
                }

                if (_geScene != null && _geScene.GetCamera() != null)
                {
                    m_cameraScene = _geScene.GetCamera();
                }

                if (m_rectDialog != null)
                {
                    m_vecDialogOffset = m_rectDialog.anchoredPosition;
                }

                if (m_rectDialogParent != null)
                {
                    m_rectDialog.SetParent(m_rectDialogParent, false);
                }

                if (m_objDialogRoot != null)
                {
                    m_labDialog = Utility.GetComponetInChild<Text>(m_objDialogRoot, "Text");
                    m_objDialogRoot.SetActive(false);
                }
            }
#endif
        }

        private void SetDialogLocalPosition()
        {
#if !LOGIC_SERVER
            Vector2 vecPos;
            Vector3 vecScreenPos = m_cameraScene.WorldToScreenPoint(ActorData.MoveData.GraphPosition + _geActor.GetOverHeadPosition());
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_rectDialogParent, vecScreenPos, m_cameraUI, out vecPos);
            if (ActorData.MoveData.FaceRight)
            {
                m_rectDialog.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                m_labDialog.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                m_rectDialog.localPosition = vecPos + new Vector2(-m_vecDialogOffset.x, m_vecDialogOffset.y) + new Vector2(0, m_petTable.PetDialogLocation);
            }
            else
            {
                m_rectDialog.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                m_labDialog.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                m_rectDialog.localPosition = vecPos + m_vecDialogOffset + new Vector2(0, m_petTable.PetDialogLocation);
            }
#endif
        }

        void _UpdateGeDialog()
        {
#if !LOGIC_SERVER
            if (_geActor != null && m_objDialogRoot != null)
            {
                if (ActorData.MoveData.TransformDirty == true)
                {
                    SetDialogLocalPosition();
                }
            }
#endif
        }

        void _ShowDialog(int a_nDialogBaseID)
        {
            if (m_objDialogRoot != null && m_labDialog != null)
            {
                ProtoTable.PetDialogBaseTable petDialogBase = TableManager.GetInstance().GetTableItem<ProtoTable.PetDialogBaseTable>(a_nDialogBaseID);
                Assert.IsTrue(petDialogBase != null);
                Assert.IsTrue(petDialogBase.DialogIDs.Count > 0);
                if (petDialogBase.FilterType == ProtoTable.PetDialogBaseTable.eFilterType.Invalid)
                {
                    ProtoTable.MonsterSpeech speechTable = TableManager.GetInstance().GetTableItem<ProtoTable.MonsterSpeech>(petDialogBase.DialogIDs[0]);
                    Assert.IsTrue(speechTable != null);
                    m_labDialog.text = speechTable.Speech;
                }
                else if (petDialogBase.FilterType == ProtoTable.PetDialogBaseTable.eFilterType.Random)
                {
                    int nIndex = UnityEngine.Random.Range(0, petDialogBase.DialogIDs.Count - 1);
                    ProtoTable.MonsterSpeech speechTable = TableManager.GetInstance().GetTableItem<ProtoTable.MonsterSpeech>(petDialogBase.DialogIDs[nIndex]);
                    Assert.IsTrue(speechTable != null);
                    m_labDialog.text = speechTable.Speech;
                }

                SetDialogLocalPosition();

                m_objDialogRoot.SetActive(true);
            }
        }

        void _HideDialog()
        {
            if (m_objDialogRoot != null)
            {
                m_objDialogRoot.SetActive(false);
            }
        }

        bool _IsHungry()
        {
            return PetDataManager.GetInstance().IsFollowPetHungry();
        }

        void _UpdateIdleAction(float timeElapsed)
        {
            if (m_bPlaySpecialIdle)
            {
                m_fSpecialIdleRemainTime -= timeElapsed;
                if (m_fSpecialIdleRemainTime <= 0.0f)
                {
                    _ResetIdleAction();
                }
            }
            else
            {
                m_fSpecialIdleRemainTime -= timeElapsed;
                if (m_fSpecialIdleRemainTime <= 0.0f)
                {
                    DoActionSpecialIdle();

                    if (null != _geActor)
                    {
                        float timeLen = _geActor.GetActionTimeLen("Anim_Idle02");
                        if (0.0f != timeLen)
                        {
                            m_fSpecialIdleRemainTime = timeLen;
                            m_bPlaySpecialIdle = true;
                        }
                    }
                }
            }
        }

        void _ResetIdleAction()
        {
            DoActionIdle();
            m_fSpecialIdleRemainTime = Global.Settings.petSpecialIdleInterval;
            m_bPlaySpecialIdle = false;
        }

        void _OnFeedSuccess(UIEvent a_event)
        {
            if (m_petTable != null && m_bDialogEnable)
            {
                _ShowDialog(m_petTable.FeedDialogID);
                m_fRemainTime = Global.Settings.petDialogLife;
            }
        }

        void _OnSatietyChanged(UIEvent a_event)
        {
            if (m_petTable != null && m_bDialogEnable)
            {
                if (_IsHungry())
                {
                    _ShowDialog(m_petTable.HungryDialogID);
                    m_fRemainTime = Global.Settings.petDialogLife;
                }
                else
                {

                }
            }
        }

        void _OnSceneChanged(UIEvent a_event)
        {
			if (_townPetActionPlay != null && _townPetActionPlay.PlayAction(BePetActionSwitchHelper.GetActionNameByType(PetActionNameType.LastAction)))
            {
                return;
            }
            ActorData.MoveData.TransformDirty = true;
            ActorData.ActionData.isDirty = true;
        }
    }
}
