using Protocol;
using UnityEngine;
using System.Collections.Generic;
using ProtoTable;
using UnityEngine.UI;
using DG.Tweening;

namespace GameClient
{
    public struct FloatEffectData
    {
        public string text;
        public int itemId;
    }   

    public class SystemNotifyManager
    {
        public static SystemNotifyManager GetInstance()
        {
            return hInstance;
        }

        public static void Clear()
        {
            CommonAnimInterval = 0f;
            CommonAnimList.Clear();
            CommonAnimInterval2 = 0f;
            CommonAnimList2.Clear();
            CommonAnimListcontent2.Clear();
            FloatingEffectInterval = 0f;
            FloatingEffectPosInterval = 0f;
            FloatingEffectListPos.Clear();
            dungeonSkillTip = null;

            for (int i = 0; i < FloatingImmediateEffectList.Count; i++)
            {
                //CGameObjectPool.instance.RecycleGameObject(FloatingImmediateEffectList[i]);
            }
            FloatingImmediateEffectList.Clear();

            for(int i = 0; i < FloatingImmediateEffectList2.Count; i++)
            {
                //CGameObjectPool.instance.RecycleGameObject(FloatingImmediateEffectList2[i]);
            }
            FloatingImmediateEffectList2.Clear();

            AwardNotifyCount = 0;
            FloatingEffectList.Clear();
        }

        // 数据来源于服务器
        public static void SysNotifyFromServer(SysNotify res)
        {
            if (res.type == (byte)CommonTipsDesc.eShowType.CT_SCROLL_LIGHT || res.word == "" || res.word == "-" || res.word.Length <= 0)
            {
                return;
            }

            if ((CommonTipsDesc.eShowType)res.type == CommonTipsDesc.eShowType.CT_TEXT_FLOAT_EFFECT)
            {
                SwitchNotifyType(res.word, CommonTipsDesc.eShowType.CT_TEXT_FLOAT_EFFECT, null, null, null, CommonTipsDesc.eShowMode.SI_QUEUE, 0, 0.0f, FrameLayer.High, false, null);
            }
            else
            {
                SwitchNotifyType(res.word, (CommonTipsDesc.eShowType)res.type, null, null, null, CommonTipsDesc.eShowMode.SI_IMMEDIATELY, 0, 0.0f, FrameLayer.High, false, null);
            }
        }

        // 数据来源于读表
        public static void SystemNotify(int NotifyID)
        {
#if !LOGIC_SERVER
 

            ComSysNotifyByTableID(NotifyID, null, null);

 #endif

        }

        public static void SystemNotifyOkCancel(int NotifyID, UnityEngine.Events.UnityAction OnOKCallBack, UnityEngine.Events.UnityAction OnCancelCallBack, FrameLayer layer = FrameLayer.High, bool bExclusive = false)
        {
            var NotifyData = TableManager.GetInstance().GetTableItem<CommonTipsDesc>(NotifyID);
            if (NotifyData == null)
            {
                SetCommonMsgBoxOK(string.Format("[通用提示表]没有添加错误码[{0}]对应的提示内容", NotifyID));
                return;
            }

            SwitchNotifyType(NotifyData.Descs, NotifyData.ShowType, NotifyData, OnOKCallBack, OnCancelCallBack, NotifyData.ShowMode, 0, 0.0f, layer, bExclusive, null);
        }

        public static void SystemNotify(int NotifyID, UnityEngine.Events.UnityAction OnOKCallBack, UnityEngine.Events.UnityAction OnCancelCallBack)
        {
            ComSysNotifyByTableID(NotifyID, OnOKCallBack, OnCancelCallBack);
        }

		public static void SystemNotify(int NotifyID, UnityEngine.Events.UnityAction OnOKCallBack)
		{
			ComSysNotifyByTableID(NotifyID, OnOKCallBack);
		}

        public static void SystemNotify(int NotifyID, params object[] args)
        {
            ComSysNotifyByTableID(NotifyID, null, null, 0, args);
        }

        public static void SystemNotify(int NotifyID, UnityEngine.Events.UnityAction OnOKCallBack, UnityEngine.Events.UnityAction OnCancelCallBack, params object[] args)
        {
            ComSysNotifyByTableID(NotifyID, OnOKCallBack, OnCancelCallBack, 0, args);
        }

        public static void SystemNotify(int NotifyID, UnityEngine.Events.UnityAction OnOKCallBack, UnityEngine.Events.UnityAction OnCancelCallBack, float fWaitTime, params object[] args)
        {
            ComSysNotifyByTableID(NotifyID, OnOKCallBack, OnCancelCallBack, fWaitTime, args);
        }

        public static void SystemNotify(int NotifyID, int itemTableId, params object[] args)
        {
            ComSysNotifyByTableID(NotifyID, null, null, itemTableId, args);
        }

        // 数据来源于调用者自定义
        public static void SysNotifyMsgBoxOK(string msgContent, UnityEngine.Events.UnityAction OnOKCallBack = null, string OkBtnText = "", bool bExclusive = false)
        {
            string FinalContent = SwitchNotifyDescription(msgContent, null);

            SetCommonMsgBoxOK(FinalContent, null, OnOKCallBack, OkBtnText, FrameLayer.High, bExclusive);
        }

        public static void SysNotifyMsgBoxOkCancel(string msgContent, UnityEngine.Events.UnityAction OnOKCallBack = null, UnityEngine.Events.UnityAction OnCancelCallBack = null, float fCountDownTime = 0.0f, bool bExclusive = false)
        {
            SwitchNotifyType(msgContent, CommonTipsDesc.eShowType.CT_MSG_BOX_OK_CANCEL, null, OnOKCallBack, OnCancelCallBack, CommonTipsDesc.eShowMode.SI_NULL, 0, fCountDownTime, FrameLayer.High, bExclusive, null);
        }

        public static void SysNotifyMsgBoxOkCancel(string msgContent, string OkBtnText, string CancelBtnText, UnityEngine.Events.UnityAction OnOKCallBack = null, UnityEngine.Events.UnityAction OnCancelCallBack = null)
        {
            string FinalContent = SwitchNotifyDescription(msgContent, null);
            SetCommonMsgBoxOKCancel(FinalContent, null, OnOKCallBack, OnCancelCallBack, OkBtnText, CancelBtnText);
        }

