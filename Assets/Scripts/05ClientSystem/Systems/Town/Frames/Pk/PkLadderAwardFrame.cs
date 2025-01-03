using UnityEngine;
using ProtoTable;
using System.Collections.Generic;
using UnityEngine.UI;

namespace GameClient
{
    public class PkLadderAwardFrame : ClientFrame
    {
        string elePath = "UIFlatten/Prefabs/Pk/PkLadderAwardEle";

        List<GameObject> ObjList = new List<GameObject>();

        List<ItemData> ShowTipItemData1 = new List<ItemData>();
        List<ItemData> ShowTipItemData2 = new List<ItemData>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk/PkLadderAward";
        }

        protected override void _OnOpenFrame()
        {
            InitInterface();
            BindUIEvent();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.pkGuideEnd);
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.pkGuideStart);
            ClearData();
            UnBindUIEvent();
        }

        void ClearData()
        {
            ObjList.Clear();
            ShowTipItemData1.Clear();
            ShowTipItemData2.Clear();
        }

        protected void BindUIEvent()
        {
        }

        protected void UnBindUIEvent()
        {
        }

        [UIEventHandle("middle/title/btClose")]
        void OnClose()
        {          
            frameMgr.CloseFrame(this);
        }

        void OnShowItemTip1(int iIndex)
        {
            if (iIndex >= ShowTipItemData1.Count)
            {
                return;
            }

            ItemTipManager.GetInstance().ShowTip(ShowTipItemData1[iIndex]);
        }

        void OnShowItemTip2(int iIndex)
        {
            if (iIndex >= ShowTipItemData2.Count)
            {
                return;
            }

            ItemTipManager.GetInstance().ShowTip(ShowTipItemData2[iIndex]);
        }

        void InitInterface()
        {
            var TableData = TableManager.GetInstance().GetTable<SeasonDailyTable>();
            var enumerator = TableData.GetEnumerator();

            int iIndex = 0;
            while (enumerator.MoveNext())
            {
                var Item = enumerator.Current.Value as SeasonDailyTable;

                GameObject EleObj = AssetLoader.instance.LoadResAsGameObject(elePath);
                if (EleObj == null)
                {
                    continue;
                }

                Utility.AttachTo(EleObj, AwardRoot);
                ObjList.Add(EleObj);

                Text[] texts = EleObj.GetComponentsInChildren<Text>();
                for(int i = 0; i < texts.Length; i++)
                {
                    if(texts[i].name == "score")
                    {
                        if(Item.MatchScore.Count > 1)
                        {
                            texts[i].text = string.Format("{0}+", Item.MatchScore[0]);
                        }
                    }
                }

                RectTransform[] rects = EleObj.GetComponentsInChildren<RectTransform>();
                for(int i = 0; i < rects.Length; i++)
                {
                    if(rects[i].name == "pos1")
                    {
                        if(Item.Rewards.Count >= 1)
                        {
                            string[] itemShowdata = Item.Rewards[0].Split(new char[] { ',' });

                            ItemData data = ItemDataManager.CreateItemDataFromTable(int.Parse(itemShowdata[0]));
                            if (data == null)
                            {
                                continue;
                            }
                            data.Count = int.Parse(itemShowdata[1]);

                            ComItem ShowItem = CreateComItem(rects[i].gameObject);

                            int idx = iIndex;
                            ShowItem.Setup(data, (GameObject obj, ItemData item) => { OnShowItemTip1(idx); });

                            ShowTipItemData1.Add(data);
                        }                        
                    }
                    else if(rects[i].name == "pos2")
                    {
                        if(Item.Rewards.Count >= 2)
                        {
                            string[] itemShowdata = Item.Rewards[1].Split(new char[] { ',' });

                            ItemData data = ItemDataManager.CreateItemDataFromTable(int.Parse(itemShowdata[0]));
                            if (data == null)
                            {
                                continue;
                            }
                            data.Count = int.Parse(itemShowdata[1]);

                            ComItem ShowItem = CreateComItem(rects[i].gameObject);

                            int idx = iIndex;
                            ShowItem.Setup(data, (GameObject obj, ItemData item) => { OnShowItemTip2(idx); });

                            ShowTipItemData2.Add(data);
                        }                 
                    }
                }

                iIndex++;
            }
        }

        [UIObject("middle/Scroll View/Viewport/Content")]
        protected GameObject AwardRoot;
    }
}
