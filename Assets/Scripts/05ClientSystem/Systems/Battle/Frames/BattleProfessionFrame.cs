using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

namespace GameClient
{
    public class BattleProfessionFrame : ClientFrame
    {
        #region ExtraUIBind
        private ComCommonBind mBulletNum = null;
        private ComCommonBind mSkillUseCount = null;
        private GameObject mBuffNum = null;
        private ComCommonBind mShengQiShiBieDongBuff = null;
        private Text mBuffNumText = null;
        private ComCommonBind mNvDaQiangEnergyBar = null;

        private ComCommonBind mComboBuffCom = null;
        
        
        private GameObject mComboBuffNum = null;
        private UIGray mComboBuffNumGray = null;
        private Text mComboBuffNumText = null;
        private Image mComboBuffProgress = null;
        private int mCurComboNum = -1;

        protected override void _bindExUI()
        {
            mBulletNum = mBind.GetCom<ComCommonBind>("BulletNum");
            mSkillUseCount = mBind.GetCom<ComCommonBind>("SkillUseCount");
            mBuffNum = mBind.GetGameObject("BuffNum");
            mShengQiShiBieDongBuff = mBind.GetCom<ComCommonBind>("ShengQiShiBieDongBuff");
            mBuffNumText = mBind.GetCom<Text>("BuffNumText");
            mNvDaQiangEnergyBar = mBind.GetCom<ComCommonBind>("NvDaQiangEnergyBar");
            // 力法combobuff
            mComboBuffCom = mBind.GetCom<ComCommonBind>("ComboBuffCom");
            mComboBuffNum = mComboBuffCom.GetGameObject("ComboBuffNum");
            mComboBuffNumGray = mComboBuffCom.GetCom<UIGray>("ComboBuffNumGray");
            mComboBuffNumText = mComboBuffCom.GetCom<Text>("ComboBuffNumText");
            mComboBuffProgress = mComboBuffCom.GetCom<Image>("ComboBuffProgress");
        }

        protected override void _unbindExUI()
        {
            mBulletNum = null;
            mSkillUseCount = null;
            mBuffNum = null;
            mShengQiShiBieDongBuff = null;
            mBuffNumText = null;
            mNvDaQiangEnergyBar = null;

            mComboBuffCom = null;
            mComboBuffNum = null;
            mComboBuffNumGray = null;
            mComboBuffNumText = null;
            mComboBuffProgress = null;
        }
        #endregion

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleProfessionFrame";
        }

        public override void Init()
        {
            base.Init();
            InitShengqishiBuffData();
        }

        private Dictionary<int, ComCommonBind> itemDic = new Dictionary<int, ComCommonBind>();

        #region 冰冻弹子弹数量相关
        public void SetSilverBulletNum(int skillId, int num, SpecialBulletType type)
        {
            ComCommonBind commonBind = null;
            if (itemDic.ContainsKey(skillId))
                commonBind = itemDic[skillId];
            else
            {
                GameObject go = GameObject.Instantiate(mBulletNum.gameObject);
                if (go != null)
                {
                    commonBind = go.GetComponent<ComCommonBind>();
                    string path = GetIconPathByType(type);
                    Image iamge = commonBind.GetCom<Image>("Icon");
                    ETCImageLoader.LoadSprite(ref iamge, path);
                    Utility.AttachTo(go, mBind.gameObject);
                    itemDic.Add(skillId, commonBind);
                }
            }
            if (commonBind == null)
                return;
            GameObject bulletGo = commonBind.gameObject;
            Text bulletNumTxt = commonBind.GetCom<Text>("Num");
            if (bulletGo != null)
                bulletGo.CustomActive(num > 0);
            if (bulletNumTxt != null)
                bulletNumTxt.text = num.ToString();
        }

        private string GetIconPathByType(SpecialBulletType type)
        {
            string path = null;
            switch (type)
            {
                case SpecialBulletType.STRESILVER:
                case SpecialBulletType.SILVER:
                    path = "UI/Image/Packed/p_UI_Icon_skillIcon.png:UI_ZhandouUI_Zidantou_01";
                    break;
                case SpecialBulletType.ICE:
                    path = "UI/Image/Packed/p_UI_Icon_skillIcon.png:UI_ZhandouUI_Zidantou_02";
                    break;
                case SpecialBulletType.FIRE:
                    path = "UI/Image/Packed/p_UI_Icon_skillIcon.png:UI_ZhandouUI_Zidantou_03";
                    break;
            }
            return path;
        }
        #endregion


