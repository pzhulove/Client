using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  团本成员信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 团本成员信息", " 团本成员信息")]
	public class TeamCopyMember : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  玩家ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家ID", " 玩家ID")]
		public UInt64 playerId;
		/// <summary>
		///  玩家名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家名字", " 玩家名字")]
		public string playerName;
		/// <summary>
		///  玩家职业
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家职业", " 玩家职业")]
		public UInt32 playerOccu;
		/// <summary>
		///  觉醒
		/// </summary>
		[AdvancedInspector.Descriptor(" 觉醒", " 觉醒")]
		public UInt32 playerAwaken;
		/// <summary>
		///  玩家等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家等级", " 玩家等级")]
		public UInt32 playerLvl;
		/// <summary>
		///  职位
		/// </summary>
		[AdvancedInspector.Descriptor(" 职位", " 职位")]
		public UInt32 post;
		/// <summary>
		///  装备评分
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备评分", " 装备评分")]
		public UInt32 equipScore;
		/// <summary>
		///  座位
		/// </summary>
		[AdvancedInspector.Descriptor(" 座位", " 座位")]
		public UInt32 seatId;
		/// <summary>
		///  门票是否足够
		/// </summary>
		[AdvancedInspector.Descriptor(" 门票是否足够", " 门票是否足够")]
		public UInt32 ticketIsEnough;
		/// <summary>
		///  zone
		/// </summary>
		[AdvancedInspector.Descriptor(" zone", " zone")]
		public UInt32 zoneId;
		/// <summary>
		///  过期时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 过期时间", " 过期时间")]
		public UInt64 expireTime;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
			BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, playerOccu);
			BaseDLL.encode_uint32(buffer, ref pos_, playerAwaken);
			BaseDLL.encode_uint32(buffer, ref pos_, playerLvl);
			BaseDLL.encode_uint32(buffer, ref pos_, post);
			BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
			BaseDLL.encode_uint32(buffer, ref pos_, seatId);
			BaseDLL.encode_uint32(buffer, ref pos_, ticketIsEnough);
			BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
			BaseDLL.encode_uint64(buffer, ref pos_, expireTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
			UInt16 playerNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
			byte[] playerNameBytes = new byte[playerNameLen];
			for(int i = 0; i < playerNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
			}
			playerName = StringHelper.BytesToString(playerNameBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref playerOccu);
			BaseDLL.decode_uint32(buffer, ref pos_, ref playerAwaken);
			BaseDLL.decode_uint32(buffer, ref pos_, ref playerLvl);
			BaseDLL.decode_uint32(buffer, ref pos_, ref post);
			BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
			BaseDLL.decode_uint32(buffer, ref pos_, ref seatId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref ticketIsEnough);
			BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref expireTime);
		}


		#endregion

	}

}
