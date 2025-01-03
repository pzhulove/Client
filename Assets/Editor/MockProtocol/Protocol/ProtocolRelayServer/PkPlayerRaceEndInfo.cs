using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  玩家PK结算
	/// </summary>
	[AdvancedInspector.Descriptor(" 玩家PK结算", " 玩家PK结算")]
	public class PkPlayerRaceEndInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 roleId;

		public byte pos;

		public byte result;

		public UInt32 remainHp;

		public UInt32 remainMp;
		/// <summary>
		///  伤害百分比，乘10000倍
		/// </summary>
		[AdvancedInspector.Descriptor(" 伤害百分比，乘10000倍", " 伤害百分比，乘10000倍")]
		public UInt32 damagePercent;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			BaseDLL.encode_int8(buffer, ref pos_, pos);
			BaseDLL.encode_int8(buffer, ref pos_, result);
			BaseDLL.encode_uint32(buffer, ref pos_, remainHp);
			BaseDLL.encode_uint32(buffer, ref pos_, remainMp);
			BaseDLL.encode_uint32(buffer, ref pos_, damagePercent);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			BaseDLL.decode_int8(buffer, ref pos_, ref pos);
			BaseDLL.decode_int8(buffer, ref pos_, ref result);
			BaseDLL.decode_uint32(buffer, ref pos_, ref remainHp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref remainMp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref damagePercent);
		}


		#endregion

	}

}