        #region Buff数量
        public void SetBuffNum(int num)
        {
            if (mBuffNum == null) return;
            if (num <= 0)
                mBuffNum.SetActive(false);
            else
            {
                mBuffNum.CustomActive(true);
                mBuffNumText.text = num.ToString();
            }
        }
        #endregion

        #region Buff数量(力法Combobuff用)
        public void SetComboBuffNum(int num, float progress)
        {
            if (mComboBuffNum == null) 
                return;
            
            mComboBuffNum.CustomActive(true);
            if (num <= 0)
            {
                mComboBuffNumGray.enabled = true;
                mComboBuffProgress.fillAmount = 0;
            }
            else
            {
                mComboBuffNumGray.enabled = false;
                mComboBuffProgress.fillAmount = progress;
            }

            if (mCurComboNum != num)
            {
                mCurComboNum = num;
                mComboBuffNumText.text = num.ToString() + "层";
            }
        }
        #endregion
        
        #region 技能使用次数
        public void SetSkillUseCount(int skillId, int num, string iconPath)
        {
            ComCommonBind commonBind = null;
            if (itemDic.ContainsKey(skillId))
                commonBind = itemDic[skillId];
            else
            {
                GameObject go = GameObject.Instantiate(mSkillUseCount.gameObject);
                if (go != null)
                {
                    commonBind = go.GetComponent<ComCommonBind>();
                    Image iamge = commonBind.GetCom<Image>("Icon");
                    ETCImageLoader.LoadSprite(ref iamge, iconPath);
                    Utility.AttachTo(go, mBind.gameObject);
                    itemDic.Add(skillId, commonBind);
                }
            }
            if (commonBind == null)
                return;
            GameObject bulletGo = commonBind.gameObject;
            Text bulletNumTxt = commonBind.GetCom<Text>("Num");
            if (bulletGo != null)
                bulletGo.CustomActive(num >= 0);
            if (bulletNumTxt != null)
                bulletNumTxt.text = num.ToString();
        }
        #endregion

        #region 圣骑士觉醒被动Buff数量显示
        private GameObject zhufuRoot = null;
        private GameObject shenPanRoot = null;
        private GameObject zhufuContent = null;
        private GameObject shenPanContent = null;

        private GeEffectProxy[] effectArr = new GeEffectProxy[2];

        private Toggle[] zhufuBuffUI = new Toggle[5];     //祝福类Buff
        private Toggle[] shenpanBuffUI = new Toggle[5];   //审判类Buff
        private bool showEffectFlag = false;

        /// <summary>
        /// 初始化Buff数据
        /// </summary>
        private void InitShengqishiBuffData()
        {
            if (mShengQiShiBieDongBuff == null)
                return;
            if (zhufuRoot != null)
                return;
            zhufuRoot = mShengQiShiBieDongBuff.GetGameObject("ZhufuRoot");
            shenPanRoot = mShengQiShiBieDongBuff.GetGameObject("ShepanRoot");

            zhufuContent = mShengQiShiBieDongBuff.GetGameObject("ZhufuContent");
            shenPanContent = mShengQiShiBieDongBuff.GetGameObject("ShenPanContent");
            
            effectArr[0] = mShengQiShiBieDongBuff.GetCom<GeEffectProxy>("ShowEffect");
            effectArr[1] = mShengQiShiBieDongBuff.GetCom<GeEffectProxy>("HideEffect");

            if (zhufuContent != null)
            {
                for(int i=0;i< zhufuContent.transform.childCount; i++)
                {
                    if (i < 5)
                    {
                        zhufuBuffUI[i] = zhufuContent.transform.GetChild(i).GetComponent<Toggle>();
                    }
                }
            }

            if (shenPanContent != null)
            {
                for (int i = 0; i < shenPanContent.transform.childCount; i++)
                {
                    if (i < 5)
                    {
                        shenpanBuffUI[i] = shenPanContent.transform.GetChild(i).GetComponent<Toggle>();
                    }
                }
            }

            showEffectFlag = false;
        }

