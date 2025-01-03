using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ProtoTable;

namespace GameClient
{
    public struct UnlockData
    {
        public int FuncUnlockID;
        public Vector3 pos;

        public void ClearData()
        {
            FuncUnlockID = 0;
            pos = new Vector3(0,0,0);
        }
    }

    class FunctionUnlockFrame : ClientFrame
    {
        public delegate void PlayEnd();
        public PlayEnd ResPlayEnd;

        UnlockData data = new UnlockData();
        float interval = 0f;

        protected override void _OnOpenFrame()
        {
            data = (UnlockData)userData;
            InitInterface();
        }

        protected override void _OnCloseFrame()
        {
            ClearData();
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/FunctionUnlock/FunctionUnlockFrame";
        }

        void ClearData()
        {
            data.ClearData();
            interval = 0f;

            if (ResPlayEnd != null)
            {
                ResPlayEnd = null;
            }     
        }

        void InitInterface()
        {
            FunctionUnLock FunctionUnLockData = TableManager.GetInstance().GetTableItem<FunctionUnLock>(data.FuncUnlockID);
            if(FunctionUnLockData == null)
            {
                return;
            }

            if(FunctionUnLockData.IconPath != "" && FunctionUnLockData.IconPath != "-")
            {
                //Sprite Icon = AssetLoader.instance.LoadRes(FunctionUnLockData.IconPath, typeof(Sprite)).obj as Sprite;

                //if(Icon != null)
                //{
                //    icon.sprite = Icon;
                //}
                ETCImageLoader.LoadSprite(ref icon, FunctionUnLockData.IconPath);

                name.text = FunctionUnLockData.Name;
            }

            //             RectTransform rect = pos.GetComponent<RectTransform>();
            // 
            //             DOTween.To(() => rect.position, r => 
            //             {
            //                rect.position = r;
            //             }, data.pos, 2.4f).SetEase(Ease.OutQuart).OnComplete(DoTweenPlayEnd        
        }

        void DoTweenPlayEnd()
        {
            if (ResPlayEnd != null)
            {
                ResPlayEnd();
            }   
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            interval += timeElapsed;

            if(interval < 1f)
            {
                return;
            }

            RectTransform rect = pos.GetComponent<RectTransform>();

            DOTween.To(() => rect.position, r =>
            {
                rect.position = r;
            }, data.pos, 1.8f).SetEase(Ease.OutQuart).OnComplete(DoTweenPlayEnd);

            interval = 0f;
        }

        [UIObject("pos")]
        protected GameObject pos;

        [UIControl("pos/icon")]
        protected Image icon;

        [UIControl("pos/name")]
        protected Text name;
    }
}
