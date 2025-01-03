using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoTable;

namespace GameClient
{
    public static class EquipPropRate
    {
        public static CrypticInt32[] propRates;

        static EquipPropRate()
        {
            propRates = new CrypticInt32[(int)EEquipProp.Count];
            for (int i = 0; i < propRates.Length; ++i)
            {
                propRates[i] = new CrypticInt32(1);
            }
            propRates[(int)EEquipProp.Strenth] = new CrypticInt32(GlobalLogic.VALUE_1000);
            propRates[(int)EEquipProp.Intellect] = new CrypticInt32(GlobalLogic.VALUE_1000);
            propRates[(int)EEquipProp.Spirit] = new CrypticInt32(GlobalLogic.VALUE_1000);
            propRates[(int)EEquipProp.Stamina] = new CrypticInt32(GlobalLogic.VALUE_1000);
            propRates[(int)EEquipProp.AttackSpeedRate] = new CrypticInt32(GlobalLogic.VALUE_10);
            propRates[(int)EEquipProp.FireSpeedRate] = new CrypticInt32(GlobalLogic.VALUE_10);
            propRates[(int)EEquipProp.MoveSpeedRate] = new CrypticInt32(GlobalLogic.VALUE_10);
            propRates[(int)EEquipProp.HitRate] = new CrypticInt32(GlobalLogic.VALUE_10);
            propRates[(int)EEquipProp.AvoidRate] = new CrypticInt32(GlobalLogic.VALUE_10);
            propRates[(int)EEquipProp.PhysicCritRate] = new CrypticInt32(GlobalLogic.VALUE_10);
            propRates[(int)EEquipProp.MagicCritRate] = new CrypticInt32(GlobalLogic.VALUE_10);
            propRates[(int)EEquipProp.TownMoveSpeedRate] = new CrypticInt32(GlobalLogic.VALUE_10);
            propRates[(int)EEquipProp.PhysicsSkillMPChange] = new CrypticInt32(GlobalLogic.VALUE_10);
            propRates[(int)EEquipProp.PhysicsSkillCDChange] = new CrypticInt32(GlobalLogic.VALUE_10);
            propRates[(int)EEquipProp.MagicSkillMPChange] = new CrypticInt32(GlobalLogic.VALUE_10);
            propRates[(int)EEquipProp.MagicSkillCDChange] = new CrypticInt32(GlobalLogic.VALUE_10);
            propRates[(int)EEquipProp.IgnorePhysicsAttackRate] = new CrypticInt32(GlobalLogic.VALUE_100);
            propRates[(int)EEquipProp.IgnoreMagicAttackRate] = new CrypticInt32(GlobalLogic.VALUE_100);
            propRates[(int)EEquipProp.IgnorePhysicsDefenseRate] = new CrypticInt32(GlobalLogic.VALUE_100);
            propRates[(int)EEquipProp.IgnoreMagicDefenseRate] = new CrypticInt32(GlobalLogic.VALUE_100);
            propRates[(int)EEquipProp.Independence] = new CrypticInt32(GlobalLogic.VALUE_1000);
            propRates[(int)EEquipProp.IngoreIndependence] = new CrypticInt32(GlobalLogic.VALUE_1000);
        }
    }

    public class EquipProp
    {
        public class BuffSkillInfo
        {
            public int jobID;
            public string jobName;
            public List<string> skillDescs;
        }

        #region prop format
        enum ESign
        {
            /// <summary>
            ///
            /// </summary>
            Positive,
            /// <summary>
            ///
            /// </summary>
            Negative,
            /// <summary>
            ///
            /// </summary>
            Both,
        }

        class PropValue
        {
            public EEquipProp prop;
            public ESign sign;
            public bool isInteger;

            private bool mIsFloatConvert;

            public bool isFloatConvert 
            {
                get 
                {
                    return mIsFloatConvert && !isInteger;
                }
            }

            public PropValue(EEquipProp a_prop, ESign a_sign, bool a_isInteger = true, bool isFloatConvert = false) 
            {
                prop = a_prop;
                sign = a_sign;
                isInteger = a_isInteger;
                mIsFloatConvert = isFloatConvert;
            }
        }

        class PropFormat
        {
            public string format;
            public PropValue[] values;

            public PropFormat(string a_format, params PropValue[] a_values)
            {
                format = a_format;
                values = a_values;
            }
        }

        class PropFormats
        {
            public PropFormat[] formats;

            public PropFormats(params PropFormat[] a_formats)
            {
                formats = a_formats;
            }

