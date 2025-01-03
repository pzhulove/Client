using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    public class AwardItemData
    {
        public int ID;
        public int Num;
        public int EquipType;
        public int StrengthenLevel;
    }

    public class CommonItemAwardFrame : ClientFrame
    {
        const int OddNum = 9;
        const int EvenNum = 8;

        float TimeFlag = 0.0f;

        protected override void _OnOpenFrame()
        {
        }

        protected override void _OnCloseFrame()
        {
            TimeFlag = 0.0f;
        }

        public override string GetPrefabPath()
        {
			return "";
        }

        public void SetAwardItems(List<AwardItemData> ItemsData)
        {
            if (ItemsData.Count <= 0)
            {
                return;
            }

            int iMaxNum = 0;
            RectTransform[] Positions;
            Text[] PosNames;
         
            int flag = ItemsData.Count % 2;
            if (flag == 1)
            {
                iMaxNum = OddNum;
                Positions = Oddpos;
                PosNames = OddName;
            }
            else
            {
                iMaxNum = EvenNum;
                Positions = Evenpos;
                PosNames = EvenName;
            }

            for (int i = 0; i < ItemsData.Count && i < iMaxNum; i++)
            {
                ItemData data = ItemDataManager.CreateItemDataFromTable(ItemsData[i].ID);
                if (data == null)
                {
                    continue;
                }

                ItemTable TableData = TableManager.GetInstance().GetTableItem<ItemTable>(ItemsData[i].ID);
                if(TableData == null)
                {
                    continue;
                }

                ComItem ShowItem = CreateComItem(Positions[i].gameObject);
                data.Count = ItemsData[i].Num;
                ShowItem.Setup(data, null);

                PosNames[i].text = TableData.Name;
            }
        }

        public void SetTitle(string titlePath)
        {
            if (titlePath != "" || titlePath != "-")
            {
                return;
            }

            //Sprite Icon = AssetLoader.instance.LoadRes(titlePath, typeof(Sprite)).obj as Sprite;
            //if (Icon == null)
            //{
            //    return;
            //}

            // title.sprite = Icon;
            ETCImageLoader.LoadSprite(ref title, titlePath);
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            TimeFlag += timeElapsed;

            if (TimeFlag > 2.0f)
            {
                frameMgr.CloseFrame(this);
                return;
            }       
        }

        [UIControl("Dotween/title")]
        protected Image title;

        [UIControl("Dotween/OddPos/pos{0}", typeof(RectTransform), 1)]
        protected RectTransform[] Oddpos = new RectTransform[OddNum];

        [UIControl("Dotween/OddPos/pos{0}/name", typeof(Text), 1)]
        protected Text[] OddName = new Text[OddNum];

        [UIControl("Dotween/EvenPos/pos{0}", typeof(RectTransform), 1)]
        protected RectTransform[] Evenpos = new RectTransform[EvenNum];

        [UIControl("Dotween/EvenPos/pos{0}/name", typeof(Text), 1)]
        protected Text[] EvenName = new Text[EvenNum];
    }
}
