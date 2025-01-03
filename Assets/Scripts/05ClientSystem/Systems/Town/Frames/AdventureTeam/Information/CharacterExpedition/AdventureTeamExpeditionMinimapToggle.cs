using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public class AdventureTeamExpeditionMinimapToggle : MonoBehaviour
    {
        [SerializeField] private Text mAdventureTeamLvText;
        [SerializeField] private Toggle mMinimapToggle;
        [SerializeField] private Image mMinimapImage;
        [SerializeField] private GameObject mLockStateObj;
        [SerializeField] private GameObject mGetRewardStateObj;
        [SerializeField] private GameObject mDispatchStateObj;
        //[SerializeField] private GameObject mSelectObj;

        [SerializeField] private Protocol.ExpeditionMapBaseInfo tempMapBaseInfo;
        [SerializeField] private byte tempMapId;
        [SerializeField] private ExpeditionStatus tempStatus;

        [SerializeField] private CommonFrameButtonBuryPoint mBuryPoint;
        private string thisToggleTypeName = "";

        private GameObject[] mDispatchObjArray;
        private bool mIsLock = false;
        private const string ExpeditionDispathcRolePath = "UIFlatten/Prefabs/AdventureTeam/ExpeditionDispathcRole";
        private bool isSelect = false;        

        string tr_expedition_map_role_level_limit;
        private void Awake()
        {
            ClearData();
        }

        private void OnDestroy()
        {
            ClearData();
            mDispatchObjArray = null;
        }

        public void InitItemView(Protocol.ExpeditionMapBaseInfo mapBaseInfo, byte id)
        {
            tempMapBaseInfo = mapBaseInfo;
            tempMapId = id;
            tempStatus = (ExpeditionStatus)mapBaseInfo.expeditionStatus;
            mMinimapToggle.onValueChanged.AddListener(_OnChangeMapToggleClick);
            isSelect = false;

            //刷新埋点
            _TrySetSendBuryPointInfo();
        }

        public void UpdateItemInfo()
        {
            if (AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic.ContainsKey(tempMapId))
            {
                ExpeditionMapModel tempModel = AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic[tempMapId];
                if (mAdventureTeamLvText)
                {
                    mAdventureTeamLvText.text = tempModel.playerLevelLimit + "级";
                }

                if (mMinimapImage &&!string.IsNullOrEmpty(tempModel.miniMapImagePath))
                    ETCImageLoader.LoadSprite(ref mMinimapImage, tempModel.miniMapImagePath);
                if(mDispatchObjArray==null)
                {
                    mDispatchObjArray = new GameObject[tempModel.rolesCapacity];
                    for(int i = 0;i< tempModel.rolesCapacity; i++)
                    {
                        mDispatchObjArray[i] = LoadEffect(ExpeditionDispathcRolePath, mDispatchStateObj);
                    }
                }
                if (null != mMinimapToggle && tempModel.playerLevelLimit <= AdventureTeamDataManager.GetInstance().PlayerMaxLevel)
                {
                    mMinimapToggle.enabled = true;
                    mIsLock = false;
                    _UpdateMiniMapState();
                }
                else if (null != mMinimapToggle) 
                {
                    mMinimapToggle.enabled = false;
                    mIsLock = true;
                    _UpdateMiniMapState();
                }
                if (mMinimapToggle)
                {
                    bool tempFlag = mMinimapToggle.enabled;
                    mMinimapToggle.enabled = true;
                    mMinimapToggle.enabled = tempFlag;
                }
            }
        }

        public void ChangeToggleState(bool isOn)
        {
            mMinimapToggle.isOn = isOn;
        }

        private void _UpdateMiniMapState()
        {
            if (mIsLock)
            {
                mAdventureTeamLvText.text = "敬请期待";
                mLockStateObj.SetActive(true);
                mGetRewardStateObj.SetActive(false);
                mDispatchStateObj.SetActive(false);
            }
            else
            {
                switch (tempStatus)
                {
                    case ExpeditionStatus.EXPEDITION_STATUS_PREPARE:
                        mLockStateObj.SetActive(false);
                        mGetRewardStateObj.SetActive(false);
                        mDispatchStateObj.SetActive(false);
                        break;
                    case ExpeditionStatus.EXPEDITION_STATUS_IN:
                        mLockStateObj.SetActive(false);
                        mGetRewardStateObj.SetActive(false);
                        mDispatchStateObj.SetActive(true);
                        SetDispatchNum(tempMapBaseInfo.memberNum);
                        break;
                    case ExpeditionStatus.EXPEDITION_STATUS_OVER:
                        mLockStateObj.SetActive(false);
                        mGetRewardStateObj.SetActive(true);
                        mDispatchStateObj.SetActive(false);
                        break;
                    default:
                        break;
                }
            }
        }

        private void SetDispatchNum(int num)
        {
            for(int i =0;i< mDispatchObjArray.Length; i++)
            {
                if(mDispatchObjArray[i] != null)
                {
                    if (i < num)
                    {
                        mDispatchObjArray[i].SetActive(true);
                    }
                    else
                    {
                        mDispatchObjArray[i].SetActive(false);
                    }
                }
            }
        }

        private void _OnChangeMapToggleClick(bool isOn)
        {
            if (isOn == isSelect)
                return;
            isSelect = isOn; //避免多次点击做判断

            if (isOn)
            {
                AdventureTeamDataManager.GetInstance().SetExpeditionMapId(tempMapId);
                AdventureTeamDataManager.GetInstance().ReqExpeditionMapInfo(tempMapId);                
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAdventureTeamExpeditionIDChanged);

                //埋点
                if(mBuryPoint != null)
                {
                    mBuryPoint.ButtonName = thisToggleTypeName;
                    mBuryPoint.OnSendBuryingPoint();
                }
            }
        }
        

        public void OnItemRecycle()
        {
            ClearData();
            mMinimapToggle.onValueChanged.RemoveListener(_OnChangeMapToggleClick);
            mLockStateObj.SetActive(false);
            mGetRewardStateObj.SetActive(false);
            mDispatchStateObj.SetActive(false);
        }

        private void ClearData()
        {
            tempMapBaseInfo = null;
            tempMapId = 0;
            tempStatus = 0;
            mIsLock = false;
            thisToggleTypeName = "";
        }

        private GameObject LoadEffect(string effectPath, GameObject parent)
        {
            if (string.IsNullOrEmpty(effectPath) || null == parent)
            {
                return null;
            }

            GameObject effect = AssetLoader.instance.LoadResAsGameObject(effectPath);

            if (effect != null)
            {
                Utility.AttachTo(effect, parent);

            }
            return effect;
        }

        private void _TrySetSendBuryPointInfo( )
        {
            if (AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo == null)
                return;
            if (AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic == null)
                return;
            if (AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic.ContainsKey(tempMapId))
            {
                ExpeditionMapModel tempModel = AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic[tempMapId];
                if (tempModel != null)
                {
                    thisToggleTypeName = string.Format("Toggle{0}", tempModel.playerLevelLimit);
                }
            }
        }
    }
}
