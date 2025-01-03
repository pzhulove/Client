using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ProtoTable;

namespace GameClient
{
    public class TitleData
    {
        public int iTitleID;
        public int iVipLevel;
        public string gangIcon;
        public string gangName;
        public byte gangDuty;
        public int iLv;
        public int pkRank;
        public string name;
        public bool bHasGang;
        public string adventTeamName;
        public bool bHasAdventureTeam;
        public Protocol.PlayerWearedTitleInfo playerWearedTitleInfo;
        //public bool bTitleName;
        public int guildEmblemLv;
    }

    public class TitleComponent : MonoBehaviour
    {
        private ComCommonBind[] layoutInsArray = new ComCommonBind[(int)TitleLayoutType.TLT_COUNT];
        private GameObject curLayoutIns = null;
private Canvas curLayoutCanvas = null;
        private Color textColor;
        private bool bShowPKLv = false;

        public static TitleComponent Create(GameObject goParent)
        {
            if (goParent == null)
            {
                return null;
            }
            
            GameObject goTitle = CGameObjectPool.instance.GetGameObject("UIFlatten/Prefabs/TownUI/HeadTitle", enResourceType.UIPrefab, (uint)GameObjectPoolFlag.ReserveLast);
            if (goTitle == null)
            {
                return null;
            }

            Utility.AttachTo(goTitle, goParent);

            return goTitle.GetComponent<TitleComponent>();
        }

        public void OnRecycle()
        {
            if (layoutInsArray != null)
            {
                for (int i = 0; i < layoutInsArray.Length; i++)
                {
                    if (layoutInsArray[i] != null && layoutInsArray[i].gameObject != null)
                    {
                        TitleConvert tc = layoutInsArray[i].GetCom<TitleConvert>("TitleConvert");
                        if (tc != null)
                            tc.OnRecycle();
                        CGameObjectPool.instance.RecycleGameObject(layoutInsArray[i].gameObject);
                    }
                }
                layoutInsArray = null;
            }
            
			curLayoutCanvas = null;
            curLayoutIns = null;

            sName_TLT_LV_NAME = string.Empty;
            sName_TLT_LV_NAME_TITLE_VIP = string.Empty;
            sName_TLT_GANG = string.Empty;
            sName_TLT_NAME = string.Empty;
            sName_TLT_PKLV_NAME = string.Empty;
            sName_TLT_TLT_LV_NAME_TITLE = string.Empty;
            sName_TLT_ADVENT = string.Empty;
            sAdventureName_TLT_ADVENT = string.Empty;
            iTitleID_TLT_TLT_LV_NAME_TITLE = -1;
            iTitleID_TLT_LV_NAME_TITLE_VIP = -1;
            iLv_TLT_LV_NAME_TITLE_VIP = -1;
            iLv_TLT_LV_NAME = -1;
            iLv_TLT_TLT_LV_NAME_TITLE = -1;
            gangDuty_TLT_GANG = 100;
            iPkRank_TLT_PKLV_NAME = -1;

            gameObject.transform.SetParent(null);
            Level = 0;
            TitleID = 0;
            PKRank = 0;
            VipLevel = 0;
            GangDuty = 0;
            CancelInvoke("_UpdateTurn");
            gameObject.CustomActive(false);
            CGameObjectPool.instance.RecycleGameObject(gameObject);
        }

        void OnDestroy()
        {
            if (layoutInsArray != null)
            {
                for (int i = 0; i < layoutInsArray.Length; i++)
                {
                    if (layoutInsArray[i] != null && layoutInsArray[i].gameObject)
                    {
                        TitleConvert tc = layoutInsArray[i].GetCom<TitleConvert>("TitleConvert");
                        if (tc != null)
                            tc.OnRecycle();
                        CGameObjectPool.instance.RecycleGameObject(layoutInsArray[i].gameObject);
                    }
                }
                layoutInsArray = null;
            }
            
            CancelInvoke("_UpdateTurn");
        }

        TitleData kData = new TitleData();

        public int Level
        {
            get
            {
                return kData.iLv;
            }
            set
            {
                kData.iLv = value;
            }
        }

        public int TitleID
        {
            get
            {
                return kData.iTitleID;
            }
            set
            {
                kData.iTitleID = value;
                _MarkDirty();
            }
        }

