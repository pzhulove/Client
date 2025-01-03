using System;
using System.Text;

namespace Mock.Protocol
{

	public class RollDropResultItem : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		/// roll物品索引
		/// </summary>
		[AdvancedInspector.Descriptor("roll物品索引", "roll物品索引")]
		public byte rollIndex;
		/// <summary>
		/// 请求类型
		/// </summary>
		[AdvancedInspector.Descriptor("请求类型", "请求类型")]
		public byte opType;
		/// <summary>
		/// roll点
		/// </summary>
		[AdvancedInspector.Descriptor("roll点", "roll点")]
		public UInt32 point;
		/// <summary>
		/// 玩家id
		/// </summary>
		[AdvancedInspector.Descriptor("玩家id", "玩家id")]
		public UInt64 playerId;
		/// <summary>
		/// 玩家名字
		/// </summary>
		[AdvancedInspector.Descriptor("玩家名字", "玩家名字")]
		public string playerName;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, rollIndex);
			BaseDLL.encode_int8(buffer, ref pos_, opType);
			BaseDLL.encode_uint32(buffer, ref pos_, point);
			BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
			BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref rollIndex);
			BaseDLL.decode_int8(buffer, ref pos_, ref opType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref point);
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
