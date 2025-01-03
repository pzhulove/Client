using System;
using System.Text;

namespace Mock.Protocol
{

	public class DungeonRaceEndInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 sessionId;

		public UInt32 dungeonId;

		public UInt32 usedTime;
		/// <summary>
		///  各玩家的结算信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 各玩家的结算信息", " 各玩家的结算信息")]
		public DungeonPlayerRaceEndInfo[] infoes = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, sessionId);
			BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			BaseDLL.encode_uint32(buffer, ref pos_, usedTime);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infoes.Length);
			for(int i = 0; i < infoes.Length; i++)
			{
				infoes[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref sessionId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref usedTime);
			UInt16 infoesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref infoesCnt);
			infoes = new DungeonPlayerRaceEndInfo[infoesCnt];
			for(int i = 0; i < infoes.Length; i++)
			{
				infoes[i] = new DungeonPlayerRaceEndInfo();
				infoes[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
