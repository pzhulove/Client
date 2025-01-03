using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world -> client 领取查询经验包返回
	/// </summary>
	[AdvancedInspector.Descriptor("world -> client 领取查询经验包返回", "world -> client 领取查询经验包返回")]
	public class WorldAventurePassExpPackRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609508;
		public UInt32 Sequence;
		/// <summary>
		/// 0 未解锁 1已解锁未领取 2已领取
		/// </summary>
		[AdvancedInspector.Descriptor("0 未解锁 1已解锁未领取 2已领取", "0 未解锁 1已解锁未领取 2已领取")]
		public byte type;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
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
