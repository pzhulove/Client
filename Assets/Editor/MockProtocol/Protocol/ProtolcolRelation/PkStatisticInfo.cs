using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  Pk信息
	/// </summary>
	[AdvancedInspector.Descriptor(" Pk信息", " Pk信息")]
	public class PkStatisticInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  胜场数
		/// </summary>
		[AdvancedInspector.Descriptor(" 胜场数", " 胜场数")]
		public UInt32 totalWinNum;
		/// <summary>
		///  负场数
		/// </summary>
		[AdvancedInspector.Descriptor(" 负场数", " 负场数")]
		public UInt32 totalLoseNum;
		/// <summary>
		///  总场数
		/// </summary>
		[AdvancedInspector.Descriptor(" 总场数", " 总场数")]
		public UInt32 totalNum;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, totalWinNum);
			BaseDLL.encode_uint32(buffer, ref pos_, totalLoseNum);
			BaseDLL.encode_uint32(buffer, ref pos_, totalNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref totalWinNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref totalLoseNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref totalNum);
		}


		#endregion

	}

}
