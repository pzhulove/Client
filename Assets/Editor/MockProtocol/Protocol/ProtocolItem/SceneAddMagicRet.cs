using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 装备uid
	/// </summary>
	/// <summary>
	/// 附魔请求返回
	/// </summary>
	[AdvancedInspector.Descriptor("附魔请求返回", "附魔请求返回")]
	public class SceneAddMagicRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500945;
		public UInt32 Sequence;

		public UInt32 code;
		/// <summary>
		/// 返回码
		/// </summary>
		[AdvancedInspector.Descriptor("返回码", "返回码")]
		public UInt64 itemUid;
		/// <summary>
		/// 附魔的道具
		/// </summary>
		[AdvancedInspector.Descriptor("附魔的道具", "附魔的道具")]
		public UInt32 cardId;
		/// <summary>
		/// 附魔的附魔卡表ID
		/// </summary>
		[AdvancedInspector.Descriptor("附魔的附魔卡表ID", "附魔的附魔卡表ID")]
		public byte cardLev;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
			BaseDLL.encode_uint32(buffer, ref pos_, cardId);
			BaseDLL.encode_int8(buffer, ref pos_, cardLev);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref cardId);
			BaseDLL.decode_int8(buffer, ref pos_, ref cardLev);
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
