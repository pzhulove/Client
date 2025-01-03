using GameClient;
using Protocol;
using ProtoTable;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleView : MonoBehaviour
{
    [SerializeField]
    private Toggle[] MainTab;

    [SerializeField]
    private ComUIListScript titleComUIList;

    [SerializeField]
    private Button TakeUp;

    [SerializeField]
    private Button TakeOff;

    [SerializeField]
    private Text TitleName;

    [SerializeField]
    private Image mTitleImg;

    [SerializeField]
    private Text TitleDes;

    [SerializeField]
    private Text TitleSource;

    [SerializeField]
    private GameObject PeopleMode;

    [SerializeField]
    private GeAvatarRendererEx m_AvatarRenderer;

    [SerializeField]
    private GameObject NoTitleTips;

    [SerializeField]private GameObject mDesRoot;
    [SerializeField]private GameObject mSourceRoot;
    [SerializeField]private GameObject mTitleRoot;
    [SerializeField]private Image mHonorImg;

    List<PlayerTitleInfo> titleList = new List<PlayerTitleInfo>();
    PlayerTitleInfo selectTitleData = new PlayerTitleInfo();
    int selectToggleIndex;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="index"></param>
    public void InitUI(int index)
    {
        selectTitleData = new PlayerTitleInfo();
        titleList.Clear();
        //titleItemGo.Clear();
        titleComUIList.Initialize();
        titleComUIList.onItemVisiable = (item) =>
        {
            if (item.m_index >= 0 && item.m_index < titleList.Count)
            {
                _UpdateItem(item);
            }
        };
        titleComUIList.OnItemRecycle = (item) =>
        {
            if (item.m_index >= 0 && item.m_index < titleList.Count)
            {
                ComCommonBind mBind = item.GetComponent<ComCommonBind>();
                if (mBind == null)
                {
                    return;
                }
                var mSelectToggle = mBind.GetCom<Toggle>("selectToggle");
                mSelectToggle.onValueChanged.RemoveAllListeners();
            }
        };
        _InitModel();
        //JobTable job = TableManager.instance.GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
        //if (job != null)
        //{
        //    ResTable res = TableManager.instance.GetTableItem<ResTable>(job.Mode);
        //    m_AvatarRenderer.LoadAvatar(res.ModelPath);

        //    PlayerBaseData.GetInstance().AvatarEquipFromCurrentEquiped(m_AvatarRenderer);
        //    //PlayerBaseData.GetInstance().AvatarEquipFromItems(m_AvatarRenderer,
        //    //            relationData.avatar.equipItemIds,
        //    //            relationData.occu,
        //    //            (int)(relationData.avatar.weaponStrengthen),
        //    //            null,
        //    //            false,
        //    //            relationData.avatar.isShoWeapon);
        //    m_AvatarRenderer.AttachAvatar("Aureole", "Effects/Scene_effects/Effectui/EffUI_chuangjue_fazhen_JS", "[actor]Orign", false);
        //    m_AvatarRenderer.ChangeAction("Anim_Show_Idle", 1.0f, true);
        //}
        TakeUp.onClick.RemoveAllListeners();
        TakeUp.onClick.AddListener(() =>
        {
            TitleDataManager.GetInstance().SendNewTitleTakeUpReq(selectTitleData.guid,selectTitleData.titleId);
        });

        TakeOff.onClick.RemoveAllListeners();
        TakeOff.onClick.AddListener(() =>
        {
            TitleDataManager.GetInstance().SendNewTitleTakeOffReq(selectTitleData.guid,selectTitleData.titleId);
        });
        for(int i = 0;i<MainTab.Length;i++)
        {
            int tempIndex = i;
            MainTab[i].onValueChanged.RemoveAllListeners();
            MainTab[i].onValueChanged.AddListener((value) =>
            {
                if(value)
                {
                    selectToggleIndex = tempIndex;
                    UpdateToggleUI(tempIndex);
                }
            });
        }
    }
    public void _InitModel()
    {
        JobTable job = TableManager.instance.GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
        if (job == null)
        {
            Logger.LogError("职业ID找不到 " + PlayerBaseData.GetInstance().JobTableID.ToString() + "\n");
        }
        else
        {
            ResTable res = TableManager.instance.GetTableItem<ResTable>(job.Mode);

            if (res == null)
            {
                Logger.LogError("职业ID Mode表 找不到 " + PlayerBaseData.GetInstance().JobTableID.ToString() + "\n");
            }
            else
            {
                m_AvatarRenderer.LoadAvatar(res.ModelPath);

                PlayerBaseData.GetInstance().AvatarEquipFromCurrentEquiped(m_AvatarRenderer);

                m_AvatarRenderer.AttachAvatar("Aureole", "Effects/Scene_effects/Effectui/EffUI_chuangjue_fazhen_JS", "[actor]Orign", false);
                m_AvatarRenderer.ChangeAction("Anim_Show_Idle", 1.0f, true);
            }
        }
    }
    private void Update()
    {
        if (null != m_AvatarRenderer)
        {
            while (global::Global.Settings.avatarLightDir.x > 360)
                global::Global.Settings.avatarLightDir.x -= 360;
            while (global::Global.Settings.avatarLightDir.x < 0)
                global::Global.Settings.avatarLightDir.x += 360;

            while (global::Global.Settings.avatarLightDir.y > 360)
                global::Global.Settings.avatarLightDir.y -= 360;
            while (global::Global.Settings.avatarLightDir.y < 0)
                global::Global.Settings.avatarLightDir.y += 360;

            while (global::Global.Settings.avatarLightDir.z > 360)
                global::Global.Settings.avatarLightDir.z -= 360;
            while (global::Global.Settings.avatarLightDir.z < 0)
                global::Global.Settings.avatarLightDir.z += 360;

            m_AvatarRenderer.m_LightRot = global::Global.Settings.avatarLightDir;
        }
    }
    //public void UpdateMode(float timeElapsed)
    //{
    //    if (null != m_AvatarRenderer)
    //    {
    //        while (global::Global.Settings.avatarLightDir.x > 360)
    //            global::Global.Settings.avatarLightDir.x -= 360;
    //        while (global::Global.Settings.avatarLightDir.x < 0)
    //            global::Global.Settings.avatarLightDir.x += 360;

    //        while (global::Global.Settings.avatarLightDir.y > 360)
    //            global::Global.Settings.avatarLightDir.y -= 360;
    //        while (global::Global.Settings.avatarLightDir.y < 0)
    //            global::Global.Settings.avatarLightDir.y += 360;

    //        while (global::Global.Settings.avatarLightDir.z > 360)
    //            global::Global.Settings.avatarLightDir.z -= 360;
    //        while (global::Global.Settings.avatarLightDir.z < 0)
    //            global::Global.Settings.avatarLightDir.z += 360;

    //        m_AvatarRenderer.m_LightRot = global::Global.Settings.avatarLightDir;
    //    }
    //}
    /// <summary>
    /// 切换页签
    /// </summary>
    /// <param name="index"></param>
    public void UpdateToggleUI(int index = -1)
    {
        //如果传过来是-1，则刷新当前页签
        if(index == -1)
        {
            titleList = TitleDataManager.GetInstance().getTitleListForSubType(selectToggleIndex);
        }
        else
        {
            titleList = TitleDataManager.GetInstance().getTitleListForSubType(index);
        }
        
        if (titleList != null)
        {
            if (titleList.Count > 0)
            {
                if(selectTitleData.guid != 0)
                {
                    _UpdateSelectItem(selectTitleData);
                }
                else
                {
                    _UpdateSelectItem(titleList[0]);
                }
            }
            titleComUIList.SetElementAmount(titleList.Count);
        }
        if(titleList == null || titleList.Count == 0)
        {
            //处理没有称号的时候的ui处理
            NoTitleTips.CustomActive(true);
            SetGameObjectIsShow(false);
        }
        else
        {
            NoTitleTips.CustomActive(false);
        }
        
    }

    void SetGameObjectIsShow(bool isFlag)
    {
        mDesRoot.CustomActive(isFlag);
        mSourceRoot.CustomActive(isFlag);
        mTitleImg.CustomActive(isFlag);
        TakeUp.CustomActive(isFlag);
        TakeOff.CustomActive(isFlag);
        mTitleRoot.CustomActive(isFlag);
    }

    /// <summary>
    /// 选择称号后刷新
    /// </summary>
    /// <param name="titleData"></param>
    private void _UpdateSelectItem(PlayerTitleInfo titleData)
    {
        var newTitleTableItem = TableManager.GetInstance().GetTableItem<NewTitleTable>((int)titleData.titleId);
        if (newTitleTableItem != null)
        {
            selectTitleData = titleData;

            if (TitleName != null)
                TitleName.CustomActive(false);
            if (mTitleImg != null)
                mTitleImg.CustomActive(false);
            if (mTitleRoot != null)
                mTitleRoot.CustomActive(false);
            if (mHonorImg != null)
                mHonorImg.CustomActive(false);

            if (titleData.style==(int)TitleDataManager.eTitleStyle.Txt)
            {
                if (mTitleRoot != null)
                    mTitleRoot.CustomActive(true);
                if (TitleName != null)
                    TitleName.CustomActive(true);

                if (TitleName!=null)
                {
                    TitleName.text = titleData.name;
                }
             
            }else if(titleData.style == (int)TitleDataManager.eTitleStyle.Img)
            {
                if (mTitleImg != null)
                    mTitleImg.CustomActive(true);

                if (mTitleImg != null)
                {
                    ETCImageLoader.LoadSprite(ref mTitleImg, newTitleTableItem.path);
                    mTitleImg.SetNativeSize();
                }
            }else if (titleData.style == (int)TitleDataManager.eTitleStyle.Group)
            {
                if (mTitleRoot != null)
                    mTitleRoot.CustomActive(true);
                if (TitleName != null)
                    TitleName.CustomActive(true);
                if (mHonorImg != null)
                    mHonorImg.CustomActive(true);

                if (TitleName != null)
                {
                    TitleName.text = titleData.name;
                }

                if (mHonorImg != null)
                {
                    ETCImageLoader.LoadSprite(ref mHonorImg, newTitleTableItem.path);
                    mHonorImg.SetNativeSize();
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(mTitleRoot.GetComponent<RectTransform>());
            }
           
            TitleDes.text = newTitleTableItem.Describe;
            TitleSource.text = newTitleTableItem.SourceDescribe;

            if(titleData.guid == PlayerBaseData.GetInstance().TitleGuid)
            {
                TakeUp.CustomActive(false);
                TakeOff.CustomActive(true);
            }
            else
            {
                TakeUp.CustomActive(true);
                TakeOff.CustomActive(false);
            }
        }
    }

    private void _UpdateItem(ComUIListElementScript item)
    {
        ComCommonBind mBind = item.GetComponent<ComCommonBind>();
        if (mBind == null)
        {
            return;
        }

        var mHaveEquip = mBind.GetGameObject("haveEquip");
        var mItemName = mBind.GetCom<Text>("itemName");
        var mItem_Img = mBind.GetCom<Image>("Item_Img");//图片称号
        var mSelectToggle = mBind.GetCom<Toggle>("selectToggle");
        var mHaveSelect = mBind.GetGameObject("haveSelect");
        GameObject mTitleRoot = mBind.GetGameObject("TitleRoot");
        Image mHonorImg = mBind.GetCom<Image>("HonorImg");

        if (mItemName != null)
            mItemName.CustomActive(false);

        if (mItem_Img != null)
            mItem_Img.CustomActive(false);

        if (mHonorImg != null)
            mHonorImg.CustomActive(false);

        if (mTitleRoot != null)
            mTitleRoot.CustomActive(false);

        PlayerTitleInfo titleData = titleList[item.m_index];
        var newTitleTableItem = TableManager.GetInstance().GetTableItem<NewTitleTable>((int)titleData.titleId);
        if (newTitleTableItem != null)
        {
            //mItemName.text = newTitleTableItem.name;
            var playerTitleInfo= titleList[item.m_index];
            if (playerTitleInfo == null) return;
            if(newTitleTableItem.Style==(int)TitleDataManager.eTitleStyle.Txt)
            {
                if (mItemName != null)
                    mItemName.CustomActive(true);

                if (mTitleRoot != null)
                    mTitleRoot.CustomActive(true);

                if (mItemName != null)
                    mItemName.text = playerTitleInfo.name;
            }
            else if(newTitleTableItem.Style == (int)TitleDataManager.eTitleStyle.Img)
            {
                if (mItem_Img != null)
                    mItem_Img.CustomActive(true);

                if (mItem_Img != null)
                {
                    ETCImageLoader.LoadSprite(ref mItem_Img, newTitleTableItem.path);
                    mItem_Img.SetNativeSize();
                }
            }
            else if(newTitleTableItem.Style == (int)TitleDataManager.eTitleStyle.Group)
            {
                if (mItemName != null)
                    mItemName.CustomActive(true);

                if (mHonorImg != null)
                    mHonorImg.CustomActive(true);

                if (mTitleRoot != null)
                    mTitleRoot.CustomActive(true);

                if (mItemName != null)
                    mItemName.text = playerTitleInfo.name;

                if (mHonorImg != null)
                {
                    ETCImageLoader.LoadSprite(ref mHonorImg, newTitleTableItem.path);
                    mHonorImg.SetNativeSize();
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(mTitleRoot.GetComponent<RectTransform>());
            }
           
            mSelectToggle.onValueChanged.RemoveAllListeners();
            mSelectToggle.onValueChanged.AddListener((value) =>
            {
                if(value)
                {
                    _UpdateSelectItem(titleList[item.m_index]);
                    mHaveSelect.CustomActive(true);
                }
                else
                {
                    mHaveSelect.CustomActive(false);
                }
            });
            if(PlayerBaseData.GetInstance().TitleGuid == titleList[item.m_index].guid)
            {
                mHaveEquip.CustomActive(true);
            }
            else
            {
                mHaveEquip.CustomActive(false);
            }

            if(selectTitleData.guid == titleList[item.m_index].guid)
            {
                mSelectToggle.isOn = true;
            }
        }
    }
}
