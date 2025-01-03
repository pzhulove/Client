using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GameClient
{
    public class GuildMergeAskItem : MonoBehaviour
    {
        [SerializeField]
        private Text mDesTxt;
        [SerializeField]
        private Image mArgeeImg;
        [SerializeField]
        private Button mViewBtn;

        private GuildEntry mGuildEntry;

        private void Awake()
        {
            mViewBtn.SafeAddOnClickListener(OnViewBtnClick);
        }
        private void OnDestroy()
        {
            mViewBtn.SafeRemoveOnClickListener(OnViewBtnClick);
        }

        private void OnViewBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<GuildMergeAskInfoFrame>(FrameLayer.Middle, mGuildEntry);
        }

        public void SetData(GuildEntry guildEntry)
        {
            if(guildEntry!=null)
            {
                mGuildEntry = guildEntry;
                string des = string.Format(TR.Value("guildmerge_askinfo"), guildEntry.name);
                mDesTxt.SafeSetText(des);
                mArgeeImg.CustomActive(guildEntry.isRequested!=0);
            }
      
        }

       
    }
}