            /// <summary>
            /// 某个属性
            /// </summary>
            /// <param name="a_equipProp"></param>
            /// <param name="iProIndex"></param>
            /// <returns></returns>
            public string GetFinalStr(EquipProp a_equipProp, int iProIndex = -1, bool bOriginalProcess = true)
            {

                if (formats == null)
                {
                    return string.Empty;
                }

                for (int i = 0; i < formats.Length; ++i)
                {
                    if (_CheckFormatSuitable(formats[i], a_equipProp, iProIndex))
                    {
                        PropValue[] propValues = formats[i].values;
                        List<object> values = new List<object>();

                        for (int j = 0; j < propValues.Length; ++j)
                        {
                            PropValue propValue = propValues[j];
                            float floatValue = 1.0f * a_equipProp.props[(int)propValue.prop] / EquipPropRate.propRates[(int)propValue.prop];

                            int iElementsIndex = -1;
                            #region 攻击附带属性
                            if (propValue.prop == EEquipProp.Elements)
                            {
                                if (iProIndex >= 0 && iProIndex < a_equipProp.abnormalResists.Length)
                                {
                                    if (a_equipProp.magicElements[iProIndex + 1] > 0)
                                    {
                                        floatValue = a_equipProp.magicElements[iProIndex + 1];
                                        iElementsIndex = iProIndex + 1;
                                    }
                                }
                            }
                            #endregion

                            #region 属性强化，抗性
                            if (propValue.prop >= EEquipProp.LightAttack && propValue.prop <= EEquipProp.DarkAttack)
                            {
                                if (propValue.prop - EEquipProp.LightAttack + 1 < a_equipProp.magicElementsAttack.Length)
                                {
                                    floatValue = a_equipProp.magicElementsAttack[propValue.prop - EEquipProp.LightAttack + 1];
                                }
                            }
                            else if (propValue.prop >= EEquipProp.LightDefence && propValue.prop <= EEquipProp.DarkDefence)
                            {
                                if (propValue.prop - EEquipProp.LightDefence + 1 < a_equipProp.magicElementsDefence.Length)
                                {
                                    floatValue = a_equipProp.magicElementsDefence[propValue.prop - EEquipProp.LightDefence + 1];
                                }
                            }
                            else if (propValue.prop >= EEquipProp.abnormalResist1 && propValue.prop <= EEquipProp.abnormalResist13)
                            {
                                floatValue = a_equipProp.abnormalResists[propValue.prop - EEquipProp.abnormalResist1];
                            }
                            else if (propValue.prop == EEquipProp.AbormalResists)
                            {
                                if (iProIndex >= 0 && iProIndex < a_equipProp.abnormalResists.Length)
                                {
                                    floatValue = a_equipProp.abnormalResists[iProIndex];
                                }
                            }
                            #endregion

                            int intValue = (int)(floatValue);

                            if (propValue.sign == ESign.Both)
                            {
                                
                                if (propValue.prop == EEquipProp.Elements)
                                {
                                    values.Add(Utility.GetEnumDescription((MagicElementType)iElementsIndex));
                                }
                                else if (propValue.prop >= EEquipProp.abnormalResist1 && propValue.prop <= EEquipProp.abnormalResist13)
                                {
                                    values.Add(Utility.GetEnumDescription((BuffType)((int)BuffType.FLASH + propValue.prop - EEquipProp.abnormalResist1)));
                                }
                                else if(propValue.prop == EEquipProp.AbormalResists)
                                {
                                    values.Add(Utility.GetEnumDescription((BuffType)((int)BuffType.FLASH + iProIndex)));
                                }

                                if(propValue.prop != EEquipProp.Elements)
                                {
                                    if (floatValue > 0)
                                    {
                                        values.Add("+");
                                    }
                                    else
                                    {
                                        values.Add("");
                                    }

                                    if (propValue.isInteger)
                                    {
                                        values.Add(intValue);
                                    }
                                    else if (propValue.isFloatConvert)
                                    {
                                        values.Add(_getTheString(floatValue, bOriginalProcess));
                                    }
                                    else
                                    {
                                        values.Add(floatValue.ToString("F1"));
                                    }
                                }
                            }
                            else if (propValue.sign == ESign.Positive || propValue.sign == ESign.Negative)
                            {
                                if (propValue.isInteger)
                                {
                                    values.Add(Mathf.Abs(intValue).ToString());
                                }
                                else if (propValue.isFloatConvert)
                                {
                                    values.Add(_getTheString(Mathf.Abs(floatValue)));
                                }
                                else
                                {
                                    values.Add(Mathf.Abs(floatValue).ToString("F1"));
                                }
                            }
                        }

                        return TR.Value(formats[i].format, values.ToArray());
                    }
                }

                return string.Empty;
            }

            /// <summary>
            /// 2018-01-12 03:27:37
            /// 无用函数
            /// 没有
            /// </summary>
            private string _getTheString(float v,bool bOriginalProcess = true)
            {
                if(bOriginalProcess)
                {
                    if (v >= 0.0f)
                    {
                        return TR.Value("tip_rate_2_level_format_up", Utility.ConvertItemDataRateValue2IntLevel(v), v);
                    }
                    else
                    {
                        return TR.Value("tip_rate_2_level_format_down", Utility.ConvertItemDataRateValue2IntLevel(v), v);
                    }
                }
                else
                {
                    if (v >= 0.0f)
                    {
                        return TR.Value("tip_rate_2_level_format_up2", Utility.ConvertItemDataRateValue2IntLevel(v), v);
                    }
                    else
                    {
                        return TR.Value("tip_rate_2_level_format_down2", Utility.ConvertItemDataRateValue2IntLevel(v), v);
                    }
                }
            }


            bool _CheckFormatSuitable(PropFormat a_propFormat, EquipProp a_equipProp, int iProIndex = -1)
            {
                PropValue[] propValues = a_propFormat.values;
                if (propValues != null)
                {
					int invalidCount = 0;
                    for (int j = 0; j < propValues.Length; ++j)
                    {
                        PropValue propValue = propValues[j];
                        int value = a_equipProp.props[(int)propValue.prop];

                        if(propValue.prop == EEquipProp.Elements)
                        {
                            /*
                            for(int k = 0; k < a_equipProp.magicElements.Length; k++)
                            {
                                if(a_equipProp.magicElements[k] > 0)
                                {
                                    value = a_equipProp.magicElements[k];
                                    break;
                                }
                            }               */
                            if (iProIndex >= 0 && iProIndex < a_equipProp.magicElements.Length)
                            {
                                value = a_equipProp.magicElements[iProIndex+1];
                            }
                        }
                        else if (propValue.prop >= EEquipProp.LightAttack && propValue.prop <= EEquipProp.DarkAttack)
                        {
                            if(propValue.prop - EEquipProp.LightAttack + 1 < a_equipProp.magicElementsAttack.Length)
                            {
                                value = a_equipProp.magicElementsAttack[propValue.prop - EEquipProp.LightAttack + 1];
                            }                       
                        }
                        else if(propValue.prop >= EEquipProp.LightDefence && propValue.prop <= EEquipProp.DarkDefence)
                        {
                            if (propValue.prop - EEquipProp.LightDefence + 1 < a_equipProp.magicElementsDefence.Length)
                            {
                                value = a_equipProp.magicElementsDefence[propValue.prop - EEquipProp.LightDefence + 1];
                            }
                        }
                        else if (propValue.prop >= EEquipProp.abnormalResist1 && propValue.prop <= EEquipProp.abnormalResist13)
                        {
                            value = a_equipProp.abnormalResists[propValue.prop - EEquipProp.abnormalResist1];
                        }
                        else if (propValue.prop == EEquipProp.AbormalResists)
                        {
                            if(iProIndex >= 0 && iProIndex < a_equipProp.abnormalResists.Length)
                            {
                                value = a_equipProp.abnormalResists[iProIndex];
                            }
                        }

                        if (
                            (value == 0) ||
                            (value > 0 && propValue.sign == ESign.Negative) ||
                            (value < 0 && propValue.sign == ESign.Positive)
                            )
                        {
							invalidCount++;
                        }
						else 
							invalidCount--;
                    }

					return invalidCount<=0;
                }
                else
                {
                    return false;
                }
            }
        }

