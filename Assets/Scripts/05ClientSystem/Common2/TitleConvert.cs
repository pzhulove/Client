using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace GameClient
{
    public class TitleConvert : MonoBehaviour
    {
        public int width;
        public int height;
        public LayoutElement element;

        public GameObject ani;
        public GeAnimFrameBillboard animator;
        public SpriteRenderer render;

        public Vector2 scale = Vector2.one;
        public bool bUseScale = false;
        public int iSortInLayer = 0;
        Canvas canvas;
        public int sortingLayerID = 0;

        bool bNeedUpdateScale = false;

        int iTitleID = 0;
        public int TitleID
        {
            get
            {
                return iTitleID;
            }
            set
            {
                if(iTitleID != value)
                {
                    iTitleID = value;
                    _OnUpdateResource();
                }
            }
        }

        public void Active(bool active)
        {
            if (null != animator)
            {
                animator.SetPause(!active);
                animator.SetVisible(active);
            }
            //element.ignoreLayout = !active;
        }

        public float GetAnimationTime(float fDefault = 5.0f)
        {
            if(ani != null && animator != null)
            {
                float length = animator.GetTimeLength();
                return length < fDefault ? fDefault : length;
            }
            return fDefault;
        }

        void _OnUpdateResource()
        {
            if (ani != null)
            {
                ani.transform.SetParent(null);
                CGameObjectPool.instance.RecycleGameObject(ani.gameObject);
                ani = null;
                animator = null;
                render = null;
            }
            var itemData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iTitleID);
            if(itemData != null)
            {
                ani = CGameObjectPool.instance.GetGameObject(itemData.ModelPath, enResourceType.UIPrefab, (uint)GameObjectPoolFlag.ReserveLast);
                if(ani != null)
                {
                    animator = ani.GetComponent<GeAnimFrameBillboard>();
                    animator.Init();
                    render = ani.GetComponent<SpriteRenderer>();
                    Utility.AttachTo(ani, gameObject);
                    gameObject.SetActive(true);
                    Active(false);
                    if (render != null)
                    {
                        render.sortingOrder = iSortInLayer;
                    }
                }
                if(ani == null || animator == null || render == null)
                {
                    Logger.LogErrorFormat("create title failed with title id = {0}", iTitleID);
                }
            }

            if (element != null && ani != null)
            {
                ani.layer = sortingLayerID;

                element.ignoreLayout = false;
                if(!bUseScale)
                {
                    element.preferredWidth = ani.transform.localScale.x / 100.0f * width;
                    element.preferredHeight = ani.transform.localScale.y / 100.0f * height;
                }
                else
                {
                    ani.transform.localScale = new Vector3(ani.transform.localScale.x * scale.x, ani.transform.localScale.y * scale.y, 1.0f);
                    element.preferredWidth = ani.transform.localScale.x / 100.0f * width;
                    element.preferredHeight = ani.transform.localScale.y / 100.0f * height;
                }
            }
        }

        public void OnRecycle()
        {
            if (ani != null)
            {
                ani.transform.SetParent(null);
                CGameObjectPool.instance.RecycleGameObject(ani.gameObject);
                ani = null;
                animator = null;
                render = null;
            }

            iTitleID = 0;
        }
    }
}