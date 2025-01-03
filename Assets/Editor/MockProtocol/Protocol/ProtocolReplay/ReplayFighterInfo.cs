using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  录像玩家信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 录像玩家信息", " 录像玩家信息")]
	public class ReplayFighterInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 roleId;

		public string name;

		public byte occu;

		public UInt16 level;

		public byte pos;
		/// <summary>
		///  赛季段位
		/// </summary>
		[AdvancedInspector.Descriptor(" 赛季段位", " 赛季段位")]
		public UInt32 seasonLevel;
		/// <summary>
		///  赛季星星
		/// </summary>
		[AdvancedInspector.Descriptor(" 赛季星星", " 赛季星星")]
		public byte seasonStars;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, occu);
			BaseDLL.encode_uint16(buffer, ref pos_, level);
			BaseDLL.encode_int8(buffer, ref pos_, pos);
			BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
			BaseDLL.encode_int8(buffer, ref pos_, seasonStars);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			BaseDLL.decode_uint16(buffer, ref pos_, ref level);
			BaseDLL.decode_int8(buffer, ref pos_, ref pos);
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
			BaseDLL.decode_int8(buffer, ref pos_, ref seasonStars);
		}


		#endregion

	}

}