        /// <summary>
        /// 根据数量设置UI显示
        /// </summary>
        public void SetShengQiBeiDongBuff(int index, int curNum, int maxNum)
        {
            if (mShengQiShiBieDongBuff == null)
                return;
            InitShengqishiBuffData();
            Toggle[] toggleArr = index == 0 ? zhufuBuffUI : shenpanBuffUI;
            GameObject root = index == 0 ? zhufuRoot : shenPanRoot;
            for (int i = 0; i < toggleArr.Length; i++)
            {
                if (i < curNum && !toggleArr[i].isOn)
                {
                    ShowEffect(toggleArr[i].gameObject);
                    toggleArr[i].isOn = true;
                }

                if (i >= curNum && toggleArr[i].isOn)
                {
                    ShowEffect(toggleArr[i].gameObject, true);
                    toggleArr[i].isOn = false;
                }
            }
            root.CustomActive(curNum > 0);
            mShengQiShiBieDongBuff.CustomActive(index != -1);
        }

        /// <summary>
        /// 播放特效
        /// </summary>
        /// <param name="rootNode"></param>
        private void ShowEffect(GameObject rootNode,bool isHide = false)
        {
            GeEffectProxy effect = isHide ? effectArr[1] : effectArr[0];
            effect.CustomActive(false);
            if (showEffectFlag)
            {
                //第一次的时候别调整位置 因为父节点的位置还没有调整好 父节点是动态挂载上去的
                effect.transform.position = rootNode.transform.position;
            }
            effect.CustomActive(true);
            showEffectFlag = true;
        }
        #endregion
        #region 女大枪被动觉醒能两条
        private RectTransform pointerTransform;
        private GameObject effectRoot;
        private int energyProcess = 0;
        private int energyMaxProcess = 0;
        public int[] angle = new int[] { 120, 195, 260, 360 };
        public float[] size = new float[] { 0.55f, 0.65f, 0.75f, 1 };
        public void InitNvDaQiangEnergyBar(int n)
        {
            energyMaxProcess = n;
            if(null != mNvDaQiangEnergyBar)
            {
                mNvDaQiangEnergyBar.gameObject.SetActive(true);
            }
            if (null == pointerTransform && null != mNvDaQiangEnergyBar)
            {
                pointerTransform = mNvDaQiangEnergyBar.GetCom<RectTransform>("Pointer");
            }
            if(null == effectRoot && null != mNvDaQiangEnergyBar)
            {
                effectRoot = mNvDaQiangEnergyBar.GetGameObject("effectRoot");
                if(null != effectRoot)
                {
                    effectRoot.SetActive(false);
                }
            }
        }
        public void SetNvDaQiangEnergyBar(int times)
        {
            if (null != pointerTransform && times >= 0 && times < angle.Length) 
            {
                var mainQuence = DOTween.Sequence();
                int startIndex = 0;
                int endIndex = 0;
                if(energyProcess > times)
                {
                    for(int i = times; i < energyProcess; ++i)
                    {
                        var quence = DOTween.Sequence();
                        int length = endIndex - startIndex;
                        quence.Append(pointerTransform.transform.DORotate(new Vector3(0, 0, angle[i]), 0.166f));
                        mainQuence.Join(quence);
                    }
                }
                else if(energyProcess < times)
                {
                    for (int i = energyProcess + 1; i <= times; ++i)
                    {
                        var quence = DOTween.Sequence();
                        int length = endIndex - startIndex;
                        quence.Append(pointerTransform.transform.DORotate(new Vector3(0, 0, angle[i]), 0.5f));
                        mainQuence.Join(quence);
                    }
                }
                pointerTransform.transform.DOScale(new Vector3(size[times], size[times], 0), 0.5f);
                energyProcess = times;
                mainQuence.Play();
            }
            if(null != effectRoot)
            {
                if(times == energyMaxProcess && !effectRoot.activeInHierarchy)
                {
                    effectRoot.SetActive(true);
                }
                else if(times < energyMaxProcess && effectRoot.activeInHierarchy)
                {
                    effectRoot.SetActive(false);
                }
            }
        }
        #endregion
    }
}
