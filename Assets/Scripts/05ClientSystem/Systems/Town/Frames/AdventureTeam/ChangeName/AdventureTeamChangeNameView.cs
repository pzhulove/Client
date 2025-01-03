using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AdventureTeamChangeNameView : MonoBehaviour
    {
        #region MODEL PARAMS

        AdventureTeamRenameModel renameModel = null;

        private string tr_title_adventure_team_change_name = "";
        private string tr_common_msg_btn_ok = "";

        #endregion

        #region VIEW PARAMS
        private int _limitNumber = 7;

        [Space(10)]
        [HeaderAttribute("Title")]
        [SerializeField] private Text titleLabel = null;

        [Space(10)] [HeaderAttribute("nameInput")]
        [SerializeField] private InputField nameInputFiled;
        [SerializeField] private Text nameInputPlaceHolder;
        [SerializeField] private Text nameInputNumberLabel;

        [Space(10)]
        [HeaderAttribute("Button")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button okButton;
        [SerializeField] private SetComButtonCD comBtnCD;
        [SerializeField] private Text okButtonText;

        [SerializeField] private float waitTimeToUseRenameCard = 1f;

        #endregion


        #region PIRVATE METHODS

        private void Awake()
        {
            _BindUIEvents();
            _InitTR();
        }

        private void OnDestroy()
        {
            _UnBindUIEvents();
        }

        private void _BindUIEvents()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(_OnCloseButtonClickCallBack);
            }

            if (okButton != null)
            {
                okButton.onClick.AddListener(_OnOkButtonClickCallBack);
            }

            if (nameInputFiled != null)
            {
                nameInputFiled.onValueChanged.AddListener(_OnNameInputValueChanged);
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamRenameSucc, _OnAdventureTeamRenameSucc);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamRenameCardBuySucc, _OnBuyAdventureTeamRenameCardSucc);
        }

        private void _UnBindUIEvents()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveListener(_OnCloseButtonClickCallBack);

            if (okButton != null)
                okButton.onClick.RemoveListener(_OnOkButtonClickCallBack);

            if (nameInputFiled != null)
                nameInputFiled.onValueChanged.RemoveListener(_OnNameInputValueChanged);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamRenameSucc, _OnAdventureTeamRenameSucc);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamRenameCardBuySucc, _OnBuyAdventureTeamRenameCardSucc);
        }

        private void _InitTR()
        {
            tr_title_adventure_team_change_name = TR.Value("adventure_team_change_name");
            tr_common_msg_btn_ok = TR.Value("common_data_sure");
        }

        private void _ClearView()
        {
            tr_title_adventure_team_change_name = null;
            tr_common_msg_btn_ok = null;

            renameModel = null;
        }

        private void _InitBaseData()
        {
            if (titleLabel != null)
                titleLabel.text = tr_title_adventure_team_change_name;

            if (okButtonText != null)
                okButtonText.text = tr_common_msg_btn_ok;
        }

        private void _InitNameInputData()
        {
            _limitNumber = AdventureTeamDataManager.GetInstance().RenameLimitCharNum;
            //初始化
            if (nameInputFiled != null)
            {
                //限制字数最少为7个数字
                if (nameInputFiled.characterLimit > _limitNumber)
                    _limitNumber = nameInputFiled.characterLimit;
            }

            if (nameInputPlaceHolder != null)
            {
                nameInputPlaceHolder.text = TR.Value("adventure_team_input_new_name");
            }

            //更新数字
            _UpdateNameInputData();
        }

        private void _UpdateNameInputData()
        {
            if(nameInputNumberLabel != null
               && nameInputFiled != null 
               && nameInputFiled.text != null)
            {
                nameInputNumberLabel.text = string.Format(TR.Value("adventure_team_name_count"),
                    nameInputFiled.text.Length,
                    _limitNumber);

                if (this.renameModel != null)
                {
                    this.renameModel.newNameStr = nameInputFiled.text;
                }
            }
        }

        private void _CloseFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<AdventureTeamChangeNameFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<AdventureTeamChangeNameFrame>();
            }
        }

        private void _OnNameInputValueChanged(string text)
        {
            _UpdateNameInputData();
        }

        private void _OnCloseButtonClickCallBack()
        {
            _CloseFrame();           
        }

        private void _OnOkButtonClickCallBack()
        {
            if (comBtnCD == null)
            {
                return;
            }
            if (comBtnCD.IsBtnWork())
            {
                if (this.renameModel != null)
                {
                    //新增流程 ： 先尝试使用 后判断 再购买 最后生效
                    _TryUseOrBuyAdventureTeamRenameCard();
                }
            }
        }

        private void _OnAdventureTeamRenameSucc(UIEvent _uiEvent)
        {
            _CloseFrame();
        }

        private void _OnBuyAdventureTeamRenameCardSucc(UIEvent _uiEvent)
        {
            if (_uiEvent != null && _uiEvent.Param1 != null)
            {
                var tempItemData = _uiEvent.Param1 as ItemSimpleData;
                if (tempItemData == null)
                {
                    return;
                }
                var mallItemTable = TableManager.GetInstance().GetTableItem<ProtoTable.MallItemTable>(tempItemData.ItemID);
                if (null == mallItemTable)
                {
                    return;
                }
                if (mallItemTable.itemid == AdventureTeamDataManager.GetInstance().RenameCardTableId)
                {
                    _TryUseOrBuyAdventureTeamRenameCard();
                }
            }
        }

        private void _TryUseOrBuyAdventureTeamRenameCard()
        {
            if (this.renameModel == null)
            {
                return;
            }

            List<ulong> adventureTeamRenameCardGUIDs = ItemDataManager.GetInstance().GetItemsByPackageThirdType(EPackageType.Consumable, 
                ProtoTable.ItemTable.eSubType.ChangeName, 
                ProtoTable.ItemTable.eThirdType.ChangeAdventureName);
            if (adventureTeamRenameCardGUIDs == null || adventureTeamRenameCardGUIDs.Count <= 0)
            {
                AdventureTeamDataManager.GetInstance().QuickMallBuyAndChangeTeamName();
            }
            else
            {
                ulong itemGUID = adventureTeamRenameCardGUIDs[0];
                ItemData itemData = ItemDataManager.GetInstance().GetItem(itemGUID);
                this.renameModel.renameItemGUID = itemGUID;
                if (itemData != null)
                {
                    this.renameModel.renameItemTableId = (uint)itemData.TableID;
                }
                AdventureTeamDataManager.GetInstance().ReqChangeTeamName(this.renameModel);
            }
        }

        #endregion

        #region PUBLIC METHODS

        public void InitData(AdventureTeamRenameModel model)
        {
            this.renameModel = model;
            _InitBaseData();
            _InitNameInputData();
        }

        public void Clear()
        {
            _ClearView();
        }

        #endregion
    }
}