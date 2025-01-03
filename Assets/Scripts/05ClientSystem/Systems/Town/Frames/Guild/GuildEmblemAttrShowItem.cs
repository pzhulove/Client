using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using DG.Tweening;

namespace GameClient
{
    public class GuildEmblemAttrShowItem : MonoBehaviour
    {
        [SerializeField]
        TextEx nameText = null;

        [SerializeField]
        TextEx stageLvText = null;

        [SerializeField]
        Text attrs = null;

        [SerializeField]
        Image getNext = null;

        [SerializeField]
        Image getNow = null;

        [SerializeField]
        Image icon = null; 

        // Use this for initialization
        void Start()
        {

        }

        private void OnDestroy()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetUp(int iEmblemLv)
        {
            GuildEmblemTable guildEmblemTable = TableManager.GetInstance().GetTableItem<GuildEmblemTable>(iEmblemLv);
            if (guildEmblemTable == null)
            {
                return;
            }

            nameText.SafeSetText(guildEmblemTable.name);
            nameText.SafeSetColor(GuildEmblemAttrItem.GetStageNameColor(guildEmblemTable));

            stageLvText.SafeSetText(GuildEmblemAttrItem.GetStageString(iEmblemLv));

            // 刷新属性条目
            UpdateAttrs(iEmblemLv);

            getNow.CustomActive(GuildDataManager.GetInstance().GetEmblemLv() == iEmblemLv);
            //getNext.CustomActive(GuildDataManager.GetInstance().GetEmblemLv() + 1 == iEmblemLv);           

            icon.SafeSetImage(GuildDataManager.GetInstance().GetEmblemIconPath(iEmblemLv));

            return;
        }

        void UpdateAttrs(int iEmblemLv)
        {
            if (attrs == null)
            {
                return;
            }

            int nSkillID = GuildDataManager.GetInstance().GetEmblemSkillID();
            List<string> skillDesList = SkillDataManager.GetInstance().GetSkillDesList(nSkillID, (byte)iEmblemLv);
            attrs.text = "";           

            for (int i = 0; i < skillDesList.Count; i++)
            {
                string[] StrArray = skillDesList[i].Split(new char[] { ':' });

                if (StrArray.Length < 2)
                {
                    continue;
                }

                string attrName = StrArray[0];
                string attrValue = StrArray[1];

                int iValue = 0;
                int.TryParse(attrValue, out iValue);
                if (iValue == 0)
                {
                    continue;
                }

                attrs.text += string.Format("{0}+{1}", attrName, attrValue);
                attrs.text += "  ";
            }

            attrs.color = Color.white;
            if(iEmblemLv == GuildDataManager.GetInstance().GetEmblemLv())
            {
                attrs.color = Color.green;
            }

            return;
        }
    }
}


