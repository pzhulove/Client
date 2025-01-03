using UnityEngine.UI;
using UnityEngine;
using GameClient;
using ProtoTable;
using Protocol;
using System;
using System.Collections.Generic;
using Network;
using UnityEngine.EventSystems;

class ComSkillSlotUnit : MonoBehaviour
{
    public Drag_Me dragMe;
    public Drop_Me dropMe;
    public StateController stateControl;
    public Image icon;
    public Image plus;
    public Image Lock;
    public Text OpenLv;

    void Start()
    {
        _BindUIEvent();
        _Init();     
    }

    void OnDestroy()
    {
        _UnBindUIEvent();

        if (dragMe != null)
        {
            dragMe.ResponseDrag = null;
        }

        if (dropMe != null)
        {
            dropMe.ResponseDrop = null;
        }
    }

    private void _BindUIEvent()
    {
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SkillBarChanged, _OnSkillSlotChanged);
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSelectSkillPage, _OnSkillSlotChanged);
    }

    private void _UnBindUIEvent()
    {
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SkillBarChanged, _OnSkillSlotChanged);
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSelectSkillPage, _OnSkillSlotChanged);
    }

    private void _OnSkillSlotChanged(UIEvent iEvent)
    {
        if(dragMe == null)
        {
            Logger.LogError("dragMe is null in [_OnSkillSlotChanged]");
            return;
        }

        if(dragMe.DragGroup != EDragGroup.SkillConfigGroup)
        {
            return;
        }

        if(dragMe.GroupIndex <= 0)
        {
            return;
        }

        _RefreshState();
    }

    private void _Init()
    {
        if (dragMe == null)
        {
            Logger.LogError("dragMe is null in [ComSkillSlotUnit]");
            return;
        }

        if (dropMe == null)
        {
            Logger.LogErrorFormat("dropMe is null in [ComSkillSlotUnit], id = {0}, GroupIndex = {1}", dragMe.id, dragMe.GroupIndex);
            return;
        }

        if(stateControl == null)
        {
            Logger.LogErrorFormat("stateControl is null in [ComSkillSlotUnit], id = {0}, GroupIndex = {1}", dragMe.id, dragMe.GroupIndex);
            return;
        }

        dragMe.ResponseDrag = _DealDrag;
        dropMe.ResponseDrop = _DealDrop;

        if (dragMe.GroupIndex > 0)
        {
            _RefreshState();
        }
    }

    private void _RefreshState()
    {
        SkillBarGrid slotinfo = SkillDataManager.GetInstance().GetCurSlotInfoBySlot(dragMe.GroupIndex);

        if (dragMe.GroupIndex <= SkillConfigurationFrame.UnLockSkillSlotNum)
        {
            if (slotinfo != null && slotinfo.id > 0)
            {
                dragMe.id = slotinfo.id;

                _UpdateIcon();
                stateControl.Key = "normal";
            }
            else
            {
                dragMe.id = 0;
                stateControl.Key = "unlock";
            }
        }
        else
        {
            dragMe.id = 0;
            stateControl.Key = "lock";
            //如果是最后一个被锁了
            if (dragMe.GroupIndex == Utility.GetClientIntValue(ClientConstValueTable.eKey.SKILL_CONFIG_SET_COUNT, 12))
            {
                OpenLv.SafeSetText(TR.Value("skill_task_skill_des"));
            }
            //其他都是等级没到
            else
            {
                OpenLv.SafeSetText(TR.Value("skill_config_set_lock_lv_tip", TableManager.GetInstance().GetLevelBySkillBarIndex(dragMe.GroupIndex + 1)));
            }
            if(slotinfo != null && slotinfo.id > 0)
            {
                SystemNotifyManager.SysNotifyMsgBoxOK(string.Format("技能槽位越界.Index = {0}, skillID = {1}, 技能配置解锁个数 = {2}.", dragMe.GroupIndex, slotinfo.id, SkillConfigurationFrame.UnLockSkillSlotNum));
            } 
        }
    }

    private void _UpdateIcon()
    {
        if (icon != null)
        {
            SkillTable table = TableManager.GetInstance().GetTableItem<SkillTable>(dragMe.id);
            if(table != null)
            {
                ETCImageLoader.LoadSprite(ref icon, table.Icon);
            }
        }
    }

    private bool _DealDrag(PointerEventData DragData)
    {
        GameObject DragObj = DragData.pointerPress;
        if(DragObj == null)
        {
            return false;
        }

        Drag_Me dragme = DragObj.GetComponent<Drag_Me>();
        if (dragme == null)
        {
            Logger.LogError("[SkillSlotUnit] DragMe is null,检查预制体");
            return false;
        }

        if (dragme.DragGroup != EDragGroup.SkillConfigGroup)
        {
            Logger.LogErrorFormat("[SkillSlotUnit]DragGroup类型配置错误,DragGroup = {0},请在预制体上修改", dragme.DragGroup);
            return false;
        }

        SkillBarGrid slotinfo = SkillDataManager.GetInstance().GetCurSlotInfoBySlot(dragme.GroupIndex);

        if (slotinfo == null || (slotinfo != null && slotinfo.id <= 0))
        {
            return false;
        }

        return SkillDataManager.GetInstance().CheckDrag(slotinfo.id);
    }

    private bool _DealDrop(PointerEventData DragData, GameObject ReceiveImgObj)
    {
        GameObject DragObj = DragData.pointerDrag;

        if(DragObj == null || ReceiveImgObj == null)
        {
            return false;
        }

        // 拖拽的来源点
        Drag_Me mDrag = DragObj.GetComponent<Drag_Me>();
        if (mDrag == null)
        {
            return false;
        }

        // 拖拽的目标点
        Drop_Me mReceive = ReceiveImgObj.GetComponent<Drop_Me>();
        if (mReceive == null)
        {
            return false;
        }

        // 有Drop_Me的地方不一定有Drag_Me,但是SkillSlotUnit上是一定要有Drag_Me的，所以还要判断下Drag_Me存不存在
        Drag_Me mReceiveDragMe = ReceiveImgObj.GetComponent<Drag_Me>();
        if (mReceiveDragMe == null)
        {
            return false;
        }

        int iDragSkillid = -1;
        if (mDrag.DragGroup == EDragGroup.SkillTreeGroup)
        {
            iDragSkillid = mDrag.id;
        }
        else if (mDrag.DragGroup == EDragGroup.SkillConfigGroup)
        {
            SkillBarGrid slotinfo = SkillDataManager.GetInstance().GetCurSlotInfoBySlot(mDrag.GroupIndex);

            if (slotinfo == null || (slotinfo != null && slotinfo.id <= 0))
            {
                // 空槽位禁止拖拽
                return false;
            }
            else
            {
                // 自己拖拽放到自己位置上
                if(slotinfo.slot == mReceiveDragMe.GroupIndex)
                {
                    return false;
                }

                iDragSkillid = slotinfo.id;
            }
        }
        else
        {
            return false;
        }

        if (iDragSkillid <= 0)
        {
            return false;
        }

        if (!SkillDataManager.GetInstance().CheckDrop(iDragSkillid, mDrag, mReceive, mReceiveDragMe))
        {
            return false;
        }

        // 发送协议
        SkillBarGrid targetSlotNewSkill = new SkillBarGrid();

        targetSlotNewSkill.slot = (byte)mReceiveDragMe.GroupIndex;
        targetSlotNewSkill.id = (UInt16)iDragSkillid;

        return SkillDataManager.GetInstance().SendChangeSlotSkillReq(targetSlotNewSkill);
    }
}