using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  返回公会战鼓舞信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 返回公会战鼓舞信息", " 返回公会战鼓舞信息")]
	public class WorldGuildBattleInspireInfoRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601964;
		public UInt32 Sequence;

		public UInt32 result;
		/// <summary>
		///  领地ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 领地ID", " 领地ID")]
		public byte terrId;
		/// <summary>
		///  鼓舞信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 鼓舞信息", " 鼓舞信息")]
		public GuildBattleInspireInfo[] inspireInfos = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_int8(buffer, ref pos_, terrId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)inspireInfos.Length);
			for(int i = 0; i < inspireInfos.Length; i++)
			{
				inspireInfos[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			BaseDLL.decode_int8(buffer, ref pos_, ref terrId);
			UInt16 inspireInfosCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref inspireInfosCnt);
			inspireInfos = new GuildBattleInspireInfo[inspireInfosCnt];
			for(int i = 0; i < inspireInfos.Length; i++)
			{
				inspireInfos[i] = new GuildBattleInspireInfo();
				inspireInfos[i].decode(buffer, ref pos_);
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
