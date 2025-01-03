using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world -> client  返回兼并请求
	/// </summary>
	[AdvancedInspector.Descriptor("world -> client  返回兼并请求", "world -> client  返回兼并请求")]
	public class WorldGuildReceiveMergerRequestRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601982;
		public UInt32 Sequence;
		/// <summary>
		/// 是否有兼并请求 0没有 1有
		/// </summary>
		[AdvancedInspector.Descriptor("是否有兼并请求 0没有 1有", "是否有兼并请求 0没有 1有")]
		public byte isHave;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, isHave);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref isHave);
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
