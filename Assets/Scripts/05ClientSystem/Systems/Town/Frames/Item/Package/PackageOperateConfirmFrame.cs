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
    public class PackageOperateConfirmFrameParam
    {
        public int Mode; //PackageNewFrame.EItemsShowMode
        public ItemData Equip;
        public Action CallBack;
        public bool IsPrecious;
    }

    // 珍品类装备分解、出售二次确认界面
    public class PackageOperateConfirmFrame : ClientFrame
    {
        private PackageOperateConfirmView mView;
        PackageOperateConfirmFrameParam mParam;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Package/PackageOperateConfirmFrame";
        }

        protected override void _OnOpenFrame()
        {
            var data = userData as PackageOperateConfirmFrameParam;
            if (data == null || frame == null)
            {
                Close();
            }
            else
            {
                mParam = data;
                mView = frame.GetComponent<PackageOperateConfirmView>();
                if (mView != null)
                {
                    string title = TR.Value("package_confirm_title_decompse"); 
                    string tip = TR.Value("package_confirm_tip_decompse", data.Equip.GetColorName("[{0}]", true));
                    Dictionary<int, KeyValuePair<int, int>> dic = new Dictionary<int, KeyValuePair<int, int>>();

                    if (data.Equip.EquipType == EEquipType.ET_REDMARK)
                    {
                        tip = TR.Value("package_confirm_tip_redmark_decompse", data.Equip.GetColorName("[{0}]", true));
                    }
                    else if (data.IsPrecious)
                    {
                        tip = TR.Value("package_confirm_tip_decompse_precious", data.Equip.GetColorName("[{0}]", true));
                    }

                    if ((PackageNewFrame.EItemsShowMode)data.Mode == PackageNewFrame.EItemsShowMode.QuickSell)
                    {
                        title = TR.Value("package_confirm_title_sell");
                        tip = TR.Value("package_confirm_tip_sell", data.Equip.GetColorName("[{0}]", true));
                        if (data.Equip.EquipType == EEquipType.ET_REDMARK)
                        {
                            tip = TR.Value("package_confirm_tip_redmark_sell", data.Equip.GetColorName("[{0}]", true));
                        }
                        else if (data.IsPrecious)
                        {
                            tip = TR.Value("package_confirm_tip_sell_precious", data.Equip.GetColorName("[{0}]", true));
                        }

                        int id = data.Equip.TableData.SellItemID;
                        int price = data.Equip.TableData.Price;
                        if (id > 0 && price > 0)
                        {
                            if (dic.ContainsKey(id))
                            {
                                dic[id] = new KeyValuePair<int, int>(dic[id].Key + price, dic[id].Key + price);
                            }
                            else
                            {
                                dic.Add(id, new KeyValuePair<int, int>(price, price));
                            }
                        }
                    }
                    else
                    {
                        var decomposeTable = GameUtility.Item.GetDecomposeData(data.Equip);
                        if (decomposeTable != null)
                        {
                            if(data.Equip.SubType == (int)ItemTable.eSubType.ST_BXY_EQUIP)
                            {
                                if((int)data.Equip.Quality == 3)
                                {
                                    dic.Add(910000024, new KeyValuePair<int, int>(50, 100));
                                }
                                else if((int)data.Equip.Quality == 4)
                                {
                                    dic.Add(910000024, new KeyValuePair<int, int>(100, 200));
                                }
                                else if((int)data.Equip.Quality == 5)
                                {
                                    dic.Add(910000024, new KeyValuePair<int, int>(200, 320));
                                }
                                else
                                {
                                    dic.Add(910000024, new KeyValuePair<int, int>(320, 640));
                                }
                            }
                            else
                            {
                                //无色材料
                                GameUtility.Item.UpdateNumDecomposeMaterial(decomposeTable.ColorLessMatNumLength, decomposeTable.ColorLessMatNumArray, decomposeTable.ColorLessMatId, dic);
                                //有色材料
                                GameUtility.Item.UpdateNumDecomposeMaterial(decomposeTable.ColorMatNumLength, decomposeTable.ColorMatNumArray, decomposeTable.ColorMatId, dic);
                                //宇宙之眼
                                GameUtility.Item.UpdateNumDecomposeMaterial(decomposeTable.DogEyeNumLength, decomposeTable.DogEyeNumArray, decomposeTable.DogEyeId, dic);
                                //异界材料
                                GameUtility.Item.UpdateStringDecomposeMaterial(decomposeTable.MagicItemNumLength, decomposeTable.MagicItemNumArray, null, decomposeTable.MagicItemId, dic);
                                //特殊材料
                                GameUtility.Item.UpdateStringDecomposeMaterial(1, null, decomposeTable.PinkItemNum, decomposeTable.PinkItemId, dic);
                                //异界气息材料
                                GameUtility.Item.UpdateStringDecomposeMaterial(1, null, decomposeTable.RedItemNum, decomposeTable.RedItemId, dic);
                            }
                        }
                    }

                    mView.Init(dic, title, tip, _OnConfirmClick, _OnCloseClick);
                }
            }
        }

        private void _OnCloseClick()
        {
            Close();
        }

        private void _OnConfirmClick()
        {
            if (mParam != null)
            {
                ulong[] guids = new ulong[1];
                guids[0] = mParam.Equip.GUID;

                if (mParam.Mode == (int)PackageNewFrame.EItemsShowMode.Decompose)
                {
                    ItemDataManager.GetInstance().SendDecomposeItem(guids);
                }
                else
                {
                    ItemDataManager.GetInstance().SendSellItem(guids);
                }
                mParam.CallBack?.Invoke();
            }
            Close();
        }

        protected override void _bindExUI()
        {
            //itemsRoot = mBind.GetGameObject("itemsRoot");
            //itemTemplate = mBind.GetGameObject("itemTemplate");
            //noItemTips = mBind.GetGameObject("noItemTips");
        }

        protected override void _unbindExUI()
        {

        }

    }
}
