using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  ս����¼��Ա
	/// </summary>
	[AdvancedInspector.Descriptor(" ս����¼��Ա", " ս����¼��Ա")]
	public class PremiumLeagueRecordFighter : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  id
		/// </summary>
		[AdvancedInspector.Descriptor(" id", " id")]
		public UInt64 id;
		/// <summary>
		///  ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ����", " ����")]
		public string name;
		/// <summary>
		///  ��:ս��ǰ����ʤ�� Ӯ:ս������ʤ
		/// </summary>
		[AdvancedInspector.Descriptor(" ��:ս��ǰ����ʤ�� Ӯ:ս������ʤ", " ��:ս��ǰ����ʤ�� Ӯ:ս������ʤ")]
		public byte winStreak;
		/// <summary>
		///  ��û���
		/// </summary>
		[AdvancedInspector.Descriptor(" ��û���", " ��û���")]
		public UInt16 gotScore;
		/// <summary>
		///  �ܻ���
		/// </summary>
		[AdvancedInspector.Descriptor(" �ܻ���", " �ܻ���")]
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
