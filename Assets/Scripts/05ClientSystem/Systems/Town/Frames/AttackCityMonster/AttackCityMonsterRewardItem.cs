using UnityEngine;
using UnityEngine.UI;

using ProtoTable;

namespace GameClient
{
    public class AttackCityMonsterRewardItem : MonoBehaviour
    {

        [SerializeField] private GameObject rewardItemRoot = null;

        private RewardItemDataModel _rewardItem = null;
        private ComItem _comRewardItem = null;

        public void InitData(RewardItemDataModel rewardItem)
        {
            _rewardItem = rewardItem;
            InitRewardItemContent();
        }

        private void InitRewardItemContent()
        {
            if(rewardItemRoot == null)
                return;

            var rewardItemId = _rewardItem.Id;
            var rewardItemCount = _rewardItem.Number;

            if (_comRewardItem == null)
            {
                _comRewardItem = ComItemManager.Create(rewardItemRoot.gameObject);
            }

            var rewardItemData = ItemDataManager.CreateItemDataFromTable(rewardItemId);

            if (rewardItemData != null)
            {
                _comRewardItem.CustomActive(true);
                rewardItemData.Count = rewardItemCount;
                _comRewardItem.Setup(rewardItemData, ShowItemTip);
            }
        }

        private void ShowItemTip(GameObject go, ItemData itemData)
        {
            if (itemData != null)
                ItemTipManager.GetInstance().ShowTip(itemData);
        }

    }
}