        public int PKRank
        {
            get
            {
                return kData.pkRank;
            }
            set
            {
                kData.pkRank = value;
            }
        }

        public int VipLevel
        {
            get
            {
                return kData.iVipLevel;
            }
            set
            {
                kData.iVipLevel = value;
                //TODO vip sprite
                _MarkDirty();
            }
        }

        public string Name
        {
            get
            {
                return kData.name;
            }
            set
            {
                kData.name = value;
                _MarkDirty();
            }
        }

        public string GangIcon
        {
            get
            {
                return kData.gangIcon;
            }
            set
            {
                kData.gangIcon = value;
            }
        }

        public string GangName
        {
            get
            {
                return kData.gangName;
            }

            set
            {
                kData.gangName = value;

                var eDuty = GuildDataManager.GetInstance().GetClientDuty(kData.gangDuty);
                HasGang = eDuty != EGuildDuty.Invalid;
            }
        }
        public byte GangDuty
        {
            get
            {
                return kData.gangDuty;
            }
            set
            {
                kData.gangDuty = value;

                var eDuty = GuildDataManager.GetInstance().GetClientDuty(value);
                HasGang = eDuty != EGuildDuty.Invalid;
            }
        }

        bool HasGang
        {
            get
            {
                return kData.bHasGang;
            }
            set
            {
                kData.bHasGang = value;

                _MarkDirty();
            }
        }

        public string AdventTeamName
        {
            get
            {
                return kData.adventTeamName;
            }
            set
            {
                kData.adventTeamName = value;

                HasAdventureTeam = !string.IsNullOrEmpty(kData.adventTeamName);
            }
        }

     
        public Protocol.PlayerWearedTitleInfo PlayerWearedTitleInfo
        {
            get { return kData.playerWearedTitleInfo; }
            set
            {
                kData.playerWearedTitleInfo = value;
            }
        }

        public int GuildEmblemLv
        {
            get
            {
                return kData.guildEmblemLv;
            }
            set
            {
                kData.guildEmblemLv = value;
                _MarkDirty();
            }
        }
        bool HasAdventureTeam
        {
            get {
                return kData.bHasAdventureTeam;
            }
            set {
                kData.bHasAdventureTeam = value;
                _MarkDirty();
            }
        }

        enum TitleShowType : int
        {
            TST_NOGANG_NOTITTLE = 0,
            TST_NOGANG_TITTLE,
            TST_GANG_NOTITTLE,
            TST_GANG_TITTLE,
            TST_VIP_NOGANG_NOTITTLE,
            TST_VIP_NOGANG_TITTLE,
            TST_VIP_GANG_NOTITTLE,
            TST_VIP_GANG_TITTLE,
            TST_ADVENTTEAM_NOVIP_NOGANG_NOTITLE,
            TST_ADVENTTEAM_NOVIP_NOGANG_TITLE,
            TST_ADVENTTEAM_NOVIP_GANG_NOTITLE,
            TST_ADVENTTEAM_NOVIP_GANG_TITLE,
            TST_ADVENTTEAM_VIP_NOGANG_NOTITLE,
            TST_ADVENTTEAM_VIP_NOGANG_TITLE,
            TST_ADVENTTEAM_VIP_GANG_NOTITLE,
            TST_ADVENTTEAM_VIP_GANG_TITLE,
            TST_COUNT,
            TST_TITTLE_STATUS = 5,
        }

        enum TitleLayoutType
        {
            TLT_NAME,
            TLT_PKLV_NAME,
            TLT_LV_NAME_TITLE,
            TLT_GANG,
            TLT_LV_NAME,
            TLT_LV_NAME_TITLE_VIP,
            TLT_ADVENT_LV_NAME_TITLE,
            TLT_ALL,
            TLT_COUNT
        } 
     
        int m_iPreStatus = 0;
        TitleShowType m_eShowType = TitleShowType.TST_NOGANG_NOTITTLE;
        int m_iIndex;

