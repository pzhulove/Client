using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    class ActorShowEquipData
    {
		public struct PetData
		{
			public int dataID;
			public int level;
			public int hunger;
			public int skillIndex;
            public int petScore;
        }

        public List<ItemData> m_akEquipts;
        public List<ItemData> m_akFashions;
        public Dictionary<int, EquipSuitObj> m_dictEquipSuitObjs;
        public int m_iLevel;
        public string m_kName;
        public int m_iJob;
        public ulong m_guid;
        //跨服查询，区域Id和查询类型
        public uint m_zoneId;
        public uint m_queryPlayerType;
        public PkStatisticInfo m_pkInfo;
        public uint pkValue;
        public uint matchScore;
		public int vip;
		public string guildName;
		public int guildJob;
        public string prefabPath;
        public bool bCompare = false;
		public PetData[] pets = new PetData[3];
        public PlayerAvatar avatar = null;
        public string adventureTeamName;
        public string adventureTeamGrade;
        public uint adventureTeamRank;
        public int emblemLv; // 徽记等级
        public uint totalEquipScore;//装备评分

		public bool HasGuild()
		{
			return guildName != null && guildName.Length > 0;
		}

		public bool HasVip()
		{
			return vip > 0 && vip <= 30;
		}

        public bool HasAdventureTeam()
        {
            return adventureTeamName != null && adventureTeamName.Length > 0;
        }
    }

    class ActorShowEquip : ClientFrame
    {
        public ActorShowEquipData m_kData;

        //查询类型和区域Id
        public static uint m_queryPlayerType;
        public static uint m_zoneId;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ActorShow/ActorShowEquip";
        }

        protected override void _OnOpenFrame()
        {
            m_akCachedEquiptItemObjects.Clear();
            m_kData = userData as ActorShowEquipData;

            //设置全局的查询类型和区域
            if (m_kData != null)
            {
                m_queryPlayerType = m_kData.m_queryPlayerType;
                m_zoneId = m_kData.m_zoneId;
            }
            else
            {
                m_queryPlayerType = 0;
                m_zoneId = 0;
            }

            _InitSlots();
            _InitEquipts();
        }

        protected override void _OnCloseFrame()
        {
            m_akCachedEquiptItemObjects.Clear();

            m_queryPlayerType = 0;
            m_zoneId = 0;
        }

        GameObject m_goLeft;
        GameObject m_goRight;

        void _InitSlots()
        {
            m_goLeft = Utility.FindChild(frame, "Equips/Left");
            m_goRight = Utility.FindChild(frame, "Equips/Right");
            List<ComItem> akGoItem = new List<ComItem>();
            int iCount = (int)EEquipWearSlotType.EquipMax - ((int)EEquipWearSlotType.EquipInvalid + 1);
            for (int i = 0; i < iCount ; ++i)
            {
                ComItem comItem = CreateComItem(i < iCount/2 ? m_goLeft : m_goRight);
                akGoItem.Add(comItem);
            }

            for(int i = (int)EEquipWearSlotType.EquipInvalid + 1; i < (int)EEquipWearSlotType.EquipMax;++i)
            {
                MapIndex mapIndex = Utility.GetEnumAttribute<EEquipWearSlotType, MapIndex>((EEquipWearSlotType)i);
                if (mapIndex.Index >= 0 && mapIndex.Index < akGoItem.Count)
                {
                    var comItem = akGoItem[mapIndex.Index];
                    GameObject goParent = comItem.transform.parent.gameObject;
                    GameObject goLocal = comItem.transform.gameObject;
                    EEquipWearSlotType eEEquipWearSlotType = (EEquipWearSlotType)i;
                    m_akCachedEquiptItemObjects.Create((EEquipWearSlotType)i, new object[] { goParent,goLocal, eEEquipWearSlotType,this,null });
                }
            }
        }

        void _InitEquipts()
        {
            if(m_kData.m_akEquipts != null)
            {
                for(int i = 0; i < m_kData.m_akEquipts.Count; ++i)
                {
                    var itemData = m_kData.m_akEquipts[i];
                    if (itemData != null)
                    {
                        if(m_akCachedEquiptItemObjects.HasObject(itemData.EquipWearSlotType))
                        {
                            m_akCachedEquiptItemObjects.RefreshObject(itemData.EquipWearSlotType, new object[] { itemData });
                        }
                    }
                }
            }
        }

        [UIEventHandle("close")]
        void OnClickClose()
        {
            CloseThisFrame();
        }

        public void CloseThisFrame()
        {
            frameMgr.CloseFrame(this);
        }

        #region equit
        public class EquipItemObject : CachedObject
        {
            protected GameObject goLocal;
            protected GameObject goParent;
            protected EEquipWearSlotType eEEquipWearSlotType;
            protected ActorShowEquip THIS;

            ComItem comItem;
            ItemData itemData;

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goLocal = param[1] as GameObject;
                eEEquipWearSlotType = (EEquipWearSlotType)param[2];
                THIS = param[3] as ActorShowEquip;
                itemData = param[4] as ItemData;
                comItem = goLocal.GetComponent<ComItem>();
                comItem.Setup(itemData, OnItemClicked);

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
                OnCreate(new object[] { goParent, goLocal, eEEquipWearSlotType, THIS, param[0] });
            }
            public override bool NeedFilter(object[] param) { return false; }

            void _UpdateItem()
            {
                string part = Utility.GetEnumDescription<EEquipWearSlotType>(eEEquipWearSlotType);
                part = TR.Value(part);
                comItem.SetupSlot(ComItem.ESlotType.Opened, part);
            }

            void OnItemClicked(GameObject obj, ItemData item)
            {
                if(item != null)
                {
                    LinkManager.GetInstance().AttachDatas = THIS.m_kData;
                    Parser.ItemParser.OnItemLink(item.GUID, (int) item.TableID,
                        m_queryPlayerType,
                        m_zoneId);
                }
            }
        }

        CachedObjectDicManager<EEquipWearSlotType, EquipItemObject> m_akCachedEquiptItemObjects = new CachedObjectDicManager<EEquipWearSlotType, EquipItemObject>();
        #endregion
    }
}