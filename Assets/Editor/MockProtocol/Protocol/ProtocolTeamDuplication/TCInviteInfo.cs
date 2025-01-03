using System;
using System.Text;

namespace Mock.Protocol
{

	public class TCInviteInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  队伍ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍ID", " 队伍ID")]
		public UInt32 teamId;
		/// <summary>
		///  队伍模式
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍模式", " 队伍模式")]
		public UInt32 teamModel;
		/// <summary>
		///  队伍难度
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍难度", " 队伍难度")]
		public UInt32 teamGrade;
		/// <summary>
		///  名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 名字", " 名字")]
		public string name;
		/// <summary>
		///  职业
		/// </summary>
		[AdvancedInspector.Descriptor(" 职业", " 职业")]
		public UInt32 occu;
		/// <summary>
		///  觉醒
		/// </summary>
		[AdvancedInspector.Descriptor(" 觉醒", " 觉醒")]
		public UInt32 awaken;
		/// <summary>
		///  等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 等级", " 等级")]
		public UInt32 level;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, teamId);
			BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
			BaseDLL.encode_uint32(buffer, ref pos_, teamGrade);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, occu);
			BaseDLL.encode_uint32(buffer, ref pos_, awaken);
			BaseDLL.encode_uint32(buffer, ref pos_, level);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamGrade);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref occu);
			BaseDLL.decode_uint32(buffer, ref pos_, ref awaken);
			BaseDLL.decode_uint32(buffer, ref pos_, ref level);
		}


		#endregion

	}

}
