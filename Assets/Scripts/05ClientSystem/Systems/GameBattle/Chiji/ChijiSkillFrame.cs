using System;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using Protocol;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace GameClient
{
    public class ChijiSkillFrame : ClientFrame
    {
        private bool bInit = false;
        private byte bCurSelectSkillLv = 0;
        private Dictionary<int, SkillLevelAddInfo> pvpSkillLevelAddInfo = new Dictionary<int, SkillLevelAddInfo>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chiji/ChijiSkillFrame";
        }

        protected override void _OnOpenFrame()
        {
            InitData();

            BindUIEvent();

            _InitSkillListScrollListBind();
            _RefreshSkillListCount();

            _UpdateSkillBar(true);

            //mRightRoot.CustomActive(SkillDataManager.GetInstance().ChijiSkillList.Count > 0);

            bInit = true;
        }

        protected override void _OnCloseFrame()
        {
            UnBindUIEvent();
            _ClearData();
        }

        private void _ClearData()
        {
            bInit = false;
            bCurSelectSkillLv = 0;
            pvpSkillLevelAddInfo.Clear();
        }

        private void InitData()
        {
            var equips = PlayerBaseData.GetInstance().GetEquipedEquipments();

            pvpSkillLevelAddInfo = SkillDataManager.GetInstance().GetSkillLevelAddInfo(false, equips);
        }

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SkillBarChanged, _OnSkillBarChanged);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SkillBarChanged, _OnSkillBarChanged);
        }

        private void _OnSkillBarChanged(UIEvent iEvent)
        {
            _UpdateSkillBar();
        }

        void _InitSkillListScrollListBind()
        {
            mChijiSkillUIListScript.Initialize();

            mChijiSkillUIListScript.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    _UpdateSkillScrollListBind(item);
                }
            };

            mChijiSkillUIListScript.OnItemRecycle = (item) =>
            {
                ComCommonBind combind = item.GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    return;
                }

                Toggle tgSelect = combind.GetCom<Toggle>("tgSkill");
                tgSelect.onValueChanged.RemoveAllListeners();
            };
        }

        void _UpdateSkillScrollListBind(ComUIListElementScript item)
        {
            ComCommonBind combind = item.GetComponent<ComCommonBind>();
            if (combind == null)
            {
                return;
            }
            
            if (item.m_index < 0 || item.m_index >= SkillDataManager.GetInstance().ChijiSkillList.Count)
            {
                return;
            }
            
            Skill skillInfo = SkillDataManager.GetInstance().ChijiSkillList[item.m_index];

            // 徐鹏程说该被动技能比较特殊，不要显示出来
            if(skillInfo.id == 1919)
            {
                combind.gameObject.CustomActive(false);
                return;
            }

            SkillTable skillData = TableManager.GetInstance().GetTableItem<SkillTable>(skillInfo.id);
            if (skillData == null)
            {
                return;
            }

            Image skillIcon = combind.GetCom<Image>("skillIcon");
            Text Level = combind.GetCom<Text>("Level");
            Image allocate = combind.GetCom<Image>("allocate");
            Toggle tgSkill = combind.GetCom<Toggle>("tgSkill");
             
            if (skillIcon != null)
            {
                ETCImageLoader.LoadSprite(ref skillIcon, skillData.Icon);
            }

            if(Level != null)
            {
                Level.text = string.Format("Lv.{0}", skillInfo.level);
            }

            if(allocate != null)
            {
                List<SkillBarGrid> ChijiSkillBar = SkillDataManager.GetInstance().ChijiSkillBar;

                bool bEquiped = false;
                for(int i = 0; i < ChijiSkillBar.Count; i++)
                {
                    if(ChijiSkillBar[i].id == skillInfo.id)
                    {
                        bEquiped = true;
                        break;
                    }
                }

                allocate.CustomActive(bEquiped);
            }

            if(tgSkill != null)
            {
                tgSkill.onValueChanged.RemoveAllListeners();
                int iIndex = item.m_index;
                tgSkill.onValueChanged.AddListener((value) => { _OnSelectSkill(iIndex, value); });

                if(!bInit)
                {
                    if(item.m_index == 0)
                    {
                        tgSkill.isOn = true;
                    }
                }
            }

            combind.gameObject.CustomActive(true);
        }

        void _RefreshSkillListCount()
        {
            if(SkillDataManager.GetInstance().ChijiSkillList != null)
            {
                mRightRoot.gameObject.CustomActive(SkillDataManager.GetInstance().ChijiSkillList.Count > 0);

                mChijiSkillUIListScript.SetElementAmount(SkillDataManager.GetInstance().ChijiSkillList.Count);
            }          
        }

        void _UpdateSkillBar(bool bIsiInit = false)
        {
            for(int i = 0; i < skillBarPosBind.Length; i++)
            {
                Toggle tgSkill = skillBarPosBind[i].GetCom<Toggle>("SkillBarElement");
                Image icon = skillBarPosBind[i].GetCom<Image>("Icon");
                Image plus = skillBarPosBind[i].GetCom<Image>("plus");
                Drag_Me dragme = skillBarPosBind[i].GetCom<Drag_Me>("dragme");
                Drop_Me dropme = skillBarPosBind[i].GetCom<Drop_Me>("dropme");

                int skillid = 0;

                for(int j = 0; j < SkillDataManager.GetInstance().ChijiSkillBar.Count; j++)
                {
                    SkillBarGrid skillBarGrid = SkillDataManager.GetInstance().ChijiSkillBar[j];

                    if ((i + 1) == skillBarGrid.slot)
                    {
                        skillid = skillBarGrid.id;
                        break;
                    }
                }

                SkillTable skillData = TableManager.GetInstance().GetTableItem<SkillTable>(skillid);

                if (plus != null)
                {
                    plus.CustomActive(skillData == null);
                }

                if (icon != null)
                {
                    if(skillData == null)
                    {
                        icon.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                    }
                    else
                    {
                        icon.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    }
                }

                if(bIsiInit)
                {
                    if (dragme != null)
                    {
                        //dragme.index = i;
                        dragme.ResponseDrag = _DealSkillBarDrag;
                    }

                    if (dropme != null)
                    {
                        //dropme.slot = i;
                        //dropme.ResponseDrop = _DealSkillBarDrop;
                    }
                }
                
                if (skillData == null)
                {
                    continue;
                }

                if (icon != null)
                {
                    ETCImageLoader.LoadSprite(ref icon, skillData.Icon);
                }
            }
        }

        void _OnSelectSkill(int index, bool value)
        {
            if(index < 0 || !value)
            {
                return;
            }

            if(index >= SkillDataManager.GetInstance().ChijiSkillList.Count)
            {
                return;
            }

            Skill skillInfo = SkillDataManager.GetInstance().ChijiSkillList[index];

            _UpdateSelectedSkillInfo(skillInfo);
        }

        void _UpdateSelectedSkillInfo(Skill skillInfo)
        {
            SkillTable skillData = TableManager.GetInstance().GetTableItem<SkillTable>(skillInfo.id);
            if (skillData == null)
            {
                return;
            }

            if (mName != null)
            {
                mName.text = skillData.Name;
            }

            if (mSkillIcon != null)
            {
                ETCImageLoader.LoadSprite(ref mSkillIcon, skillData.Icon);
            }

            if (mRightLevel != null)
            {
                mRightLevel.text = string.Format("Lv.{0}", skillInfo.level);
            }

            if (mSkillType != null)
            {
                mSkillType.text = SkillDataManager.GetInstance().GetSkillType(skillData);
            }

            if(mJobtip != null)
            {
                bool bAdapt = SkillDataManager.GetInstance().IsSkillJobAdaptToTargetJob(skillData, PlayerBaseData.GetInstance().JobTableID);
                mJobtip.CustomActive(!bAdapt);
            }

            if (mRightContentDes != null)
            {
                mRightContentDes.text = SkillDataManager.GetInstance().GetSkillDescription(skillData);
            }

            bCurSelectSkillLv = skillInfo.level;

            UpdateSkillAttribute(skillData);

            UpdateSkillCDText(skillInfo.id, skillData, bCurSelectSkillLv);

            UpdateSkillConsumeMP(skillData, bCurSelectSkillLv);
        }

        /// <summary>
        /// 刷新技能MP消耗
        /// </summary>
        /// <param name="skillTable"></param>
        /// <param name="curSkillLv"></param>
        private void UpdateSkillConsumeMP(SkillTable skillTable, byte curSkillLv)
        {
            byte lv = 1;
            if (curSkillLv >= 1)
            {
                lv = curSkillLv;
            }

            float fMP = (float)TableManager.GetValueFromUnionCell(skillTable.MPCost, lv, false);

            if(mSkillConsumeMpText != null)
            mSkillConsumeMpText.text = string.Format("{0}", fMP);

            float fCrystalCost = (float)TableManager.GetValueFromUnionCell(skillTable.CrystalCost, lv, false);
            if (fCrystalCost > 0)
            {
                if (mSkillConsumeThingText != null)
                {
                    mSkillConsumeThingText.gameObject.CustomActive(true);
                    mSkillConsumeThingText.text = string.Format("{0}个", fCrystalCost);
                }
            }
            else
            {
                if(mSkillConsumeThingText != null)
                mSkillConsumeThingText.gameObject.CustomActive(false);
            }
        }

        /// <summary>
        /// 刷新技能CD
        /// </summary>
        /// <param name="skillID"></param>
        /// <param name="skillTable"></param>
        /// <param name="curSkillLevel"></param>
        private void UpdateSkillCDText(UInt16 skillID,SkillTable skillTable,byte curSkillLevel)
        {
            float fCollDownTimePVP;
            SkillLevelAddInfo kSkillLevelAddInfo = new SkillLevelAddInfo();
            kSkillLevelAddInfo = SkillDataManager.GetInstance().GetAddedSkillInfo(skillID, pvpSkillLevelAddInfo);
            if (kSkillLevelAddInfo == null)
            {
                if (curSkillLevel > 0)
                {
                    fCollDownTimePVP = TableManager.GetValueFromUnionCell(skillTable.RefreshTimePVP, curSkillLevel) / 1000f;
                }
                else
                {
                    fCollDownTimePVP = TableManager.GetValueFromUnionCell(skillTable.RefreshTimePVP, 1) / 1000f;
                }
            }
            else
            {
                if (curSkillLevel + kSkillLevelAddInfo.totalAddLevel > 0)
                {
                    fCollDownTimePVP = TableManager.GetValueFromUnionCell(skillTable.RefreshTimePVP, curSkillLevel + kSkillLevelAddInfo.totalAddLevel) / 1000f;
                }
                else
                {
                    fCollDownTimePVP = TableManager.GetValueFromUnionCell(skillTable.RefreshTimePVP, 1) / 1000f;
                }
            }

            if(mSkillCDNum != null)
            mSkillCDNum.text = string.Format("{0}S", fCollDownTimePVP);
        }

        /// <summary>
        /// 刷新技能属性
        /// </summary>
        /// <param name="skillTable"></param>
        private void UpdateSkillAttribute(SkillTable skillTable)
        {
            for (int i = 0; i < attributeGos.Length; i++)
            {
                attributeGos[i].gameObject.CustomActive(false);
            }

            if (skillTable == null)
            {
                return;
            }

            if (skillTable.SkillEffect == null)
            {
                return;
            }

            for (int i = 0; i < skillTable.SkillEffect.Length; i++)
            {
                if (i >= attributeGos.Length)
                {
                    continue;
                }

                string str = GetSkillTypeText((byte)skillTable.SkillEffect[i]);
                if (str != "")
                {
                    attributeTypeTexts[i].text = str;
                    attributeGos[i].gameObject.CustomActive(true);
                }
            }
        }

        private bool _DealSkillBarDrag(PointerEventData DragData)
        {
            GameObject DragObj = DragData.lastPress;
            if(DragObj == null)
            {
                return false;
            }

            GameObject DragGo = DragObj.transform.parent.gameObject;
            if(DragGo == null)
            {
                return false;
            }

            ComCommonBind bind = DragGo.GetComponent<ComCommonBind>();
            if(bind == null)
            {
                return false;
            }

            Drag_Me dragme = bind.GetCom<Drag_Me>("dragme");
            if(dragme == null)
            {
                return false;
            }

            bool bFind = false; 
            for(int i = 0; i < SkillDataManager.GetInstance().ChijiSkillBar.Count; i++)
            {
                //if((dragme.index + 1) == SkillDataManager.GetInstance().ChijiSkillBar[i].slot)
                {
                    bFind = true;
                    break;
                }
            }

            if(!bFind)
            {
                return false;
            }

            return true;
        }

        private void _DealSkillBarDrop(PointerEventData DragData)
        {
            GameObject DragObj = DragData.lastPress;
            if (DragObj == null)
            {
                return;
            }

            GameObject DragGo = DragObj.transform.parent.gameObject;
            if (DragGo == null)
            {
                return;
            }

            ComCommonBind bind = DragGo.GetComponent<ComCommonBind>();
            if (bind == null)
            {
                return;
            }

            Drag_Me dragme = bind.GetCom<Drag_Me>("dragme");
            if (dragme == null)
            {
                return;
            }

            //             GameObject DropGo = BeDropedImgParent.transform.parent.gameObject;
            GameObject DropGo = null;
            if (DropGo == null)
            {
                return;
            }

            ComCommonBind Dropbind = DropGo.GetComponent<ComCommonBind>();
            if (Dropbind == null)
            {
                return;
            }

            Drop_Me dropme = Dropbind.GetCom<Drop_Me>("dropme");
            if(dropme == null)
            {
                return;
            }

            //if(dragme.index == dropme.slot)
            {
                return;
            }

            SkillBarGrid dragskill = new SkillBarGrid();
            SkillBarGrid dropskill = new SkillBarGrid();

            //dragskill.slot = (byte)(dragme.index + 1);
           // dropskill.slot = (byte)(dropme.slot + 1);

            for (int i = 0; i < SkillDataManager.GetInstance().ChijiSkillBar.Count; i++)
            {
                if (dragskill.slot == SkillDataManager.GetInstance().ChijiSkillBar[i].slot)
                {
                    if (SkillDataManager.GetInstance().ChijiSkillBar[i].id != 0)
                    {
                        dragskill.id = SkillDataManager.GetInstance().ChijiSkillBar[i].id;
                    }
                }

                if (dropskill.slot == SkillDataManager.GetInstance().ChijiSkillBar[i].slot)
                {
                    dropskill.id = SkillDataManager.GetInstance().ChijiSkillBar[i].id;
                }
            }

            if(dragskill.id == 0)
            {
                return;
            }

            UInt16 temp = dragskill.id;
            dragskill.id = dropskill.id;
            dropskill.id = temp;

            SceneExchangeSkillBarReq req = new SceneExchangeSkillBarReq();

            req.skillBars.index = 1;

            req.skillBars.bar = new SkillBar[1];
            req.skillBars.bar[0] = new SkillBar();
            req.skillBars.bar[0].grids = new SkillBarGrid[2];

            req.skillBars.bar[0].index = 1;
            req.skillBars.bar[0].grids[0] = dropskill;
            req.skillBars.bar[0].grids[1] = dragskill;

            req.configType = (byte)SkillConfigType.SKILL_CONFIG_PVP;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        private string GetSkillTypeText(byte effectIndex)
        {
            string resultStr = "";
            switch (effectIndex)
            {
                case (byte)ProtoTable.SkillTable.eSkillEffect.START_SKILL:
                    resultStr = TR.Value("skill_start");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.CONTINUOUS_SKILL:
                    resultStr = TR.Value("skill_continuous");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.HURT_SKILL:
                    resultStr = TR.Value("skill_hurt");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.DISPLACEMENT_SKILL:
                    resultStr = TR.Value("displacement_skilll");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.CONTROL_SKILL:
                    resultStr = TR.Value("control_skill");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.GRAB_SKILL:
                    resultStr = TR.Value("grab_skill");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.DEFENSE_SKILL:
                    resultStr = TR.Value("defense_skill");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.ASSISTANT_SKILL:
                    resultStr = TR.Value("assistant_skill");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.PHYSICAL_SKILL:
                    resultStr = TR.Value("physical_skill");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.MAGIC_SKILL:
                    resultStr = TR.Value("magic_skill");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.NEAR_SKILL:
                    resultStr = TR.Value("near_skill");
                    break;
                case (byte)ProtoTable.SkillTable.eSkillEffect.FAR_SKILL:
                    resultStr = TR.Value("far_skill");
                    break;
            }
            return resultStr;
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
        }

        [UIControl("skillBarRoot/BarPos{0}", typeof(ComCommonBind), 1)]
        ComCommonBind[] skillBarPosBind = new ComCommonBind[12];

        private GameObject[] attributeGos = new GameObject[6];
        private Text[] attributeTypeTexts = new Text[6];

        #region ExtraUIBind
        private Button mClose = null;
        private ComUIListScript mChijiSkillUIListScript = null;
        private GameObject mRightRoot = null;
        private Text mName = null;
        private Image mSkillIcon = null;
        private Text mRightLevel = null;
        private Text mSkillType = null;
        private Text mJobtip = null;
        private Text mSkillCDNum = null;
        private Text mSkillConsumeMpText = null;
        private Text mSkillConsumeThingText = null;
        private Text mRightContentDes = null;

        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("Close");
            if (null != mClose)
            {
                mClose.onClick.AddListener(_onCloseButtonClick);
            }
            mChijiSkillUIListScript = mBind.GetCom<ComUIListScript>("ChijiSkillUIListScript");
            mRightRoot = mBind.GetGameObject("rightRoot");
            mName = mBind.GetCom<Text>("name");
            mSkillIcon = mBind.GetCom<Image>("skillIcon");
            mRightLevel = mBind.GetCom<Text>("rightLevel");
            mSkillType = mBind.GetCom<Text>("skillType");
            mJobtip = mBind.GetCom<Text>("jobtip");
            attributeGos[0] = mBind.GetGameObject("Type0");
            attributeGos[1] = mBind.GetGameObject("Type1");
            attributeGos[2] = mBind.GetGameObject("Type2");
            attributeGos[3] = mBind.GetGameObject("Type3");
            attributeGos[4] = mBind.GetGameObject("Type4");
            attributeGos[5] = mBind.GetGameObject("Type5");

            attributeTypeTexts[0] = mBind.GetCom<Text>("Text0");
            attributeTypeTexts[1] = mBind.GetCom<Text>("Text1");
            attributeTypeTexts[2] = mBind.GetCom<Text>("Text2");
            attributeTypeTexts[3] = mBind.GetCom<Text>("Text3");
            attributeTypeTexts[4] = mBind.GetCom<Text>("Text4");
            attributeTypeTexts[5] = mBind.GetCom<Text>("Text5");

            mSkillCDNum = mBind.GetCom<Text>("skillCDNum");
            mSkillConsumeMpText = mBind.GetCom<Text>("skillConsumeMPText");
            mSkillConsumeThingText = mBind.GetCom<Text>("skillConsumeThingText");
            mRightContentDes = mBind.GetCom<Text>("Content");
        }

        protected override void _unbindExUI()
        {
            if (null != mClose)
            {
                mClose.onClick.RemoveListener(_onCloseButtonClick);
            }
            mClose = null;
            mChijiSkillUIListScript = null;
            mRightRoot = null;
            mName = null;
            mSkillIcon = null;
            mRightLevel = null;
            mSkillType = null;
            mJobtip = null;
            attributeGos[0] = null;
            attributeGos[1] = null;
            attributeGos[2] = null;
            attributeGos[3] = null;
            attributeGos[4] = null;
            attributeGos[5] = null;

            attributeTypeTexts[0] = null;
            attributeTypeTexts[1] = null;
            attributeTypeTexts[2] = null;
            attributeTypeTexts[3] = null;
            attributeTypeTexts[4] = null;
            attributeTypeTexts[5] = null;

            mSkillCDNum = null;
            mSkillConsumeMpText = null;
            mSkillConsumeThingText = null;
            mRightContentDes = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}
