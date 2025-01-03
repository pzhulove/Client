using System;
using System.Text;

namespace Mock.Protocol
{

	public class RaceEquip : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 uid;

		public UInt32 id;

		public UInt32 pos;

		public UInt32 phyAtk;

		public UInt32 magAtk;

		public UInt32 phydef;

		public UInt32 magdef;

		public UInt32 strenth;

		public UInt32 stamina;

		public UInt32 intellect;

		public UInt32 spirit;

		public RaceItemRandProperty[] properties = null;

		public UInt32 magicCard;

		public UInt32 disphyAtk;

		public UInt32 disMagAtk;

		public UInt32 disphydef;

		public UInt32 dismagdef;

		public byte strengthen;

		public UInt32 fashionAttrId;

		public UInt32 phyDefPercent;

		public UInt32 magDefPercent;

		public UInt32 preciousBeadId;

		public byte isTimeLimit;

		public Int32[] atkPropEx = new Int32[0];
		/// <summary>
		///  属强EquipExProp	
		/// </summary>
		[AdvancedInspector.Descriptor(" 属强EquipExProp	", " 属强EquipExProp	")]
		public Int32[] strPropEx = new Int32[0];
		/// <summary>
		///  属抗EquipExProp
		/// </summary>
		[AdvancedInspector.Descriptor(" 属抗EquipExProp", " 属抗EquipExProp")]
		public Int32[] defPropEx = new Int32[0];

		public Int32 abnormalResistsTotal;
		/// <summary>
		///  异抗EquipAbnormalResist
		/// </summary>
		[AdvancedInspector.Descriptor(" 异抗EquipAbnormalResist", " 异抗EquipAbnormalResist")]
		public Int32[] abnormalResists = new Int32[0];

