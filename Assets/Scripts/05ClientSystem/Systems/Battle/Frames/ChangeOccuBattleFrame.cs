using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using DG.Tweening;

namespace GameClient
{
    public class ChangeOccuBattleFrame : ClientFrame
    {
        const int mMaxOccuNum = 3;
        private Toggle[] mToggleOccus = new Toggle[mMaxOccuNum];
        private Dictionary<int, Toggle> mTogglesIdDic = new Dictionary<int, Toggle>();
        private Image[] mImageOccus = new Image[mMaxOccuNum];
        private Image[] mImageSelectOccus = new Image[mMaxOccuNum];
        private Text[] mTextOccus = new Text[mMaxOccuNum];
        private ImageEx[] mImageSelected = new ImageEx[mMaxOccuNum];

        private Text mCharacterDesc = null;
        private Text mJobDesc = null;
        private ImageEx mJobWeaponIcon = null;
        private ImageEx mName1 = null;
        private ImageEx mName2 = null;
        private ImageEx mName3 = null;
        private ImageEx mName4 = null;
        List<Image> imageNameList = new List<Image>();
        List<DOTweenAnimation> allAnimList = new List<DOTweenAnimation>();

        private ComChangeOccuParam mComChangeOccuParam;
        
        private ButtonEx mButtonFinish;
        private ButtonEx mButtonCD;
        
        private GameObject mSpineRoot;
        private GeObjectRenderer mObjRender;
        private GameObject mObjJobImage;
        private int mSelectJobId = 0;
        private bool isFirst = false;
        ChangeOccuBattle mBattle;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/ChangeOccuBattleFrame";
        }

