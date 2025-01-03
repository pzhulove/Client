using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ChangeJobFinishView : MonoBehaviour
    {
        [SerializeField] private List<Image> mImgNames;
        [SerializeField] private Text mTextDesp;
        [SerializeField] private Text mTextSubDesp;
        [SerializeField] private Text mTextWeapon;
        [SerializeField] private Text mTextWore;
        [SerializeField] private GameObject mSpineRoot;
        [SerializeField] private GeObjectRenderer mObjRender;
        private GameObject mObjJobImage;
        [SerializeField] private GameObject mObjTalent;
        [SerializeField] private GameObject mObjSkill;
        [SerializeField] private List<GameObject> mObjSkillItem;
        [SerializeField] private List<Image> mImgSkillItem;
        //当前职业
        private int mCurJobId = 0;
        //之前的预转职职业
        private int mPreJob = 0;
        //是否是转职（不是则对应切换职业）
        private bool mIsJobMission = true;
        public void OnInit()
        {
            mCurJobId = PlayerBaseData.GetInstance().JobTableID;
            mPreJob = PlayerBaseData.GetInstance().PreChangeJobTableID;
            mIsJobMission = ChangeJobSelectFrame.changeType == ChangeJobType.ChangeJobMission;
            _SetJobInfo();
        }

        public void OnUninit()
        {

        }

        private void _SetJobInfo()
        {
            JobTable tableData = TableManager.GetInstance().GetTableItem<JobTable>(mCurJobId);
            //描述
            mTextDesp.text = TR.Value("creat_role_character_desc", tableData.RecommendedAttribute);
            //子描述
            mTextSubDesp.text = tableData.JobDes[0];
            //武器
            mTextWeapon.SafeSetText(tableData.RecWeapon);
            //衣服
            mTextWore.SafeSetText(tableData.RecDefence);
            //名字
            int index = 0;
            for(; index < tableData.JobNameImgPaths.Length; ++index)
            {
                if (index >= mImgNames.Count)
                {
                    Logger.LogErrorFormat("转职界面职业名字过长 预制体中准备字体不足");
                    break;
                }
                mImgNames[index].CustomActive(true);
                mImgNames[index].SafeSetImage(tableData.JobNameImgPaths[index]);
            }
            for(; index < mImgNames.Count; ++index)
            {
                mImgNames[index].CustomActive(false);
            }
            //动画
            if (mObjJobImage != null)
            {
                GameObject.DestroyImmediate(mObjJobImage);
                mObjJobImage = null;
            }
            if (tableData.JobImage.Contains("Animation"))
            {
                _ShowModule(tableData.ID, tableData.JobImage);
                mSpineRoot.SetActive(false);

                ClientSystemManager.GetInstance().delayCaller.DelayCall(100, () =>
                {
                    mSpineRoot.CustomActive(true);
                });
            }
            else
            {
                _HideModule();
                mObjJobImage = AssetLoader.instance.LoadResAsGameObject(tableData.JobImage);
                if (mObjJobImage != null && mSpineRoot != null)
                {
                    mSpineRoot.CustomActive(true);
                    Utility.AttachTo(mObjJobImage, mSpineRoot);

                    mObjJobImage.transform.SetAsFirstSibling();
                }
            }
            //新增技能
            _ShowSkillList();
            //天赋系统
            mObjTalent.CustomActive(mIsJobMission);
        }
        //技能显示
        private void _ShowSkillList()
        {
            JobTable tableData = TableManager.GetInstance().GetTableItem<JobTable>(mCurJobId);
            mObjSkill.CustomActive(mCurJobId != mPreJob);
            if (mCurJobId != mPreJob)
            {
                int index = 0;
                for(; index < tableData.changeFinishShowSkills.Count; ++index)
                {
                    mObjSkillItem[index].CustomActive(true);
                    SkillTable skill = TableManager.instance.GetTableItem<SkillTable>(tableData.changeFinishShowSkills[index]);
                    mImgSkillItem[index].SafeSetImage(skill.Icon);
                }
                for(; index < mObjSkillItem.Count; ++index)
                {
                    mObjSkillItem[index].CustomActive(false);
                }
            }
        }

        void _ShowModule(int jobID, string path)
        {
            if (mObjRender != null)
            {
                mObjRender.gameObject.CustomActive(true);
                mObjRender.ClearObject();

                try
                {
                    mObjRender.LoadObject(path, 28);
                }
                catch (System.Exception e)
                {
                    Logger.LogErrorFormat("create spineModule failed: {0}", e.ToString());
                }
            }
        }
        void _HideModule()
        {
            if (mObjRender != null)
            {
                mObjRender.gameObject.CustomActive(false);
            }
        }

        //查看技能
        public void OnClickGoToSkill()
        {
            ClientSystemManager.GetInstance().CloseFrame<ChangeJobFinish>();
            ClientSystemManager.GetInstance().OpenFrame<SkillFrame>();
        }
    }
}
