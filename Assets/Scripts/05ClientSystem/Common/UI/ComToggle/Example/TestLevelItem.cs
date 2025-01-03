using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    //个性化Item
    public class TestLevelItem : ComToggleItem
    {

        [Space(10)]
        [HeaderAttribute("ComToggleItem")]
        [Space(5)]
        [SerializeField] private Text levelName;

        public override void InitItem(ComControlData comToggleData,
            OnComToggleClick onComToggleClick = null)
        {
            base.InitItem(comToggleData, onComToggleClick);

            //初始化个性化的数值
            if (levelName != null)
            {
                var testLevelData = _comToggleData as TestLevelData;
                if (testLevelData != null)
                {
                    if (testLevelData.Id % 2 == 0)
                    {
                        levelName.gameObject.CustomActive(true);
                        levelName.text = testLevelData.LevelName;
                    }
                    else
                    {
                        levelName.gameObject.CustomActive(false);
                    }
                }
            }
        }

    }
}
