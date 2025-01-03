using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    class GuildCreateFrame : ClientFrame
    {
        [UIControl("Content/Name/InputField")]
        InputField m_nameInput;

        [UIControl("Content/Name/InputField/Text")]
        Text m_labName;

        [UIControl("Content/Name/Count")]
        Text m_labNameCount;

        [UIControl("Content/Declaration/InputField/Text")]
        Text m_labDeclaration;

        [UIControl("Content/Declaration/Count")]
        Text m_labWordsCount;

        [UIControl("Content/Declaration/InputField", typeof(InputField))]
        InputField m_editLabDeclaration;

        [UIControl("Content/Ok/Count")]
        Text m_labCostCount;

        [UIControl("Content/Ok/Icon")]
        Image m_imgCostIcon;

        //[UIControl("Title/Help", typeof(ComButtonEx))]
        //ComButtonEx m_comBtnHelp;

        [UIObject("HelpDesc")]
        GameObject m_objHelpContent;

        [UIControl("HelpDesc/Text")]
        Text m_labTipCreateCost;

        CostItemManager.CostInfo m_costInfo = null;
        int m_nNameMaxWords;
        int m_nDeclarationMaxWords;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildCreate";
        }

        protected override void _OnOpenFrame()
        {
            m_labName.text = string.Empty;
            m_labDeclaration.text = string.Empty;

            m_costInfo = new CostItemManager.CostInfo();
            m_costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.GOLD);
            m_costInfo.nCount = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_GUILD_CREATE_COST).Value;

            if (CostItemManager.GetInstance().IsEnough2Cost(m_costInfo))
            {
                m_labCostCount.text = m_costInfo.nCount.ToString();
            }
            else
            {
                m_labCostCount.text = TR.Value("color_red", m_costInfo.nCount);
            }

            ItemData pointData = ItemDataManager.GetInstance().GetMoneyTableDataByType(ItemTable.eSubType.GOLD);
            // m_imgCostIcon.sprite = AssetLoader.instance.LoadRes(pointData.Icon, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref m_imgCostIcon, pointData.Icon);

            m_labTipCreateCost.text = TR.Value("guild_tip_create_cost", m_costInfo.nCount);

            m_nNameMaxWords = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_GUILD_NAME_MAX_WORDS).Value;
            m_nDeclarationMaxWords = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_GUILD_DECLARATION_MAX_WORDS).Value;

            m_labNameCount.text = string.Format("{0}/{1}", 0, m_nNameMaxWords);

            m_nameInput.onValueChanged.RemoveAllListeners();
            m_nameInput.onValueChanged.AddListener((string a_strValue) =>
            {
                m_labNameCount.text = string.Format("{0}/{1}", a_strValue.Length, m_nNameMaxWords);
            });

            m_labWordsCount.text = string.Format("{0}/{1}", 0, m_nDeclarationMaxWords);

            m_editLabDeclaration.onValueChanged.RemoveAllListeners();
            m_editLabDeclaration.onValueChanged.AddListener((string a_strValue) =>
            {
                m_labWordsCount.text = string.Format("{0}/{1}",
                    a_strValue.Length,
                    m_nDeclarationMaxWords
                    );
            });

            //m_comBtnHelp.onMouseDown.RemoveAllListeners();
            //m_comBtnHelp.onMouseDown.AddListener((var) => { m_objHelpContent.SetActive(true); });
            //m_comBtnHelp.onMouseUp.RemoveAllListeners();
            //m_comBtnHelp.onMouseUp.AddListener((var) => { m_objHelpContent.SetActive(false); });
            m_objHelpContent.SetActive(false);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildCreateSuccess, _OnCreateSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PlayerDataBaseUpdated, _OnPlayerDataBaseUpdated);
        }
        
        protected override void _OnCloseFrame()
        {
            m_costInfo = null;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildCreateSuccess, _OnCreateSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayerDataBaseUpdated, _OnPlayerDataBaseUpdated);
        }

        void _OnCreateSuccess(UIEvent a_event)
        {
            frameMgr.CloseFrame(this);
            frameMgr.CloseFrame<GuildListFrame>();
            frameMgr.OpenFrame<GuildMainFrame>(FrameLayer.Middle);
        }

        void _OnPlayerDataBaseUpdated(UIEvent a_event)
        {
            if (CostItemManager.GetInstance().IsEnough2Cost(m_costInfo))
            {
                m_labCostCount.text = m_costInfo.nCount.ToString();
            }
            else
            {
                m_labCostCount.text = TR.Value("color_red", m_costInfo.nCount);
            }
        }

        [UIEventHandle("Content/Ok")]
        void _OnCreateGuildOKClicked()
        {
            if (string.IsNullOrEmpty(m_labName.text))
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_create_need_name"));
                return;
            }

            if (string.IsNullOrEmpty(m_labDeclaration.text))
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_create_need_declaration"));
                return;
            }

            if (m_labName.text.Length > m_nNameMaxWords)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_create_name_length_invalid", m_nNameMaxWords));
                return;
            }

            if (m_labDeclaration.text.Length > m_nDeclarationMaxWords)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_create_declaration_length_invalid", m_nDeclarationMaxWords));
                return;
            }

            CostItemManager.GetInstance().TryCostMoneyDefault(m_costInfo, () =>
            {
                GuildDataManager.GetInstance().CreateGuild(m_labName.text, m_labDeclaration.text);
            });
        }

        [UIEventHandle("Title/Close")]
        void _OnCreateGuildCancelClicked()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