        static byte[][] ms_akTurnList = new byte[(int)TitleShowType.TST_COUNT][]
        {
            new byte[]{ 1, 2, 5 },
            new byte[]{ 1, 2, 3 },
            new byte[]{ 2, 4, 5 },
            new byte[]{ 2, 3, 4 },
            new byte[]{ 1, 2, 3 },
            new byte[]{ 1, 2, 6 },
            new byte[]{ 2, 3, 4 },
            new byte[]{ 2, 6, 4 },
            new byte[]{ 1, 2, 5, 7 },
            new byte[]{ 1, 2, 3 ,7 },
            new byte[]{ 2, 4, 5 ,7 },
            new byte[]{ 2, 3, 4 ,7 },
            new byte[]{ 1, 2, 3 ,7 },
            new byte[]{ 1, 2, 6 ,7 },
            new byte[]{ 2, 3, 4 ,7 },
            new byte[]{ 2, 6, 4 ,7 },
        };

        float SwitchStatus(int iStatus)
        {
            float fLastTime = 2.0f;

            if (layoutInsArray == null)
                layoutInsArray = new ComCommonBind[(int)TitleLayoutType.TLT_COUNT];

            TitleLayoutType type = (TitleLayoutType) iStatus;

			if (curLayoutCanvas != null)
            {
                curLayoutCanvas.enabled = false;
                curLayoutCanvas = null;
            }
            if (curLayoutIns != null)
            {
                curLayoutIns.SetLayer("OUTUI");
                //curLayoutIns.SetActive(false);
                curLayoutIns = null;
                //TODO Use OUTUI 
            }

            ComCommonBind ccb = layoutInsArray[iStatus];
            if (ccb == null)
            {
                string sPath = "UIFlatten/Prefabs/TownUI/TitleLayout_" + type.ToString();
                GameObject layoutIns = CGameObjectPool.instance.GetGameObject(sPath, enResourceType.UIPrefab, (uint)GameObjectPoolFlag.ReserveLast);

                if (layoutIns == null)
                    return fLastTime;

                Utility.AttachTo(layoutIns, this.gameObject);
                ccb = layoutIns.GetComponent<ComCommonBind>();
                layoutInsArray[iStatus] = ccb;
            }

            curLayoutIns = ccb.gameObject;

            if (curLayoutIns != null)
            {
                curLayoutIns.SetLayer("SCENEUI");
                //curLayoutIns.SetActive(true);
            }
			curLayoutCanvas = ccb.GetCom<Canvas>("Canvas");
            if (curLayoutCanvas != null)
                curLayoutCanvas.enabled = true;

            switch (type)
            {
                case TitleLayoutType.TLT_NAME:
                    _UpdateLayout_TLT_NAME(ccb);
                    break;
                case TitleLayoutType.TLT_PKLV_NAME:
                    _UpdateLayout_TLT_PKLV_NAME(ccb);
                    break;
                case TitleLayoutType.TLT_LV_NAME_TITLE:
                    fLastTime = _UpdateLayout_TLT_LV_NAME_TITLE(ccb);
                    break;
                case TitleLayoutType.TLT_GANG:
                    _UpdateLayout_TLT_GANG(ccb);
                    break;
                case TitleLayoutType.TLT_LV_NAME:
                    _UpdateLayout_TLT_LV_NAME(ccb);
                    break;
                case TitleLayoutType.TLT_LV_NAME_TITLE_VIP:
                    fLastTime = _UpdateLayout_TLT_LV_NAME_TITLE_VIP(ccb);
                    break;
                case TitleLayoutType.TLT_ADVENT_LV_NAME_TITLE:
                    _UpdateLayout_TLT_ADVENT(ccb);
                    break;
                case TitleLayoutType.TLT_ALL:
                    _UpdateLayout_TLT_ALL(ccb);
                    break;
            }

            return fLastTime;
        }

        private void _SetText(Text text, string content)
        {
            if (text == null) 
                return;

            text.font.RequestCharactersInTexture(content, text.fontSize, FontStyle.Normal);
            CharacterInfo characterInfo;
            float width = 1f;
            for (int i = 0; i < content.Length; i++)
            {
                text.font.GetCharacterInfo(content[i], out characterInfo, text.fontSize);
                width += characterInfo.advance;
            }

            text.rectTransform.sizeDelta = new Vector2(width, 20);
            text.text = content;
            return;
        }

        private string sName_TLT_NAME = string.Empty;
        private void _UpdateLayout_TLT_NAME(ComCommonBind ccb)
        {
            Text name = ccb.GetCom<Text>("Name");
            bool dirty = false;
            if (name != null && sName_TLT_NAME != kData.name)
            {
                _SetText(name, kData.name);
                name.color = textColor;
                sName_TLT_NAME = kData.name;
            }
        }

