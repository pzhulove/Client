using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  运营活动接任务返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 运营活动接任务返回", " 运营活动接任务返回")]
	public class SceneOpActivityGetCounterRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 507413;
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
		/// <summary>
		///  计数值
		/// </summary>
		[AdvancedInspector.Descriptor(" 计数值", " 计数值")]
		public UInt32 counterValue;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, opActId);
			BaseDLL.encode_uint32(buffer, ref pos_, counterId);
			BaseDLL.encode_uint32(buffer, ref pos_, counterValue);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref counterId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref counterValue);
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
