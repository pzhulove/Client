using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Protocol;
using Network;

namespace GameClient
{
    class MenuItem
    {
        public string name;
        public delegate void MenuItemCallBack();
        public MenuItemCallBack callback;
        public bool bEnableDefault = true; // 默认是否启用状态
    }

    class MenuData
    {
        public Vector3 kWorldPos;
        public string name;
		public int level;
		public int vip;
		public string guildName;
		public int jobID;
        public int pkLevel;
        public List<MenuItem> items;
        public int ZoneID;
        public string adventureTeamName;
        public PlayerWearedTitleInfo WearedTitleInfo;
        public int guildLv;
        public ulong GUID;

		public bool HasGuild()
		{
			return guildName != null && guildName.Length > 0;
		}

		public bool HasVip()
		{
			return vip > 0;
		}

        public bool HasAdventureTeam()
        {
            return adventureTeamName != null && adventureTeamName.Length > 0;
        }
    }

    class ActorShowMenu : ClientFrame
    {
		private Text mLevel = null;
		private Image mPortrait = null;
		private Text mName = null;
		private Text mGuildName = null;
		private Text mVip = null;
        private Text mJobName = null;
        private Text mPkLevel = null;
        private Image mPkLevelImg = null;
        private Image mPkLevelNum = null;
        private UINumber mVipLv = null;

		private GameObject mMenuitem = null;
		private GameObject mMenuArray = null;

		private GameObject mVipParent = null;


		protected override void _bindExUI()
		{
			mLevel = mBind.GetCom<Text>("level");
			mPortrait = mBind.GetCom<Image>("portrait");
			mName = mBind.GetCom<Text>("name");
			mGuildName = mBind.GetCom<Text>("guildName");
			mVip = mBind.GetCom<Text>("vip");
            mJobName = mBind.GetCom<Text>("jobName");
            mPkLevel = mBind.GetCom<Text>("pkLevel");
            mPkLevelImg = mBind.GetCom<Image>("pkLevelImg");
            mPkLevelNum = mBind.GetCom<Image>("pkLevelNum");
            mVipLv = mBind.GetCom<UINumber>("vipLv");

			mMenuitem = mBind.GetGameObject("menuitem");
			mMenuArray = mBind.GetGameObject("menuArray");

			mVipParent = mBind.GetGameObject("vipParent");

			mMenuitem.CustomActive(false);
		}

		protected override void _unbindExUI()
		{
			mLevel = null;
			mPortrait = null;
			mName = null;
			mGuildName = null;
			mVip = null;
            mJobName = null;
            mPkLevel = null;
            mPkLevelImg = null;
            mPkLevelNum = null;
            mVipLv = null;
			mMenuitem = null;
			mMenuArray = null;

			mVipParent = null;
		}


        static MenuData ms_menuParams;
        public MenuData MenuParams
        {
            get;set;
        }

        MenuData m_kMenuData;

        public static void CloseMenu()
        {
            ms_menuParams = null;
            if (ClientSystemManager.GetInstance().IsFrameOpen<ActorShowMenu>())
            {
                ClientSystemManager.GetInstance().CloseFrame<ActorShowMenu>();
            }
        }

        public static void Open(MenuData menuData)
        {
            if(menuData != null)
            {
                if(!ClientSystemManager.GetInstance().IsFrameOpen<ActorShowMenu>())
                {
                    ClientSystemManager.GetInstance().OpenFrame<ActorShowMenu>(FrameLayer.Middle,menuData);
                }
                else
                {
                    ms_menuParams = menuData;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateActorShowMenu);
                }
            }
        }

        public override string GetPrefabPath()
        {
			return "UIFlatten/Prefabs/CheckInfo/ActorShowMenu";
        }

        protected override void _OnOpenFrame()
        {
            m_kMenuData = userData as MenuData;
            m_akCachedItemObjects.Clear();
            m_goUIRoot = GameObject.Find("UIRoot");
            m_kCanvas = Utility.FindComponent<Canvas>(m_goUIRoot,"UI2DRoot");

            Button btnClose = Utility.FindComponent<Button>(frame, "Close");
            btnClose.onClick.AddListener(() =>
            {
                CloseMenu();
                frameMgr.CloseFrame(this);
            });

            _InitMenuItems();
        }

