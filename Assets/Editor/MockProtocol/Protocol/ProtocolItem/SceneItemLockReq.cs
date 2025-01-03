using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->scene装备加锁解锁
	/// </summary>
	[AdvancedInspector.Descriptor(" client->scene装备加锁解锁", " client->scene装备加锁解锁")]
	public class SceneItemLockReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501025;
		public UInt32 Sequence;
		/// <summary>
		///  操作类型，0解锁，否则加锁
		/// </summary>
		[AdvancedInspector.Descriptor(" 操作类型，0解锁，否则加锁", " 操作类型，0解锁，否则加锁")]
		public UInt32 opType;
		/// <summary>
		///  道具uid
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具uid", " 道具uid")]
		public UInt64 itemUid;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, opType);
			BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref opType);
			BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
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
