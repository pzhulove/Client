using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{
    enum ShowRedPacketMode
    {
        Open, // 开红包 
        Show, // 展示红包 (不显示自己抢了多少，不显示喊话)
    }
    class ShowOpenedRedPacketFrame : ClientFrame
    {
        [UIControl("Head/Viewport/Image")]
        Image m_imgOwnerHead;

        [UIControl("Head/Level")]
        Text m_labOwnerLevel;

        [UIControl("Head/Name")]
        Text m_labOwnerName;

        [UIControl("RedPackName")]
        Text m_labRedPacketName;

        [UIControl("Owned/Group/Text")]
        Text m_labMyGetDesc;

        [UIControl("Owned/Group/Des")]
        Text m_labMyGetDes;

        [UIControl("Owned/Group/Icon")]
        Image m_imgMyGetIcon;

        [UIControl("Desc/Text")]
        Text m_labTotalDesc1;

        [UIControl("Desc/Icon")]
        Image m_imgTotalIcon;

        [UIControl("Desc/Text2")]
        Text m_labTotalDesc2;

        [UIObject("ScrollView/Viewport/Content")]
        GameObject m_objReceiverRoot;

        [UIObject("ScrollView/Viewport/Content/Template")]
        GameObject m_objReceiverTemplate;

        [UIControl("Akeypropaganda/Des")]
        Text m_labAkeyppdDes;

        [UIControl("Akeypropaganda/AKeyBtn")]
        UIGray m_grayAKeyBtn;

        bool isClick = false;

        public static ShowRedPacketMode showRedPacketMode = ShowRedPacketMode.Open;

        #region ui bind
        private GameObject Akeypropaganda = null;
        private GameObject Owned = null;
        private ScrollRect ScrollView = null;
        private GameObject Desc = null;
        #endregion

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/RedPack/ShowOpenedRedPack";
        }

        protected override void _OnOpenFrame()
        {
            RedPacketDetail data = userData as RedPacketDetail;
            if (data == null)
            {
                Logger.LogErrorFormat("open ShowOpenedRedPacketFrame, userdata is invalid!");
                return;
            }

            m_objReceiverTemplate.SetActive(false);

            string strGetMoneyIcon = RedPackDataManager.GetInstance().GetGetMoneyIcon(data.baseEntry.reason);
            string strCostMoneyIcon = RedPackDataManager.GetInstance().GetCostMoneyIcon(data.baseEntry.reason);

            // m_imgOwnerHead.sprite = AssetLoader.instance.LoadRes(_GetHeadByJobID(data.ownerIcon.occu), typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref m_imgOwnerHead, _GetHeadByJobID(data.ownerIcon.occu));
            m_labOwnerLevel.text = data.ownerIcon.level.ToString();
            m_labOwnerName.text = TR.Value("WhoistheRedPack", data.ownerIcon.name);
            m_labRedPacketName.text = data.baseEntry.name;
            m_labTotalDesc1.text = TR.Value("guild_redpacket_total_desc1", data.baseEntry.totalNum, data.baseEntry.totalMoney);
            // m_imgTotalIcon.sprite = AssetLoader.instance.LoadRes(strCostMoneyIcon, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref m_imgTotalIcon, strCostMoneyIcon);
            int nRemain = data.baseEntry.totalNum - data.receivers.Length;
            if (nRemain > 0)
            {
                m_labTotalDesc2.text = TR.Value("guild_redpacket_total_desc2_remain", nRemain);
            }
            else
            {
                m_labTotalDesc2.text = TR.Value("guild_redpacket_total_desc2_finish");
            }

            uint nBest = 0;
            GameObject objBest = null;
            RedPacketReceiverEntry myReceiver = null;
            for (int i = 0; i < data.receivers.Length; ++i)
            {
                RedPacketReceiverEntry receiver = data.receivers[i];
                if (receiver.icon.id == PlayerBaseData.GetInstance().RoleID)
                {
                    myReceiver = receiver;
                }

                GameObject obj = GameObject.Instantiate(m_objReceiverTemplate);
                obj.transform.SetParent(m_objReceiverRoot.transform, false);
                obj.SetActive(true);

                //Utility.GetComponetInChild<Image>(obj, "Head/Viewport/Image").sprite =
                //    AssetLoader.instance.LoadRes(_GetHeadByJobID(receiver.icon.occu), typeof(Sprite)).obj as Sprite;
                {
                    Image img = Utility.GetComponetInChild<Image>(obj, "Head/Viewport/Image");
                    ETCImageLoader.LoadSprite(ref img, _GetHeadByJobID(receiver.icon.occu));
                }
                Utility.GetComponetInChild<Text>(obj, "Name").text = receiver.icon.name;
                Utility.GetComponetInChild<Text>(obj, "Count/Text").text = receiver.money.ToString();
                //Utility.GetComponetInChild<Image>(obj, "Count/Icon").sprite =
                //    AssetLoader.instance.LoadRes(strGetMoneyIcon, typeof(Sprite)).obj as Sprite;
                {
                    Image img = Utility.GetComponetInChild<Image>(obj, "Count/Icon");
                    ETCImageLoader.LoadSprite(ref img, strGetMoneyIcon);
                }

                if (data.receivers.Length >= data.baseEntry.totalNum && nBest < receiver.money)
                {
                    nBest = receiver.money;
                    if (objBest != null)
                    {
                        objBest.SetActive(false);
                    }
                    objBest = Utility.FindGameObject(obj, "Best");
                    objBest.SetActive(true);
                }
                else
                {
                    Utility.FindGameObject(obj, "Best").SetActive(false);
                }
            }

            if (myReceiver != null)
            {
                m_labMyGetDesc.text = myReceiver.money.ToString();
                m_labAkeyppdDes.text = TR.Value("guild_redpacket_my_get_des", data.ownerIcon.name);
                m_imgMyGetIcon.gameObject.SetActive(true);
                m_labMyGetDes.gameObject.SetActive(true);
                // m_imgMyGetIcon.sprite = AssetLoader.instance.LoadRes(strGetMoneyIcon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref m_imgMyGetIcon, strGetMoneyIcon);
            }
            else
            {
                m_labMyGetDesc.text = TR.Value("guild_redpacket_my_get_none");
                m_labAkeyppdDes.text = TR.Value("guild_redpacket_my_get_no", data.ownerIcon.name);
                m_imgMyGetIcon.gameObject.SetActive(false);
                m_labMyGetDes.gameObject.SetActive(false);
            }

            if (data.baseEntry.type == (int)RedPacketType.GUILD)
            {
                _AddButton("Akeypropaganda/AKeyBtn", AKeyGuildPropaganda);
            }
            else if (data.baseEntry.type == (int)RedPacketType.NEW_YEAR)
            {
                _AddButton("Akeypropaganda/AKeyBtn", AKeyPropaganda);
            }

            if (showRedPacketMode == ShowRedPacketMode.Show)
            {
                Akeypropaganda.CustomActive(false);
                Owned.CustomActive(false);

                MoveOffset(ref Desc, 0, 80);

                GameObject go = ScrollView.gameObject;
                MoveOffset(ref go, 0, 80);

                ScrollView.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(ScrollView.gameObject.GetComponent<RectTransform>().sizeDelta.x, 360);
            }
        }

        void MoveOffset(ref GameObject obj, int ix, int iy)
        {
            if (obj == null)
            {
                return;
            }

            RectTransform objrect = obj.GetComponent<RectTransform>();

            Vector3 endPos = new Vector3();
            endPos = objrect.localPosition;

            endPos.x += ix;
            endPos.y += iy;

            obj.transform.localPosition = endPos;
            return;
        }

        protected override void _OnCloseFrame()
        {
            showRedPacketMode = ShowRedPacketMode.Open;

            m_grayAKeyBtn.enabled = false;
            isClick = false;
        }

        protected override void _bindExUI()
        {
            Akeypropaganda = mBind.GetGameObject("Akeypropaganda");
            Owned = mBind.GetGameObject("Owned");
            ScrollView = mBind.GetCom<ScrollRect>("ScrollView");
            Desc = mBind.GetGameObject("Desc");
        }

        protected override void _unbindExUI()
        {
            Akeypropaganda = null;
            Owned = null;
            ScrollView = null;
            Desc = null;
        }

        string _GetHeadByJobID(int a_nJobID)
        {
            string strHead = string.Empty;
            ProtoTable.JobTable table = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(a_nJobID);
            if (table != null)
            {
                ProtoTable.ResTable resTable = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(table.Mode);
                if (resTable != null)
                {
                    strHead = resTable.IconPath;
                }
            }
            return strHead;
        }
        void AKeyPropaganda()
        {
            if (isClick)
            {
                return;
            }
            string content = m_labAkeyppdDes.text;

            var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_WORLD_CHAT_LV_LIMIT);
            if (systemValue != null && systemValue.Value > PlayerBaseData.GetInstance().Level)
            {
                SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("chat_world_talk_need_level"), systemValue.Value), CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                return;
            }
            int iFreeTimes = ChatManager.GetInstance().FreeWorldChatLeftTimes;
            if (iFreeTimes <= 0)
            {
                if (!ChatManager.GetInstance().CheckWorldActivityValueEnough())
                {
                    SystemNotifyManager.SystemNotify(7006, () =>
                    {
                        ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle, VipTabType.VIP);
                    });
                    return;
                }
            }
            if (!ChatManager.world_chat_try_enter_cool_down())
            {
                SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("chat_world_talk_need_interval"), ChatManager.world_cool_time), CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                return;
            }
            SystemNotifyManager.SysNotifyTextAnimation("发送喊话成功!", CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
            ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, content);

            m_grayAKeyBtn.enabled = true;
            isClick = true;
        }

        void AKeyGuildPropaganda()
        {
            if (isClick)
            {
                return;
            }
            string content = m_labAkeyppdDes.text;

            SystemNotifyManager.SysNotifyTextAnimation("发送喊话成功!", CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
            ChatManager.GetInstance().SendChat(ChatType.CT_GUILD, content);

            m_grayAKeyBtn.enabled = true;
            isClick = true;
        }
    }
}
