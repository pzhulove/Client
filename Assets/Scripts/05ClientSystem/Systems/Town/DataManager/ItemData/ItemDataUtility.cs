using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using ProtoTable;

namespace GameClient
{
    //道具助手类
    public static class ItemDataUtility
    {

        //物品的交易次数是否限制
        public static bool IsItemTradeLimitBuyNumber(ItemData itemData)
        {
            if (itemData == null)
                return false;

            if (itemData.TableData == null)
                return false;

            //表中字段配置了交易次数，则说明限制
            if (itemData.TableData.TransactionsNum > 0)
                return true;

            //没有限制
            return false;
        }

        //交易的剩余次数
        public static int GetItemTradeLeftTime(ItemData itemData)
        {
            if (itemData == null)
                return 0;

            if (itemData.TableData == null)
                return 0;

            var leftTime = itemData.TableData.TransactionsNum - itemData.ItemTradeNumber;

            if (leftTime <= 0)
                return 0;
            else
            {
                return leftTime;
            }
        }

        //前，后
        //得到属性改变的列表
        public static List<BetterEquipmentData> GetPlayerAttributeDisplayChangeList(
            DisplayAttribute beforePlayerAttribute,
            DisplayAttribute afterPlayerAttribute)
        {
            if (afterPlayerAttribute == null
                || beforePlayerAttribute == null)
                return null;

            var betterEquipmentDataList = new List<BetterEquipmentData>();

            #region DisplayAttribute
            //生命
            var betterEquipmentData = CreateBetterData(beforePlayerAttribute.maxHp,
                afterPlayerAttribute.maxHp, TR.Value("player_attribute_format_maxHp"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //魔力
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.maxMp,
                afterPlayerAttribute.maxMp, TR.Value("player_attribute_format_maxMp"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //生命恢复量
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.hpRecover,
                afterPlayerAttribute.hpRecover, TR.Value("player_attribute_format_hpRecover"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //魔力恢复量
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.mpRecover,
                afterPlayerAttribute.mpRecover, TR.Value("player_attribute_format_mpRecover"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //体力
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.baseSta,
                afterPlayerAttribute.baseSta, TR.Value("player_attribute_format_baseSta"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //智力
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.baseInt,
                afterPlayerAttribute.baseInt, TR.Value("player_attribute_format_baseInt"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //固定攻击
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.baseIndependence,
                afterPlayerAttribute.baseIndependence, TR.Value("player_attribute_format_baseIndependence"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //精神
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.baseSpr,
                afterPlayerAttribute.baseSpr, TR.Value("player_attribute_format_baseSpr"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //物理攻击
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.attack,
                afterPlayerAttribute.attack, TR.Value("player_attribute_format_attack"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //魔法攻击
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.magicAttack,
                afterPlayerAttribute.magicAttack, TR.Value("player_attribute_format_magicAttack"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //物理防御
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.defence,
                afterPlayerAttribute.defence, TR.Value("player_attribute_format_defence"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //魔法防御
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.magicDefence,
                afterPlayerAttribute.magicDefence, TR.Value("player_attribute_format_magicDefence"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //攻速
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.attackSpeed,
                afterPlayerAttribute.attackSpeed, TR.Value("player_attribute_format_attackSpeed"),true);
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //施法速度
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.spellSpeed,
                afterPlayerAttribute.spellSpeed, TR.Value("player_attribute_format_spellSpeed"),true);
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //移速
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.moveSpeed,
                afterPlayerAttribute.moveSpeed, TR.Value("player_attribute_format_moveSpeed"),true);
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //物爆
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.ciriticalAttack,
                afterPlayerAttribute.ciriticalAttack, 
                TR.Value("player_attribute_format_ciriticalAttack"),
                true);
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //魔爆
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.ciriticalMagicAttack,
                afterPlayerAttribute.ciriticalMagicAttack,
                TR.Value("player_attribute_format_ciriticalMagicAttack"),
                true);
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //命中
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.dex,
                afterPlayerAttribute.dex, TR.Value("player_attribute_format_dex"),
                true);
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //闪避
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.dodge,
                afterPlayerAttribute.dodge, TR.Value("player_attribute_format_dodge")
                ,true);
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //僵值
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.frozen,
                afterPlayerAttribute.frozen, TR.Value("player_attribute_format_frozen"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //硬值
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.hard,
                afterPlayerAttribute.hard, TR.Value("player_attribute_format_hard"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //侵蚀抗性
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.resistMagic,
                afterPlayerAttribute.resistMagic, TR.Value("player_attribute_format_resistMagic"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //光属性强化
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.lightAttack,
                afterPlayerAttribute.lightAttack, TR.Value("player_attribute_format_lightAttack"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //火属性强化
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.fireAttack,
                afterPlayerAttribute.fireAttack, TR.Value("player_attribute_format_fireAttack"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //冰属性强化
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.iceAttack,
                afterPlayerAttribute.iceAttack, TR.Value("player_attribute_format_iceAttack"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //暗属性强化
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.darkAttack,
                afterPlayerAttribute.darkAttack, TR.Value("player_attribute_format_darkAttack"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //光属性抗性
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.lightDefence,
                afterPlayerAttribute.lightDefence, TR.Value("player_attribute_format_lightDefence"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //火属性抗性
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.fireDefence,
                afterPlayerAttribute.fireDefence, TR.Value("player_attribute_format_fireDefence"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //冰属性抗性
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.iceDefence,
                afterPlayerAttribute.iceDefence, TR.Value("player_attribute_format_iceDefence"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //暗属性抗性
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.darkDefence,
                afterPlayerAttribute.darkDefence, TR.Value("player_attribute_format_darkDefence"));
            if (betterEquipmentData.DataState != EquipmentDataState.PROPERTY_NO_CHANGE)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }
            #endregion

            return betterEquipmentDataList;
        }

        //前，后
        //得到属性增加列表
        public static List<BetterEquipmentData> GetPlayerAttributeDisplayChangeUpList(
            DisplayAttribute beforePlayerAttribute,
            DisplayAttribute afterPlayerAttribute)
        {
            if (afterPlayerAttribute == null
                || beforePlayerAttribute == null)
                return null;

            var betterEquipmentDataList = new List<BetterEquipmentData>();

            #region DisplayAttribute
            //生命
            var betterEquipmentData = CreateBetterData(beforePlayerAttribute.maxHp,
                afterPlayerAttribute.maxHp, TR.Value("player_attribute_format_maxHp"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //魔力
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.maxMp,
                afterPlayerAttribute.maxMp, TR.Value("player_attribute_format_maxMp"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //生命恢复量
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.hpRecover,
                afterPlayerAttribute.hpRecover, TR.Value("player_attribute_format_hpRecover"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //魔力恢复量
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.mpRecover,
                afterPlayerAttribute.mpRecover, TR.Value("player_attribute_format_mpRecover"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //体力
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.baseSta,
                afterPlayerAttribute.baseSta, TR.Value("player_attribute_format_baseSta"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //智力
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.baseInt,
                afterPlayerAttribute.baseInt, TR.Value("player_attribute_format_baseInt"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //固定攻击
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.baseIndependence,
                afterPlayerAttribute.baseIndependence, TR.Value("player_attribute_format_baseIndependence"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //精神
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.baseSpr,
                afterPlayerAttribute.baseSpr, TR.Value("player_attribute_format_baseSpr"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //物理攻击
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.attack,
                afterPlayerAttribute.attack, TR.Value("player_attribute_format_attack"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //魔法攻击
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.magicAttack,
                afterPlayerAttribute.magicAttack, TR.Value("player_attribute_format_magicAttack"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //物理防御
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.defence,
                afterPlayerAttribute.defence, TR.Value("player_attribute_format_defence"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //魔法防御
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.magicDefence,
                afterPlayerAttribute.magicDefence, TR.Value("player_attribute_format_magicDefence"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //攻速
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.attackSpeed,
                afterPlayerAttribute.attackSpeed, TR.Value("player_attribute_format_attackSpeed"), true);
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //施法速度
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.spellSpeed,
                afterPlayerAttribute.spellSpeed, TR.Value("player_attribute_format_spellSpeed"), true);
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //移速
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.moveSpeed,
                afterPlayerAttribute.moveSpeed, TR.Value("player_attribute_format_moveSpeed"), true);
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //物爆
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.ciriticalAttack,
                afterPlayerAttribute.ciriticalAttack,
                TR.Value("player_attribute_format_ciriticalAttack"),
                true);
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //魔爆
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.ciriticalMagicAttack,
                afterPlayerAttribute.ciriticalMagicAttack,
                TR.Value("player_attribute_format_ciriticalMagicAttack"),
                true);
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //命中
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.dex,
                afterPlayerAttribute.dex, TR.Value("player_attribute_format_dex"),
                true);
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //闪避
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.dodge,
                afterPlayerAttribute.dodge, TR.Value("player_attribute_format_dodge")
                , true);
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //僵值
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.frozen,
                afterPlayerAttribute.frozen, TR.Value("player_attribute_format_frozen"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //硬值
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.hard,
                afterPlayerAttribute.hard, TR.Value("player_attribute_format_hard"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //侵蚀抗性
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.resistMagic,
                afterPlayerAttribute.resistMagic, TR.Value("player_attribute_format_resistMagic"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //光属性强化
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.lightAttack,
                afterPlayerAttribute.lightAttack, TR.Value("player_attribute_format_lightAttack"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //火属性强化
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.fireAttack,
                afterPlayerAttribute.fireAttack, TR.Value("player_attribute_format_fireAttack"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //冰属性强化
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.iceAttack,
                afterPlayerAttribute.iceAttack, TR.Value("player_attribute_format_iceAttack"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //暗属性强化
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.darkAttack,
                afterPlayerAttribute.darkAttack, TR.Value("player_attribute_format_darkAttack"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //光属性抗性
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.lightDefence,
                afterPlayerAttribute.lightDefence, TR.Value("player_attribute_format_lightDefence"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //火属性抗性
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.fireDefence,
                afterPlayerAttribute.fireDefence, TR.Value("player_attribute_format_fireDefence"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //冰属性抗性
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.iceDefence,
                afterPlayerAttribute.iceDefence, TR.Value("player_attribute_format_iceDefence"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }

            //暗属性抗性
            betterEquipmentData = CreateBetterData(beforePlayerAttribute.darkDefence,
                afterPlayerAttribute.darkDefence, TR.Value("player_attribute_format_darkDefence"));
            if (betterEquipmentData.DataState == EquipmentDataState.PROPERTY_UP)
            {
                betterEquipmentDataList.Add(betterEquipmentData);
            }
            #endregion

            return betterEquipmentDataList;
        }


        private static BetterEquipmentData CreateBetterData(float beforeData, float afterData,
            string name,
            bool isPercentNumber = false)
        {
            BetterEquipmentData betterEquipData = new BetterEquipmentData();
            betterEquipData.DataState = EquipmentDataState.PROPERTY_NO_CHANGE;

            if (Math.Abs(afterData - beforeData) > 0.1)
            {
                betterEquipData.name = name;
                if (afterData > beforeData)
                {
                    betterEquipData.DataState = EquipmentDataState.PROPERTY_UP;
                }
                else
                {
                    betterEquipData.DataState = EquipmentDataState.PROPERTY_DOWN;
                }

                if (isPercentNumber == true)
                {
                    betterEquipData.PreData = string.Format("{0:F1}%", beforeData);
                    betterEquipData.CurData = string.Format("{0:F1}%", afterData);
                }
                else
                {
                    betterEquipData.PreData = string.Format("{0:F1}", beforeData);
                    betterEquipData.CurData = string.Format("{0:F1}", afterData);
                }
            }

            return betterEquipData;

        }

        #region ItemGuidListSort
        //对道具ItemGuidList进行排序，主要针对角色仓库中某一页的道具
        public static void ArrangeItemGuidList(List<ulong> itemGuidList)
        {
            if (itemGuidList == null || itemGuidList.Count <= 0)
                return;

            itemGuidList.Sort((x, y) =>
            {
                ItemData dataX = ItemDataManager.GetInstance().GetItem(x);
                ItemData dataY = ItemDataManager.GetInstance().GetItem(y);
                if (dataX == null || dataY == null)
                    return 0;

                if (dataX.isInSidePack && !dataY.isInSidePack)
                    return -1;
                if (!dataX.isInSidePack && dataY.isInSidePack)
                    return 1;

                int comp = dataY.Quality.CompareTo(dataX.Quality);
                if (comp == 0)
                {
                    comp = dataY.TableID.CompareTo(dataX.TableID);
                    if (comp == 0)
                    {
                        comp = dataY.Count.CompareTo(dataX.Count);
                        //强化等级按照从小到大进行排序
                        if (comp == 0)
                        {
                            comp = dataX.StrengthenLevel.CompareTo(dataY.StrengthenLevel);
                        }
                    }
                }
                return comp;
            });
        }
        #endregion

        #region ItemTipModel

        //道具的Tip上是否展示Avatar模型
        public static bool IsItemTipShowModelAvatar(ItemData itemData)
        {
            if (itemData == null)
                return false;

            var isNeedShowModelAvatarByItemTypeAndSubType = IsNeedShowModelAvatarByItemTypeAndSubType(itemData.TableData);
            if (isNeedShowModelAvatarByItemTypeAndSubType == false)
                return false;

            bool isNeedShowModelAvatarByItemGuid = IsNeedShowModelAvatarByItemGuid(itemData);
            if (isNeedShowModelAvatarByItemGuid == false)
                return false;

            return true;
        }


        //根据道具的子类型判断道具的ItemTip上是否可以展示model
        public static bool IsNeedShowModelAvatarByItemTypeAndSubType(ItemTable itemTable)
        {

            //道具类型：头像框(12),不显示
            //道具类型：虚拟礼包（9）
            //道具子类型：武器（1）；称号（10）；时装翅膀（11）；时装头饰（12）；时装腰饰（13）
            //时装衣服（14）；时装裤子（15）；时装胸饰（16）；时装武器装扮（81）；时装光环（92）；
            //宠物蛋（44）；礼包（29）
            
            if (itemTable == null)
                return false;

            var itemType = itemTable.Type;
            //道具类型：头像框不显示
            if (itemType == ItemTable.eType.HEAD_FRAME)
                return false;

            //道具类型：虚拟礼包，判断是否需要展示
            if (itemType == ItemTable.eType.VirtualPack)
            {
                var isNeedShowTipModelByGiftData = IsNeedShowModelAvatarByGiftData(itemTable.PackageID);
                return isNeedShowTipModelByGiftData;
            }

            //判断子类型
            var subType = itemTable.SubType;
            switch (subType)
            {
                case ItemTable.eSubType.WEAPON:                         //武器
                case ItemTable.eSubType.FASHION_HAIR:                   //时装翅膀
                case ItemTable.eSubType.FASHION_HEAD:                   //时装头饰
                case ItemTable.eSubType.FASHION_SASH:                   //时装腰饰
                case ItemTable.eSubType.FASHION_CHEST:                  //时装衣服
                case ItemTable.eSubType.FASHION_LEG:                    //时装裤子
                case ItemTable.eSubType.FASHION_EPAULET:                //时装胸饰
                case ItemTable.eSubType.FASHION_WEAPON:                 //时装武器装扮
                case ItemTable.eSubType.FASHION_AURAS:                  //时装光环
                    //身上的部位，判断是否存在资源
                    var isOwnerResModel = IsItemTableOwnerResModel(itemTable);
                    return isOwnerResModel;
                case ItemTable.eSubType.TITLE:                          //称号
                    return true;
                case ItemTable.eSubType.PetEgg:                         //宠物蛋
                    return true;
                case ItemTable.eSubType.GiftPackage:                    //礼包
                    var isNeedShowTipModelByGiftData = IsNeedShowModelAvatarByGiftData(itemTable.PackageID);
                    return isNeedShowTipModelByGiftData;
                default:
                    return false;
            }
        }

        //礼包类型是否显示道具模型
        private static bool IsNeedShowModelAvatarByGiftData(int giftPackId)
        {
            //礼包不存在
            var giftPackTable = TableManager.GetInstance().GetTableItem<GiftPackTable>(giftPackId);
            if (giftPackTable == null)
                return false;

            //礼包的展示类型为none
            if (giftPackTable.ShowAvatarModelType == GiftPackTable.eShowAvatarModelType.None)
                return false;

            return true;
        }

        //根据道具的Guid，判断道具的ItemTip上是否可以展示model
        public static bool IsNeedShowModelAvatarByItemGuid(ItemData itemData)
        {
            if (itemData == null)
                return false;

            //假道具（角色没有获得的道具），展示
            if (itemData.GUID <= 0)
                return true;

            //如果是宠物，礼包类型，都可以展示
            if (itemData.TableData != null)
            {
                //虚拟礼包
                if (itemData.TableData.Type == ItemTable.eType.VirtualPack)
                    return true;

                //宠物和礼包
                if (itemData.TableData.SubType == ItemTable.eSubType.PetEgg
                    || itemData.TableData.SubType == ItemTable.eSubType.GiftPackage)
                    return true;
            }

            //角色获得过
            var item = ItemDataManager.GetInstance().GetItem(itemData.GUID);
            if (item != null)
                return false;

            return true;
        }

        //这个道具是否存在职业适配
        //角色是基础职业，只能适配0和基础职业；
        //角色是小职业，可以适配0，基础职业和小职业
        //如果职业不匹配的话，保存其他职业的Id
        public static bool IsSuitPlayerProfession(ItemTable itemTable, out int otherProfessionId)
        {
            otherProfessionId = 0;
            //null，返回false
            if (itemTable == null)
                return false;

            var suitProfessionIdList = itemTable.Occu.ToList();
            if (suitProfessionIdList == null || suitProfessionIdList.Count <= 0)
                return true;

            //是否为小职业
            var isAlreadyChangeProfession = false;
            var basePlayerProfessionId = 0;

            //当前的职业
            var currentPlayerProfessionId = PlayerBaseData.GetInstance().JobTableID;
            var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(currentPlayerProfessionId);
            if (jobTable != null)
            {
                if (jobTable.JobType == 1)
                {
                    //当前为小职业，标志位设置为true，获得基础职业
                    isAlreadyChangeProfession = true;
                    basePlayerProfessionId = jobTable.prejob;
                }
            }

            //遍历道具的适配职业
            for (var i = 0; i < suitProfessionIdList.Count; i++)
            {
                var suitProfessionId = suitProfessionIdList[i];
                //全职业适配
                if (suitProfessionId == 0)
                    return true;

                if (isAlreadyChangeProfession == true)
                {
                    //当前角色为小职业，道具的适配职业为小职业或者对应的基础职业都可以
                    if (suitProfessionId == currentPlayerProfessionId
                        || suitProfessionId == basePlayerProfessionId)
                        return true;
                }
                else
                {
                    //当前角色为基础职业,道具的适配职业只能是基础职业才可以
                    if (suitProfessionId == currentPlayerProfessionId)
                        return true;
                }
            }

            //职业不匹配
            for (var i = 0; i < suitProfessionIdList.Count; i++)
            {
                var currentProfessionId = suitProfessionIdList[i];
                if(currentProfessionId <= 0)
                    continue;

                //获得第一个其他职业的Id
                otherProfessionId = currentProfessionId;
                //如果是小职业，获得基础职业
                var currentJobTable = TableManager.GetInstance().GetTableItem<JobTable>(currentProfessionId);
                if (currentJobTable.JobType == 1)
                    otherProfessionId = currentJobTable.prejob;

                break;
            }

            return false;
        }

        #endregion

        #region ItemTipGiftPack

        //分开预览礼包
        private static List<ItemTable> GetFinalGiftPackShowGiftPackList(GiftPackTable giftPackTable)
        {
            if (giftPackTable == null)
                return null;

            var giftIdList = TableManager.GetInstance().GetGiftTableData(giftPackTable.ID);
            if (giftIdList == null || giftIdList.Count <= 0)
                return null;

            List<ItemTable> itemTableList = new List<ItemTable>();

            for (var i = 0; i < giftIdList.Count; i++)
            {
                var giftTable = giftIdList[i];
                //礼品为null
                if(giftTable == null)
                    continue;

                //是否适配职业
                bool isGiftTableSuitProfession = IsGiftTableSuitProfession(giftTable);
                if(isGiftTableSuitProfession == false)
                    continue;

                //礼品对应的道具
                var giftItemId = giftTable.ItemID;
                var giftItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(giftItemId);
                if(giftItemTable == null)
                    continue;

                //对应的道具不是礼包
                if(giftItemTable.SubType != ItemTable.eSubType.GiftPackage
                   && giftItemTable.Type != ItemTable.eType.VirtualPack)
                    continue;

                //对应道具的礼包
                var currentGiftPackTable =
                    TableManager.GetInstance().GetTableItem<GiftPackTable>(giftItemTable.PackageID);
                if(currentGiftPackTable == null)
                    continue;

                //对应道具是礼包，礼包的展示类型为：组合预览，整套预览，单一预览
                //其他类型不展示
                var currentShowAvatarModelType = currentGiftPackTable.ShowAvatarModelType;
                if (currentShowAvatarModelType == GiftPackTable.eShowAvatarModelType.Combination
                    || currentShowAvatarModelType == GiftPackTable.eShowAvatarModelType.Complete
                    || currentShowAvatarModelType == GiftPackTable.eShowAvatarModelType.Single)
                {
                    itemTableList.Add(giftItemTable);
                }
            }

            return itemTableList;
        }

        //获得礼包需要展示的数据
        public static GiftPackModelAvatarDataModel GetGiftPackModelAvatarDataModel(int giftPackId)
        {
            //礼包不存在
            var giftPackTable = TableManager.GetInstance().GetTableItem<GiftPackTable>(giftPackId);
            if (giftPackTable == null)
                return null;

            //展示类型 不满足
            var giftPackShowAvatarModelType = giftPackTable.ShowAvatarModelType;
            if (giftPackShowAvatarModelType == GiftPackTable.eShowAvatarModelType.None)
                return null;

            //特殊预览类型：分开预览礼包
            if (giftPackShowAvatarModelType == GiftPackTable.eShowAvatarModelType.CompleteEnumeration)
            {
                var showGiftPackList = GetFinalGiftPackShowGiftPackList(giftPackTable);
                if (showGiftPackList == null || showGiftPackList.Count <= 0)
                    return null;

                var giftPackModelAvatarDataModelWithGiftPack = new GiftPackModelAvatarDataModel()
                {
                    GiftPackShowModelAvatarType = giftPackShowAvatarModelType,
                    GiftPackShowItemTableList = showGiftPackList,
                };

                return giftPackModelAvatarDataModelWithGiftPack;
            }

            bool isOwnerCompleteShowType = false;
            //得到礼包下面可以展示的物品
            var giftPackShowItemTableList = GetFinalGiftPackShowItemTableList(giftPackId,
                giftPackShowAvatarModelType,
                out isOwnerCompleteShowType);
            //礼包下面没有可以展示的道具
            if (giftPackShowItemTableList == null || giftPackShowItemTableList.Count <= 0)
                return null;

            var itemTableListCount = giftPackShowItemTableList.Count;
            //更新礼包展示的类型
            if (itemTableListCount == 1)
            {
                giftPackShowAvatarModelType = GiftPackTable.eShowAvatarModelType.Single;
            }
            else
            {
                //组合+宠物预览，如果最后一个不是宠物蛋，则类型变为组合预览
                if (giftPackShowAvatarModelType == GiftPackTable.eShowAvatarModelType.Matching)
                {
                    var finalItemTable = giftPackShowItemTableList[itemTableListCount - 1];
                    if (finalItemTable == null || finalItemTable.SubType != ItemTable.eSubType.PetEgg)
                        giftPackShowAvatarModelType = GiftPackTable.eShowAvatarModelType.Combination;
                }
            }

            var giftPackModelAvatarDataModel = new GiftPackModelAvatarDataModel
            {
                GiftPackShowModelAvatarType = giftPackShowAvatarModelType,
                GiftPackShowItemTableList = giftPackShowItemTableList,
                IsOwnerCompleteShowType = isOwnerCompleteShowType,
            };

            return giftPackModelAvatarDataModel;
        }

        //得到礼包需要展示的道具
        public static List<ItemTable> GetFinalGiftPackShowItemTableList(int giftPackId,
            GiftPackTable.eShowAvatarModelType showAvatarModelType,
            out bool isOwnerCompleteShowType)
        {
            isOwnerCompleteShowType = false;

            if (giftPackId <= 0)
                return null;

            bool currentOwnerCompleteShowType = false;
            //找到这个礼包（包括二级礼包）下面所有符合条件的道具
            var originalItemTableList = GetOriginalGiftPackShowItemTableList(giftPackId, true,
                out currentOwnerCompleteShowType);
            if (currentOwnerCompleteShowType == true)
                isOwnerCompleteShowType = true;
            
            //过滤掉重复的道具
            var filterItemTableList = GetFilterGiftPackShowItemTableList(originalItemTableList,
                showAvatarModelType);

            return filterItemTableList;
        }

        //得到原始的数据
        //如果礼包中包含礼包，只查找下一层
        private static List<ItemTable> GetOriginalGiftPackShowItemTableList(int giftPackId,
            bool isFindNextLayer,
            out bool isOwnerCompleteShowType)
        {
            isOwnerCompleteShowType = false;
            var giftPackTable = TableManager.GetInstance().GetTableItem<GiftPackTable>(giftPackId);
            if (giftPackTable == null)
                return null;
            
            var giftDataList = TableManager.GetInstance().GetGiftTableData(giftPackTable.ID);
            if (giftDataList == null || giftDataList.Count <= 0)
                return null;

            //当前礼包是整套预览
            if (giftPackTable.ShowAvatarModelType == GiftPackTable.eShowAvatarModelType.Complete)
                isOwnerCompleteShowType = true;

            List<ItemTable> itemTableList = new List<ItemTable>();
            //礼包中的礼品列表
            for (var i = 0; i < giftDataList.Count; i++)
            {
                //礼包中具体的某个道具
                var giftTable = giftDataList[i];
                if (giftTable == null)
                    continue;

                var giftTableRecommendJobs = giftTable.RecommendJobs.ToList();
                //判断当前礼物的推荐职业
                if (giftTableRecommendJobs != null && giftTableRecommendJobs.Count > 0)
                {
                    //不包含通用职业和当前职业
                    if (giftTableRecommendJobs.Contains(0) == false
                       && giftTableRecommendJobs.Contains(PlayerBaseData.GetInstance().JobTableID) == false)
                        continue;
                }

                //礼品对应的道具ID
                var giftItemId = giftTable.ItemID;
                var giftItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(giftItemId);
                if(giftItemTable == null)
                    continue;

                //礼品对应的道具类型是头像框类型，直接过滤
                if(giftItemTable.Type == ItemTable.eType.HEAD_FRAME)
                    continue;

                //礼包中包含一个礼包或者虚拟礼包
                //在当前的这个礼包中查找
                if (giftItemTable.SubType == ItemTable.eSubType.GiftPackage ||
                    giftItemTable.Type == ItemTable.eType.VirtualPack)
                {
                    //不再进行查找下一层，直接返回
                    if (isFindNextLayer == false)
                    {
                        continue;
                    }
                    else
                    {
                        var nextLayerIsOwnerCompleteShowType = false;
                        //下一层查找
                        var findItemTableList = GetOriginalGiftPackShowItemTableList(giftItemTable.PackageID,
                            false,
                            out nextLayerIsOwnerCompleteShowType);
                        if (findItemTableList != null && findItemTableList.Count > 0)
                        {
                            itemTableList.AddRange(findItemTableList.ToList());
                            //下面一层包含整套预览
                            if (nextLayerIsOwnerCompleteShowType == true)
                                isOwnerCompleteShowType = true;
                        }
                    }
                }
                else
                {
                    //子类型过滤
                    var isSuitItemSubType = IsSuitItemSubTypeInGiftPack(giftItemTable);
                    if (isSuitItemSubType == false)
                        continue;

                    var otherProfessionId = 0;
                    //职业过滤
                    var isSuitPlayerProfession = IsSuitPlayerProfession(giftItemTable, out otherProfessionId);
                    if (isSuitPlayerProfession == false)
                        continue;

                    itemTableList.Add(giftItemTable);
                }
            }
            return itemTableList;
        }


        //得到过滤掉重复的资源
        private static List<ItemTable> GetFilterGiftPackShowItemTableList(List<ItemTable> itemTableList,
            GiftPackTable.eShowAvatarModelType showAvatarModelType)
        {
            if (itemTableList == null || itemTableList.Count <= 0)
                return null;

            List<ItemTable> filterItemTableList = new List<ItemTable>();

            //过滤掉资源重复
            Dictionary<int, ItemTable> petDictionary = new Dictionary<int, ItemTable>();
            Dictionary<int, ItemTable> titleDictionary = new Dictionary<int, ItemTable>();
            Dictionary<int, ItemTable> resModelDictionary = new Dictionary<int, ItemTable>();
            //称号和宠物只取第一个
            ItemTable firstTitleItemTable = null;
            ItemTable firstPetItemTable = null;

            //遍历道具：过滤掉资源重复的道具
            //宠物蛋道具，一个道具Id，对应一个宠物蛋
            //称号道具：一个道具Id，对应一个称号Id
            //其他道具：一个资源Id，对应一个道具
            for (var i = 0; i < itemTableList.Count; i++)
            {
                var itemTable = itemTableList[i];
                if (itemTable == null)
                    continue;

                if (itemTable.SubType == ItemTable.eSubType.PetEgg)
                {
                    //一个道具Id对应一个宠物蛋
                    //宠物蛋没有包含
                    if (petDictionary.ContainsKey(itemTable.ID) == false)
                    {
                        petDictionary[itemTable.ID] = itemTable;
                        filterItemTableList.Add(itemTable);

                        //保留一个宠物蛋
                        if (firstPetItemTable == null)
                            firstPetItemTable = itemTable;
                    }
                }
                else if (itemTable.SubType == ItemTable.eSubType.TITLE)
                {
                    //一个道具Id对应一个称号
                    //称号已经包含
                    if (titleDictionary.ContainsKey(itemTable.ID) == false)
                    {
                        titleDictionary[itemTable.ID] = itemTable;
                        filterItemTableList.Add(itemTable);

                        //保留一个称号
                        if (firstTitleItemTable == null)
                            firstTitleItemTable = itemTable;
                    }
                }
                else
                {
                    //判断礼包中的道具是否存在资源（存在模型)
                    var isItemTableOwnerResModel = IsItemTableOwnerResModel(itemTable);
                    if(isItemTableOwnerResModel == false)
                        continue;
                    
                    //资源Id没有包含
                    if (resModelDictionary.ContainsKey(itemTable.ResID) == false)
                    {
                        resModelDictionary[itemTable.ResID] = itemTable;
                        filterItemTableList.Add(itemTable);
                    }
                }
            }

            //分开预览，只需要过滤掉重复资源的Id，之后一个个的预览

            if (showAvatarModelType == GiftPackTable.eShowAvatarModelType.Enumeration)
                return filterItemTableList;


            //其他形式：单一预览，组合预览，或者组合+宠物预览
            //宠物蛋如果存在只取一个；称号如果存在也只取一个；
            //对武器，时装等SubType进行过滤，每个部位只取一个；
            filterItemTableList.Clear();

            //对同一个部位的道具进行过滤，每个部位只取一个
            Dictionary<int, ItemTable> itemSubTypeDictionary = new Dictionary<int, ItemTable>();
            var resModelEnumerator = resModelDictionary.GetEnumerator();
            while (resModelEnumerator.MoveNext())
            {
                var itemTable = resModelEnumerator.Current.Value;
                if(itemTable == null)
                    continue;

                if (itemSubTypeDictionary.ContainsKey((int) itemTable.SubType) == false)
                {
                    itemSubTypeDictionary[(int) itemTable.SubType] = itemTable;
                }
            }

            //武器，时装部位等类型的道具添加
            var itemSubTypeEnumerator = itemSubTypeDictionary.GetEnumerator();
            while (itemSubTypeEnumerator.MoveNext())
            {
                var itemTable = itemSubTypeEnumerator.Current.Value;
                if (itemTable == null)
                    continue;

                filterItemTableList.Add(itemTable);
            }

            //添加一个存在的称号
            if (firstTitleItemTable != null)
                filterItemTableList.Add(firstTitleItemTable);
            //最后添加一个存在的宠物蛋
            if (firstPetItemTable != null)
                filterItemTableList.Add(firstPetItemTable);

            return filterItemTableList;
        }


        //礼包中的道具是否满足道具的子类型
        public static bool IsSuitItemSubTypeInGiftPack(ItemTable itemTable)
        {
            if (itemTable == null)
                return false;

            var subType = itemTable.SubType;
            switch (subType)
            {
                case ItemTable.eSubType.WEAPON:                         //武器
                    return true;
                case ItemTable.eSubType.TITLE:                          //称号
                    return true;
                case ItemTable.eSubType.FASHION_HAIR:                   //时装翅膀
                    return true;
                case ItemTable.eSubType.FASHION_HEAD:                   //时装头饰
                    return true;
                case ItemTable.eSubType.FASHION_SASH:                   //时装腰饰
                    return true;
                case ItemTable.eSubType.FASHION_CHEST:                  //时装衣服
                    return true;
                case ItemTable.eSubType.FASHION_LEG:                    //时装裤子
                    return true;
                case ItemTable.eSubType.FASHION_EPAULET:                //时装胸饰
                    return true;
                case ItemTable.eSubType.FASHION_WEAPON:                 //时装武器装扮
                    return true;
                case ItemTable.eSubType.FASHION_AURAS:                  //时装光环
                    return true;
                case ItemTable.eSubType.PetEgg:                         //宠物蛋
                    return true;
                default:
                    return false;
            }
        }

        //礼包是否适配职业
        private static bool IsGiftTableSuitProfession(GiftTable giftTable)
        {
            if (giftTable == null)
                return false;

            var giftTableRecommendJobs = giftTable.RecommendJobs.ToList();
            if (giftTableRecommendJobs != null && giftTableRecommendJobs.Count > 0)
            {
                //通用职业
                if (giftTableRecommendJobs.Contains(0) == true)
                    return true;

                //角色职业
                if (giftTableRecommendJobs.Contains(PlayerBaseData.GetInstance().JobTableID) == true)
                    return true;
            }

            return false;
        }
        #endregion

        //判断这个道具是否存在资源，主要是人物身上穿着的资源
        public static bool IsItemTableOwnerResModel(ItemTable itemTable)
        {
            if (itemTable == null)
                return false;

            var resTable = TableManager.GetInstance().GetTableItem<ResTable>(itemTable.ResID);
            if (resTable == null)
                return false;

            return true;
        }

        //根据道具表中的道具Id，获得宠物蛋的宠物Id
        public static int GetPetIdByItemTable(ItemTable itemTable)
        {
            if (itemTable == null)
                return 0;

            var petTables = TableManager.GetInstance().GetTable<PetTable>();
            var petTablesEnumerator = petTables.GetEnumerator();

            while (petTablesEnumerator.MoveNext())
            {
                var curPetTable = petTablesEnumerator.Current.Value as PetTable;
                if (curPetTable == null)
                    continue;

                if (curPetTable.PetEggID != itemTable.ID)
                    continue;

                //得到宠物蛋
                return curPetTable.ID;
            }

            return 0;
        }

        /// <summary>
        ///  /// <summary>
        /// 得到礼包道具集合
        /// </summary>
        /// <param name="jobId">适合自己的职业</param>
        /// <returns></returns>
        /// </summary>
        /// <param name="giftId"></param>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public static List<ItemData> GetGiftItemDataList(int giftId,int jobId)
        {
            List<ItemData> list = new List<ItemData>();

            var giftPackTable = TableManager.GetInstance().GetTableItem<GiftPackTable>(giftId);
            if (giftPackTable != null)
            {
                var giftDataList = TableManager.GetInstance().GetGiftTableData(giftPackTable.ID);
                for (int i = 0; i < giftDataList.Count; i++)
                {
                    var giftTable = giftDataList[i];
                    
                    if (giftTable == null)
                    {
                        continue;
                    }

                    if (!giftTable.RecommendJobs.Contains(jobId))
                    {
                        continue;
                    }

                    ItemData item = ItemDataManager.CreateItemDataFromTable(giftTable.ItemID);
                    item.EquipType = (EEquipType)giftTable.EquipType;
                    item.StrengthenLevel = giftTable.Strengthen;

                    list.Add(item);
                }
            }

            return list;
        }
    }
}
