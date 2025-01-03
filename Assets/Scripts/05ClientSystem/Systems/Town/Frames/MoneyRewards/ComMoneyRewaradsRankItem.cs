using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    class MoneyRewaradsRankItemData
    {
        public int rank;
        public string name;
        public int score;
        public int maxScore;

        public bool Mark
        {
            get
            {
                return rank <= 8 && rank >= 1;
            }
        }

        public string Rank
        {
            get
            {
                if(rank >= 1 && rank <= 100)
                {
                    return rank.ToString();
                }
                return TR.Value("money_rewards_has_no_rank");
            }
        }
    }

    class ComMoneyRewaradsRankItem : MonoBehaviour
    {
        public Text Rank;
        public Image Mark;
        public Text Name;
        public Text CurrentScore;
        public Text MaxScore;
        public Image Line;

        MoneyRewaradsRankItemData data;
        public void OnItemVisible(MoneyRewaradsRankItemData value)
        {
            data = value;
            if(null != data)
            {
                if(null != Rank)
                {
                    Rank.text = data.Rank;
                }

                if(null != Mark)
                {
                    Mark.CustomActive(data.Mark);
                }

                if(null != Name)
                {
                    Name.text = data.name;
                }

                if(null != CurrentScore)
                {
                    CurrentScore.text = data.score.ToString();
                }

                if (null != MaxScore)
                {
                    MaxScore.text = data.maxScore.ToString();
                }

                if (null != Line)
                {
                    Line.CustomActive(data.rank == 8);
                }
            }
        }

        void OnDestroy()
        {
            data = null;
        }
    }
}