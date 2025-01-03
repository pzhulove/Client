using System.Collections;
using UnityEngine;

namespace GameClient
{
    public class DungeonPreTips : ClientFrame
    {
        #region Override
        public override string GetPrefabPath()
        {
            return "UI/Prefabs/Chapter/DungeonPreTips";
        }
        #endregion


        [UIControl("Root/Board/Item{0}", typeof(ComMouCountTips), 0)]
        private ComMouCountTips[] mMoutips = new ComMouCountTips[3];

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            for (int i = 0; i < mMoutips.Length; ++i)
            {
                mMoutips[i].SetMonsterNumber();
            }

            StartCoroutine(_delayClose(2));
        }

        private IEnumerator _delayClose(int s)
        {
            yield return Yielders.GetWaitForSeconds(s);

            _onHandle();
        }

        [UIEventHandle("Background")]
        private void _onHandle()
        {
            Close();
        }
    }
}
