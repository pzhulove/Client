using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

namespace GameClient
{
    class BudoResultFrameData
    {
        public bool bNeedOpenBudoInfo = false;
        public bool bOver = false;
        public PKResult eResult = PKResult.DRAW;
        public delegate void OnClose();
        public OnClose onClose = null;
        public bool bDebug = false;
    }

    class BudoResultFrame : ClientFrame
    {
        public static void Open(BudoResultFrameData data)
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<BudoResultFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<BudoResultFrame>();
            }
            ClientSystemManager.GetInstance().OpenFrame<BudoResultFrame>(FrameLayer.Middle, data);
        }

        public override string GetPrefabPath()
        {
            m_kData = userData as BudoResultFrameData;
            if(m_kData == null)
            {
                Logger.LogError("【BudoResultFrame】 must set a BudoResultFrameData!!");
                return "";
            }

            if(m_kData.bOver)
            {
                return "UIFlatten/Prefabs/Budo/BudoResult_over";
            }
            else if(m_kData.eResult == PKResult.LOSE)
            {
                return "UIFlatten/Prefabs/Budo/BudoResult_lose_ing";
            }
            else 
                return "UIFlatten/Prefabs/Budo/BudoResult_Win_ing";
        }

       
        BudoResultFrameData m_kData = null;
        ComCommonBind       m_kComBind;
 

        void _OnClickAward()
        {
            var itemData = BudoManager.GetInstance().GetJarDataByTimes();
            if (m_kData.bDebug)
            {
                itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(330000006);
            }
            if (itemData != null)
            {

                BudoManager.GetInstance().SendSceneWudaoRewardReq();
                BudoManager.GetInstance().NeedOpenBudoInfoFrame = m_kData.bNeedOpenBudoInfo;
                frameMgr.CloseFrame(this);
            }
            
        }

       

        Text        m_kJarName;
        const int   mLoseStarCount = 3;
        GameObject[] m_LoseStar = new GameObject[mLoseStarCount];
        Text    m_kBudoWinTimes;
        Button  m_Close;
        Button  m_JarOpen;
        Text    m_OverText;
        Text    m_kClickHint;
        bool    m_bCanClose = false;

        void ShowCurrentLoseStars(int count,bool bAnimation)
        {
            for(int i = 0; i < mLoseStarCount; ++i)
            {
                m_LoseStar[i].CustomActive(i <= count);
            }

            if(bAnimation && count >= 0 && count < mLoseStarCount)
            {
                DOTweenAnimation[] anims = m_LoseStar[count].GetComponents<DOTweenAnimation>();
                for(int j = 0; j < anims.Length; ++j)
                {
                    anims[j].autoPlay = true;
                    anims[j].CreateTween();
                }
            }
        }

        protected override void _OnOpenFrame()
        {
            m_kComBind = frame.GetComponent<ComCommonBind>();

            m_kJarName = m_kComBind.GetCom<Text>("JarName");
            m_LoseStar[0] = m_kComBind.GetGameObject("LoseStar1");
            m_LoseStar[1] = m_kComBind.GetGameObject("LoseStar2");
            m_LoseStar[2] = m_kComBind.GetGameObject("LoseStar3");
            m_kBudoWinTimes = m_kComBind.GetCom<Text>("WinText");
            m_Close = m_kComBind.GetCom<Button>("Close");
            if(m_Close != null)
            m_Close.onClick.AddListener(_OnClickClose);
            m_JarOpen = m_kComBind.GetCom<Button>("JarOpen");

            if(m_JarOpen != null)
            {
                m_JarOpen.onClick.AddListener(_OnClickAward);
            }

            m_OverText = m_kComBind.GetCom<Text>("OverText");
            m_kClickHint = mBind.GetCom<Text>("ClickHint");

            ShowCurrentLoseStars(BudoManager.GetInstance().LoseTimes - 1,m_kData.eResult == PKResult.LOSE);

            if(m_kData.bOver)
            {
                
            }
            else if(m_kData.eResult == PKResult.LOSE)
            {

            }

            
            m_bCanClose = false;
            InvokeMethod.Invoke(this, 1.0f, () =>
            {
                m_bCanClose = true;
            });
            
            _UpdateBudoInfo();
        }

        bool mCanAward = false;
        void _UpdateBudoInfo()
        {
            if(m_kData.bOver && m_kData.eResult == PKResult.WIN || m_kData.bDebug)
            {
                m_kBudoWinTimes.text = string.Format(TR.Value("budo_win_times"), BudoManager.GetInstance().WinTimes - 1);

                var itemData = BudoManager.GetInstance().GetPreJarDataByTimes();
                if(m_kData.bDebug)
                {
                    itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(330000002);
                }
                if (itemData != null)
                {
                    m_kJarName.text = itemData.GetColorName();
                }
            }
            else
            {
                m_kBudoWinTimes.text = string.Format(TR.Value("budo_win_times"), BudoManager.GetInstance().WinTimes);

                var itemData = BudoManager.GetInstance().GetJarDataByTimes();
                if (itemData != null)
                {
                    m_kJarName.text = itemData.GetColorName();
                }
            }

            bool bCanAward = BudoManager.GetInstance().CanAcqured;
            mCanAward = bCanAward;

            if(!bCanAward)
            {
                m_kClickHint.text = "点击屏幕任意位置关闭";
            }
            else
            {
                m_kClickHint.text = "请领取武道会奖励!";
            }
        }

        void _OnClickClose()
        {
            if(!m_bCanClose)
            {
                return;
            }
            
            if(mCanAward)
            {
                return;
            }
            
            frameMgr.CloseFrame(this);
        }

        protected override void _OnCloseFrame()
        {
            m_kData = null;
            InvokeMethod.RemoveInvokeCall(this);
        }
    }
}