        protected override void _OnCloseFrame()
        {
            m_akCachedItemObjects.Clear();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdateActorShowMenu, OnUpdateMenu);
        }

        GameObject m_goUIRoot;
        Canvas m_kCanvas;

        void _InitMenuItems()
        {

            _CreateMenuItemsFromData();

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdateActorShowMenu, OnUpdateMenu);
        }

        void OnUpdateMenu(UIEvent uiEvent)
        {
            _UpdateMenuItems();
        }

        void _UpdateMenuItems()
        {
            m_kMenuData = ms_menuParams;
            if(m_kMenuData == null || m_kMenuData.items == null || m_kMenuData.items.Count <= 0)
            {
                frameMgr.CloseFrame(this);
                return;
            }

            m_akCachedItemObjects.RecycleAllObject();

            _CreateMenuItemsFromData();
        }

        void _CreateMenuItemsFromData()
        {
            if (m_kMenuData != null)
            {
				mLevel.text = "Lv." + m_kMenuData.level;

				string path = "";
				var jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(m_kMenuData.jobID);
				if (jobData != null)
				{
                    if (mJobName)
                        mJobName.text = jobData.Name;
					ProtoTable.ResTable resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(jobData.Mode);
					if (resData != null)
					{
						path = resData.IconPath;
					}
				}
                // mPortrait.sprite = AssetLoader.instance.LoadRes(path, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref mPortrait, path);
				mName.text = m_kMenuData.name;

				if (m_kMenuData.HasGuild())
					mGuildName.text = "公会 " + m_kMenuData.guildName;
				else
					mGuildName.text = "";

				if (m_kMenuData.HasVip())
				{
					mVipParent.CustomActive(true);
                    if (mVipLv)
                        mVipLv.Value = m_kMenuData.vip;
					//mVip.text = "贵族" + m_kMenuData.vip;
				}
				else {
					mVipParent.CustomActive(false);
				}

                if(mPkLevel)
                    mPkLevel.text = SeasonDataManager.GetInstance().GetRankName(m_kMenuData.pkLevel);
                if (mPkLevelImg)
                {
                    // mPkLevelImg.sprite = AssetLoader.GetInstance().LoadRes(SeasonDataManager.GetInstance().GetMainSeasonLevelSmallIcon(m_kMenuData.pkLevel),typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref mPkLevelImg, SeasonDataManager.GetInstance().GetMainSeasonLevelSmallIcon(m_kMenuData.pkLevel));
                }
                if (mPkLevelNum)
                {
                    // mPkLevelNum.sprite = AssetLoader.GetInstance().LoadRes(SeasonDataManager.GetInstance().GetSubSeasonLevelIcon(m_kMenuData.pkLevel),typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref mPkLevelNum, SeasonDataManager.GetInstance().GetSubSeasonLevelIcon(m_kMenuData.pkLevel));
                }

                for (int i = 0; i < m_kMenuData.items.Count; ++i)
                {
					MenuItemObject obj = m_akCachedItemObjects.Create(new object[] { mMenuArray, mMenuitem, m_kMenuData.items[i] ,this});               
                    if(i > 1 && obj != null && frameMgr.IsFrameOpen<Pk3v3CrossWaitingRoom>())
                    {
                        obj.Disable();
                    }
                }

                GameObject bg1 = mBind.GetGameObject("BG1");
                GameObject bg2 = mBind.GetGameObject("BG2");

                if (bg1 != null && bg2 != null)
                {
                    if (frameMgr.IsFrameOpen<Pk3v3CrossWaitingRoom>())
                    {
                        bg1.CustomActive(false);
                        bg2.CustomActive(true);
                    }
                    else
                    {
                        bg1.CustomActive(true);
                        bg2.CustomActive(false);
                    }
                }

                // UpdatePosition();
            }
        }

        void UpdatePosition()
        {
            if(m_kMenuData != null && m_kCanvas != null)
            {
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(m_kCanvas.transform as RectTransform, Camera.main.WorldToScreenPoint(m_kMenuData.kWorldPos), m_kCanvas.worldCamera, out pos);
                RectTransform rect = frame.transform as RectTransform;
                rect.anchoredPosition = pos;
            }
        }

        #region MenuItemObject
        public class MenuItemObject : CachedObject
        {
            protected GameObject goLocal;
            protected GameObject goParent;
            protected GameObject goPrefab;
            protected MenuItem menuItem;
            protected ActorShowMenu THIS;

            Button button;
            Text text;

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                menuItem = param[2] as MenuItem;
                THIS = param[3] as ActorShowMenu;

                if (goLocal == null)
                {
					goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);

                    //TO ADD CODE
                    button = goLocal.GetComponent<Button>();
                    text = Utility.FindComponent<Text>(goLocal, "Text");
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(()=>OnClickMenu());
                }

                Enable();
                _UpdateItem();

                if (button != null)
                {              
                    button.interactable = true;
                    UIGray uigrayPre = button.gameObject.GetComponent<UIGray>();
                    if(uigrayPre != null)
                    {
                        uigrayPre.enabled = false;
                    }
                }
            }

            void OnClickMenu()
            {
                if(menuItem != null && menuItem.callback != null)
                {
                    menuItem.callback.Invoke();
                }

                THIS.Close();
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
                if(menuItem != null)
                {
                    text.text = menuItem.name;
                }
            }

            public void BtnDisable()
            {
                if(button != null)
                {
                    button.onClick.RemoveAllListeners();
                    button.interactable = false;

                    UIGray uigrayPre = button.gameObject.SafeAddComponent<UIGray>();
                    if(uigrayPre != null)
                    {
                        uigrayPre.enabled = true;
                    }                  
                }
            }
        }

        CachedObjectListManager<MenuItemObject> m_akCachedItemObjects = new CachedObjectListManager<MenuItemObject>();
        #endregion
    }
}