        public static void SysNotifyMsgBoxCancelOk(string msgContent, UnityEngine.Events.UnityAction OnCancelCallBack = null, UnityEngine.Events.UnityAction OnOKCallBack = null, float fCountDownTime = 0.0f, bool bExclusive = false, CommonMsgBoxCancelOKParams param = null)
        {
            SwitchNotifyType(msgContent, CommonTipsDesc.eShowType.CT_MSG_BOX_CANCEL_OK, null, OnOKCallBack, OnCancelCallBack, CommonTipsDesc.eShowMode.SI_NULL, 0, fCountDownTime, FrameLayer.High, bExclusive, param);
        }

        public static void SysNotifyMsgBoxCancelOk(string msgContent, string CancelBtnText, string OkBtnText, UnityEngine.Events.UnityAction OnCancelCallBack = null, UnityEngine.Events.UnityAction OnOKCallBack = null, float fCountDownTime = 0.0f, bool bExclusive = false, CommonMsgBoxCancelOKParams param = null)
        {
            string FinalContent = SwitchNotifyDescription(msgContent, null);
            SetCommonMsgBoxCancelOK(FinalContent, null, OnCancelCallBack, OnOKCallBack, CancelBtnText, OkBtnText, fCountDownTime, FrameLayer.High, bExclusive, param);
        }

        public static void SysNotifyMsgBoxCancelOk(string msgContent, string CancelBtnText, string OkBtnText, UnityEngine.Events.UnityAction OnCancelCallBack = null, UnityEngine.Events.UnityAction OnOKCallBack = null, float fCountDownTime = 0.0f, bool bExclusive = false, CommonMsgBoxCancelOKParams param = null, bool bIsCountDownTimeOKBtn = false)
        {
            SetCommonMsgBoxCancelOK(msgContent, null, OnCancelCallBack, OnOKCallBack, CancelBtnText, OkBtnText, fCountDownTime, FrameLayer.High, bExclusive, param, bIsCountDownTimeOKBtn);
        }

        public static void SysNotifyConfirmFrame(string msgContent, UnityEngine.Events.UnityAction OnOKCallBack = null)
        {
            SwitchNotifyType(msgContent, CommonTipsDesc.eShowType.CT_CLICK_CONFIRM_FRAME, null, OnOKCallBack, null, CommonTipsDesc.eShowMode.SI_NULL, 0, 0.0f, FrameLayer.High, false, null);
        }

        public static void SysNotifyTextAnimation(string msgContent, CommonTipsDesc.eShowMode eShowMode = CommonTipsDesc.eShowMode.SI_UNIQUE)
        {
            SwitchNotifyType(msgContent, CommonTipsDesc.eShowType.CT_SYSTEM_TEXT_ANIMATION, null, null, null, eShowMode, 0, 0.0f, FrameLayer.High, false, null);
        }

        public static void SysNotifyFloatingEffect(string msgContent, CommonTipsDesc.eShowMode eShowMode = CommonTipsDesc.eShowMode.SI_QUEUE, int itemId = 0)
        {
            SwitchNotifyType(msgContent, CommonTipsDesc.eShowType.CT_TEXT_FLOAT_EFFECT, null, null, null, eShowMode, itemId, 0.0f, FrameLayer.High, false, null);
        }

        public static void BaseMsgBoxOK(string msgContent, UnityEngine.Events.UnityAction OnOKCallBack = null, string OKtext = "")
        {
            SetBaseMsgBoxOK(msgContent, OKtext, null, OnOKCallBack);
        }

        public static void HotUpdateMsgBoxOkCancel(string msgContent, UnityEngine.Events.UnityAction OnOKCallBack = null, UnityEngine.Events.UnityAction OnCancelCallBack = null)
        {
            SwitchNotifyType(msgContent, CommonTipsDesc.eShowType.CT_HOT_UPDATE_OK_CANCEL, null, OnOKCallBack, OnCancelCallBack, CommonTipsDesc.eShowMode.SI_NULL, 0, 0.0f, FrameLayer.High, false, null);
        }

        public static void BaseMsgBoxOkCancel(string msgContent, UnityEngine.Events.UnityAction OnOKCallBack = null, UnityEngine.Events.UnityAction OnCancelCallBack = null, string OkBtnText = "", string CancelBtnText = "")
        {
            SetBaseMsgBoxOKCancel(msgContent, null, OnOKCallBack, OnCancelCallBack, OkBtnText, CancelBtnText);
        }

        public static void HotUpdateMsgBoxOk(string msgContent, UnityEngine.Events.UnityAction OnOKCallBack = null)
		{
			SwitchNotifyType(msgContent, CommonTipsDesc.eShowType.CT_HOT_UPDATE_OK, null, OnOKCallBack, null, CommonTipsDesc.eShowMode.SI_NULL, 0, 0.0f, FrameLayer.High, false, null);
		}

        // 表格没有注册的显示类型的接口
        public static void SysNotifyFloatingEffectByPos(string msgContent, CommonTipsDesc.eShowMode eShowMode = CommonTipsDesc.eShowMode.SI_QUEUE)
        {
            SetFloatingEffectShowMode(msgContent, eShowMode, 0, true);
        }

        public static CommonItemAwardFrame CreateSysNotifyCommonItemAwardFrame()
        {
            string name = string.Format("ItemAwardNotify{0}", AwardNotifyCount);
            CommonItemAwardFrame ItemAwardShow = ClientSystemManager.instance.OpenFrame<CommonItemAwardFrame>(FrameLayer.High, null, name) as CommonItemAwardFrame;

            AwardNotifyCount++;
            return ItemAwardShow;
        }

        public static void SysDungeonAnimation(string msgContent, CommonTipsDesc.eShowMode eShowMode = CommonTipsDesc.eShowMode.SI_UNIQUE)
        {
            SetDungeonAnimationShowMode(msgContent, eShowMode);
        }

