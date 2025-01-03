using System;
using System.Text;

namespace Mock.Protocol
{

	public class DungeonChest : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 itemId;

		public UInt32 num;

		public byte type;

		public UInt32 goldReward;

		public byte isRareControl;

		public byte strenth;
		/// <summary>
		///  装备类型，对应枚举EquipType
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备类型，对应枚举EquipType", " 装备类型，对应枚举EquipType")]
		public byte equipType;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			BaseDLL.encode_uint32(buffer, ref pos_, num);
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint32(buffer, ref pos_, goldReward);
			BaseDLL.encode_int8(buffer, ref pos_, isRareControl);
			BaseDLL.encode_int8(buffer, ref pos_, strenth);
			BaseDLL.encode_int8(buffer, ref pos_, equipType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref goldReward);
			BaseDLL.decode_int8(buffer, ref pos_, ref isRareControl);
			BaseDLL.decode_int8(buffer, ref pos_, ref strenth);
			BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
		}


		#endregion

	}

}
