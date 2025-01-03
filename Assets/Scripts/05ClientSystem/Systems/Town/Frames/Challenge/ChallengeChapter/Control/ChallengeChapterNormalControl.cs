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
    //通用元素：名字，描述，背景图片，人物图片，是否解锁
    public class ChallengeChapterNormalControl : MonoBehaviour
    {

        private int _dungeonId;
        private DungeonTable _dungeonTable;

        [SerializeField] private Text chapterName;
        [SerializeField] private Text chapterDescription;

        [SerializeField] private Image chapterBackground;
        [SerializeField] private GameObject chapterLock;

        [Space(5)]
        [HeaderAttribute("抗魔值")]
        [SerializeField] private GameObject levelResistValueRoot;
        [SerializeField] private Text levelResistValueNumber;
        [SerializeField] private Text ownerResistValueNumber;
        [SerializeField] private Text rebornLimitNumberValue;
        [SerializeField] private HelpAssistant helpAssistant;
       

        private void Awake()
        {
        }

        private void OnDestroy()
        {
            ClearData();
        }

        private void ClearData()
        {
            _dungeonTable = null;
            _dungeonId = 0;
        }

        public void UpdateNormalControl(int dungeonId, DungeonTable dungeonTable)
        {
            _dungeonId = dungeonId;
            _dungeonTable = dungeonTable;

            if (_dungeonTable == null)
                return;

            UpdateNormalView();
        }

        private void UpdateNormalView()
        {
            if (chapterName != null)
                chapterName.text = _dungeonTable.Name;

            if (chapterDescription != null)
                chapterDescription.text = _dungeonTable.Description;

            if (chapterBackground != null)
                ETCImageLoader.LoadSprite(ref chapterBackground, _dungeonTable.TumbPath);

            //if (chapterIcon != null)
            //    ETCImageLoader.LoadSprite(ref chapterIcon, _dungeonTable.TumbChPath);

            if (chapterLock != null)
            {
                var isChapterLock = ChallengeUtility.IsDungeonLock(_dungeonId);
                chapterLock.gameObject.CustomActive(isChapterLock);
            }

            UpdateLevelResistValue(_dungeonId);
        }


        //更新关卡的抗魔值
        private void UpdateLevelResistValue(int dungeonId)
        {
            var dungeonResistValue = DungeonUtility.GetDungeonResistMagicValueById(dungeonId);
            if (dungeonResistValue <= 0)
            {
                //关卡不存在抗魔值，不显示，直接返回
                levelResistValueRoot.gameObject.CustomActive(false);
            }
            else
            {
                //关卡存在抗魔值，显示抗魔值信息，以及根据情况显示抗魔值不足的提示

                levelResistValueRoot.gameObject.CustomActive(true);
                levelResistValueNumber.text = dungeonResistValue.ToString();

                //总的抗魔值
                var ownerMagicValue = DungeonUtility.GetDungeonMainPlayerResistMagicValue();
                //buff增加的抗魔值
                var magicValueByBuff = BeUtility.GetMainPlayerResistAddByBuff();
                //不存在buff加成的抗魔值
                if (magicValueByBuff == 0)
                {
                    if (dungeonResistValue > ownerMagicValue)
                    {
                        ownerResistValueNumber.text = string.Format(TR.Value("resist_magic_value_less"),
                            ownerMagicValue);
                    }
                    else
                    {
                        ownerResistValueNumber.text = string.Format(TR.Value("resist_magic_value_normal"),
                            ownerMagicValue);
                    }
                }
                else
                {
                    //存在buff加成的抗魔值
                    var baseMagicValue = ownerMagicValue - magicValueByBuff;
                    if (dungeonResistValue > baseMagicValue)
                    {
                        ownerResistValueNumber.text = string.Format(TR.Value("resist_magic_value_add_buff_less"),
                            baseMagicValue, magicValueByBuff);
                    }
                    else
                    {
                        ownerResistValueNumber.text = string.Format(TR.Value("resist_magic_value_add_buff_normal"),
                            baseMagicValue, magicValueByBuff);
                    }
                }

                //只有组队的时候才可能显示侵蚀抗性不足的提示
                if (TeamDataManager.GetInstance().HasTeam() == true)
                {
                    DungeonUtility.ShowResistMagicValueTips(dungeonResistValue);
                }
                
            }

            if (rebornLimitNumberValue != null)
            {
                rebornLimitNumberValue.text = DungeonUtility.GetDungeonRebornValue(dungeonId);
            }

            //如果是虚空地下城
            if (_dungeonTable.SubType == DungeonTable.eSubType.S_DEVILDDOM)
            {
                if (helpAssistant != null)
                {
                    helpAssistant.eType = HelpFrameContentTable.eHelpType.HT_LEVEL_RESISTMAGIC;
                }
            }
            else
            {
                if (helpAssistant != null)
                {
                    helpAssistant.eType = HelpFrameContentTable.eHelpType.HT_WEEKOFTENHELL_RESISTMAGIC;
                }
            }
        }

    }
}
