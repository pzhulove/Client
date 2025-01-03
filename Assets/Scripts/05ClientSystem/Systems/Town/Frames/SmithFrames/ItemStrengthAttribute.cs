using ProtoTable;
using System.Collections.Generic;

namespace GameClient
{
    class StrengthenAttributeItemData
    {
        public string kDesc = null;
        public float iCurValue = 0.0f;
        public string valueFormat = string.Empty;

        public string ToValueDesc()
        {
            return string.Format(valueFormat, iCurValue);
        }

        public static string ToValueDesc(string valueFormat,float iValue)
        {
            return string.Format(valueFormat, iValue);
        }
    }
    class ItemStrengthAttribute
    {
        #region staticvar
        static int ms_min_level = 1;
        static int ms_max_level = 20;
        static IList<int>[] ms_arr_mod = new IList<int>[25];
        static string[] ms_strengthen_desc_pre = new string[]
        {
            "tip_attr_ignore_def_physic_attack_pre",
            "tip_attr_ignore_def_magic_attack_pre",
            "tip_attr_ignore_atk_physics_def_pre",
            "tip_attr_ignore_atk_magic_def_pre",
            "tip_attr_ignore_atk_physics_def_rate_pre",
            "tip_attr_ignore_atk_magic_def_rate_pre",
            "tip_attr_ignore_def_independence_pre"
        };
        #endregion

        #region memer_var
        List<StrengthenAttributeItemData> attributes = new List<StrengthenAttributeItemData>();
        ItemData itemData = null;
        EquipStrModTable equipStrMode = null;
        EquipStrModTable equipStrMode2 = null;
        EquipAttrTable attrData = null;
        bool bPvp = false;
        EquipStrModIndAtkTable equipStrModeIndependence = null;
        #endregion

        private ItemStrengthAttribute(ItemData itemData, ProtoTable.EquipAttrTable equipAttr, EquipStrModTable equipStrMode, EquipStrModTable equipStrMode2, EquipStrModIndAtkTable equipStrModeIndependence, bool bPvp)
        {
            this.itemData = itemData;
            this.attrData = equipAttr;
            this.equipStrMode = equipStrMode;
            this.equipStrMode2 = equipStrMode2;
            this.equipStrModeIndependence = equipStrModeIndependence;
            this.bPvp = bPvp;
        }

        public static ItemStrengthAttribute Create(int iTableID, bool bPvp = false)
        {
            var tableItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)iTableID);
            if (tableItem == null)
            {
                return null;
            }

            var itemData = ItemDataManager.CreateItemDataFromTable(iTableID);
            if (itemData == null)
            {
                return null;
            }

            var equipAttr = TableManager.GetInstance().GetTableItem<ProtoTable.EquipAttrTable>(tableItem.EquipPropID);
            if (equipAttr == null)
            {
                return null;
            }

            var equipStrMode = TableManager.GetInstance().GetTableItem<ProtoTable.EquipStrModTable>(bPvp ? 2 : 1);
            if (equipStrMode == null)
            {
                return null;
            }

            var equipStrMode2 = TableManager.GetInstance().GetTableItem<ProtoTable.EquipStrModTable>(bPvp ? 4 : 3);
            if(equipStrMode2 == null)
            {
                return null;
            }

            var equipStrModeIndependence = TableManager.GetInstance().GetTableItem<EquipStrModIndAtkTable>(bPvp ? 2 : 1);
            if (equipStrModeIndependence == null)
            {
                return null;
            }

