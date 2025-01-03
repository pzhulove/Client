using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GameClient
{
    public class ComboSkipFrame : ClientFrame
    {
        private Button skipBtn;
        private Button needBtn;
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/ComboSkipFrame";
        }

        protected override void _bindExUI()
        {
            base._bindExUI();
            skipBtn = mBind.GetCom<Button>("cancel");
            needBtn = mBind.GetCom<Button>("ok");

            skipBtn.onClick.RemoveAllListeners();
            skipBtn.onClick.AddListener(Skip);

            needBtn.onClick.RemoveAllListeners();
            needBtn.onClick.AddListener(WatchDemonstration);
        }

        void WatchDemonstration()
        {
            Close();
            GameFrameWork.instance.StartCoroutine(SkillComboControl.instance.StartCastSkill());
            
        }

        void Skip()
        {
            Close();
            SkillComboControl.instance.hasPassed = true;
            GameFrameWork.instance.StartCoroutine(SkillComboControl.instance.StartCastSkill());
           
        }

    }
}