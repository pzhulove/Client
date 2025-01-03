using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using DG.Tweening;

namespace GameClient
{
    class EquipForgeResultFrame : ClientFrame
    {
        enum EState
        {
            Invalid,
            Forgeing,
            Failed,
            Successed,
        }

        [UIControl("State/Forging")]
        Toggle m_toggleForging;

        [UIControl("State/Failed")]
        Toggle m_toggleFailed;

        [UIControl("State/Successed")]
        Toggle m_toggleSuccessed;

        [UIControl("State")]
        ToggleGroup m_toggleGroup;

        [UIControl("State/Forging/Group/Progress")]
        Slider m_progress;

        [UIControl("State/Failed/Group/Reason")]
        Text m_labFailReason;

        [UIObject("State/Successed/Group/EquipRoot")]
        GameObject m_objEquipRoot;

        [UIObject("Effect/Foring/Image_CZ")]
        GameObject m_objForingEffect;

        [UIControl("State/Successed/Group/EquipName")]
        Text m_labEquipName;

        [UIControl("State/Successed/Group/GotTip")]
        Text m_labGotTip;

        EState m_resultState = EState.Invalid;
        int m_nEquipID = 0;
        uint m_uFailedCode = 0;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/EquipForge/EquipForgeResult";
        }

        protected override void _OnOpenFrame()
        {
            m_nEquipID = (int)userData;
            _InitUI();
            _RegisterUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            _ClearUI();
            _UnRegisterUIEvent();
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EquipForgeSuccess, _OnEquipForgeSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EquipForgeFail, _OnEquipForgeFail);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EquipForgeSuccess, _OnEquipForgeSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EquipForgeFail, _OnEquipForgeFail);
        }

        void _InitUI()
        {
            m_toggleGroup.SetAllTogglesOff();
            m_toggleForging.onValueChanged.RemoveAllListeners();
            m_toggleForging.onValueChanged.AddListener(var =>
            {
                if (var)
                {
                    _OnEnterForingState();
                }
                else
                {
                    _OnLeaveForingState();
                }
            });
            m_toggleFailed.onValueChanged.RemoveAllListeners();
            m_toggleFailed.onValueChanged.AddListener(var =>
            {
                if (var)
                {
                    _OnEnterFailedState();
                }
                else
                {
                    _OnLeaveFailedState();
                }
            });
            m_toggleSuccessed.onValueChanged.RemoveAllListeners();
            m_toggleSuccessed.onValueChanged.AddListener(var =>
            {
                if (var)
                {
                    _OnEnterSuccessedState();
                }
                else
                {
                    _OnLeaveSuccessedState();
                }
            });

            _SetCurrentState(EState.Forgeing);
        }

        void _ClearUI()
        {
            m_resultState = EState.Invalid;
            m_nEquipID = 0;
            m_uFailedCode = 0;
        }

        void _SetCurrentState(EState a_state)
        {
            if (a_state == EState.Forgeing)
            {
                m_toggleForging.isOn = false;
                m_toggleForging.isOn = true;
            }
            else if (a_state == EState.Successed)
            {
                m_toggleSuccessed.isOn = false;
                m_toggleSuccessed.isOn = true;
				AudioManager.instance.PlaySound(12);
            }
            else if (a_state == EState.Failed)
            {
                m_toggleFailed.isOn = false;
                m_toggleFailed.isOn = true;
				AudioManager.instance.PlaySound(11);
            }
        }

        void _OnEnterForingState()
        {
            m_progress.StartCoroutine(_StartForing());
        }

        void _OnLeaveForingState()
        {
            
        }

        void _OnEnterFailedState()
        {
            m_labFailReason.text = TableManager.GetInstance().GetTableItem<ProtoTable.CommonTipsDesc>((int)m_uFailedCode).Descs;

        }

        void _OnLeaveFailedState()
        {

        }

        void _OnEnterSuccessedState()
        {
            ItemData equip = ItemDataManager.CreateItemDataFromTable(m_nEquipID);

            ComItem comItem = m_objEquipRoot.GetComponentInChildren<ComItem>();
            if (comItem == null)
            {
                comItem = CreateComItem(m_objEquipRoot);
            }
            comItem.Setup(equip, (var1, var2) =>
            {
                ItemTipManager.GetInstance().ShowTip(var2);
            });

            m_labEquipName.text = equip.GetColorName();
            m_labGotTip.text = TR.Value("equipforge_got_tip", equip.GetColorName());
        }

        void _OnLeaveSuccessedState()
        {

        }

        IEnumerator _StartForing()
        {
            m_progress.value = 0.0f;

            float fTime = 2.1f;
            float fRate = 1.0f / fTime;
            while (fTime > 0.0f)
            {
                yield return Yielders.EndOfFrame;
                fTime -= Time.deltaTime;
                if (fTime < 0.0f)
                {
                    fTime = 0.0f;
                }
                m_progress.value = 1.0f - fTime * fRate;
            }

            m_progress.value = 1.0f;

            EquipForgeDataManager.GetInstance().ForgeEquip(m_nEquipID);

            while (m_resultState == EState.Invalid)
            {
                yield return Yielders.EndOfFrame;
            }

            bool bAnimCompleted = false;
            DOTweenAnimation anim = m_objForingEffect.GetComponent<DOTweenAnimation>();
            anim.onStepComplete.RemoveAllListeners();
            anim.onStepComplete.AddListener(() =>
            {
                bAnimCompleted = true;
                anim.DOPause();
            });

            while (bAnimCompleted == false)
            {
                yield return Yielders.EndOfFrame;
            }

            _SetCurrentState(m_resultState);
        }

        void _OnEquipForgeSuccess(UIEvent a_event)
        {
            m_nEquipID = (int)a_event.Param1;
            m_resultState = EState.Successed;
        }

        void _OnEquipForgeFail(UIEvent a_event)
        {
            m_uFailedCode = (uint)a_event.Param1;
            m_resultState = EState.Failed;
        }

        [UIEventHandle("State/Forging/Group/Func")]
        void _OnCancelForgingClicked()
        {
            m_progress.StopAllCoroutines();
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("State/Failed/Group/Func")]
        void _OnFailedReturnClicked()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("State/Successed/Group/Func")]
        void _OnSuccessedReturnClicked()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
