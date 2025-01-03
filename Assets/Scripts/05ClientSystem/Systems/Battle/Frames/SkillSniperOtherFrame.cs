using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace GameClient
{
    public class SkillSniperOtherFrame : ClientFrame
    {
        #region ExtraUIBind
        private RectTransform mTarget = null;
        private DOTweenAnimation mFireAni = null;
        private DOTweenAnimation mCloseAni = null;
        private RectTransform mShotEffect = null;
        
        protected float mPreMoveTime = 0.2f;

        protected override void _bindExUI()
        {
            mTarget = mBind.GetCom<RectTransform>("Target");
            mFireAni = mBind.GetCom<DOTweenAnimation>("FireAni");
            mCloseAni = mBind.GetCom<DOTweenAnimation>("CloseAni");
            mShotEffect = mBind.GetCom<RectTransform>("ShotEffect");
        }

        protected override void _unbindExUI()
        {
            mTarget = null;
            mFireAni = null;
            mCloseAni = null;
            mShotEffect = null;
        }
        #endregion

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/SkillSniperOtherFrame";
        }

        //设置枪口的坐标
        public void QiangkouMove(Vector3  pos)
        {
            if (mTarget != null)
            {
                DOTween.To(() => mTarget.transform.localPosition, r => mTarget.transform.localPosition = r, pos, mPreMoveTime).SetEase(Ease.OutQuad);
            }
        }

        //进行一次攻击
        public void Attack()
        {
            PlayAttackEffect();
        }

        public RectTransform GetTargetParent()
        {
            return mTarget.parent as RectTransform;
        }

        //播放攻击特效
        protected void PlayAttackEffect()
        {
            if (mShotEffect == null || ClientSystemManager.GetInstance() == null)
                return;
            mShotEffect.gameObject.CustomActive(true);
            ClientSystemManager.GetInstance().delayCaller.DelayCall(300, () =>
            {
                if (mShotEffect != null)
                {
                    mShotEffect.gameObject.CustomActive(false);
                }
            }, 0, 0, true);
        }

        public GameObject gameObject
        {
            get
            {
                return frame;
            }
        }

        public void PlayCloseAni()
        {
            if (mCloseAni != null)
            {
                mCloseAni.DORestartById("xiaoshi");
            }
        }
    }
}
