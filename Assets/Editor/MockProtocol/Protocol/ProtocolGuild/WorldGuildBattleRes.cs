using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求公会战返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求公会战返回", " 请求公会战返回")]
	public class WorldGuildBattleRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601943;
		public UInt32 Sequence;

		public UInt32 result;

		public byte terrId;

		public UInt32 enrollSize;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_int8(buffer, ref pos_, terrId);
			BaseDLL.encode_uint32(buffer, ref pos_, enrollSize);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			BaseDLL.decode_int8(buffer, ref pos_, ref terrId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref enrollSize);
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
