using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameClient
{
    class MissionScoreAwardFrame : ClientFrame
    {
        public static void Open(int id)
        {
            ClientSystemManager.GetInstance().CloseFrame<MissionScoreAwardFrame>();
            var data = MissionManager.GetInstance().MissionScoreDatas.Find(x =>
            {
                return x.missionScoreItem.ID == id;
            });
            if(data != null)
            {
                ClientSystemManager.GetInstance().OpenFrame<MissionScoreAwardFrame>(FrameLayer.Middle, data);
            }
        }
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Mission/MissionScoreAwardFrame";
        }

        MissionManager.MissionScoreData m_kData;
        protected override void _OnOpenFrame()
        {
            m_kData = userData as MissionManager.MissionScoreData;
            _InitItemAwardObjects();
            _UpdateItemAwardObjects();
            _InitHintInfo();
            _UpdateHintInfo();
            MissionManager.GetInstance().onDailyScoreChanged += OnDailyScoreChanged;
            MissionManager.GetInstance().onChestIdsChanged += OnChestIdsChanged;
        }
        protected override void _OnCloseFrame()
        {
            MissionManager.GetInstance().onDailyScoreChanged -= OnDailyScoreChanged;
            MissionManager.GetInstance().onChestIdsChanged -= OnChestIdsChanged;

            _UnInitHintInfo();
            _UnInitItemAwardObjects();
            m_kData = null;
        }

        [UIEventHandle("Close")]
        void _OnClickClose()
        {
            frameMgr.CloseFrame(this);
        }

        void OnDailyScoreChanged(int score)
        {
            _UpdateItemAwardObjects();
            _UpdateHintInfo();
        }

        void OnChestIdsChanged()
        {
            _UpdateItemAwardObjects();
            _UpdateHintInfo();
        }

        #region awards
        class AwardItemObject : CachedObject
        {
            protected GameObject goLocal;
            protected GameObject goParent;
            protected GameObject goPrefab;

            ComItem comItem;
            Text awardDesc;
            AwardItemData data;
            ItemData itemData;
            MissionScoreAwardFrame THIS;

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                data = param[2] as AwardItemData;
                THIS = param[3] as MissionScoreAwardFrame;

                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);
                    comItem = THIS.CreateComItem(Utility.FindChild(goLocal, "AwardIcon"));
                    awardDesc = Utility.FindComponent<Text>(goLocal, "AwardDesc");
                }

                Enable();
                _UpdateItem();
            }

            public override void OnRecycle()
            {
                Disable();
            }
            public override void Enable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(true);
                }
            }

            public override void Disable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override void OnDecycle(object[] param) { OnCreate(param); }
            public override void OnRefresh(object[] param)
            {
                if(param != null && param.Length > 0)
                {
                    data = param[0] as AwardItemData;
                }
                _UpdateItem();
            }
            public override bool NeedFilter(object[] param) { return false; }

            void _UpdateItem()
            {
                awardDesc.text = null;
                itemData = GameClient.ItemDataManager.CreateItemDataFromTable(data.ID);
                bool bGrey = false;
                var tableItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(data.ID);
                if(null != tableItem)
                {
                    bGrey = tableItem.Color2 == 1;
                }
                if (itemData != null)
                {
                    itemData.Count = data.Num;
                    string value = null;
                    if (itemData.Type == ProtoTable.ItemTable.eType.INCOME)
                    {
                        //value = string.Format("<color={0}>{1}{2}</color>", itemData.Quality.ToString().ToLower(), itemData.Count, itemData.Name);
                        value = string.Format("<color={0}>{1}{2}</color>", ItemData.GetQualityInfo(itemData.Quality, bGrey).ColStr, itemData.Count, itemData.Name);
                    }
                    else
                    {
                        if (itemData.Count > 1)
                        {
                            //value = string.Format("<color={0}>{1}X{2}</color>", itemData.Quality.ToString().ToLower(), itemData.Name, itemData.Count);
                            value = string.Format("<color={0}>{1}X{2}</color>", ItemData.GetQualityInfo(itemData.Quality, bGrey).ColStr, itemData.Name, itemData.Count);
                        }
                        else
                        {
                            //value = string.Format("<color={0}>{1}</color>", itemData.Quality.ToString().ToLower(), itemData.Name);
                            value = string.Format("<color={0}>{1}</color>", ItemData.GetQualityInfo(itemData.Quality, bGrey).ColStr, itemData.Name);
                        }
                    }

                    awardDesc.text = value;
                }
                comItem.Setup(itemData, _OnItemClicked);
            }

            void _OnItemClicked(GameObject obj, ItemData item)
            {
                ItemTipManager.GetInstance().ShowTip(item);
            }
        }

        CachedObjectListManager<AwardItemObject> m_akAwardItemObjects = new CachedObjectListManager<AwardItemObject>();
        GameObject m_goParent;
        GameObject m_goPrefab;
        void _InitItemAwardObjects()
        {
            m_goParent = Utility.FindChild(frame, "AwardArray");
            m_goPrefab = Utility.FindChild(frame, "AwardArray/AwardItem");
            m_goPrefab.CustomActive(false);
        }
        void _UnInitItemAwardObjects()
        {
            m_akAwardItemObjects.DestroyAllObjects();
            m_goParent = null;
            m_goPrefab = null;
        }

        void _UpdateItemAwardObjects()
        {
            m_akAwardItemObjects.RecycleAllObject();
            if (m_kData != null)
            {
                for (int i = 0; i < m_kData.awards.Count; ++i)
                {
                    m_akAwardItemObjects.Create(new object[] { m_goParent, m_goPrefab, m_kData.awards[i], this });
                }
            }
        }
        #endregion
        #region hintInfo
        Text hint;
        Button button;
        UIGray gray;
        GameObject hadGotGo;
        void _InitHintInfo()
        {
            hint = Utility.FindComponent<Text>(frame, "AwardHint");
            button = Utility.FindComponent<Button>(frame, "BtnAward");
            gray = Utility.FindComponent<UIGray>(frame, "BtnAward");
            hadGotGo = Utility.FindChild(frame, "HadGot");
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(_OnClickAcquired);
        }
        void _UpdateHintInfo()
        {
            bool bHasAcquired = MissionManager.GetInstance().AcquiredChestIDs.Contains(m_kData.missionScoreItem.ID);
            hadGotGo.CustomActive(bHasAcquired);
            button.CustomActive(!bHasAcquired);
            if (m_kData.missionScoreItem.Score <= MissionManager.GetInstance().Score && !bHasAcquired)
            {
                button.enabled = true;
                gray.enabled = false;
                hint.text = string.Format(TR.Value("mission_score_enable"), m_kData.missionScoreItem.Score);
            }
            else
            {
                button.enabled = false;
                gray.enabled = true;
                if (m_kData.missionScoreItem.Score > MissionManager.GetInstance().Score)
                {
                    hint.text = string.Format(TR.Value("mission_score_disable"), m_kData.missionScoreItem.Score);
                }
                else
                {
                    hint.text = string.Format(TR.Value("mission_score_enable"), m_kData.missionScoreItem.Score);
                }
            }
        }
        void _UnInitHintInfo()
        {
            hint = null;
            button.onClick.RemoveAllListeners();
            button = null;
            gray = null;
        }
        void _OnClickAcquired()
        {
            MissionManager.GetInstance().SendAcquireAwards(m_kData.missionScoreItem.ID);
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}