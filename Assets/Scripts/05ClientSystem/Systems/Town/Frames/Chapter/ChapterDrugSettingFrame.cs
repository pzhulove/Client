using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    public class ChapterDrugSettingFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chapter/Normal/ChapterDrugSettingFrame";
        }

#region ExtraUIBind
        private Text mTitleText;
        private ComChapterInfoDrug mBuffDrugInfo;
        private Button mSubmitButton;
        private UIGray mSubmitUIGray;

        protected override void _bindExUI()
        {
            mTitleText = mBind.GetCom<Text>("TitleText");
            mBuffDrugInfo = mBind.GetCom<ComChapterInfoDrug>("BuffDrugInfo");
            mSubmitButton = mBind.GetCom<Button>("SubmitButton");
            mSubmitButton.onClick.AddListener(_onSubmitButtonClick);
            mSubmitUIGray = mBind.GetCom<UIGray>("SubmitUIGray");
        }

        protected override void _unbindExUI()
        {
            mTitleText = null;
            mBuffDrugInfo = null;
            mSubmitButton.onClick.RemoveListener(_onSubmitButtonClick);
            mSubmitUIGray = null;
        }
        #endregion

        protected DungeonTable mDungeonTable = null;
        protected DungeonID mDungeonID = new DungeonID(0);



        protected override void _OnLoadPrefabFinish()
        {
            _loadData();
        }
        protected override void _OnOpenFrame()
        {
            ChapterBuffDrugManager.GetInstance().ResetBuffDrugsFromLocal(mDungeonTable.BuffDrugConfig);
            if(null != mBuffDrugInfo)
            {
                mBuffDrugInfo.SetBuffDrugs(mDungeonTable.BuffDrugConfig);
            }
        }

        protected override void _OnCloseFrame()
        {
            ChapterBuffDrugManager.GetInstance().ResetAllMarkedBuffDrugs();
            ChapterBuffDrugManager.GetInstance().ResetBuffDrugsFromLocal(mDungeonTable.BuffDrugConfig);
        }

        protected override void _OnUpdate(float delta)
        {

        }

        private void _onSubmitButtonClick()
        {
            ChapterBuffDrugManager.GetInstance().ResetAllLocalMarkedBuffDrugs(mDungeonTable.BuffDrugConfig);
            ChapterBuffDrugManager.GetInstance().SetMarkedBuffDrugsAtLocal();
            ChapterBuffDrugManager.GetInstance().SetBuffDrugToggleState(true);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BuffDrugSettingSubmit);
            this.Close();
        }

        protected virtual void _loadData()
        {
            //mDungeonID.dungeonID = ChapterBaseFrame.sDungeonID;
            var dungeonId = (int) userData;
            mDungeonID.dungeonID = dungeonId;

            mDungeonTable = _getDungeonTable<DungeonTable>(mDungeonID.dungeonID);
        }

        protected T _getDungeonTable<T>(int id)
        {
            var dungeonItem = TableManager.instance.GetTableItem<T>(id);
            if (dungeonItem != null)
            {
                return dungeonItem;
            }

            return default(T);
        }
    }
}