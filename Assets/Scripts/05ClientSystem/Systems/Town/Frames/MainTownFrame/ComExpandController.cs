using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

namespace GameClient
{
    class ComExpandController : MonoBehaviour
    {
        // 伸展收缩对子节点在prefab里的摆放顺序没有要求,伸缩距离取决于List里元素的先后顺序
        public List<RectTransform> ChildrenExpandList = null;
        public List<RectTransform> SecendChildrenExpandList = null;

        public Vector3 FirstExpandPos;
        public Vector3 SecendExpandPos;
        public Vector3 MiddelExpandPos;
        public float PosSpacing;

        public float FirstDotweenDuraton;
        public float DurationSpacing;

        public RectTransform BackImg = null;
        public GameObject ExpandRoot = null;
        public Image ParentIcon = null;
        public GameObject ParticalEffect = null;
        public string NormalStateIconPath = "";
        public string ExpandStateIconPath = "";

        public bool bExpanding = false;

        void OnDestroy()
        {          
            ChildrenExpandList = null;
            BackImg = null;
            ExpandRoot = null;
            ParentIcon = null;
            ParticalEffect = null;
            SecendChildrenExpandList = null;
        }

        // Use this for initialization
        void Start()
        {
            if(!CanGoOn())
            {
                return;
            }

            bExpanding = false;

            UpdateExpand(bExpanding);
        }

        /// <summary>
        /// 判断是否要两排显示
        /// </summary>
        /// <returns></returns>
        bool IsTwoLine()
        {
            int count = 0;
            for (int i = 0; i < SecendChildrenExpandList.Count; i++)
            {
                RectTransform child = SecendChildrenExpandList[i];

                if (!child.gameObject.activeSelf)
                {
                    continue;
                }

                count++;
            }

            if (count > 0)
            {
                return true;
            }

            return false;
        }

        public void UpdateExpand(bool bExpand)
        {
            if (!CanGoOn())
            {
                return;
            }

            bExpanding = bExpand;

            if (bExpanding)
            {
                ExpandRoot.SetActive(true);
            }

            if (IsTwoLine())
            {
                PlayeDoTween(ChildrenExpandList, FirstExpandPos);
                PlayeDoTween(SecendChildrenExpandList, SecendExpandPos);
            }
            else
            {
                PlayeDoTween(ChildrenExpandList, MiddelExpandPos);
            }

            //Vector3 BackImgEndPos = new Vector3();
            //Vector3 ParentIconPos = new Vector3();
            //float BackImgDuration = 0.0f;
            //Sprite spr = null;

            if (bExpanding)
            {
                //float BackImgWidth = BackImg.offsetMax.x - BackImg.offsetMin.x;
                //float BackImgLengthSpacing = PosSpacing;
                //float fOffset = 200.0f;

                //if(ShowNum > 0)
                //{
                //    BackImgLengthSpacing = (BackImgWidth - fOffset) / ChildrenExpandList.Count;
                //}
             
                //BackImgEndPos.x = BackImgWidth - BackImgLengthSpacing * ShowNum - fOffset;
                //ParentIconPos.z = 133.0f;

                ParticalEffect.SetActive(true);

                //spr = AssetLoader.instance.LoadRes(ExpandStateIconPath, typeof(Sprite)).obj as Sprite;
            }
            else
            {
                //BackImgEndPos.x = BackImg.offsetMax.x - BackImg.offsetMin.x;
                //ParentIconPos.z = 0.0f;

                ParticalEffect.SetActive(false);

                //spr = AssetLoader.instance.LoadRes(NormalStateIconPath, typeof(Sprite)).obj as Sprite;
            }

            //BackImgDuration = FirstDotweenDuraton + DurationSpacing * (ShowNum - 1);

            //DOTween.To(() => BackImg.localPosition, r => { BackImg.localPosition = r; }, BackImgEndPos, BackImgDuration).SetEase(Ease.Linear);
            //ParentIcon.gameObject.transform.DOLocalRotate(ParentIconPos, BackImgDuration);
            
            //             if (spr != null)
            //             {
            //                 ParentIcon.sprite = spr;
            //             }
        }

        void PlayeDoTween(List<RectTransform> ChildrenExpandList, Vector3 FirstExpandPos)
        {
            int ShowNum = 0;
            for (int i = 0; i < ChildrenExpandList.Count; i++)
            {
                RectTransform child = ChildrenExpandList[i];

                if (!child.gameObject.activeSelf)
                {
                    continue;
                }

                ShowNum++;

                Vector3 ChildEndPos = new Vector3();
                float ChildDuration = 0.0f;

                if (bExpanding)
                {
                    ChildEndPos.x = FirstExpandPos.x - PosSpacing * (ShowNum - 1);
                }
                else
                {
                    ChildEndPos.x = 0.0f;
                }

                ChildEndPos.y = FirstExpandPos.y;

                ChildDuration = FirstDotweenDuraton + DurationSpacing * (ShowNum - 1);

                Tweener doTweener = DOTween.To(() => child.localPosition, r => { child.localPosition = r; }, ChildEndPos, ChildDuration);
                doTweener.SetEase(Ease.Linear);
                doTweener.OnComplete(_OnDotweenComplete);
            }
        }

        void _OnDotweenComplete()
        {
            if (!bExpanding)
            {
                ExpandRoot.SetActive(false);
            }
        }

        bool CanGoOn()
        {
            if (ChildrenExpandList == null || BackImg == null || ExpandRoot == null || ParentIcon == null ||
                ParticalEffect == null || NormalStateIconPath == "" || ExpandStateIconPath == "")
            {
                return false;
            }

            return true;
        }

        void Update()
        {
        }
    }
}