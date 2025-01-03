using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 比赛记录
	/// </summary>
	[AdvancedInspector.Descriptor("比赛记录", "比赛记录")]
	public class ChampionBattleRecord : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		/// 第几场比赛
		/// </summary>
		[AdvancedInspector.Descriptor("第几场比赛", "第几场比赛")]
		public UInt32 order;
		/// <summary>
		/// 比赛ID
		/// </summary>
		[AdvancedInspector.Descriptor("比赛ID", "比赛ID")]
		public UInt64 raceID;
		/// <summary>
		/// 胜者id
		/// </summary>
		[AdvancedInspector.Descriptor("胜者id", "胜者id")]
		public UInt64 winner;
		/// <summary>
		/// 是否结束 0是未结束 1是已结束
		/// </summary>
		[AdvancedInspector.Descriptor("是否结束 0是未结束 1是已结束", "是否结束 0是未结束 1是已结束")]
		public byte isEnd;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, order);
			BaseDLL.encode_uint64(buffer, ref pos_, raceID);
			BaseDLL.encode_uint64(buffer, ref pos_, winner);
			BaseDLL.encode_int8(buffer, ref pos_, isEnd);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref order);
			BaseDLL.decode_uint64(buffer, ref pos_, ref raceID);
			BaseDLL.decode_uint64(buffer, ref pos_, ref winner);
			BaseDLL.decode_int8(buffer, ref pos_, ref isEnd);
		}


		#endregion

	}

}
