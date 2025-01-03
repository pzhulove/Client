using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class AchievementLevelUpPlayFrameData
    {
        public int iId = 2;
    }

    public class AchievementLevelUpPlayFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ActiveGroup/AchievementLevelUpPlayFrame";
        }

        AchievementLevelUpPlayFrameData _Data = null;
        [UIControl("custom_Image/ICON", typeof(Image))]
        Image _Image;
        [UIControl("custom_Image/TextIcon", typeof(Image))]
        Image _TextIcon;

        public static void CommandOpen(object argv)
        {
            ClientSystemManager.GetInstance().OpenFrame<AchievementLevelUpPlayFrame>(FrameLayer.Middle,argv);
        }

        protected override void _OnOpenFrame()
        {
            _Data = userData as AchievementLevelUpPlayFrameData;
            if(null == _Data)
            {
                _Data = new AchievementLevelUpPlayFrameData();
            }

            _AddButton("Close", () => { frameMgr.CloseFrame(this); });
            _LoadImage();
        }

        void _LoadImage()
        {
            var item = TableManager.GetInstance().GetTableItem<ProtoTable.AchievementLevelInfoTable>(_Data.iId);
            if(null != item)
            {
                ETCImageLoader.LoadSprite(ref _Image, item.Icon);
            }
            if(null != item)
            {
                ETCImageLoader.LoadSprite(ref _TextIcon, item.TextIcon);
            }
        }

        protected override void _OnCloseFrame()
        {
            _Data = null;
        }
    }
}