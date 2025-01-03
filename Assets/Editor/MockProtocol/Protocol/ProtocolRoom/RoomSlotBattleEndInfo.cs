using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 结算位置信息
	/// </summary>
	[AdvancedInspector.Descriptor("结算位置信息", "结算位置信息")]
	public class RoomSlotBattleEndInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public byte resultFlag;

		public UInt32 roomId;

		public byte roomType;

		public UInt64 roleId;

		public byte seat;

		public UInt32 seasonLevel;

		public UInt32 seasonStar;

		public UInt32 seasonExp;

		public UInt32 scoreWarBaseScore;

		public UInt32 scoreWarContriScore;

		public UInt32 getHonor;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, resultFlag);
			BaseDLL.encode_uint32(buffer, ref pos_, roomId);
			BaseDLL.encode_int8(buffer, ref pos_, roomType);
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			BaseDLL.encode_int8(buffer, ref pos_, seat);
			BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
			BaseDLL.encode_uint32(buffer, ref pos_, seasonStar);
			BaseDLL.encode_uint32(buffer, ref pos_, seasonExp);
			BaseDLL.encode_uint32(buffer, ref pos_, scoreWarBaseScore);
			BaseDLL.encode_uint32(buffer, ref pos_, scoreWarContriScore);
			BaseDLL.encode_uint32(buffer, ref pos_, getHonor);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref resultFlag);
			BaseDLL.decode_uint32(buffer, ref pos_, ref roomId);
			BaseDLL.decode_int8(buffer, ref pos_, ref roomType);
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			BaseDLL.decode_int8(buffer, ref pos_, ref seat);
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonStar);
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonExp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref scoreWarBaseScore);
			BaseDLL.decode_uint32(buffer, ref pos_, ref scoreWarContriScore);
			BaseDLL.decode_uint32(buffer, ref pos_, ref getHonor);
		}


		#endregion

	}

}
