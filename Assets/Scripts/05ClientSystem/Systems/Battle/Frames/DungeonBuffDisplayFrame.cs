using ProtoTable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace GameClient
{
    public class BuffInfoTip
    {
        public string IconPath;
        public string Name;
        public string LeftTime;
        public int[] attriArray;
        public string Detail;
        public int buffID;
        public int sortOrder;
    }

    public class HeadBuffMiniIconData
    {
        //public BeBuff bebuff;
        public int buffPid;
        public BuffTable tableData;
    }

    public class DungeonBuffDisplayFrame
    {
        public List<HeadBuffMiniIconData> mBuffList = new List<HeadBuffMiniIconData>();

        private GameObject mInfoRoot;
        //private BeActor player;
        private byte seat = byte.MaxValue;
        private ComBuffIcon[] mBuffIconGameObjects;

        private bool updateBuffTipLock = false;
        public List<BuffInfoTip> mBuffTip = new List<BuffInfoTip>();

        #region BUFF_TIPS_PATH
        protected static List<string> AttriName = new List<string>(new string[]
        {
            "生命值{0}",
            "魔力值{0}",
            "力量{0}",
            "智力{0}",
            "体力{0}",
            "精神{0}",
            "物理攻击力{0}",
            "魔法攻击力{0}",
            "物理防御{0}",
            "魔法防御{0}",
            "物理暴击{0}%",
            "魔法暴击{0}%",
            "攻击速度{0}%",
            "释放速度{0}%",
            "移动速度{0}%",
            "闪避{0}%",
            "命中{0}%",
            "HP回复{0}",
            "MP回复{0}",
            "硬直{0}",
            "增加伤害{0}%",
            "物理攻击{0}%",
            "魔法攻击{0}%",
        });

        #endregion
        public bool IsInited
        {
            get { return isInited; }
        }
        private bool isInited = false;

        public void Init(GameObject root, byte _seat)
        {
            if (root == null)
            {
                return;
            }
            mInfoRoot = root;
            mBuffIconGameObjects = mInfoRoot.GetComponentsInChildren<ComBuffIcon>(true);
            this.seat = _seat;
            isInited = true;
        }

        public void DeInit()
        {
            seat = byte.MaxValue;
            mBuffIconGameObjects = null;
            mInfoRoot = null;
            isInited = false;
        }

        public void _addBuff(BeBuff addBuff, int infoTableId = 0)
        {
            if (addBuff == null && infoTableId == 0)
            {
                return;
            }
            var iconData = _findBuffUnit(addBuff == null ? infoTableId : addBuff.buffID);
            if (iconData != null && addBuff != null)//这段逻辑是为了表现相同idbuff被顶掉 
            {
                iconData.buffPid = addBuff.PID;
                return;
            }

            //var data = new BuffDisplayData(infoTableId);
            if (null == addBuff.buffData || addBuff.buffData.IconSortOrder <= -1)
            {
                return;
            }
            var buffUnit = addBuff == null ?
                new HeadBuffMiniIconData() { tableData = TableManager.GetInstance().GetTableItem<BuffTable>(infoTableId) } :
                new HeadBuffMiniIconData() { buffPid = addBuff.PID, tableData = addBuff.buffData };
            mBuffList.Add(buffUnit);

            mBuffList.Sort(_Sort);
            if (updateBuffTipLock)
            {
                AddBuffInfoData(buffUnit);
                mBuffTip.Sort(_SortBuffInfoTip);
            }
        }

        public void _removeBuff(BeBuff removeBuff, int infoTableId = 0)
        {
            HeadBuffMiniIconData unit = _findBuffUnit(removeBuff == null ? infoTableId : removeBuff.buffID);
            if (unit != null)
            {
                mBuffList.Remove(unit);
            }
            if (updateBuffTipLock)
            {
                BuffInfoTip tip = _findBuffTip(removeBuff.buffID);
                if (tip != null)
                {
                    mBuffTip.Remove(tip);
                    mBuffTip.Sort(_SortBuffInfoTip);
                }
            }
        }

        public void _updateBuff(float timeElapsed)
        {
            if (!IsInited || seat == byte.MaxValue)
            {
                return;
            }
            if (updateBuffTipLock)
            {
                for (int i = 0; i < mBuffList.Count; ++i)
                {
                    if (mBuffList[i] == null || mBuffList[i].tableData == null || mBuffList[i].tableData.IconSortOrder < 0)
                    {
                        continue;
                    }
                    var beBuff = GetBuffByPid(mBuffList[i].buffPid);//mBuffList[i].bebuff;
                    if (beBuff != null)
                    {
                        mBuffTip[i].LeftTime = beBuff.GetLeftTime() == -1 || beBuff.duration >= 3600000 ? foreverString : BeUtility.Format("{0}{1}", (beBuff.GetLeftTime() / 1000).ToString(), "秒");
                    }
                }
            }
            for (int i = 0; i < mBuffIconGameObjects.Length; ++i)
            {
                if (mBuffIconGameObjects[i] == null)
                {
                    continue;
                }
                if (i > mBuffList.Count - 1)
                {
                    mBuffIconGameObjects[i].ResetIcon();
                }
                else
                {
                    if (mBuffList.Count > mBuffIconGameObjects.Length && i == mBuffIconGameObjects.Length - 1)
                    {
                        mBuffIconGameObjects[i].SetOverFlowIconActive(true);
                        continue;
                    }
                    if (mBuffIconGameObjects[i].BeBuffPid != mBuffList[i].buffPid)
                    {
                        mBuffIconGameObjects[i].BeBuffPid = mBuffList[i].buffPid;
                        mBuffIconGameObjects[i].ActorSeat = this.seat;
                    }
                    if (mBuffIconGameObjects[i].BattleBuffTableData == null ||
                                        mBuffIconGameObjects[i].BattleBuffTableData.ID != mBuffList[i].tableData.ID)
                    {
                        mBuffIconGameObjects[i].BattleBuffTableData = mBuffList[i].tableData;
                        mBuffIconGameObjects[i].gameObject.SetActive(true);
                        mBuffIconGameObjects[i].MarkDirty();
                    }
                    if (mBuffIconGameObjects[i].BeBuffPid != 0)
                    {
                        mBuffIconGameObjects[i].MarkCountDirty();
                    }
                    mBuffIconGameObjects[i].SetOverFlowIconActive(false);
                }
            }
        }

        private bool isResistMagic(int buffid)
        {
            bool isResistMagicBuff = false;
            for (int i = 0; i < ResistMagicBuffID.Length; ++i)
            {
                if (buffid == ResistMagicBuffID[i])
                {
                    isResistMagicBuff = true;
                    break;
                }
            }
            return isResistMagicBuff;
        }

        private void AddBuffInfoData(HeadBuffMiniIconData icon)
        {
            int buffID = icon.tableData.ID;
            if (!isResistMagic(icon.tableData.ID))
            {
                _addBuffInfoData(icon);
            }
            else
            {
                _addBuffInfoData(buffID);
            }
        }

        public int GetValidBuffCount()
        {
            if (mBuffList != null)
            {
                return mBuffList.Count;
            }
            return 0;
        }

        public int GetBuffTipsCount()
        {
            if (mBuffTip != null)
            {
                return mBuffTip.Count;
            }
            return 0;
        }

        public BuffInfoTip GetBuffTipsByIndex(int index)
        {
            if (index >= 0 && index < mBuffTip.Count)
            {
                return mBuffTip[index];
            }
            else
            {
                return null;
            }
        }

        public void SetBuffTipListUpdate()
        {
            for (int i = 0; i < mBuffList.Count; ++i)
            {
                AddBuffInfoData(mBuffList[i]);
            }
            updateBuffTipLock = true;
        }

        public void CloseBuffTipListUpdate()
        {
            mBuffTip.Clear();
            updateBuffTipLock = false;
        }

        private HeadBuffMiniIconData _findBuffUnit(int buffID)
        {
            for (int i = 0; i < mBuffList.Count; ++i)
            {
                if (buffID == mBuffList[i].tableData.ID)
                {
                    return mBuffList[i];
                }
            }
            return null;
        }

        private BuffInfoTip _findBuffTip(int id)
        {
            for (int i = 0; i < mBuffTip.Count; ++i)
            {
                if (id == mBuffTip[i].buffID)
                {
                    return mBuffTip[i];
                }
            }
            return null;
        }

        private int _Sort(HeadBuffMiniIconData left, HeadBuffMiniIconData right)
        {
            if (left == null || right == null || left.tableData == null || right.tableData == null)
            {
                return 0;
            }
            if (left != null && right != null)
            {
                if (left.tableData.IconSortOrder == right.tableData.IconSortOrder)
                {
                    return left.tableData.ID - right.tableData.ID;
                }
            }
            else//他们俩之间或许有一个为空了,无意义直接返回0
            {
                return 0;
            }
            if (left.tableData.IconSortOrder == -1)
            {
                return 1;
            }
            else if (right.tableData.IconSortOrder == -1)
            {
                return -1;
            }
            else
            {
                return left.tableData.IconSortOrder - right.tableData.IconSortOrder;
            }
        }

        private int _SortBuffInfoTip(BuffInfoTip left, BuffInfoTip right)
        {
            if (left != null && right != null)
            {
                if (left.sortOrder == right.sortOrder)
                {
                    return left.buffID - right.buffID;
                }
            }
            else//他们俩之间或许有一个为空了,无意义直接返回0
            {
                return 0;
            }
            if (left.sortOrder == -1)
            {
                return 1;
            }
            else if (right.sortOrder == -1)
            {
                return -1;
            }
            else
            {
                return left.sortOrder - right.sortOrder;
            }
        }

        private int[] ResistMagicBuffID = new int[] { 820001, 820002, 820003 };

        private void _addBuffInfoData(HeadBuffMiniIconData unit)
        {
            var beBuff = GetBuffByPid(unit.buffPid);
            if (beBuff != null)
            {
                var buffInfoTip = new BuffInfoTip
                {
                    IconPath = beBuff.buffData.Icon,
                    Name = BeUtility.Format(beBuff.buffData.BuffDisName, beBuff.level),
                    LeftTime = string.Empty,
                    buffID = beBuff.buffID,
                    sortOrder = beBuff.buffData.IconSortOrder,
                };
                if (buffInfoTip.attriArray == null)
                {
                    buffInfoTip.attriArray = new int[AttriName.Count];
                }
                mBuffTip.Add(buffInfoTip);

                #region 正常buff
                //计算属性的情况
                if (beBuff.buffData.BuffType.Count != 0)
                {
                    var mechanismList = GamePool.ListPool<BeMechanism>.Get();
                    //机制影响的buff加成统计
                    beBuff.GetMechanismList(mechanismList);
                    if (mechanismList != null)
                    {
                        for (int i = 0; i < mechanismList.Count; ++i)
                        {
                            var mechanism = mechanismList[i] as Mechanism1017;
                            if (mechanism != null)
                            {
                                var gainArray = mechanism.GetRecoverValueArr();
                                if (gainArray == null)
                                    continue;
                                for (int j = 0; j < beBuff.buffData.BuffType.Count; ++j)
                                {
                                    switch (beBuff.buffData.BuffType[j])
                                    {
                                        case ProtoTable.BuffTable.eBuffType.HP_MAX:
                                            buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[j]] += gainArray[0];
                                            break;
                                        case ProtoTable.BuffTable.eBuffType.MP_MAX:
                                            buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[j]] += gainArray[1];
                                            break;
                                        case ProtoTable.BuffTable.eBuffType.BASE_ATK:
                                            buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[j]] += gainArray[4];
                                            break;
                                        case ProtoTable.BuffTable.eBuffType.BASE_INT:
                                            buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[j]] += gainArray[5];
                                            break;
                                        case ProtoTable.BuffTable.eBuffType.STA:
                                            break;
                                        case ProtoTable.BuffTable.eBuffType.SPR:
                                            break;
                                        case ProtoTable.BuffTable.eBuffType.ATTACK:
                                            buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[j]] += gainArray[8];
                                            break;
                                        case ProtoTable.BuffTable.eBuffType.MAGIC_ATTACK:
                                            buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[j]] += gainArray[9];
                                            break;
                                        case ProtoTable.BuffTable.eBuffType.DEFENCE:
                                            buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[j]] += gainArray[2];
                                            break;
                                        case ProtoTable.BuffTable.eBuffType.MAGIC_DEFENCE:
                                            buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[j]] += gainArray[3];
                                            break;
                                        case ProtoTable.BuffTable.eBuffType.CIRITICAL_ATTACK:
                                            break;
                                        case ProtoTable.BuffTable.eBuffType.CIRITICAL_MAGIC_ATTACK:
                                            break;
                                        case ProtoTable.BuffTable.eBuffType.ATTACK_SPEED:
                                            break;
                                        case ProtoTable.BuffTable.eBuffType.SPELL_SPEED:
                                            break;
                                        case ProtoTable.BuffTable.eBuffType.MOVE_SPEED:
                                            break;
                                        case ProtoTable.BuffTable.eBuffType.DODGE:
                                            break;
                                        case ProtoTable.BuffTable.eBuffType.DEX:
                                            break;
                                        case ProtoTable.BuffTable.eBuffType.HP_RECOVER:
                                            buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[j]] += gainArray[10];
                                            break;
                                        case ProtoTable.BuffTable.eBuffType.MP_RECOVER:
                                            buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[j]] += gainArray[11];
                                            break;
                                        case ProtoTable.BuffTable.eBuffType.HARD:
                                            break;
                                        case ProtoTable.BuffTable.eBuffType.ADD_DAMAGE_PERCENT:
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    
                    GamePool.ListPool<BeMechanism>.Release(mechanismList);
                    //buffData加成 如果有 /10 操作是千分制转化百分制
                    for (int i = 0; i < beBuff.buffData.BuffType.Count; ++i)
                    {
                        switch (beBuff.buffData.BuffType[i])
                        {
                            case ProtoTable.BuffTable.eBuffType.HP_MAX:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.maxHp, beBuff.level);
                                break;
                            case ProtoTable.BuffTable.eBuffType.MP_MAX:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.maxMp, beBuff.level);
                                break;
                            case ProtoTable.BuffTable.eBuffType.BASE_ATK:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.baseAtk, beBuff.level) / 1000;
                                break;
                            case ProtoTable.BuffTable.eBuffType.BASE_INT:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.baseInt, beBuff.level) / 1000;
                                break;
                            case ProtoTable.BuffTable.eBuffType.STA:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.sta, beBuff.level) / 1000;
                                break;
                            case ProtoTable.BuffTable.eBuffType.SPR:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.spr, beBuff.level) / 1000;
                                break;
                            case ProtoTable.BuffTable.eBuffType.ATTACK:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.attack, beBuff.level);
                                break;
                            case ProtoTable.BuffTable.eBuffType.MAGIC_ATTACK:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.magicAttack, beBuff.level);
                                break;
                            case ProtoTable.BuffTable.eBuffType.DEFENCE:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.defence, beBuff.level);
                                break;
                            case ProtoTable.BuffTable.eBuffType.MAGIC_DEFENCE:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.magicDefence, beBuff.level);
                                break;
                            case ProtoTable.BuffTable.eBuffType.CIRITICAL_ATTACK:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.ciriticalAttack, beBuff.level) / 10;
                                break;
                            case ProtoTable.BuffTable.eBuffType.CIRITICAL_MAGIC_ATTACK:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.ciriticalMagicAttack, beBuff.level) / 10;
                                break;
                            case ProtoTable.BuffTable.eBuffType.ATTACK_SPEED:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.attackSpeed, beBuff.level) / 10;
                                break;
                            case ProtoTable.BuffTable.eBuffType.SPELL_SPEED:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.spellSpeed, beBuff.level) / 10;
                                break;
                            case ProtoTable.BuffTable.eBuffType.MOVE_SPEED:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.moveSpeed, beBuff.level) / 10;
                                break;
                            case ProtoTable.BuffTable.eBuffType.DODGE:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.dodge, beBuff.level) / 10;
                                break;
                            case ProtoTable.BuffTable.eBuffType.DEX:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.dex, beBuff.level) / 10;
                                break;
                            case ProtoTable.BuffTable.eBuffType.HP_RECOVER:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.hpRecover, beBuff.level);
                                break;
                            case ProtoTable.BuffTable.eBuffType.MP_RECOVER:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.mpRecover, beBuff.level);
                                break;
                            case ProtoTable.BuffTable.eBuffType.HARD:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.hard, beBuff.level);
                                break;
                            case ProtoTable.BuffTable.eBuffType.ADD_DAMAGE_PERCENT:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.addDamagePercent, beBuff.level) / 10;
                                break;
                            case ProtoTable.BuffTable.eBuffType.ATTACK_ADD_RATE:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.attackAddRate, beBuff.level) / 10;
                                break;
                            case ProtoTable.BuffTable.eBuffType.MAGIC_ATTACK_ADD_RATE:
                                buffInfoTip.attriArray[(int)beBuff.buffData.BuffType[i]] += TableManager.GetValueFromUnionCell(beBuff.buffData.magicAttackAddRate, beBuff.level) / 10;
                                break;
                            default:
                                break;
                        }
                    }

                    _convertAttriData(buffInfoTip);
                }
                else
                {
                    if (beBuff.buffData.Description == "-")
                        return;
                    buffInfoTip.Detail = beBuff.buffData.Description;
                }
                #endregion
            }
        }

        public static BeActor GetPlayerBySeat(byte _seat)
        {
            if (BattleMain.instance == null || BattleMain.instance.GetPlayerManager() == null)
                return null;
            var playerActor = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(_seat);
            if (playerActor == null)
            {
                return null;
            }
            var player = playerActor.playerActor;
            return player;
        }

        protected BeBuff GetBuffByPid(int pid)
        {
            var player = GetPlayerBySeat(this.seat);
            if (player != null)
            {
                return player.buffController.GetBuffByPID(pid);
            }
            return null;
        }

        static string resistMagicGreen = "<color=#11EE11> +{0}%</color>";
        static string resistMagicRed = "<color=#ff0004ff> {0}%</color>";
        static string foreverString = "永久";

        private void _addBuffInfoData(int buffID)
        {
            var tableData = TableManager.GetInstance().GetTableItem<ProtoTable.BuffTable>(buffID);
            if (tableData == null)
            {
                return;
            }
            bool flag = false;
            if (ResistMagicBuffID != null)
            {
                for (int i = 0; i < ResistMagicBuffID.Length; ++i)
                {
                    if (buffID == ResistMagicBuffID[i])
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (flag)
            {
                var buffInfoTip = new BuffInfoTip
                {
                    IconPath = tableData.Icon,
                    Name = string.Format(tableData.BuffDisName, 1),
                    buffID = buffID,
                    LeftTime = foreverString,
                };
                mBuffTip.Add(buffInfoTip);
                #region 抗魔值显示buff
                var player = GetPlayerBySeat(this.seat);
                VFactor resistMagicRate = player.attribute.GetResistMagicRate();
                var rate = (resistMagicRate * GlobalLogic.VALUE_100).integer;
                string addText;
                if (rate >= 0)
                    addText = BeUtility.Format(resistMagicGreen, rate);
                else
                    addText = BeUtility.Format(resistMagicRed, rate);

                buffInfoTip.Detail = string.Format(tableData.Description, addText, addText);
                #endregion
            }
        }

        StringBuilder tStringBuilder = new StringBuilder(128);
        private void _convertAttriData(BuffInfoTip tip)
        {
            if (tip != null && tip.attriArray != null && tip.attriArray.Length == AttriName.Count)//冗长的判空
            {
                tStringBuilder.Length = 0;
                int firstIndex = -1;
                for (int i = 0; i < tip.attriArray.Length; ++i)
                {
                    var value = tip.attriArray[i];
                    if (value == 0)
                    {
                        continue;
                    }
                    else
                    {
                        if (firstIndex == -1)
                        {
                            AddAttriValue(tStringBuilder, "{0}", AttriName[i], value);
                            firstIndex = i;
                        }
                        else
                        {
                            AddAttriValue(tStringBuilder, " {0}", AttriName[i], value);
                        }
                    }
                }
                tip.Detail = tStringBuilder.ToString();
            }
        }

        static string greenFormat = "<color=#49DB1A>+{0}</color>";
        static string redFormat = "<color=#B8120A>{0}</color>";

        private void AddAttriValue(StringBuilder builder, string format, string attriName, float value)
        {
            string temp = string.Empty;
            if (value > 0)
            {
                temp = greenFormat;
            }
            else if (value < 0)
            {
                temp = redFormat;
            }
            if (!string.IsNullOrEmpty(temp))
            {
                var tempString = BeUtility.Format(temp, value);
                tempString = BeUtility.Format(attriName, tempString);
                builder.AppendFormat(format, tempString);
            }
        }
    }
}