        public static void SysDungeonAnimation2(string msgContent, CommonTipsDesc.eShowMode eShowMode = CommonTipsDesc.eShowMode.SI_UNIQUE)
        {
            SetDungeonAnimation2ShowMode(msgContent, eShowMode);
        }

		public static GameObject SysDungeonSkillTip(string msgContent, float duration=5.0f)
		{
			GameObject AnimationObj = Utility.FindGameObject(ClientSystemManager.instance.HighLayer, "CommonTipSkill", false);
			if (AnimationObj == null)
			{
				AnimationObj = CreateSysDungeonTip();
			}

			//GameObject TipAnimation = CreateSysDungeonTip();
			ComCommonBind bind = AnimationObj.GetComponent<ComCommonBind>();
			if (bind != null)
			{
				var txt = bind.GetCom<Text>("txtTip");
				if (txt != null)
					txt.text = msgContent;
			}

            AutoCloseBattle ac = AnimationObj.GetComponent<AutoCloseBattle>();
			if (ac == null)
				ac = AnimationObj.AddComponent<AutoCloseBattle>();
			ac.SetCloseTime(duration);
            dungeonSkillTip = AnimationObj;
            return AnimationObj;
		}

        public static void ClearDungeonSkillTip()
        {
            if (dungeonSkillTip == null)
                return;
            GameObject.Destroy(dungeonSkillTip);
        }

        public static void SysNotifyTextAnimation2(string msgContent, string msgContent2, CommonTipsDesc.eShowMode eShowMode = CommonTipsDesc.eShowMode.SI_UNIQUE)
        {
            SetNotifyTextAnimation2ShowMode(msgContent, msgContent2, eShowMode);
        }

        // 读表数据入口
        private static void ComSysNotifyByTableID(int NotifyID, UnityEngine.Events.UnityAction OnOKCallBack = null, UnityEngine.Events.UnityAction OnCancelCallBack = null)
        {
            var NotifyData = TableManager.GetInstance().GetTableItem<CommonTipsDesc>(NotifyID);
            if (NotifyData == null)
            {
                SetCommonMsgBoxOK(string.Format("[通用提示表]没有添加错误码[{0}]对应的提示内容", NotifyID));
                return;
            }

            SwitchNotifyType(NotifyData.Descs, NotifyData.ShowType, NotifyData, OnOKCallBack, OnCancelCallBack, NotifyData.ShowMode, 0, 0.0f, FrameLayer.High, false, null);
        }

        private static void ComSysNotifyByTableID(int NotifyID, UnityEngine.Events.UnityAction OnOKCallBack, UnityEngine.Events.UnityAction OnCancelCallBack, int iItemTableID, params object[] args)
        {
            var NotifyData = TableManager.GetInstance().GetTableItem<CommonTipsDesc>(NotifyID);
            if (NotifyData == null)
            {
                SetCommonMsgBoxOK(string.Format("[通用提示表]没有添加错误码[{0}]对应的提示内容", NotifyID));
                return;
            }

            SwitchNotifyType(NotifyData.Descs, NotifyData.ShowType, NotifyData, OnOKCallBack, OnCancelCallBack, NotifyData.ShowMode, iItemTableID, 0.0f, FrameLayer.High, false, args);
        }

        private static void ComSysNotifyByTableID(int NotifyID, UnityEngine.Events.UnityAction OnOKCallBack, UnityEngine.Events.UnityAction OnCancelCallBack, float fWaitTime, params object[] args)
        {
            var NotifyData = TableManager.GetInstance().GetTableItem<CommonTipsDesc>(NotifyID);
            if (NotifyData == null)
            {
                SetCommonMsgBoxOK(string.Format("[通用提示表]没有添加错误码[{0}]对应的提示内容", NotifyID));
                return;
            }

            SwitchNotifyType(NotifyData.Descs, NotifyData.ShowType, NotifyData, OnOKCallBack, OnCancelCallBack, NotifyData.ShowMode, 0, fWaitTime, FrameLayer.High, false, args);
        }

        public static void SysNotifyGetNewItemEffect(ItemData itemData, bool bHighValue = false,string msgContent = "")
        {
            GetItemEffectFrame frame = null;
            if (ClientSystemManager.GetInstance().IsFrameOpen<GetItemEffectFrame>())
            {                
                frame = ClientSystemManager.GetInstance().GetFrame(typeof(GetItemEffectFrame)) as GetItemEffectFrame;
            }
            else
            {                
                frame = ClientSystemManager.GetInstance().OpenFrame<GetItemEffectFrame>(FrameLayer.Middle, null) as GetItemEffectFrame;
            }

            if (frame != null)
            {
                frame.AddNewItem(itemData,bHighValue);
            }
            else
            {
                Logger.LogError("can not open or get GetItemEffectFrame");
            }
        }

