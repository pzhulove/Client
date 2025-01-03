using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->server 删除邮件
	/// </summary>
	[AdvancedInspector.Descriptor(" client->server 删除邮件", " client->server 删除邮件")]
	public class WorldDeleteMail : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601510;
		public UInt32 Sequence;
		/// <summary>
		/// 邮件ID列表
		/// </summary>
		[AdvancedInspector.Descriptor("邮件ID列表", "邮件ID列表")]
		public UInt64[] ids = new UInt64[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)ids.Length);
			for(int i = 0; i < ids.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, ids[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 idsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref idsCnt);
			ids = new UInt64[idsCnt];
			for(int i = 0; i < ids.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref ids[i]);
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
