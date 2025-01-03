using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求发红包
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求发红包", " 请求发红包")]
	public class WorldSendRedPacketReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607306;
		public UInt32 Sequence;
		/// <summary>
		///  id
		/// </summary>
		[AdvancedInspector.Descriptor(" id", " id")]
		public UInt64 id;
		/// <summary>
		///  红包数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包数量", " 红包数量")]
		public byte num;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			BaseDLL.encode_int8(buffer, ref pos_, num);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			BaseDLL.decode_int8(buffer, ref pos_, ref num);
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
