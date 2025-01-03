using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  运营活动接任务请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 运营活动接任务请求", " 运营活动接任务请求")]
	public class SceneOpActivityGetCounterReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 507412;
		public UInt32 Sequence;
		/// <summary>
		///  运营活动id
		/// </summary>
		[AdvancedInspector.Descriptor(" 运营活动id", " 运营活动id")]
		public UInt32 opActId;
		/// <summary>
		///  计数id
		/// </summary>
		[AdvancedInspector.Descriptor(" 计数id", " 计数id")]
		public UInt32 counterId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, opActId);
			BaseDLL.encode_uint32(buffer, ref pos_, counterId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref counterId);
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
