using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ProtoTable;

namespace GameClient
{
    public class ComChapterInfoDropUnit : MonoBehaviour
    {
        public ComCommonBind mBind;

        private enum eType 
        {
            None,
            Item,
            ItemCollection,
        }

        private eType mType = eType.None;
        private int   mID   = -1;

        public void Load(int id)
        {
            mID   = id;
            mType = _getType(id);
            _createItemCollection(id);
            _bind();
        }

        public void ShowName(bool show)
        {
            if (null != mBind)
            {
                Text name    = mBind.GetCom<Text>("name");
                name.enabled = show;
            }
        }

        public void ShowLevel(bool show)
        {
            if (null != mBind)
            {
                Text level   = mBind.GetCom<Text>("level");

                if (!string.IsNullOrEmpty(level.text))
                {
                    level.enabled = show;
                }
            }
        }

        public void ShowFatigueCombustionBuffRoot(int mDungeonID = 0)
        {
            string errormsg = "";
            try
            {
                if (null != mBind)
                {

                    GameObject go = mBind.GetGameObject("FatigueCombustionBuff");
                    if(go == null)
                    {
                        errormsg = "[go is empty]";
                    }
                    if (TableManager.GetInstance() == null)
                    {
                        errormsg += "[table is null]";
                    }
                    bool mBisFlag = false;
                    ActivityLimitTime.ActivityLimitTimeData data = null;
                    if(ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance() == null)
                    {
                        errormsg += "[ActivityLimitTimeCombineManager is null]";
                    }
                    if (ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager != null)
                    {
                        ActivityLimitTime.ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.FindFatigueCombustionActivityIsOpen(ref mBisFlag, ref data);
                    }

                    bool mBIsmainGate = false;
                    //mDungeonID 有可能为空 ，例如公会地下城
                    var mDungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(mDungeonID);
                    if (mDungeonTable != null)
                    {
                        if (mDungeonTable.SubType != DungeonTable.eSubType.S_NORMAL && mDungeonTable.SubType != DungeonTable.eSubType.S_WUDAOHUI)
                        {
                            mBIsmainGate = false;
                        }
                        else
                        {
                            mBIsmainGate = true;
                        }
                    }
                  
                    if (data != null)
                    {
                        bool isShow = false;
                        ActivityLimitTime.ActivityLimitTimeDetailData mActivityLimitTimeDetailData = null;
                        for (int i = 0; i < data.activityDetailDataList.Count; i++)
                        {
                            if (data.activityDetailDataList[i].ActivityDetailState == ActivityLimitTime.ActivityTaskState.Finished)
                            {
                                isShow = true;
                                mActivityLimitTimeDetailData = data.activityDetailDataList[i];
                            }
                        }

                        if (mActivityLimitTimeDetailData == null)
                        {
                            return;
                        }

                        // 精英地下城不参与精力燃烧活动
                        if(TeamUtility.IsEliteDungeonID(mDungeonID))
                        {
                            isShow = false;
                        }
                        bool mFlag = mBisFlag && mBIsmainGate && isShow;
                        var mTaskId = mActivityLimitTimeDetailData.DataId;

                        string mStrID = mTaskId.ToString();
                        string mStr = mStrID.Substring(mStrID.Length - 1);
                        int mIndex = 0;

                        if (int.TryParse(mStr, out mIndex))
                        {
                            if (mIndex == 1)
                            {
                                if (mType == eType.Item)
                                {
                                    var mItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(mID);
                                    if (mItemTable == null)
                                    {
                                        return;
                                    }

                                    if (mItemTable.SubType == ItemTable.eSubType.EXP)
                                    {
                                       
                                        go.CustomActive(mFlag);
                                        
                                    }
                                }
                            }
                            else
                            {
                                if (mType == eType.Item)
                                {
                                    var mItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(mID);
                                    if (mItemTable == null)
                                    {
                                        return;
                                    }

                                    if (mItemTable.SubType == ItemTable.eSubType.EXP || mItemTable.Color == ItemTable.eColor.PINK)
                                    {
                                        go.CustomActive(mFlag);
                                    }

                                }
                                else
                                {
                                    ItemCollectionTable itemCollectionTable = TableManager.instance.GetTableItem<ItemCollectionTable>(mID);
                                    if (itemCollectionTable == null)
                                    {
                                        return;
                                    }

                                    for (int i = 0; i < itemCollectionTable.Color.Count; i++)
                                    {
                                        if (itemCollectionTable.Color[i] != 5)
                                        {
                                            continue;
                                        }

                                        go.CustomActive(mFlag);
                                    }
                                }
                            }
                        }
                    }

                }
            }
            catch(System.Exception e)
            {
                Logger.LogErrorFormat("ShowFatigueCombustionBuffRoot:{0} reason {1}",e.Message, errormsg);
            }
        }