        // 最终所有数据的入口
        private static void SwitchNotifyType(string Content, CommonTipsDesc.eShowType eShowtype, CommonTipsDesc NotifyData = null, UnityEngine.Events.UnityAction OnOKCallBack = null, UnityEngine.Events.UnityAction OnCancelCallBack = null, CommonTipsDesc.eShowMode eShowMode = CommonTipsDesc.eShowMode.SI_UNIQUE, int itemId = 0, float fCountDownTime = 0.0f, FrameLayer layer = FrameLayer.High, bool bExclusive = false, params object[] args)
        {
            string FinalContent = SwitchNotifyDescription(Content, args);

            //FinalContent = "超级长的一段字符串，应该有两行了吧大啊啊啊啊";
            //FinalContent = "只有一行的字符串";

            //SetCommonMsgBoxOKCancel(FinalContent, NotifyData, OnOKCallBack, OnCancelCallBack, fCountDownTime, layer, bExclusive);
            //SetCommonMsgBoxCancelOK(FinalContent, NotifyData, OnCancelCallBack, OnOKCallBack, fCountDownTime, layer, bExclusive);
            //SetCommonMsgBoxOK(FinalContent, NotifyData, OnOKCallBack, layer, bExclusive);

            //return;

            if (eShowtype == CommonTipsDesc.eShowType.CT_MSG_BOX_OK)
            { 
                SetCommonMsgBoxOK(FinalContent, NotifyData, OnOKCallBack, "" ,layer, bExclusive);
            }
            else if (eShowtype == CommonTipsDesc.eShowType.CT_MSG_BOX_OK_CANCEL)
            {
                SetCommonMsgBoxOKCancel(FinalContent, NotifyData, OnOKCallBack, OnCancelCallBack, "", "", fCountDownTime, layer, bExclusive);
            }
            else if(eShowtype == CommonTipsDesc.eShowType.CT_MSG_BOX_CANCEL_OK)
            {
                CommonMsgBoxCancelOKParams param = new CommonMsgBoxCancelOKParams();
                if (args != null && args.Length > 0)
                {
                    param = (CommonMsgBoxCancelOKParams)args[0];
                }
                SetCommonMsgBoxCancelOK(FinalContent, NotifyData, OnCancelCallBack, OnOKCallBack, "", "", fCountDownTime, layer, bExclusive, param);
            }
            else if (eShowtype == CommonTipsDesc.eShowType.CT_CLICK_CONFIRM_FRAME)
            {
                //SetCommonConfirmFrame(FinalContent, NotifyData, OnOKCallBack);
            }
            else if (eShowtype == CommonTipsDesc.eShowType.CT_SYSTEM_TEXT_ANIMATION)
            {
                SetCommonTextAnimationShowMode(FinalContent, eShowMode, layer);
            }
            else if (eShowtype == CommonTipsDesc.eShowType.CT_TEXT_FLOAT_EFFECT)
            {
                SetFloatingEffectShowMode(FinalContent, CommonTipsDesc.eShowMode.SI_QUEUE, itemId, false, layer);
            }
            else if (eShowtype == CommonTipsDesc.eShowType.CT_DUNGEON_TEXT_ANIMATION)
            {
                SetDungeonAnimationShowMode(FinalContent, eShowMode, layer);
            }
            else if (eShowtype == CommonTipsDesc.eShowType.CT_DUNGEON_TEXT_ANIMATION_2)
            {
                SetDungeonAnimation2ShowMode(FinalContent, eShowMode, layer);
            }
            else if(eShowtype == CommonTipsDesc.eShowType.CT_HOT_UPDATE_OK)
            {
				SetBaseMsgBoxOK(FinalContent, "确定", NotifyData, OnOKCallBack, layer);
            }
            else if(eShowtype == CommonTipsDesc.eShowType.CT_HOT_UPDATE_OK_CANCEL)
            {
                SetBaseMsgBoxOKCancel(FinalContent, NotifyData, OnOKCallBack, OnCancelCallBack, "" , "", layer);
            }
        }

        // 字符串处理
        private static string SwitchNotifyDescription(string des, params object[] args)
        {
            string FinalStr = des;

            if (args != null)
            {
                string descs = System.Text.RegularExpressions.Regex.Replace(FinalStr, "\\{[\\w]*\\:([\\d]*)\\}", "{$1}");
                FinalStr = string.Format(descs, args);
            }
            if(!string.IsNullOrEmpty(FinalStr))
            {
                FinalStr = FinalStr.Replace("\\n", "\n");
            }
            return FinalStr;
        }

        // 设置界面类提示的参数
        private static void SetCommonMsgBoxOK(string content, CommonTipsDesc NotifyData = null, UnityEngine.Events.UnityAction OnOKCallBack = null, string OkBtnText = "", FrameLayer layer = FrameLayer.High, bool bExclusive = false)
        {
            if(content == "" || content == "-" || content.Length <= 0)
            {
                return;
            }

            CommonMsgBoxOK MsgBox = CreateSysNotifyMsgBoxOK(layer, bExclusive);

            if (NotifyData != null)
            {
                MsgBox.SetNotifyDataByTable(NotifyData, content);
            }
            else
            {
                MsgBox.SetMsgContent(content);

                if(OkBtnText != "")
                {
                    MsgBox.SetOkBtnText(OkBtnText);
                }
            }

            if (OnOKCallBack != null)
            {
                MsgBox.AddListener(OnOKCallBack);
            }
        }

        private static void SetCommonMsgBoxOKCancel(string content, CommonTipsDesc NotifyData = null, UnityEngine.Events.UnityAction OnOKCallBack = null, UnityEngine.Events.UnityAction OnCancelCallBack = null, string OkBtnText = "", string CancelBtnText = "", float fCountDownTime = 0.0f, FrameLayer layer = FrameLayer.High, bool bExclusive = false)
        {
            if (content == "" || content == "-" || content.Length <= 0)
            {
                return;
            }

            CommonMsgBoxOKCancel MsgBox = CreateSysNotifyMsgBoxOKCancel(layer, bExclusive);

            MsgBox.SetMsgContent(content);

            if (NotifyData != null)
            {
                MsgBox.SetNotifyDataByTable(NotifyData);
            }
            else
            {
                if(OkBtnText != "")
                {
                    MsgBox.SetOkBtnText(OkBtnText);
                }

                if(CancelBtnText != "")
                {
                    MsgBox.SetCancelBtnText(CancelBtnText);
                }
            }

            if (OnOKCallBack != null || OnCancelCallBack != null)
            {
                MsgBox.AddListener(OnOKCallBack, OnCancelCallBack);
            }

            if (fCountDownTime > 0.0f)
            {
                MsgBox.SetCountDownTime(fCountDownTime);
            }
        }

        public static void SetMallBuyMsgBoxOKCancel(string content, UnityEngine.Events.UnityAction OnOKCallBack = null, UnityEngine.Events.UnityAction OnCancelCallBack = null, FrameLayer layer = FrameLayer.High, bool bExclusive = false)
        {
            if (content == "" || content == "-" || content.Length <= 0)
            {
                return;
            }

            CommonMsgBoxOKCancel MsgBox = CreateSysNotifyMsgBoxOKCancel(layer, bExclusive);

            MsgBox.SetMsgContent(content);
            MsgBox.SetbNotify(true);
          
            if (OnOKCallBack != null || OnCancelCallBack != null)
            {
                MsgBox.AddListener(OnOKCallBack, OnCancelCallBack);
            }
            
        }

