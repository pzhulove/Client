using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace GameClient
{
    public class PetDetailFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pet/PetDetailTips";
        }

        Regex m_regex = new Regex(@"([^\d]*)(\d+\.?\d*)(%?)$");

        protected override void _OnOpenFrame()
        {
            _RefreshData();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PetItemsInfoUpdate, _OnUpdatePetList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PetSlotChanged, _OnPetSlotChanged);
        }

        void _OnUpdatePetList(UIEvent uiEvent)
        {
            _RefreshData();
        }

        void _OnPetSlotChanged(UIEvent uiEvent)
        {
            _RefreshData();
        }

        void _RefreshData()
        {
            mDetailTitle.SafeSetText(TR.Value("pet_tips_head", PetDataManager.GetInstance().GetOnUsePetList().Count));
            mAttContents.SafeSetText( GetPetAttributesData());


            if (null == mSkillsRoot)
            {
                return;
            }

            for (int i = 0; i < mSkillsRoot.childCount; i++)
            {
                var obj = mSkillsRoot.GetChild(i).gameObject;
                obj.CustomActive(false);
            }

            var skillsList = GetPetSkillsData();

            for (int i = 0; i < skillsList.Count; i++)
            {
                var obj = mSkillsRoot.GetChild(i).gameObject;
                var data = skillsList[i];
                if (null == obj || null == data)
                {
                    continue;
                }

                GameObject petName = Utility.FindChild("PetName", obj);
                if (null != petName)
                {
                    petName.GetComponent<Text>().SafeSetText(data.petName);
                }

                GameObject skillName = Utility.FindChild("SkillName", obj);
                if (null != skillName)
                {
                    skillName.GetComponent<Text>().SafeSetText(data.skillName);
                }

                GameObject skillDesc = Utility.FindChild("SkillDesc", obj);
                if (null != skillDesc)
                {
                    skillDesc.GetComponent<Text>().SafeSetText(data.skillDesc);
                }

                obj.CustomActive(true);
            }
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PetItemsInfoUpdate, _OnUpdatePetList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PetSlotChanged, _OnPetSlotChanged);
        }

        class TipsUnitData
        {
            public float value;
            public string append;
        }

        private string GetPetAttributesData()
        {
            string attContent = "";
            var petInfos = PetDataManager.GetInstance().GetOnUsePetList();
            if (null != petInfos)
            {
                Dictionary<string, TipsUnitData> dictKeyValues = new Dictionary<string, TipsUnitData>();
                
                for (int i = 0; i < petInfos.Count; ++i)
                {
                    var petInfo = petInfos[i];
                    if (null == petInfo)
                    {
                        continue;
                    }

                    var petItem = TableManager.GetInstance().GetTableItem<ProtoTable.PetTable>((int)petInfo.dataId);
                    if (null == petItem)
                    {
                        continue;
                    }

                    var skillDescList = SkillDataManager.GetInstance().GetSkillDesList(petItem.InnateSkill, (byte)petInfo.level);
                    if (null != skillDescList)
                    {
                        for (int k = 0; k < skillDescList.Count; ++k)
                        {
                            if (string.IsNullOrEmpty(skillDescList[k]))
                            {
                                continue;
                            }

                            var match = m_regex.Match(skillDescList[k]);
                            if (!match.Success)
                            {
                                continue;
                            }

                            float fValue = 0.0f;
                            if (!float.TryParse(match.Groups[2].Value, out fValue))
                            {
                                continue;
                            }
                            string key = match.Groups[1].Value;

                            if (dictKeyValues.ContainsKey(key))
                            {
                                dictKeyValues[key].value += fValue;
                            }
                            else
                            {
                                dictKeyValues[key] = new TipsUnitData
                                {
                                    append = match.Groups[3].Value,
                                    value = 0.0f,
                                };
                                dictKeyValues[key].value = fValue;
                            }
                        }
                    }
                }

                var enumerator = dictKeyValues.GetEnumerator();
                
                while (enumerator.MoveNext())
                {
                    attContent += enumerator.Current.Key + string.Format("<color=#CFCFCFFF>{0}</color>", enumerator.Current.Value.value) + enumerator.Current.Value.append + "\n";
                }
            }
            return attContent;
        }


        class TipsSkillData
        {
            public string petName;
            public string skillName;
            public string skillDesc;
        }

        List<TipsSkillData> GetPetSkillsData()
        {
            List<TipsSkillData> skillsList = new List<TipsSkillData>();
            var petInfos = PetDataManager.GetInstance().GetOnUsePetList();
            if (null != petInfos)
            {
                for (int i = 0; i < petInfos.Count; ++i)
                {
                    var petInfo = petInfos[i];
                    if (null == petInfo)
                    {
                        continue;
                    }

                    int skillID = PetDataManager.GetPetSkillIDByJob((int)petInfo.dataId, PlayerBaseData.GetInstance().JobTableID, (int)petInfo.skillIndex);
                    var skillDescList = SkillDataManager.GetInstance().GetSkillDesList(skillID, (byte)petInfo.level);
                    if (null == skillDescList)
                    {
                        continue;
                    }

                    var skillItem = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>((int)skillID);
                    if (null == skillItem)
                    {
                        continue;
                    }

                    var petItem = TableManager.GetInstance().GetTableItem<ProtoTable.PetTable>((int)petInfo.dataId);
                    if (null == petItem)
                    {
                        continue;
                    }

                    TipsSkillData tipData = new TipsSkillData();
                    tipData.petName = PetDataManager.GetInstance().GetColorName(petItem.Name, petItem.Quality);
                    tipData.skillName = skillItem.Name;
                    tipData.skillDesc = "";

                    var curSkillTips = PetDataManager.GetInstance().GetPetCurSkillTips(petItem, PlayerBaseData.GetInstance().JobTableID, (int)petInfo.skillIndex, (int)petInfo.level, false).Split('\r', '\n');
                    for (int j = 0; j < curSkillTips.Length; ++j)
                    {
                        if (!string.IsNullOrEmpty(curSkillTips[j]))
                        {
                            tipData.skillDesc += curSkillTips[j] + "\n";
                        }
                    }
                    skillsList.Add(tipData);
                }
            }

            return skillsList;
        }

        #region ExtraUIBind
        private Text mDetailTitle = null;
        private Text mAttContents = null;
        private RectTransform mSkillsRoot = null;

        protected override void _bindExUI()
        {
            mDetailTitle = mBind.GetCom<Text>("DetailTitle");
            mAttContents = mBind.GetCom<Text>("AttContents");
            mSkillsRoot = mBind.GetCom<RectTransform>("SkillsRoot");
        }

        protected override void _unbindExUI()
        {
            mDetailTitle = null;
            mAttContents = null;
            mSkillsRoot = null;
        }
        #endregion
    }
}

