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
    class ChangeJobItem : CachedSelectedObject<ProtoTable.JobTable, ChangeJobItem>
    {
        static Vector3 ms_scale_selected = new Vector3(1.02f, 1.0f, 1.0f);
        static Vector3 ms_scale_normal = new Vector3(1.0f, 1.0f, 1.0f);
        static Vector3 ms_text_selected = new Vector3(0.98f, 1.0f, 1.0f);
        Text jobName;
        GameObject goCheckMark;
        Text checkJobName;
        ToggleGroup toggleGroup;
        Image imgBack;


        Image jobHead;  
        Text textJobName;
        Image imgCheckMark;
        Text tips;
        GameObject goAppointmentRole;

        GameObject imgHeadCutRoot = null;
        Image imgHeadCut = null;
        Text textJobNameCut = null;
        UIGray appointmentRoleGray = null;

        GameObject unselect = null;
        GameObject select = null;
        GameObject imgLockRoot = null;

        public override void Initialize()
        {
//             jobName = Utility.FindComponent<Text>(goLocal, "Text");
//             goCheckMark = Utility.FindChild(goLocal, "CheckMark");
//             checkJobName = Utility.FindComponent<Text>(goLocal, "CheckMark/Text");
//             imgBack = goLocal.GetComponent<Image>();


            var bind = goLocal.GetComponent<ComCommonBind>();
            if (bind != null)
            {
                jobHead = bind.GetCom<Image>("imgHead");
                textJobName = bind.GetCom<Text>("textJobName");
                imgCheckMark = bind.GetCom<Image>("imgCheckMark");
                tips = bind.GetCom<Text>("tips");
                goAppointmentRole = bind.GetGameObject("AppointmentRole");

                imgHeadCutRoot = bind.GetGameObject("imgHeadCutRoot");
                imgHeadCut = bind.GetCom<Image>("imgHeadCut");
                textJobNameCut = bind.GetCom<Text>("textJobNameCut");
                appointmentRoleGray = bind.GetCom<UIGray>("appointmentRoleGray");

                unselect = bind.GetGameObject("unselect");
                select = bind.GetGameObject("select");
                imgLockRoot = bind.GetGameObject("imgLockRoot");
            }

            if (Value.Open <= 0)
            {
                toggleGroup = toggle.group;
                toggle.group = null;
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener((bool bValue) =>
                {
                    if(bValue)
                    {
                        toggle.isOn = false;
                        if(onSelected != null)
                        {
                            onSelected.Invoke(data);
                        }
                        OnDisplayChanged(false);
                    }
                });
            }
            else
            {
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener((bool bValue) =>
                {
                    if (bValue && onSelected != null)
                    {
                        if (Selected != this)
                        {
                            if (Selected != null)
                            {
                                Selected.OnDisplayChanged(false);
                            }
                            Selected = this;
                        }
                        Selected.OnDisplayChanged(true);
                        onSelected.Invoke(data);
                    }
                });
            }
        }

        public override void OnRecycle()
        {
            base.OnRecycle();
            if(toggleGroup != null)
            {
                toggle.group = toggleGroup;
                toggleGroup = null;
            }
        }

        public override void UnInitialize()
        {
            checkJobName = null;
            goCheckMark = null;
            jobName = null;

            jobHead = null;
            goCheckMark = null;
            jobName = null;
            textJobName = null;
            goAppointmentRole = null;
            imgHeadCutRoot = null;
            imgHeadCut = null;
            textJobNameCut = null;
            appointmentRoleGray = null;
            unselect = null;
            select = null;
            imgLockRoot = null;
        }

        public override void OnUpdate()
        {
            textJobName.text = (data as ProtoTable.JobTable).Name;
            textJobNameCut.text = (data as ProtoTable.JobTable).Name;

            Utility.createSprite((data as ProtoTable.JobTable).JobHead, ref jobHead);
            Utility.createSprite((data as ProtoTable.JobTable).JobHead, ref imgHeadCut);           
        }

        public override void OnDisplayChanged(bool bShow)
        {
            select.CustomActive(bShow);
            unselect.CustomActive(!bShow);

            if (Value != null)
            {
                imgHeadCutRoot.CustomActive(Value.Open != 0);
                imgLockRoot.CustomActive(Value.Open == 0);
                
                textJobNameCut.CustomActive(Value.Open != 0);
                tips.CustomActive(Value.Open == 0);
            }            
        }
    }
}
