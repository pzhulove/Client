using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 神器罐子折扣信息请求
	/// </summary>
	[AdvancedInspector.Descriptor("神器罐子折扣信息请求", "神器罐子折扣信息请求")]
	public class SceneArtifactJarDiscountInfoReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 507402;
		public UInt32 Sequence;
		/// <summary>
		///  运营活动id
		/// </summary>
		[AdvancedInspector.Descriptor(" 运营活动id", " 运营活动id")]
		public UInt32 opActId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, opActId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
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
