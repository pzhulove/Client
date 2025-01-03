using System;
using System.Text;

namespace Mock.Protocol
{

	public class StakeRecord : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  场次id
		/// </summary>
		[AdvancedInspector.Descriptor(" 场次id", " 场次id")]
		public UInt32 courtId;
		/// <summary>
		///  押注射手
		/// </summary>
		[AdvancedInspector.Descriptor(" 押注射手", " 押注射手")]
		public UInt32 stakeShooter;
		/// <summary>
		///  赔率
		/// </summary>
		[AdvancedInspector.Descriptor(" 赔率", " 赔率")]
		public UInt32 odds;
		/// <summary>
		///  支援数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 支援数量", " 支援数量")]
		public UInt32 stakeNum;
		/// <summary>
		///  盈利
		/// </summary>
		[AdvancedInspector.Descriptor(" 盈利", " 盈利")]
		public Int32 profit;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, courtId);
			BaseDLL.encode_uint32(buffer, ref pos_, stakeShooter);
			BaseDLL.encode_uint32(buffer, ref pos_, odds);
			BaseDLL.encode_uint32(buffer, ref pos_, stakeNum);
			BaseDLL.encode_int32(buffer, ref pos_, profit);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref courtId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref stakeShooter);
			BaseDLL.decode_uint32(buffer, ref pos_, ref odds);
			BaseDLL.decode_uint32(buffer, ref pos_, ref stakeNum);
			BaseDLL.decode_int32(buffer, ref pos_, ref profit);
		}


		#endregion

	}

}
