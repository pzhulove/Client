using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  返回捐赠日志
	/// </summary>
	[AdvancedInspector.Descriptor(" 返回捐赠日志", " 返回捐赠日志")]
	public class WorldGuildDonateLogRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601930;
		public UInt32 Sequence;

		public GuildDonateLog[] logs = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)logs.Length);
			for(int i = 0; i < logs.Length; i++)
			{
				logs[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 logsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref logsCnt);
			logs = new GuildDonateLog[logsCnt];
			for(int i = 0; i < logs.Length; i++)
			{
				logs[i] = new GuildDonateLog();
				logs[i].decode(buffer, ref pos_);
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
