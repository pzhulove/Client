using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{

    public class ItemTipModelAvatarEnumerationItem : MonoBehaviour
    {

        private ItemTipModelAvatarEnumerationDataModel _itemTipModelAvatarEnumerationDataModel;
        private OnItemTipModelAvatarEnumerationItemClick _onItemTipModelAvatarEnumerationItemClick;

        [Space(10)]
        [HeaderAttribute("ItemContent")]
        [Space(10)]
        [SerializeField] private Image itemBg;
        [SerializeField] private Image itemIcon;
        [SerializeField] private Text itemNameLabel;
        [Space(10)]
        [HeaderAttribute("PlayerProfession")]
        [Space(10)]
        [SerializeField] private Image playerFrameIcon;

        [Space(10)]
        [HeaderAttribute("ItemSelected")] [Space(10)]
        [SerializeField] private GameObject itemSelectFlag;
        [SerializeField] private Button itemSelectButton;

        private void Awake()
        {
            BindUiEvents();
        }

        private void OnDestroy()
        {
            UnBindUiEvents();
            ClearData();
        }

        private void BindUiEvents()
        {
            if (itemSelectButton != null)
            {
                itemSelectButton.onClick.RemoveAllListeners();
                itemSelectButton.onClick.AddListener(OnItemSelectButtonClicked);
            }
        }

        private void UnBindUiEvents()
        {
            if(itemSelectButton != null)
                itemSelectButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _itemTipModelAvatarEnumerationDataModel = null;
        }


        public void InitItem(ItemTipModelAvatarEnumerationDataModel itemTipModelAvatarDataModel,
            OnItemTipModelAvatarEnumerationItemClick onItemTipModelAvatarEnumerationItemClick)
        {
            _itemTipModelAvatarEnumerationDataModel = itemTipModelAvatarDataModel;
            _onItemTipModelAvatarEnumerationItemClick = onItemTipModelAvatarEnumerationItemClick;

            if (_itemTipModelAvatarEnumerationDataModel == null)
                return;

            CommonUtility.UpdateImageVisible(itemBg, false);
            CommonUtility.UpdateImageVisible(playerFrameIcon, false);

            if (_itemTipModelAvatarEnumerationDataModel.IsPlayerProfessionType == true)
            {
                CommonUtility.UpdateImageVisible(playerFrameIcon, true);
                //职业头像
                InitPlayerItem();
            }
            else
            {
                CommonUtility.UpdateImageVisible(itemBg, true);
                //道具
                InitItemView();
            }
        }

        private void InitPlayerItem()
        {
            
            var jobTable = TableManager.GetInstance()
                .GetTableItem<JobTable>(_itemTipModelAvatarEnumerationDataModel.ProfessionId);
            if (jobTable == null)
                return;

            //名字
            if (itemNameLabel != null)
                itemNameLabel.text = jobTable.Name;

            //职业头像
            if (itemIcon != null)
            {
                var playerIconPath =
                    PlayerUtility.GetPlayerProfessionHeadIconPath(_itemTipModelAvatarEnumerationDataModel.ProfessionId);
                if (string.IsNullOrEmpty(playerIconPath) == false)
                {
                    ETCImageLoader.LoadSprite(ref itemIcon, playerIconPath);
                }
            }

            //更新选中的内容
            UpdateItemSelectedFlag();
        }

        //初始化
        private void InitItemView()
        {
            var itemTable = _itemTipModelAvatarEnumerationDataModel.ItemTable;
            if (itemTable == null)
                return;

            //背景
            var qualityInfo = ItemData.GetQualityInfo(itemTable.Color);
            if (itemBg != null && qualityInfo != null)
                ETCImageLoader.LoadSprite(ref itemBg, qualityInfo.Background);

            //宠物
            if (itemTable.SubType == ItemTable.eSubType.PetEgg)
            {
                var petId = ItemDataUtility.GetPetIdByItemTable(itemTable);
                var petTable = TableManager.GetInstance().GetTableItem<PetTable>(petId);
                if (petTable != null)
                {
                    if (itemIcon != null)
                        ETCImageLoader.LoadSprite(ref itemIcon, petTable.IconPath);

                    if (itemNameLabel != null)
                    {
                        var petNameStr = CommonUtility.GetPetItemName(petTable);
                        itemNameLabel.text = petNameStr;
                    }
                }
            }
            else
            {
                if(itemIcon != null)
                    ETCImageLoader.LoadSprite(ref itemIcon, itemTable.Icon);

                if (itemNameLabel != null)
                {
                    var itemNameStr = CommonUtility.GetItemColorName(itemTable);
                    itemNameLabel.text = itemNameStr;
                }
            }
            
            UpdateItemSelectedFlag();

        }

        //更新
        public void UpdateItem()
        {
            UpdateItemSelectedFlag();
        }

        //回收
        public void RecycleItem()
        {
            _itemTipModelAvatarEnumerationDataModel = null;
        }


        //点击
        private void OnItemSelectButtonClicked()
        {
            if (_itemTipModelAvatarEnumerationDataModel == null)
                return;

            if (_itemTipModelAvatarEnumerationDataModel.IsSelectedFlag == true)
                return;

            _itemTipModelAvatarEnumerationDataModel.IsSelectedFlag = true;
            UpdateItemSelectedFlag();

            if (_onItemTipModelAvatarEnumerationItemClick != null)
                _onItemTipModelAvatarEnumerationItemClick(_itemTipModelAvatarEnumerationDataModel.Index);
        }

        private void UpdateItemSelectedFlag()
        {
            if (_itemTipModelAvatarEnumerationDataModel == null)
                return;

            CommonUtility.UpdateGameObjectVisible(itemSelectFlag,
                _itemTipModelAvatarEnumerationDataModel.IsSelectedFlag);
        }
    }
}