        protected override void _bindExUI()
        {
            for (int i = 0; i < mMaxOccuNum; i++)
            {
                mToggleOccus[i] = mBind.GetCom<Toggle>("toggle" + i);
                mImageOccus[i] = mBind.GetCom<Image>("icon" + i);
                mImageSelectOccus[i] = mBind.GetCom<Image>("select_icon" + i);
                mTextOccus[i] = mBind.GetCom<Text>("name" + i);
                mImageSelected[i] = mBind.GetCom<ImageEx>("selected" + i);
            }
            mButtonFinish = mBind.GetCom<ButtonEx>("finish");
            mButtonCD = mBind.GetCom<ButtonEx>("cd");
            mSpineRoot = mBind.GetGameObject("SpineRoot");
            mObjRender = mBind.GetCom<GeObjectRenderer>("objRender");
            
            mCharacterDesc = mBind.GetCom<Text>("TextDesp");
            mJobDesc = mBind.GetCom<Text>("TextSubDesp");
            mJobWeaponIcon = mBind.GetCom<ImageEx>("JobWeaponIcon");
            mName1 = mBind.GetCom<ImageEx>("Name1");
            mName2 = mBind.GetCom<ImageEx>("Name2");
            mName3 = mBind.GetCom<ImageEx>("Name3");
            mName4 = mBind.GetCom<ImageEx>("Name4");
            imageNameList.Add(mName1);
            imageNameList.Add(mName2);
            imageNameList.Add(mName3);
            imageNameList.Add(mName4);
            
            mComChangeOccuParam = mBind.GetCom<ComChangeOccuParam>("ComChangeOccuParam");
            if (mComChangeOccuParam != null)
            {
                for (int i = 0; i < mComChangeOccuParam.AllAnimList.Length; i++)
                {
                    DOTweenAnimation[] anim = mComChangeOccuParam.AllAnimList[i].GetComponents<DOTweenAnimation>();
                    allAnimList.AddRange(anim);
                }
            }
            
            
            GlobalEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonOnFight, OnDungeonOnFight);
        }

        protected override void _unbindExUI()
        {
            isFirst = false;
            for (int i = 0; i < mMaxOccuNum; i++)
            {
                mToggleOccus[i] = null;
                mImageOccus[i] = null;
                mImageSelectOccus[i] = null;
                mTextOccus[i] = null;
                mImageSelected[i] = null;
            }
            mButtonFinish = null;
            mButtonCD = null;
            mTogglesIdDic.Clear();
            
            mCharacterDesc = null;
            mJobDesc = null;
            mJobWeaponIcon = null;
            mName1 = null;
            mName2 = null;
            mName3 = null;
            mName4 = null;
            if(imageNameList != null)
            {
                imageNameList.Clear();
            }
            if(allAnimList != null)
            {
                allAnimList.Clear();
            }
            
            GlobalEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonOnFight, OnDungeonOnFight);
        }

        private void OnDungeonOnFight(UIEvent uiEvent)
        {
            if(isFirst)
                return;
            isFirst = true;
            mBattle.PauseChangeOccuFight();
            ClientSystemManager.GetInstance().delayCaller.DelayCall(500, () =>
            {
                mBattle = BattleMain.instance.GetBattle() as ChangeOccuBattle;
                if (mBattle == null)
                {
                    return;
                }
                if (mTogglesIdDic.ContainsKey(mBattle.MainPlayerOccuID))
                {
                    mTogglesIdDic[mBattle.MainPlayerOccuID].isOn = false;
                    mTogglesIdDic[mBattle.MainPlayerOccuID].isOn = true;
                }
            });
        }

        public void InitData()
        {
            mBattle = BattleMain.instance.GetBattle() as ChangeOccuBattle;
            if (mBattle == null)
            {
                return;
            }

            foreach (var item in mToggleOccus)
            {
                item.CustomActive(false);
            }
            
            mTogglesIdDic.Clear();

            mSelectJobId = 0;
            var occuIds = mBattle.mAllOccus;
            for (int i = 0; i < occuIds.Count && i < mMaxOccuNum; i++)
            {
                int index = i;
                int id = occuIds[i];
                var data = TableManager.GetInstance().GetTableItem<JobTable>(id);
                if (data != null && data.Open == 1)
                {
                    mTextOccus[i].text = data.Name;
                    ETCImageLoader.LoadSprite(ref mImageOccus[i], data.JobHead);
                    ETCImageLoader.LoadSprite(ref mImageSelectOccus[i], data.JobHead);
                    //Utility.createSprite((data as ProtoTable.JobTable).JobHead, ref mImageOccus[i]);

                    var toggle = mToggleOccus[i];
                    if (toggle != null)
                    {
                        toggle.onValueChanged.RemoveAllListeners();
                        toggle.onValueChanged.AddListener((bool isOn) =>
                        {
                            if (isOn)
                            {
                                if (mSelectJobId != id)
                                {
                                    mSelectJobId = id;
                                    _ShowJobInfo();
                                    mBattle.PauseChangeOccuFight();
                                    ClientSystemManager.GetInstance().delayCaller.DelayCall((int)(mComChangeOccuParam.ShowTime * 1000), () =>
                                    {
                                        _HideJobInfo();
                                        mBattle.ResumeChangeOccuFight();
                                        mBattle.ChangeOccuId(id);
                                        for (int j = 0; j < mToggleOccus.Length; j++)
                                        {
                                            mImageSelected[j].CustomActive(j == index);
                                        }
                                    });
                                }
                            }
                        });


                        toggle.CustomActive(true);
                        mTogglesIdDic.Add(id, toggle);
                    }
                }
            }

            if (mButtonFinish != null)
            {
                mButtonFinish.onClick.RemoveAllListeners();
                mButtonFinish.onClick.AddListener(() =>
                {
                    mBattle.Exit();
                });
            }

            if (mButtonCD != null)
            {
                mButtonCD.onClick.RemoveAllListeners();
                mButtonCD.onClick.AddListener(() =>
                {
                    mBattle.RefreshCD();
                });
            }
        }
        
        protected override void _OnOpenFrame()
        {
            InitData();
        }

        void PlayAnim()
        {
            for (int i = 0; i < allAnimList.Count; i++)
            {
                allAnimList[i].DORestart();
            }
        }
        
        private void _ShowJobInfo()
        {
            JobTable tableData = TableManager.GetInstance().GetTableItem<JobTable>(mSelectJobId);
            if (null == tableData)
            {
                Logger.LogErrorFormat("职业表中找不到id={0}的职业", mSelectJobId);
                return;
            }
            
            if (mCharacterDesc != null)
            {
                mCharacterDesc.text = TR.Value("creat_role_character_desc", tableData.RecommendedAttribute);
            }

            if (mJobDesc != null)
            {
                mJobDesc.text = tableData.JobDes[0];
            }

            //名字
            int index = 0;
            for (; index < tableData.JobNameImgPaths.Length; ++index)
            {
                if (index >= imageNameList.Count)
                {
                    break;
                }
                imageNameList[index].CustomActive(true);
                imageNameList[index].SafeSetImage(tableData.JobNameImgPaths[index]);
            }
            for (; index < imageNameList.Count; ++index)
            {
                imageNameList[index].CustomActive(false);
            }

            if (mJobWeaponIcon != null)
            {
                Image jobWeaponIcon = mJobWeaponIcon;
                ETCImageLoader.LoadSprite(ref jobWeaponIcon, tableData.JobWeaponIcon);
                jobWeaponIcon.SetNativeSize();
            }

            //动画
            if (mObjJobImage != null)
            {
                GameObject.DestroyImmediate(mObjJobImage);
                mObjJobImage = null;
            }
            if (tableData.JobImage.Contains("Animation"))
            {
                _ShowModule(tableData.ID, tableData.JobImage);
                mSpineRoot.SetActive(false);

                ClientSystemManager.GetInstance().delayCaller.DelayCall(100, () =>
                {
                    mSpineRoot.CustomActive(true);
                });
            }
            else
            {
                _HideModule();
                mObjJobImage = AssetLoader.instance.LoadResAsGameObject(tableData.JobImage);
                if (mObjJobImage != null && mSpineRoot != null)
                {
                    mSpineRoot.CustomActive(true);
                    Utility.AttachTo(mObjJobImage, mSpineRoot);

                    mObjJobImage.transform.SetAsFirstSibling();
                }
            }
            
            PlayAnim();
        }

        private void _HideJobInfo()
        {
            mSpineRoot.CustomActive(false);
            _HideModule();
        }

        void _ShowModule(int jobID, string path)
        {
            if (mObjRender != null)
            {
                mObjRender.gameObject.CustomActive(true);
                mObjRender.ClearObject();

                try
                {
                    mObjRender.LoadObject(path, 28);
                }
                catch (System.Exception e)
                {
                    Logger.LogErrorFormat("create spineModule failed: {0}", e.ToString());
                }
            }
        }
        void _HideModule()
        {
            if (mObjRender != null)
            {
                mObjRender.gameObject.CustomActive(false);
            }
        }
    }
}