		public RacePrecBead[] mountPrecBeads = null;
		/// <summary>
		/// 附魔卡等级
		/// </summary>
		[AdvancedInspector.Descriptor("附魔卡等级", "附魔卡等级")]
		public byte magicCardLv;
		/// <summary>
		/// 装备类型:普通0/带气息1/红字2	
		/// </summary>
		[AdvancedInspector.Descriptor("装备类型:普通0/带气息1/红字2	", "装备类型:普通0/带气息1/红字2	")]
		public byte equipType;
		/// <summary>
		/// 红字装备增幅类型:力量0/智力1/体力2/精神3
		/// </summary>
		[AdvancedInspector.Descriptor("红字装备增幅类型:力量0/智力1/体力2/精神3", "红字装备增幅类型:力量0/智力1/体力2/精神3")]
		public byte enhanceType;
		/// <summary>
		/// 红字装备增幅数值
		/// </summary>
		[AdvancedInspector.Descriptor("红字装备增幅数值", "红字装备增幅数值")]
		public UInt32 enhanceNum;
		/// <summary>
		/// 铭文
		/// </summary>
		[AdvancedInspector.Descriptor("铭文", "铭文")]
		public UInt32[] inscriptionIds = new UInt32[0];
		/// <summary>
		/// 独立攻击力
		/// </summary>
		[AdvancedInspector.Descriptor("独立攻击力", "独立攻击力")]
		public UInt32 independAtk;
		/// <summary>
		/// 独立攻击力强化加成
		/// </summary>
		[AdvancedInspector.Descriptor("独立攻击力强化加成", "独立攻击力强化加成")]
		public UInt32 indpendAtkStreng;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, uid);
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, pos);
			BaseDLL.encode_uint32(buffer, ref pos_, phyAtk);
			BaseDLL.encode_uint32(buffer, ref pos_, magAtk);
			BaseDLL.encode_uint32(buffer, ref pos_, phydef);
			BaseDLL.encode_uint32(buffer, ref pos_, magdef);
			BaseDLL.encode_uint32(buffer, ref pos_, strenth);
			BaseDLL.encode_uint32(buffer, ref pos_, stamina);
			BaseDLL.encode_uint32(buffer, ref pos_, intellect);
			BaseDLL.encode_uint32(buffer, ref pos_, spirit);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)properties.Length);
			for(int i = 0; i < properties.Length; i++)
			{
				properties[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, magicCard);
			BaseDLL.encode_uint32(buffer, ref pos_, disphyAtk);
			BaseDLL.encode_uint32(buffer, ref pos_, disMagAtk);
			BaseDLL.encode_uint32(buffer, ref pos_, disphydef);
			BaseDLL.encode_uint32(buffer, ref pos_, dismagdef);
			BaseDLL.encode_int8(buffer, ref pos_, strengthen);
			BaseDLL.encode_uint32(buffer, ref pos_, fashionAttrId);
			BaseDLL.encode_uint32(buffer, ref pos_, phyDefPercent);
			BaseDLL.encode_uint32(buffer, ref pos_, magDefPercent);
			BaseDLL.encode_uint32(buffer, ref pos_, preciousBeadId);
			BaseDLL.encode_int8(buffer, ref pos_, isTimeLimit);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)atkPropEx.Length);
			for(int i = 0; i < atkPropEx.Length; i++)
			{
				BaseDLL.encode_int32(buffer, ref pos_, atkPropEx[i]);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)strPropEx.Length);
			for(int i = 0; i < strPropEx.Length; i++)
			{
				BaseDLL.encode_int32(buffer, ref pos_, strPropEx[i]);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)defPropEx.Length);
			for(int i = 0; i < defPropEx.Length; i++)
			{
				BaseDLL.encode_int32(buffer, ref pos_, defPropEx[i]);
			}
			BaseDLL.encode_int32(buffer, ref pos_, abnormalResistsTotal);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)abnormalResists.Length);
			for(int i = 0; i < abnormalResists.Length; i++)
			{
				BaseDLL.encode_int32(buffer, ref pos_, abnormalResists[i]);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mountPrecBeads.Length);
			for(int i = 0; i < mountPrecBeads.Length; i++)
			{
				mountPrecBeads[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_int8(buffer, ref pos_, magicCardLv);
			BaseDLL.encode_int8(buffer, ref pos_, equipType);
			BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
			BaseDLL.encode_uint32(buffer, ref pos_, enhanceNum);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)inscriptionIds.Length);
			for(int i = 0; i < inscriptionIds.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, inscriptionIds[i]);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, independAtk);
			BaseDLL.encode_uint32(buffer, ref pos_, indpendAtkStreng);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref pos);
			BaseDLL.decode_uint32(buffer, ref pos_, ref phyAtk);
			BaseDLL.decode_uint32(buffer, ref pos_, ref magAtk);
			BaseDLL.decode_uint32(buffer, ref pos_, ref phydef);
			BaseDLL.decode_uint32(buffer, ref pos_, ref magdef);
			BaseDLL.decode_uint32(buffer, ref pos_, ref strenth);
			BaseDLL.decode_uint32(buffer, ref pos_, ref stamina);
			BaseDLL.decode_uint32(buffer, ref pos_, ref intellect);
			BaseDLL.decode_uint32(buffer, ref pos_, ref spirit);
			UInt16 propertiesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref propertiesCnt);
			properties = new RaceItemRandProperty[propertiesCnt];
			for(int i = 0; i < properties.Length; i++)
			{
				properties[i] = new RaceItemRandProperty();
				properties[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref magicCard);
			BaseDLL.decode_uint32(buffer, ref pos_, ref disphyAtk);
			BaseDLL.decode_uint32(buffer, ref pos_, ref disMagAtk);
			BaseDLL.decode_uint32(buffer, ref pos_, ref disphydef);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dismagdef);
			BaseDLL.decode_int8(buffer, ref pos_, ref strengthen);
			BaseDLL.decode_uint32(buffer, ref pos_, ref fashionAttrId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref phyDefPercent);
			BaseDLL.decode_uint32(buffer, ref pos_, ref magDefPercent);
			BaseDLL.decode_uint32(buffer, ref pos_, ref preciousBeadId);
			BaseDLL.decode_int8(buffer, ref pos_, ref isTimeLimit);
			UInt16 atkPropExCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref atkPropExCnt);
			atkPropEx = new Int32[atkPropExCnt];
			for(int i = 0; i < atkPropEx.Length; i++)
			{
				BaseDLL.decode_int32(buffer, ref pos_, ref atkPropEx[i]);
			}
			UInt16 strPropExCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref strPropExCnt);
			strPropEx = new Int32[strPropExCnt];
			for(int i = 0; i < strPropEx.Length; i++)
			{
				BaseDLL.decode_int32(buffer, ref pos_, ref strPropEx[i]);
			}
			UInt16 defPropExCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref defPropExCnt);
			defPropEx = new Int32[defPropExCnt];
			for(int i = 0; i < defPropEx.Length; i++)
			{
				BaseDLL.decode_int32(buffer, ref pos_, ref defPropEx[i]);
			}
			BaseDLL.decode_int32(buffer, ref pos_, ref abnormalResistsTotal);
			UInt16 abnormalResistsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref abnormalResistsCnt);
			abnormalResists = new Int32[abnormalResistsCnt];
			for(int i = 0; i < abnormalResists.Length; i++)
			{
				BaseDLL.decode_int32(buffer, ref pos_, ref abnormalResists[i]);
			}
			UInt16 mountPrecBeadsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref mountPrecBeadsCnt);
			mountPrecBeads = new RacePrecBead[mountPrecBeadsCnt];
			for(int i = 0; i < mountPrecBeads.Length; i++)
			{
				mountPrecBeads[i] = new RacePrecBead();
				mountPrecBeads[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_int8(buffer, ref pos_, ref magicCardLv);
			BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
			BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref enhanceNum);
			UInt16 inscriptionIdsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref inscriptionIdsCnt);
			inscriptionIds = new UInt32[inscriptionIdsCnt];
			for(int i = 0; i < inscriptionIds.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionIds[i]);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref independAtk);
			BaseDLL.decode_uint32(buffer, ref pos_, ref indpendAtkStreng);
		}


		#endregion

	}

}
