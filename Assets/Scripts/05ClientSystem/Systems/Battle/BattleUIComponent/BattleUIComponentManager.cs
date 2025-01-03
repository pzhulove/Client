using ProtoTable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    /// <summary>
    /// 战斗UI组件管理器
    /// </summary>
    public class BattleUIComponentManager
    {
        Dictionary<Type, BattleUIBase> _battleUIComponentDic;
        List<BattleUIBase> _needUpdateComponents;
        protected GameObject _rootObj = null;
        
        public void Init(GameObject root)
        {
            _battleUIComponentDic = new Dictionary<Type, BattleUIBase>();
            _needUpdateComponents = new List<BattleUIBase>();
            _rootObj = root;
            InitBattleUIComponents();
        }

        /// <summary>
        /// 根据地下城功能开关表 初始化战斗UI组件列表
        /// </summary>
        private void InitBattleUIComponents()
        {
            //录像模式
            if (ReplayServer.instance.IsReplay())
            {
                CreateBattleUIComponent<BattleUIPvp>();
                CreateBattleUIComponent<BattleUIReplay>();
                return;
            }

            if (BattleMain.instance == null)
                return;
            var type = BattleMain.battleType;
            DungeonFunctionSwitchTable data = TableManager.instance.GetTableItem<DungeonFunctionSwitchTable>((int)type);
            if (data == null)
                return;
            if (data.DungeonMap)
                CreateBattleUIComponent<BattleUIDungeonMap>();
            if(data.DungeonScore)
                CreateBattleUIComponent<BattleUIDungeonScore>();
            if (data.Drug)
                CreateBattleUIComponent<BattleUIDrug>();
            if (data.SwitchEquipAndWeapon)
                CreateBattleUIComponent<BattleUISwitchWeaAndEquip>();
            if (data.Voice)
                CreateBattleUIComponent<BattleUIVoice>();
            if (data.Test)
                CreateBattleUIComponent<BattleUITest>();
            if (data.PvpCommon)
                CreateBattleUIComponent<BattleUIPvp>();
            if (data.PveCommon)
                CreateBattleUIComponent<BattleUIPve>();
            if (data.ChampionName)
                CreateBattleUIComponent<BattleUIChampionName>();
            if (data.PvpTrain)
                CreateBattleUIComponent<BattleUITrainPvp>();
            if (data.PveTrain)
                CreateBattleUIComponent<BattleUITrainPve>();
            if (data.Pvp3v3)
                CreateBattleUIComponent<BattleUI3V3>();
            if(data.PauseBtn)
                CreateBattleUIComponent<BattleUIPauseBtn>();
            if (!string.IsNullOrEmpty(data.CustomBattleUI))
                CreateBattleUIComponent(Type.GetType(data.CustomBattleUI, true));
            CreateBattleUIComponent<BattleUICommon>();
        }

        /// <summary>
        /// 获取UI组件
        /// </summary>
        public T GetBattleUIComponent<T>() where T : BattleUIBase
        {
            var key = typeof(T);
            if (_battleUIComponentDic == null)
            {
                return default(T);
            }
            if (_battleUIComponentDic.ContainsKey(key))
                return (T)_battleUIComponentDic[key];
            return default(T);
        }

        /// <summary>
        /// 创建战斗UI组件
        /// </summary>
        public T CreateBattleUIComponent<T>() where T : BattleUIBase
        {
            BattleUIBase battleUI = CreateBattleUIComponent(typeof(T));
            return (T) battleUI;
        }

        public BattleUIBase CreateBattleUIComponent(Type type)
        {
            if (_rootObj == null)
            {
                Logger.LogErrorFormat("root is null");
                return default;
            }
            if (_battleUIComponentDic.ContainsKey(type))
            {
                Logger.LogErrorFormat("m_BattleUIComponentDic is exist key:{0}", type);
                return default;
            }

            BattleUIBase battleUI = Activator.CreateInstance(type) as BattleUIBase;
            if (battleUI == null)
                return default;
            if (!_needUpdateComponents.Contains(battleUI) && battleUI.NeedUpdate())
                _needUpdateComponents.Add(battleUI);
            battleUI.Init(_rootObj);
            _battleUIComponentDic.Add(type, battleUI);
            return battleUI;
        }

        /// <summary>
        /// 进入
        /// </summary>
        public void Enter()
        {
            if (_battleUIComponentDic == null)
                return;
            var enumerator = _battleUIComponentDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                var battleBaseUI = current.Value;
                if (battleBaseUI == null)
                    continue;
                battleBaseUI.Enter();
            }
        }

        /// <summary>
        /// 该函数在OnEnter后面调用
        /// </summary>
        public void Start()
        {
            if (_battleUIComponentDic == null)
                return;
            var enumerator = _battleUIComponentDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                var battleBaseUI = current.Value;
                if (battleBaseUI == null)
                    continue;
                battleBaseUI.Start();
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        public void Exit()
        {
            if (_battleUIComponentDic == null)
                return;
            var enumerator = _battleUIComponentDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                var battleBaseUI = current.Value;
                if (battleBaseUI == null)
                    continue;
                battleBaseUI.Exit();
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update(float timeElapsed)
        {
            if (_battleUIComponentDic == null)
                return;
            for (int i = 0; i < _needUpdateComponents.Count; i++)
            {
                var battleBaseUI = _needUpdateComponents[i];
                if (battleBaseUI == null)
                    continue;
                battleBaseUI.Update(timeElapsed);
            }
        }

        public void DeInit()
        {
            Exit();
            if (_battleUIComponentDic == null)
                return;
            var enumerator = _battleUIComponentDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                var battleBaseUI = current.Value;
                if (battleBaseUI == null)
                    continue;
                battleBaseUI.DeInit();
                battleBaseUI = null;
            }

            if (_battleUIComponentDic != null)
            {
                _battleUIComponentDic.Clear();
                _battleUIComponentDic = null;
            }

            if (_needUpdateComponents != null)
            {
                _needUpdateComponents.Clear();
                _needUpdateComponents = null;
            }
        }
        public void EnableUI(bool bEnable)
        {
            if(_rootObj != null)
            {
                _rootObj.CustomActive(bEnable);
            }
        }
        int m_EnableUIIndex = 0;
        public void EnableUINext(bool bEnable)
        {
            var enumerator = _battleUIComponentDic.GetEnumerator();
            int index = 0;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                var battleBaseUI = current.Value;
                if (battleBaseUI == null)
                    continue;
                index++;
                if (index  > m_EnableUIIndex)
                {
                    battleBaseUI.Enable(bEnable);
                    return;
                }
            }
            m_EnableUIIndex++;
        }
    }
}