using DataModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    namespace ActivityTreasureLottery
    {
        public class ActivityTreasureLotteryRankItem : MonoBehaviour
        {
            [SerializeField] private TextEx mTextName;
            [SerializeField] private TextEx mTextInfo;

            public void Init(IActivityTreasureLotteryDrawModel model, int index)
            {
                mTextName.SafeSetText(string.Format("{0}.{1}", index + 1, model.TopFiveInvestPlayers[index].Name));
                mTextInfo.SafeSetText(string.Format("{0}%{1}", model.TopFiveInvestPlayers[index].Rate, model.TopFiveInvestPlayers[index].ServerName));
            }
        }
    }
}
