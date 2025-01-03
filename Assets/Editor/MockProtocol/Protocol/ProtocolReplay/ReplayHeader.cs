using System;
using System.Text;

namespace Mock.Protocol
{

	public class ReplayHeader : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 magicCode;

		public UInt32 version;

		public UInt32 startTime;

		public UInt64 sessionId;

		public byte raceType;

		public UInt32 pkDungeonId;

		public UInt32 duration;

		public RacePlayerInfo[] players = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, magicCode);
			BaseDLL.encode_uint32(buffer, ref pos_, version);
			BaseDLL.encode_uint32(buffer, ref pos_, startTime);
			BaseDLL.encode_uint64(buffer, ref pos_, sessionId);
			BaseDLL.encode_int8(buffer, ref pos_, raceType);
			BaseDLL.encode_uint32(buffer, ref pos_, pkDungeonId);
			BaseDLL.encode_uint32(buffer, ref pos_, duration);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)players.Length);
			for(int i = 0; i < players.Length; i++)
			{
				players[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref magicCode);
			BaseDLL.decode_uint32(buffer, ref pos_, ref version);
			BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
			BaseDLL.decode_uint64(buffer, ref pos_, ref sessionId);
			BaseDLL.decode_int8(buffer, ref pos_, ref raceType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref pkDungeonId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref duration);
			UInt16 playersCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref playersCnt);
			players = new RacePlayerInfo[playersCnt];
			for(int i = 0; i < players.Length; i++)
			{
				players[i] = new RacePlayerInfo();
				players[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
