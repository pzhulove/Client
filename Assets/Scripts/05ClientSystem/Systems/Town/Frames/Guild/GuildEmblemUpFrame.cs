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
using System.Collections;
using DG.Tweening;

namespace GameClient
{
    public class GuildEmblemUpFrame : ClientFrame
    {
        #region val
        CachedObjectDicManager<int, CostMaterialItem> needCostMaterials = null;
        int nGuildLvLimit = 3;
        int nPlayerLvLimit = 30;
        int nHonourLvLimit = 3;

        #endregion

        #region ui bind
        private Button activeEmblem = null;
        private Button levelUpEmblem = null;
        private GameObject lv1 = null;
        private GameObject lvMax = null;
        private GameObject compare = null;
        private Text lvUpText = null;
        private Text limit = null;
        private GameObject goMaterialParent;
        private GameObject goMaterialPrefab;
        private GuildEmblemAttrItem emblemLv1 = null;
        private GuildEmblemAttrItem emblemLvMax = null;
        private GuildEmblemAttrItem emblemLvNow = null;
        private GuildEmblemAttrItem emblemLvNext = null;
        private Button Close = null;
        private Image icon = null;
        private Image maxLv = null;
        private Text caiLiaoTip = null;
        private Text activeEmblemText = null;
        private Button showAttrs = null;

        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildEmblemUp";
        }

        protected override void _OnOpenFrame()
        {
            needCostMaterials = new CachedObjectDicManager<int, CostMaterialItem>();
            BindUIEvent();

            nGuildLvLimit = GuildDataManager.GetInstance().GetEmblemLvUpGuildLvLimit();
            nPlayerLvLimit = GuildDataManager.GetInstance().GetEmblemLvUpPlayerLvLimit();
            nHonourLvLimit = GuildDataManager.GetInstance().GetEmblemLvUpHonourLvLimit();

            UpdateEmblemInfo();
        }

        protected override void _OnCloseFrame()
        {
            needCostMaterials.DestroyAllObjects();
            UnBindUIEvent();
        }

