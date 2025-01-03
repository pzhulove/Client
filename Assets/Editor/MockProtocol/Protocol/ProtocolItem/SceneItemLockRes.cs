using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  scene -> client装备加锁解锁返回
	/// </summary>
	[AdvancedInspector.Descriptor(" scene -> client装备加锁解锁返回", " scene -> client装备加锁解锁返回")]
	public class SceneItemLockRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501026;
		public UInt32 Sequence;
		/// <summary>
		///  道具uid
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具uid", " 道具uid")]
		public UInt64 itemUid;
		/// <summary>
		///  返回值
		/// </summary>
		[AdvancedInspector.Descriptor(" 返回值", " 返回值")]
		public UInt32 ret;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
			BaseDLL.encode_uint32(buffer, ref pos_, ret);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
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
