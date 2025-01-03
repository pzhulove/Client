using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  boss血量信息
	/// </summary>
	[AdvancedInspector.Descriptor(" boss血量信息", " boss血量信息")]
	public class GuildDungeonBossBlood : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  地下城id
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城id", " 地下城id")]
		public UInt32 dungeonId;
		/// <summary>
		///  剩余血量
		/// </summary>
		[AdvancedInspector.Descriptor(" 剩余血量", " 剩余血量")]
		public UInt64 oddBlood;
		/// <summary>
		///  待验证血量
		/// </summary>
		[AdvancedInspector.Descriptor(" 待验证血量", " 待验证血量")]
		public UInt64 verifyBlood;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			BaseDLL.encode_uint64(buffer, ref pos_, oddBlood);
			BaseDLL.encode_uint64(buffer, ref pos_, verifyBlood);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref oddBlood);
			BaseDLL.decode_uint64(buffer, ref pos_, ref verifyBlood);
		}


		#endregion

	}

}
