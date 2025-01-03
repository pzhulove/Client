using System;
using System.Text;

namespace Mock.Protocol
{

	public class FightergResult : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public byte flag;

		public byte seat;

		public UInt32 accid;

		public UInt64 roldid;
		/// <summary>
		/// 剩余血量(百分比)
		/// </summary>
		[AdvancedInspector.Descriptor("剩余血量(百分比)", "剩余血量(百分比)")]
		public UInt32 remainHp;
		/// <summary>
		/// 剩余魔量(百分比)
		/// </summary>
		[AdvancedInspector.Descriptor("剩余魔量(百分比)", "剩余魔量(百分比)")]
		public UInt32 remainMp;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, flag);
			BaseDLL.encode_int8(buffer, ref pos_, seat);
			BaseDLL.encode_uint32(buffer, ref pos_, accid);
			BaseDLL.encode_uint64(buffer, ref pos_, roldid);
			BaseDLL.encode_uint32(buffer, ref pos_, remainHp);
			BaseDLL.encode_uint32(buffer, ref pos_, remainMp);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref flag);
			BaseDLL.decode_int8(buffer, ref pos_, ref seat);
			BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
			BaseDLL.decode_uint64(buffer, ref pos_, ref roldid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref remainHp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref remainMp);
		}


		#endregion

	}

}
