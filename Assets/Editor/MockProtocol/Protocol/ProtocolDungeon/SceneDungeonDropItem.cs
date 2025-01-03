using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneDungeonDropItem : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 id;

		public UInt32 typeId;

		public UInt32 num;

		public byte isDouble;

		public byte strenth;
		/// <summary>
		///  装备类型，对应枚举EquipType
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备类型，对应枚举EquipType", " 装备类型，对应枚举EquipType")]
		public byte equipType;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, typeId);
			BaseDLL.encode_uint32(buffer, ref pos_, num);
			BaseDLL.encode_int8(buffer, ref pos_, isDouble);
			BaseDLL.encode_int8(buffer, ref pos_, strenth);
			BaseDLL.encode_int8(buffer, ref pos_, equipType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref typeId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			BaseDLL.decode_int8(buffer, ref pos_, ref isDouble);
			BaseDLL.decode_int8(buffer, ref pos_, ref strenth);
			BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
		}


		#endregion

	}

}
