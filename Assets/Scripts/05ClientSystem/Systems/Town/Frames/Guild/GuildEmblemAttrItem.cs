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
    public class GuildEmblemAttrItem : MonoBehaviour
    {
        [SerializeField]
        TextEx nameText = null;

        [SerializeField]
        TextEx stageLvText = null;

        [SerializeField]
        GameObject attrTemplate = null;

        [SerializeField]
        GameObject attrParent = null;

        [SerializeField]
        bool isLevelUp = false;

        [SerializeField]
        ScrollRect scrollRect = null;

        //[SerializeField]
        static Color[] stageLvNameColors = new Color[]
        {
            Color.white,
            new Color(0.42f,0.75f,0.86f),
            new Color(0.75f,0.27f,0.93f),
        };

        static string[] lvString = new string[] {
            "·一阶",
            "·二阶",
            "·三阶",
            "·四阶",
            "·五阶",
            "·六阶",
            "·七阶",
            "·八阶",
            "·九阶",
            "·十阶"};

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

        private string GetColorString(string text, string color)
        {
            return TR.Value("common_color_text", "#" + color, text);
        }

        public static Color GetStageNameColor(GuildEmblemTable guildEmblemTable)
        {
            if (guildEmblemTable == null || stageLvNameColors == null)
            {
                return Color.white;
            }

            if (guildEmblemTable.needHonourLevel - 1 < 0 || guildEmblemTable.needHonourLevel - 1 >= stageLvNameColors.Length)
            {
                return Color.white;
            }

            return stageLvNameColors[guildEmblemTable.needHonourLevel - 1];
        }

        public static string GetStageString(int iEmvleLv)
        {
            if (lvString == null)
            {
                return "";
            }

            GuildEmblemTable guildEmblemTable = TableManager.GetInstance().GetTableItem<GuildEmblemTable>(iEmvleLv);
            if (guildEmblemTable == null)
            {
                return "";
            }

            if (guildEmblemTable.stageLevel > lvString.Length || guildEmblemTable.stageLevel <= 0)
            {
                return "";
            }

            return lvString[guildEmblemTable.stageLevel - 1];
        }

        public void SetUp(int iEmblemLv)
        {
            GuildEmblemTable guildEmblemTable = TableManager.GetInstance().GetTableItem<GuildEmblemTable>(iEmblemLv);
            if(guildEmblemTable == null)
            {
                return;
            }

            nameText.SafeSetText(guildEmblemTable.name);
            nameText.SafeSetColor(GetStageNameColor(guildEmblemTable));
            stageLvText.SafeSetText(GetStageString(iEmblemLv));

            // 刷新属性条目
            UpdateAttrs(iEmblemLv);

            return;
        }
        
        string GetAttrValueStr(List<string> skillDesList,string attrName)
        {
            if(skillDesList == null)
            {
                return "";
            }

            for (int i = 0; i < skillDesList.Count; i++)
            {
                string[] StrArray = skillDesList[i].Split(new char[] { ':' });

                if (StrArray.Length < 2)
                {
                    continue;
                }

                string strName = StrArray[0];
                string strValue = StrArray[1];

                if(attrName == strName)
                {
                    return strValue;
                }
            }
                return "";
        }

        void UpdateAttrs(int iEmblemLv)
        {
            if(attrParent == null)
            {
                return;
            }

            if(attrTemplate == null)
            {
                return;
            }

            for (int i = 0; i < attrParent.transform.childCount; ++i)
            {
                GameObject go = attrParent.transform.GetChild(i).gameObject;
                GameObject.Destroy(go);
            }

            int nSkillID = GuildDataManager.GetInstance().GetEmblemSkillID();
            //nSkillID = 10203;
            List<string> skillDesList = SkillDataManager.GetInstance().GetSkillDesList(nSkillID, (byte)iEmblemLv);

            List<string> skillDesListPreLv = null;
            if(isLevelUp)
            {
                skillDesListPreLv = SkillDataManager.GetInstance().GetSkillDesList(nSkillID, (byte)(iEmblemLv - 1));
            }

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
                if(iValue == 0)
                {
                    continue;
                }

                GameObject goCurrent = GameObject.Instantiate(attrTemplate.gameObject);
                Utility.AttachTo(goCurrent, attrParent);
                goCurrent.CustomActive(true);

                ComCommonBind bind = goCurrent.GetComponent<ComCommonBind>();
                if (bind != null)
                {
                    StaticUtility.SafeSetText(bind, "attrName", attrName + ":");
                    StaticUtility.SafeSetText(bind, "attrValue", attrValue);
                    StaticUtility.SafeSetVisible<Image>(bind, "up", false);

                    if(isLevelUp)
                    {
                        string attrValuePre = GetAttrValueStr(skillDesListPreLv, attrName);

                        int iPre = 0;
                        int iNow = 0;
                        int.TryParse(attrValuePre, out iPre);
                        int.TryParse(attrValue,out iNow);

                        StaticUtility.SafeSetVisible<Image>(bind, "up", iNow > iPre);
                    }
                }                
            }

            if(scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 1.0f;
            }
        }
    }
}


