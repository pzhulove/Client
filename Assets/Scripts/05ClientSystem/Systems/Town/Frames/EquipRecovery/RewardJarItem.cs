using UnityEngine.UI;
using Scripts.UI;
using System.Collections.Generic;
using UnityEngine;
using EItemType = ProtoTable.ItemTable.eType;
using ProtoTable;

namespace GameClient
{
    class RewardJarItem
    {
        const string equipRecoverJarPath = "UIFlatten/Prefabs/EquipRecovery/EquipRecoveryJar";
        private RectTransform mJarRect = null;
        private Text mLabel = null;
        private GameObject mCheck = null;
        private GameObject mEffect = null;
        private Button mGetReward = null;
        private Image mBox = null;
        private int needScore;
        private int jarTableID;
        private GameObject mBlue = null;
        private GameObject mYellow = null;
        private GameObject mOpenEffect = null;
        private Image mRewardIcon = null;

        bool canOpen = false;
        RewardJarStatic curStatic = RewardJarStatic.UnOpen;
        //奖励积分为多少时，进度条是满的
        SystemValueTable jarPointNumData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_REQUIP_RECOVERY_JAR_MAX);
        
        public void CreateGo(GameObject parent, EquipRecoveryRewardTable tableData)
        {
            GameObject equipRecoverJarItem = AssetLoader.instance.LoadResAsGameObject(equipRecoverJarPath);
            if (equipRecoverJarItem == null)
            {
                return;
            }
            var mBind = equipRecoverJarItem.GetComponent<ComCommonBind>();
            if (mBind == null)
            {
                return;
            }
            Utility.AttachTo(equipRecoverJarItem, parent);
            mJarRect = mBind.GetCom<RectTransform>("JarRect");
            mLabel = mBind.GetCom<Text>("Label");
            mCheck = mBind.GetGameObject("Check");
            mEffect = mBind.GetGameObject("Effect");
            mGetReward = mBind.GetCom<Button>("GetReward");
            mBox = mBind.GetCom<Image>("Box");
            mBlue = mBind.GetGameObject("blue");
            mYellow = mBind.GetGameObject("yellow");
            mOpenEffect = mBind.GetGameObject("OpenEffect");
            mRewardIcon = mBind.GetCom<Image>("RewardIcon");
            int maxjarPointNum = jarPointNumData.Value;

            needScore = tableData.Integral;
            jarTableID = tableData.JarID;
            mLabel.text = tableData.Integral.ToString();

            mGetReward.onClick.RemoveAllListeners();
            mGetReward.onClick.AddListener(() =>
            {
                if(!canOpen)
                {
                    ClientSystemManager.GetInstance().OpenFrame<RewardShow>(FrameLayer.Middle, tableData.ShowID);
                }
                else
                {
                    EquipRecoveryDataManager.GetInstance()._OpenRewardJar(tableData.JarID);
                }
            });
            ETCImageLoader.LoadSprite(ref mBox, tableData.IconPath);
            if(tableData.IconPath2 == "-")
            {
                mRewardIcon.CustomActive(false);
            }
            else
            {
                mRewardIcon.CustomActive(true);
                ETCImageLoader.LoadSprite(ref mRewardIcon, tableData.IconPath2);
            }
            //保存罐子初始状态
            curStatic = EquipRecoveryDataManager.GetInstance()._GetJarState(tableData.ID);
        }
        public void UpdateRewardJar(int index)
        {
            RewardJarStatic jarStatic = EquipRecoveryDataManager.GetInstance()._GetJarState(index);
            mCheck.CustomActive(false);
            
            mYellow.CustomActive(false);
            mBlue.CustomActive(false);
            canOpen = false;

            switch (jarStatic)
            {
                case RewardJarStatic.UnOpen:
                    mBlue.CustomActive(true);
                    mEffect.CustomActive(false);
                    break;
                case RewardJarStatic.CanOpen:
                    if(curStatic != RewardJarStatic.CanOpen)
                    {
                        _TryPlayEffect();
                    }
                    mYellow.CustomActive(true);
                    canOpen = true;
                    mEffect.CustomActive(true);
                    break;
                case RewardJarStatic.HaveOpen:
                    mYellow.CustomActive(true);
                    mCheck.CustomActive(true);
                    mEffect.CustomActive(false);
                    break;
            }
            curStatic = jarStatic;
        }

        private void _TryPlayEffect()
        {
            if(mOpenEffect.activeSelf)
            {
                mOpenEffect.CustomActive(false);
            }
            mOpenEffect.CustomActive(true);
        }

        public int GetNeedScore()
        {
            return needScore;
        }

        public int GetJarID()
        {
            return jarTableID;
        }
    }
}
