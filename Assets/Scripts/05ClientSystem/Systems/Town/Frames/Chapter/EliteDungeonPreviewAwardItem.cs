using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using Network;
using Protocol;

namespace GameClient
{
    // 精英地下城关卡预览中的奖励道具item
    public class EliteDungeonPreviewAwardItem : MonoBehaviour
    {
        [SerializeField]
        ComChapterInfoDropUnit comChapterInfoDropUnit = null;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDestroy()
        {
            ComChapterInfoDropUnit unit = comChapterInfoDropUnit;
            if (null != unit)
            {
                unit.Unload();
            }
        }

        public void SetUp(object data)
        {
            if (!(data is int))
            {
                return;
            }

            int itemTableID = (int)data;

            ComChapterInfoDropUnit unit = comChapterInfoDropUnit;
            if (null != unit)
            {
                unit.Load(itemTableID);
                unit.ShowName(true);
                unit.ShowLevel(true);   
            }
        }
    }
}