        private static void SetCommonMsgBoxCancelOK(string content, CommonTipsDesc NotifyData = null, UnityEngine.Events.UnityAction OnCancelCallBack = null, UnityEngine.Events.UnityAction OnOKCallBack = null, string CancelBtnText = "", string OkBtnText = "", float fCountDownTime = 0.0f, FrameLayer layer = FrameLayer.High, bool bExclusive = false, CommonMsgBoxCancelOKParams param = null, bool bIsCountDownTimeOKBtn = false)
        {
            if (content == "" || content == "-" || content.Length <= 0)
            {
                return;
            }

            CommonMsgBoxCancelOK MsgBox = CreateSysNotifyMsgBoxCancelOK(layer, bExclusive);

            MsgBox.SetMsgContent(content);

            MsgBox.InitMsgBox(param);

            if (NotifyData != null)
            {
                MsgBox.SetNotifyDataByTable(NotifyData);
            }
            else
            {
                if (CancelBtnText != "")
                {
                    MsgBox.SetCancelBtnText(CancelBtnText);
                }

                if (OkBtnText != "")
                {
                    MsgBox.SetOkBtnText(OkBtnText);
                }
            }

            if (OnOKCallBack != null || OnCancelCallBack != null)
            {
                MsgBox.AddListener(OnOKCallBack, OnCancelCallBack);
            }

            if (fCountDownTime > 0.0f)
            {
                MsgBox.SetCountDownTime(fCountDownTime, bIsCountDownTimeOKBtn);
            }
        }

        private static void SetBaseMsgBoxOK(string content, string OKtext = "", CommonTipsDesc NotifyData = null, UnityEngine.Events.UnityAction OnOKCallBack = null, FrameLayer layer = FrameLayer.High)
        {
            if (content == "" || content == "-" || content.Length <= 0)
            {
                return;
            }

            BaseMsgBoxOK MsgBox = CreateBaseMsgBoxOK();

            if (NotifyData != null)
            {
                MsgBox.SetNotifyDataByTable(NotifyData, content);
            }
            else
            {
                MsgBox.SetMsgContent(content);
            }

            if(OKtext != "")
            {
                MsgBox.SetOKBtnText(OKtext);
            }

            if (OnOKCallBack != null)
            {
                MsgBox.AddListener(OnOKCallBack);
            }
        }

        private static void SetBaseMsgBoxOKCancel(string content, CommonTipsDesc NotifyData = null, UnityEngine.Events.UnityAction OnOKCallBack = null, UnityEngine.Events.UnityAction OnCancelCallBack = null, string OkBtnText = "", string CancelBtnText = "", FrameLayer layer = FrameLayer.High)
        {
            if (content == "" || content == "-" || content.Length <= 0)
            {
                return;
            }

            BaseMsgBoxOKCancel MsgBox = CreateBaseMsgBoxOKCancel();

            if (NotifyData != null)
            {
                MsgBox.SetNotifyDataByTable(NotifyData, content);
            }
            else
            {
                MsgBox.SetMsgContent(content);

                if(OkBtnText != "")
                {
                    MsgBox.SetOkBtnText(OkBtnText);
                }
                
                if(CancelBtnText != "")
                {
                    MsgBox.SetCancelBtnText(CancelBtnText);
                }       
            }

            if (OnOKCallBack != null || OnCancelCallBack != null)
            {
                MsgBox.AddListener(OnOKCallBack, OnCancelCallBack);
            }
        }

        private static void SetCommonConfirmFrame(string content, CommonTipsDesc NotifyData = null, UnityEngine.Events.UnityAction OnOKCallBack = null, FrameLayer layer = FrameLayer.High)
        {
            CommonConfirmFrame FullScreenTip = CreateSysNotifyConfirmFrame();

            if (NotifyData != null)
            {
                FullScreenTip.SetNotifyDataByTable(NotifyData, content);
            }
            else
            {
                FullScreenTip.SetMsgContent(content);
            }

            if (OnOKCallBack != null)
            {
                FullScreenTip.AddListener(OnOKCallBack);
            }
        }

        // 设置动画类提示的显示模式
        private static void SetCommonTextAnimationShowMode(string content, CommonTipsDesc.eShowMode eShowMode, FrameLayer layer = FrameLayer.High)
        {
            SetFloatingEffectShowMode(content, eShowMode);
//             if(eShowMode == CommonTipsDesc.eShowMode.SI_UNIQUE)
//             {
//                 GameObject AnimationObj = Utility.FindGameObject(ClientSystemManager.instance.TopLayer, "CommonTipAnimation", false);
//                 if (AnimationObj != null)
//                 {
//                     return;
//                 }
//             }
//             else if(eShowMode == CommonTipsDesc.eShowMode.SI_QUEUE)
//             {
//                 CommonAnimList.Add(content);
//                 return;
//             }
// 
//             GameObject TipAnimation = CreateSysCommonTextAnimation();
// 
//             GameObject objText = TipAnimation.transform.Find("Text").gameObject;
//             objText.GetComponent<UnityEngine.UI.Text>().text = content;
        }

