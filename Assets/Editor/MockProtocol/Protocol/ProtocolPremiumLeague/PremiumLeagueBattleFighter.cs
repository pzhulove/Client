using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  �ͽ�������̭����Ա��Ϣ
	/// </summary>
	[AdvancedInspector.Descriptor(" �ͽ�������̭����Ա��Ϣ", " �ͽ�������̭����Ա��Ϣ")]
	public class PremiumLeagueBattleFighter : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  ��ɫID
		/// </summary>
		[AdvancedInspector.Descriptor(" ��ɫID", " ��ɫID")]
		public UInt64 id;
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

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, occu);
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
			BaseDLL.decode_int8(buffer, ref pos_, ref occu);
		}


		#endregion

	}

}
