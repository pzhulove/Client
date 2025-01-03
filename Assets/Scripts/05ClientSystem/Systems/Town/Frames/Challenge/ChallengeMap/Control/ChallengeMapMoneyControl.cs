using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Protocol;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class ChallengeMapMoneyControl : MonoBehaviour
    {

        [Space(10)]
        [HeaderAttribute("NormalCostItemList")]
        [Space(10)]
        [SerializeField] private List<ComCommonConsume> costItemList = new List<ComCommonConsume>();

        [Space(10)]
        [HeaderAttribute("SpriteCostItem")]
        [Space(10)]
        [SerializeField]
        private ComCommonConsume spriteConsumeItem;

        private void Awake()
        {

        }

        private void OnDestroy()
        {
        }

        private void OnEnable()
        {
            BindEvents();
        }

        private void OnDisable()
        {
            UnBindEvents();
        }

        private void BindEvents()
        {
        }

        private void UnBindEvents()
        {
        }

        //需要传递默认的参数
        public void Init(DungeonModelTable.eType modelTableType)
        {
            ResetCostMoneyItemList();

            UpdateMapMoneyItemList(modelTableType);
        }

        private void ResetCostMoneyItemList()
        {
            if (spriteConsumeItem != null)
                CommonUtility.UpdateGameObjectVisible(spriteConsumeItem.gameObject, false);

            if (costItemList == null || costItemList.Count <= 0)
                return;

            for (var i = 0; i < costItemList.Count; i++)
            {
                var curMoneyItem = costItemList[i];
                if(curMoneyItem == null)
                    continue;

                CommonUtility.UpdateGameObjectVisible(curMoneyItem.gameObject, false);
            }
        }

        private void UpdateMapMoneyItemList(DungeonModelTable.eType modelTableType)
        {
            bool isShowSpriteConsume = ChallengeUtility.GetDungeonModelShowSpriteFlag(modelTableType);
            if (spriteConsumeItem != null)
            {
                CommonUtility.UpdateGameObjectVisible(spriteConsumeItem.gameObject, isShowSpriteConsume);
                if (isShowSpriteConsume == true)
                    spriteConsumeItem.SetData(ComCommonConsume.eType.Count,
                        ComCommonConsume.eCountType.Fatigue, 0);
            }

            var costDataList = ChallengeUtility.GetDungeonModelCostDataList(modelTableType);
            if (costDataList == null || costDataList.Count <= 0)
                return;

            if (costItemList == null || costItemList.Count <= 0)
                return;

            var costDataNumber = costDataList.Count;
            var costMoneyItemNumber = costItemList.Count;

            for (var i = 0; i < costDataNumber && i < costMoneyItemNumber; i++)
            {
                var curCostData = costDataList[i];
                var curCostItem = costItemList[i];

                if (curCostData > 0 && curCostItem != null)
                {
                    curCostItem.SetData(ComCommonConsume.eType.Item, ComCommonConsume.eCountType.Fatigue,
                        curCostData);
                    CommonUtility.UpdateGameObjectVisible(curCostItem.gameObject, true);
                }
            }
        }



    }
}