        protected override void _bindExUI()
        {
            activeEmblem = mBind.GetCom<Button>("activeEmblem");  
            activeEmblem.SafeSetOnClickListener(() => 
            {
                if(GuildDataManager.GetInstance().myGuild != null && GuildDataManager.GetInstance().myGuild.nLevel < nGuildLvLimit)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("emblem_active_need_guild_lv", nGuildLvLimit));
                    return;
                }

                if(PlayerBaseData.GetInstance().Level < nPlayerLvLimit)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("emblem_active_need_player_lv", nPlayerLvLimit));
                    return;
                }

                if(GuildDataManager.GetInstance().GetBuildingLevel(GuildBuildingType.HONOUR) < nHonourLvLimit)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("emblem_active_need_honour_lv", nHonourLvLimit));
                    return;
                }

                // 再判断材料是否足够
                List<int> notEnoughItemIDs = null;
                if (!GuildDataManager.GetInstance().IsCostEnoughToLvUpEmblem(ref notEnoughItemIDs))
                {
                    if (notEnoughItemIDs != null && notEnoughItemIDs.Count > 0)
                    {
                        ItemComeLink.OnLink(notEnoughItemIDs[0], 0);
                        return;
                    }
                }
                GuildDataManager.GetInstance().SendWorldGuildEmblemUpReq();
            });

            levelUpEmblem = mBind.GetCom<Button>("levelUpEmblem");
            levelUpEmblem.SafeSetOnClickListener(() => 
            {
                int emblemLv = GuildDataManager.GetInstance().GetEmblemLv();              

                // 先判断荣耀等级
                int nHonourLv = GuildDataManager.GetInstance().GetBuildingLevel(GuildBuildingType.HONOUR);
                int nNeedHonourLv = GuildDataManager.GetInstance().GetEmblemNeedHonourLv(emblemLv + 1);
                if(nHonourLv < nNeedHonourLv)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("emblem_lv_up_need_honour_1v_not_ok2",nNeedHonourLv));
                    return;
                }

                // 再判断材料是否足够
                List<int> notEnoughItemIDs = null;
                if(!GuildDataManager.GetInstance().IsCostEnoughToLvUpEmblem(ref notEnoughItemIDs))
                {
                    if(notEnoughItemIDs != null && notEnoughItemIDs.Count > 0)
                    {
                        ItemComeLink.OnLink(notEnoughItemIDs[0], 0);
                        return;
                    }
                }                

                GuildDataManager.GetInstance().SendWorldGuildEmblemUpReq();
            });

            lv1 = mBind.GetGameObject("lv1");
            lvMax = mBind.GetGameObject("lvMax");
            compare = mBind.GetGameObject("compare");

            lvUpText = mBind.GetCom<Text>("lvUpText");
            limit = mBind.GetCom<Text>("limit");

            goMaterialPrefab = mBind.GetGameObject("ItemParent");
            goMaterialParent = mBind.GetGameObject("CostMaterials");

            emblemLv1 = mBind.GetCom<GuildEmblemAttrItem>("emblemLv1");
            emblemLvMax = mBind.GetCom<GuildEmblemAttrItem>("emblemLvMax");
            emblemLvNow = mBind.GetCom<GuildEmblemAttrItem>("emblemLvNow");
            emblemLvNext = mBind.GetCom<GuildEmblemAttrItem>("emblemLvNext");

            Close = mBind.GetCom<Button>("Close");   
            Close.SafeSetOnClickListener(() => 
            {
                frameMgr.CloseFrame(this);
            });

            icon = mBind.GetCom<Image>("icon");

            maxLv = mBind.GetCom<Image>("maxLv");
            caiLiaoTip = mBind.GetCom<Text>("caiLiaoTip");

            activeEmblemText = mBind.GetCom<Text>("activeEmblemText");

            showAttrs = mBind.GetCom<Button>("showAttrs");
            showAttrs.SafeSetOnClickListener(() => 
            {
                frameMgr.OpenFrame<GuildEmblemAttrShowFrame>(FrameLayer.Middle);
            });
        }

        protected override void _unbindExUI()
        {
            activeEmblem = null;
            levelUpEmblem = null;
            lv1 = null;
            lvMax = null;
            compare = null;
            lvUpText = null;
            limit = null;
            goMaterialPrefab = null;
            goMaterialParent = null;
            emblemLv1 = null;
            emblemLvMax = null;
            emblemLvNow = null;
            emblemLvNext = null;
            Close = null;
            icon = null;
            maxLv = null;
            caiLiaoTip = null;
            activeEmblemText = null;
            showAttrs = null;
        }

        #endregion 

        #region method

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGuildEmblemLevelUp, _OnGuildEmblemLevelUp);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemNotifyGet, _OnUpdateLevelUpMaterials);
            PlayerBaseData.GetInstance().onMoneyChanged += _OnMoneyChanged;
            ItemDataManager.GetInstance().onAddNewItem += _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += OnUpdateItem;
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGuildEmblemLevelUp, _OnGuildEmblemLevelUp);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemNotifyGet, _OnUpdateLevelUpMaterials);
            PlayerBaseData.GetInstance().onMoneyChanged -= _OnMoneyChanged;
            ItemDataManager.GetInstance().onAddNewItem -= _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= OnUpdateItem;
        }        

        void UpdateEmblemInfo()
        {
            int emblemLv = GuildDataManager.GetInstance().GetEmblemLv();
            if(emblemLv < 0)
            {
                return;
            }

            int nMaxEmblemLv = GuildDataManager.GetInstance().GetMaxEmblemLv();

            activeEmblem.CustomActive(emblemLv == 0);
            levelUpEmblem.CustomActive(emblemLv > 0);

            lv1.CustomActive(emblemLv == 0);
            lvMax.CustomActive(emblemLv == nMaxEmblemLv);
            compare.CustomActive(emblemLv > 0 && emblemLv < nMaxEmblemLv);

            int nHonourLv = GuildDataManager.GetInstance().GetBuildingLevel(GuildBuildingType.HONOUR);
            int nNeedHonourLv = GuildDataManager.GetInstance().GetEmblemNeedHonourLv(emblemLv + 1);
            if (nNeedHonourLv > nHonourLv)
            {
                lvUpText.SafeSetText(TR.Value("emblem_lv_up_need_honour_1v", nNeedHonourLv));
                activeEmblemText.SafeSetText(TR.Value("emblem_lv_up_need_honour_1v", nNeedHonourLv));

                activeEmblem.SafeSetGray(true);
                levelUpEmblem.SafeSetGray(true);
            }
            else
            {
                lvUpText.SafeSetText(TR.Value("emblem_lv_up"));
                activeEmblemText.SafeSetText(TR.Value("emblem_active"));

                activeEmblem.SafeSetGray(false);
                levelUpEmblem.SafeSetGray(false);
            }

            //levelUpEmblem.SafeSetGray(emblemLv == nMaxEmblemLv);      

            UpdateLevelUpMaterials();

            if(emblemLv == 0)
            {
                if(emblemLv1 != null)
                {
                    emblemLv1.SetUp(1);
                }
            }
            else if(emblemLv == nMaxEmblemLv)
            {
                if(emblemLvMax != null)
                {
                    emblemLvMax.SetUp(nMaxEmblemLv);
                }                
            }
            else
            {
                if(emblemLvNow != null)
                {
                    emblemLvNow.SetUp(emblemLv);
                }

                if(emblemLvNext != null)
                {
                    emblemLvNext.SetUp(emblemLv + 1);
                }
            }

            int showEmblemLv = 1;
            if(emblemLv == 0)
            {
                showEmblemLv = 1;
            }
            else if(emblemLv == nMaxEmblemLv)
            {
                showEmblemLv = nMaxEmblemLv;
            }
            else
            {
                showEmblemLv = emblemLv;
            }

            icon.SafeSetImage(GuildDataManager.GetInstance().GetEmblemIconPath(showEmblemLv));

            if(emblemLv == nMaxEmblemLv)
            {
                levelUpEmblem.CustomActive(false);
                caiLiaoTip.CustomActive(false);
            }
        }

        void UpdateLevelUpMaterials()
        {
            needCostMaterials.RecycleAllObject();

            int[] materials = null;
            int[] material_counts = null;

            int nEmblemLv = GuildDataManager.GetInstance().GetEmblemLv();
            GuildDataManager.GetInstance().GetEmblemLvUpCost(nEmblemLv + 1, ref materials, ref material_counts);

            if(materials != null && material_counts != null)
            {
                int iCount = Math.Min(materials.Length, material_counts.Length);
                for (int i = 0; i < iCount; ++i)
                {
                    var materialData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(materials[i]);
                    if (materialData != null)
                    {
                        if (needCostMaterials.HasObject(materials[i]))
                        {
                            needCostMaterials.RefreshObject(materials[i], new object[] { materialData, false, material_counts[i] });
                        }
                        else
                        {
                            needCostMaterials.Create(materials[i], new object[] { goMaterialParent, goMaterialPrefab, materialData, this, false, material_counts[i] });
                        }
                    }
                }
                needCostMaterials.Filter(null);
            }            
        }

        void _OnMoneyChanged(PlayerBaseData.MoneyBinderType eMoneyBinderType)
        {
            UpdateLevelUpMaterials();
        }

        void _OnAddNewItem(List<Item> items)
        {
            UpdateLevelUpMaterials();
        }

        void OnUpdateItem(List<Item> items)
        {
            UpdateLevelUpMaterials();
        }

        #endregion

        #region ui event

        void _OnUpdateLevelUpMaterials(UIEvent uiEvent)
        {
            UpdateLevelUpMaterials();
        }

        void _OnGuildEmblemLevelUp(UIEvent a_event)
        {
            UpdateEmblemInfo();
        }        

        #endregion
    }
}
