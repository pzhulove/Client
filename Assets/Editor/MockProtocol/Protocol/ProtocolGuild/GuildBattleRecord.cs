using System;
using System.Text;

namespace Mock.Protocol
{

	public class GuildBattleRecord : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 index;
		/// <summary>
		///  胜利者
		/// </summary>
		[AdvancedInspector.Descriptor(" 胜利者", " 胜利者")]
		public GuildBattleMember winner = null;
		/// <summary>
		///  失败者
		/// </summary>
		[AdvancedInspector.Descriptor(" 失败者", " 失败者")]
		public GuildBattleMember loser = null;
		/// <summary>
		///  时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 时间", " 时间")]
		public UInt32 time;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, index);
			winner.encode(buffer, ref pos_);
			loser.encode(buffer, ref pos_);
			BaseDLL.encode_uint32(buffer, ref pos_, time);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref index);
			winner.decode(buffer, ref pos_);
			loser.decode(buffer, ref pos_);
			BaseDLL.decode_uint32(buffer, ref pos_, ref time);
		}


		#endregion

	}

}
