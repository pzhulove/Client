using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ProtoTable;

namespace GameClient
{

    //商店按钮
    public class CommonShopButtonControl : MonoBehaviour
    {

        [SerializeField] private int shopId;
        [Space(15)]
        [HeaderAttribute("Control")]
        [Space(15)]
        [SerializeField] private Button shopButton;
        [SerializeField] private Text shopName;
        [SerializeField] private Image shopImage;
        

        private void Awake()
        {
            if (shopButton != null)
            {
                shopButton.onClick.RemoveAllListeners();
                shopButton.onClick.AddListener(OnShopButtonClicked);
            }
        }

        private void OnDestroy()
        {
            if(shopButton != null)
                shopButton.onClick.RemoveAllListeners();
        }

        //
        private void Start()
        {
            UpdateShopButtonControl();
        }

        private void OnShopButtonClicked()
        {
            if (shopId <= 0)
                return;

            ShopNewDataManager.GetInstance().JumpToShopById(shopId);
        }

        private void UpdateShopButtonControl()
        {
            if (shopId <= 0)
                return;

            var shopTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopId);
            if (shopTable == null)
                return;

            if (shopName != null)
                shopName.text = shopTable.ShopName;

        }

        //设置商店的Id
        public void SetShopId(int id)
        {
            shopId = id;
            UpdateShopButtonControl();
        }

    }
}