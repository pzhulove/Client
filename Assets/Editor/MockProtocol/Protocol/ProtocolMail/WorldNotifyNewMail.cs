using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  server->client 通知新邮件
	/// </summary>
	[AdvancedInspector.Descriptor(" server->client 通知新邮件", " server->client 通知新邮件")]
	public class WorldNotifyNewMail : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601509;
		public UInt32 Sequence;
		/// <summary>
		/// 邮件标题信息
		/// </summary>
		[AdvancedInspector.Descriptor("邮件标题信息", "邮件标题信息")]
		public MailTitleInfo info = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			info.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			info.decode(buffer, ref pos_);
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
