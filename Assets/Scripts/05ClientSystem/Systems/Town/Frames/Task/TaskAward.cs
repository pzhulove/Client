using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;


namespace GameClient
{
    public class TaskAward : ClientFrame
    {
        public class TaskAwardData
        {
            public Int32 iID;
            public List<AwardItemData> awards;
        }
        TaskAwardData m_kData;

        [UIControl("Title", typeof(Text))]
        Text m_kTitle;

        #region awardItemObject
        class AwardItemObject : CachedObject
        {
            protected GameObject goLocal;
            protected GameObject goParent;
            protected GameObject goPrefab;

            ComItem comItem;
            Text awardDesc;
            AwardItemData data;
            ItemData itemData;
            TaskAward THIS;

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                data = (AwardItemData)param[2];
                THIS = param[3] as TaskAward;

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
            public override void OnRefresh(object[] param) { OnCreate(param); }
            public override bool NeedFilter(object[] param) { return false; }

            void _UpdateItem()
            {
                awardDesc.text = null;
                itemData = GameClient.ItemDataManager.CreateItemDataFromTable(data.ID);
                if(itemData != null)
                {
                    itemData.Count = data.Num;
                    string value = null;
                    if (itemData.Type == ProtoTable.ItemTable.eType.INCOME)
                    {
                        value = string.Format("<color={0}>{1}{2}</color>", itemData.Quality.ToString().ToLower(), itemData.Count, itemData.Name);
                    }
                    else
                    {
                        if (itemData.Count > 1)
                        {
                            value = string.Format("<color={0}>{1}X{2}</color>", itemData.Quality.ToString().ToLower(), itemData.Name, itemData.Count);
                        }
                        else
                        {
                            value = string.Format("<color={0}>{1}</color>", itemData.Quality.ToString().ToLower(), itemData.Name);
                        }
                    }

                    awardDesc.text = value;
                }
                itemData.Count = 1;
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

        void _UpdateItemAwardObjects()
        {
            if(m_kData != null)
            {
                var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(m_kData.iID);
                if(missionItem != null)
                {
                    m_kTitle.text = missionItem.TaskName;
                }
                m_akAwardItemObjects.RecycleAllObject();
                for(int i = 0; i < m_kData.awards.Count; ++i)
                {
                    m_akAwardItemObjects.Create(new object[] { m_goParent, m_goPrefab, m_kData.awards[i] ,this});
                }
            }
        }
        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Mission/MissionComplete";
        }

        protected override void _OnOpenFrame()
        {
            m_kData = userData as TaskAwardData;
            m_akAwardItemObjects.Clear();
            m_goParent = Utility.FindChild(frame, "Di/AwardArray");
            m_goPrefab = Utility.FindChild(frame, "Di/AwardArray/AwardItem");
            m_goPrefab.CustomActive(false);
            _UpdateItemAwardObjects();

			AudioManager.instance.PlaySound(10);

            //AudioManager.instance.PlaySound(Utility.GetSoundPath(Utility.SoundKind.SK_ACQUIRE_AWARD), AudioType.AudioEffect);
        }

        protected override void _OnCloseFrame()
        {
            m_akAwardItemObjects.Clear();

            //AudioManager.instance.PlaySound(Utility.GetSoundPath(Utility.SoundKind.SK_COMPLETE_TASK), AudioType.AudioEffect);
        }
        #endregion

        [UIEventHandle("BtnOK")]
        private void OnClickOk()
        {
            ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(m_kData.iID);
            if (missionItem == null)
            {
                Close();
                Logger.LogErrorFormat("can not find mission with id = {0} in missiontable !!", m_kData.iID);
                return;
            }

            MissionManager.SingleMissionInfo singleMissionInfo;
            if (!MissionManager.GetInstance().taskGroup.TryGetValue((uint)m_kData.iID, out singleMissionInfo))
            {
                Close();
                //Logger.LogErrorFormat("can not find mission with id = {0} in taskgroup !!", m_kData.iID);
                return;
            }

            if (Utility.IsNormalMission((uint)m_kData.iID))
            {
                if (singleMissionInfo.status == (int)Protocol.TaskStatus.TASK_FINISHED)
                {
                    MissionManager.GetInstance().sendCmdSubmitTask((uint)m_kData.iID, (TaskSubmitType)missionItem.FinishType, (uint)missionItem.MissionFinishNpc);
                }
            }

            Close();
        }
    }
}