using System;
using System.Text;

namespace Mock.Protocol
{

	public class shooterRankInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  选手id
		/// </summary>
		[AdvancedInspector.Descriptor(" 选手id", " 选手id")]
		public UInt32 shooterId;
		/// <summary>
		///  参赛次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 参赛次数", " 参赛次数")]
		public UInt32 battleNum;
		/// <summary>
		///  胜率
		/// </summary>
		[AdvancedInspector.Descriptor(" 胜率", " 胜率")]
		public UInt32 winRate;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, shooterId);
			BaseDLL.encode_uint32(buffer, ref pos_, battleNum);
			BaseDLL.encode_uint32(buffer, ref pos_, winRate);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref shooterId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref winRate);
		}


		#endregion

	}

}
