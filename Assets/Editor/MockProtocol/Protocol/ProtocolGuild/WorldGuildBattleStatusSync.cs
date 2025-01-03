using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  同步公会战状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 同步公会战状态", " 同步公会战状态")]
	public class WorldGuildBattleStatusSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601958;
		public UInt32 Sequence;
		/// <summary>
		///  类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 类型", " 类型")]
		public byte type;
		/// <summary>
		///  状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 状态", " 状态")]
		public byte status;
		/// <summary>
		///  状态存在时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 状态存在时间", " 状态存在时间")]
		public UInt32 time;
		/// <summary>
		///  公会战结束信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会战结束信息", " 公会战结束信息")]
		public GuildBattleEndInfo[] endInfo = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_int8(buffer, ref pos_, status);
			BaseDLL.encode_uint32(buffer, ref pos_, time);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)endInfo.Length);
			for(int i = 0; i < endInfo.Length; i++)
			{
				endInfo[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_int8(buffer, ref pos_, ref status);
			BaseDLL.decode_uint32(buffer, ref pos_, ref time);
			UInt16 endInfoCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref endInfoCnt);
			endInfo = new GuildBattleEndInfo[endInfoCnt];
			for(int i = 0; i < endInfo.Length; i++)
			{
				endInfo[i] = new GuildBattleEndInfo();
				endInfo[i].decode(buffer, ref pos_);
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
