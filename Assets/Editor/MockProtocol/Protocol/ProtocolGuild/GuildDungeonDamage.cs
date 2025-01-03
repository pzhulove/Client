using System;
using System.Text;

namespace Mock.Protocol
{

	public class GuildDungeonDamage : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  排名
		/// </summary>
		[AdvancedInspector.Descriptor(" 排名", " 排名")]
		public UInt32 rank;
		/// <summary>
		///  伤害值
		/// </summary>
		[AdvancedInspector.Descriptor(" 伤害值", " 伤害值")]
		public UInt64 damageVal;
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

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, rank);
			BaseDLL.encode_uint64(buffer, ref pos_, damageVal);
			BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
			BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref rank);
			BaseDLL.decode_uint64(buffer, ref pos_, ref damageVal);
			BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
			UInt16 playerNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
			byte[] playerNameBytes = new byte[playerNameLen];
			for(int i = 0; i < playerNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
			}
			playerName = StringHelper.BytesToString(playerNameBytes);
		}


		#endregion

	}

}
