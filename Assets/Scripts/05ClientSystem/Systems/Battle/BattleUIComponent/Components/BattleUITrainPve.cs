using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameClient
{
    /// <summary>
    /// Pve练习场相关UI
    /// </summary>
    public class BattleUITrainPve : BattleUIBase
    {
        public BattleUITrainPve() : base() { }

        protected override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleUITrainPve";
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            InitTrainingPveBattle();
        }

        //初始化修炼场UI数据
        protected void InitTrainingPveBattle()
        {
            if (BattleMain.instance == null)
                return;
            var battle = BattleMain.instance.GetBattle() as TrainingPVEBattle;
            if (battle == null)
                return;
            battle.Init();
        }
    }
}