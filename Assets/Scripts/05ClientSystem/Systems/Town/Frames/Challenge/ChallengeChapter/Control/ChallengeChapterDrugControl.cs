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

    public class ChallengeChapterDrugControl : MonoBehaviour
    {

        private bool _isInit = false;

        private int _dungeonId;
        private DungeonTable _dungeonTable;

        private bool _isUseDrug = false;

        [Space(10)] [HeaderAttribute("DrugValue")] [Space(10)] [SerializeField]
        private Text attackValueText;

        [SerializeField] private Text magicAttackValueText;
        [SerializeField] private Text hpValueText;
        [SerializeField] private Text critValueText;
        [SerializeField] private Text dodgeValueText;
        [SerializeField] private Color selectColor;
        [SerializeField] private Color normalColor = Color.white;


        [Space(10)] [HeaderAttribute("ButtonSetting")] [Space(10)] [SerializeField]
        private Button drugSettingButton;

        [Space(10)] [HeaderAttribute("ButtonUse")] [Space(10)] [SerializeField]
        private Button drugUseButton;

        [SerializeField] private Image drugUseImage;
        [SerializeField] private GameObject drugUseWithoutCostRoot;
        [SerializeField] private GameObject drugUseWithCostRoot;
        [SerializeField] private Text drugUseCostValueText;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            ChapterBuffDrugManager.GetInstance().SetBuffDrugToggleState(_isUseDrug);
            ClearData();
            UnBindEvents();
        }

        private void ClearData()
        {
            _dungeonTable = null;
            _dungeonId = 0;
            _isUseDrug = false;
            _isInit = false;
        }

        private void BindEvents()
        {
            if (drugUseButton != null)
            {
                drugUseButton.onClick.RemoveAllListeners();
                drugUseButton.onClick.AddListener(OnDrugUseButtonClick);
            }

            if (drugSettingButton != null)
            {
                drugSettingButton.onClick.RemoveAllListeners();
                drugSettingButton.onClick.AddListener(OnDrugSettingButtonClick);
            }

            UIEventSystem.GetInstance()
                .RegisterEventHandler(EUIEventID.BuffDrugSettingSubmit, OnDrugSettingSubmit);
        }

        private void UnBindEvents()
        {
            if (drugUseButton != null)
                drugUseButton.onClick.RemoveAllListeners();

            if (drugSettingButton != null)
                drugSettingButton.onClick.RemoveAllListeners();

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BuffDrugSettingSubmit, OnDrugSettingSubmit);
        }

        public void UpdateDrugControl(int dungeonId)
        {
            if (_dungeonId == dungeonId)
                return;

            _dungeonId = dungeonId;
            _dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(_dungeonId);

            if (_dungeonTable == null)
            {
                Logger.LogErrorFormat("ChallengeChapterDrugControl dungeonTable is null and dungeonId is {0}",
                    _dungeonId);
                return;
            }

            UpdateDrugInitUsed();

            SetDrugValueInfo();
            SetDrugCostInfo();

        }

        //第一次，对drugControl进行初始化操作
        private void UpdateDrugInitUsed()
        {
            if (_isInit == false)
            {
                _isInit = true;

                if (ChapterBuffDrugManager.GetInstance().IsBuffDrugToggleOn() == true)
                {
                    _isUseDrug = true;
                    //是否进入地下城带入某些装备
                    if (_dungeonTable != null)
                        ChapterBuffDrugManager.GetInstance().ResetBuffDrugsFromLocal(_dungeonTable.BuffDrugConfig);
                }
                else
                {
                    _isUseDrug = false;
                }
            }

            UpdateUseDrugButtonFlag();
        }

        #region DrugValue

        private void SetDrugValueInfo()
        {
            if (_dungeonTable == null)
                return;

            var buffDrugList = _dungeonTable.BuffDrugConfig;
            bool isSelectDrug = ChapterBuffDrugManager.GetInstance().IsBuffDrugSetted(buffDrugList[0])
                || ChapterBuffDrugManager.GetInstance().IsBuffDrugSetted(buffDrugList[0])
                || ChapterBuffDrugManager.GetInstance().IsBuffDrugSetted(buffDrugList[1])
                || ChapterBuffDrugManager.GetInstance().IsBuffDrugSetted(buffDrugList[2])
                || ChapterBuffDrugManager.GetInstance().IsBuffDrugSetted(buffDrugList[3]);

            BeEntityData beEntityData = null;
            BattleData bData = null;
            DisplayAttribute attribute = null;
            float attack = 0f;
            float magicAttack = 0f;
            if (isSelectDrug)
            {
                attribute = BeUtility.GetMainPlayerActorAttribute();
                attack = attribute.attack;
                magicAttack = attribute.magicAttack;
                beEntityData = BeUtility.GetMainPlayerActor().GetEntityData();
                bData = beEntityData.battleData;
            }

            if (buffDrugList.Count < 4)
            {
                return;
            }

            SetAttackInfo(attackValueText,
                attack,
                ChapterBuffDrugManager.GetInstance().IsBuffDrugSetted(buffDrugList[0]),
                buffDrugList[0]);

            SetAttackInfo(magicAttackValueText,
                magicAttack,
                ChapterBuffDrugManager.GetInstance().IsBuffDrugSetted(buffDrugList[0]),
                buffDrugList[0]);

            SetHpInfo(hpValueText,
                bData,
                ChapterBuffDrugManager.GetInstance().IsBuffDrugSetted(buffDrugList[1]),
                buffDrugList[1]);

            SetPercentInfo(critValueText,
                ChapterBuffDrugManager.GetInstance().IsBuffDrugSetted(buffDrugList[2]),
                buffDrugList[2]);

            SetPercentInfo(dodgeValueText,
                ChapterBuffDrugManager.GetInstance().IsBuffDrugSetted(buffDrugList[3]),
                buffDrugList[3]);
        }

        void SetAttackInfo(Text infoText, float attack, bool isItemSelected, int drugId)
        {
            if (infoText == null)
                return;

            var buffDrug = TableManager.GetInstance().GetTableItem<ItemTable>(drugId);
            if (null == buffDrug)
                return;

            int buffId = buffDrug.OnUseBuffId;
            var buff = TableManager.GetInstance().GetTableItem<BuffTable>(buffId);
            if (null == buff)
                return;

            var baseAdd = buff.attack.fixValue;
            var percent = buff.attackAddRate.fixValue;
            if (isItemSelected)
            {
                infoText.text = "+" + (ChallengeUtility.FloatToInt(((attack + baseAdd) * (1 + (float)percent / GlobalLogic.VALUE_1000)) - attack)).ToString();
                infoText.color = selectColor;
            }
            else
            {
                infoText.text = TR.Value("chapter_value_string", 0);
                infoText.color = normalColor;
            }
        }

        void SetHpInfo(Text infoText, BattleData bData, bool isItemSelected, int drugId)
        {
            if (infoText == null)
                return;

            if (isItemSelected)
            {
                var buffDrug = TableManager.GetInstance().GetTableItem<ItemTable>(drugId);
                if (null == buffDrug)
                    return;

                int buffId = buffDrug.OnUseBuffId;
                var buff = TableManager.GetInstance().GetTableItem<BuffTable>(buffId);
                if (null == buff)
                    return;

                var baseAdd = buff.maxHp.fixValue;
                var percent = buff.maxHpAddRate.fixValue;
                int baseHpBuff = ChallengeUtility.FloatToInt(((bData.fMaxHp + baseAdd) * (1 + percent / (float)(GlobalLogic.VALUE_1000)) - bData.fMaxHp));
                int baseMaxHp1 = bData.fMaxHp;
                bData._maxHp += baseAdd;
                bData._maxHp += IntMath.Float2Int(bData._maxHp * (percent / (float)(GlobalLogic.VALUE_1000)));
                int actualHpBuff = bData.fMaxHp - baseMaxHp1;
                infoText.text = TR.Value("chapter_buffdrug_hpdisplay", baseHpBuff, actualHpBuff);
            }
            else
            {
                infoText.text = TR.Value("chapter_value_string", 0);
                infoText.color = normalColor;
            }
        }

        void SetPercentInfo(Text infoText, bool isItemSelected, int drugId)
        {
            if (infoText == null)
                return;

            var buffDrug = TableManager.GetInstance().GetTableItem<ItemTable>(drugId);
            if (null == buffDrug)
                return;

            var buffId = buffDrug.OnUseBuffId;
            var buff = TableManager.GetInstance().GetTableItem<BuffTable>(buffId);
            if (null == buff)
                return;

            var percent = buff.ciriticalAttack.fixValue == 0 ? buff.dodge.fixValue : buff.ciriticalAttack.fixValue;
            if (isItemSelected)
            {
                infoText.text = TR.Value("chapter_percent_string", percent / 10);
                infoText.color = selectColor;
            }
            else
            {
                infoText.text = TR.Value("chapter_percent_string", 0);
                infoText.color = normalColor;
            }
        }

        #endregion

        #region DrugCost
        private void SetDrugCostInfo()
        {
            if (_isUseDrug == false)
            {
                SetDrugUseRoot(_isUseDrug);
                return;
            }

            var totalCount = 0;
            List<CostItemManager.CostInfo> costInfoList = ChapterBuffDrugManager.GetInstance()
                .GetAllMarkedBuffDrugsCost(_dungeonId);
            if (costInfoList == null || costInfoList.Count <= 0)
                totalCount = 0;
            else
            {
                for (var i = 0; i < costInfoList.Count; i++)
                {
                    totalCount += costInfoList[i].nCount;
                }
            }

            if (totalCount <= 0)
            {
                SetDrugUseRoot(false);
            }
            else
            {
                SetDrugUseRoot(true);
                drugUseCostValueText.text = totalCount.ToString();
            }
        }

        private void SetDrugUseRoot(bool flag)
        {
            if (drugUseWithCostRoot != null)
                drugUseWithCostRoot.CustomActive(flag);

            if (drugUseWithoutCostRoot != null)
                drugUseWithoutCostRoot.CustomActive(!flag);
        }
        #endregion


        private void OnDrugSettingButtonClick()
        {
            var dungeonId = _dungeonId;
            ClientSystemManager.instance.OpenFrame<ChapterDrugSettingFrame>(GameClient.FrameLayer.Middle, dungeonId);
        }

        private void OnDrugUseButtonClick()
        {
            _isUseDrug = !_isUseDrug;
            UpdateUseDrugButtonFlag();

            if (_isUseDrug == true)
            {
                if (_dungeonTable != null)
                    ChapterBuffDrugManager.GetInstance().ResetBuffDrugsFromLocal(_dungeonTable.BuffDrugConfig);
            }
            else
            {
                ChapterBuffDrugManager.GetInstance().ResetAllMarkedBuffDrugs();
            }

            SetDrugCostInfo();
        }

        private void OnDrugSettingSubmit(UIEvent uiEvent)
        {
            _isUseDrug = true;
            UpdateUseDrugButtonFlag();

            SetDrugValueInfo();
            SetDrugCostInfo();
        }

        private void UpdateUseDrugButtonFlag()
        {
            if (drugUseImage != null)
                drugUseImage.gameObject.CustomActive(_isUseDrug);
        }


    }
}
