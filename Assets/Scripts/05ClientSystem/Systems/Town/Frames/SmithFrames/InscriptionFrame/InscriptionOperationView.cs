using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class InscriptionOperationView : MonoBehaviour
    {
        [SerializeField] private Text mDesc;
        [SerializeField] private Text mSuccessRate;
        [SerializeField] private Button mOkBtn;
        [SerializeField] private InscriptionExtractConsumeItem mInscriptionExtractConsumeItem;
        [SerializeField] private InscriptionExtractItem mInscriptionExtractItem;
        [SerializeField] private Toggle mInscriptionFractureTog;
        [SerializeField] private Toggle mInscriptionPickTog;
        [SerializeField] private GameObject mLockGo;
        [SerializeField] private GameObject mFarctureCheckMarkGo;
        [SerializeField] private GameObject mPickCheckMarkGo;

        private List<InscriptionConsume> mInscriptionConsumeList = new List<InscriptionConsume>();
        private InscriptionExtractItemData mInscriptionExtractItemData;
        private InscriptionOperationType mInscriptionOperationType = InscriptionOperationType.InscriptionFracture;
        private int iSuccessRate = 0;
        private bool bIsInscriptionPick = false;

        private void Awake()
        {
            if(mOkBtn != null)
            {
                mOkBtn.onClick.RemoveAllListeners();
                mOkBtn.onClick.AddListener(OnOkBtnClick);
            }

            if(mInscriptionFractureTog != null)
            {
                mInscriptionFractureTog.onValueChanged.RemoveAllListeners();
                mInscriptionFractureTog.onValueChanged.AddListener(OnInscriptionFractureTogValueChanged);
            }

            if(mInscriptionPickTog != null)
            {
                mInscriptionPickTog.onValueChanged.RemoveAllListeners();
                mInscriptionPickTog.onValueChanged.AddListener(OnInscriptionPickTogValueChanged);
            }
        }

        private void OnDestroy()
        {
            if (mInscriptionConsumeList != null)
                mInscriptionConsumeList.Clear();

            mInscriptionExtractItemData = null;
            iSuccessRate = 0;
        }

        public void InitView(InscriptionExtractItemData inscriptionExtractItemData)
        {
            if (inscriptionExtractItemData == null)
                return;

            mInscriptionExtractItemData = inscriptionExtractItemData;

            if(mInscriptionExtractItem != null)
            {
                mInscriptionExtractItem.OnItemVisiable(mInscriptionExtractItemData, null, false);
            }

            bIsInscriptionPick = InscriptionMosaicDataManager.GetInstance().CheckInscriptionItemIsExtract(mInscriptionExtractItemData.inscriptionItem.TableID);

            if(mLockGo != null)
            {
                mLockGo.CustomActive(!bIsInscriptionPick);
            }

            if(mInscriptionFractureTog != null)
            {
                mInscriptionFractureTog.isOn = true;
            }
        }

        private void UpdateDesc()
        {
            if(mDesc != null)
            {
                if (mInscriptionOperationType == InscriptionOperationType.InscriptionFracture)
                {
                    mDesc.text = "碎裂后铭文会消失";
                }
                else if (mInscriptionOperationType == InscriptionOperationType.InscriptionPick)
                {
                    mDesc.text = "摘取后铭文不会消失，会进入背包";
                }
            }
        }

        private void UpdateInscriptionFractureConsum()
        {
            if (mInscriptionExtractItemData.inscriptionItem == null)
                return;

            int inscriptionId = mInscriptionExtractItemData.inscriptionItem.TableID;

            if (mInscriptionConsumeList != null)
                mInscriptionConsumeList.Clear();


            List<InscriptionConsume> datas = null;
            if (mInscriptionOperationType == InscriptionOperationType.InscriptionFracture)
            {
                datas = InscriptionMosaicDataManager.GetInstance().GetInscriptionFractureConsume(inscriptionId);
            }
            else if(mInscriptionOperationType == InscriptionOperationType.InscriptionPick)
            {
                datas = InscriptionMosaicDataManager.GetInstance().GetInscriptionExtractConsume(inscriptionId);
            }
            
            if(datas != null)
            {
                mInscriptionConsumeList.AddRange(datas);
                if (datas.Count > 0)
                {
                    InscriptionConsume inscriptionConsume = datas[0];
                    if (inscriptionConsume != null)
                    {
                        if (mInscriptionExtractConsumeItem != null)
                        {
                            mInscriptionExtractConsumeItem.OnItemVisiable(inscriptionConsume);
                        }
                    }
                }
            }
        }

        private void UpdateInscriptionFractureSuccessRateDesc()
        {
            if (mInscriptionExtractItemData.inscriptionItem == null)
                return;

            int inscriptionId = mInscriptionExtractItemData.inscriptionItem.TableID;
            
            if (mInscriptionOperationType == InscriptionOperationType.InscriptionFracture)
            {
                iSuccessRate = InscriptionMosaicDataManager.GetInstance().GetInscriptionFractureSuccessRate(inscriptionId);
            }
            else if (mInscriptionOperationType == InscriptionOperationType.InscriptionPick)
            {
                iSuccessRate = InscriptionMosaicDataManager.GetInstance().GetInscriptionExtractSuccessRate(inscriptionId);
            }
            
            if (mSuccessRate != null)
            {
                mSuccessRate.text = InscriptionMosaicDataManager.GetInstance().GetInscriptionExtractSuccessRateDesc(iSuccessRate);
            }
        }

        private void OnInscriptionFractureTogValueChanged(bool value)
        {
            if(value)
            {
                mInscriptionOperationType = InscriptionOperationType.InscriptionFracture;
                UpdateDesc();
                UpdateInscriptionFractureConsum();
                UpdateInscriptionFractureSuccessRateDesc();
            }

            if (mFarctureCheckMarkGo != null)
                mFarctureCheckMarkGo.CustomActive(value);
        }

        private void OnInscriptionPickTogValueChanged(bool value)
        {
            if(value)
            {
                //不能摘取
                if (!bIsInscriptionPick)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("只有传说品质铭文可摘取");

                    if(mInscriptionFractureTog != null)
                    {
                        mInscriptionFractureTog.isOn = true;
                    }
                    return;
                }

                mInscriptionOperationType = InscriptionOperationType.InscriptionPick;
                UpdateDesc();
                UpdateInscriptionFractureConsum();
                UpdateInscriptionFractureSuccessRateDesc();
            }

            if (bIsInscriptionPick)
            {
                if (mPickCheckMarkGo != null)
                    mPickCheckMarkGo.CustomActive(value);
            }
        }

        private void OnOkBtnClick()
        {
            if(mInscriptionExtractItemData == null)
                return;

            if (mInscriptionExtractItemData.inscriptionItem == null)
                return;

            if (mInscriptionExtractItemData.equipmentItem == null)
                return;

            if (mInscriptionOperationType == InscriptionOperationType.InscriptionFracture)
            {
                List<CostItemManager.CostInfo> costInfos = new List<CostItemManager.CostInfo>();
                if (mInscriptionConsumeList != null)
                {
                    for (int i = 0; i < mInscriptionConsumeList.Count; i++)
                    {
                        InscriptionConsume consume = mInscriptionConsumeList[i];

                        if (consume == null)
                        {
                            continue;
                        }

                        CostItemManager.CostInfo info = new CostItemManager.CostInfo();
                        info.nMoneyID = consume.itemId;
                        info.nCount = consume.count;
                        costInfos.Add(info);
                    }
                }

                CostItemManager.GetInstance().TryCostMoneiesDefault(costInfos, () =>
                {
                    string mContent = string.Format("本次碎裂的铭文为[{0}],您确定要碎裂该铭文吗?", mInscriptionExtractItemData.inscriptionItem.GetColorName());
                    SystemNotifyManager.SysNotifyMsgBoxOkCancel(mContent, () =>
                    {
                        InscriptionMosaicDataManager.GetInstance().OnSceneEquipInscriptionDestroyReq(mInscriptionExtractItemData.equipmentItem.GUID, (uint)mInscriptionExtractItemData.inscriptionItem.TableID, (uint)mInscriptionExtractItemData.index);
                    });
                });
            }
            else if (mInscriptionOperationType == InscriptionOperationType.InscriptionPick)
            {
                //检查材料是否足够
                bool isMaterialEnough = true;
                for (int i = 0; i < mInscriptionConsumeList.Count; i++)
                {
                    InscriptionConsume consume = mInscriptionConsumeList[i];
                    int iTotalNumber = ItemDataManager.GetInstance().GetItemCountInPackage(consume.itemId);
                    int iNeedNumber = consume.count;

                    if (iTotalNumber < iNeedNumber)
                    {
                        isMaterialEnough = false;
                        break;
                    }
                }

                //材料不足
                if (isMaterialEnough == false)
                {
                    SystemNotifyManager.SysNotifyTextAnimation("摘取失败,摘取材料不足");
                    return;
                }

                //成功率为100%
                if ((iSuccessRate / 1000) >= 1)
                {
                    string mContent = "您确定要摘下该铭文吗?本次摘取必定成功。";
                    SystemNotifyManager.SysNotifyMsgBoxOkCancel(mContent, () =>
                    {
                        InscriptionMosaicDataManager.GetInstance().OnSceneEquipInscriptionExtractReq(mInscriptionExtractItemData.equipmentItem.GUID, (uint)mInscriptionExtractItemData.inscriptionItem.TableID, (uint)mInscriptionExtractItemData.index);
                    });

                    return;
                }
                else
                {
                    string mContent = string.Format("您确定要摘下该铭文吗?本次摘取成功率[{0}]，摘取失败铭文将消失。", InscriptionMosaicDataManager.GetInstance().GetInscriptionExtractSuccessRateDesc(iSuccessRate));
                    SystemNotifyManager.SysNotifyMsgBoxOkCancel(mContent, () =>
                    {
                        InscriptionMosaicDataManager.GetInstance().OnSceneEquipInscriptionExtractReq(mInscriptionExtractItemData.equipmentItem.GUID, (uint)mInscriptionExtractItemData.inscriptionItem.TableID, (uint)mInscriptionExtractItemData.index);
                    });

                    return;
                }
            }
        }
    }
}