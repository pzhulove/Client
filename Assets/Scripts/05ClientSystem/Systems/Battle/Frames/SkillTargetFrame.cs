using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace GameClient
{
    public class SkillTargetFrame : ClientFrame
    {
        private RectTransform target;
        private Image bulluteProgress;
        private Slider timeProgress;
        private System.Action JoyStickMoveCallBack;
        private Vector3 cameraOriginPos;       
        private Vector3 targetPos;
        private Transform BullteContainer;
        private Image maskMaterial;
        //private Transform frag;
        private Transform danke;
        private DOTweenAnimation fireAnim;
        private List<GameObject> bulleteList = new List<GameObject>();
        private List<GameObject> bullteContainerlist = new List<GameObject>();
        protected override void _bindExUI()
        {
            target = mBind.GetCom<RectTransform>("Target");
            timeProgress = mBind.GetCom<Slider>("TimeProgress");
            bulluteProgress = mBind.GetCom<Image>("BulleteProgress");
            maskMaterial= mBind.GetCom<Image>("maskMaterial");
            BullteContainer = mBind.GetCom<Transform>("BullteContainer");
            //frag = mBind.GetCom<Transform>("Frag");
            danke = mBind.GetCom<Transform>("danke");
            fireAnim = mBind.GetCom<DOTweenAnimation>("Fire");
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/SkillTargetFrame";
        }

        protected override void _OnOpenFrame()
        {
            cameraOriginPos = BattleMain.instance.Main.currentGeScene.GetCamera().GetController().transform.localPosition;
            targetPos = target.localPosition;
            InitJoystick();
            maskMaterial.material.SetTextureOffset("_Mask", Vector2.zero);
            InitBullet();
        }

        private void InitBullet()
        {
            bulleteList.Clear();
            bullteContainerlist.Clear();
            for (int i = 0; i < BullteContainer.childCount; i++)
            {
                if (i <= 9)
                {
                    bullteContainerlist.Add(BullteContainer.GetChild(i).gameObject);
                }
                else
                {
                    bulleteList.Add(BullteContainer.GetChild(i).gameObject);
                }
            }
        }

        void InitJoystick()
        {
            InputManager.instance.SetJoyStickMoveEndCallback(_OnJoyStickStop);
            InputManager.instance.SetJoyStickMoveCallback(_OnJoyStickMove);
        }

        public GameObject gameObject
        {
            get
            {
                return frame;
            }
        }

        public Vector3 GetWorldCenterPoint()
        {
            return target.position;
        }

        public Vector2 GetCenterPoint()
        {
            if (target == null)
                return Vector2.zero;
            return target.localPosition;
        }
        Vector2 v = Vector2.zero;
        public float cameraMoveSpeed = 0.2f;
        public float targetMoveSpeed = 30;
        private void _OnJoyStickMove(Vector2 arg0)
        {
            ////狙击口的移动
            targetPos.x += arg0.x * targetMoveSpeed;
            targetPos.y += arg0.y * targetMoveSpeed;
            targetPos.x = Mathf.Clamp(targetPos.x, -1920 / 2, 1920 / 2);
            targetPos.y = Mathf.Clamp(targetPos.y, -1080 / 2, 1080 / 2);
            target.localPosition = targetPos;

            //mask遮罩的移动
            v.x = -targetPos.x / 2340.0f;
            v.y = -targetPos.y / 1440.0f;
            maskMaterial.material.SetTextureOffset("_Mask", v);
            //相机的移动
            cameraOriginPos.x += arg0.x* cameraMoveSpeed;
            BattleMain.instance.Main.currentGeScene.GetCamera().GetController().SetCameraPos(cameraOriginPos);        
        }

        public void SetTimeProgress(int curValue, int maxValue)
        {
            timeProgress.value = (float)curValue / maxValue;
        } 

        public void SetBulletProgress(int curValue, int maxValue)
        {
            var bulletCount = curValue % 10;
            if (bulletCount == 0)
                bulletCount = 10;

            if (curValue <= maxValue)
            {
                if (danke != null)
                {
                    var d = GameObject.Instantiate(danke, danke.parent, true);
                    d.position = bulleteList[bulletCount - 1].transform.position + Vector3.left * 20f;
                    d.gameObject.SetActive(true);
                    GameObject.Destroy(d.gameObject, 1f);
                }
            }
            int count = bullteContainerlist.Count;
            for (int i = 0; i < count; i++)
            {
                if (i < bulletCount)
                {
                    bulleteList[i].SetActive(false);
                }
                else
                {
                    bulleteList[i].SetActive(true);
                }
            }
        }

        public Vector3 GetBulletPosition(int curBullet)
        {
            var index = curBullet % 10;
            if (index == 0)
                index = 10;
            return bulleteList[index - 1].transform.position;
        }

        public void PlayFireAnimation()
        {
            if (fireAnim != null)
            {
                fireAnim.DORestart();
            }
        }

        public RectTransform GetTargetParent()
        {
            return target.parent as RectTransform;
        }

        void _OnJoyStickStop()
        {
            if (JoyStickMoveCallBack != null)
                JoyStickMoveCallBack();
        }

        public void SetJoyStickMoveCallBack(System.Action action)
        {
            this.JoyStickMoveCallBack = action;
        }

        public void RemoveJoyStickMoveCallBack()
        {
            this.JoyStickMoveCallBack = null;
        }

        protected override void _OnCloseFrame()
        {
            maskMaterial.material.SetTextureOffset("_Mask", Vector2.zero);
            InputManager.instance.ReleaseJoyStickMoveEndCallback(_OnJoyStickStop);
            InputManager.instance.ReleaseJoyStickMoveCallback(_OnJoyStickMove);
            RemoveJoyStickMoveCallBack();
        }
    }
}