        private string sName_TLT_PKLV_NAME = string.Empty;
        private int iPkRank_TLT_PKLV_NAME = -1;
        private void _UpdateLayout_TLT_PKLV_NAME(ComCommonBind ccb)
        {
            bool dirty = false;
            Text name = ccb.GetCom<Text>("Name");
            if (name != null && sName_TLT_PKLV_NAME != kData.name)
            {
                _SetText(name, kData.name);
                name.color = textColor;
                sName_TLT_PKLV_NAME = kData.name;
                dirty = true;
            }

            RectTransform pkGroup = ccb.GetCom<RectTransform>("PkGroup");
            if(pkGroup != null && pkGroup.gameObject.activeInHierarchy != bShowPKLv)
                pkGroup.gameObject.SetActive(bShowPKLv);

            Image seasonMainLevel = ccb.GetCom<Image>("SeasonMainLevel");
            Image seasonSubLevel = ccb.GetCom<Image>("SeasonSubLevel");
            if (seasonMainLevel != null && seasonSubLevel != null && iPkRank_TLT_PKLV_NAME != kData.pkRank)
            {
                if (SeasonDataManager.GetInstance().IsLevelValid(kData.pkRank))
                {
                    seasonMainLevel.gameObject.SetActive(true);
                    //seasonMainLevel.sprite = AssetLoader.GetInstance().LoadRes(
                    //    SeasonDataManager.GetInstance().GetMainSeasonLevelSmallIcon(kData.pkRank), typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref seasonMainLevel, SeasonDataManager.GetInstance().GetMainSeasonLevelSmallIcon(kData.pkRank));
                    //seasonSubLevel.sprite = AssetLoader.GetInstance().LoadRes(
                    //    SeasonDataManager.GetInstance().GetSubSeasonLevelIcon(kData.pkRank), typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref seasonSubLevel, SeasonDataManager.GetInstance().GetSubSeasonLevelIcon(kData.pkRank));
                    seasonSubLevel.SetNativeSize();

                    iPkRank_TLT_PKLV_NAME = kData.pkRank;
                    dirty = true;
                }
            }

            if(dirty)
                pkGroup.localPosition = (seasonMainLevel.preferredWidth * 0.5f + name.preferredWidth * 0.5f) * Vector3.left;
        }

        private string sName_TLT_TLT_LV_NAME_TITLE = string.Empty;
        private int iLv_TLT_TLT_LV_NAME_TITLE = -1;
        private int iTitleID_TLT_TLT_LV_NAME_TITLE = -1;
        private float _UpdateLayout_TLT_LV_NAME_TITLE(ComCommonBind ccb)
        {
            bool dirty = false;
            Text name = ccb.GetCom<Text>("Name");
            if (name != null && sName_TLT_TLT_LV_NAME_TITLE != kData.name)
            {
                _SetText(name, kData.name);
                name.color = textColor;
                sName_TLT_TLT_LV_NAME_TITLE = kData.name;
                dirty = true;
            }

            Text lv = ccb.GetCom<Text>("Lv");
            if (lv != null && iLv_TLT_TLT_LV_NAME_TITLE != kData.iLv)
            {
                _SetText(lv, string.Format("Lv.{0}", kData.iLv));
                lv.color = textColor;
                iTitleID_TLT_TLT_LV_NAME_TITLE = kData.iLv;
                dirty = true;
            }

            TitleConvert tc = ccb.GetCom<TitleConvert>("TitleConvert");
            if (tc != null && iTitleID_TLT_TLT_LV_NAME_TITLE != kData.iTitleID)
            {
                tc.TitleID = kData.iTitleID;
                tc.Active(true);
                iTitleID_TLT_TLT_LV_NAME_TITLE = kData.iTitleID;
            }

            if (dirty)
            {
                lv.rectTransform.localPosition = (name.preferredWidth * 0.5f + 5f) * Vector3.left;
                name.rectTransform.localPosition = (lv.preferredWidth * 0.5f + 5f) * Vector3.right;
            }

            return tc.GetAnimationTime();
        }

