using System;
using System.Text;

namespace Mock.Protocol
{

	public class ArtifactJarLotteryRecord : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  服务器名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 服务器名字", " 服务器名字")]
		public string serverName;
		/// <summary>
		///  玩家名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家名字", " 玩家名字")]
		public string playerName;
		/// <summary>
		///  记录时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 记录时间", " 记录时间")]
		public UInt64 recordTime;
		/// <summary>
		///  道具id
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具id", " 道具id")]
		public UInt32 itemId;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] serverNameBytes = StringHelper.StringToUTF8Bytes(serverName);
			BaseDLL.encode_string(buffer, ref pos_, serverNameBytes, (UInt16)(buffer.Length - pos_));
			byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
			BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint64(buffer, ref pos_, recordTime);
			BaseDLL.encode_uint32(buffer, ref pos_, itemId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 serverNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref serverNameLen);
			byte[] serverNameBytes = new byte[serverNameLen];
			for(int i = 0; i < serverNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref serverNameBytes[i]);
			}
			serverName = StringHelper.BytesToString(serverNameBytes);
			UInt16 playerNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
			byte[] playerNameBytes = new byte[playerNameLen];
			for(int i = 0; i < playerNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
			}
			playerName = StringHelper.BytesToString(playerNameBytes);
			BaseDLL.decode_uint64(buffer, ref pos_, ref recordTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
		}


		#endregion

	}

}
