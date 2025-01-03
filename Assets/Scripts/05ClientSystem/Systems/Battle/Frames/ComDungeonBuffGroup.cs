using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace GameClient
{


    public class ComDungeonBuffGroup : MonoBehaviour
    {
        private enum BuffGroupType
        {
            LEFT = 0,
            Right = 1,
        }

        [SerializeField]
        private GameObject mBuffContent = null;
        [SerializeField]
        private Button mBuffTipBtn = null;
        [SerializeField]
        private BuffGroupType mType;

        private float mCountTime = 0.0f;
        void Awake()
        {
            _bindExUI();
            _bindEvents();
        }

        void Update()
        {
            mCountTime += Time.deltaTime;

            if (mCountTime > 0.2f)
            {
                UpdateBattleBuff(mCountTime);
                mCountTime = 0;
            }
        }

        void OnDestroy()
        {
            _unBindExUI();
            _unBindEvents();
            ClearEventHandle();
        }

        protected void _bindExUI()
        {
            if (null != mBuffTipBtn)
            {
                mBuffTipBtn.onClick.AddListener(_onBuffTipBtnButtonClick);
            }
        }

        protected void _unBindExUI()
        {
            mBuffContent = null;
            if (null != mBuffTipBtn)
            {
                mBuffTipBtn.onClick.RemoveListener(_onBuffTipBtnButtonClick);
            }
            mBuffTipBtn = null;
        }

        private void _bindEvents()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BattleBuffAdded, _addBuffEvent);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BattleBuffCancel, _removeBuffEvent);
        }

        private void _unBindEvents()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BattleBuffAdded, _addBuffEvent);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BattleBuffCancel, _removeBuffEvent);
        }



        private void _onBuffTipBtnButtonClick()
        {
            OpenBattleTips();
        }

        #region Buff相关
        private DungeonBuffDisplayFrame buffDisplayFrame = new DungeonBuffDisplayFrame();
        //private BeActor buffActor;
        private byte buffActorSeat = byte.MaxValue;
        private int buffActorPid = 0;

        private IBeEventHandle onBuffStartEventHandle;
        private IBeEventHandle onBuffCancelEventHandle;
        private void ClearEventHandle()
        {
            if(onBuffStartEventHandle != null)
            {
                onBuffStartEventHandle.Remove();
                onBuffStartEventHandle = null;
            }
            if(onBuffCancelEventHandle != null)
            {
                onBuffCancelEventHandle.Remove();
                onBuffCancelEventHandle = null;
            }
        }
        public void InitBattleBuff(BeActor actor,byte _seat)
        {
            if (actor == null)
                return;
            buffDisplayFrame.Init(mBuffContent, _seat);

            
            buffActorPid = actor.GetPID();

            onBuffStartEventHandle = actor.RegisterEventNew(BeEventType.onAddBuff, (args) =>
            {
                BeBuff tempBuff = args.m_Obj as BeBuff;
                if(tempBuff != null)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BattleBuffAdded, tempBuff, buffActorPid);
                }
                
            });

            onBuffCancelEventHandle = actor.RegisterEventNew(BeEventType.onRemoveBuff, (args) =>
            {
                int buffId = (int)args.m_Int;
                BeBuff tempBuff = args.m_Obj as BeBuff;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BattleBuffCancel, tempBuff, buffActorPid);
            });

            //对界面初始化之前的buff做处理
            var buffList = actor.buffController.GetBuffList();
            for (int i = 0; i < buffList.Count; ++i)
            {
                if (buffDisplayFrame.IsInited)
                {
                    buffDisplayFrame._addBuff(buffList[i]);
                }
            }
        }

        private void _addBuffEvent(UIEvent ui)
        {
            BeBuff buff = ui.Param1 as BeBuff;
            int actorPid = (int)ui.Param2;
            int buffID = ui.Param3 == null ? 0 : (int)ui.Param3;
            if (actorPid != buffActorPid)
            {
                return;
            }
            if (buffDisplayFrame.IsInited)
            {
                buffDisplayFrame._addBuff(buff, buffID);
            }
        }

        private void _removeBuffEvent(UIEvent ui)
        {
            BeBuff buff = ui.Param1 as BeBuff;
            int actorPid = (int)ui.Param2;
            int buffID = ui.Param3 == null ? 0 : (int)ui.Param3;
            if (actorPid != buffActorPid)
            {
                return;
            }
            if (buffDisplayFrame.IsInited)
            {
                buffDisplayFrame._removeBuff(buff, buffID);
            }
        }

        private void UpdateBattleBuff(float timeElapsed)
        {
            if (buffDisplayFrame.IsInited)
            {
                buffDisplayFrame._updateBuff(timeElapsed);
            }
        }

        private void OpenBattleTips()
        {
            if (ClientSystemManager.instance.IsFrameOpen<BattleBuffTipsFrame>())
            {
                ClientSystemManager.instance.CloseFrame<BattleBuffTipsFrame>();
            }
            else if(buffDisplayFrame.GetValidBuffCount() > 0)
            {
                var pos = mBuffTipBtn.transform.position;
                pos.x -= 39;
                if(mType == BuffGroupType.Right)
                {
                    pos.x = pos.x - 225 + 78;
                }
                buffDisplayFrame.SetBuffTipListUpdate();
                ClientSystemManager.instance.OpenFrame<BattleBuffTipsFrame>(FrameLayer.Bottom, new object[] { buffDisplayFrame , pos });
            }
        }
        #endregion
    }
}