        static PropFormats[] ms_propFormats = {
            new PropFormats(new PropFormat("tip_attr_physic_atk", new PropValue(EEquipProp.PhysicsAttack, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_magic_atk", new PropValue(EEquipProp.MagicAttack, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_physic_def", new PropValue(EEquipProp.PhysicsDefense, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_magic_def", new PropValue(EEquipProp.MagicDefense, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_strenth", new PropValue(EEquipProp.Strenth, ESign.Both, false))),
            new PropFormats(new PropFormat("tip_attr_intellect", new PropValue(EEquipProp.Intellect, ESign.Both, false))),
            new PropFormats(new PropFormat("tip_attr_spirit", new PropValue(EEquipProp.Spirit, ESign.Both, false))),
            new PropFormats(new PropFormat("tip_attr_stamina", new PropValue(EEquipProp.Stamina, ESign.Both, false))),
            new PropFormats(new PropFormat("tip_attr_physic_skill_change", new PropValue(EEquipProp.PhysicsSkillMPChange, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_physic_skill_change_cd", new PropValue(EEquipProp.PhysicsSkillCDChange, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_magic_skill_change", new PropValue(EEquipProp.MagicSkillMPChange, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_magic_skill_change_cd", new PropValue(EEquipProp.MagicSkillCDChange, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_hp_max", new PropValue(EEquipProp.HPMax, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_mp_max", new PropValue(EEquipProp.MPMax, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_hp_recover", new PropValue(EEquipProp.HPRecover, ESign.Positive)), new PropFormat("tip_attr_hp_cost", new PropValue(EEquipProp.HPRecover, ESign.Negative))),
            new PropFormats(new PropFormat("tip_attr_mp_recover", new PropValue(EEquipProp.MPRecover, ESign.Positive)), new PropFormat("tip_attr_mp_cost", new PropValue(EEquipProp.MPRecover, ESign.Negative))),
            new PropFormats(new PropFormat("tip_attr_atk_speed_rate", new PropValue(EEquipProp.AttackSpeedRate, ESign.Both, false, true))),
            new PropFormats(new PropFormat("tip_attr_fire_speed_rate", new PropValue(EEquipProp.FireSpeedRate, ESign.Both, false, true))),
            new PropFormats(new PropFormat("tip_attr_move_speed_rate", new PropValue(EEquipProp.MoveSpeedRate, ESign.Both, false, true))),
            new PropFormats(new PropFormat("tip_attr_abormalResist", new PropValue(EEquipProp.AbormalResist, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_abormalResists", new PropValue(EEquipProp.AbormalResists, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_elements", new PropValue(EEquipProp.Elements, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_lightAttack", new PropValue(EEquipProp.LightAttack, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_fireAttack", new PropValue(EEquipProp.FireAttack, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_iceAttack", new PropValue(EEquipProp.IceAttack, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_darkAttack", new PropValue(EEquipProp.DarkAttack, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_lightDefence", new PropValue(EEquipProp.LightDefence, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_fireDefence", new PropValue(EEquipProp.FireDefence, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_iceDefence", new PropValue(EEquipProp.IceDefence, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_darkDefence", new PropValue(EEquipProp.DarkDefence, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_hit_rate", new PropValue(EEquipProp.HitRate, ESign.Both, false, true))),
            new PropFormats(new PropFormat("tip_attr_avoid_rate", new PropValue(EEquipProp.AvoidRate, ESign.Both, false, true))),
            new PropFormats(new PropFormat("tip_attr_physic_crit", new PropValue(EEquipProp.PhysicCritRate, ESign.Both, false, true))),
            new PropFormats(new PropFormat("tip_attr_magic_crit", new PropValue(EEquipProp.MagicCritRate, ESign.Both, false, true))),
            new PropFormats(new PropFormat("tip_attr_spasticity", new PropValue(EEquipProp.Spasticity, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_jump", new PropValue(EEquipProp.Jump, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_town_move_speed_rate", new PropValue(EEquipProp.TownMoveSpeedRate, ESign.Both, false, true))),
            new PropFormats(new PropFormat("tip_attr_ignore_def_physic_attack_rate", new PropValue(EEquipProp.IgnorePhysicsAttackRate, ESign.Both, false))),
            new PropFormats(new PropFormat("tip_attr_ignore_def_magic_attack_rate", new PropValue(EEquipProp.IgnoreMagicAttackRate, ESign.Both, false))),
            new PropFormats(new PropFormat("tip_attr_ignore_def_physic_attack", new PropValue(EEquipProp.IgnorePhysicsAttack, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_ignore_def_magic_attack", new PropValue(EEquipProp.IgnoreMagicAttack, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_ignore_atk_physics_def_rate", new PropValue(EEquipProp.IgnorePhysicsDefenseRate, ESign.Both, false))),
            new PropFormats(new PropFormat("tip_attr_ignore_atk_magic_def_rate", new PropValue(EEquipProp.IgnoreMagicDefenseRate, ESign.Both, false))),
            new PropFormats(new PropFormat("tip_attr_ignore_atk_physics_def", new PropValue(EEquipProp.IgnorePhysicsDefense, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_ignore_atk_magic_def", new PropValue(EEquipProp.IgnoreMagicDefense, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_abnormalResist_1", new PropValue(EEquipProp.abnormalResist1, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_abnormalResist_2", new PropValue(EEquipProp.abnormalResist2, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_abnormalResist_3", new PropValue(EEquipProp.abnormalResist3, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_abnormalResist_4", new PropValue(EEquipProp.abnormalResist4, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_abnormalResist_5", new PropValue(EEquipProp.abnormalResist5, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_abnormalResist_6", new PropValue(EEquipProp.abnormalResist6, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_abnormalResist_7", new PropValue(EEquipProp.abnormalResist7, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_abnormalResist_8", new PropValue(EEquipProp.abnormalResist8, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_abnormalResist_9", new PropValue(EEquipProp.abnormalResist9, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_abnormalResist_10", new PropValue(EEquipProp.abnormalResist10, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_abnormalResist_11", new PropValue(EEquipProp.abnormalResist11, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_abnormalResist_12", new PropValue(EEquipProp.abnormalResist12, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_abnormalResist_13", new PropValue(EEquipProp.abnormalResist13, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_resist_magic_value", new PropValue(EEquipProp.resistMagic, ESign.Both, false))),
            new PropFormats(new PropFormat("tip_attr_independence_attack", new PropValue(EEquipProp.Independence, ESign.Both))),
            new PropFormats(new PropFormat("tip_attr_ignore_def_independence", new PropValue(EEquipProp.IngoreIndependence, ESign.Both))),
        };
        #endregion

        public CrypticInt32[] props = new CrypticInt32[(int)EEquipProp.Count];
        public List<int> attachBuffIDs = new List<int>();
        public List<int> attachMechanismIDs = new List<int>();
        public List<int> attachPVPBuffIDs = new List<int>();
        public List<int> attachPVPMechanismIDs = new List<int>();
#if OLD_EQUIP_PROP
        public string attachBuffDesc;
        public string attachMechanismDesc;
#else
        public string attachBuffDesc
        {
            get
            {
                if (this.mTableData != null)
                {
                    return this.mTableData.AttachBuffDesc;
                }

                return "";
            }
        }

        public string attachMechanismDesc
        {
            get
            {
                if (this.mTableData != null)
                {
                    return this.mTableData.AttachMechanismDesc;
                }

                return "";
            }
        }

        public EquipAttrTable TableData
        {
            set
            {
                this.mTableData = value;
            }
        }

        private EquipAttrTable mTableData;
#endif
        //武器附带攻击属性
        public int[] magicElements = new int[(int)MagicElementType.MAX];
        //属强
        public int[] magicElementsAttack = new int[(int)(MagicElementType.MAX)];
        //属抗
        public int[] magicElementsDefence = new int[(int)(MagicElementType.MAX)];
        //异抗,13种
        public int[] abnormalResists = new int[Global.ABNORMAL_COUNT];

        public static EquipProp CreateFromTable(int propID)
        {
            ProtoTable.EquipAttrTable tableData = TableManager.GetInstance().GetTableItem<ProtoTable.EquipAttrTable>(propID);
            if (tableData != null)
            {
                EquipProp equipProp = new EquipProp();

                equipProp.TableData = tableData;

#if OLD_EQUIP_PROP
                equipProp.props[(int)EEquipProp.PhysicsAttack] = tableData.Atk;
                equipProp.props[(int)EEquipProp.MagicAttack] = tableData.MagicAtk;
                equipProp.props[(int)EEquipProp.PhysicsDefense] = tableData.Def;
                equipProp.props[(int)EEquipProp.MagicDefense] = tableData.MagicDef;
                equipProp.props[(int)EEquipProp.Strenth] = tableData.Strenth;
                equipProp.props[(int)EEquipProp.Intellect] = tableData.Intellect;
                equipProp.props[(int)EEquipProp.Spirit] = tableData.Spirit;
                equipProp.props[(int)EEquipProp.Stamina] = tableData.Stamina;
                equipProp.props[(int)EEquipProp.PhysicsSkillMPChange] = tableData.PhySkillMp;
                equipProp.props[(int)EEquipProp.PhysicsSkillCDChange] = tableData.PhySkillCd;
                equipProp.props[(int)EEquipProp.MagicSkillMPChange] = tableData.MagSkillMp;
                equipProp.props[(int)EEquipProp.MagicSkillCDChange] = tableData.MagSkillCd;
                equipProp.props[(int)EEquipProp.HPMax] = tableData.HPMax;
                equipProp.props[(int)EEquipProp.MPMax] = tableData.MPMax;
                equipProp.props[(int)EEquipProp.HPRecover] = tableData.HPRecover;
                equipProp.props[(int)EEquipProp.MPRecover] = tableData.MPRecover;
                equipProp.props[(int)EEquipProp.AttackSpeedRate] = tableData.AttackSpeedRate;
                equipProp.props[(int)EEquipProp.FireSpeedRate] = tableData.FireSpeedRate;
                equipProp.props[(int)EEquipProp.MoveSpeedRate] = tableData.MoveSpeedRate;
                equipProp.props[(int)EEquipProp.HitRate] = tableData.HitRate;
                equipProp.props[(int)EEquipProp.AvoidRate] = tableData.AvoidRate;
                equipProp.props[(int)EEquipProp.PhysicCritRate] = tableData.PhysicCrit;
                equipProp.props[(int)EEquipProp.MagicCritRate] = tableData.MagicCrit;
                equipProp.props[(int)EEquipProp.Spasticity] = tableData.Spasticity;
                equipProp.props[(int)EEquipProp.Jump] = tableData.Jump;
                equipProp.props[(int)EEquipProp.TownMoveSpeedRate] = tableData.TownMoveSpeedRate;
                equipProp.props[(int)EEquipProp.IgnorePhysicsAttackRate] = 0;
                equipProp.props[(int)EEquipProp.IgnoreMagicAttackRate] = 0;
                equipProp.props[(int)EEquipProp.IgnorePhysicsAttack] = 0;
                equipProp.props[(int)EEquipProp.IgnoreMagicAttack] = 0;
                equipProp.props[(int)EEquipProp.IgnorePhysicsDefenseRate] = 0;
                equipProp.props[(int)EEquipProp.IgnoreMagicDefenseRate] = 0;
                equipProp.props[(int)EEquipProp.IgnorePhysicsDefense] = 0;
                equipProp.props[(int)EEquipProp.IgnoreMagicDefense] = 0;
#else
                equipProp.props[(int)EEquipProp.PhysicsAttack] = new CrypticInt32(tableData.Atk);
                equipProp.props[(int)EEquipProp.MagicAttack] = new CrypticInt32(tableData.MagicAtk);
                equipProp.props[(int)EEquipProp.PhysicsDefense] = new CrypticInt32(tableData.Def);
                equipProp.props[(int)EEquipProp.MagicDefense] = new CrypticInt32(tableData.MagicDef);
                equipProp.props[(int)EEquipProp.Strenth] = new CrypticInt32(tableData.Strenth);
                equipProp.props[(int)EEquipProp.Intellect] = new CrypticInt32(tableData.Intellect);
                equipProp.props[(int)EEquipProp.Spirit] = new CrypticInt32(tableData.Spirit);
                equipProp.props[(int)EEquipProp.Stamina] = new CrypticInt32(tableData.Stamina);
                equipProp.props[(int)EEquipProp.PhysicsSkillMPChange] = new CrypticInt32(tableData.PhySkillMp);
                equipProp.props[(int)EEquipProp.PhysicsSkillCDChange] = new CrypticInt32(tableData.PhySkillCd);
                equipProp.props[(int)EEquipProp.MagicSkillMPChange] = new CrypticInt32(tableData.MagSkillMp);
                equipProp.props[(int)EEquipProp.MagicSkillCDChange] = new CrypticInt32(tableData.MagSkillCd);
                equipProp.props[(int)EEquipProp.HPMax] = new CrypticInt32(tableData.HPMax);
                equipProp.props[(int)EEquipProp.MPMax] = new CrypticInt32(tableData.MPMax);
                equipProp.props[(int)EEquipProp.HPRecover] = new CrypticInt32(tableData.HPRecover);
                equipProp.props[(int)EEquipProp.MPRecover] = new CrypticInt32(tableData.MPRecover);
                equipProp.props[(int)EEquipProp.AttackSpeedRate] = new CrypticInt32(tableData.AttackSpeedRate);
                equipProp.props[(int)EEquipProp.FireSpeedRate] = new CrypticInt32(tableData.FireSpeedRate);
                equipProp.props[(int)EEquipProp.MoveSpeedRate] = new CrypticInt32(tableData.MoveSpeedRate);
                equipProp.props[(int)EEquipProp.HitRate] = new CrypticInt32(tableData.HitRate);
                equipProp.props[(int)EEquipProp.AvoidRate] = new CrypticInt32(tableData.AvoidRate);
                equipProp.props[(int)EEquipProp.PhysicCritRate] = new CrypticInt32(tableData.PhysicCrit);
                equipProp.props[(int)EEquipProp.MagicCritRate] = new CrypticInt32(tableData.MagicCrit);
                equipProp.props[(int)EEquipProp.Spasticity] = new CrypticInt32(tableData.Spasticity);
                equipProp.props[(int)EEquipProp.Jump] = new CrypticInt32(tableData.Jump);
                equipProp.props[(int)EEquipProp.TownMoveSpeedRate] = new CrypticInt32(tableData.TownMoveSpeedRate);
                equipProp.props[(int)EEquipProp.IgnorePhysicsAttackRate] = new CrypticInt32(0);
                equipProp.props[(int)EEquipProp.IgnoreMagicAttackRate] = new CrypticInt32(0);
                equipProp.props[(int)EEquipProp.IgnorePhysicsAttack] = new CrypticInt32(0);
                equipProp.props[(int)EEquipProp.IgnoreMagicAttack] = new CrypticInt32(0);
                equipProp.props[(int)EEquipProp.IngoreIndependence] = new CrypticInt32(0);
                equipProp.props[(int)EEquipProp.IgnorePhysicsDefenseRate] = new CrypticInt32(0);
                equipProp.props[(int)EEquipProp.IgnoreMagicDefenseRate] = new CrypticInt32(0);
                equipProp.props[(int)EEquipProp.IgnorePhysicsDefense] = new CrypticInt32(0);
                equipProp.props[(int)EEquipProp.IgnoreMagicDefense] = new CrypticInt32(0);
                equipProp.props[(int)EEquipProp.resistMagic] = tableData.ResistMagic;
                equipProp.props[(int)EEquipProp.Independence] = new CrypticInt32(tableData.Independence);
#endif
                equipProp.attachBuffIDs = new List<int>(tableData.AttachBuffInfoIDs);
                equipProp.attachMechanismIDs = new List<int>(tableData.AttachMechanismIDs);
                equipProp.attachPVPBuffIDs = new List<int>(tableData.PVPAttachBuffInfoIDs);
                equipProp.attachPVPMechanismIDs = new List<int>(tableData.PVPAttachMechanismIDs);
#if OLD_EQUIP_PROP
                equipProp.attachBuffDesc = tableData.AttachBuffDesc;
                equipProp.attachMechanismDesc = tableData.AttachMechanismDesc;
#endif
                
                //攻击属性，属强，属抗
                for(int i=0; i<tableData.Elements.Count; ++i)
                {
                    if (tableData.Elements[i] > 0 && tableData.Elements[i] < (int)MagicElementType.MAX)
                    {
                        equipProp.magicElements[tableData.Elements[i]] = 1;
                    }
                }

                int[] elementAttacks = new int[]
                {
                    tableData.LightAttack, tableData.FireAttack, tableData.IceAttack, tableData.DarkAttack
                };

                int[] elementDefences = new int[]
                {
                    tableData.LightDefence, tableData.FireDefence, tableData.IceDefence, tableData.DarkDefence
                };

                for (int i=1; i<(int)MagicElementType.MAX; ++i)
                {
                    equipProp.magicElementsAttack[i] = elementAttacks[i-1];
                    equipProp.magicElementsDefence[i] = elementDefences[i-1];
                }

                //异抗
                equipProp.props[(int)EEquipProp.AbormalResist] = tableData.AbormalResist;

                /*
                var values = BeUtility.ParseAbnormalResistString(tableData.AbormalResists);
                for (int i = 0; i < values.Length; ++i)
                    equipProp.abnormalResists[i] = values[i];
                    */
                int[] abnormalResistV = new int[]
                {
                    tableData.abnormalResist1,
                    tableData.abnormalResist2,
                    tableData.abnormalResist3,
                    tableData.abnormalResist4,
                    tableData.abnormalResist5,
                    tableData.abnormalResist6,
                    tableData.abnormalResist7,
                    tableData.abnormalResist8,
                    tableData.abnormalResist9,
                    tableData.abnormalResist10,
                    tableData.abnormalResist11,
                    tableData.abnormalResist12,
                    tableData.abnormalResist13,
                };
                for(int i=0; i< abnormalResistV.Length; ++i)
                {
                    equipProp.abnormalResists[i] = abnormalResistV[i];
                }
                    

                return equipProp;
            }
            else
            {
                Logger.LogWarningFormat("can not find equip prop data with id:{0}", propID);
                return null;
            }
        }

        public void ResetProperties()
        {
            for(int i = 0; i < props.Length; ++i)
            {
                props[i] = new CrypticInt32(0);
            }

            for (int i = 0; i < magicElements.Length; ++i)
                magicElements[i] = 0;
            for (int i = 0; i < magicElementsAttack.Length; ++i)
                magicElementsAttack[i] = 0;
            for (int i = 0; i < magicElementsDefence.Length; ++i)
                magicElementsDefence[i] = 0;
            for (int i = 0; i < abnormalResists.Length; ++i)
                abnormalResists[i] = 0;


            attachBuffIDs.Clear();
            attachPVPBuffIDs.Clear();
            attachMechanismIDs.Clear();
            attachPVPMechanismIDs.Clear();
        }

        public string GetPropFormatStr(EEquipProp a_prop, int iProIndex = -1)
        {
            if (a_prop > EEquipProp.Invalid && a_prop < EEquipProp.Count)
            {
                return ms_propFormats[(int)a_prop].GetFinalStr(this, iProIndex);
            }
            return string.Empty;
        }

        /// <summary>
        /// 属性描述
        /// </summary>
        /// <param name="bOriginalProcess">按原流程执行</param>
        /// <returns></returns>
        public List<string> GetPropsFormatStr(bool bOriginalProcess = true)
        {
            List<string> propStrs = new List<string>();
            for (int i = 0; i < ms_propFormats.Length; ++i)
            {
                string str = null;
                if (i == (int)EEquipProp.AbormalResists)
                {
                    for (int j = 0; j < Global.ABNORMAL_COUNT; j++)
                    {
                        string temp = ms_propFormats[i].GetFinalStr(this, j, bOriginalProcess);

                        if (string.IsNullOrEmpty(temp) == false && !propStrs.Contains(temp))
                        {
                            propStrs.Add(temp);
                        }
                    }
                }
                else if (i == (int)EEquipProp.Elements)
                {
                    for (int j = 0; j < Global.ELEMENT_COUNT; ++j)
                    {
                        string temp = ms_propFormats[i].GetFinalStr(this, j, bOriginalProcess);
                        if (string.IsNullOrEmpty(temp) == false && !propStrs.Contains(temp))
                        {
                            propStrs.Add(temp);
                        }
                    }
                }
                else
                    str = ms_propFormats[i].GetFinalStr(this,-1, bOriginalProcess);
                if (string.IsNullOrEmpty(str) == false && propStrs.Contains(str) == false)
                {
                    propStrs.Add(str);
                }
            }

            return propStrs;
        }
        
        public List<BuffSkillInfo> GetBuffSkillInfos()
        {
            List<BuffSkillInfo> buffSkillInfos = new List<BuffSkillInfo>();
            for (int i = 0; i < attachBuffIDs.Count; ++i)
            {
                int buffID = attachBuffIDs[i];
                if (attachPVPBuffIDs.Contains(buffID) == false)
                {
                    continue;
                }
                ProtoTable.BuffInfoTable buffInfoTable = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(attachBuffIDs[i]);
                if (buffInfoTable != null)
                {
                    if (buffInfoTable.DescType == ProtoTable.BuffInfoTable.eDescType.SkillLevel)
                    {
                        int baseJobID = _GetBaseJobID(buffInfoTable);
                        BuffSkillInfo buffSkillInfo = buffSkillInfos.Find(data => { return data.jobID == baseJobID; });
                        if (buffSkillInfo == null)
                        {
                            buffSkillInfo = new BuffSkillInfo();
                            buffSkillInfo.jobID = baseJobID;
                            ProtoTable.JobTable jobTable = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(baseJobID);
                            buffSkillInfo.jobName = (jobTable == null ? "" : jobTable.Name);
                            buffSkillInfo.skillDescs = new List<string>(buffInfoTable.Description);
                            buffSkillInfos.Add(buffSkillInfo);
                        }
                        else
                        {
                            buffSkillInfo.skillDescs.AddRange(buffInfoTable.Description);
                        }
                    }
                }
            }

            return buffSkillInfos;
        }

        public List<string> GetBuffCommonDescs()
        {
            List<string> buffDescs = new List<string>();
            for (int i = 0; i < attachBuffIDs.Count; ++i)
            {
                int buffID = attachBuffIDs[i];
                if (attachPVPBuffIDs.Contains(buffID) == false)
                {
                    continue;
                }
                ProtoTable.BuffInfoTable buffInfoTable = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(buffID);
                if (buffInfoTable != null)
                {
                    if (buffInfoTable.DescType == ProtoTable.BuffInfoTable.eDescType.Common)
                    {
                        buffDescs.AddRange(buffInfoTable.Description);
                    }
                }
            }

            return buffDescs;
        }

        public List<string> GetMechanismDescs()
        {
            List<string> mechanismDescs = new List<string>();
            for (int i = 0; i < attachMechanismIDs.Count; ++i)
            {
                int mechanismID = attachMechanismIDs[i];
                if (attachPVPMechanismIDs.Contains(mechanismID) == false)
                {
                    continue;
                }
                ProtoTable.MechanismTable mechanismTable = TableManager.GetInstance().GetTableItem<ProtoTable.MechanismTable>(mechanismID);
                if (mechanismTable != null)
                {
                    mechanismDescs.Add(mechanismTable.Description);
                }
            }

            return mechanismDescs;
        }

        FieldInfo[] propFieldInfos = null;
        int[] propFiledIDs = null;
        //TODO 反射
        public ItemProperty ToItemProp(int strengthen = 0, int grid = 0,EGrowthAttrType growthAttrType = EGrowthAttrType.GAT_NONE,int growthAttrNum = 0)
        {
            ItemProperty prop = new ItemProperty();
            prop.strengthen = strengthen;
            prop.grid = grid;

            prop.maxHp = this.props[(int)EEquipProp.HPMax];
            prop.maxMp = this.props[(int)EEquipProp.MPMax];
            prop.hpRecover = this.props[(int)EEquipProp.HPRecover];
            prop.mpRecover = this.props[(int)EEquipProp.MPRecover];
            prop.baseSta = this.props[(int)EEquipProp.Stamina];
            prop.baseAtk = this.props[(int)EEquipProp.Strenth];
            prop.baseInt = this.props[(int)EEquipProp.Intellect];
            prop.baseSpr = this.props[(int)EEquipProp.Spirit];
            prop.attack = this.props[(int)EEquipProp.PhysicsAttack];
            prop.magicAttack = this.props[(int)EEquipProp.MagicAttack];
            prop.defence = this.props[(int)EEquipProp.PhysicsDefense];
            prop.magicDefence = this.props[(int)EEquipProp.MagicDefense];
            prop.attackSpeed = this.props[(int)EEquipProp.AttackSpeedRate];
            prop.spellSpeed = this.props[(int)EEquipProp.FireSpeedRate];
            prop.moveSpeed = this.props[(int)EEquipProp.MoveSpeedRate];
            prop.ciriticalAttack = this.props[(int)EEquipProp.PhysicCritRate];
            prop.ciriticalMagicAttack = this.props[(int)EEquipProp.MagicCritRate];
            prop.dex = this.props[(int)EEquipProp.HitRate];
            prop.dodge = this.props[(int)EEquipProp.AvoidRate];
            prop.hard = this.props[(int)EEquipProp.Spasticity];
            prop.abnormalResist = this.props[(int)EEquipProp.AbormalResist];
            prop.jumpForce = this.props[(int)EEquipProp.Jump];
            prop.mpCostReduceRate = this.props[(int)EEquipProp.PhysicsSkillMPChange];
            prop.mpCostReduceRateMagic = this.props[(int)EEquipProp.MagicSkillMPChange];
            prop.cdReduceRate = this.props[(int)EEquipProp.PhysicsSkillCDChange];
            prop.cdReduceRateMagic = this.props[(int)EEquipProp.MagicSkillCDChange];
            prop.attackAddRate = this.props[(int)EEquipProp.IgnorePhysicsAttackRate];
            prop.magicAttackAddRate = this.props[(int)EEquipProp.IgnoreMagicAttackRate];
            prop.ignoreDefAttackAdd = this.props[(int)EEquipProp.IgnorePhysicsAttack];
            prop.ignoreDefMagicAttackAdd = this.props[(int)EEquipProp.IgnoreMagicAttack];
            prop.ingoreIndependence = this.props[(int)EEquipProp.IngoreIndependence];
            prop.attackReduceRate = this.props[(int)EEquipProp.IgnorePhysicsDefenseRate];
            prop.magicAttackReduceRate = this.props[(int)EEquipProp.IgnoreMagicDefenseRate];
            prop.attackReduceFix = this.props[(int)EEquipProp.IgnorePhysicsDefense];
            prop.magicAttackReduceFix = this.props[(int)EEquipProp.IgnoreMagicDefense];
            prop.independence = this.props[(int)EEquipProp.Independence];
            /// FieldInfo[] fieldInfos = prop.GetType().GetFields();
            /// for (int i = 0; i < fieldInfos.Length; ++i)
            /// {
            ///     FieldInfo field = fieldInfos[i];
            ///     object[] attrs = field.GetCustomAttributes(typeof(EquipPropAttribute), false);
            ///     if (attrs.Length > 0)
            ///     {
            ///         int id = (int)(attrs[0] as EquipPropAttribute).Prop;
            /// 
            ///         field.SetValue(prop, props[id]);
            ///     }
            /// }
            prop.attachBuffIDs = attachBuffIDs;
            prop.attachMechanismIDs = attachMechanismIDs;
            prop.attachPVPBuffIDs = attachPVPBuffIDs;
            prop.attachPVPMechanismIDs = attachPVPMechanismIDs;

            prop.magicElements = magicElements;

            for (int i = 0; i < magicElementsAttack.Length; ++i)
                prop.magicElementsAttack[i] = magicElementsAttack[i];
            for (int i = 0; i < magicElementsDefence.Length; ++i)
                prop.magicElementsDefence[i] = magicElementsDefence[i];
            for (int i = 0; i < abnormalResists.Length; ++i)
                prop.abnormalResists[i] = abnormalResists[i];

            //抗魔值
            prop.resistMagic = this.props[(int)EEquipProp.resistMagic];

            //增幅属性值
            switch (growthAttrType)
            {
                case EGrowthAttrType.GAT_NONE:
                    break;
                case EGrowthAttrType.GAT_STRENGTH:
                    prop.baseAtk += growthAttrNum * GlobalLogic.VALUE_1000;
                    break;
                case EGrowthAttrType.GAT_INTELLIGENCE:
                    prop.baseInt += growthAttrNum * GlobalLogic.VALUE_1000;
                    break;
                case EGrowthAttrType.GAT_STAMINA:
                    prop.baseSta += growthAttrNum * GlobalLogic.VALUE_1000;
                    break;
                case EGrowthAttrType.GAT_SPIRIT:
                    prop.baseSpr += growthAttrNum * GlobalLogic.VALUE_1000;
                    break;
            }

            /*

            prop.magicElementsAttack = magicElementsAttack;
            prop.magicElementsDefence = magicElementsDefence;

            prop.abnormalResists = abnormalResists;
            */

            return prop;
        }

        public static EquipProp operator +(EquipProp lhs, EquipProp rhs)
        {
            EquipProp result = new EquipProp();
            for (int i = 0; i < (int)EEquipProp.Count; ++i)
            {
                result.props[i] = lhs.props[i] + rhs.props[i];
            }

            for(int i=1; i<(int)MagicElementType.MAX; ++i)
            {
                result.magicElements[i] = lhs.magicElements[i] + rhs.magicElements[i];
                result.magicElementsAttack[i] = lhs.magicElementsAttack[i] + rhs.magicElementsAttack[i];
                result.magicElementsDefence[i] = lhs.magicElementsDefence[i] + rhs.magicElementsDefence[i];
            }

            for(int i=0; i<Global.ABNORMAL_COUNT; ++i)
            {
                result.abnormalResists[i] = lhs.abnormalResists[i] + rhs.abnormalResists[i];
            }

            if (lhs.attachBuffIDs.Count > 0)
            {
                result.attachBuffIDs.InsertRange(result.attachBuffIDs.Count, lhs.attachBuffIDs);
            }
            if (rhs.attachBuffIDs.Count > 0)
            {
                result.attachBuffIDs.InsertRange(result.attachBuffIDs.Count, rhs.attachBuffIDs);
            }
            if (lhs.attachMechanismIDs.Count > 0)
            {
                result.attachMechanismIDs.InsertRange(result.attachMechanismIDs.Count, lhs.attachMechanismIDs);
            }
            if (rhs.attachMechanismIDs.Count > 0)
            {
                result.attachMechanismIDs.InsertRange(result.attachMechanismIDs.Count, rhs.attachMechanismIDs);
            }

            if (lhs.attachPVPBuffIDs.Count > 0)
            {
                result.attachPVPBuffIDs.InsertRange(result.attachPVPBuffIDs.Count, lhs.attachPVPBuffIDs);
            }
            if (rhs.attachPVPBuffIDs.Count > 0)
            {
                result.attachPVPBuffIDs.InsertRange(result.attachPVPBuffIDs.Count, rhs.attachPVPBuffIDs);
            }
            if (lhs.attachPVPMechanismIDs.Count > 0)
            {
                result.attachPVPMechanismIDs.InsertRange(result.attachPVPMechanismIDs.Count, lhs.attachPVPMechanismIDs);
            }
            if (rhs.attachPVPMechanismIDs.Count > 0)
            {
                result.attachPVPMechanismIDs.InsertRange(result.attachPVPMechanismIDs.Count, rhs.attachPVPMechanismIDs);
            }

            

            return result;
        }

        public static EquipProp operator -(EquipProp lhs, EquipProp rhs)
        {
            EquipProp result = new EquipProp();
            for (int i = 0; i < (int)EEquipProp.Count; ++i)
            {
                result.props[i] = lhs.props[i] - rhs.props[i];
            }
            return result;
        }

        int _GetBaseJobID(ProtoTable.BuffInfoTable a_buffInfoTable)
        {
            int baseJobID = 0;
            for (int j = 0; j < a_buffInfoTable.SkillID.Count; ++j)
            {
                ProtoTable.SkillTable skillTable = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(a_buffInfoTable.SkillID[j]);
                if (skillTable != null)
                {
                    for (int k = 0; k < skillTable.JobID.Count; ++k)
                    {
                        int jobID = skillTable.JobID[k];
                        ProtoTable.JobTable jobTable = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(jobID);
                        if (jobTable != null && jobTable.prejob != 0)
                        {
                            jobID = jobTable.prejob;
                            jobTable = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(jobID);
                        }

                        if (baseJobID == 0)
                        {
                            baseJobID = jobID;
                        }
//                         else
//                         {
//                             if (baseJobID != jobID)
//                             {
//                                 Logger.LogErrorFormat("bufferInfo:{0} can not contain multiple base job!", a_buffInfoTable.ID);
//                             }
//                         }
                    }
                }
            }

            return baseJobID;
        }
    }

    public class EquipSuitRes
    {
        /// <summary>
        /// 装备套装表ID
        /// </summary>
        public int id;

        /// <summary>
        /// 套装名称
        /// </summary>
        public string name;

        /// <summary>
        /// 套装包含的装备、时装等（道具ID列表）
        /// </summary>
        public IList<int> equips = new List<int>();

        /// <summary>
        /// 穿戴的套装件数与对应的属性
        /// </summary>
        public Dictionary<int, EquipProp> props = new Dictionary<int, EquipProp>();    // key --> currItemCount, value --> all props
    }

    public class EquipSuitObj
    {
        /// <summary>
        /// 穿戴中的装备、时装（道具ID列表）
        /// </summary>
        public List<int> wearedEquipIDs = null;
        /// <summary>
        /// 套装资源，表格数据
        /// </summary>
        public EquipSuitRes equipSuitRes = null;

        //套装中的装备是否被激活
        public bool IsSuitEquipActive(ItemData suitEquip)
        {
            if (suitEquip == null)
                return false;

            //只有穿了，才表示被激活，装备类型
            if (wearedEquipIDs.Contains(suitEquip.TableID))
            {
                return true;
            }
            
            //如果wearedEquipIds 中不存在，并且该装备为时装类型。则检测其他的情况
            if (suitEquip.Type == ItemTable.eType.FASHION)
            {
                for (var i = 0; i < wearedEquipIDs.Count; i++)
                {
                    var item = ItemDataManager.GetInstance().GetCommonItemTableDataByID(wearedEquipIDs[i]);
                    if (item.Type == ItemTable.eType.FASHION
                        && item.FashionWearSlotType == suitEquip.FashionWearSlotType)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsEquipActive(ItemData a_equip)
        {
            if (wearedEquipIDs.Contains(a_equip.TableID))
            {
                return true;
            }
            else
            {
                for (int i = 0; i < wearedEquipIDs.Count; ++i)
                {
                    ItemData item = ItemDataManager.GetInstance().GetCommonItemTableDataByID(wearedEquipIDs[i]);
                    if (item.IsWearSoltEqual(a_equip))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public class EquipMasterData
    {
        public int id;
        public int jobID;
        public CrypticInt32 quality;
        public int part;
        public int materialType;        // 装备材质类型 （布甲，重甲，板甲.....）
        public CrypticInt32[] qualityParam = new CrypticInt32[(int)EEquipProp.Count];
        public CrypticInt32[] partParam = new CrypticInt32[(int)EEquipProp.Count];
        public CrypticInt32[] fixParam = new CrypticInt32[(int)EEquipProp.Count];
        public ProtoTable.EquipMasterTable masterItem = null;

        public EquipProp GetEquipProp(int equipLevel)
        {
            EquipProp equipProp = new EquipProp();
            for (int i = 0; i < equipProp.props.Length; ++i)
            {
                //float temp = Mathf.Round(equipLevel / 5.0f);
				//float qualityParamValue = qualityParam[i] / (float)GlobalLogic.VALUE_1000;
				//float partParamValue = partParam[i] / (float)GlobalLogic.VALUE_1000;
				//float fixParamValue = fixParam[i] / (float)GlobalLogic.VALUE_1000;

				int temp = IntMath.Float2Int(Mathf.Round(equipLevel / 5.0f)); /*IntMath.Float2Int(equipLevel / 5.0f);*/
				VFactor qualityParamValue = new VFactor(GlobalLogic.VALUE_1 * qualityParam[i], 	GlobalLogic.VALUE_1000);
				VFactor partParamValue = new VFactor(GlobalLogic.VALUE_1 * partParam[i], 		GlobalLogic.VALUE_1000);
				VFactor fixParamValue = new VFactor(GlobalLogic.VALUE_1 * fixParam[i], 			GlobalLogic.VALUE_1000);

				VFactor value = qualityParamValue * partParamValue + temp * partParamValue + fixParamValue;
				equipProp.props[i] = (value * EquipPropRate.propRates[i]).roundInt;
            }
            return equipProp;
        }
    }
}
