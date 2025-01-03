using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  设置队伍属性
	/// </summary>
	[AdvancedInspector.Descriptor(" 设置队伍属性", " 设置队伍属性")]
	public class WorldSetTeamOption : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601625;
		public UInt32 Sequence;
		/// <summary>
		///  操作类型（TeamOptionOperType）
		/// </summary>
		[AdvancedInspector.Descriptor(" 操作类型（TeamOptionOperType）", " 操作类型（TeamOptionOperType）")]
		public byte type;
		/// <summary>
		///  下面不同情况下代表不同的意义
		/// </summary>
		[AdvancedInspector.Descriptor(" 下面不同情况下代表不同的意义", " 下面不同情况下代表不同的意义")]
		public string str;

		public UInt32 param1;

		public UInt32 param2;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			byte[] strBytes = StringHelper.StringToUTF8Bytes(str);
			BaseDLL.encode_string(buffer, ref pos_, strBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, param1);
			BaseDLL.encode_uint32(buffer, ref pos_, param2);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			UInt16 strLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref strLen);
			byte[] strBytes = new byte[strLen];
			for(int i = 0; i < strLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref strBytes[i]);
			}
			str = StringHelper.BytesToString(strBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref param1);
			BaseDLL.decode_uint32(buffer, ref pos_, ref param2);
		}

		public UInt32 GetSequence()
		{
			return Sequence;
		}

		public void SetSequence(UInt32 sequence)
		{
			Sequence = sequence;
		}

		#endregion

	}

}
