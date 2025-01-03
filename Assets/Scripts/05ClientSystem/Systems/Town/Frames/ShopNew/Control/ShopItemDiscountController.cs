using System;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ShopItemDiscountController : MonoBehaviour
    {

        [Space(10)] [HeaderAttribute("NormalDiscount")] [Space(10)]
        [SerializeField] private GameObject normalDiscountRoot;

        [SerializeField] private Image normalDiscountNumberImage;

        [Space(10)] [HeaderAttribute("SpecialDiscount")] [Space(10)]
        [SerializeField] private GameObject specialDiscountRoot;
        [SerializeField] private Image specialDiscountFirstNumberImage;
        [SerializeField] private Image specialDiscountSecondNumberImage;

        private const string imagePath = "UI/Image/Packed/p_UI_Rongyu.png:UI_Rongyu_Zhekou_0{0}";

        public void InitShopItemDiscount(int discountValue)
        {
            CommonUtility.UpdateGameObjectVisible(normalDiscountRoot, false);
            CommonUtility.UpdateGameObjectVisible(specialDiscountRoot, false);

            var firstNumber = discountValue / 10;
            var secondNumber = discountValue % 10;

            
            if (secondNumber == 0)
            {
                //7折,只有一个数字
                CommonUtility.UpdateGameObjectVisible(normalDiscountRoot, true);
                var numberPathStr = string.Format(imagePath, firstNumber);
                ETCImageLoader.LoadSprite(ref normalDiscountNumberImage, numberPathStr);
            }
            else
            {
                //7.5折扣，两个数字
                CommonUtility.UpdateGameObjectVisible(specialDiscountRoot, true);
                var firstNumberPathStr = string.Format(imagePath, firstNumber);
                ETCImageLoader.LoadSprite(ref specialDiscountFirstNumberImage, firstNumberPathStr);

                var secondNumberPathStr = string.Format(imagePath, secondNumber);
                ETCImageLoader.LoadSprite(ref specialDiscountSecondNumberImage, secondNumberPathStr);
            }
        }
    }
}
