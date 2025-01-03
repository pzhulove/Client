using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    enum EGuildCommonModifyMode
    {
        Short,
        Long,
    }

    class GuildCommonModifyData
    {
        public string strTitle;
        public string strEmptyDesc;
        public string strDefultContent;
        public int nMaxWords;
        public bool bHasCost;
        public int nCostID;
        public int nCostCount;
        public EGuildCommonModifyMode eMode;
        public UnityEngine.Events.UnityAction<string> onOkClicked;
        public UnityEngine.Events.UnityAction onCancelClicked;
    }

    class GuildCommonModifyFrame : ClientFrame
    {
        [UIControl("Content/BG/Count")]
        Text m_labWordsCount;

        [UIControl("Bottom/Ok/Cost/Icon")]
        Image m_imgCostIcon;

        [UIControl("Bottom/Ok/Cost/Count")]
        Text m_labCostCount;

        [UIObject("Bottom/Ok/Cost")]
        GameObject m_objCost;

        [UIControl("Title/Name")]
        Text m_labTitle;

        [UIControl("Content/BG/InputField/Placeholder")]
        Text m_labPlaceholder;

        [UIControl("Bottom/Ok")]
        Button m_btnOK;

        [UIControl("Title/Close")]
        Button m_btnCancel;

        [UIControl("Content/BG/InputField", typeof(InputField))]
        InputField m_editLabDeclaration;

        [UIControl("Content/BG/InputField/Text")]
        Text m_labEdit;

        GuildCommonModifyData m_data = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildCommonModify";
        }

        protected override void _OnOpenFrame()
        {
            m_data = userData as GuildCommonModifyData;
            if (m_data == null)
            {
                return;
            }

            RectTransform rectTransform = frame.GetComponent<RectTransform>();

            Vector2 size = rectTransform.sizeDelta;

            if (m_data.eMode == EGuildCommonModifyMode.Short)
            {
                size.y = 500.0f;
                rectTransform.sizeDelta = size;
                m_labEdit.alignment = TextAnchor.MiddleCenter;
            }
            else
            {
                size.y = 600.0f;
                rectTransform.sizeDelta = size;
                m_labEdit.alignment = TextAnchor.UpperLeft;
            }

            m_labTitle.SafeSetText(m_data.strTitle);
            if (m_data.bHasCost)
            {
                m_objCost.CustomActive(true);
                ItemData costItem = ItemDataManager.CreateItemDataFromTable(m_data.nCostID);
                if (costItem != null)
                {
                    // m_imgCostIcon.sprite = AssetLoader.instance.LoadRes(costItem.Icon, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref m_imgCostIcon, costItem.Icon);
                }
                m_labCostCount.SafeSetText(m_data.nCostCount.ToString());
            }
            else
            {
                m_objCost.CustomActive(false);
            }
            m_labWordsCount.SafeSetText(string.Format("{0}/{1}", 0, m_data.nMaxWords));
            m_labPlaceholder.SafeSetText(m_data.strEmptyDesc);
            
            if(m_btnOK!=null)
            {
                m_btnOK.onClick.RemoveAllListeners();
                m_btnOK.onClick.AddListener(() =>
                {
                    if(m_editLabDeclaration!=null)
                    {
                        if (m_editLabDeclaration.text.Length <= 0)
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_modify_words_none"));
                        }
                        else if (m_editLabDeclaration.text.Length > m_data.nMaxWords)
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_modify_words_more"));
                        }
                        else
                        {
                            if (m_data.onOkClicked != null)
                            {
                                m_data.onOkClicked.Invoke(m_editLabDeclaration.text);
                            }
                            //frameMgr.CloseFrame(this);
                        }
                    }
                });
            }
           
            if(m_btnCancel!=null)
            {
                m_btnCancel.onClick.RemoveAllListeners();
                m_btnCancel.onClick.AddListener(() =>
                {
                    if (m_data.onCancelClicked != null)
                    {
                        m_data.onCancelClicked.Invoke();
                    }
                    frameMgr.CloseFrame(this);
                });
            }
           
            if(m_editLabDeclaration!=null)
            {
                m_editLabDeclaration.onValueChanged.RemoveAllListeners();
                m_editLabDeclaration.onValueChanged.AddListener((string a_strValue) =>
                {
                    m_labWordsCount.SafeSetText(string.Format("{0}/{1}",
                        a_strValue.Length,
                        m_data.nMaxWords
                        ));
                });

                m_editLabDeclaration.text = m_data.strDefultContent;
            }
         

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildHasDismissed, _OnGuildDismissed);
        }

        protected override void _OnCloseFrame()
        {
            m_data = null;

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildHasDismissed, _OnGuildDismissed);
        }

        void _OnGuildDismissed(UIEvent a_event)
        {
            frameMgr.CloseFrame(this);
        }
    }
}