        public void CloseFatigueCombustionBuffRoot()
        {
            GameObject go = mBind.GetGameObject("FatigueCombustionBuff");
            if (null != go)
            {
                go.CustomActive(false);
            }
        }

        public void SetSize(Vector2 size)
        {
            if (null != mBind)
            {
                LayoutElement layout = mBind.GetCom<LayoutElement>("layout");

                if (size.x > 0)
                {
                    layout.preferredWidth = size.x;
                }

                if (size.y > 0)
                {
                    layout.preferredHeight = size.y;
                }
            }
        }


        public void Unload()
        {
            _unbind();
        }

        private eType _getType(int id)
        {
            ItemTable item = TableManager.instance.GetTableItem<ItemTable>(id);
            if (null != item)
            {
                return eType.Item;
            }

            ItemCollectionTable items = TableManager.instance.GetTableItem<ItemCollectionTable>(id);
            if (null != items)
            {
                return eType.ItemCollection;
            }
            
            Logger.LogErrorFormat("无法找到 id 为 {0} 的道具和集合", id);
            return eType.None;
        }

        private void _showItemTips(int id)
        {
            ItemData data = ItemDataManager.CreateItemDataFromTable(id);
            if (null != data)
            {
                ItemTipManager.GetInstance().ShowTip(data, null);
            }
        }

        private void _onClick()
        {
            switch (mType)
            {
                case eType.Item:
                    _showItemTips(mID);
                    break;
                case eType.ItemCollection:

                    ItemCollectionTable items = TableManager.instance.GetTableItem<ItemCollectionTable>(mID);
                    if (null != items)
                    {
                        if (items.TipsType == ItemCollectionTable.eTipsType.COLLECTION)
                        {
                            //List<int[]> tips = new List<int[]>();
                            List<int[]> tips = GamePool.ListPool<int[]>.Get();
                            for (int i = 0; i < items.TipsContent.Count; ++i)
                            {
                                if (items.TipsContent[i].valueType == UnionCellType.union_everyvalue)
                                {
                                    if (items.TipsContent[i].eValues.everyValues != null)
                                    {
                                        //tips.Add(items.TipsContent[i].eValues.everyValues.ToArray());
									var tmpValues = items.TipsContent[i].eValues.everyValues;
									int[] tmpListInt = new int[tmpValues.Count];


										for(int tmpi =0; tmpi < tmpValues.Count; ++tmpi)
										{
										tmpListInt[tmpi] = tmpValues[i];
											//tips.Add(tmpValues[tmpi]);
										}
									tips.Add(tmpListInt);

                                    }
                                }
                            }

                            if (tips.Count > 0)
                            {
                                ChapterInfoDropTipsFrame.ShowTips(tips);
                            }

                            GamePool.ListPool<int[]>.Release(tips);
                        }
                        else if (items.TipsType == ItemCollectionTable.eTipsType.SINGLE)
                        {
                            ChapterTempTipsFrame.Show(mID);
                        }
                    }

                    break;
            }
        }
    
        private void _bind()
        {
            if (null != mBind)
            {
                Button click = mBind.GetCom<Button>("button");
                if (null != click)
                {
                    click.onClick.AddListener(_onClick);
                }
            }
        }

        private void _unbind()
        {
            if (null != mBind)
            {
                Button click = mBind.GetCom<Button>("button");
                if (null != click)
                {
                    click.onClick.RemoveListener(_onClick);
                }
            }
        }

        private string _getColorString(string color, string name)
        {
            return string.Format("<color={0}>{1}</color>", color, name);
        }

