using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AdventureTeamCharacterCollectionItem : MonoBehaviour
    {
        private const string EFFUI_NEW_ACTIVATED_ROLE_UNLOCK_RES_PATH = "Effects/UI/Prefab/EffUI_yongbingtuan/Prefab/EffUI_yongbingtuan_juesejiesuo";

        [SerializeField] private ComCommonBind bind;
        [Space(10)]
        [HeaderAttribute("解锁动画时长")]
        [SerializeField] private float totalUnlockAnimDuration = 3f;
        //[HeaderAttribute("解锁动画Slider Value Min")]
        //[SerializeField] private float unlockAnimMinValue = -3f;
        //[HeaderAttribute("解锁动画Slider Value Main")]
        //[SerializeField] private float unlockAnimMaxValue = 4f;
        //[Space(5)]
        //[HeaderAttribute("解锁特效后的等待其他动画时长")]
        //[SerializeField] private float activeEffectDuration = 3f;


        GameObject hasjobRoot = null;
        GameObject nojobRoot = null;
        Image jobPhoto = null;
        Image jobName = null;
        UIGray jobinfoGray = null;
        GameObject effectRoot = null;

        private UnityEngine.Coroutine waitToPlayUnlockAnim;

        private CharacterCollectionModel tempModel = null;

        private GameObject unlockEffectGo = null;
        private MeshRenderer unlockEffectRed = null;
        private Material unlockEffectMat = null;

        public delegate void WaitActiveEffectPlayEndHandler(CharacterCollectionModel model);
        [HideInInspector]
        public WaitActiveEffectPlayEndHandler onWaitActiveEffectPlayEnd;

        void Awake()
        {
            if (bind == null)
            {
                Logger.LogError("[AdventureTeamCharacterCollectionItem] - comBind is null");
                return;
            }
            hasjobRoot = bind.GetGameObject("hasjobRoot");
            nojobRoot = bind.GetGameObject("nojobRoot");
            jobPhoto = bind.GetCom<Image>("jobPhoto");
            jobName = bind.GetCom<Image>("jobName");
            jobinfoGray = bind.GetCom<UIGray>("jobinfoGray");
            effectRoot = bind.GetGameObject("effectRoot");
        }

        void OnDestroy()
        {
            hasjobRoot = null;
            nojobRoot = null;
            jobPhoto = null;
            jobName = null;
            jobinfoGray = null;
            effectRoot = null;

            unlockEffectGo = null;
            unlockEffectRed = null;
            unlockEffectMat = null;

            Clear();
        }

        public void Clear()
        {
            if (onWaitActiveEffectPlayEnd != null)
            {
                var playEndDelegates = onWaitActiveEffectPlayEnd.GetInvocationList();
                if (playEndDelegates != null)
                {
                    for (int i = 0; i < playEndDelegates.Length; i++)
                    {
                        onWaitActiveEffectPlayEnd -= playEndDelegates[i] as WaitActiveEffectPlayEndHandler;
                    }
                }
            }
            onWaitActiveEffectPlayEnd = null;

            if (waitToPlayUnlockAnim != null)
            {
                StopCoroutine(waitToPlayUnlockAnim);
                waitToPlayUnlockAnim = null;
            }

            this.tempModel = null;

            //解锁特效默认置为false
            if (unlockEffectGo != null)
            {
                unlockEffectGo.CustomActive(false);
            }
        }

        public void InitCollectionItem(CharacterCollectionModel collectionModel)
        {
            this.tempModel = collectionModel;

            if (collectionModel == null)
            {
                return;
            }

            bool bJobOpen = collectionModel.isJobOpened;
            //不能这么判断
            //bool bJobActived = collectionModel.activateStatus == CharacterCollectionActivateStatus.Activated ? true : false;
            bool bJobOwn = collectionModel.isOwned;    
            bool bOwnJobAnimNeedPlay = collectionModel.needPlay;
            string photoPath = collectionModel.jobPhotoPath;
            string namePath = collectionModel.jobNamePath;

            hasjobRoot.CustomActive(bJobOpen);
            nojobRoot.CustomActive(!bJobOpen);
            //初始化时 默认特效关闭
            //effectRoot.CustomActive(false);

            if (bJobOpen == false)
            {
                return;
            }

            bool isJobPhotoLoadSucc = false;
            bool isJobNameLoadSucc = false;

            if (!string.IsNullOrEmpty(photoPath))
            {
                isJobPhotoLoadSucc = ETCImageLoader.LoadSprite(ref jobPhoto, photoPath);
            }

            if (!string.IsNullOrEmpty(namePath))
            {
                isJobNameLoadSucc = ETCImageLoader.LoadSprite(ref jobName, namePath);
            }

            if (!isJobNameLoadSucc || !isJobPhotoLoadSucc)
            {
                Logger.LogProcessFormat("[AdventureTeamCharacterCollectionItem] - JobTable Transfer Job id:{0}, has no right photo resPath or name resPath", collectionModel.jobTableId);
                hasjobRoot.CustomActive(false);
                nojobRoot.CustomActive(true);
                return;
            }

            _SetRoleActivatedStatus(bJobOwn && !bOwnJobAnimNeedPlay);
        }

        private void _SetRoleActivatedStatus(bool bActivated)
        {
            if (jobinfoGray)
            {
                jobinfoGray.SetEnable(!bActivated);
                //jobinfoGray.enabled = !bActivated;
            }
        }

        public void PlayNewJobActivate()
        {
            if(!AdventureTeamDataManager.GetInstance().CheckIsSelectJobSatisfyConditions(this.tempModel))
            {
                return;
            }
            if (this.tempModel.needPlay == false)
            {
                return;
            }

            //effectRoot.CustomActive(true);
            _ShowUnlockEffect(true);
        }

        private void _ShowUnlockEffect(bool bShow)
        {
            if (bShow)
            {
                _CreateEffectObj();
            }
            if (unlockEffectGo != null)
            {
                //_ControlUnlockValue(unlockAnimMinValue);
                unlockEffectGo.CustomActive(bShow);
            }

            if (bShow)
            {
                //播放解锁动画 先激活图片
                _SetRoleActivatedStatus(true);

                if (waitToPlayUnlockAnim != null)
                {
                    StopCoroutine(waitToPlayUnlockAnim);
                }
                waitToPlayUnlockAnim = StartCoroutine(_WaitToPlayUnlockAnim());
            }
        }

        private void _CreateEffectObj()
        {
            if (unlockEffectGo == null)
            {
                unlockEffectGo = _LoadEffectByResPath(EFFUI_NEW_ACTIVATED_ROLE_UNLOCK_RES_PATH);
                Utility.AttachTo(unlockEffectGo, effectRoot);
            }
            if (unlockEffectGo == null)
            {
                return;
            }
            unlockEffectGo.transform.localScale = new Vector3(2, 2, 2);
            var bind = unlockEffectGo.GetComponent<AdventureTeamCharacterCollectionUnlockEffectBind>();
            if (bind == null)
            {
                return;
            }
            unlockEffectRed = bind.renderer;
            if (unlockEffectRed != null)
            {
                unlockEffectMat = unlockEffectRed.material;
            }
            if (this.tempModel != null)
            {
                string[] resPaths = this.tempModel.jobPhotoPath.Split(':');
                if (resPaths != null && resPaths.Length >= 2)
                {
                    //Logger.LogError(resPaths[0]);  //贴图资源路径不需要写全 （不需要带:）
                    Texture jobTex = AssetLoader.instance.LoadRes(resPaths[0], typeof(Texture)).obj as Texture;
                    _SetUnlockMatTexture(jobTex);
                }
            }
            //_ControlUnlockValue(unlockAnimMinValue);
        }
        IEnumerator _WaitToPlayUnlockAnim()
        {
            //float delta = unlockAnimMaxValue - unlockAnimMinValue;

            //delta = delta > 0f ? delta : -delta;

            //if (delta == 0)
            //{
            //    yield break;
            //}

            //float duration = unlockAnimDuration / delta;
            //float value = unlockAnimMinValue;

            //while (delta >= 0)
            //{
            //    if (unlockAnimDuration == 0f)
            //    {
            //        value = unlockAnimMaxValue;
            //        _ControlUnlockValue(value);
            //        yield break;
            //    }
            //    value += 1 / duration;
            //    _ControlUnlockValue(value);
            //    delta = unlockAnimMaxValue - value;
            //    yield return Yielders.GetWaitForSeconds(duration);
            //}

            //yield return Yielders.GetWaitForSeconds(activeEffectDuration);

            yield return Yielders.GetWaitForSeconds(totalUnlockAnimDuration);

            if (onWaitActiveEffectPlayEnd != null)
            {
                onWaitActiveEffectPlayEnd(tempModel);
            }
        }

        /*
        private void _ControlUnlockValue(float value)
        {
            if (value < unlockAnimMinValue)
            {
                value = unlockAnimMinValue;
            }
            else if (value > unlockAnimMaxValue)
            {
                value = unlockAnimMaxValue;
            }

            if (unlockEffectMat != null)
            {
                unlockEffectMat.SetFloat("_MeltProgress", value);
            }
        }
         * */

        private void _SetUnlockMatTexture(Texture texture)
        {
            if (texture == null)
            {
                return;
            }
            if (unlockEffectMat != null)
            {
                unlockEffectMat.SetTexture("_MainTex", texture);
            }
        }

        private GameObject _LoadEffectByResPath(string effectPath)
        {
            GameObject effectGo = null;
            if (string.IsNullOrEmpty(effectPath))
            {
                return effectGo;
            }
            effectGo = AssetLoader.GetInstance().LoadResAsGameObject(effectPath);
            return effectGo;
        }
    }
}