using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    class MoneyRewardsRecordFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/MoneyRewards/MoneyRewardsRecordFrame";
        }

        public static void CommandOpen(object argv)
        {
            if(MoneyRewardsDataManager.GetInstance().Records.Count == 0)
            {
                MoneyRewardsDataManager.GetInstance().RequestRecords(false,-1, 20, () =>
                {
                    ClientSystemManager.GetInstance().OpenFrame<MoneyRewardsRecordFrame>(FrameLayer.Middle, argv);
                });
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<MoneyRewardsRecordFrame>(FrameLayer.Middle, argv);
            }
        }

        [UIControl("ScrollView", typeof(ComUIListScript))]
        ComUIListScript scripts;
        [UIControl("TG0", typeof(Toggle))]
        Toggle toggle;

        TextGenerator cachedTextGenerator = new TextGenerator(256);
        TextGenerationSettings textGeneratorSetting = new TextGenerationSettings();
        [UIControl("ScrollView/ViewPort/Content/RecordItem/Name", typeof(Text))]
        Text generate;

        void _InitGeneratorSetting()
        {
            Vector2 extents = new Vector2(generate.rectTransform.rect.size.x, 0);
            var settings = generate.GetGenerationSettings(extents);
            textGeneratorSetting.font = settings.font;
            textGeneratorSetting.fontSize = settings.fontSize;
            textGeneratorSetting.fontStyle = settings.fontStyle;
            textGeneratorSetting.lineSpacing = settings.lineSpacing;
            textGeneratorSetting.horizontalOverflow = HorizontalWrapMode.Wrap;
            textGeneratorSetting.verticalOverflow = VerticalWrapMode.Overflow;
            textGeneratorSetting.alignByGeometry = false;
            textGeneratorSetting.resizeTextForBestFit = settings.resizeTextForBestFit;
            textGeneratorSetting.richText = settings.richText;
            textGeneratorSetting.scaleFactor = 1.0f;
            textGeneratorSetting.updateBounds = settings.updateBounds;
            textGeneratorSetting.generationExtents = extents;
        }

        List<int> m_searched_list = new List<int>();
        protected override void _OnOpenFrame()
        {
            _InitGeneratorSetting();
            _AddButton("Close", () => { frameMgr.CloseFrame(this); });

            if(null != toggle)
            {
                toggle.onValueChanged.AddListener(_OnToggleChanged);
            }

            if (null != scripts)
            {
                scripts.Initialize();
                scripts.onBindItem = (GameObject go) =>
                {
                    if (null != go)
                    {
                        return go.GetComponent<ComMoneyRewardsRecords>();
                    }
                    return null;
                };
                scripts.onItemVisiable = (ComUIListElementScript item) =>
                {
                    var count = MoneyRewardsDataManager.GetInstance().getDataCount(toggle.isOn);
                    if (null != item && item.m_index >= 0 && item.m_index < count)
                    {
                        var script = item.gameObjectBindScript as ComMoneyRewardsRecords;
                        if (null != script)
                        {
                            script.OnItemVisible(MoneyRewardsDataManager.GetInstance().getData(item.m_index,toggle.isOn));
                            if(item.m_index == 0)
                            {
                                var first = MoneyRewardsDataManager.GetInstance().Records[0];
                                if (null != first && !m_searched_list.Contains(first.iIndex))
                                {
                                    m_searched_list.Add(first.iIndex);
                                    int iCnt = 0;
                                    if(first.iIndex > 0)
                                    {
                                        if(first.iIndex < 20)
                                        {
                                            iCnt = first.iIndex;
                                        }
                                        else
                                        {
                                            iCnt = 20;
                                        }
                                    }
                                    if(iCnt > 0)
                                    {
                                        MoneyRewardsDataManager.GetInstance().RequestRecords(false, first.iIndex, iCnt,null);
                                    }
                                }
                            }
                        }
                    }
                };
            }
            _UpdateRecords();

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsRecordsChanged,_OnMoneyRewardsRecordsChanged);
        }

        void _UpdateRecords()
        {
            if (null != scripts)
            {
                var count = MoneyRewardsDataManager.GetInstance().getDataCount(toggle.isOn);
                List<Vector2> elementsSize = GamePool.ListPool<Vector2>.Get();
                for (int i = 0; i < count; ++i)
                {
                    var data = MoneyRewardsDataManager.GetInstance().getData(i, toggle.isOn);
                    if(null != data)
                    {
                        if (!data.measured)
                        {
                            data.saveValue = data.ToRecords();
                            float h = cachedTextGenerator.GetPreferredHeight(data.saveValue, textGeneratorSetting);
                            float w = textGeneratorSetting.generationExtents.x;
                            data.h = h;
                            data.w = w;
                            //Logger.LogErrorFormat("size = {0}|{1}", data.w, data.h);
                            data.measured = true;
                        }
                        elementsSize.Add(new Vector2(data.w, data.h));
                    }
                }
                scripts.SetElementAmount(elementsSize.Count, elementsSize);
                GamePool.ListPool<Vector2>.Release(elementsSize);
            }
        }

        void _OnToggleChanged(bool bValue)
        {
            _UpdateRecords();
        }

        void _OnMoneyRewardsRecordsChanged(UIEvent uiEvent)
        {
            _UpdateRecords();
        }

        protected override void _OnCloseFrame()
        {
            if (null != scripts)
            {
                scripts.onBindItem = null;
                scripts.onItemVisiable = null;
                scripts = null;
            }

            if (null != toggle)
            {
                toggle.onValueChanged.RemoveListener(_OnToggleChanged);
            }

            m_searched_list.Clear();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsRecordsChanged, _OnMoneyRewardsRecordsChanged);
        }
    }
}