        private static void SetFloatingEffectShowMode(string content, CommonTipsDesc.eShowMode eShowMode, int itemId = 0, bool bByPos = false, FrameLayer layer = FrameLayer.High)
        {
            if (!bByPos)
            {
                if (eShowMode == CommonTipsDesc.eShowMode.SI_UNIQUE)
                {
                    GameObject FloatingObj = Utility.FindGameObject(ClientSystemManager.instance.HighLayer, "FloatingEffect", false);
                    if (FloatingObj != null)
                    {
                        return;
                    }
                }
                else if (eShowMode == CommonTipsDesc.eShowMode.SI_QUEUE)
                {
                    //if(FloatingEffectList.Count > 0)
                    {
                        FloatEffectData data = new FloatEffectData();
                        data.text = content;
                        data.itemId = itemId;

                        FloatingEffectList.Add(data);
                        return;
                    }
                }

                GameObject FloatingEffect = CreateSysNotifyFloatingEffect();

                ComCalTime time = FloatingEffect.GetComponent<ComCalTime>();
                if (time != null)
                {
                    time.BeginCalTime(true);
                }

                GameObject objPos = FloatingEffect.transform.Find("pos").gameObject;
                GameObject objText = FloatingEffect.transform.Find("text").gameObject;
                GameObject objText2 = FloatingEffect.transform.Find("text2").gameObject;

                ItemTable TableData = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);
                if (itemId <= 0 ||TableData == null)
                {
                    // 纯文字提示
                    objPos.SetActive(false);
                    objText.SetActive(false);
                    objText2.GetComponent<Text>().text = content;

                    FloatingImmediateEffectList.Add(FloatingEffect);

                    return;
                }

                // 道具提示
                // Sprite spr = AssetLoader.instance.LoadRes(TableData.Icon, typeof(Sprite)).obj as Sprite;
                // Sprite SprBack = GetItemIconBackByQuality(TableData);

                Image[] imgs = FloatingEffect.GetComponentsInChildren<Image>();
                for(int i = 0; i < imgs.Length; i++)
                {
                    if(imgs[i].name.Equals("icon"))
                    {
                        //if(spr != null)
                        //{
                        //    imgs[i].sprite = spr;
                        //}

                        Image objPosImg = objPos.GetComponent<Image>();
                        if (!string.IsNullOrEmpty(TableData.Icon))
                        {
                            GetItemIconBackByQuality(ref objPosImg, TableData);

                            objPosImg.CustomActive(true);
                            ETCImageLoader.LoadSprite(ref imgs[i], TableData.Icon);
                        }
                        else
                        {
                            objPosImg.CustomActive(false);
                            objText.transform.localPosition = objPos.transform.localPosition;
                        }

                        //if(SprBack != null)
                        //{
                        //    objPos.GetComponent<Image>().sprite = GetItemIconBackByQuality(TableData);
                        //}       
                        
                    }
                }
               
                objText.GetComponent<Text>().text = content;
                objText2.SetActive(false);

                FloatingImmediateEffectList.Add(FloatingEffect);
            }
            else
            {
//                 var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
//                 if (current == null)
//                 {
//                     return;
//                 }
// 
//                 if (current.MainPlayer == null)
//                 {
//                     return;
//                 }
// 
//                 if(eShowMode == CommonTipsDesc.eShowMode.SI_IMMEDIATELY)
//                 {
//                     current.MainPlayer.CreatePropertyRiseEffect(content);
//                 }
//                 else
//                 {
//                     FloatingEffectListPos.Add(content);
//                 }
            }
        }

        private static void SetDungeonAnimationShowMode(string content, CommonTipsDesc.eShowMode eShowMode, FrameLayer layer = FrameLayer.High)
        {
            if(eShowMode == CommonTipsDesc.eShowMode.SI_UNIQUE || eShowMode == CommonTipsDesc.eShowMode.SI_QUEUE)
            {
                GameObject AnimationObj = Utility.FindGameObject(ClientSystemManager.instance.HighLayer, "CommonSysDungeonAnimation", false);
                if (AnimationObj != null)
                {
                    return;
                }
            }

            GameObject TipAnimation = CreateSysDungeonAnimation();

            GameObject objText = TipAnimation.transform.Find("Text").gameObject;
            objText.GetComponent<UnityEngine.UI.Text>().text = content;
        }

        private static void SetDungeonAnimation2ShowMode(string content, CommonTipsDesc.eShowMode eShowMode, FrameLayer layer = FrameLayer.High)
        {
            if (eShowMode == CommonTipsDesc.eShowMode.SI_UNIQUE || eShowMode == CommonTipsDesc.eShowMode.SI_QUEUE)
            {
                GameObject AnimationObj = Utility.FindGameObject(ClientSystemManager.instance.HighLayer, "CommonSysDungeonAnimation2", false);
                if (AnimationObj != null)
                {
                    return;
                }
            }

            GameObject TipAnimation = CreateSysDungeonAnimation2();

            GameObject objText = TipAnimation.transform.Find("Text").gameObject;
            objText.GetComponent<UnityEngine.UI.Text>().text = content;
        }

        private static void SetNotifyTextAnimation2ShowMode(string content, string content2, CommonTipsDesc.eShowMode eShowMode)
        {
            if (eShowMode == CommonTipsDesc.eShowMode.SI_UNIQUE)
            {
                GameObject AnimationObj = Utility.FindGameObject(ClientSystemManager.instance.HighLayer, "CommonTipAnimation2", false);
                if (AnimationObj != null)
                {
                    return;
                }
            }
            else if(eShowMode == CommonTipsDesc.eShowMode.SI_QUEUE)
            {
                CommonAnimList2.Add(content);
                CommonAnimListcontent2.Add(content2);

                return;
            }

            GameObject TipAnimation = CreateSysCommonTextAnimation2();

            GameObject objText = TipAnimation.transform.Find("Text").gameObject;
            objText.GetComponent<UnityEngine.UI.Text>().text = content;

            GameObject objText2 = TipAnimation.transform.Find("Text2").gameObject;
            objText2.GetComponent<UnityEngine.UI.Text>().text = content2;
        }

