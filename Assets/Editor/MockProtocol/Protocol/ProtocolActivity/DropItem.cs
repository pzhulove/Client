using System;
using System.Text;

namespace Mock.Protocol
{

	public class DropItem : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 itemId;

		public UInt32 num;

		public byte strenth;
		/// <summary>
		///  装备类型(对应枚举EquipType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备类型(对应枚举EquipType)", " 装备类型(对应枚举EquipType)")]
		public byte equipType;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			BaseDLL.encode_uint32(buffer, ref pos_, num);
			BaseDLL.encode_int8(buffer, ref pos_, strenth);
			BaseDLL.encode_int8(buffer, ref pos_, equipType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			BaseDLL.decode_int8(buffer, ref pos_, ref strenth);
			BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
		}


		#endregion

	}

}
