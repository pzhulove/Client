using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  战场报名请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 战场报名请求", " 战场报名请求")]
	public class BattleEnrollReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 2200005;
		public UInt32 Sequence;
		/// <summary>
		///  0取消报名，非0报名
		/// </summary>
		[AdvancedInspector.Descriptor(" 0取消报名，非0报名", " 0取消报名，非0报名")]
		public UInt32 isMatch;

		public UInt32 accId;

		public UInt64 roleId;

		public string playerName;

		public byte playerOccu;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, isMatch);
			BaseDLL.encode_uint32(buffer, ref pos_, accId);
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
			BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, playerOccu);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref isMatch);
			BaseDLL.decode_uint32(buffer, ref pos_, ref accId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			UInt16 playerNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
			byte[] playerNameBytes = new byte[playerNameLen];
			for(int i = 0; i < playerNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
			}
			playerName = StringHelper.BytesToString(playerNameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref playerOccu);
		}

		public UInt32 GetSequence()
		{
			return Sequence;
		}

		public void SetSequence(UInt32 sequence)
		{
			Sequence = sequence;
		}

		#endregion

	}

}
