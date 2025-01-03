using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 单组数据
	/// </summary>
	[AdvancedInspector.Descriptor("单组数据", "单组数据")]
	public class ChampionGroup : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 groupID;

		public UInt64 roleA;

		public UInt64 roleB;

		public UInt32 scoreA;

		public UInt32 scoreB;
		/// <summary>
		/// 0 未开始 1已开始 2已结束
		/// </summary>
		[AdvancedInspector.Descriptor("0 未开始 1已开始 2已结束", "0 未开始 1已开始 2已结束")]
		public byte status;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, groupID);
			BaseDLL.encode_uint64(buffer, ref pos_, roleA);
			BaseDLL.encode_uint64(buffer, ref pos_, roleB);
			BaseDLL.encode_uint32(buffer, ref pos_, scoreA);
			BaseDLL.encode_uint32(buffer, ref pos_, scoreB);
			BaseDLL.encode_int8(buffer, ref pos_, status);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref groupID);
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleA);
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleB);
			BaseDLL.decode_uint32(buffer, ref pos_, ref scoreA);
			BaseDLL.decode_uint32(buffer, ref pos_, ref scoreB);
			BaseDLL.decode_int8(buffer, ref pos_, ref status);
		}


		#endregion

	}

}
