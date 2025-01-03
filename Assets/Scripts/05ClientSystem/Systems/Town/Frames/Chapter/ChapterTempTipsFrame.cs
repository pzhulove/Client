using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Protocol;
using Network;
using ProtoTable;


namespace GameClient
{
    public class ChapterTempTipsFrame : ClientFrame
    {
        private int mTunBenBuffid = 55003;//团本buffID
        public static void Show(int id)
        {
            ChapterTempTipsFrame frame = ClientSystemManager.instance.OpenFrame<ChapterTempTipsFrame>() as ChapterTempTipsFrame;

            if (null != frame)
            {
                frame.SetID(id);
            }
        }

#region ExtraUIBind
        private Image mIcon   = null;
        private Text mName    = null;
        private Text mDesc    = null;
        private Text mColor   = null;
        private Button mClose = null;
        private Image mIconBg = null;
        private Image mBg     = null;

        private Text mMaybeDropTxt;

        protected override void _bindExUI()
        {
            mIcon             = mBind.GetCom<Image>("icon");
            mName             = mBind.GetCom<Text>("name");
            mDesc             = mBind.GetCom<Text>("desc");
            mColor            = mBind.GetCom<Text>("color");
            mClose            = mBind.GetCom<Button>("close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mIconBg           = mBind.GetCom<Image>("iconBg");
            mBg               = mBind.GetCom<Image>("bg");
            mMaybeDropTxt     = mBind.GetCom<Text>("MaybeDropTxt");
        }

        protected override void _unbindExUI()
        {
            mIcon             = null;
            mName             = null;
            mDesc             = null;
            mColor            = null;
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose            = null;
            mIconBg           = null;
            mBg               = null;
            mMaybeDropTxt     = null;
        }
#endregion  

#region Callback
        private void _onCloseButtonClick()
        {
            /* put your code in here */

            _onClose();
        }
#endregion 


        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chapter/Normal/ChapterCollectTipsContent";
        }

        protected override void _OnOpenFrame()
        {
        }

        protected override void _OnCloseFrame()
        {

        }

        public void SetID(int id)
        {
            ItemCollectionTable items = TableManager.instance.GetTableItem<ItemCollectionTable>(id);
            if (null != items)
            {
                string bgpath = string.Empty;
                string collection = string.Empty;

                List<int> quas = new List<int>(items.Color);
                quas.Sort();

                ItemData.QualityInfo maxqi = null;
                ItemData.QualityInfo minqi = null;

                if (quas.Count > 0)
                {
                    try 
                    { 
                        maxqi = ItemData.GetQualityInfo((ItemTable.eColor)quas[quas.Count-1], items.Color2);
                        minqi = ItemData.GetQualityInfo((ItemTable.eColor)quas[0], items.Color2);
                    } 
                    catch 
                    {
                        maxqi = ItemData.GetQualityInfo(ItemTable.eColor.WHITE);
                        minqi = ItemData.GetQualityInfo(ItemTable.eColor.WHITE);

                    }


                    string bg = maxqi.Background;

                    if (maxqi == minqi || (maxqi.Quality == minqi.Quality))
                    {
                        collection = string.Format("{0}", _getColorString(maxqi.ColStr, maxqi.Desc));
                    }
                    else 
                    {
                        collection = string.Format("{0}-{1}", _getColorString(minqi.ColStr, minqi.Desc), _getColorString(maxqi.ColStr, maxqi.Desc));

                    }

                    // mIconBg.sprite = AssetLoader.instance.LoadRes(bg, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref mIconBg, bg);

                    if (maxqi.Quality == ItemTable.eColor.WHITE)
                    {
                        mBg.color = Color.clear;
                    }
                    else 
                    {
                        // mBg.sprite = AssetLoader.instance.LoadRes(maxqi.TitleBG2, typeof(Sprite)).obj as Sprite;
                        ETCImageLoader.LoadSprite(ref mBg, maxqi.TitleBG2);
                    }
                }



                // mIcon.sprite = AssetLoader.instance.LoadRes(items.Icon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref mIcon, items.Icon);
                mColor.text = collection;
                mName.text = items.Name;
                mDesc.text = items.Desc;
                //mLevel.text = items.Level;
            }

            if(mTunBenBuffid==id)//团本buff 将可能掉落字样隐藏
            {
                mMaybeDropTxt.SafeSetText(string.Empty);
            }
        }

        private string _getColorString(string color, string name)
        {
            return string.Format("<color={0}>{1}</color>", color, name);
        }

        private void _onClose()
        {
            ClientSystemManager.instance.CloseFrame(this);
        }
    }
}