        private string sName_TLT_GANG = string.Empty;
        private byte gangDuty_TLT_GANG = 100;
        private void _UpdateLayout_TLT_GANG(ComCommonBind ccb)
        {
            Text name = ccb.GetCom<Text>("Name");
            if (name != null && sName_TLT_GANG != kData.name)
            {
                _SetText(name, kData.name);
                name.color = textColor;
                sName_TLT_GANG = kData.name;
            }

            Text gangName = ccb.GetCom<Text>("GangName");
            if (gangName != null && gangDuty_TLT_GANG != kData.gangDuty)
            {
                var eDuty = GuildDataManager.GetInstance().GetClientDuty(kData.gangDuty);
                if (eDuty == EGuildDuty.Invalid)
                    _SetText(gangName, "");
                else
                    _SetText(gangName, kData.gangName + " " + TR.Value(eDuty.GetDescription()));
                gangDuty_TLT_GANG = kData.gangDuty;
            }
        }

        private string sName_TLT_ADVENT = string.Empty;
        private string sAdventureName_TLT_ADVENT = string.Empty;
        private void _UpdateLayout_TLT_ADVENT(ComCommonBind ccb)
        {
            Text name = ccb.GetCom<Text>("Name");
            if (name != null && sName_TLT_ADVENT != kData.name)
            {
                _SetText(name, kData.name);
                name.color = textColor;
                sName_TLT_ADVENT = kData.name;
            }

            Text adventName = ccb.GetCom<Text>("AdventTeamName");
            if (adventName != null && sAdventureName_TLT_ADVENT != kData.adventTeamName)
            {
                if(string.IsNullOrEmpty(kData.adventTeamName))
                {
                     _SetText(adventName, "");
                }else
                {
                    string _adventureTeamName = string.Format(TR.Value("adventure_team_role_head_name"), kData.adventTeamName);
                    _SetText(adventName, _adventureTeamName);
                }
                sAdventureName_TLT_ADVENT = kData.adventTeamName;
            }
        }

        private void _UpdateLayout_TLT_ALL(ComCommonBind ccb)
        {
            //名字
            Text name = ccb.GetCom<Text>("Name");
            if (name != null && kData.name != null)
            {
                _SetText(name, kData.name);
                name.color = textColor;
            }

            //称号
            TitleConvert tc = ccb.GetCom<TitleConvert>("TitleConvert");
            if (tc != null && iTitleID_TLT_LV_NAME_TITLE_VIP != kData.iTitleID)
            {
                tc.TitleID = kData.iTitleID;
                tc.Active(true);
            }

            GameObject titleRoot = ccb.GetGameObject("GroupRoot");
            Text groupTitle = ccb.GetCom<Text>("GroupTitle");
            Image honorImage = ccb.GetCom<Image>("HonorImage");
            //头衔
            Text title = ccb.GetCom<Text>("Title");//文字头衔
            Image titel_img = ccb.GetCom<Image>("Title_Img");//图片头衔
            GameObject title_imgParent = ccb.GetGameObject("Title_Img_Parent");

            if (title != null)
            {
                title.CustomActive(false);
            }

            if (titleRoot != null)
            {
                titleRoot.CustomActive(false);
            }

            if (title_imgParent != null)
                title_imgParent.CustomActive(false);

            if (kData.playerWearedTitleInfo != null)
            {
                if (kData.playerWearedTitleInfo.style == (int)TitleDataManager.eTitleStyle.Txt)
                {
                    string titleName = kData.playerWearedTitleInfo.name;

                    if (title != null && titleName != null && titleName != "")
                    {
                        title.CustomActive(true);
                        _SetText(title, titleName);
                    }
                    
                    //LayoutRebuilder.ForceRebuildLayoutImmediate(titleRoot.GetComponent<RectTransform>());
                }
                else if (kData.playerWearedTitleInfo.style == (int)TitleDataManager.eTitleStyle.Img)
                {
                    if (titel_img != null)
                    {
                        title_imgParent.CustomActive(true);
                        var tabelItem = TableManager.instance.GetTableItem<NewTitleTable>((int)kData.playerWearedTitleInfo.titleId);
                        if (tabelItem != null)
                        {
                            ETCImageLoader.LoadSprite(ref titel_img, tabelItem.path);
                            titel_img.SetNativeSize();
                        }
                    }
                }
                else if (kData.playerWearedTitleInfo.style == (int)TitleDataManager.eTitleStyle.Group)
                {
                    string titleName = kData.playerWearedTitleInfo.name;

                    if (groupTitle != null && titleName != null && titleName != "")
                    {
                        if (titleRoot != null)
                            titleRoot.CustomActive(true);

                        _SetText(groupTitle, titleName);

                        var tabelItem = TableManager.instance.GetTableItem<NewTitleTable>((int)kData.playerWearedTitleInfo.titleId);
                        if (tabelItem != null)
                        {
                            if (honorImage != null)
                            {
                                string path = tabelItem.path;
                                if (path == string.Empty)
                                {
                                    return;
                                }

                                ETCImageLoader.LoadSprite(ref honorImage, path);
                                honorImage.SetNativeSize();

                                RectTransform rect = honorImage.GetComponent<RectTransform>();
                                if (rect != null)
                                {
                                    rect.sizeDelta = new Vector2(rect.sizeDelta.x / 2, rect.sizeDelta.y / 2);
                                }

                                honorImage.CustomActive(true);
                            }

                        }
                        
                        //LayoutRebuilder.ForceRebuildLayoutImmediate(titleRoot.GetComponent<RectTransform>());
                    }
                }
            }
        }

