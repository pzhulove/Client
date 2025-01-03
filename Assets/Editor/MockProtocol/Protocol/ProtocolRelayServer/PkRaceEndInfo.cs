using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  pk结算
	/// </summary>
	[AdvancedInspector.Descriptor(" pk结算", " pk结算")]
	public class PkRaceEndInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 gamesessionId;
		/// <summary>
		/// 所有玩家的结算信息
		/// </summary>
		[AdvancedInspector.Descriptor("所有玩家的结算信息", "所有玩家的结算信息")]
		public PkPlayerRaceEndInfo[] infoes = null;
		/// <summary>
		/// 录像评分
		/// </summary>
		[AdvancedInspector.Descriptor("录像评分", "录像评分")]
		public UInt32 replayScore;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, gamesessionId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infoes.Length);
			for(int i = 0; i < infoes.Length; i++)
			{
				infoes[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, replayScore);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref gamesessionId);
			UInt16 infoesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref infoesCnt);
			infoes = new PkPlayerRaceEndInfo[infoesCnt];
			for(int i = 0; i < infoes.Length; i++)
			{
				infoes[i] = new PkPlayerRaceEndInfo();
				infoes[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref replayScore);
		}


		#endregion

	}

}
