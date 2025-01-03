using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public interface ILimitTimeNote
    {
        string NotePrefabPath { get; }//logo资源路径
        uint StartTime { get; }
        string RuleDesc { get; }//规则描述
        string LogoDesc { get; }//logo描述
        uint EndTime { get; }
        string LogoPath { get; }//logo资源路径
        string NoteBgPath { get; }//logo资源路径
    }

    public class ActivityNote : MonoBehaviour, IDisposable
    {
        [SerializeField] private RectTransform mLimitTimeNoteRoot = null;
        [SerializeField] private string mDefaultLogoPath = "UI/Image/Background/UI_Xianshihuodong_SloganBg_01";
        protected ComCommonBind mNoteBind = null;
        const string NotePrefabResPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/ActivityLimitTimeNote";

        private Text mTextTime;
        private Text mTextRule;
        private Text mTextReceiveTime;
        private Image mImageNoteBg;
        private Image mImageLogo;
        private Text mTextLogo;

        public void Init(ILimitTimeNote data, bool isShowLogoText = true, ComCommonBind extendBind = null)
        {
            if (mNoteBind == null)
            {
                if (extendBind != null)
                {
                    mNoteBind = extendBind;
                }
                else
                {
                    var prefabResPath = string.IsNullOrEmpty(data.NotePrefabPath) ? NotePrefabResPath : data.NotePrefabPath;
                    var note = CGameObjectPool.instance.GetGameObject(prefabResPath, enResourceType.UIPrefab, (uint)GameObjectPoolFlag.ReserveLast);
                    if (note != null)
                    {
                        mNoteBind = note.GetComponent<ComCommonBind>();
                        note.transform.SetParent(mLimitTimeNoteRoot, false);

                    }
                }
             
            }

            if (mNoteBind)
            {
                mTextTime = mNoteBind.GetCom<Text>("ActivityTime");
                mTextRule = mNoteBind.GetCom<Text>("ActivityRole");
                mTextReceiveTime = mNoteBind.GetCom<Text>("ReceiveTime");
                mImageNoteBg = mNoteBind.GetCom<Image>("BgImg");
                mTextLogo = mNoteBind.GetCom<Text>("TextIntroduce");
                mImageLogo = mNoteBind.GetCom<Image>("LogoImg");
                mTextTime.SafeSetText(string.Format("{0}~{1}", _TransTimeStampToStr(data.StartTime), _TransTimeStampToStr(data.EndTime)));
                mTextReceiveTime.SafeSetText(string.Format("{0}~{1}", _TransTimeStampToStr(data.StartTime), _TransTimeStampToStr(data.EndTime)));
                //mTextRule.SafeSetText(string.Format("{0}", data.RuleDesc));
                mTextRule.SafeSetText(data.RuleDesc.Replace('|', '\n'));

                if (!string.IsNullOrEmpty(data.LogoDesc))
                {
                    mTextLogo.SafeSetText(data.LogoDesc);
                }
                //TODO 临时修改,需要策划后期表里面配
                else
                {
                    mTextLogo.SafeSetText(TR.Value("activity_login_introduce"));
                }

                mTextLogo.CustomActive(isShowLogoText);

                if (mImageLogo != null)
                {
                    if (!string.IsNullOrEmpty(data.LogoPath))
                    {
                        ETCImageLoader.LoadSprite(ref mImageLogo, data.LogoPath);
                    }
                    else
                    {
                        ETCImageLoader.LoadSprite(ref mImageLogo, mDefaultLogoPath);
                    }
                    mImageLogo.SetNativeSize();
                }

                if (mImageNoteBg != null)
                {
                    if (!string.IsNullOrEmpty(data.NoteBgPath))
                    {
                        ETCImageLoader.LoadSprite(ref mImageNoteBg, data.NoteBgPath);
                    }
                }
            }
        }

        public void ShowLogoText(bool isShow)
        {
            mTextLogo.CustomActive(isShow);
        }

        public void SetActivityTimeStr(string timeStr)
        {
            mTextTime.SafeSetText(timeStr);
        }

        private string _TransTimeStampToStr(UInt32 timeStamp)
        {
            System.DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime dt = time.AddSeconds(timeStamp);// unix 总秒数
            
            return string.Format("{0}年{1}月{2}日{3:HH:mm}", dt.Year, dt.Month, dt.Day, dt);
            //return string.Format("{0}月{1}日{2:HH:mm}", dt.Month, dt.Day, dt);
        }

        public void Dispose()
        {
            if (mNoteBind != null)
            {
                CGameObjectPool.instance.RecycleGameObject(mNoteBind.gameObject);
                mNoteBind = null;
            }
        }
    }
}

