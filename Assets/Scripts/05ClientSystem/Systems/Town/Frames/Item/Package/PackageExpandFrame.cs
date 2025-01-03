using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;
using Network;

namespace GameClient
{
    // 珍品类装备分解、出售二次确认界面
    public class PackageExpandFrame : ClientFrame
    {
        private PackageExpandView mView;
        private int mCostId;
        private int mCostCount;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Package/PackageExpandFrame";
        }

        protected override void _OnOpenFrame()
        {
            {
                mView = frame.GetComponent<PackageExpandView>();
                if (mView != null)
                {
                    string icon = string.Empty;
                    int key = PlayerBaseData.GetInstance().PackBaseSize + 8;
                    var tableData = TableManager.GetInstance().GetTableItem<ProtoTable.PackageEnlargeTable>(key);
                    if (tableData != null)
                    {
                        var strs = tableData.Consume.Split('_');
                        if (strs.Length != 2)
                        {
                            _OnCloseClick();
                        }
                        else
                        {
                            if (int.TryParse(strs[0], out mCostId) && int.TryParse(strs[1], out mCostCount))
                            {
                                var iconItem = TableManager.GetInstance().GetTableItem<ItemTable>(mCostId);
                                if (iconItem != null)
                                {
                                    icon = iconItem.Icon;
                                }
                            }
                            else
                            {
                                _OnCloseClick();
                            }
                        }
                    }
                    else
                    {
                        _OnCloseClick();
                    }

                    mView.Init(mCostCount, icon, _OnConfirmClick, _OnCloseClick);
                }
            }
        }

        private void _OnCloseClick()
        {
            Close();
        }

        private void _OnConfirmClick()
        {
            CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo
            {
                nMoneyID = mCostId,
                nCount = mCostCount
            };
            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
            {
                SceneEnlargePackage msg = new SceneEnlargePackage();
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);
                WaitNetMessageManager.GetInstance().Wait<SceneEnlargePackageRet>(msgRet =>
                {
                    if (msgRet == null)
                    {
                        return;
                    }

                    if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                    {
                        SystemNotifyManager.SystemNotify((int)msgRet.code);
                    }
                    else
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdatePackageTabRedPoint);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdatePackageGrids);
                        _OnCloseClick();
                    }
                });
            });
        }
    }
}
