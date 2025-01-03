using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;
using UnityEngine.UI;
using System;

namespace GameClient
{
    public class MagicBoxFrame : ClientFrame
    {

        public class MagicBoxResultFrameData
        {
            public List<ItemData> itemRewards;
            public List<ItemData> ItemDoubleRewards;

        }

        MagicBoxResultFrameData m_kData;

        [UIControl("UseAll_8/LeftItemList/Scroll View")]
        ComUIListScript m_comGetItemList;

        [UIControl("UseAll_8/RightItemList/Scroll View")]
        ComUIListScript m_comGetDoubleItemList;

        [UIControl("RewardItem/Scroll View")]
        ComUIListScript m_comGetItemListReward;

        [UIControl("DoubleRewardItem/Scroll View")]
        ComUIListScript m_comGetDoubleListDoubleReward;

        [UIControl("UseAll_8")]
        RectTransform useAll_8Rect;

        [UIControl("RewardItem")]
        RectTransform rewardItemRect;

        [UIControl("DoubleRewardItem")]
        RectTransform doubleRewardItemRect;

        [UIControl("UseAll_8/Text")]
        Text useAllText;

        [UIControl("RewardItem/Text")]
        Text rewardItemText;

        [UIControl("DoubleRewardItem/Text")]
        Text doubleRewardText;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Jar/MagicboxFrame";
        }


        protected override void _OnOpenFrame()
        {
            ShowItem();
        }

        protected override void _OnCloseFrame()
        {
            m_kData = null;
            MagicBoxDataManager.GetInstance().Clear();
        }
        
        void ShowItem()
        {
            m_kData = userData as MagicBoxResultFrameData;
            int times = 0;
            ProtoTable.ItemTable item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(800002001);
            if (item == null)
            {
                return;
            }
            ProtoTable.JarBonus  jarItem = TableManager.GetInstance().GetTableItem<ProtoTable.JarBonus>(item.ID);

            if (jarItem != null)
            {
                int count = (CountDataManager.GetInstance().GetCount(jarItem.CounterKey)+ jarItem.ComboBuyNum) / jarItem.ComboBuyNum;
                int multiple = Mathf.CeilToInt((float)count / jarItem.ExBonusNum_1 + 0.1f);

                if (multiple < 1)
                {
                    times = jarItem.ExBonusNum_1 - count ;
                }
                else
                {
                    times = multiple * jarItem.ExBonusNum_1 - count ;
                }
            }
            
            if (m_kData.itemRewards.Count > 0 && m_kData.ItemDoubleRewards.Count > 0)
            {
                useAll_8Rect.gameObject.CustomActive(true);
                rewardItemRect.gameObject.CustomActive(false);
                doubleRewardItemRect.gameObject.CustomActive(false);
                
                _ShowOpenMagicBoxItem(m_kData, m_comGetItemList);

                _ShowOpenMagicBoxDoubleItem(m_kData, m_comGetDoubleItemList);

               
                 useAllText.text = "再开启" + times + "个神秘匣子即可获得1次双倍奖励";
               
                
            }
            else if (m_kData.itemRewards.Count > 0)
            {
                rewardItemRect.gameObject.CustomActive(true);
                useAll_8Rect.gameObject.CustomActive(false);
                doubleRewardItemRect.gameObject.CustomActive(false);

                _ShowOpenMagicBoxItem(m_kData, m_comGetItemListReward);

             
                rewardItemText.text = "再开启" + times + "个神秘匣子即可获得1次双倍奖励";
              
            }
            else if (m_kData.ItemDoubleRewards.Count > 0)
            {
                doubleRewardItemRect.gameObject.CustomActive(true);
                useAll_8Rect.gameObject.CustomActive(false);
                rewardItemRect.gameObject.CustomActive(false);

                _ShowOpenMagicBoxDoubleItem(m_kData, m_comGetDoubleListDoubleReward);

               
                doubleRewardText.text = "再开启" + times + "个神秘匣子即可获得1次双倍奖励";
               
            }
        }
        void _ShowOpenMagicBoxItem(MagicBoxResultFrameData data, ComUIListScript script)
        {

            if (data == null)
            {
                return;
            }

            ItemData itemData = null;
            for (int i = 0; i < data.itemRewards.Count; i++)
            {
                for (int j = i+1; j < data.itemRewards.Count; j++)
                {
                    if (data.itemRewards[i].Quality < data.itemRewards[j].Quality)
                    {
                        itemData = data.itemRewards[i];
                        data.itemRewards[i] = data.itemRewards[j];
                        data.itemRewards[j] = itemData;
                    }
                }
            }

            script.Initialize();

            script.onBindItem = var =>
            {
                return CreateComItem(Utility.FindGameObject(var, "ItemParent"));
            };

            script.onItemVisiable = var =>
              {
                  if (data.itemRewards != null)
                  {
                      List<ItemData> items = data.itemRewards;
                      if (var.m_index >= 0 && var.m_index < items.Count)
                      {
                          ComItem comItem = var.gameObjectBindScript as ComItem;
                          comItem.Setup(items[var.m_index], (var1, var2) =>
                           {
                               ItemTipManager.GetInstance().ShowTip(var2);
                           });

                          Utility.GetComponetInChild<Text>(var.gameObject, "Name").text = items[var.m_index].GetColorName() + " " + items[var.m_index].Count + "个";
                      }
                      
                  }
              };

            if (data.itemRewards != null)
            {
                script.SetElementAmount(data.itemRewards.Count);
            }
            else
            {
                script.SetElementAmount(0);
            }

        }

        void _ShowOpenMagicBoxDoubleItem(MagicBoxResultFrameData data, ComUIListScript script)
        {

            if (data == null)
            {
                return;
            }

            ItemData itemData = null;
            for (int i = 0; i < data.ItemDoubleRewards.Count; i++)
            {
                for (int j = i + 1; j < data.ItemDoubleRewards.Count; j++)
                {
                    if (data.ItemDoubleRewards[i].Quality < data.ItemDoubleRewards[j].Quality)
                    {
                        itemData = data.ItemDoubleRewards[i];
                        data.ItemDoubleRewards[i] = data.ItemDoubleRewards[j];
                        data.ItemDoubleRewards[j] = itemData;
                    }
                }
            }

            script.Initialize();

            script.onBindItem = var =>
            {
                return CreateComItem(Utility.FindGameObject(var, "ItemParent"));
            };

            script.onItemVisiable = var =>
            {
                if (data.ItemDoubleRewards != null)
                {
                    List<ItemData> items = data.ItemDoubleRewards;
                    if (var.m_index >= 0 && var.m_index < items.Count)
                    {
                        ComItem comItem = var.gameObjectBindScript as ComItem;
                        comItem.Setup(items[var.m_index], (var1, var2) =>
                        {
                            ItemTipManager.GetInstance().ShowTip(var2);
                        });

                        Utility.GetComponetInChild<Text>(var.gameObject, "Name").text = items[var.m_index].GetColorName()+" "+items[var.m_index].Count+ "个";
                    }
                }
            };

            script.SetElementAmount(data.ItemDoubleRewards.Count);

        }
        [UIEventHandle("UseAll_8/Title/Close")]
        void OnCloseClick()
        {
            frameMgr.CloseFrame(this);
        }
        [UIEventHandle("RewardItem/close")]
        void RewardClose()
        {
            OnCloseClick();
        }
        [UIEventHandle("DoubleRewardItem/close")]
        void DoubleRewardClose()
        {
            OnCloseClick();
        }

    }
}

