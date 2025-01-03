using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public class ChallengeChapterShopControl : MonoBehaviour
    {

        private int _dungeonId;             //地下城ID，
        private int _chapterShopId;

        [Space(20)]
        [HeaderAttribute("ShopRoot")]
        [Space(10)]
        [SerializeField] private Button shopButton;
        [SerializeField] private Image shopIcon;
        [SerializeField] private Text shopName;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
            if (shopButton != null)
            {
                shopButton.onClick.RemoveAllListeners();
                shopButton.onClick.AddListener(OnShopButtonClick);
            }
        }

        private void UnBindEvents()
        {
            if (shopButton != null)
                shopButton.onClick.RemoveAllListeners();
        }
        
        private void ClearData()
        {
            _dungeonId = 0;
            _chapterShopId = 0;
        }

        public void InitShopControl(int dungeonId)
        {
            _dungeonId = dungeonId;

            UpdateChapterExtraContent();
        }
        
        private void UpdateChapterExtraContent()
        {

            gameObject.CustomActive(false);
            _chapterShopId = 0;

            var baseDungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(_dungeonId);
            if (baseDungeonTable == null
                || string.IsNullOrEmpty(baseDungeonTable.ExchangeStoreEntrance) == true)
            {
                return;
            }

            string[] strList = baseDungeonTable.ExchangeStoreEntrance.Split('|');
            if (strList.Length == 2)
            {
                _chapterShopId = int.Parse(strList[0]);

                string shopIconPath = strList[1];

                if (shopIcon != null)
                    ETCImageLoader.LoadSprite(ref shopIcon, shopIconPath);

                if (shopName != null)
                {
                    var shopTable = TableManager.GetInstance().GetTableItem<ShopTable>(_chapterShopId);
                    if (shopTable != null)
                        shopName.text = shopTable.ShopName;
                }
                gameObject.CustomActive(true);
            }
        }

        private void OnShopButtonClick()
        {
            if (_chapterShopId <= 0)
            {
                Logger.LogErrorFormat("ChapterShopId is less zero and shopId is {0}", _chapterShopId);
                return;
            }

            ShopNewDataManager.GetInstance().OpenShopNewFrame(_chapterShopId);
        }

    }
}
