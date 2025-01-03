using GameClient;
using UnityEngine.UI;

namespace GameClient
{
    /// <summary>
    /// 擂台赛名称
    /// </summary>
    public class BattleUIChampionName : BattleUIBase
    {
        #region ExtraUIBind
        private Text mTextChampionMatch = null;

        protected override void _bindExUI()
        {
            mTextChampionMatch = mBind.GetCom<Text>("TextChampionMatch");
        }

        protected override void _unbindExUI()
        {
            mTextChampionMatch = null;
        }
        #endregion

        protected override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleUIComponent/BattleUIChampionName";
        }

        public void SetChampionMatchName(string name)
        {
            if (mTextChampionMatch != null)
                mTextChampionMatch.text = name;
        }
    }
}