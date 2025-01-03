using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  返回申请入公会的列表
	/// </summary>
	[AdvancedInspector.Descriptor(" 返回申请入公会的列表", " 返回申请入公会的列表")]
	public class WorldGuildRequesterRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601910;
		public UInt32 Sequence;
		/// <summary>
		///  申请人列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 申请人列表", " 申请人列表")]
		public GuildRequesterInfo[] requesters = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)requesters.Length);
			for(int i = 0; i < requesters.Length; i++)
			{
				requesters[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 requestersCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref requestersCnt);
			requesters = new GuildRequesterInfo[requestersCnt];
			for(int i = 0; i < requesters.Length; i++)
			{
				requesters[i] = new GuildRequesterInfo();
				requesters[i].decode(buffer, ref pos_);
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