        private string sName_TLT_LV_NAME = string.Empty;
        private int iLv_TLT_LV_NAME = -1;
        private void _UpdateLayout_TLT_LV_NAME(ComCommonBind ccb)
        {
            bool dirty = false;
            Text name = ccb.GetCom<Text>("Name");
            if (name != null && sName_TLT_LV_NAME != kData.name)
            {
                _SetText(name, kData.name);
                name.color = textColor;
                sName_TLT_LV_NAME = kData.name;
                dirty = true;
            }

            Text lv = ccb.GetCom<Text>("Lv");
            if (lv != null && iLv_TLT_LV_NAME != kData.iLv)
            {
                _SetText(lv, string.Format("Lv.{0}", kData.iLv));
                lv.color = textColor;
                iLv_TLT_LV_NAME = kData.iLv;
                dirty = true;
            }

            if (dirty)
            {
                lv.rectTransform.localPosition = (name.preferredWidth * 0.5f + 5f) * Vector3.left;
                name.rectTransform.localPosition = (lv.preferredWidth * 0.5f + 5f) * Vector3.right;
            }
        }

        private string sName_TLT_LV_NAME_TITLE_VIP = string.Empty;
        private int iLv_TLT_LV_NAME_TITLE_VIP = -1;
        private int iTitleID_TLT_LV_NAME_TITLE_VIP = -1;
        private float _UpdateLayout_TLT_LV_NAME_TITLE_VIP(ComCommonBind ccb)
        {
            bool dirty = false;
            Text name = ccb.GetCom<Text>("Name");
            if (name != null && sName_TLT_LV_NAME_TITLE_VIP != kData.name)
            {
                _SetText(name, kData.name);
                name.color = textColor;
                sName_TLT_LV_NAME_TITLE_VIP = kData.name;
                dirty = true;
            }

            Text lv = ccb.GetCom<Text>("Lv");
            if (lv != null && iLv_TLT_LV_NAME_TITLE_VIP != kData.iLv)
            {
                _SetText(lv, string.Format("Lv.{0}", kData.iLv));
                lv.color = textColor;
                iLv_TLT_LV_NAME_TITLE_VIP = kData.iLv;
                dirty = true;
            }

            TitleConvert tc = ccb.GetCom<TitleConvert>("TitleConvert");
            if (tc != null && iTitleID_TLT_LV_NAME_TITLE_VIP != kData.iTitleID)
            {
                tc.TitleID = kData.iTitleID;
                tc.Active(true);
            }

            //TODO vip sprite
            //Image vip = ccb.GetCom<Image>("Vip");
            //if (vip != null)
            //{

            //}

            if (dirty)
            {
                lv.rectTransform.localPosition = (name.preferredWidth * 0.5f + 5f) * Vector3.left;
                name.rectTransform.localPosition = (lv.preferredWidth * 0.5f + 5f) * Vector3.right;
            }

            return tc.GetAnimationTime();
        }

