using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 请求快捷使用关卡道具
	/// </summary>
	[AdvancedInspector.Descriptor("请求快捷使用关卡道具", "请求快捷使用关卡道具")]
	public class SceneQuickUseItemReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500950;
		public UInt32 Sequence;

		public byte idx;

		public UInt32 dungenid;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, idx);
			BaseDLL.encode_uint32(buffer, ref pos_, dungenid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref idx);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dungenid);
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
