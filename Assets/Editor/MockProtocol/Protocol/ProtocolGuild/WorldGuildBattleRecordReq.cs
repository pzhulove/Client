using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求领地战斗记录
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求领地战斗记录", " 请求领地战斗记录")]
	public class WorldGuildBattleRecordReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601948;
		public UInt32 Sequence;

		public byte isSelf;

		public Int32 startIndex;

		public UInt32 count;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, isSelf);
			BaseDLL.encode_int32(buffer, ref pos_, startIndex);
			BaseDLL.encode_uint32(buffer, ref pos_, count);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref isSelf);
			BaseDLL.decode_int32(buffer, ref pos_, ref startIndex);
			BaseDLL.decode_uint32(buffer, ref pos_, ref count);
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
