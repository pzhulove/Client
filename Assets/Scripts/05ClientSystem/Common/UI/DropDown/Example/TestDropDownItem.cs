using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    //个性化Item
    public class TestDropDownItem : ComDropDownItem
    {

        [Space(10)]
        [HeaderAttribute("ComToggleItem")]
        [Space(5)]
        [SerializeField] private Text dropDownName;

        public override void InitItem(ComControlData comControlData,
            OnComDropDownItemButtonClick onItemButtonClick = null)
        {
            base.InitItem(comControlData, onItemButtonClick);

            //初始化个性化的数值
            if (dropDownName != null)
            {
                var testDropDownData = _comControlData as TestDropDownData;
                if (testDropDownData != null)
                {
                    if (testDropDownData.Id % 3 == 0)
                    {
                        dropDownName.gameObject.CustomActive(true);
                        dropDownName.text = testDropDownData.DropDownName;
                    }
                    else
                    {
                        dropDownName.gameObject.CustomActive(false);
                    }
                }
            }
        }

    }
}
