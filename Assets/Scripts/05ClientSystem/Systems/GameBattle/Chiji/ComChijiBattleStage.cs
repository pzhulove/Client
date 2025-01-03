using System;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using Protocol;
using Network;
using System;
using System.Collections.Generic;

namespace GameClient
{
    public class ComChijiBattleStage : MonoBehaviour
    {
        public ChiJiTimeTable.eBattleStage BattleStage = ChiJiTimeTable.eBattleStage.BS_NONE;
        public ChiJiTimeTable.eBattleStage BigStage = ChiJiTimeTable.eBattleStage.BS_NONE;
        public Image icon = null;
        public GameObject EffectObjRoot = null;
        public string FinishIconPath = "";
        public string BigIconPath = "";
        public Vector2 normalSize = new Vector2(0.0f, 0.0f);
        public Vector2 bigSize = new Vector2(0.0f, 0.0f);

        private bool bHasLoadedBigIcon = false;
        private bool bHasLoadedFinishIcon = false;

        private void Start()
        {
            _BindUIEvent();

            if(ChijiDataManager.GetInstance().CurBattleStage == BattleStage)
            {
                if(!bHasLoadedBigIcon)
                {
                    _SetBigIcon();
                    bHasLoadedBigIcon = true;
                }
            }
            else if(ChijiDataManager.GetInstance().CurBattleStage >= BigStage)
            {
                if(!bHasLoadedFinishIcon)
                {
                    _SetFinishIcon();
                    bHasLoadedFinishIcon = true;
                }
            }
        }

        private void OnDestroy()
        {
            _UnBindUIEvent();

            BattleStage = ChiJiTimeTable.eBattleStage.BS_NONE;
            bHasLoadedBigIcon = false;
            bHasLoadedFinishIcon = false;

            if (icon != null)
            {
                icon = null;
            }
        }

        private void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChijiBattleStageChanged, _OnStageChanged);
        }

        private void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChijiBattleStageChanged, _OnStageChanged);
        }

        private void _OnStageChanged(UIEvent iEvent)
        {
            if(ChijiDataManager.GetInstance().CurBattleStage < BattleStage)
            {
                return;
            }

            if(BattleStage == ChiJiTimeTable.eBattleStage.BS_PUT_ITEM_1)
            {
                int kkkk = 0;
            }

            if (ChijiDataManager.GetInstance().CurBattleStage == BattleStage)
            {
                if (!bHasLoadedBigIcon)
                {
                    _SetBigIcon();
                    bHasLoadedBigIcon = true;
                }
            }
            else if (ChijiDataManager.GetInstance().CurBattleStage >= BigStage)
            {
                if (!bHasLoadedFinishIcon)
                {
                    _SetFinishIcon();
                    bHasLoadedFinishIcon = true;
                }
            }
        }

        private void _SetFinishIcon()
        {
            if (icon != null)
            {
                ETCImageLoader.LoadSprite(ref icon, FinishIconPath);

                RectTransform rect = icon.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.sizeDelta = normalSize;
                }
            }

            if(EffectObjRoot != null)
            {
                EffectObjRoot.CustomActive(false);
            }
        }

        private void _SetBigIcon()
        {
            if (icon != null)
            {
                ETCImageLoader.LoadSprite(ref icon, BigIconPath);

                RectTransform rect = icon.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.sizeDelta = bigSize;
                }
            }

            if(EffectObjRoot != null)
            {
                EffectObjRoot.CustomActive(true);
            }
        }

        private void Update()
        {
        }
    }
}
