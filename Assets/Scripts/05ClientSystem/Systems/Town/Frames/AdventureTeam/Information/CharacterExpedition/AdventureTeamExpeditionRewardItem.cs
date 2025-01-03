using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{

    public class AdventureTeamExpeditionRewardItem : MonoBehaviour
    {
        [SerializeField] private GameObject mRichObj;
        [SerializeField] private Text mTitleText;
        [SerializeField] private Text mRewardText;
        //[SerializeField] private Text mNumText;
        [SerializeField] private GameObject mItemRoot;

        [SerializeField] private ExpeditionMapModel tempInfo;
        [SerializeField] private int tempIndex;
        [SerializeField] private ExpeditionRewardCondition tempCondition;

        private string tr_expedition_base_reward = "";
        private string tr_expedition_extra_reward = "";
        private ComItem _rewardComItem = null;

        private void Awake()
        {
            _InitTR();
            ClearData();
        }

        private void OnDestroy()
        {
            _ClearTR();
            ClearData();
            if (_rewardComItem)
            {
                ComItemManager.Destroy(_rewardComItem);
                _rewardComItem = null;
            }
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

        public void UpdateExpeditionMapBaseDate()
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

            ItemTable mTempItem = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);

            //加载图片 根据表格信息
            //if(mTempItem != null)
            //{
            //    ETCImageLoader.LoadSprite(ref mItemImage, mTempItem.Icon);
            //}
            if (mItemRoot && mTempItem != null)
            {
                ItemData rewardItemData = ItemDataManager.CreateItemDataFromTable((int)itemId);
                if (!_rewardComItem)
                {
                    _rewardComItem = ComItemManager.Create(mItemRoot);
                }
                if(rewardItemData != null)
                {
                _rewardComItem.Setup(rewardItemData, (var1,var2) => 
                {
                    ItemTipManager.GetInstance().ShowTip(var2, null, TextAnchor.MiddleCenter, true);
                });
                }
                _rewardComItem.SetCountFormatter((var) =>
                {
                    if (num <= 0)
                    {
                        return "";
                    }
                    return num.ToString();
                });
                _rewardComItem.CustomActive(true);
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
                string condition = AdventureTeamDataManager.GetInstance().TryGetExpeditionMapRewardConition((int)tempCondition);
                mRewardText.text = string.Format(condition, 0, tempInfo.rewardList[tempIndex].rolesNum);
            }
        }

        public void OnExpeditionTimeChanged()
        {
            uint mutiple = AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.durationOfExpedition;
            int num = 0;

            if (!string.IsNullOrEmpty(tempInfo.rewardList[tempIndex].rewards))
            {
                string[] tempReward = tempInfo.rewardList[tempIndex].rewards.Split(':');
                if (tempReward.Length != 0)
                {
                    int.TryParse(tempReward[1], out num);
                }
            }
            if (_rewardComItem)
            {
                _rewardComItem.SetCountFormatter((var) =>
                {
                    if (num <= 0)
                    {
                        return "";
                    }
                    return (num * mutiple).ToString();
                });
            }
        }

        public void  OnExpeditionRolesChanged()
        {
            List<ExpeditionMemberInfo> tempRoles = AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.roles;
            if (tempRoles == null) return;
            //根据条件及派遣队成员判断
            int reach = 0;
            int total = tempInfo.rewardList[tempIndex].rolesNum;
            string condition = AdventureTeamDataManager.GetInstance().TryGetExpeditionMapRewardConition((int)tempCondition);

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

                    reach = AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.roles.Count;
                    isReach = UpdateRewardText(reach, total, condition);
                    break;
                case ExpeditionRewardCondition.REQUIRE_ANY_SAME_BASE_OCCU:

                    reach = AdventureTeamDataManager.GetInstance().IsAnySameBaseOccu(occuList.ToArray());
                    isReach = UpdateRewardText(reach, total, condition);
                    break;
                case ExpeditionRewardCondition.REQUIRE_ANY_DIFF_BASE_OCCU:

                    reach = AdventureTeamDataManager.GetInstance().IsAnyDiffBaseOccu(occuList.ToArray());
                    isReach = UpdateRewardText(reach, total, condition);
                    break;
                case ExpeditionRewardCondition.REQUIRE_ANY_DIFF_CHANGED_OCCU:

                    reach = AdventureTeamDataManager.GetInstance().IsAnyDiffChangedOccu(occuList.ToArray());
                    isReach = UpdateRewardText(reach, total, condition);
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

        private bool UpdateRewardText(int reach,int total,string condition)
        {
            if(reach >= total)
            {
                mRewardText.color = Color.yellow;
                mRewardText.text = string.Format(condition, reach, total);
                return true;
            }
            else
            {
                mRewardText.color = Color.white;
                mRewardText.text = string.Format(condition, reach, total);
                return false;
            }

        }

        public bool IsReach()
        {
            return mRewardText.color == Color.yellow;
        }

        public void OnItemRecycle()
        {
            ClearData();
        }

        private void ClearData()
        {
            tempCondition = 0;
            tempIndex = 0;
            tempInfo = null;
        }

        
    }
}