            return new ItemStrengthAttribute(itemData, equipAttr, equipStrMode, equipStrMode2, equipStrModeIndependence, bPvp);
        }

        public void SetStrength(int iStrength)
        {
            attributes.Clear();

            iStrength = (int)IntMath.Clamp(iStrength, 0, ms_max_level);

            // 系数计算
            double wpStrMod = _GetWpStrMode(iStrength);
            double wpClQaMod = _GetWpClQaMod();
            double wpClQbMod = _GetWpClQbMod();
            double wpPhyMod = _GetWpPhyMod();
            double wpMagMod = _GetWpMagMod();

            //固定攻击系数
            double wpStrModIndependence = _GetWpStrModeIndependence(iStrength);
            double wpClQaModIndependence = _GetWpClQaModIndependence();
            double wpClQbModIndependence = _GetWpClQbModIndependence();
            double wpPhyModIndependence = _GetWpPhyModIndependence();

            double armStrMod = _GetArmStrMod(iStrength,equipStrMode);
            double armClQaMod = _GetArmClQaMod(equipStrMode);
            double armClQbMod = _GetArmClQbMod();

            double jewStrMod = _GetJewStrMod(iStrength, equipStrMode);
            double jewClQaMod = _GetJewClQaMod(equipStrMode);
            double jewClQbMod = _GetJewClQbMod();

            double armStrMod2 = _GetArmStrMod(iStrength,equipStrMode2);
            double armClQaMod2 = _GetArmClQaMod(equipStrMode2);

            double jewStrMod2 = _GetJewStrMod(iStrength, equipStrMode2);
            double jewClQaMod2 = _GetJewClQaMod(equipStrMode2);

            int m_DisPhyAtk = 0;
            if (_IsWeapon((ItemTable.eSubType)itemData.SubType))
            {
                if (0 != attrData.Atk)
                {
                    if (!bPvp)
                    {
                        m_DisPhyAtk = (int)(((itemData.LevelLimit + wpClQaMod) * 0.125 * wpStrMod * wpClQbMod * wpPhyMod * 1.1) + 0.5);
                        if (m_DisPhyAtk < 1) m_DisPhyAtk = 0;
                    }
                    else
                    {
                        m_DisPhyAtk = (int)(((itemData.LevelLimit + wpClQaMod) * 0.125 * wpStrMod * wpClQbMod * wpPhyMod * 1.1) + 0.5);
                        if (m_DisPhyAtk < 1) m_DisPhyAtk = 0;
                    }

                    attributes.Add(new StrengthenAttributeItemData
                    {
                        kDesc = TR.Value(ms_strengthen_desc_pre[0]),
                        iCurValue = m_DisPhyAtk,
                        valueFormat = "{0}",
                    });
                }
            }
            itemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsAttack] = m_DisPhyAtk;

            int m_DisMagAtk = 0;
            if(_IsWeapon((ItemTable.eSubType)itemData.SubType))
            {
                if (0 != attrData.MagicAtk)
                {
                    if (!bPvp)
                    {
                        m_DisMagAtk = (int)(((itemData.LevelLimit + wpClQaMod) * 0.125 * wpStrMod * wpClQbMod * wpMagMod * 1.1) + 0.5);
                        if (m_DisMagAtk < 1) m_DisMagAtk = 0;
                    }
                    else
                    {
                        m_DisMagAtk = (int)(((itemData.LevelLimit + wpClQaMod) * 0.125 * wpStrMod * wpClQbMod * wpMagMod * 1.1) + 0.5);
                        if (m_DisMagAtk < 1) m_DisMagAtk = 0;
                    }

                    attributes.Add(new StrengthenAttributeItemData
                    {
                        kDesc = TR.Value(ms_strengthen_desc_pre[1]),
                        iCurValue = m_DisMagAtk,
                        valueFormat = "{0}",
                    });
                }
            }
            itemData.BaseProp.props[(int)EEquipProp.IgnoreMagicAttack] = m_DisMagAtk;

            int m_DisPhyIndependence = 0;
            if (_IsWeapon((ItemTable.eSubType)itemData.SubType))
            {
                if (0 != attrData.Independence)
                {
                    m_DisPhyIndependence = (int)((((itemData.LevelLimit + wpClQaModIndependence) * 0.125) * wpStrModIndependence * wpClQbModIndependence * wpPhyModIndependence * 1.1));
                    if (m_DisPhyIndependence < 1) m_DisPhyIndependence = 0;

                    attributes.Add(new StrengthenAttributeItemData
                    {
                        kDesc = TR.Value(ms_strengthen_desc_pre[6]),
                        iCurValue = m_DisPhyIndependence,
                        valueFormat = "{0}",
                    });
                }
            }

            itemData.BaseProp.props[(int)EEquipProp.IngoreIndependence] = m_DisPhyIndependence;

            int m_DisPhyDef = 0;
            if (_IsArmy((ItemTable.eSubType)itemData.SubType))
            {
                if (0 != attrData.Def)
                {
                    if (!bPvp)
                    {
                        m_DisPhyDef = (int)(((itemData.LevelLimit + armClQaMod) * 0.125 * armStrMod * armClQbMod) + 0.5);
                        if (m_DisPhyDef < 1) m_DisPhyDef = 0;
                    }
                    else
                    {
                        m_DisPhyDef = (int)(((itemData.LevelLimit + armClQaMod) * 0.125 * armStrMod * armClQbMod) + 0.5);
                        if (m_DisPhyDef < 1) m_DisPhyDef = 0;
                    }

                    attributes.Add(new StrengthenAttributeItemData
                    {
                        kDesc = TR.Value(ms_strengthen_desc_pre[2]),
                        iCurValue = m_DisPhyDef,
                        valueFormat = "{0}",
                    });
                }
            }
            itemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefense] = m_DisPhyDef;

            int m_DisMagDef = 0;
            if (_IsJewelry((ItemTable.eSubType)itemData.SubType))
            {
                if (0 != attrData.MagicDef)
                {
                    if (!bPvp)
                    {
                        m_DisMagDef = (int)(((itemData.LevelLimit + jewClQaMod) * 0.125 * jewStrMod * jewClQbMod) + 0.5);
                        if (m_DisMagDef < 1) m_DisMagDef = 0;
                    }
                    else
                    {
                        m_DisMagDef = (int)(((itemData.LevelLimit + jewClQaMod) * 0.125 * jewStrMod * jewClQbMod) + 0.5);
                        if (m_DisMagDef < 1) m_DisMagDef = 0;
                    }

                    attributes.Add(new StrengthenAttributeItemData
                    {
                        kDesc = TR.Value(ms_strengthen_desc_pre[3]),
                        iCurValue = m_DisMagDef,
                        valueFormat = "{0}",
                    });
                }
            }
            itemData.BaseProp.props[(int)EEquipProp.IgnoreMagicDefense] = m_DisMagDef;

            int m_IgnorePhysicsDefenseRate = 0;
            if (_IsArmy((ItemTable.eSubType)itemData.SubType))
            {
                m_IgnorePhysicsDefenseRate = (int)(armStrMod2 * armClQaMod2 * 10000 + 0.50f);

                attributes.Add(new StrengthenAttributeItemData
                {
                    kDesc = TR.Value(ms_strengthen_desc_pre[4]),
                    iCurValue = m_IgnorePhysicsDefenseRate * 0.01f,
                    valueFormat = "{0:F2}%",
                });
            }
            itemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefenseRate] = m_IgnorePhysicsDefenseRate;

            int m_iIgnoreMagicDefenseRate = 0;
            if (_IsJewelry((ItemTable.eSubType)itemData.SubType))
            {
                m_iIgnoreMagicDefenseRate = (int)(jewStrMod2 * jewClQaMod2 * 10000 + 0.50f);

                attributes.Add(new StrengthenAttributeItemData
                {
                    kDesc = TR.Value(ms_strengthen_desc_pre[5]),
                    iCurValue = m_iIgnoreMagicDefenseRate * 0.01f,
                    valueFormat = "{0:F2}%",
                });
            }
            itemData.BaseProp.props[(int)EEquipProp.IgnoreMagicDefenseRate] = m_iIgnoreMagicDefenseRate;

            itemData.StrengthenLevel = iStrength;
        }

        public List<StrengthenAttributeItemData> Attributes
        {
            get
            {
                return attributes;
            }
        }

        public ItemData GetItemData()
        {
           return itemData;
        }

        #region _inner_interface
        bool _IsJewelry(ProtoTable.ItemTable.eSubType eSubType)
        {
            switch(eSubType)
            {
                case ItemTable.eSubType.RING:
                case ItemTable.eSubType.NECKLASE:
                case ItemTable.eSubType.BRACELET:
                    return true;
            }

            return false;
        }

        bool _IsArmy(ProtoTable.ItemTable.eSubType eSubType)
        {
            switch (eSubType)
            {
                case ItemTable.eSubType.HEAD:
                case ItemTable.eSubType.CHEST:
                case ItemTable.eSubType.BELT:
                case ItemTable.eSubType.LEG:
                case ItemTable.eSubType.BOOT:
                    return true;
            }

            return false;
        }

        bool _IsWeapon(ProtoTable.ItemTable.eSubType eSubType)
        {
            return eSubType == ItemTable.eSubType.WEAPON;
        }

        double _GetWpStrMode(int iLevel)
        {
            if (iLevel < ms_min_level || iLevel > ms_max_level)
            {
                return 0.0;
            }

            if (iLevel - 1 < 0 || iLevel > equipStrMode.WpStrenthMod.Count)
            {
                return 0.0;
            }

            return equipStrMode.WpStrenthMod[iLevel - 1] * 0.01;
        }

        double _GetWpClQaMod()
        {
            if (itemData.Quality > ItemTable.eColor.CL_NONE && itemData.Quality <= ItemTable.eColor.YELLOW)
            {
                if (itemData.Quality > 0 && (int)itemData.Quality <= equipStrMode.WpColorQaMod.Count)
                {
                    return equipStrMode.WpColorQaMod[(int)itemData.Quality - 1] * 0.01;
                }
            }
            return 0.0;
        }

        double _GetWpClQbMod()
        {
            if (itemData.Quality > ItemTable.eColor.CL_NONE && itemData.Quality <= ItemTable.eColor.YELLOW)
            {
                if (itemData.Quality > 0 && (int)itemData.Quality <= equipStrMode.WpColorQbMod.Count)
                {
                    return equipStrMode.WpColorQbMod[(int)itemData.Quality - 1] * 0.01;
                }
            }
            return 0.0;
        }

        double _GetWpPhyMod()
        {
            ms_arr_mod[0] = equipStrMode.HugeSword;
            ms_arr_mod[1] = equipStrMode.Katana;
            ms_arr_mod[2] = equipStrMode.ShortSword;
            ms_arr_mod[3] = equipStrMode.BeamSword;
            ms_arr_mod[4] = equipStrMode.Blunt;

            ms_arr_mod[5] = equipStrMode.Revolver;
            ms_arr_mod[6] = equipStrMode.CrossBow;
            ms_arr_mod[7] = equipStrMode.HandCannon;
            ms_arr_mod[8] = equipStrMode.AutoRifle;
            ms_arr_mod[9] = equipStrMode.AutoPistal;

            ms_arr_mod[10] = equipStrMode.MagicStick;
            ms_arr_mod[11] = equipStrMode.Twig;
            ms_arr_mod[12] = equipStrMode.Pike;
            ms_arr_mod[13] = equipStrMode.Stick;
            ms_arr_mod[14] = equipStrMode.Besom;

            ms_arr_mod[15] = equipStrMode.Glove;
            ms_arr_mod[16] = equipStrMode.Bikai;
            ms_arr_mod[17] = equipStrMode.Claw;
            ms_arr_mod[18] = equipStrMode.Ofg;
            ms_arr_mod[19] = equipStrMode.East_stick;
            ms_arr_mod[20] = equipStrMode.SICKLE;
            ms_arr_mod[21] = equipStrMode.TOTEM;
            ms_arr_mod[22] = equipStrMode.AXE;
            ms_arr_mod[23] = equipStrMode.BEADS;
            ms_arr_mod[24] = equipStrMode.CROSS;


            if (itemData.ThirdType > 0 && (int)itemData.ThirdType <= ms_arr_mod.Length)
            {
                if (ms_arr_mod[(int)itemData.ThirdType - 1].Count > 0)
                {
                    return ms_arr_mod[(int)itemData.ThirdType - 1][0] * 0.01;
                }
            }

            return 0.0;
        }

        double _GetWpMagMod()
        {
            ms_arr_mod[0] = equipStrMode.HugeSword;
            ms_arr_mod[1] = equipStrMode.Katana;
            ms_arr_mod[2] = equipStrMode.ShortSword;
            ms_arr_mod[3] = equipStrMode.BeamSword;
            ms_arr_mod[4] = equipStrMode.Blunt;

            ms_arr_mod[5] = equipStrMode.Revolver;
            ms_arr_mod[6] = equipStrMode.CrossBow;
            ms_arr_mod[7] = equipStrMode.HandCannon;
            ms_arr_mod[8] = equipStrMode.AutoRifle;
            ms_arr_mod[9] = equipStrMode.AutoPistal;

            ms_arr_mod[10] = equipStrMode.MagicStick;
            ms_arr_mod[11] = equipStrMode.Twig;
            ms_arr_mod[12] = equipStrMode.Pike;
            ms_arr_mod[13] = equipStrMode.Stick;
            ms_arr_mod[14] = equipStrMode.Besom;

            ms_arr_mod[15] = equipStrMode.Glove;
            ms_arr_mod[16] = equipStrMode.Bikai;
            ms_arr_mod[17] = equipStrMode.Claw;
            ms_arr_mod[18] = equipStrMode.Ofg;
            ms_arr_mod[19] = equipStrMode.East_stick;
            ms_arr_mod[20] = equipStrMode.SICKLE;
            ms_arr_mod[21] = equipStrMode.TOTEM;
            ms_arr_mod[22] = equipStrMode.AXE;
            ms_arr_mod[23] = equipStrMode.BEADS;
            ms_arr_mod[24] = equipStrMode.CROSS;


            if (itemData.ThirdType > 0 && (int)itemData.ThirdType <= ms_arr_mod.Length)
            {
                if (ms_arr_mod[(int)itemData.ThirdType - 1].Count > 1)
                {
                    return ms_arr_mod[(int)itemData.ThirdType - 1][1] * 0.01;
                }
            }

            return 0.0;
        }

        double _GetArmStrMod(int iLevel, EquipStrModTable equipStrMode)
        {
            if (iLevel < ms_min_level || iLevel > ms_max_level)
            {
                return 0;
            }

            if (iLevel - 1 < 0 || iLevel > equipStrMode.ArmStrenthMod.Count)
            {
                return 0;
            }

            return equipStrMode.ArmStrenthMod[iLevel - 1] * 0.01;
        }

        double _GetArmClQaMod(EquipStrModTable equipStrMode)
        {
            if (itemData.Quality > ItemTable.eColor.CL_NONE && itemData.Quality <= ItemTable.eColor.YELLOW)
            {
                if (itemData.Quality > 0 && (int)itemData.Quality <= equipStrMode.ArmColorQaMod.Count)
                {
                    return equipStrMode.ArmColorQaMod[(int)itemData.Quality - 1] * 0.01;
                }
            }
            return 0.0;
        }

        double _GetArmClQbMod()
        {
            if (itemData.Quality > ItemTable.eColor.CL_NONE && itemData.Quality <= ItemTable.eColor.YELLOW)
            {
                if (itemData.Quality > 0 && (int)itemData.Quality <= equipStrMode.ArmColorQbMod.Count)
                {
                    return equipStrMode.ArmColorQbMod[(int)itemData.Quality - 1] * 0.01;
                }
            }
            return 0.0;
        }

        double _GetJewStrMod(int iLevel,EquipStrModTable equipStrMode)
        {
            if (iLevel < ms_min_level || iLevel > ms_max_level)
            {
                return 0;
            }

            if (iLevel - 1 < 0 || iLevel > equipStrMode.JewStrenthMod.Count)
            {
                return 0;
            }

            return equipStrMode.JewStrenthMod[iLevel - 1] * 0.01;
        }

        double _GetJewClQaMod(EquipStrModTable equipStrMode)
        {
            if (itemData.Quality > ItemTable.eColor.CL_NONE && itemData.Quality <= ItemTable.eColor.YELLOW)
            {
                if (itemData.Quality > 0 && (int)itemData.Quality <= equipStrMode.JewColorQaMod.Count)
                {
                    return equipStrMode.JewColorQaMod[(int)itemData.Quality - 1] * 0.01;
                }
            }
            return 0.00;
        }

        double _GetJewClQbMod()
        {
            if (itemData.Quality > ItemTable.eColor.CL_NONE && itemData.Quality <= ItemTable.eColor.YELLOW)
            {
                if (itemData.Quality > 0 && (int)itemData.Quality <= equipStrMode.JewColorQbMod.Count)
                {
                    return equipStrMode.JewColorQbMod[(int)itemData.Quality - 1] * 0.01;
                }
            }
            return 0.0;
        }

        double _GetWpStrModeIndependence(int iLevel)
        {
            if (iLevel < ms_min_level || iLevel > ms_max_level)
            {
                return 0.0;
            }

            if (iLevel - 1 < 0 || iLevel > equipStrModeIndependence.WpStrenthMod_1.Count)
            {
                return 0.0;
            }

            return equipStrModeIndependence.WpStrenthMod_1[iLevel - 1] * 0.01;
        }

        double _GetWpClQaModIndependence()
        {
            if (itemData.Quality > ItemTable.eColor.CL_NONE && itemData.Quality <= ItemTable.eColor.YELLOW)
            {
                if (itemData.Quality > 0 && (int)itemData.Quality <= equipStrModeIndependence.WpColorQaMod_1.Count)
                {
                    return equipStrModeIndependence.WpColorQaMod_1[(int)itemData.Quality - 1] * 0.01;
                }
            }
            return 0.0;
        }

        double _GetWpClQbModIndependence()
        {
            if (itemData.Quality > ItemTable.eColor.CL_NONE && itemData.Quality <= ItemTable.eColor.YELLOW)
            {
                if (itemData.Quality > 0 && (int)itemData.Quality <= equipStrModeIndependence.WpColorQbMod_1.Count)
                {
                    return equipStrModeIndependence.WpColorQbMod_1[(int)itemData.Quality - 1] * 0.01;
                }
            }
            return 0.0;
        }

        double _GetWpPhyModIndependence()
        {
            return equipStrModeIndependence.EquipMod[0] * 0.01;
        }
        #endregion
    }
}