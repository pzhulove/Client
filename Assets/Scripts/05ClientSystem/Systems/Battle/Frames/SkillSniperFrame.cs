using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace GameClient
{
    public class SkillSniperFrame : ClientFrame
    {
        #region ExtraUIBind
        private RectTransform mTarget = null;
        private Image mMaskMaterial = null;
        private RectTransform mBullteContainer = null;
        private RectTransform mShotEffect = null;
        private Toggle mZidanTemp = null;
        private List<Toggle> mZidanList = new List<Toggle>();
        private DOTweenAnimation mFireAni = null;
        private DOTweenAnimation mCloseAni = null;
        private DOTweenAnimation mDakeAni = null;
        private RectTransform mCdDi = null;
        private RectTransform mCdSlider = null;
        private Text mSkillTime = null;
        private CanvasGroup mZidanCanvas = null;
        private CanvasGroup mCdDiCanvas = null;
        private CanvasGroup mCDSliderCanvas = null;
        private CanvasGroup mSkillTimeCanvas = null;
        private CanvasGroup mImageDiCanvas = null;
        private Image mCDSlider1 = null;
        private Image mCDSlider2 = null;
        private Image mCDSlider3 = null;
        private Image mCDSlider4 = null;
        private Image mCDSlider5 = null;
        private Image mCDSlider6 = null;
        private Image mCDSlider7 = null;
        private Image mCDSlider8 = null;
        private Image mCDSlider9 = null;
        private Image mCDSlider10 = null;

        protected override void _bindExUI()
        {
            mTarget = mBind.GetCom<RectTransform>("Target");
            mMaskMaterial = mBind.GetCom<Image>("maskMaterial");
            mBullteContainer = mBind.GetCom<RectTransform>("BullteContainer");
            mShotEffect = mBind.GetCom<RectTransform>("ShotEffect");
            mZidanTemp = mBind.GetCom<Toggle>("ZidanTemp");
            mFireAni = mBind.GetCom<DOTweenAnimation>("FireAni");
            mCloseAni = mBind.GetCom<DOTweenAnimation>("CloseAni");
            mDakeAni = mBind.GetCom<DOTweenAnimation>("DakeAni");
            mCdDi = mBind.GetCom<RectTransform>("CdDi");
            mCdSlider = mBind.GetCom<RectTransform>("CdSlider");
            mSkillTime = mBind.GetCom<Text>("SkillTime");
            mZidanCanvas = mBind.GetCom<CanvasGroup>("ZidanCanvas");
            mCdDiCanvas = mBind.GetCom<CanvasGroup>("CdDiCanvas");
            mCDSliderCanvas = mBind.GetCom<CanvasGroup>("CDSliderCanvas");
            mSkillTimeCanvas = mBind.GetCom<CanvasGroup>("SkillTimeCanvas");
            mImageDiCanvas = mBind.GetCom<CanvasGroup>("ImageDiCanvas");
            mCDSlider1 = mBind.GetCom<Image>("CDSlider1");
            mCDSlider2 = mBind.GetCom<Image>("CDSlider2");
            mCDSlider3 = mBind.GetCom<Image>("CDSlider3");
            mCDSlider4 = mBind.GetCom<Image>("CDSlider4");
            mCDSlider5 = mBind.GetCom<Image>("CDSlider5");
            mCDSlider6 = mBind.GetCom<Image>("CDSlider6");
            mCDSlider7 = mBind.GetCom<Image>("CDSlider7");
            mCDSlider8 = mBind.GetCom<Image>("CDSlider8");
            mCDSlider9 = mBind.GetCom<Image>("CDSlider9");
            mCDSlider10 = mBind.GetCom<Image>("CDSlider10");
        }

        protected override void _unbindExUI()
        {
            mTarget = null;
            mMaskMaterial = null;
            mBullteContainer = null;
            mShotEffect = null;
            mZidanTemp = null;
            mFireAni = null;
            mCloseAni = null;
            mDakeAni = null;
            mCdDi = null;
            mCdSlider = null;
            mSkillTime = null;
            mZidanCanvas = null;
            mCdDiCanvas = null;
            mCDSliderCanvas = null;
            mSkillTimeCanvas = null;
            mImageDiCanvas = null;
            mCDSlider1 = null;
            mCDSlider2 = null;
            mCDSlider3 = null;
            mCDSlider4 = null;
            mCDSlider5 = null;
            mCDSlider6 = null;
            mCDSlider7 = null;
            mCDSlider8 = null;
            mCDSlider9 = null;
            mCDSlider10 = null;
        }
        #endregion

        public Vector3 targetPos;
        public float targetMoveSpeed = 22f;                     //狙击枪口移动速度
        Vector2 v = Vector2.zero;
        private Vector3 cameraOriginPos;
        public float cameraMoveSpeed = 0.15f;                    //相机移动速度
        public float m_MoveXOffset = 20f;                       //X轴偏移
        public BeActor m_Owner;
        protected Vector3 m_CenterOriginalPos = Vector3.zero;   //狙击镜中心点初始坐标

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/SkillSniperFrame";
        }

        protected override void _OnOpenFrame()
        {
            InitData();
            targetPos = mTarget.localPosition;
            cameraOriginPos = BattleMain.instance.Main.currentGeScene.GetCamera().GetController().transform.localPosition;
        }

        public void InitCenterPos()
        {
            m_CenterOriginalPos = GetCenterScenePos(Vector3.zero);
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            base._OnUpdate(timeElapsed);
        }

        protected override void _OnCloseFrame()
        {
            InitData();
        }

        protected void InitData()
        {
            mCdDi.gameObject.CustomActive(true);
            mCdSlider.gameObject.CustomActive(true);
            mZidanList.Clear();
            if (mMaskMaterial != null)
            {
                mMaskMaterial.material.SetTextureOffset("_Mask", Vector2.zero);
                mMaskMaterial.material.SetColor("_Alpha", new Color(1.0f, 1.0f, 1.0f, 0));
            }

            if (mImageDiCanvas != null)
            {
                mImageDiCanvas.alpha = 1;
            }
        }

        //进行一次攻击
        public void Attack(int curNum)
        {
            PlayAttackEffect(curNum);
            SetZiDanUsed(curNum, true);
        }

        //枪口移动
        public void _OnJoyStickMove(Vector2 offset)
        {
            bool canMove = ((GetCenterMoveDis() > m_MoveXOffset) && !IsXMoveLeft(offset)) || ((GetCenterMoveDis() < -m_MoveXOffset) && IsXMoveLeft(offset));
            if (canMove)
                return;
            bool isInSceneLeftEdge = BattleMain.instance.Main.currentGeScene.GetCamera().GetController().IsInSceneLeftEdge();
            bool isInSceneRightEdge = BattleMain.instance.Main.currentGeScene.GetCamera().GetController().IsInSceneRightEdge();
            int moveOffsetX = isInSceneLeftEdge || isInSceneRightEdge ? 1920 : 400;

            if (offset.x != 0)
            {
                if (!isInSceneLeftEdge && !isInSceneRightEdge)                                          //在相机可以移动的区域内
                {
                    if (targetPos.x < -moveOffsetX / 2 || targetPos.x > moveOffsetX / 2)                //枪口移动旁边以后 开始移动相机
                    {
                        CameraMove(offset);
                    }
                    else if (targetPos.x == -moveOffsetX / 2)                                           //UI移动旁边 刚好到达左边移动相机的临界点
                    {
                        if (IsXMoveLeft(offset))
                        {
                            CameraMove(offset);                                                         //在临界点往左边移动
                        }
                        else
                        {
                            GunMove(offset.x, 0, moveOffsetX);                                          //在临界点往右边移动
                        }
                    }
                    else if (targetPos.x == moveOffsetX / 2)                                            //UI移动旁边 刚好到达右边移动相机的临界点
                    {
                        if (IsXMoveLeft(offset))
                        {
                            GunMove(offset.x, 0, moveOffsetX);                                          //在临界点往左边移动
                        }
                        else
                        {
                            CameraMove(offset);                                                         //在临界点往右边移动
                        }
                    }
                    else
                    {
                        GunMove(offset.x, 0, moveOffsetX);                                              //其他情况下都是移动枪口
                    }
                }
                else
                {
                    if (isInSceneLeftEdge)                                                               //在左边相机不可以移动的区域内
                    {
                        if (IsXMoveLeft(offset))
                        {
                            GunMove(offset.x, 0, moveOffsetX);
                        }
                        else
                        {
                            if (targetPos.x >= 0)                                                       //在左边相机不可以移动的区域 并且枪口处于屏幕右边时（包括中间）
                            {
                                CameraMove(offset);
                            }
                            else
                            {
                                GunMove(offset.x, 0, moveOffsetX);
                            }
                        }
                    }

                    if (isInSceneRightEdge)                                                             //在右边相机不可以移动的区域内
                    {
                        if (IsXMoveLeft(offset))
                        {
                            if (targetPos.x <= 0)                                                       //在左边相机不可以移动的区域 并且枪口处于屏幕左边时（包括中间）
                            {
                                CameraMove(offset);
                            }
                            else
                            {
                                GunMove(offset.x, 0, moveOffsetX);
                            }
                        }
                        else
                        {
                            GunMove(offset.x, 0, moveOffsetX);
                        }
                    }
                }
            }


            if (offset.y != 0)
            {
                GunMove(0, offset.y, moveOffsetX);
            }
            UpdateUIAlpha();
        }

        protected bool IsXMoveLeft(Vector2 offset)
        {
            return offset.x < 0;
        }

        //狙击枪口移动
        protected void GunMove(float xOffset, float yOffset, int moveOffsetX)
        {
            if (xOffset != 0)
            {
                //狙击口的移动
                targetPos.x += xOffset * targetMoveSpeed;
                targetPos.x = Mathf.Clamp(targetPos.x, -moveOffsetX / 2, moveOffsetX / 2);
            }

            if (yOffset != 0)
            {
                targetPos.y += yOffset * targetMoveSpeed;
                targetPos.y = Mathf.Clamp(targetPos.y, -1080 / 2, 1080 / 2);
            }

            mTarget.localPosition = targetPos;
            //mask遮罩的移动
            v.x = -targetPos.x / 1920;
            v.y = -targetPos.y / 1080;
            mMaskMaterial.material.SetTextureOffset("_Mask", v);
        }

        //相机移动
        protected void CameraMove(Vector2 offset)
        {
            //相机的移动
            Vector3 cameraNewPos = BattleMain.instance.Main.currentGeScene.GetCamera().GetController().transform.localPosition;
            cameraNewPos.x += offset.x * cameraMoveSpeed;
            BattleMain.instance.Main.currentGeScene.GetCamera().GetController().SetCameraPos(cameraNewPos);
        }

        //更新UI透明度
        protected void UpdateUIAlpha()
        {
            UpdateZidanAlpha();
            UpdateCDAlpha();
            UpdateSkillTimeAlpha();
        }

        //获取准星移动的偏移
        protected float GetCenterMoveDis()
        {
            return GetCenterScenePos(GetWorldCenterPoint()).x - m_CenterOriginalPos.x;
        }

        //获取准星对应的场景坐标
        public Vector3 GetCenterScenePos(Vector3 centerWorldPos)
        {
            Vector3 pos = Vector3.zero;
            var vec = ClientSystemManager.GetInstance().UICamera.WorldToScreenPoint(centerWorldPos);
            Ray ray = Camera.main.ScreenPointToRay(vec);
            var t = -ray.origin.y / ray.direction.y;
            return new Vector3(ray.origin.x + t * ray.direction.x, 0, ray.origin.z + t * ray.direction.z);
        }

        //返回瞄准镜中心点坐标
        public Vector2 GetCenterPoint()
        {
            return mTarget.localPosition;
        }

        public Vector3 GetWorldCenterPoint()
        {
            return mTarget.position;
        }

        public RectTransform GetTargetParent()
        {
            return mTarget.parent as RectTransform;
        }

        public GameObject gameObject
        {
            get
            {
                return frame;
            }
        }

        //刷新CD进度条
        public void RefreshProgress(float progress)
        {
            if (mCDSlider1 == null)
                return;
            float value = progress * 10;
            int num = Mathf.CeilToInt(value);
            float lastValue = (value - num) * 10;
            mCDSlider1.fillAmount = num >= 1 ? 1 : 0;
            mCDSlider2.fillAmount = num >= 2 ? 1 : 0;
            mCDSlider3.fillAmount = num >= 3 ? 1 : 0;
            mCDSlider4.fillAmount = num >= 4 ? 1 : 0;
            mCDSlider5.fillAmount = num >= 5 ? 1 : 0;
            mCDSlider6.fillAmount = num >= 6 ? 1 : 0;
            mCDSlider7.fillAmount = num >= 7 ? 1 : 0;
            mCDSlider8.fillAmount = num >= 8 ? 1 : 0;
            mCDSlider9.fillAmount = num >= 9 ? 1 : 0;
            mCDSlider10.fillAmount = num >= 10 ? 1 : 0;
            switch (num)
            {
                case 0:
                    mCDSlider1.fillAmount = lastValue;
                    break;
                case 1:
                    mCDSlider2.fillAmount = lastValue;
                    break;
                case 2:
                    mCDSlider3.fillAmount = lastValue;
                    break;
                case 3:
                    mCDSlider4.fillAmount = lastValue;
                    break;
                case 4:
                    mCDSlider5.fillAmount = lastValue;
                    break;
                case 5:
                    mCDSlider6.fillAmount = lastValue;
                    break;
                case 6:
                    mCDSlider7.fillAmount = lastValue;
                    break;
                case 7:
                    mCDSlider8.fillAmount = lastValue;
                    break;
                case 8:
                    mCDSlider9.fillAmount = lastValue;
                    break;
                case 9:
                    mCDSlider10.fillAmount = lastValue;
                    break;
            }
        }

        //隐藏攻击间隔CD条
        public void CloseProgress()
        {
            mCdDi.gameObject.CustomActive(false);
            mCdSlider.gameObject.CustomActive(false);
        }

        //播放攻击特效
        protected void PlayAttackEffect(int curNum)
        {
            PlayFireAni();
            SetZiDanUsed(curNum, true);
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

        //初始化子弹
        public void InitZiDan(int maxBulletNum)
        {
            if (mSkillTime != null)
            {
                mSkillTime.CustomActive(true);
            }
            if (mZidanCanvas != null)
            {
                mZidanCanvas.gameObject.CustomActive(true);
            }
            for (int i = 0; i < maxBulletNum; i++)
            {
                CreateZiDan(i);
                SetZiDanUsed(i, false);
            }
        }

        protected void CreateZiDan(int index)
        {
            var zidan = GameObject.Instantiate(mZidanTemp, mZidanTemp.transform.parent);
            zidan.name = index.ToString();
            zidan.CustomActive(true);
            mZidanList.Add(zidan);
        }

        //显示技能剩余时间
        public void ShowSkillTime(int time)
        {
            string t = (time / 1000.0f).ToString("0");
            if (mSkillTime != null)
                mSkillTime.text = t;
        }

        //设置子弹显示或隐藏 保留底
        protected void SetZiDanUsed(int index, bool used)
        {
            mZidanList[index].isOn = !used;
            if (mDakeAni != null)
            {
                mDakeAni.transform.position = mZidanList[index].transform.position;
                if (used)
                    mDakeAni.gameObject.CustomActive(true);
                mDakeAni.DORestart();
            }
        }

        protected void PlayFireAni()
        {
            AudioManager.instance.PlaySound(3868);
            AudioManager.instance.PlaySound(3870);
            if (mFireAni != null)
            {
                mFireAni.DORestartById("sheji");
            }

            if (mDakeAni != null)
            {
                mDakeAni.gameObject.CustomActive(true);
                mDakeAni.DORestartById("danke1");
                mDakeAni.DORestartById("danke2");
            }
        }

        public void PlayCloseAni()
        {
            if (mCloseAni != null)
            {
                mCloseAni.DORestartById("xiaoshi");
            }

            DOTween
                .To(() => { return 0.0f; },
                    (x) =>
                    {
                        if (mMaskMaterial != null)
                        {
                            mMaskMaterial.material.SetColor("_Alpha", new Color(1.0f, 1.0f, 1.0f, x));
                        }

                        if (mImageDiCanvas != null)
                        {
                            mImageDiCanvas.alpha = 1 - x;
                        }
                    },
                    1.0f,
                    0.16f)
                .SetEase(Ease.OutQuad);

            if (mSkillTime != null)
            {
                mSkillTime.CustomActive(false);
            }
            if (mZidanCanvas != null)
            {
                mZidanCanvas.gameObject.CustomActive(false);
            }
        }

        protected void UpdateZidanAlpha()
        {
            if (mZidanCanvas != null)
            {
                Vector2 centerLocalPos = GetCenterPoint();
                if (centerLocalPos.x > 320 && centerLocalPos.y < 460)
                {
                    if (mZidanCanvas.alpha == 1.0f)
                        mZidanCanvas.alpha = 0.4f;
                }
                else
                {
                    if (mZidanCanvas.alpha == 0.4f)
                        mZidanCanvas.alpha = 1.0f;
                }
            }
        }

        protected void UpdateCDAlpha()
        {
            if (mCDSliderCanvas != null)
            {
                Vector2 centerLocalPos = GetCenterPoint();
                if ((centerLocalPos.x > -660 || centerLocalPos.x < 660) && centerLocalPos.y < 0)
                {
                    if (mCDSliderCanvas.alpha == 1.0f)
                        mCDSliderCanvas.alpha = 0.4f;
                    if (mCdDiCanvas.alpha == 1.0f)
                        mCdDiCanvas.alpha = 0.4f;
                }
                else
                {
                    if (mCDSliderCanvas.alpha == 0.4f)
                        mCDSliderCanvas.alpha = 1.0f;
                    if (mCdDiCanvas.alpha == 0.4f)
                        mCdDiCanvas.alpha = 1.0f;
                }
            }
        }

        protected void UpdateSkillTimeAlpha()
        {
            if (mSkillTimeCanvas != null)
            {
                Vector2 centerLocalPos = GetCenterPoint();
                if (centerLocalPos.x > 140 && (centerLocalPos.y < 350 || centerLocalPos.y > -640))
                {
                    if (mSkillTimeCanvas.alpha == 1.0f)
                        mSkillTimeCanvas.alpha = 0.4f;
                }
                else
                {
                    if (mSkillTimeCanvas.alpha == 0.4f)
                        mSkillTimeCanvas.alpha = 1.0f;
                }
            }
        }
    }
}
