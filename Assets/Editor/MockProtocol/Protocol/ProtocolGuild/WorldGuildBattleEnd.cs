using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会战结束
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会战结束", " 公会战结束")]
	public class WorldGuildBattleEnd : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601954;
		public UInt32 Sequence;

		public GuildBattleEndInfo[] info = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)info.Length);
			for(int i = 0; i < info.Length; i++)
			{
				info[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 infoCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref infoCnt);
			info = new GuildBattleEndInfo[infoCnt];
			for(int i = 0; i < info.Length; i++)
			{
				info[i] = new GuildBattleEndInfo();
				info[i].decode(buffer, ref pos_);
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
