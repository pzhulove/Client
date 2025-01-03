using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
	public enum ComGuildItemType
	{
		CGIT_BLANK =  0,
		CGIT_ITEM_NORMAL,
		CGIT_ITEM_UNSELECTED,
		CGIT_ITEM_SELECTED,
		CGIT_COUNT,
	}

    class ComGuildItemData
    {
        public bool bClear = false;
        public int iSelectedCount = 0;
        public bool bAsSelector = false;
        public ItemData itemData = null;
    }

    class ComGuildItem : MonoBehaviour 
	{
		static string[] ms_static_descs = new string[(int)ComGuildItemType.CGIT_COUNT]
		{
			"Blank","ItemNormal","ItemUnSelected","ItemSelected"
		};
		public static ComGuildItem ms_selected = null;
		public GameObject goItemParent;
		public StateController comState;
		public Toggle toggle;
		ComItem comItem = null;
        ComGuildItemData data = new ComGuildItemData();
        ComItem.OnItemClicked onItemClicked = null;

        public ComGuildItemData Value
        {
            get
            {
                return data;
            }
            private set
            {
                data = Value;
            }
        }

		static List<ComGuildItem> pools = new List<ComGuildItem>();
		public static void ClearPools()
		{
			pools.Clear();
		}

		public static List<ComGuildItem> PoolItems
		{
			get 
			{
				return pools;
			}
		}

        public static void CancelSelectedItems()
        {
            List<object> templates = GamePool.ListPool<object>.Get();
            try
            {
                for (int i = 0; i < pools.Count; ++i)
                {
                    templates.Add(pools[i]);
                }
                for (int i = 0; i < templates.Count; ++i)
                {
                    var current = (templates[i] as ComGuildItem);
                    if (null != current)
                    {
                        current.OnItemSelected(false);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogErrorFormat(ex.ToString());
            }
            GamePool.ListPool<object>.Release(templates);
        }

		public void OnItemSelected(bool bSelected)
		{
			if (null != comState)
			{
				if (bSelected)
				{
                    if(!Value.bClear)
                    {
                        int iUsedSpace = GuildDataManager.GetInstance().storeDatas.Count + PoolItems.Count;
                        if (iUsedSpace >= GuildDataManager.GetInstance().storeageCapacity)
                        {
                            if (null != toggle)
                            {
                                toggle.onValueChanged.RemoveListener(OnItemSelected);
                            }
                            toggle.isOn = false;
                            if (null != toggle)
                            {
                                toggle.onValueChanged.AddListener(OnItemSelected);
                            }
                            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_store_house_is_full"));
                            return;
                        }
                    }

					if (!pools.Contains (this)) 
					{
						pools.Add (this);
					}
                    if(null != Value.itemData)
                    {
                        Value.iSelectedCount = Value.itemData.Count;
                    }
                    _RefreshData();

                    if(null != Value.itemData && Value.itemData.Count > 1)
                    {
                        ClientSystemManager.GetInstance().OpenFrame<GuildStoreConfirmFrame>(FrameLayer.High, new GuildStoreConfirmFrameData
                        {
                            title = (Value.bClear ? TR.Value("guild_op_title_1") : TR.Value("guild_op_title_0")),
                            iCurCount = Value.itemData.Count,
                            iMax = Value.itemData.Count,
                            onOk = (int iCount)=>
                            {
                                Value.iSelectedCount = iCount;
                                _RefreshData();
                            }
                        });
                    }
				} 
				else
				{
					pools.Remove(this);
                    Value.iSelectedCount = 0;
                    _RefreshData();

                    if(toggle.isOn != false)
                    {
                        if (null != toggle)
                        {
                            toggle.onValueChanged.RemoveListener(OnItemSelected);
                        }
                        toggle.isOn = false;
                        if (null != toggle)
                        {
                            toggle.onValueChanged.AddListener(OnItemSelected);
                        }
                    }
                }
			}
		}

		public void InitComGuildItem(ComItem.OnItemClicked onItemClicked,bool bAsSelector,bool bClear)
		{
            this.onItemClicked = onItemClicked;
            Value.bAsSelector = bAsSelector;
            Value.bClear = bClear;
        }

        void _RefreshData()
        {
            if (null == Value.itemData)
            {
                if (null != comState)
                {
                    comState.Key = ms_static_descs[(int)ComGuildItemType.CGIT_BLANK];
                }
                if (null != toggle)
                {
                    toggle.onValueChanged.RemoveListener(OnItemSelected);
                }
            }
            else
            {
                if (null == comItem)
                {
                    comItem = ComItemManager.Create(goItemParent);
                }

                if (Value.bAsSelector)
                {
                    if (!PoolItems.Contains(this))
                    {
                        comState.Key = ms_static_descs[(int)ComGuildItemType.CGIT_ITEM_UNSELECTED];
                    }
                    else
                    {
                        comState.Key = ms_static_descs[(int)ComGuildItemType.CGIT_ITEM_SELECTED];
                    }

                    if (null != toggle)
                    {
                        toggle.onValueChanged.RemoveListener(OnItemSelected);
                        toggle.onValueChanged.AddListener(OnItemSelected);
                    }
                }
                else
                {
                    comState.Key = ms_static_descs[(int)ComGuildItemType.CGIT_ITEM_NORMAL];
                    if (null != toggle)
                    {
                        toggle.onValueChanged.RemoveListener(OnItemSelected);
                    }
                }
            }

            if (null != comItem)
            {
                if (!PoolItems.Contains(this))
                {
                    comItem.SetCountFormatter(null);
                    if (null == Value || null == Value.itemData)
                    {
                        comItem.Setup(null, null);
                    }
                    else
                    {
                        comItem.Setup(Value.itemData, onItemClicked);
                    }
                }
                else
                {
                    if (null == Value || null == Value.itemData)
                    {
                        comItem.SetCountFormatter(null);
                        comItem.Setup(null, null);
                    }
                    else
                    {
                        if (Value.itemData.Count <= 1)
                        {
                            comItem.SetCountFormatter(null);
                        }
                        else
                        {
                            comItem.SetCountFormatter(_CountFormatter);
                        }
                        comItem.Setup(Value.itemData, onItemClicked);
                    }
                }
            }
        }

		public void SetItemData(ItemData data)
		{
            this.data.itemData = data;
            _RefreshData();
        }

        string _CountFormatter(ComItem a_comItem)
        {
            if(null != a_comItem && data.itemData.Count > 0 && null != Value && null != Value.itemData)
            {
                return string.Format("{0}/{1}",Value.iSelectedCount, data.itemData.Count);
            }
            return string.Empty;
        }

        public void OnDestroy()
		{
			if (null != comItem) 
			{
				ComItemManager.Destroy(comItem);
				comItem = null;
			}
			if (null != toggle) 
			{
				toggle.onValueChanged.RemoveListener(OnItemSelected);
				toggle = null;
			}
            goItemParent = null;
            comState = null;
            data = null;
            onItemClicked = null;
        }
	}
}