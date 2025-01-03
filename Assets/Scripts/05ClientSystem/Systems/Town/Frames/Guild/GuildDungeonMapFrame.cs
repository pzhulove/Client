using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;
using System.Reflection;

namespace GameClient
{
    public class GuildDungeonMapFrame : ClientFrame
    {
        #region val

        Dictionary<int, int> buffAddInfo = null;        
        #endregion

        #region ui bind

        Button btnClose = null;
        JuniorGuildDungeon Junior0 = null;
        JuniorGuildDungeon Junior1 = null;
        JuniorGuildDungeon Junior2 = null;

        MediumGuildDungeon Medium0 = null;
        MediumGuildDungeon Medium1 = null;
        MediumGuildDungeon Medium2 = null;

        BossGuildDungeon BOSS = null;

        Text buf1 = null;    
        GameObject buffers = null;
        Text txtNoBuf = null;

        Button btLookUp = null;

        private Text txtKillInfo = null;
        private Slider process = null;

        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildDungeonMap";
        }

        protected override void _OnOpenFrame()
        {
            buffAddInfo = new Dictionary<int, int>();
            BindUIEvent();  
            GuildDataManager.GetInstance().SendWorldGuildDungeonCopyReq();
            _OnUpdateMapInfo(null);
            _OnUpdateBufInfo(null);
            _OnUpdateActivityData(null);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildDungeonShowFireworks);
        }

        protected override void _OnCloseFrame()
        {
            buffAddInfo = null;
            UnBindUIEvent();
        }

        protected override void _bindExUI()
        {
            btnClose = mBind.GetCom<Button>("BtnClose");         
            if (btnClose != null)
            {
                btnClose.onClick.RemoveAllListeners();
                btnClose.onClick.AddListener(() => 
                {
                    GuildDataManager.GetInstance().SwitchToGuildScene();
                    frameMgr.CloseFrame(this);
                });
            }

            Junior0 = mBind.GetCom<JuniorGuildDungeon>("Junior0");
            Junior1 = mBind.GetCom<JuniorGuildDungeon>("Junior1");
            Junior2 = mBind.GetCom<JuniorGuildDungeon>("Junior2");

            Medium0 = mBind.GetCom<MediumGuildDungeon>("Medium0");
            Medium1 = mBind.GetCom<MediumGuildDungeon>("Medium1");
            Medium2 = mBind.GetCom<MediumGuildDungeon>("Medium2");

            BOSS = mBind.GetCom<BossGuildDungeon>("BOSS");

            buf1 = mBind.GetCom<Text>("buf1");
            buffers = mBind.GetGameObject("buffers");
            txtNoBuf = mBind.GetCom<Text>("txtNoBuf");

            btLookUp = mBind.GetCom<Button>("btLookUp");
            if(btLookUp != null)
            {
                btLookUp.onClick.RemoveAllListeners();
                btLookUp.onClick.AddListener(() => 
                {
                    ClientSystemManager.GetInstance().OpenFrame<GuildDungeonBufAddUpDetailFrame>();
                });
            }

            txtKillInfo = mBind.GetCom<Text>("txtKillInfo");
            process = mBind.GetCom<Slider>("process");
        }

        protected override void _unbindExUI()
        {
            btnClose = null;
            Junior0 = null;
            Junior1 = null;
            Junior2 = null;
            Medium0 = null;
            Medium1 = null;
            Medium2 = null;
            BOSS = null;
            buf1 = null;
            buffers = null;
            btLookUp = null;
            txtKillInfo = null;
            process = null;
            txtNoBuf = null;
        }

        #endregion 

        #region method

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildDungeonUpdateDungeonMapInfo, _OnUpdateMapInfo);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildDungeonUpdateDungeonBufInfo, _OnUpdateBufInfo);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildDungeonUpdateActivityData, _OnUpdateActivityData);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildDungeonUpdateDungeonMapInfo, _OnUpdateMapInfo);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildDungeonUpdateDungeonBufInfo, _OnUpdateBufInfo);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildDungeonUpdateActivityData, _OnUpdateActivityData);
        }      

        void UpdateKillInfo()
        {
            GuildDataManager.GuildDungeonActivityData data = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
            if (data != null)
            {
                if (txtKillInfo != null)
                {
                    txtKillInfo.text = string.Format("{0}/{1}", data.nBossOddHp, data.nBossMaxHp);
                }

                if (process != null && data.nBossMaxHp > 0)
                {
                    process.value = (float)data.nBossOddHp / (float)data.nBossMaxHp;
                }
            }
        }
        
        void CalBufAddInfo()
        {
            if(buffAddInfo == null)
            {
                return;
            }

            buffAddInfo.Clear();
            return;
        }

        string GetBufAddType(int iType)
        {
            return "";
        }        

        #endregion

        #region ui event

        private void _OnUpdateBufInfo(UIEvent uiEvent)
        {
            GuildDataManager.GuildDungeonActivityData data = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
            if (data == null)
            {
                return;
            }

            if(buffers != null && buf1 != null)
            {       
                for (int i = 0; i < buffers.transform.childCount; ++i)
                {
                    GameObject go = buffers.transform.GetChild(i).gameObject;    
                    GameObject.Destroy(go);
                }

                for (int i = 0;i < data.buffAddUpInfos.Count;i++)
                {
                    GuildDataManager.BuffAddUpInfo addInfo = data.buffAddUpInfos[i];
                    if(addInfo == null)
                    {
                        continue;
                    }

                    GameObject goCurrent = GameObject.Instantiate(buf1.gameObject);
                    Utility.AttachTo(goCurrent, buffers);
                    goCurrent.CustomActive(true);

                    Text txtBuf = goCurrent.GetComponent<Text>();
                    if (txtBuf != null)
                    {
                        txtBuf.text = GuildDataManager.GetBuffAddUpInfo((int)addInfo.nBuffID, (int)addInfo.nBuffLv);
                    }
                }

                txtNoBuf.CustomActive(data.buffAddUpInfos.Count == 0);
            }           

            return;
        }

        private void _OnUpdateMapInfo(UIEvent uiEvent)
        {
            GuildDataManager.GuildDungeonActivityData data = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
            if (data == null)
            {
                return;
            }

            List<JuniorGuildDungeon> juniors = new List<JuniorGuildDungeon>();
            juniors.Add(Junior0);
            juniors.Add(Junior1);
            juniors.Add(Junior2);

            List<MediumGuildDungeon> mediums = new List<MediumGuildDungeon>();
            mediums.Add(Medium0);
            mediums.Add(Medium1);
            mediums.Add(Medium2);

            for(int i = 0;i < data.juniorDungeonDamgeInfos.Count;i++)
            {
                if(i < juniors.Count)
                {
                    JuniorGuildDungeon junior = juniors[i];
                    if(junior != null)
                    {
                        junior.SetUp(data.juniorDungeonDamgeInfos[i]);
                    }
                }
            }

            for(int i = 0;i < data.mediumDungeonDamgeInfos.Count;i++)
            {
                if(i < mediums.Count)
                {
                    MediumGuildDungeon medium = mediums[i];
                    if(medium != null)
                    {
                        medium.SetUp(data.mediumDungeonDamgeInfos[i]);
                    }
                }
            }

           if(data.bossDungeonDamageInfos.Count > 0 && BOSS != null)
           {
                BOSS.SetUp(data.bossDungeonDamageInfos[0]);
           }
        }

        private void _OnUpdateActivityData(UIEvent uiEvent)
        {
            UpdateKillInfo();
        }

        #endregion
    }
}
