using System;
using System.Text;

namespace Mock.Protocol
{

	public class WorldNotifyRaceStart : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 606701;
		public UInt32 Sequence;

		public UInt64 roleId;

		public UInt64 sessionId;

		public SockAddr addr = null;
		/// <summary>
		///  对应枚举（RaceType）
		/// </summary>
		[AdvancedInspector.Descriptor(" 对应枚举（RaceType）", " 对应枚举（RaceType）")]
		public byte raceType;

		public UInt32 dungeonId;

		public RacePlayerInfo[] players = null;
		/// <summary>
		///  是否记录日志
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否记录日志", " 是否记录日志")]
		public byte isRecordLog;
		/// <summary>
		///  是否记录录像
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否记录录像", " 是否记录录像")]
		public byte isRecordReplay;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			BaseDLL.encode_uint64(buffer, ref pos_, sessionId);
			addr.encode(buffer, ref pos_);
			BaseDLL.encode_int8(buffer, ref pos_, raceType);
			BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)players.Length);
			for(int i = 0; i < players.Length; i++)
			{
				players[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_int8(buffer, ref pos_, isRecordLog);
			BaseDLL.encode_int8(buffer, ref pos_, isRecordReplay);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref sessionId);
			addr.decode(buffer, ref pos_);
			BaseDLL.decode_int8(buffer, ref pos_, ref raceType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			UInt16 playersCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref playersCnt);
			players = new RacePlayerInfo[playersCnt];
			for(int i = 0; i < players.Length; i++)
			{
				players[i] = new RacePlayerInfo();
				players[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_int8(buffer, ref pos_, ref isRecordLog);
			BaseDLL.decode_int8(buffer, ref pos_, ref isRecordReplay);
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
