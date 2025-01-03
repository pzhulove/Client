using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using Network;
using Protocol;

namespace GameClient
{
    // 公会领地城主属性item
    internal class GuildManorOwnerAttrAddUpItem : ComUIListTemplateItem
    {
        [SerializeField]
        int skillID = 0;

        [SerializeField]
        Text attrs = null;

        private void Start()
        {
            SetUp(skillID);
        }

        private void OnDestroy()
        {
            
        }

        public override void SetUp(object data)
        {
            int iSkillID = (int)data;
            attrs.SafeSetText(CalcSkillAttrDesc(iSkillID));
        }

        string CalcSkillAttrDesc(int skillID)
        {
            List<string> skillDesList = SkillDataManager.GetInstance().GetSkillDesList(skillID, 1);
            string attrStr = "";

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

                attrStr += string.Format("{0}+{1}", attrName, attrValue);
                attrStr += "  ";
            }

            return attrStr;
        }

    }
}
