using DataModel;
using Protocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    namespace ActivityTreasureLottery
    {
        /// <summary>
        /// 夺宝活动城镇界面提示Frame
        /// </summary>
        public sealed class ActivityTreasureLotteryTipFrame : ClientFrame
        {
            GameObject mEffetKaiShi;
            GameObject mEffetKaiShiBao;
            const float DelayTime = 0.3f;
            public override string GetPrefabPath()
            {
                return "UIFlatten/Prefabs/ActivityTreasureLottery/ActivityTreasureLotteryTipFrame";
            }

            protected override void _OnOpenFrame()
            {
                base._OnOpenFrame();
                if (mEffetKaiShi == null)
                {
                    string path = mBind.GetPrefabPath("EffectKaiShi");
                    if (path != null)
                    {
                        mEffetKaiShi = AssetLoader.instance.LoadResAsGameObject(path);
                    }

                    if (mEffetKaiShi != null)
                    {
                        Utility.AttachTo(mEffetKaiShi, frame);
                    }
                }
                InvokeMethod.Invoke(DelayTime, InitEffectBao);
                bool isDrawFrameOpen = ClientSystemManager.GetInstance().IsFrameOpen<ActivityTreasureLotteryDrawFrame>();
                bool isActivityFrameOpen = ClientSystemManager.GetInstance().IsFrameOpen<ActivityTreasureLotteryFrame>();
                //如果活动界面是打开的，则覆盖在活动界面上面
                if (isActivityFrameOpen)
                {
                    SetSiblingIndex(ClientSystemManager.GetInstance().GetFrame(typeof(ActivityTreasureLotteryFrame).Name).GetSiblingIndex() + 1);
                }

                //如果开奖界面打开的，则被开奖界面覆盖
                if (isDrawFrameOpen)
                {
                    //frame.transform.GetSiblingIndex()
                    SetSiblingIndex(ClientSystemManager.GetInstance().GetFrame(typeof(ActivityTreasureLotteryDrawFrame).Name).GetSiblingIndex() - 1);
                }

                //如果是两者都没打开，和其他UI的关系由预制体上设置。
                if (!isActivityFrameOpen && !isDrawFrameOpen)
                {
                    SetSiblingIndex(mComClienFrame.GetZOrder());
                }
            }

            void InitEffectBao()
            {
                if (mBind == null)      //延迟调用该函数，在调用时该对象存在已被销毁的可能，所以要加判空
                {
                    return;
                }

                if (mEffetKaiShiBao == null)
                {
                    string path = mBind.GetPrefabPath("EffectKaiShiBao");
                    if (path != null)
                    {
                        mEffetKaiShiBao = AssetLoader.instance.LoadResAsGameObject(path);
                        if (mEffetKaiShiBao != null)
                        {
                            Utility.AttachTo(mEffetKaiShiBao, frame);
                        }
                    }
                }
                else
                {
                    mEffetKaiShiBao.CustomActive(true);
                }
            }

            protected override void _OnCloseFrame()
            {
                base._OnCloseFrame();
                mEffetKaiShiBao.CustomActive(false);
            }

            #region ExtraUIBind
            private Button mButtonClose = null;
            private Button mButtonCheck = null;

            protected override void _bindExUI()
            {
                mButtonClose = mBind.GetCom<Button>("ButtonClose");
                mButtonClose.SafeAddOnClickListener(_onButtonCloseButtonClick);
                mButtonCheck = mBind.GetCom<Button>("ButtonCheck");
                mButtonCheck.SafeAddOnClickListener(_onButtonCheckButtonClick);
            }

            protected override void _unbindExUI()
            {
                mButtonClose.SafeRemoveOnClickListener(_onButtonCloseButtonClick);
                mButtonClose = null;
                mButtonCheck.SafeRemoveOnClickListener(_onButtonCheckButtonClick);
                mButtonCheck = null;
            }
            #endregion

            #region Callback
            private void _onButtonCloseButtonClick()
            {
                ActivityTreasureLotteryDataManager.GetInstance().DequeueDrawLottery();
                if (ActivityTreasureLotteryDataManager.GetInstance().GetDrawLotteryCount() > 0)
                {
                    ReOpen();
                }
                else
                {
                    Close();
                }
            }
            private void _onButtonCheckButtonClick()
            {
                if (ActivityTreasureLotteryDataManager.GetInstance().GetDrawLotteryCount() > 1)
                {
                    ReOpen();
                }
                else
                {
                    Close();
                }
                ClientSystemManager.GetInstance().OpenFrame<ActivityTreasureLotteryDrawFrame>();
            }

            void ReOpen()
            {
                if(mEffetKaiShiBao != null)
                {
                    mEffetKaiShiBao.CustomActive(false);
                    mEffetKaiShiBao.CustomActive(true);
                }
            }
            #endregion
        }
    }
}