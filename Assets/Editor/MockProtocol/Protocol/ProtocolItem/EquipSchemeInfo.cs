using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 装备方案信息
	/// </summary>
	[AdvancedInspector.Descriptor("装备方案信息", "装备方案信息")]
	public class EquipSchemeInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		/// 唯一id
		/// </summary>
		[AdvancedInspector.Descriptor("唯一id", "唯一id")]
		public UInt64 guid;
		/// <summary>
		/// 类型
		/// </summary>
		[AdvancedInspector.Descriptor("类型", "类型")]
		public byte type;
		/// <summary>
		/// 方案id
		/// </summary>
		[AdvancedInspector.Descriptor("方案id", "方案id")]
		public UInt32 id;
		/// <summary>
		/// 是否穿戴
		/// </summary>
		[AdvancedInspector.Descriptor("是否穿戴", "是否穿戴")]
		public byte weared;
		/// <summary>
		/// 装备id
		/// </summary>
		[AdvancedInspector.Descriptor("装备id", "装备id")]
		public UInt64[] equips = new UInt64[0];

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, guid);
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_int8(buffer, ref pos_, weared);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equips.Length);
			for(int i = 0; i < equips.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, equips[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_int8(buffer, ref pos_, ref weared);
			UInt16 equipsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref equipsCnt);
			equips = new UInt64[equipsCnt];
			for(int i = 0; i < equips.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref equips[i]);
			}
		}


		#endregion

	}

}
