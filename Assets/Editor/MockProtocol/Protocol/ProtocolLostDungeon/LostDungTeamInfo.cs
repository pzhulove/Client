using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 队伍信息
	/// </summary>
	[AdvancedInspector.Descriptor("队伍信息", "队伍信息")]
	public class LostDungTeamInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		/// 队伍id
		/// </summary>
		[AdvancedInspector.Descriptor("队伍id", "队伍id")]
		public UInt32 teamId;
		/// <summary>
		/// 队伍索引[1-4]
		/// </summary>
		[AdvancedInspector.Descriptor("队伍索引[1-4]", "队伍索引[1-4]")]
		public byte index;
		/// <summary>
		/// 战斗状态枚举LostTeamBattleSt
		/// </summary>
		[AdvancedInspector.Descriptor("战斗状态枚举LostTeamBattleSt", "战斗状态枚举LostTeamBattleSt")]
		public byte battleState;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, teamId);
			BaseDLL.encode_int8(buffer, ref pos_, index);
			BaseDLL.encode_int8(buffer, ref pos_, battleState);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
			BaseDLL.decode_int8(buffer, ref pos_, ref index);
			BaseDLL.decode_int8(buffer, ref pos_, ref battleState);
		}


		#endregion

	}

}