        public void SetTitleData(int iTitleID,
         int a_nPKRank,
         int iVipLevel,
         byte guildDuty,
         string gangName,
         int iLv,
         string name,
         string adventteamname,
         Protocol.PlayerWearedTitleInfo playerWearedTitleInfo,
         int guildEmblemLv,
         Color nameColor)
        {
            PKRank = a_nPKRank;
            GangDuty = guildDuty;
            GangName = gangName;
            Level = iLv;
            Name = name;
            TitleID = iTitleID;
            VipLevel = iVipLevel;
            AdventTeamName = adventteamname;
            PlayerWearedTitleInfo = playerWearedTitleInfo;
            GuildEmblemLv = guildEmblemLv;
            SetPKEnable(true);

            this.textColor = nameColor;
        }

		public void SetPKEnable(bool flag)
		{
            bool bOpened = (Level >= Utility.GetFunctionUnlockLevel(FunctionUnLock.eFuncType.Duel)) && flag;
		    bShowPKLv = bOpened;
		}

        //public void SetNameColor(Color color)
        //{
        //    if(kName != null)
        //    {
        //        kName.color = color;
        //    }
        //    if(kLv != null)
        //    {
        //        kLv.color = color;
        //    }
        //}
        void _MarkDirty()
        {
            m_eShowType = _GetTargetTittlePlayType();
            //CancelInvoke("_UpdateTurn");
            //Invoke("_UpdateTurn", 0.0f);

            //CancelInvoke("_SetAllTitle");
            //Invoke("_SetAllTitle",0.0f);
            _SetAllTitle();
        }

        void _UpdateTurn()
        {
            if(m_eShowType >= 0 && m_eShowType < TitleShowType.TST_COUNT &&
                m_iIndex >= 0 && m_iIndex < ms_akTurnList[(int)m_eShowType].Length)
            {
                int iTargetType = ms_akTurnList[(int)m_eShowType][ m_iIndex] - 1;
                float iLastTime = SwitchStatus(iTargetType);
                m_iIndex = (m_iIndex + 1) % ms_akTurnList[(int)m_eShowType].Length;
                Invoke("_UpdateTurn",iLastTime);
            }
        }
        /// <summary>
        /// 界面固定模式下，加载所有信息
        /// </summary>
        void _SetAllTitle()
        {
            SwitchStatus((int)TitleLayoutType.TLT_ALL);
        }

        TitleShowType _GetTargetTittlePlayType()
        {
            bool bHasTittle = kData.iTitleID != 0;
            bool bHasGang = kData.bHasGang;
            bool bHasVip = kData.iVipLevel > 0;
            bool bHasAdventTeam = !string.IsNullOrEmpty(kData.adventTeamName);
            int iMark = 0;
            int k = 0;

            iMark |= ((bHasTittle ? 1 : 0) << k); ++k;

            iMark |= ((bHasGang ? 1 : 0) << k); ++k;

            iMark |= ((bHasVip ? 1 : 0) << k); ++k;

            iMark |= ((bHasAdventTeam ? 1 : 0) << k); ++k;

            return (TitleShowType)iMark;
        }

        // Use this for initialization
        void Start()
        {
            m_iIndex = 0;
            _MarkDirty();
        }

        public static void OnChangedLv(int iPlayerID, int iLevel)
        {
            if (ClientSystem.IsTargetSystem<ClientSystemTown>())
            {
                var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                if (current == null)
                {
                    current = ClientSystemManager.GetInstance().TargetSystem as ClientSystemTown;
                }

                if (current != null)
                {
                    Logger.LogProcessFormat("iPlayerID = {0} 的称号系统刷新了!", iPlayerID);
                    current.OnNotifyTownPlayerLvChanged((uint)iPlayerID, iLevel);
                }

                return;
            }
        }

//         public static void OnPkPointsChanged(int iPlayerID, int iPkPoints)
//         {
//             if (ClientSystem.IsTargetSystem<ClientSystemTown>())
//             {
//                 var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
//                 if (current == null)
//                 {
//                     current = ClientSystemManager.GetInstance().TargetSystem as ClientSystemTown;
//                 }
// 
//                 if (current != null)
//                 {
//                     Logger.LogProcessFormat("iPlayerID = {0} 的称号系统刷新了!", iPlayerID);
//                     current.OnNotifyTownPlayerPkPointsChanged((uint)iPlayerID, iPkPoints);
//                 }
//             }
//         }

