using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AdventureTeamExpeditionOnekeyRewardItem : MonoBehaviour
    {
        [SerializeField] private GameObject mRichObj;
        [SerializeField] private Text mTitleText;
        [SerializeField] private Image mRewardIcon;
        [SerializeField] private Text mRewardText;

        [SerializeField] private Color highlightColor = Color.white;
        [SerializeField] private Color normalColor = Color.white;

        private ExpeditionMapModel tempInfo;
        private int tempIndex;
        private ExpeditionRewardCondition tempCondition;

        private string tr_expedition_base_reward = "";
        private string tr_expedition_extra_reward = "";

        private void Awake()
        {
            _InitTR();
            _ClearData();
        }

        private void OnDestroy()
        {
            _ClearTR();
            _ClearData();
        }

        private void _InitTR()
        {
            tr_expedition_base_reward = TR.Value("adventure_team_expedition_base_reward_text");
            tr_expedition_extra_reward = TR.Value("adventure_team_expedition_extra_reward_text");
        }

        private void _ClearTR()
        {
            tr_expedition_base_reward = "";
            tr_expedition_extra_reward = "";
        }

        public void InitItemView(int index, ExpeditionMapModel mapInfo, ExpeditionRewardCondition Condition)
        {
            tempIndex = index;
            tempInfo = mapInfo;
            tempCondition = mapInfo.rewardList[index].rewardCondition;
        }

        public void UpdateExpeditionMapBaseData()
        {
            int itemId = 600002535;
            int num = 0;

            if (!string.IsNullOrEmpty(tempInfo.rewardList[tempIndex].rewards))
            {
                string[] tempReward = tempInfo.rewardList[tempIndex].rewards.Split(':');
                if (tempReward.Length != 0)
                {
                    int.TryParse(tempReward[0], out itemId);
                    int.TryParse(tempReward[1], out num);
                }
            }

            ProtoTable.ItemTable mTempItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(itemId);

            if (mRewardIcon != null)
            {
                ETCImageLoader.LoadSprite(ref mRewardIcon, mTempItem.Icon);
            }
            if (mTitleText)
            {
                if (tempIndex == 0)
                {
                    mTitleText.text = tr_expedition_base_reward;
                }
                else
                {
                    mTitleText.text = string.Format(tr_expedition_extra_reward, tempIndex);
                }
            }
            if (mRewardText)
            {
                mRewardText.SafeSetText(TR.Value("adventure_team_expeidtion_dispatch_reward_count", num.ToString()));
            }
        }

        public void OnExpeditionTimeChanged()
        {
            if (tempInfo == null || tempInfo.mapNetInfo == null)
            {
                return;
            }
            int num = 0;
            if (!string.IsNullOrEmpty(tempInfo.rewardList[tempIndex].rewards))
            {
                string[] tempReward = tempInfo.rewardList[tempIndex].rewards.Split(':');
                if (tempReward.Length != 0)
                {
                    int.TryParse(tempReward[1], out num);
                }
            }
            var multipleNum = num * tempInfo.mapNetInfo.durationOfExpedition;
            if (mRewardText)
            {
                mRewardText.SafeSetText(TR.Value("adventure_team_expeidtion_dispatch_reward_count", multipleNum.ToString()));
            }
        }

        public void OnExpeditionRolesChanged()
        {
            if (tempInfo == null || tempInfo.mapNetInfo == null)
            {
                return;
            }
            var tempRoles = tempInfo.mapNetInfo.roles;
            if (tempRoles == null)
            {
                return;
            }
            //根据条件及派遣队成员判断
            int reach = 0;
            int total = tempInfo.rewardList[tempIndex].rolesNum;
            string condition = AdventureTeamDataManager.GetInstance().TryGetExpeditionMapRewardConition((int)tempCondition);

            int num = 0;
            if (!string.IsNullOrEmpty(tempInfo.rewardList[tempIndex].rewards))
            {
                string[] tempReward = tempInfo.rewardList[tempIndex].rewards.Split(':');
                if (tempReward.Length != 0)
                {
                    int.TryParse(tempReward[1], out num);
                }
            }

            List<int> occuList = new List<int>();
            bool isReach = false;
            for (int i = 0; i < tempRoles.Count; i++)
            {
                if (tempRoles[i] != null)
                {
                    occuList.Add(tempRoles[i].occu);
                }
            }
            switch (tempCondition)
            {
                case ExpeditionRewardCondition.REQUIRE_ANY_OCCU:

                    reach = tempRoles.Count;
                    isReach = _UpdateRewardText(reach, total, num);
                    break;
                case ExpeditionRewardCondition.REQUIRE_ANY_SAME_BASE_OCCU:

                    reach = AdventureTeamDataManager.GetInstance().IsAnySameBaseOccu(occuList.ToArray());
                    isReach = _UpdateRewardText(reach, total, num);
                    break;
                case ExpeditionRewardCondition.REQUIRE_ANY_DIFF_BASE_OCCU:

                    reach = AdventureTeamDataManager.GetInstance().IsAnyDiffBaseOccu(occuList.ToArray());
                    isReach = _UpdateRewardText(reach, total, num);
                    break;
                case ExpeditionRewardCondition.REQUIRE_ANY_DIFF_CHANGED_OCCU:

                    reach = AdventureTeamDataManager.GetInstance().IsAnyDiffChangedOccu(occuList.ToArray());
                    isReach = _UpdateRewardText(reach, total, num);
                    break;
                default:
                    break;
            }
            if (isReach)
            {
                mRichObj.SetActive(true);
            }
            else
            {
                mRichObj.SetActive(false);
            }
        }

        public void OnItemRecycle()
        {
            _ClearData();
        }

        private bool _UpdateRewardText(int reach,int total, int rewardNum)
        {           
            if(reach >= total)
            {
                if (mRewardText)
                {
                    //mRewardText.color = Color.yellow;
                    mRewardText.SafeSetText(TR.Value("adventure_team_expeidtion_dispatch_reward_count", rewardNum.ToString()));
                }
                if (mTitleText)
                {
                    mTitleText.color = highlightColor;
                }
                return true;
            }
            else
            {
                if (mRewardText)
                {
                    //mRewardText.color = Color.white;
                    mRewardText.SafeSetText(TR.Value("adventure_team_expeidtion_dispatch_reward_count", rewardNum.ToString()));
                }
                if (mTitleText)
                {
                    mTitleText.color = normalColor;
                }
                return false;
            }
        }

        private void _ClearData()
        {
            tempCondition = 0;
            tempIndex = 0;
            tempInfo = null;
        }
    }
}
