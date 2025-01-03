using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  查看玩家的信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 查看玩家的信息", " 查看玩家的信息")]
	public class PlayerWatchInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 id;

		public string name;

		public byte occu;

		public UInt16 level;

		public ItemBaseInfo[] equips = null;

		public ItemBaseInfo[] fashionEquips = null;

		public RetinueInfo retinue = null;

		public PkStatisticInfo pkInfo = null;

		public UInt32 pkValue;

		public UInt32 matchScore;
		/// <summary>
		///  vip等级
		/// </summary>
		[AdvancedInspector.Descriptor(" vip等级", " vip等级")]
		public byte vipLevel;
		/// <summary>
		///  公会称号
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会称号", " 公会称号")]
		public GuildTitle guildTitle = null;
		/// <summary>
		///  赛季段位等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 赛季段位等级", " 赛季段位等级")]
		public UInt32 seasonLevel;
		/// <summary>
		///  赛季段位星级
		/// </summary>
		[AdvancedInspector.Descriptor(" 赛季段位星级", " 赛季段位星级")]
		public UInt32 seasonStar;
		/// <summary>
		///  宠物
		/// </summary>
		[AdvancedInspector.Descriptor(" 宠物", " 宠物")]
		public PetBaseInfo[] pets = null;
		/// <summary>
		///  外观
		/// </summary>
		[AdvancedInspector.Descriptor(" 外观", " 外观")]
		public PlayerAvatar avatar = null;
		/// <summary>
		///  在线时间类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 在线时间类型", " 在线时间类型")]
		public byte activeTimeType;
		/// <summary>
		///  师傅类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 师傅类型", " 师傅类型")]
		public byte masterType;
		/// <summary>
		///  地区id
		/// </summary>
		[AdvancedInspector.Descriptor(" 地区id", " 地区id")]
		public byte regionId;
		/// <summary>
		///  宣言
		/// </summary>
		[AdvancedInspector.Descriptor(" 宣言", " 宣言")]
		public string declaration;
		/// <summary>
		/// 玩家标签信息
		/// </summary>
		[AdvancedInspector.Descriptor("玩家标签信息", "玩家标签信息")]
		public PlayerLabelInfo playerLabelInfo = null;
		/// <summary>
		/// 冒险队名
		/// </summary>
		[AdvancedInspector.Descriptor("冒险队名", "冒险队名")]
		public string adventureTeamName;
		/// <summary>
		/// 冒险队评级
		/// </summary>
		[AdvancedInspector.Descriptor("冒险队评级", "冒险队评级")]
		public string adventureTeamGrade;
		/// <summary>
		/// 冒险队排名
		/// </summary>
		[AdvancedInspector.Descriptor("冒险队排名", "冒险队排名")]
		public UInt32 adventureTeamRanking;
		/// <summary>
		/// 徽记
		/// </summary>
		[AdvancedInspector.Descriptor("徽记", "徽记")]
		public UInt32 emblemLevel;
		/// <summary>
		///  全身装备评分
		/// </summary>
		[AdvancedInspector.Descriptor(" 全身装备评分", " 全身装备评分")]
		public UInt32 totalEquipScore;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, occu);
			BaseDLL.encode_uint16(buffer, ref pos_, level);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equips.Length);
			for(int i = 0; i < equips.Length; i++)
			{
				equips[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)fashionEquips.Length);
			for(int i = 0; i < fashionEquips.Length; i++)
			{
				fashionEquips[i].encode(buffer, ref pos_);
			}
			retinue.encode(buffer, ref pos_);
			pkInfo.encode(buffer, ref pos_);
			BaseDLL.encode_uint32(buffer, ref pos_, pkValue);
			BaseDLL.encode_uint32(buffer, ref pos_, matchScore);
			BaseDLL.encode_int8(buffer, ref pos_, vipLevel);
			guildTitle.encode(buffer, ref pos_);
			BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
			BaseDLL.encode_uint32(buffer, ref pos_, seasonStar);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)pets.Length);
			for(int i = 0; i < pets.Length; i++)
			{
				pets[i].encode(buffer, ref pos_);
			}
			avatar.encode(buffer, ref pos_);
			BaseDLL.encode_int8(buffer, ref pos_, activeTimeType);
			BaseDLL.encode_int8(buffer, ref pos_, masterType);
			BaseDLL.encode_int8(buffer, ref pos_, regionId);
			byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
			BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
			playerLabelInfo.encode(buffer, ref pos_);
			byte[] adventureTeamNameBytes = StringHelper.StringToUTF8Bytes(adventureTeamName);
			BaseDLL.encode_string(buffer, ref pos_, adventureTeamNameBytes, (UInt16)(buffer.Length - pos_));
			byte[] adventureTeamGradeBytes = StringHelper.StringToUTF8Bytes(adventureTeamGrade);
			BaseDLL.encode_string(buffer, ref pos_, adventureTeamGradeBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, adventureTeamRanking);
			BaseDLL.encode_uint32(buffer, ref pos_, emblemLevel);
			BaseDLL.encode_uint32(buffer, ref pos_, totalEquipScore);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			BaseDLL.decode_uint16(buffer, ref pos_, ref level);
			UInt16 equipsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref equipsCnt);
			equips = new ItemBaseInfo[equipsCnt];
			for(int i = 0; i < equips.Length; i++)
			{
				equips[i] = new ItemBaseInfo();
				equips[i].decode(buffer, ref pos_);
			}
			UInt16 fashionEquipsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref fashionEquipsCnt);
			fashionEquips = new ItemBaseInfo[fashionEquipsCnt];
			for(int i = 0; i < fashionEquips.Length; i++)
			{
				fashionEquips[i] = new ItemBaseInfo();
				fashionEquips[i].decode(buffer, ref pos_);
			}
			retinue.decode(buffer, ref pos_);
			pkInfo.decode(buffer, ref pos_);
			BaseDLL.decode_uint32(buffer, ref pos_, ref pkValue);
			BaseDLL.decode_uint32(buffer, ref pos_, ref matchScore);
			BaseDLL.decode_int8(buffer, ref pos_, ref vipLevel);
			guildTitle.decode(buffer, ref pos_);
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonStar);
			UInt16 petsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref petsCnt);
			pets = new PetBaseInfo[petsCnt];
			for(int i = 0; i < pets.Length; i++)
			{
				pets[i] = new PetBaseInfo();
				pets[i].decode(buffer, ref pos_);
			}
			avatar.decode(buffer, ref pos_);
			BaseDLL.decode_int8(buffer, ref pos_, ref activeTimeType);
			BaseDLL.decode_int8(buffer, ref pos_, ref masterType);
			BaseDLL.decode_int8(buffer, ref pos_, ref regionId);
			UInt16 declarationLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
			byte[] declarationBytes = new byte[declarationLen];
			for(int i = 0; i < declarationLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
			}
			declaration = StringHelper.BytesToString(declarationBytes);
			playerLabelInfo.decode(buffer, ref pos_);
			UInt16 adventureTeamNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref adventureTeamNameLen);
			byte[] adventureTeamNameBytes = new byte[adventureTeamNameLen];
			for(int i = 0; i < adventureTeamNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref adventureTeamNameBytes[i]);
			}
			adventureTeamName = StringHelper.BytesToString(adventureTeamNameBytes);
			UInt16 adventureTeamGradeLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref adventureTeamGradeLen);
			byte[] adventureTeamGradeBytes = new byte[adventureTeamGradeLen];
			for(int i = 0; i < adventureTeamGradeLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref adventureTeamGradeBytes[i]);
			}
			adventureTeamGrade = StringHelper.BytesToString(adventureTeamGradeBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref adventureTeamRanking);
			BaseDLL.decode_uint32(buffer, ref pos_, ref emblemLevel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref totalEquipScore);
		}


		#endregion

	}

}
