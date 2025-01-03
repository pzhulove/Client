using System;
using System.Text;

namespace Mock.Protocol
{

	public class TeamCopyApplyProperty : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
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
		///  等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 等级", " 等级")]
		public UInt32 playerLevel;
		/// <summary>
		///  装备评分
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备评分", " 装备评分")]
		public UInt32 equipScore;
		/// <summary>
		///  是否金主(非0是金主)
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否金主(非0是金主)", " 是否金主(非0是金主)")]
		public UInt32 isGold;
		/// <summary>
		///  公会id
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会id", " 公会id")]
		public UInt64 guildId;
		/// <summary>
		///  区服id
		/// </summary>
		[AdvancedInspector.Descriptor(" 区服id", " 区服id")]
		public UInt32 zoneId;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
			BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, playerOccu);
			BaseDLL.encode_uint32(buffer, ref pos_, playerAwaken);
			BaseDLL.encode_uint32(buffer, ref pos_, playerLevel);
			BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
			BaseDLL.encode_uint32(buffer, ref pos_, isGold);
			BaseDLL.encode_uint64(buffer, ref pos_, guildId);
			BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
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
			BaseDLL.decode_uint32(buffer, ref pos_, ref playerLevel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
			BaseDLL.decode_uint32(buffer, ref pos_, ref isGold);
			BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
		}


		#endregion

	}

}
