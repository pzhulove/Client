using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using JetBrains.Annotations;

namespace GameClient
{
    public class SkillMonsterSniperOtherFrame : SkillMonsterSniperFrame
    {
        protected override void _bindExUI()
        {
            base._bindExUI();
            gameObject.SetActive(false);
        }
        public override void SniperMove(Vector3  pos)
        {
            gameObject.SetActive(true);
            if (mTarget != null)
                DOTween.To(() => mTarget.anchoredPosition, r => mTarget.anchoredPosition = r, new Vector2(pos.x, pos.y), 0.25f).SetEase(Ease.OutQuad);
        }
    }
    public class SkillMonsterSniperFrame : ClientFrame
    {
        #region ExtraUIBind
        protected RectTransform mTarget = null;
        protected GameObject mShotEffectNode = null;
        protected GameObject mScreenEffectNode = null;

        private GameObject mShotEffect = null;
        private GameObject mScreenEffect = null;

        private readonly string ShotEffectPath =
            "Effects/Hero_Zhihuiguan/Eff_Zhihuiguan_Jujishou/Prefab/Eff_Zhihuiguan_Jujishou_UI_beiji01";
        private readonly string ScreenEffectPath =
            "Effects/Hero_Zhihuiguan/Eff_Zhihuiguan_Jujishou/Prefab/Eff_Zhihuiguan_Jujishou_UI_beiji02";
        

        protected override void _bindExUI()
        {
            mTarget = mBind.GetCom<RectTransform>("Target");
            mShotEffectNode = mBind.GetGameObject("ShotEffect");
            mScreenEffectNode = mBind.GetGameObject("ScreenEffect");

            if (mShotEffectNode != null) mShotEffectNode.SetActive(false);
            if (mScreenEffectNode != null) mScreenEffectNode.SetActive(false);
            
            InitScreenLimit();
            InitEffect();
        }

        private void InitEffect()
        {
            if (mShotEffect == null)
                mShotEffect = CreateEffect(ShotEffectPath, mShotEffectNode.gameObject);
            
            if (mScreenEffect == null)
                mScreenEffect = CreateEffect(ScreenEffectPath, mScreenEffect.gameObject);
        }
        private GameObject CreateEffect(string path, GameObject parent)
        {
            if (parent == null) 
                return null;
            GameObject effect = AssetLoader.instance.LoadResAsGameObject(path);
            if (effect != null)
            {
                Utility.AttachTo(effect, parent);
            }

            return effect;
        }
        
        protected override void _unbindExUI()
        {
            mTarget = null;
            mShotEffectNode = null;
            mScreenEffectNode = null;
            mShotEffect = null;
            mScreenEffect = null;
        }
        #endregion
        
        protected float[] m_XLimitArr = new float[2];
        protected float[] m_YLimitArr = new float[2];
        protected int m_SignRadius = 260;
        protected int m_EndScale = 1000;
        protected int m_BeginScale = 2000;
        protected int m_CurScale = 1000;
        protected int m_ScaleSpeed = 50;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/SkillMonsterSniperFrame";
        }

        public void InitSniperScale(int endScale, int beginScale, int scaleSpeed)
        {
            m_ScaleSpeed = scaleSpeed;
            m_EndScale = endScale;
            m_BeginScale = beginScale;
            _SetSniperScale(m_BeginScale);
        }

        public void SniperScale()
        {
            if (m_CurScale <= m_EndScale)
                return;

            var scale = m_CurScale - m_ScaleSpeed;
            _SetSniperScale(scale);
        }
        private void _SetSniperScale(int scale)
        {
            if(mTarget == null)
                return;
            
            m_CurScale = scale;
            mTarget.localScale = Vector3.one * scale / 1000f;
        }
        
        protected void InitScreenLimit()
        {
            var canvas = GetTargetParent();
            var rect = canvas.rect;
            m_XLimitArr[0] = -rect.width / 2 + m_SignRadius;
            m_XLimitArr[1] = rect.width / 2 - m_SignRadius;

            m_YLimitArr[0] = -rect.height / 2 + m_SignRadius;
            m_YLimitArr[1] = rect.height / 2 - m_SignRadius;
        }

        //设置枪口的坐标
        public virtual void SniperMove(Vector3  pos)
        {
            if (pos.x > m_XLimitArr[1])
                pos.x = m_XLimitArr[1];

            if (pos.x < m_XLimitArr[0])
                pos.x = m_XLimitArr[0];

            if (pos.y > m_YLimitArr[1])
                pos.y = m_YLimitArr[1];

            if (pos.y < m_YLimitArr[0])
                pos.y = m_YLimitArr[0];

            if (mTarget != null) 
                mTarget.anchoredPosition = pos; 
        }

        private RectTransform GetTargetParent()
        {
            return mTarget.parent as RectTransform;
        }

        public Vector3 GetUIPointInFrame(Vector3 screenPos)
        {
            var localPos = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GetTargetParent(), screenPos, ClientSystemManager.GetInstance().UICamera, out localPos);
            return localPos;
        }
        //播放攻击特效
        public void PlayAttackEffect()
        {
            if (mShotEffectNode == null || mScreenEffectNode == null || ClientSystemManager.GetInstance() == null)
                return;
            mShotEffectNode.CustomActive(false);
            mScreenEffectNode.CustomActive(false);
            mShotEffectNode.CustomActive(true);
            mScreenEffectNode.CustomActive(true);
        }

        public void ResetScale()
        {
            _SetSniperScale(2000);
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
            //if (mCloseAni != null)
            //{
            //    mCloseAni.DORestartById("xiaoshi");
            //}
        }

        /// <summary>
        /// 检测目标是否在枪口内
        /// </summary>
        public bool CheckTargetInSign(Vector2 targetUIPos, int radius)
        {
            var uiPos = new Vector3(targetUIPos.x, targetUIPos.y,0 );
            return (mTarget.transform.localPosition - uiPos).magnitude < radius;
        }

        private Plane mGroundPlane = new Plane(Vector3.up, Vector3.zero);
        //获取准星对应的场景坐标
        public Vector3 GetSniperScenePos(){
        
            Vector3 pos = Vector3.zero;
            var vec = ClientSystemManager.GetInstance().UICamera.WorldToScreenPoint(mTarget.position);
            var ray = Camera.main.ScreenPointToRay(new Vector3(vec.x, vec.y, 0));
            float enter = 0;
            mGroundPlane.Raycast(ray, out enter);
            var outPos = ray.GetPoint(enter);
            return outPos;
        }
    }
}
