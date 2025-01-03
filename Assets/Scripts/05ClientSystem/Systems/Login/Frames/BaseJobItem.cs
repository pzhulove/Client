using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Protocol;
using Network;
using DG.Tweening;
//using RenderHeads.Media.AVProVideo;

namespace GameClient
{
    class BaseJobItem : CachedSelectedObject<ProtoTable.JobTable,BaseJobItem>
    {
        Image jobHead;
        GameObject goCheckMark;
		Image jobName;
		Image imgCheckMark;
        Text tips;
		GameObject goAppointmentRole;

        GameObject imgHeadCutRoot = null;
        Image imgHeadCut = null;
        Image imgJobNameCut = null;
        UIGray appointmentRoleGray = null;

        GameObject unselect = null;
        GameObject select = null;
        GameObject imgLockRoot = null;


        public override void Initialize()
        {

			var bind = goLocal.GetComponent<ComCommonBind>();
			if (bind != null)
			{
				jobHead = bind.GetCom<Image>("imgHead");
				jobName = bind.GetCom<Image>("imgName");
				imgCheckMark = bind.GetCom<Image>("imgCheckMark");
                tips = bind.GetCom<Text>("tips");
				goAppointmentRole = bind.GetGameObject("AppointmentRole");

                imgHeadCutRoot = bind.GetGameObject("imgHeadCutRoot");
                imgHeadCut = bind.GetCom<Image>("imgHeadCut");
                imgJobNameCut = bind.GetCom<Image>("imgJobNameCut");
                appointmentRoleGray = bind.GetCom<UIGray>("appointmentRoleGray");

                unselect = bind.GetGameObject("unselect");
                select = bind.GetGameObject("select");
                imgLockRoot = bind.GetGameObject("imgLockRoot");
            }

            //jobHead = goLocal.GetComponent<Image>();
            goCheckMark = Utility.FindChild(goLocal,"CheckMark");
			//jobName = Utility.FindChild(goLocal,"JobName");

            if(toggle != null)
            {
                toggle.interactable = data.Open == 1;
            }

//             if(tips != null)
//             {
//                 tips.gameObject.CustomActive(data.Open == 0);
//             }
        }

        public override void UnInitialize()
        {
            jobHead = null;
            goCheckMark = null;
			jobName = null;
            goAppointmentRole = null;
            imgHeadCutRoot = null;
            imgHeadCut = null;
            imgJobNameCut = null;
            appointmentRoleGray = null;
            unselect = null;
            select = null;
            imgLockRoot = null;
        }

        public override void OnUpdate()
        {
            // jobHead.sprite = Utility.createSprite((data as ProtoTable.JobTable).JobHead);
            // jobName.sprite = Utility.createSprite((data as ProtoTable.JobTable).JobCreateName);
            Utility.createSprite((data as ProtoTable.JobTable).JobHead, ref jobHead);
            Utility.createSprite((data as ProtoTable.JobTable).JobCreateName, ref jobName);
            goAppointmentRole.CustomActive(ClientApplication.playerinfo.GetRoleHasApponintmentOccu((int)(data as ProtoTable.JobTable).ID));
            Utility.createSprite((data as ProtoTable.JobTable).JobHead, ref imgHeadCut);
            Utility.createSprite((data as ProtoTable.JobTable).JobCreateName, ref imgJobNameCut);

            if(data != null && data.Open == 0)
            {
                OnDisplayChanged(false);
            }
        }

        public override void OnDisplayChanged(bool bShow)
        {
            //goCheckMark.CustomActive(bShow);
            //imgCheckMark.gameObject.CustomActive(!bShow);

            select.CustomActive(bShow);
            unselect.CustomActive(!bShow);
            
            if(data != null)
            {
                imgHeadCutRoot.CustomActive(data.Open != 0);
                imgLockRoot.CustomActive(data.Open == 0);
            }

            if(appointmentRoleGray != null)
            {
                appointmentRoleGray.SetEnable(false);
                appointmentRoleGray.SetEnable(!bShow);
            }
        }
    }
}