using GameClient;
using ProtoTable;
using UnityEngine.UI;

public class PlayerTryOnFrame : ClientFrame {

    public override string GetPrefabPath()
    {
        return "UIFlatten/Prefabs/Common/PlayerTryOnFrame";
    }
    private int mAnniversaryItemId = 535002503;//周年派对礼包的预览。由于预览的人物比较大，显示不了全部，所以要特殊处理一下
    protected sealed override void _OnOpenFrame()
    {
        int itemId = (int) userData;
        var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);
        if(itemTableData == null)
        {
            return;
        }
        if(itemTableData.SubType == ItemTable.eSubType.GiftPackage)
        {
            var giftPackTableData = TableManager.GetInstance().GetTableItem<GiftPackTable>(itemTableData.PackageID);
            if(giftPackTableData == null)
            {
                return;
            }
            var giftList = TableManager.GetInstance().GetGiftTableData(giftPackTableData.ID);

            bool haveInitAvartar = false;
            for (int i = 0;i<giftList.Count;i++)
            {
                var giftTableData = giftList[i];
                if(giftTableData == null)
                {
                    continue;
                }
                if(!giftTableData.RecommendJobs.Contains(PlayerBaseData.GetInstance().JobTableID))
                {
                    continue;
                }
                if(!haveInitAvartar)
                {
                    haveInitAvartar = true;
                    _InitAvatar(giftTableData.ItemID);
                }
                else
                {
                    ItemTable tableData = TableManager.GetInstance().GetTableItem<ItemTable>(giftTableData.ItemID);
                    if(tableData == null)
                    {
                        continue;
                    }
                    EFashionWearSlotType slotType = GetEquipSlotType(tableData);
                    PlayerBaseData.GetInstance().AvatarEquipPart(mAvartarRenderer, slotType, tableData.ID);
                }
            }
            mAvartarRenderer.ChangeAction("Anim_Show_Idle", 1f, true);
        }
        else
        {
            _InitAvatar(itemId);
        }
    }

    public void Reset(int itemId)
    {
        _InitAvatar(itemId);
    }

    public override bool IsNeedUpdate()
    {
        return true;
    }

    protected override void _OnUpdate(float timeElapsed)
    {
        base._OnUpdate(timeElapsed);

        if (null != mAvartarRenderer)
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

            mAvartarRenderer.m_LightRot = global::Global.Settings.avatarLightDir;
        }
    }

    void _InitAvatar(int itemId)
    {
        int iJobID = PlayerBaseData.GetInstance().JobTableID;

        JobTable job = TableManager.instance.GetTableItem<JobTable>(iJobID);
        if (job == null)
        {
            Logger.LogErrorFormat("can not find JobTable with id:{0}", iJobID);
        }
        else
        {
            ResTable res = TableManager.instance.GetTableItem<ResTable>(job.Mode);

            if (res == null)
            {
                Logger.LogErrorFormat("can not find ResTable with id:{0}", job.Mode);
            }
            else
            {
                mAvartarRenderer.ClearAvatar();
                SetCameraPosYZ(itemId);
                mAvartarRenderer.LoadAvatar(res.ModelPath);

                if (iJobID == PlayerBaseData.GetInstance().JobTableID)
                {
                    PlayerBaseData.GetInstance().AvatarEquipFromCurrentEquiped(mAvartarRenderer);
                }

                mAvartarRenderer.AttachAvatar("Aureole", "Effects/Scene_effects/Effectui/EffUI_chuangjue_fazhen_JS", "[actor]Orign", false);
                mAvartarRenderer.SuitAvatar();
                if (itemId > 0)
                {
                    ItemTable tableData = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);
                    if (tableData != null)
                    {
                        if (tableData.SubType == ItemTable.eSubType.FASHION_HAIR)
                        {
                            PlayerBaseData.GetInstance().AvatarEquipWing(mAvartarRenderer, tableData.ID);
                        }
                        else if (tableData.SubType == ItemTable.eSubType.FASHION_AURAS)
                        {
                            PlayerBaseData.GetInstance().AvatarEquipHalo(mAvartarRenderer, tableData.ID);
                        }
                        else if (tableData.SubType == ItemTable.eSubType.WEAPON)
                        {
                            PlayerBaseData.GetInstance().AvatarEquipWeapon(mAvartarRenderer, iJobID, tableData.ID);
                        }
                        else if (tableData.SubType == ItemTable.eSubType.FASHION_WEAPON)
                        {
                            PlayerBaseData.GetInstance().AvatarEquipWeapon(mAvartarRenderer, iJobID, tableData.ID);
                        }
                        else
                        {
                            EFashionWearSlotType slotType = GetEquipSlotType(tableData);
                            PlayerBaseData.GetInstance().AvatarEquipPart(mAvartarRenderer, slotType, tableData.ID);
                        }
                    }
                }
                mAvartarRenderer.ChangeAction("Anim_Show_Idle", 1f, true);
            }
        }
    }

    EFashionWearSlotType GetEquipSlotType(ItemTable ItemTableData)
    {
        if (ItemTableData.SubType == ItemTable.eSubType.FASHION_HEAD)
        {
            return EFashionWearSlotType.Head; // 头饰
        }
        else if (ItemTableData.SubType == ItemTable.eSubType.FASHION_CHEST)
        {
            return EFashionWearSlotType.UpperBody; // 上装
        }
        else if (ItemTableData.SubType == ItemTable.eSubType.FASHION_EPAULET)
        {
            return EFashionWearSlotType.Chest; // 胸饰
        }
        else if (ItemTableData.SubType == ItemTable.eSubType.FASHION_LEG)
        {
            return EFashionWearSlotType.LowerBody; // 下装
        }
        else if (ItemTableData.SubType == ItemTable.eSubType.FASHION_SASH)
        {
            return EFashionWearSlotType.Waist; // 腰饰
        }
        else
        {
            return EFashionWearSlotType.Invalid;
        }
    }

    #region ExtraUIBind
    private Button mButtonClosePanel = null;
	private GeAvatarRendererEx mAvartarRenderer = null;
	
	protected override void _bindExUI()
	{
		mButtonClosePanel = mBind.GetCom<Button>("ButtonClosePanel");
		if (null != mButtonClosePanel)
		{
			mButtonClosePanel.onClick.AddListener(_onButtonClosePanelButtonClick);
		}
		mAvartarRenderer = mBind.GetCom<GeAvatarRendererEx>("Actorpos");
	}
	
	protected override void _unbindExUI()
	{
		if (null != mButtonClosePanel)
		{
			mButtonClosePanel.onClick.RemoveListener(_onButtonClosePanelButtonClick);
		}
		mButtonClosePanel = null;
		mAvartarRenderer = null;
	}
    #endregion

    private void _onButtonClosePanelButtonClick()
    {
        Close();
    }

    /// <summary>
    /// 预览的人物比较大，显示不了全部，所以要特殊处理一下
    /// </summary>
    /// <param name="itemId"></param>
    private void SetCameraPosYZ(int itemId)
    {
        if(mAvartarRenderer!=null)
         {
            if(mAnniversaryItemId == itemId)
            {
                mAvartarRenderer.m_CameraPos.y = 1.74f;
                mAvartarRenderer.m_CameraPos.z = -4.5f;
            }
          
         }
        
    }
}
