using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  ��̭�������Ϣ
	/// </summary>
	[AdvancedInspector.Descriptor(" ��̭�������Ϣ", " ��̭�������Ϣ")]
	public class PremiumLeagueBattleGamer : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  ��ɫID
		/// </summary>
		[AdvancedInspector.Descriptor(" ��ɫID", " ��ɫID")]
		public UInt64 roleId;
		/// <summary>
		///  ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ����", " ����")]
		public string name;
		/// <summary>
		///  ְҵ
		/// </summary>
		[AdvancedInspector.Descriptor(" ְҵ", " ְҵ")]
		public byte occu;
		/// <summary>
		///  ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ����", " ����")]
		public UInt32 ranking;
		/// <summary>
		///  ʤ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ʤ����", " ʤ����")]
		public UInt32 winNum;
		/// <summary>
		///  �Ƿ��Ѿ�����
		/// </summary>
		[AdvancedInspector.Descriptor(" �Ƿ��Ѿ�����", " �Ƿ��Ѿ�����")]
		public byte isLose;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, occu);
			BaseDLL.encode_uint32(buffer, ref pos_, ranking);
			BaseDLL.encode_uint32(buffer, ref pos_, winNum);
			BaseDLL.encode_int8(buffer, ref pos_, isLose);
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
			BaseDLL.decode_uint32(buffer, ref pos_, ref ranking);
			BaseDLL.decode_uint32(buffer, ref pos_, ref winNum);
			BaseDLL.decode_int8(buffer, ref pos_, ref isLose);
		}


		#endregion

	}

}
