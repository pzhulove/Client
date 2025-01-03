using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
namespace GameClient
{
    public class GuildPVEBattleFrame : ClientFrame
    {
        private Image m_HpBar = null;
        private Text m_bossName = null;
        private Image m_bossIcon = null;
        private Text m_hpPercent = null;
        private Button m_dragButton = null;
        private RectTransform m_dragRt = null;
        private GameObject m_hpbarRoot;
        private bool m_isMoveIn = true;
        private float m_HpBarStartPosX = 0;
        protected override void _bindExUI()
        {
            m_hpbarRoot = mBind.GetGameObject("GuildBossTips");

            if (m_hpbarRoot.IsNull()) return;
            m_HpBarStartPosX = m_hpbarRoot.transform.localPosition.x;
            var bind = m_hpbarRoot.GetComponent<ComCommonBind>();
            if (bind.IsNull()) return;

            m_HpBar = bind.GetCom<Image>("HP");
            m_bossIcon = bind.GetCom<Image>("bossIcon");
            m_hpPercent = bind.GetCom<Text>("HPPercent");
            m_bossName = bind.GetCom<Text>("BossName");
            m_dragButton = bind.GetCom<Button>("DraghandleButton");
            m_dragRt = bind.GetCom<RectTransform>("DraghandleRect");

            if(m_dragButton != null)
                m_dragButton.onClick.AddListener(_onDrag);

        }

        protected override void _unbindExUI()
        {
            m_HpBar = null;
            m_bossIcon = null;
            m_hpPercent = null;
            m_bossName = null;
            m_dragButton = null;
            m_dragRt = null;
            if (m_dragButton.IsNull()) return;

            m_dragButton.onClick.RemoveListener(_onDrag);
            m_dragButton = null;
            m_hpbarRoot = null;
        }
        protected sealed override void _OnLoadPrefabFinish()
        {
            if (null == mComClienFrame)
            {
                mComClienFrame = frame.AddComponent<ComClientFrame>();
            }
            mComClienFrame.SetGroupTag("system");
        }

        private void _onDrag()
        {
            if(m_hpbarRoot != null)
            {
                if (m_isMoveIn)
                {
                    var tween = m_hpbarRoot.transform.DOLocalMoveX(m_HpBarStartPosX + 330, 0.5f);
                    if(tween != null)
                    {
                        tween.OnComplete(OnMoveInAnimationComplete);
                    }
                }
                else
                {
                    var tween = m_hpbarRoot.transform.DOLocalMoveX(m_HpBarStartPosX , 0.5f);
                    if(tween != null)
                    {
                        tween.OnComplete(OnMoveOutAnimationComplete);
                    }
                }
            }
            m_isMoveIn = !m_isMoveIn;
        }
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/GuildPVEBattleFrame";
        }
        protected override void _OnOpenFrame()
        {
            _bindUIEvent();
            var bossInfo = userData as GuildPVEBattle.BossInfo;
            if (bossInfo == null) return;
            if(!m_bossName.IsNull())
                m_bossName.text = bossInfo.bossName;
            if(!m_bossIcon.IsNull())
                ETCImageLoader.LoadSprite(ref m_bossIcon, bossInfo.iconPath);
            _RefreshHP(bossInfo);
        }
        protected override void _OnCloseFrame()
        {
            _unBindUIEvent();
        }
        private void _bindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildBossHPRefresh, _OnGuildBossHPRefresh);
        }
        private void _unBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildBossHPRefresh, _OnGuildBossHPRefresh);
        }
        private void _RefreshHP(GuildPVEBattle.BossInfo bossInfo)
        {
            float curHPPercent = 0.0f;
            if (bossInfo.maxHP > 0)
            {
                curHPPercent = ((bossInfo.curHP * 1.0f) / bossInfo.maxHP);
            }
            if (!m_HpBar.IsNull())
            {
                m_HpBar.fillAmount = curHPPercent;
            }
            if (!m_hpPercent.IsNull())
            {
                m_hpPercent.text = BeUtility.Format("{0}%", (int)(curHPPercent * 100));
            }
        }
        private void _OnGuildBossHPRefresh(UIEvent uiEvent)
        {
            var bossInfo = uiEvent.Param1 as GuildPVEBattle.BossInfo;
            if (bossInfo == null) return;
             _RefreshHP(bossInfo);
        }

        private void OnMoveInAnimationComplete()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<GuildPVEBattleFrame>() && m_dragRt != null)
            {
                m_dragRt.localRotation = Quaternion.Euler(0, 0, -180);
                var pos = m_dragRt.transform.localPosition;
                pos.x = -120;
                m_dragRt.transform.localPosition = pos;
            }
        }

        private void OnMoveOutAnimationComplete()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<GuildPVEBattleFrame>() && m_dragRt != null)
            {
                m_dragRt.localRotation = Quaternion.Euler(0, 0, 0);
                var pos = m_dragRt.transform.localPosition;
                pos.x = -100;
                m_dragRt.transform.localPosition = pos;
            }
        }
    }
}

