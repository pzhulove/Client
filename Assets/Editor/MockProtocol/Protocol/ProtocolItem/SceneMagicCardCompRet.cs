using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 附魔卡B
	/// </summary>
	/// <summary>
	/// 附魔卡合成返回
	/// </summary>
	[AdvancedInspector.Descriptor("附魔卡合成返回", "附魔卡合成返回")]
	public class SceneMagicCardCompRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500947;
		public UInt32 Sequence;

		public UInt32 code;
		/// <summary>
		/// 返回码
		/// </summary>
		[AdvancedInspector.Descriptor("返回码", "返回码")]
		public UInt32 cardId;
		/// <summary>
		/// 合成的附魔卡id	
		/// </summary>
		[AdvancedInspector.Descriptor("合成的附魔卡id	", "合成的附魔卡id	")]
		public byte cardLev;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_uint32(buffer, ref pos_, cardId);
			BaseDLL.encode_int8(buffer, ref pos_, cardLev);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
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
