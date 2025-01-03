using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  战斗记录
	/// </summary>
	[AdvancedInspector.Descriptor(" 战斗记录", " 战斗记录")]
	public class PremiumLeagueRecordEntry : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  序号
		/// </summary>
		[AdvancedInspector.Descriptor(" 序号", " 序号")]
		public UInt32 index;
		/// <summary>
		///  时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 时间", " 时间")]
		public UInt32 time;
		/// <summary>
		///  胜利者
		/// </summary>
		[AdvancedInspector.Descriptor(" 胜利者", " 胜利者")]
		public PremiumLeagueRecordFighter winner = null;
		/// <summary>
		///  失败者
		/// </summary>
		[AdvancedInspector.Descriptor(" 失败者", " 失败者")]
		public PremiumLeagueRecordFighter loser = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, index);
			BaseDLL.encode_uint32(buffer, ref pos_, time);
			winner.encode(buffer, ref pos_);
			loser.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref index);
			BaseDLL.decode_uint32(buffer, ref pos_, ref time);
			winner.decode(buffer, ref pos_);
			loser.decode(buffer, ref pos_);
		}


		#endregion

	}

}
