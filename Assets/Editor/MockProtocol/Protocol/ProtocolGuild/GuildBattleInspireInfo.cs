using System;
using System.Text;

namespace Mock.Protocol
{

	public class GuildBattleInspireInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
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
		///  鼓舞次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 鼓舞次数", " 鼓舞次数")]
		public UInt32 inspireNum;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
			BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, inspireNum);
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
			BaseDLL.decode_uint32(buffer, ref pos_, ref inspireNum);
		}


		#endregion

	}

}
