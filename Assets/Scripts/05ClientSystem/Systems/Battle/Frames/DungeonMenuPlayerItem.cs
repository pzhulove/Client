using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class DungeonMenuPlayerItem : MonoBehaviour
    {
        [SerializeField]
        private Text mName;
        [SerializeField]
        private Text mJobName;
        [SerializeField]
        private Text mLevel;
        [SerializeField]
        private Text mDamage;
        [SerializeField]
        private Text mPrecentage;
        [SerializeField]
        private Slider mDamageSlider;
        [SerializeField]
        private GameObject mHighestScore;

        public void InitPlayerItem(BattlePlayer battlePlayer,long maxScore,long totalScore,bool isHighest)
        {
            long maxDamage = maxScore;
            long totalDamage = totalScore;
            BeEntity entity = battlePlayer.playerActor.GetTopOwner(battlePlayer.playerActor);
            if (entity == null)
            {
                return;
            }
            long tempDamage = entity.GetEntityData().battleData.GetTotalDamage();
            //long tempDamage = battlePlayer.playerActor.GetEntityData().battleData.GetTotalDamage();
            if (mName != null)
            {
                mName.text = battlePlayer.playerInfo.name;
            }
            if(mJobName != null)
            {
                mJobName.text = Utility.GetJobName(battlePlayer.playerInfo.occupation, 0);
            }
            if(mLevel != null)
            {
                mLevel.text = string.Format("Lv.{0}",battlePlayer.playerInfo.level.ToString());
            }
            if(mDamage != null)
            {
                mDamage.text = tempDamage.ToString();
            }
            if(mPrecentage != null)
            {
                if(totalDamage != 0)
                {
                    mPrecentage.text = string.Format("{0}%", (tempDamage * 100.0 / totalDamage).ToString("f1"));
                }
            }
            if(mDamageSlider != null)
            {
                if(maxDamage != 0)
                {
                    mDamageSlider.value = tempDamage * 1.0f / maxDamage;
                }
            }
            if(mHighestScore != null)
            {
                if(isHighest)
                {
                    mHighestScore.CustomActive(true);
                }
                else
                {
                    mHighestScore.CustomActive(false);
                }
            }
        }
    }
}

