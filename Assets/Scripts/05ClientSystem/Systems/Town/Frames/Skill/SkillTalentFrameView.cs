using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SkillTalentFrameView : MonoBehaviour
    {
        [SerializeField] private Image mImgSkillIcon;
        [SerializeField] private Text mTextSkillName;
        [SerializeField] private Text mTextSkillType;
        [SerializeField] private Text mTextTalentDesp;
        [SerializeField] private Text mTextBtnDesp;
        [SerializeField] private UIGray mGray;
        [SerializeField] private ComUIListScript mListScript;
        private int mCurSkillId;
        private Skill mSkillData;
        private TalentTable mSelectTalentTable;
        private List<TalentTable> mTalentDataList;
        public void OnInit(int skillId)
        {
            mCurSkillId = skillId;
            mSkillData = SkillDataManager.GetInstance().GetCurSkillInfoById(mCurSkillId);
            if (null == mSkillData)
            {
                Logger.LogErrorFormat("技能数据中没有找到skillid={0}的信息",mCurSkillId);
                return;
            }
            mTalentDataList = SkillDataManager.GetInstance().GetSkillTalentData(mCurSkillId);
            if (null == mTalentDataList || 0 == mTalentDataList.Count)
            {
                Logger.LogErrorFormat("天赋表中没有找到skillid={0}的天赋",mCurSkillId);
                return;
            }
            _SetSkillInfo();
            _SetTalentList();
        }

        public void OnUninit()
        {
            mListScript.UnInitialize();
        }

        public void OnTalentChanged()
        {
            mSkillData = SkillDataManager.GetInstance().GetCurSkillInfoById(mCurSkillId);
            OnTalentSelect(null);
        }

        //设置技能信息
        private void _SetSkillInfo()
        {
            var table = TableManager.instance.GetTableItem<SkillTable>(mCurSkillId);
            if (null == table)
                return;
            mImgSkillIcon.SafeSetImage(table.Icon);
            mTextSkillName.SafeSetText(TR.Value("skill_talent_cur_skill_name_lv", table.Name, mSkillData.level));
            mTextSkillType.SafeSetText(SkillDataManager.GetInstance().GetSkillType(table));
        }

        //设置天赋信息
        private void _SetTalentList()
        {
            mListScript.Initialize();
            mListScript.onItemVisiable = OnItemShow;
            mListScript.OnItemUpdate = OnItemShow;
            //反过来选 没找到就默认用第一个
            int index = mTalentDataList.Count - 1;
            for (; index > 0; --index)
            {
                if (mTalentDataList[index].ID == mSkillData.talentId)
                    break;
            }
            OnTalentSelect(mTalentDataList[index]);
        }

        private void OnItemShow(ComUIListElementScript item)
        {
            if (null == item || item.m_index >= mTalentDataList.Count)
                return;
            var script = item.GetComponent<SkillTalentItem>();
            if (null == script)
                return;
            script.OnInit(mTalentDataList[item.m_index], mSelectTalentTable.ID == mTalentDataList[item.m_index].ID, OnTalentSelect);
        }

        //选中天赋
        private void OnTalentSelect(TalentTable table)
        {
            if (null != table)
                mSelectTalentTable = table;
            mTextTalentDesp.SafeSetText(mSelectTalentTable.Desp);
            mListScript.SetElementAmount(mTalentDataList.Count);

            //pvp中无法使用 灰化
            if (!mSelectTalentTable.CanPvp && SkillDataManager.GetInstance().GetCurType() != Protocol.SkillConfigType.SKILL_CONFIG_PVE)
                mGray.enabled = true;
            else
                mGray.enabled = false;
            //是否选中 修改文字
            if (mSkillData.talentId == mSelectTalentTable.ID)
                mTextBtnDesp.SafeSetText(TR.Value("skill_talent_cancel_item"));
            else
                mTextBtnDesp.SafeSetText(TR.Value("skill_talent_select_item"));
        }

        //点击学习
        public void OnClickSelectTalent()
        {
            if (mSelectTalentTable.CanPvp && SkillDataManager.GetInstance().GetCurType() != Protocol.SkillConfigType.SKILL_CONFIG_PVE)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(string.Format(TR.Value("skill_talent_pvp_forbid")));
                return;
            }
            SkillDataManager.GetInstance().OnSendSetCurskillTalentReq(mCurSkillId, mSelectTalentTable.ID);
        }
    }
}
