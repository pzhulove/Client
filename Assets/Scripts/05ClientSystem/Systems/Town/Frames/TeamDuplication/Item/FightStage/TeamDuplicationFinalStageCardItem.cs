using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;
using Scripts.UI;

namespace GameClient
{
    //大阶段翻牌的CardItem
    public class TeamDuplicationFinalStageCardItem : MonoBehaviour
    {
        //卡牌展开的背面特效
        private string rewardItemCoverEffectPath = "Effects/Scene_effects/EffectUI/chouka/EffUI_chouka_tuanben_zhankai";

        //金牌翻牌特效和粒子特效
        private string goldCardActionPath = "Effects/Scene_effects/EffectUI/chouka/EffUI_chouka_tuanben_jinpai";
        private string goldCardEffectPath =
            "Effects/Scene_effects/EffectUI/chouka/EffUI_chouka_tuanben_guangxiao_jinpai";

        //普通卡牌翻牌特效和粒子特效
        private string normalCardActionPath = "Effects/Scene_effects/EffectUI/chouka/EffUI_chouka_tuanben";
        private string normalCardEffectPath = "Effects/Scene_effects/EffectUI/chouka/EffUI_chouka_tuanben_guangxiao";
        
        //特殊的特效（用于金牌的翻牌结束之后的展示）
        private string specialCardEffectPath = "Effects/Scene_effects/EffectUI/chouka/EffUI_chouka_tuanben_ziji";

        //奖励ItemView的路径
        private string rewardItemViewPath =
            "UIFlatten/Prefabs/TeamDuplication/View/TeamDuplicationFightStageRewardItemView";

        private TeamDuplicationFightStageRewardDataModel _finalStageRewardDataModel;
        private TeamDuplicationFinalStageRewardItem _finalStageRewardItem;

        private bool _isBeginRewardAction = false;
        private float _rewardItemActionDuringTime = 0.0f;          //奖励翻牌动画持续时间

        [Space(10)] [HeaderAttribute("cardButton")] [Space(10)]
        [SerializeField] private GameObject rewardItemCover;
        [SerializeField] private GameObject rewardItemCoverEffectRoot;
        //翻盘特效
        [SerializeField] private GameObject rewardItemActionRoot;
        [SerializeField] private GameObject rewardItemEffectRoot;

        //金牌正面的展示特效
        [Space(10)] [HeaderAttribute("OwnerCardGameObject")] [Space(10)]
        [SerializeField] private GameObject ownerRewardItemEffectRoot;

        [Space(10)]
        [HeaderAttribute("rewardItemRoot")]
        [Space(10)]
        [SerializeField] private GameObject rewardItemRoot;

        private void Awake()
        {
        }

        private void OnDestroy()
        {
            ClearData();
        }


        private void ClearData()
        {
            _finalStageRewardDataModel = null;
            _finalStageRewardItem = null;
        }

        //加载卡牌展开的特效
        public void LoadStageCardCoverEffect()
        {
            if (rewardItemCoverEffectRoot == null)
                return;

            CommonUtility.LoadGameObjectWithPath(rewardItemCoverEffectRoot, rewardItemCoverEffectPath);
        }

        //初始化
        public void Init(TeamDuplicationFightStageRewardDataModel fightStageRewardDataModel)
        {
            _finalStageRewardDataModel = fightStageRewardDataModel;
        
            ShowRewardItemView();
        }

        //展示卡牌
        private void ShowRewardItemView()
        {
            //卡牌翻转得到动画和特效

            //奖励不受限制，并且是金牌，
            //设置金牌翻牌特效的路径
            if (_finalStageRewardDataModel.IsLimit == TeamCopyFlopLimit.TEAM_COPY_FLOP_LIMIT_NULL
                && _finalStageRewardDataModel.IsGoldReward == true)
            {
                CommonUtility.SetGameObjectLoadPath(rewardItemActionRoot, goldCardActionPath);
                CommonUtility.SetGameObjectLoadPath(rewardItemEffectRoot, goldCardEffectPath);
            }
            else
            {
                //设置普通卡牌的路径
                CommonUtility.SetGameObjectLoadPath(rewardItemActionRoot, normalCardActionPath);
                CommonUtility.SetGameObjectLoadPath(rewardItemEffectRoot, normalCardEffectPath);
            }

            //设置特殊的卡牌特效的路径
            CommonUtility.SetGameObjectLoadPath(ownerRewardItemEffectRoot, specialCardEffectPath);
            //设置卡牌奖励的路径
            CommonUtility.SetGameObjectLoadPath(rewardItemRoot, rewardItemViewPath);

            //隐藏Cover
            CommonUtility.UpdateGameObjectVisible(rewardItemCover, false);

            //初始化奖励
            if (_finalStageRewardItem == null)
            {
                _finalStageRewardItem = LoadFinalStageRewardItem(rewardItemRoot);
            }

            if (_finalStageRewardItem != null)
                _finalStageRewardItem.UpdateRewardItem(_finalStageRewardDataModel);


            //首先隐藏
            CommonUtility.UpdateGameObjectVisible(rewardItemRoot, false);
            _isBeginRewardAction = true;
            _rewardItemActionDuringTime = TeamDuplicationDataManager.TeamDuplicationRewardItemActionDuringTime;

            //加载翻转动画和特效
            CommonUtility.LoadGameObject(rewardItemActionRoot);
            CommonUtility.LoadGameObject(rewardItemEffectRoot);
        }

        //加载RewardItem
        private TeamDuplicationFinalStageRewardItem LoadFinalStageRewardItem(GameObject contentRoot)
        {
            //加载预制体
            var rewardItemPrefab = CommonUtility.LoadGameObject(contentRoot);

            if (rewardItemPrefab == null)
                return null;

            var finalStageRewardItem = rewardItemPrefab.GetComponent<TeamDuplicationFinalStageRewardItem>();
            
            return finalStageRewardItem;
        }

        public TeamDuplicationFightStageRewardDataModel GetFightStageRewardDataModel()
        {
            return _finalStageRewardDataModel;
        }

        private void Update()
        {
            if (_isBeginRewardAction == false)
                return;

            _rewardItemActionDuringTime -= Time.deltaTime;
            //动画播放完全，展示奖励
            if (_rewardItemActionDuringTime <= 0.0f)
            {
                ShowRewardItemByEffectPlayFinish();
            }
        }

        //特效播放完成，展示奖励的内容
        private void ShowRewardItemByEffectPlayFinish()
        {
            CommonUtility.UpdateGameObjectVisible(rewardItemRoot, true);
            _isBeginRewardAction = false;

            ////如果是自己的奖励，加载自己的特效
            //if (_finalStageRewardDataModel.PlayerGuid == PlayerBaseData.GetInstance().RoleID)
            //{
            //    TeamDuplicationUtility.LoadRewardItemEffect(ownerRewardItemEffectRoot);
            //}

            //奖励不受限制，并且是金牌，加载金牌特殊的特效，卡牌周围转圈特效
            if (_finalStageRewardDataModel.IsLimit == TeamCopyFlopLimit.TEAM_COPY_FLOP_LIMIT_NULL
                && _finalStageRewardDataModel.IsGoldReward == true)
            {
                CommonUtility.LoadGameObject(ownerRewardItemEffectRoot);
            }

        }
    }
}
