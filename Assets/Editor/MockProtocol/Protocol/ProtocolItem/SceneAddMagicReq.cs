using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 附魔请求
	/// </summary>
	[AdvancedInspector.Descriptor("附魔请求", "附魔请求")]
	public class SceneAddMagicReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500944;
		public UInt32 Sequence;

		public UInt64 cardUid;
		/// <summary>
		/// 附魔卡uid
		/// </summary>
		[AdvancedInspector.Descriptor("附魔卡uid", "附魔卡uid")]
		public UInt64 itemUid;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, cardUid);
			BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref cardUid);
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
