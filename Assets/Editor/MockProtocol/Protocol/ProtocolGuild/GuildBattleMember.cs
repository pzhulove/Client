using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会战成员
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会战成员", " 公会战成员")]
	public class GuildBattleMember : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  ID
		/// </summary>
		[AdvancedInspector.Descriptor(" ID", " ID")]
		public UInt64 id;
		/// <summary>
		/// 名字
		/// </summary>
		[AdvancedInspector.Descriptor("名字", "名字")]
		public string name;
		/// <summary>
		///  连胜数
		/// </summary>
		[AdvancedInspector.Descriptor(" 连胜数", " 连胜数")]
		public byte winStreak;
		/// <summary>
		///  获得积分
		/// </summary>
		[AdvancedInspector.Descriptor(" 获得积分", " 获得积分")]
		public UInt16 gotScore;
		/// <summary>
		///  总积分
		/// </summary>
		[AdvancedInspector.Descriptor(" 总积分", " 总积分")]
		public UInt16 totalScore;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, winStreak);
			BaseDLL.encode_uint16(buffer, ref pos_, gotScore);
			BaseDLL.encode_uint16(buffer, ref pos_, totalScore);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref winStreak);
			BaseDLL.decode_uint16(buffer, ref pos_, ref gotScore);
			BaseDLL.decode_uint16(buffer, ref pos_, ref totalScore);
		}


		#endregion

	}

}