        public static void OnChangeGuildName(int iPlayerID, string name)
        {
            if (ClientSystem.IsTargetSystem<ClientSystemTown>())
            {
                var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                if (current == null)
                {
                    current = ClientSystemManager.GetInstance().TargetSystem as ClientSystemTown;
                }

                if (current != null)
                {
                    Logger.LogProcessFormat("iPlayerID = {0} 的称号系统刷新了!", iPlayerID);
                    current.OnNotifyTownPlayerGuildNameChanged((uint)iPlayerID, name);
                }
            }
        }

        public static void OnChangeAdventTeamName(int iPlayerID, string name)
        {
            if (ClientSystem.IsTargetSystem<ClientSystemTown>())
            {
                var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                if (current == null)
                {
                    current = ClientSystemManager.GetInstance().TargetSystem as ClientSystemTown;
                }

                if (current != null)
                {
                    Logger.LogProcessFormat("iPlayerID = {0} 的佣兵团名称刷新了!", iPlayerID);
                    current.OnNotifyTownPlayerAdventTeamNameChanged((uint)iPlayerID, name);
                }
            }
        }

        public static void OnChangeTitleName(int iPlayerID, Protocol.PlayerWearedTitleInfo playerWearedTitleInfo)
        {
            var gameBattle = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
            if (gameBattle != null)
            {
                if (gameBattle != null)
                {
                    Logger.LogProcessFormat("iPlayerID = {0} 的头衔名称刷新了!", iPlayerID);
                    gameBattle.OnNotifyTownPlayerTitleNameChanged((uint)iPlayerID, playerWearedTitleInfo);
                }
                return;
            }
            if (ClientSystem.IsTargetSystem<ClientSystemTown>())
            {
                var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                if(current == null)
                {
                    current = ClientSystemManager.GetInstance().TargetSystem as ClientSystemTown;
                }

                if(current != null)
                {
                    Logger.LogProcessFormat("iPlayerID = {0} 的头衔名称刷新了!", iPlayerID);
                    current.OnNotifyTownPlayerTitleNameChanged((uint)iPlayerID, playerWearedTitleInfo);
                }
            }
        }

        public static void OnChangeName(int iPlayerID,string name)
        {
            if (ClientSystem.IsTargetSystem<ClientSystemTown>())
            {
                var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                if (current == null)
                {
                    current = ClientSystemManager.GetInstance().TargetSystem as ClientSystemTown;
                }

                if (current != null)
                {
                    Logger.LogProcessFormat("iPlayerID = {0} 的名字刷新了!", iPlayerID);
                    current.OnNotifyTownPlayerNameChanged((uint)iPlayerID, name);
                }
            }
        }

        public static void OnChangeGuileLv(int iPlayerID,uint guildEmblemLv)
        {
            if (ClientSystem.IsTargetSystem<ClientSystemTown>())
            {
                var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                if (current == null)
                {
                    current = ClientSystemManager.GetInstance().TargetSystem as ClientSystemTown;
                }

                if (current != null)
                {
                    Logger.LogProcessFormat("iPlayerID = {0} 的工会会记刷新了!", iPlayerID);
                    current.OnNotifyTownPlayerGuildLvChanged((uint)iPlayerID, guildEmblemLv);
                }
            }
        }

        public static void OnChangeGuildDuty(int iPlayerID, byte duty)
        {
            if (ClientSystem.IsTargetSystem<ClientSystemTown>())
            {
                var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                if (current == null)
                {
                    current = ClientSystemManager.GetInstance().TargetSystem as ClientSystemTown;
                }

                if (current != null)
                {
                    Logger.LogProcessFormat("iPlayerID = {0} 的称号系统刷新了!", iPlayerID);
                    current.OnNotifyTownPlayerGuildDutyChanged((uint)iPlayerID, duty);
                }
            }
        }

        public static void OnChangedTittle(int iPlayerID, int iTittle)
        {
            if (ClientSystem.IsTargetSystem<ClientSystemTown>())
            {
                var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                if (current == null)
                {
                    current = ClientSystemManager.GetInstance().TargetSystem as ClientSystemTown;
                }

                if (current != null)
                {
                    Logger.LogProcessFormat("iPlayerID = {0} 的称号系统刷新了!", iPlayerID);
                    current.OnNotifyTownPlayerTittleChanged((uint)iPlayerID, iTittle);
                }
            }
        }
    }
}