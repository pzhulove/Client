using ProtoTable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameClient
{
    public class ComActiveAwakeSkillSlot : MonoBehaviour
    {
        [SerializeField]
        private Image mIconImg;
        [SerializeField]
        private Image mLockImg;
        [SerializeField]
        private Text mLockTxt;

        private void Start()
        {
            //UpdateGray();
            //mDragMe.ResponseDrag += DealDrag;
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUpdatePassiveSkillGray, UpdateGray);
        }

        private void UpdateGray(UIEvent uiEvent)
        {
            UpdateGray();
        }

        private void UpdateGray()
        {
//             if (SkillFrame.frameParam.frameType == SkillFrameType.Normal && SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVE)
//             {
//                 if (SkillDataManager.GetInstance().CurPVESKillPage == ESkillPage.Page1)
//                 {
//                     gray.enabled = SkillDataManager.GetInstance().ActiveAwakeSkillIsGray;
//                 }
//                 else if (SkillDataManager.GetInstance().CurPVESKillPage == ESkillPage.Page2)
//                 {
//                     gray.enabled = SkillDataManager.GetInstance().ActiveAwakeSkillIsGray2;
//                 }
//             }
        }

        private void OnDestroy()
        {
            //mDragMe.ResponseDrag -= DealDrag;
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUpdatePassiveSkillGray, UpdateGray);
        }

        public void Show()
        {
            if(!(SkillFrame.frameParam.frameType == SkillFrameType.Normal && SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVE))
            {
                gameObject.CustomActive(false);
                return;
            }

            SkillTable skillData = SkillDataManager.GetInstance().GetActiveAwakeSkillData();
            if(skillData!=null)
            {
                gameObject.CustomActive(true);
                if (PlayerBaseData.GetInstance().AwakeState <= 0)//没有觉醒
                {
                    mLockImg.CustomActive(true);
                    mLockTxt.CustomActive(true);
                    mIconImg.CustomActive(false);
                    mLockTxt.SafeSetText(TR.Value("skill_passiveAwake_skill_des"));
                }
                else
                {
                    mLockImg.CustomActive(false);
                    mLockTxt.CustomActive(false);
                    mIconImg.CustomActive(true);
                    if (mIconImg != null)
                    {
                        ETCImageLoader.LoadSprite(ref mIconImg, skillData.Icon);
                    }
                }
            }
            else
            {
                gameObject.CustomActive(false);
            }
        }
      
        private bool DealDrag(PointerEventData eventData, bool bIsDrag)
        {
            if(bIsDrag)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("skill_drag_passiveAwake_skill_tip"));
            }

            return false;
        }
    }
}

