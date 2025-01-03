using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  赏金联赛状态信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 赏金联赛状态信息", " 赏金联赛状态信息")]
	public class PremiumLeagueStatusInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  状态（对应PremiumLeagueStatus）
		/// </summary>
		[AdvancedInspector.Descriptor(" 状态（对应PremiumLeagueStatus）", " 状态（对应PremiumLeagueStatus）")]
		public byte status;
		/// <summary>
		///  开始时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 开始时间", " 开始时间")]
		public UInt32 startTime;
		/// <summary>
		///  结束时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 结束时间", " 结束时间")]
		public UInt32 endTime;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, status);
			BaseDLL.encode_uint32(buffer, ref pos_, startTime);
			BaseDLL.encode_uint32(buffer, ref pos_, endTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref status);
			BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
		}


		#endregion

	}

}