        // 创建各类界面
        private static CommonMsgBoxOK CreateSysNotifyMsgBoxOK(FrameLayer layer = FrameLayer.High, bool bExclusive = false)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<CommonMsgBoxOK>())
            {
                ClientSystemManager.GetInstance().CloseFrame<CommonMsgBoxOK>();
            }

            CommonMsgOutData outData = new CommonMsgOutData();
            outData.bExclusiveWithNewbieGuide = bExclusive;

            CommonMsgBoxOK MsgBox = ClientSystemManager.instance.OpenFrame<CommonMsgBoxOK>(layer, outData) as CommonMsgBoxOK;

            return MsgBox;
        }

        private static CommonMsgBoxOKCancel CreateSysNotifyMsgBoxOKCancel(FrameLayer layer = FrameLayer.High, bool bExclusive = false)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<CommonMsgBoxOKCancel>())
            {
                ClientSystemManager.GetInstance().CloseFrame<CommonMsgBoxOKCancel>();
            }

            CommonMsgOutData outData = new CommonMsgOutData();
            outData.bExclusiveWithNewbieGuide = bExclusive;

            CommonMsgBoxOKCancel MsgBox = ClientSystemManager.GetInstance().OpenFrame<CommonMsgBoxOKCancel>(layer, outData) as CommonMsgBoxOKCancel;

            return MsgBox;
        }

        private static CommonMsgBoxCancelOK CreateSysNotifyMsgBoxCancelOK(FrameLayer layer = FrameLayer.High, bool bExclusive = false)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<CommonMsgBoxCancelOK>())
            {
                ClientSystemManager.GetInstance().CloseFrame<CommonMsgBoxCancelOK>();
            }

            CommonMsgOutData outData = new CommonMsgOutData();
            outData.bExclusiveWithNewbieGuide = bExclusive;

            CommonMsgBoxCancelOK MsgBox = ClientSystemManager.GetInstance().OpenFrame<CommonMsgBoxCancelOK>(layer, outData) as CommonMsgBoxCancelOK;

            return MsgBox;
        }

        private static BaseMsgBoxOK CreateBaseMsgBoxOK()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<BaseMsgBoxOK>())
            {
                ClientSystemManager.GetInstance().CloseFrame<BaseMsgBoxOK>();
            }

            BaseMsgBoxOK MsgBox = ClientSystemManager.instance.OpenFrame<BaseMsgBoxOK>(FrameLayer.High) as BaseMsgBoxOK;

            return MsgBox;
        }

        private static BaseMsgBoxOKCancel CreateBaseMsgBoxOKCancel()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<BaseMsgBoxOKCancel>())
            {
                ClientSystemManager.GetInstance().CloseFrame<BaseMsgBoxOKCancel>();
            }

            BaseMsgBoxOKCancel MsgBox = ClientSystemManager.instance.OpenFrame<BaseMsgBoxOKCancel>(FrameLayer.High) as BaseMsgBoxOKCancel;

            return MsgBox;
        }

        private static CommonConfirmFrame CreateSysNotifyConfirmFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<CommonConfirmFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<CommonConfirmFrame>();
            }

            CommonConfirmFrame MsgBox = ClientSystemManager.instance.OpenFrame<CommonConfirmFrame>(FrameLayer.High) as CommonConfirmFrame;

            return MsgBox;
        }

        private static GameObject CreateSysCommonTextAnimation()
        {
            GameObject TipAnimation = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/CommonSystemNotify/CommonTipAnimation");
            TipAnimation.name = "CommonTipAnimation";

            Utility.AttachTo(TipAnimation, ClientSystemManager.instance.HighLayer);

            return TipAnimation;
        }

        private static GameObject CreateSysCommonTextAnimation2()
        {
            GameObject TipAnimation = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/CommonSystemNotify/CommonTipAnimation2");
            TipAnimation.name = "CommonTipAnimation2";

            Utility.AttachTo(TipAnimation, ClientSystemManager.instance.HighLayer);

            return TipAnimation;
        }

        private static GameObject CreateSysNotifyFloatingEffect()
        {
            GameObject FloatingEffect = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/CommonSystemNotify/CommonFloatingEffect");
            //GameObject FloatingEffect = CGameObjectPool.instance.GetGameObject("UIFlatten/Prefabs/CommonSystemNotify/CommonFloatingEffect", enResourceType.BattleScene, true);
            FloatingEffect.name = "FloatingEffect";

            Utility.AttachTo(FloatingEffect, ClientSystemManager.instance.HighLayer);

            return FloatingEffect;
        }

        private static GameObject CreateSysDungeonAnimation()
        {
            GameObject TipAnimation = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/CommonSystemNotify/CommonTipAnimation_Dungeon");
            TipAnimation.name = "CommonTipAnimation_Dungeon";

            Utility.AttachTo(TipAnimation, ClientSystemManager.instance.HighLayer);

            return TipAnimation;
        }

        private static GameObject CreateSysDungeonAnimation2()
        {
            GameObject TipAnimation = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/CommonSystemNotify/TipGWQCAnimation");
            TipAnimation.name = "CommonSysDungeonAnimation2";

            Utility.AttachTo(TipAnimation, ClientSystemManager.instance.HighLayer);

            return TipAnimation;
        }

		private static GameObject CreateSysDungeonTip()
		{
			GameObject tip = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/Battle/CommonTipSkill");
			tip.name = "CommonTipSkill";

			Utility.AttachTo(tip, ClientSystemManager.instance.HighLayer);
			return tip;
		}

        // 刷新机制
        public void OnUpdate(float timeElapsed)
        {
            CommonAnimInterval += timeElapsed;
            CommonAnimInterval2 += timeElapsed;
            FloatingEffectInterval += timeElapsed;
            FloatingEffectPosInterval += timeElapsed;

            // 上飘效果队列
            if (FloatingEffectInterval > 0.8f)
            {
                FloatingEffectInterval = 0f;

                if (FloatingEffectList.Count > 0)
                {
                    SetFloatingEffectShowMode(FloatingEffectList[0].text, CommonTipsDesc.eShowMode.SI_IMMEDIATELY, FloatingEffectList[0].itemId, false);                           
                }

                RefreshQueueFloatListPos();

                if(FloatingEffectList.Count > 0)
                {
                    FloatingEffectList.RemoveAt(0);        
                }

                if(FloatingImmediateEffectList.Count <= 0)
                {
                    bPlaySound = false;
                }
            }

            RefreshFloatEffectListFadeOut();

            // 通用显示队列
            if (CommonAnimInterval >= 0.85f)
            {
                CommonAnimInterval = 0f;

                if (CommonAnimList.Count > 0)
                {
                    SetCommonTextAnimationShowMode(CommonAnimList[0], CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                    CommonAnimList.RemoveAt(0);
                }
            }

            if(CommonAnimInterval2 >= 1.8f)
            {
                CommonAnimInterval2 = 0f;

                if(CommonAnimList2.Count > 0)
                {
                    SetNotifyTextAnimation2ShowMode(CommonAnimList2[0], CommonAnimListcontent2[0], CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                    CommonAnimList2.RemoveAt(0);
                    CommonAnimListcontent2.RemoveAt(0);
                }
            }

//             // 随角色绑定飘字效果
//             if (FloatingEffectPosInterval >= 0.75f)
//             {
//                 FloatingEffectPosInterval = 0f;
// 
//                 if (FloatingEffectListPos.Count > 0)
//                 {
//                     SetFloatingEffectShowMode(FloatingEffectListPos[0], CommonTipsDesc.eShowMode.SI_IMMEDIATELY, 0, true);
//                     FloatingEffectListPos.RemoveAt(0);
//                 }
//             }
        }

        void RefreshQueueFloatListPos()
        {
            if(FloatingImmediateEffectList.Count <= 0)
            {
                return;
            }

            FloatingImmediateEffectList2.Clear();

            if(!bPlaySound)
            {
                AudioManager.instance.PlaySound(106);
                bPlaySound = true;
            }     

            int index = 0;
            for (int i = 0; i < FloatingImmediateEffectList.Count; i++)
            {
                if(FloatingImmediateEffectList[i] == null)
                {
                    continue;
                }

                ComCalTime time = FloatingImmediateEffectList[i].GetComponent<ComCalTime>();
                if(time == null)
                {
                    continue;
                }

                RectTransform objrect = FloatingImmediateEffectList[i].GetComponent<RectTransform>();

                Vector3 endPos = new Vector3();
                endPos = objrect.localPosition;

                if(FloatingEffectList.Count > 0 || !time.GetPosyIsAdded() || (time.GetPosyIsAdded() && FloatingImmediateEffectList.Count > 3))
                {
                    endPos.y += 70.0f;
                    time.SetPosy(true);
                }

                bool bHold = true;
                if (index == 0 && time.GetPassedTime() > 2.3f)
                {
                    endPos.y += 50.0f;
                    bHold = false;
                }

                Tweener doTweener = DOTween.To(() => objrect.localPosition, r => { objrect.localPosition = r; }, endPos, 0.5f);
                doTweener.SetEase(Ease.Linear);
                //doTweener.OnComplete(() => { _OnDotweenComplete(FloatingImmediateEffectList[i]); });

                if (bHold)
                {
                    FloatingImmediateEffectList2.Add(FloatingImmediateEffectList[i]);
                }

                index++;
            }

            FloatingImmediateEffectList.Clear();

            for(int i = 0; i < FloatingImmediateEffectList2.Count; i++)
            {
                FloatingImmediateEffectList.Add(FloatingImmediateEffectList2[i]);
            }
        }

        void _OnDotweenComplete(GameObject obj)
        {
            CGameObjectPool.instance.RecycleGameObject(obj);
        }

        void RefreshFloatEffectListFadeOut()
        {
//             for()
//             {
// 
//             }
        }

        //static Sprite GetItemIconBackByQuality(ItemTable TableData)
        //{
        //    string str = "";
        //    if(TableData.Color == ItemTable.eColor.BLUE)
        //    {
        //        str = "UIPacked/pck_Common00.png:Common_itemRank_Blue";
        //    }
        //    else if(TableData.Color == ItemTable.eColor.GREEN)
        //    {
        //        str = "UIPacked/pck_Common00.png:Common_itemRank_Green";
        //    }
        //    else if (TableData.Color == ItemTable.eColor.PINK)
        //    {
        //        str = "UIPacked/pck_Common00.png:Common_itemRank_Pink";
        //    }
        //    else if (TableData.Color == ItemTable.eColor.PURPLE)
        //    {
        //        str = "UIPacked/pck_Common00.png:Common_itemRank_Purple";
        //    }
        //    else if (TableData.Color == ItemTable.eColor.YELLOW)
        //    {
        //        str = "UIPacked/pck_Common00.png:Common_itemRank_Orange";
        //    }
        //    else
        //    {
        //        str = "UIPacked/pck_Common00.png:Common_itemRank_White";
        //    }

        //    Sprite spr = AssetLoader.instance.LoadRes(str, typeof(Sprite)).obj as Sprite;
        //    if (spr != null)
        //    {
        //        return spr;
        //    }

        //    return null;
        //}
        static void GetItemIconBackByQuality(ref Image image, ItemTable tableData)
        {
            if (image == null || tableData == null)
            {
                Logger.LogError("GetItemIconBackByQuality param is null");
                return;
            }
            string qualityPath= GameUtility.Item.GetItemQualityBg(tableData.Color);
            ETCImageLoader.LoadSprite(ref image, qualityPath);
        }

        private static SystemNotifyManager hInstance = new SystemNotifyManager();

        private static float CommonAnimInterval = 0f;
        private static List<string> CommonAnimList = new List<string>();

        private static float CommonAnimInterval2 = 0f;
        private static List<string> CommonAnimList2 = new List<string>();
        private static List<string> CommonAnimListcontent2 = new List<string>();

        private static float FloatingEffectInterval = 0f;
        private static List<FloatEffectData> FloatingEffectList = new List<FloatEffectData>();

        private static float FloatingEffectPosInterval = 0f;
        private static List<string> FloatingEffectListPos = new List<string>();

        private static List<GameObject> FloatingImmediateEffectList = new List<GameObject>();
        private static List<GameObject> FloatingImmediateEffectList2 = new List<GameObject>();

        private static int AwardNotifyCount = 0;
        private static bool bPlaySound = false;

        private static GameObject dungeonSkillTip = null;

        #region CommonMsgBoxOkCancelNewFrame

        public static void OpenCommonMsgBoxOkCancelNewFrame(CommonMsgBoxOkCancelNewParamData paramData)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<CommonMsgBoxOkCancelNewFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<CommonMsgBoxOkCancelNewFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<CommonMsgBoxOkCancelNewFrame>(FrameLayer.High, paramData);

            return;
        }

        #endregion

    }
}
