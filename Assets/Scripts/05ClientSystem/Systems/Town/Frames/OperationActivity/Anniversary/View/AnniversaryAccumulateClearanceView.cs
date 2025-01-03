using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GameClient
{
    public class AnniversaryAccumulateClearanceView : LimitTimeActivityViewCommon
    {

        [SerializeField]
        private ActiveUpdate mActiveUpdate;

    
        public override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            base.Init(model, onItemClick);
          
           if(mActiveUpdate!=null)
            {
                mActiveUpdate.SetTotlaNum((int)model.Param);
            }
        }

        
    }
}
