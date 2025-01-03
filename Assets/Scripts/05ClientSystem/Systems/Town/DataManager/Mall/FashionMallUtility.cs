using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{
    //拍卖行助手类
    public static class FashionMallUtility
    {

        //主页签的数据(套装，单品，武器)
        public static List<FashionMallClothTabData> GetClothTabDataModelList()
        {
            List<FashionMallClothTabData> clothTabDataModelList = new List<FashionMallClothTabData>();

            clothTabDataModelList.Add(new FashionMallClothTabData
            {
                Index = 0,
                MallTableId = 11,
                Name = TR.Value("Fashion_mall_cloth_tab_suit"),
                ClothTabType = FashionMallClothTabType.Suit,
            });

            clothTabDataModelList.Add(new FashionMallClothTabData
            {
                Index = 1,
                MallTableId = 4,
                Name = TR.Value("Fashion_mall_cloth_tab_single"),
                ClothTabType = FashionMallClothTabType.Single,
            });

            clothTabDataModelList.Add(new FashionMallClothTabData
            {
                Index = 2,
                MallTableId = 21,
                Name = TR.Value("Fashion_mall_cloth_tab_weapon"),
                ClothTabType = FashionMallClothTabType.Weapon,
            });

            return clothTabDataModelList;
        }

        //职业的数据：需要充职业表中读取，现在是直接写死的。
        public static List<ComControlData> GetProfessionTabDataModelList()
        {
            List<ComControlData> professionTabDataModelList = new List<ComControlData>();

            var jobTable = TableManager.GetInstance().GetTable<JobTable>();
            if (jobTable == null)
            {
                Logger.LogErrorFormat("Data Error JobTable is null");
                return professionTabDataModelList;
            }

            //从表中读取数据
            var jobTableEnumerator = jobTable.GetEnumerator();
            var index = 0;
            while (jobTableEnumerator.MoveNext())
            {
                var curJobTable = jobTableEnumerator.Current.Value as JobTable;
                //基础职业，并且开放
                if (curJobTable != null
                    && curJobTable.prejob == 0
                    && curJobTable.Open == 1)
                {
                    var curProfessionTabDataModelList = new FashionMallProfessionTabData(
                        ++index,
                        curJobTable.ID,
                        curJobTable.Name,
                        false);
                    professionTabDataModelList.Add(curProfessionTabDataModelList);
                }
            }

            //按照ID进行排序
            if (professionTabDataModelList.Count > 0)
            {
                professionTabDataModelList.Sort((x, y) => x.Id.CompareTo(y.Id));
            }

            return professionTabDataModelList;
        }

        //(位置的数据，共5个位置：头，衣服，胸饰，裤子，腰饰）
        public static List<FashionMallPositionTabData> GetPositionTabDataModelList()
        {
            List<FashionMallPositionTabData> positionTabDataModelList = new List<FashionMallPositionTabData>();

            positionTabDataModelList.Add(new FashionMallPositionTabData
            {
                Index = 0,
                PositionTabType = FashionMallPositionTabType.Head,
            });

            positionTabDataModelList.Add(new FashionMallPositionTabData
            {
                Index = 1,
                PositionTabType = FashionMallPositionTabType.Clothes,
            });


            positionTabDataModelList.Add(new FashionMallPositionTabData
            {
                Index = 2,
                PositionTabType = FashionMallPositionTabType.Plastron,
            });


            positionTabDataModelList.Add(new FashionMallPositionTabData
            {
                Index = 3,
                PositionTabType = FashionMallPositionTabType.Trousers,
            });

            positionTabDataModelList.Add(new FashionMallPositionTabData
            {
                Index = 4,
                PositionTabType = FashionMallPositionTabType.Waist,
            });

            return positionTabDataModelList;
        }


        public static int GetMallType(int mallTableId)
        {
            var mallData = TableManager.GetInstance().GetTableItem<MallTypeTable>(mallTableId);
            if (mallData == null)
            {
                Logger.LogErrorFormat("MallTypeTable is null and mallTableId is {0}", mallTableId);
                return 0;
            }

            return (int) mallData.MallType;
        }

        //转换Slot类型
        public static bool IsSameSlotType(EFashionWearSlotType fashionWearSlotType,
            FashionMallPositionTabType positionTabType)
        {
            if (fashionWearSlotType == EFashionWearSlotType.Head
                && positionTabType == FashionMallPositionTabType.Head)
                return true;

            if (fashionWearSlotType == EFashionWearSlotType.UpperBody
                && positionTabType == FashionMallPositionTabType.Clothes)
                return true;

            if (fashionWearSlotType == EFashionWearSlotType.Chest
                && positionTabType == FashionMallPositionTabType.Plastron)
                return true;

            if (fashionWearSlotType == EFashionWearSlotType.LowerBody
                && positionTabType == FashionMallPositionTabType.Trousers)
                return true;

            if (fashionWearSlotType == EFashionWearSlotType.Waist
                && positionTabType == FashionMallPositionTabType.Waist)
                return true;

            if (fashionWearSlotType == EFashionWearSlotType.Weapon
                && positionTabType == FashionMallPositionTabType.Weapon)
                return true;

            return false;
        }

        //得到装备的类型
        public static EFashionWearSlotType GetEquipSlotType(ItemTable itemTable)
        {
            if (itemTable.SubType == ItemTable.eSubType.FASHION_HEAD)
            {
                return EFashionWearSlotType.Head; // 头饰
            }
            else if (itemTable.SubType == ItemTable.eSubType.FASHION_CHEST)
            {
                return EFashionWearSlotType.UpperBody; // 上装
            }
            else if (itemTable.SubType == ItemTable.eSubType.FASHION_EPAULET)
            {
                return EFashionWearSlotType.Chest; // 胸饰
            }
            else if (itemTable.SubType == ItemTable.eSubType.FASHION_LEG)
            {
                return EFashionWearSlotType.LowerBody; // 下装
            }
            else if (itemTable.SubType == ItemTable.eSubType.FASHION_SASH)
            {
                return EFashionWearSlotType.Waist; // 腰饰
            }
            else if (itemTable.SubType == ItemTable.eSubType.FASHION_WEAPON)
            {
                return EFashionWearSlotType.Weapon;         //时装武器
            }
            else
            {
                return EFashionWearSlotType.Invalid;
            }
        }

        public static int GetSelfBaseJobId()
        {
            var selfBaseId = PlayerBaseData.GetInstance().JobTableID;

            var data = TableManager.GetInstance().GetTableItem<JobTable>(selfBaseId);
            if (data == null)
                return selfBaseId;

            if (data.JobType == 1)
                selfBaseId = data.prejob;

            return selfBaseId;
        }

        //对应商城表中的MallSubType字段
        //1-5对应单件套时装的部位，6位时装套装
        public static int GetMallTableSubTypeIndex(FashionMallClothTabType clothTabType,
            FashionMallPositionTabType positionTabType)
        {
            //套装
            if (clothTabType == FashionMallClothTabType.Suit)
            {
                //套装
                return 6;
            }
            else if (clothTabType == FashionMallClothTabType.Weapon)
            {
                //武器
                return 7;
            }
            else
            {
                //单品
                switch (positionTabType)
                {
                    case FashionMallPositionTabType.Head:
                        return 1;
                    case FashionMallPositionTabType.Clothes:
                        return 2;
                    case FashionMallPositionTabType.Plastron:
                        return 3;
                    case FashionMallPositionTabType.Trousers:
                        return 4;
                    case FashionMallPositionTabType.Waist:
                        return 5;
                }

                return 1;
            }
        }

        public static void CloseFashionLimitTimeBuyFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<FashionLimitTimeBuyFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<FashionLimitTimeBuyFrame>();
        }

        public static void CloseFashionMallBuyFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<MallBuyFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<MallBuyFrame>();
        }



    }
}