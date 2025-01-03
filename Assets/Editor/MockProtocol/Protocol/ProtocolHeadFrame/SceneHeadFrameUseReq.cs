using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  使用头像框请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 使用头像框请求", " 使用头像框请求")]
	public class SceneHeadFrameUseReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509103;
		public UInt32 Sequence;
		/// <summary>
		///  头像框id
		/// </summary>
		[AdvancedInspector.Descriptor(" 头像框id", " 头像框id")]
		public UInt32 headFrameId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, headFrameId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref headFrameId);
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
