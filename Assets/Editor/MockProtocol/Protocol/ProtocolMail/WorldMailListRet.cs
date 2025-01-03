using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  server->client 返回邮件列表
	/// </summary>
	[AdvancedInspector.Descriptor(" server->client 返回邮件列表", " server->client 返回邮件列表")]
	public class WorldMailListRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601503;
		public UInt32 Sequence;

		public MailTitleInfo[] mails = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mails.Length);
			for(int i = 0; i < mails.Length; i++)
			{
				mails[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 mailsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref mailsCnt);
			mails = new MailTitleInfo[mailsCnt];
			for(int i = 0; i < mails.Length; i++)
			{
				mails[i] = new MailTitleInfo();
				mails[i].decode(buffer, ref pos_);
			}
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
