using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  吃鸡观战请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 吃鸡观战请求", " 吃鸡观战请求")]
	public class BattleObserveReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 2200017;
		public UInt32 Sequence;

		public UInt64 playerId;

		public UInt64 dstId;

		public UInt32 battleId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			BaseDLL.encode_uint64(buffer, ref pos_, dstId);
			BaseDLL.encode_uint32(buffer, ref pos_, battleId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref dstId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleId);
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
