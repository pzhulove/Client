using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Network;
using Protocol;
using UnityEngine.EventSystems;

namespace GameClient
{
    public enum ChijiMapState
    {
        Mini_Map = 0,
        Full_Map,     
    }

    public class ChijiMapFrame : ClientFrame
    {
        private ChijiMapState mapState = ChijiMapState.Mini_Map;
        float fLineTimeIntrval = 0.0f;
        Vector2 centerpos = Vector2.zero;
        float WhiteRadius = 0.0f;

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chiji/ChijiMapFrame";
        }
        protected sealed override void _OnOpenFrame()
        {
            _BindUIEvent();

            //ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            //if (systemTown == null)
            //{
            //    systemTown = ClientSystemManager.GetInstance().TargetSystem as ClientSystemTown;
            //    if (systemTown == null)
            //    {
            //        Logger.LogError("ChijiMapFrame must open in Chiji Scene!!");
            //        return;
            //    }
            //}
            ClientSystemGameBattle systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
            if (systemTown == null)
            {
                Logger.LogError("ChijiMapFrame must open in Chiji Scene!!");
                return;
            }

            if (userData != null)
            {
                mapState = (ChijiMapState)userData;
            }

            if (mTouchMoveCamera2D != null)
            {
                mTouchMoveCamera2D.enabled = mapState == ChijiMapState.Full_Map;
            }

            if (mMapScene != null)
            {
                mMapScene.Initialize();
                mMapScene.btnScene.onMouseClick.RemoveAllListeners();
                mMapScene.btnScene.onMouseClick.AddListener(_OnClickMapPos);
            }

            if (mPlayer_main != null)
            {
                mPlayer_main.Initialize();

                if (mMapScene != null)
                {
                    if (systemTown.MainPlayer != null)
                    {
                        mPlayer_main.Setup(systemTown.MainPlayer, mMapScene);
                    }
                    else
                    {
                        SystemNotifyManager.SysNotifyMsgBoxOK("吃鸡小地图[_OnOpenFrame],systemTown.MainPlayer == null");
                    }
                }
                else
                {
                    SystemNotifyManager.SysNotifyMsgBoxOK("吃鸡小地图[_OnOpenFrame],mMapScene == null");
                }
            }
            else
            {
                SystemNotifyManager.SysNotifyMsgBoxOK("吃鸡小地图[_OnOpenFrame],mPlayer_main == null");
            }

            BeTownPlayerMain.OnAutoMoveSuccess.AddListener(_OnAutoMoveEnd);
            BeTownPlayerMain.OnAutoMoveFail.AddListener(_OnAutoMoveEnd);

            if (mTouchMoveCamera2D != null)
            {
                if(mPlayer_main != null)
                {
                    mTouchMoveCamera2D.PlayerTransform = mPlayer_main.gameObject.transform;
                }
                else
                {
                    SystemNotifyManager.SysNotifyMsgBoxOK("吃鸡小地图[_OnOpenFrame]，mPlayer_main == null，mTouchMoveCamera2D.PlayerTransform没有被赋值");
                } 
            }
            else
            {
                SystemNotifyManager.SysNotifyMsgBoxOK("吃鸡小地图[_OnOpenFrame],mTouchMoveCamera2D == null");
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            _UnBindUIEvent();

            BeTownPlayerMain.OnAutoMoveSuccess.RemoveListener(_OnAutoMoveEnd);
            BeTownPlayerMain.OnAutoMoveFail.RemoveListener(_OnAutoMoveEnd);

            _ClearData();
        }

        private void _ClearData()
        {
            mapState = ChijiMapState.Mini_Map;
            fLineTimeIntrval = 0.0f;
            centerpos = Vector2.zero;
            WhiteRadius = 0.0f;
        }

        void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.JobIDReset, _OnUpdateComMapPlayerJobID);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PoisonNextStage, _OnPoisonNextStage);
        }

        void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.JobIDReset, _OnUpdateComMapPlayerJobID);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PoisonNextStage, _OnPoisonNextStage);
        }

        void _OnPoisonNextStage(UIEvent iEvent)
        {
            SetWhiteCircle(ChijiDataManager.GetInstance().PoisonRing.nextStageCenter, ChijiDataManager.GetInstance().PoisonRing.nextStageRadius);
        }

        void _OnUpdateComMapPlayerJobID(UIEvent iEvent)
        {
            if (mapState == ChijiMapState.Mini_Map)
            {
                ClientSystemGameBattle systemChiji = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
                if (systemChiji != null)
                {
                    if (mPlayer_main != null && mMapScene != null)
                    {
                        if (systemChiji.MainPlayer != null)
                        {
                            mPlayer_main.Setup(systemChiji.MainPlayer, mMapScene);
                        }
                        else
                        {
                            SystemNotifyManager.SysNotifyMsgBoxOK("吃鸡小地图[_OnUpdateComMapPlayerJobID],systemChiji.MainPlayer == null,角色尚未创建");
                        }
                    }
                    else
                    {
                        SystemNotifyManager.SysNotifyMsgBoxOK("吃鸡小地图[_OnUpdateComMapPlayerJobID],mPlayer_main == null 或者 mMapScene == null");
                    }
                }
                else
                {
                    SystemNotifyManager.SysNotifyMsgBoxOK("吃鸡小地图[_OnUpdateComMapPlayerJobID],ClientSystemGameBattle == null");
                }
            }
        }

        void _OnAutoMoveEnd()
        {
            mTargetPos.SetActive(false);
        }

        private void _OnClickMapPos(PointerEventData pointEventData)
        {
            //   ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            ClientSystemGameBattle systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
            if (systemTown == null)
            {
                return;
            }

            if (systemTown.MainPlayer == null)
            {
                return;
            }

            Vector2 pos = Vector2.zero;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(mMapScene.GetComponent<RectTransform>(), pointEventData.pressPosition, pointEventData.enterEventCamera, out pos);

            Vector3 vecPos = new Vector3(pos.x / mMapScene.XRate, 0.0f, pos.y / mMapScene.ZRate);
            Vector3 vecTargetPos = vecPos + mMapScene.offset;
            systemTown.MainPlayer.CommandMoveToScene(mMapScene.SceneID, vecTargetPos);

            if (mTargetPos != null)
            {
                mTargetPos.transform.SetParent(mMapScene.transform, false);
                mTargetPos.transform.localPosition = new Vector3(pos.x, pos.y, 0);
                mTargetPos.SetActive(true);
            }
        }

        public void SetScale(Vector2 scale)
        {
            frame.GetComponent<RectTransform>().localScale = new Vector3(scale.x, scale.y, 1.0f);
        }

        public Vector2 GetSize()
        {
            Vector2 size = frame.GetComponent<RectTransform>().rect.size;
            Vector3 scale = frame.GetComponent<RectTransform>().localScale;

            return new Vector2(size.x * scale.x, size.y * scale.y);
        }

        public Vector2 GetPlayerMainPos()
        {
            if (mPlayer_main == null)
            {
                return Vector2.zero;
            }
            else
            {
                Vector3 pos = mPlayer_main.transform.localPosition + mPlayer_main.transform.parent.localPosition;
                Vector3 scale = frame.GetComponent<RectTransform>().localScale;
                return new Vector2(pos.x * scale.x, pos.y * scale.y);
            }
        }
        public void SetBlueCircle(Vector2 center,float radius,float durTime,float totalTime)
        {
            if(mBlueCircle != null)
            {
                mBlueCircle.Setup(center,radius,durTime,totalTime,this.mMapScene);
            }
        }
        public void SetWhiteCircle(Vector2 center,float radius)
        {
            centerpos = center;
            WhiteRadius = radius;

            if (mWhiteCircle != null)
            {
                mWhiteCircle.Setup(center,radius, mMapScene);
            }
        }
        public void ResetSourceCircle(Vector2 srcPos,float srcRadius)
        {
            if (mBlueCircle != null)
            {
                mBlueCircle.ResetSource(srcRadius,srcPos);
            }
        }

        private void _UpdateDashesLine()
        {
            if(mPlayer_main == null || WhiteRadius < 1.0f)
            {
                mLine.CustomActive(false);
                return;
            }

            float distance = Mathf.Sqrt(Mathf.Pow(mPlayer_main.ServerPos.x - centerpos.x, 2) + Mathf.Pow(mPlayer_main.ServerPos.y - centerpos.y, 2));

            if (distance <= WhiteRadius)
            {
                mLine.CustomActive(false);
                return;
            }

            Vector3 PlayerLocalPos = mPlayerMain.rectTransform.anchoredPosition + mPlayerMain.rectTransform.parent.GetComponent<RectTransform>().anchoredPosition;
            Vector3 WhiteCircleCenterLocalPos = mWhiteCircleCenter.rectTransform.anchoredPosition;

            Vector2 newLength = mLine.rectTransform.sizeDelta;
            newLength.x = Mathf.Sqrt(Mathf.Pow(WhiteCircleCenterLocalPos.x - PlayerLocalPos.x, 2) + Mathf.Pow(WhiteCircleCenterLocalPos.y - PlayerLocalPos.y, 2));
            mLine.rectTransform.sizeDelta = newLength;

            Vector3 newLocalPos = mLine.rectTransform.anchoredPosition;
            newLocalPos.x = (WhiteCircleCenterLocalPos.x + PlayerLocalPos.x) / 2.0f;
            newLocalPos.y = (WhiteCircleCenterLocalPos.y + PlayerLocalPos.y) / 2.0f;
            mLine.rectTransform.anchoredPosition = newLocalPos;

            float rad = Mathf.Acos((PlayerLocalPos.x - WhiteCircleCenterLocalPos.x) / newLength.x);
            Quaternion newRotation = Quaternion.Euler(0.0f, 0.0f, rad / Mathf.PI * 180);

            if (PlayerLocalPos.y < WhiteCircleCenterLocalPos.y)
            {
                newRotation = Quaternion.Euler(0.0f, 0.0f, rad / -Mathf.PI * 180);
            }

            mLine.rectTransform.localRotation = newRotation;
            mLine.CustomActive(true);
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            fLineTimeIntrval += timeElapsed;

            if(fLineTimeIntrval >= 0.2f)
            {
                fLineTimeIntrval = 0.0f;
                _UpdateDashesLine();
            }
        }

        #region ExtraUIBind
        private TouchMoveCamera2D mTouchMoveCamera2D = null;
        private ComMapPlayer mPlayer_main = null;
        private ComBlueCircle mBlueCircle = null;
        private ComWhiteCircle mWhiteCircle = null;
        private GameObject mTargetPos = null;
        private ComMapScene mMapScene = null;
        private Image mLine = null;
        private Image mWhiteCircleCenter = null;
        private Image mPlayerMain = null;

        protected override void _bindExUI()
        {
            mTouchMoveCamera2D = mBind.GetCom<TouchMoveCamera2D>("TouchMoveCamera2D");
            mPlayer_main = mBind.GetCom<ComMapPlayer>("player_main");
            mBlueCircle  = mBind.GetCom<ComBlueCircle>("blueCircle");
            mWhiteCircle = mBind.GetCom<ComWhiteCircle>("whiteCircle");
            mTargetPos = mBind.GetGameObject("targetPos");
            mMapScene = mBind.GetCom<ComMapScene>("mapScene");
            mLine = mBind.GetCom<Image>("line");
            mWhiteCircleCenter = mBind.GetCom<Image>("whiteCircleCenter");
            mPlayerMain = mBind.GetCom<Image>("playerMain");
        }

        protected override void _unbindExUI()
        {
            mTouchMoveCamera2D = null;
            mPlayer_main = null;
            mTargetPos = null;
            mMapScene = null;
            mBlueCircle = null;
            mWhiteCircle = null;
            mLine = null;
            mWhiteCircleCenter = null;
            mPlayerMain = null;
        }
        #endregion
    }
}
