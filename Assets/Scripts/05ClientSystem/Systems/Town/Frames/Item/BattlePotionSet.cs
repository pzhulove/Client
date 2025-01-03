using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;


namespace GameClient
{
    public class BattlePotionSet : MonoBehaviour
    {
        [SerializeField]
        GeUISwitchButton switchButton0 = null;

        [SerializeField]
        GeUISwitchButton switchButton1 = null;

        [SerializeField]
        GeUISwitchButton switchButton2 = null;

        [SerializeField]
        Slider slider0 = null;

        [SerializeField]
        Text percent0 = null;

        [SerializeField]
        Slider slider1 = null;

        [SerializeField]
        Text percent1 = null;

        [SerializeField]
        Slider slider2 = null;

        [SerializeField]
        Text percent2 = null;

        [SerializeField]
        Image[] ConfigItems = new Image[0];

        [SerializeField] private GameObject[] mDrugRoots = new GameObject[0];

        int AdjustSlider(Slider slider,float current, float min, float max)
        {
            float tmpPercentHp = current;
            if (current <= min)
            {
                tmpPercentHp = min;
                slider.value = min;
            }

            if (current >= max)
            {
                tmpPercentHp = max;
                slider.value = max;
            }

            return Mathf.FloorToInt(tmpPercentHp * 100) / 5 * 5;
        }

        private void _updateBattlePostionSet()
        {
            List<uint> potionSets = PlayerBaseData.GetInstance().potionSets;
            for (int i = 0; i < ConfigItems.Length; ++i)
            {
                uint id = 0;

                if (i < potionSets.Count)
                {
                    id = potionSets[i];
                }

                _updateBattlePostionSetByIdx(i, id);
            }
        }

        private void _updateBattlePostionSetByIdx(int i, uint id)
        {
            Logger.LogProcessFormat("[PotionSet] 配置 {0}, {1}", i, id);

            ProtoTable.ItemTable itemTable = TableManager.instance.GetTableItem<ProtoTable.ItemTable>((int)id);
            if (null == itemTable)
            {
                ConfigItems[i].color = Color.clear;    
            }
            else
            {
                ConfigItems[i].color = Color.white;
                ETCImageLoader.LoadSprite(ref ConfigItems[i], itemTable.Icon);     
            }
        }

        // Use this for initialization
        void Start()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonPotionSetChanged, _onUpdateBattlePostionSet);

            if (switchButton0 != null)
            {
                switchButton0.onValueChanged.RemoveAllListeners();
                switchButton0.onValueChanged.AddListener((value) => 
                {
                    PlayerBaseData.GetInstance().SetPotionSlotMainSwitchOn(value);
                });
            }

            if(switchButton1 != null)
            {
                switchButton1.onValueChanged.RemoveAllListeners();
                switchButton1.onValueChanged.AddListener((value) =>
                {
                    PlayerBaseData.GetInstance().SetPotionSlotMainSwitchOn(value, false, "PotionSlot1Switch");
                });
            }

            if (switchButton2 != null)
            {
                switchButton2.onValueChanged.RemoveAllListeners();
                switchButton2.onValueChanged.AddListener((value) =>
                {
                    PlayerBaseData.GetInstance().SetPotionSlotMainSwitchOn(value, false, "PotionSlot2Switch");
                });
            }

            slider0.SafeSetValueChangeListener((value) => 
            {
                int percent = AdjustSlider(slider0, value, 0.05f, 0.9f);
                percent0.SafeSetText(string.Format("{0}%", percent));
                PlayerBaseData.GetInstance().SetPotionPercent(PlayerBaseData.PotionSlotType.SlotMain, percent);
            });

            slider1.SafeSetValueChangeListener((value) =>
            {
                int percent = AdjustSlider(slider1, value, 0.0f, 0.9f);
                percent1.SafeSetText(string.Format("{0}%", percent));
                PlayerBaseData.GetInstance().SetPotionPercent(PlayerBaseData.PotionSlotType.SlotExtend1, percent);
            });