        private void _createItemCollection(int id)
        {
            string bg         = string.Empty;
            string level      = string.Empty;
            string icon       = string.Empty;
            string name = string.Empty;

            ItemTable.eColor color = ItemTable.eColor.WHITE;
            int color2 = 0;

            switch (mType)
            {
                case eType.Item:
                    {
                        ItemTable items = TableManager.instance.GetTableItem<ItemTable>(id);
                        if (null != items)
                        {
                            ItemData.QualityInfo qi = ItemData.GetQualityInfo(items.Color, items.Color2);
                            
                            name = _getColorString(qi.ColStr, items.Name);
                           
                            level                   = string.Format("{0}级{1}", items.NeedLevel, items.SubTypeName);
                            icon                    = items.Icon;
                            bg                      = qi.Background;


                            color = items.Color;
                            color2 = items.Color2;
                            //if (items.NeedLevel <= 0)
                            {
                                level = string.Empty;
                            }
                        }
                    }
                    break;
                case eType.ItemCollection:
                    {
                        ItemCollectionTable items = TableManager.instance.GetTableItem<ItemCollectionTable>(id);
                        if (null != items)
                        {

                            List<int> quas = new List<int>(items.Color);
                            quas.Sort();

                            level                      = items.Level;
                            icon                       = items.Icon;

                            ItemData.QualityInfo maxqi = null;
                            ItemData.QualityInfo minqi = null;

                            if (quas.Count > 0)
                            {
                                try 
                                { 
                                    maxqi = ItemData.GetQualityInfo((ItemTable.eColor)quas[quas.Count-1]);
                                    minqi = ItemData.GetQualityInfo((ItemTable.eColor)quas[0]);
                                    color = (ItemTable.eColor)quas[quas.Count-1];
                                    color2 = items.Color2;
                                    if (color2 == 3)
                                    {
                                        if (minqi.Quality == ItemTable.eColor.PINK)
                                        {
                                            minqi = ItemData.GetQualityInfo((ItemTable.eColor) quas[0], color2);
                                        }

                                        if (maxqi.Quality == ItemTable.eColor.PINK)
                                        {
                                            maxqi = ItemData.GetQualityInfo((ItemTable.eColor) quas[quas.Count - 1],
                                                color2);
                                        }
                                    }
                                } 
                                catch 
                                {
                                    maxqi = ItemData.GetQualityInfo(ItemTable.eColor.WHITE);
                                    minqi = ItemData.GetQualityInfo(ItemTable.eColor.WHITE);
                                    color = ItemTable.eColor.WHITE;
                                    color2 = 0;
                                }

                                bg = maxqi.Background;

                                if (maxqi == minqi || (maxqi.Quality == minqi.Quality))
                                {
                                    name = string.Format("{0}", _getColorString(maxqi.ColStr, maxqi.Desc));
                                }
                                else 
                                {
                                    name = string.Format("{0}-{1}", _getColorString(minqi.ColStr, minqi.Desc), _getColorString(maxqi.ColStr, maxqi.Desc));
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            _createItemByQuality(color, icon, name, level, color2);
        }

        private void _createItemByQuality(ItemTable.eColor bgColor, string sicon, string sname, string slvl, int color2 = 0)
        {
            if (null != mBind)
            {
                Image bg     = mBind.GetCom<Image>("bg");
                Image icon   = mBind.GetCom<Image>("icon");
                Text name    = mBind.GetCom<Text>("name");
                Text level   = mBind.GetCom<Text>("level");


                // bg.sprite   = _getBgSprite(bgColor);
                _getBgSprite(bgColor, ref bg, color2);
                // icon.sprite = AssetLoader.instance.LoadRes(sicon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref icon, sicon);
                name.text   = sname.Trim();

                level.text = slvl.Trim();

                if (string.IsNullOrEmpty(level.text))
                {
                    level.enabled = false;
                }
            }
        }

        //private Sprite _getBgSprite(ItemTable.eColor color)
        //{
        //    if (null == mBind)
        //    {
        //        return null;
        //    }

        //    switch (color)
        //    {
        //        case ItemTable.eColor.CL_NONE:
        //        case ItemTable.eColor.WHITE:
        //            return mBind.GetSprite("white");
        //        case ItemTable.eColor.BLUE:
        //            return mBind.GetSprite("blue");
        //        case ItemTable.eColor.GREEN:
        //            return mBind.GetSprite("green");
        //        case ItemTable.eColor.PINK:
        //            return mBind.GetSprite("pink");
        //        case ItemTable.eColor.PURPLE:
        //            return mBind.GetSprite("purple");
        //        case ItemTable.eColor.YELLOW:
        //            return mBind.GetSprite("yellow");

        //    }

        //    return mBind.GetSprite("white");
        //}
        private void _getBgSprite(ItemTable.eColor color, ref Image image, int color2 = 0)
        {
            if (null == mBind)
            {
                return;
            }

            switch (color)
            {
                case ItemTable.eColor.CL_NONE:
                case ItemTable.eColor.WHITE:
                    mBind.GetSprite("white", ref image);
                    return;
                case ItemTable.eColor.BLUE:
                    mBind.GetSprite("blue", ref image);
                    return;
                case ItemTable.eColor.GREEN:
                    mBind.GetSprite("green", ref image);
                    return;
                case ItemTable.eColor.PINK:
                    //mBind.GetSprite("pink", ref image);
                    //Color1 = 5, Color2 = 3 粉色中的玫红
                    if (color2 == 3)
                    {
                        ItemData.QualityInfo qualityInfo = ItemData.GetQualityInfo(ItemTable.eColor.PINK, color2);
                        if (qualityInfo != null)
                        {
                            ETCImageLoader.LoadSprite(ref image, qualityInfo.Background);
                            return;
                        }
                    }
                    mBind.GetSprite("pink", ref image);
                    return;
                case ItemTable.eColor.PURPLE:
                    mBind.GetSprite("purple", ref image);
                    return;
                case ItemTable.eColor.YELLOW:
                    mBind.GetSprite("yellow", ref image);
                    return;

            }

            mBind.GetSprite("white", ref image);
            return;
        }
    }
}