            slider2.SafeSetValueChangeListener((value) =>
            {
                int percent = AdjustSlider(slider2, value, 0.0f, 0.9f);
                percent2.SafeSetText(string.Format("{0}%", percent));
                PlayerBaseData.GetInstance().SetPotionPercent(PlayerBaseData.PotionSlotType.SlotExtend2, percent);
            });

            UpdateUI();
        }

        private void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonPotionSetChanged, _onUpdateBattlePostionSet);
            PlayerBaseData.GetInstance().SavePotionPercentSetsToFile();
        }

        private void _onUpdateBattlePostionSet(UIEvent ui)
        {
            _updateBattlePostionSet();
            UpdateUI();
        }

        // Update is called once per frame
        void Update()
        {
            
        }        

        public void UpdateUI()
        {
            if (switchButton0 != null)
            {
                switchButton0.SetSwitch(PlayerBaseData.GetInstance().IsPotionSlotMainSwitchOn());

                UIGray uIGray = switchButton0.gameObject.SafeAddComponent<UIGray>(false);
                if(uIGray != null)
                {
                    uIGray.enabled = PlayerBaseData.GetInstance().GetPotionID(PlayerBaseData.PotionSlotType.SlotMain) == 0;
                }
                switchButton0.interactable = PlayerBaseData.GetInstance().GetPotionID(PlayerBaseData.PotionSlotType.SlotMain) != 0;
            }

            if (switchButton1 != null)
            {
                switchButton1.SetSwitch(PlayerBaseData.GetInstance().IsPotionSlotMainSwitchOn(PlayerBaseData.potionSlot1SwitchKeyName));

                UIGray uIGray = switchButton1.gameObject.SafeAddComponent<UIGray>(false);
                if (uIGray != null)
                {
                    uIGray.enabled = PlayerBaseData.GetInstance().GetPotionID(PlayerBaseData.PotionSlotType.SlotExtend1) == 0;
                }
                switchButton1.interactable = PlayerBaseData.GetInstance().GetPotionID(PlayerBaseData.PotionSlotType.SlotExtend1) != 0;
            }

            if (switchButton2 != null)
            {
                switchButton2.SetSwitch(PlayerBaseData.GetInstance().IsPotionSlotMainSwitchOn(PlayerBaseData.potionSlot2SwitchKeyName));

                UIGray uIGray = switchButton2.gameObject.SafeAddComponent<UIGray>(false);
                if (uIGray != null)
                {
                    uIGray.enabled = PlayerBaseData.GetInstance().GetPotionID(PlayerBaseData.PotionSlotType.SlotExtend2) == 0;
                }
                switchButton2.interactable = PlayerBaseData.GetInstance().GetPotionID(PlayerBaseData.PotionSlotType.SlotExtend2) != 0;
            }

            int percent = 0;
            percent = PlayerBaseData.GetInstance().GetPotionPercent(PlayerBaseData.PotionSlotType.SlotMain);
            percent0.SafeSetText(string.Format("{0}%", percent));
            slider0.SafeSetValue(percent / 100.0f);
     
            percent = PlayerBaseData.GetInstance().GetPotionPercent(PlayerBaseData.PotionSlotType.SlotExtend1);
            percent1.SafeSetText(string.Format("{0}%", percent));
            slider1.SafeSetValue(percent / 100.0f);

            percent = PlayerBaseData.GetInstance().GetPotionPercent(PlayerBaseData.PotionSlotType.SlotExtend2);
            percent2.SafeSetText(string.Format("{0}%", percent));
            slider2.SafeSetValue(percent / 100.0f);

            _updateBattlePostionSet();

            slider0.SafeSetGray(PlayerBaseData.GetInstance().GetPotionID(PlayerBaseData.PotionSlotType.SlotMain) == 0);
            slider1.SafeSetGray(PlayerBaseData.GetInstance().GetPotionID(PlayerBaseData.PotionSlotType.SlotExtend1) == 0);
            slider2.SafeSetGray(PlayerBaseData.GetInstance().GetPotionID(PlayerBaseData.PotionSlotType.SlotExtend2) == 0);
        }
    }
}


