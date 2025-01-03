using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  ���������Ϣ
	/// </summary>
	[AdvancedInspector.Descriptor(" ���������Ϣ", " ���������Ϣ")]
	public class RedPacketBaseEntry : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  ���ID
		/// </summary>
		[AdvancedInspector.Descriptor(" ���ID", " ���ID")]
		public UInt64 id;
		/// <summary>
		///  ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ����", " ����")]
		public string name;
		/// <summary>
		///  ������ID
		/// </summary>
		[AdvancedInspector.Descriptor(" ������ID", " ������ID")]
		public UInt64 ownerId;
		/// <summary>
		///  ����������
		/// </summary>
		[AdvancedInspector.Descriptor(" ����������", " ����������")]
		public string ownerName;
		/// <summary>
		///  ״̬����Ӧö��RedPacketStatus��
		/// </summary>
		[AdvancedInspector.Descriptor(" ״̬����Ӧö��RedPacketStatus��", " ״̬����Ӧö��RedPacketStatus��")]
		public byte status;
		/// <summary>
		///  ��û�д򿪹�
		/// </summary>
		[AdvancedInspector.Descriptor(" ��û�д򿪹�", " ��û�д򿪹�")]
		public byte opened;
		/// <summary>
		///  �������(��Ӧö��RedPacketType)
		/// </summary>
		[AdvancedInspector.Descriptor(" �������(��Ӧö��RedPacketType)", " �������(��Ӧö��RedPacketType)")]
		public byte type;
		/// <summary>
		///  �����Դ
		/// </summary>
		[AdvancedInspector.Descriptor(" �����Դ", " �����Դ")]
		public UInt16 reason;
		/// <summary>
		///  ������
		/// </summary>
		[AdvancedInspector.Descriptor(" ������", " ������")]
		public UInt32 totalMoney;
		/// <summary>
		///  �������
		/// </summary>
		[AdvancedInspector.Descriptor(" �������", " �������")]
		public byte totalNum;
		/// <summary>
		///  ���ʣ������
		/// </summary>
		[AdvancedInspector.Descriptor(" ���ʣ������", " ���ʣ������")]
		public byte remainNum;
		/// <summary>
		///  ����ϵ��ս����ʱ���
		/// </summary>
		[AdvancedInspector.Descriptor(" ����ϵ��ս����ʱ���", " ����ϵ��ս����ʱ���")]
		public UInt32 guildTimeStamp;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint64(buffer, ref pos_, ownerId);
			byte[] ownerNameBytes = StringHelper.StringToUTF8Bytes(ownerName);
			BaseDLL.encode_string(buffer, ref pos_, ownerNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, status);
			BaseDLL.encode_int8(buffer, ref pos_, opened);
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint16(buffer, ref pos_, reason);
			BaseDLL.encode_uint32(buffer, ref pos_, totalMoney);
			BaseDLL.encode_int8(buffer, ref pos_, totalNum);
			BaseDLL.encode_int8(buffer, ref pos_, remainNum);
			BaseDLL.encode_uint32(buffer, ref pos_, guildTimeStamp);
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
			BaseDLL.decode_uint64(buffer, ref pos_, ref ownerId);
			UInt16 ownerNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref ownerNameLen);
			byte[] ownerNameBytes = new byte[ownerNameLen];
			for(int i = 0; i < ownerNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref ownerNameBytes[i]);
			}
			ownerName = StringHelper.BytesToString(ownerNameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref status);
			BaseDLL.decode_int8(buffer, ref pos_, ref opened);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint16(buffer, ref pos_, ref reason);
			BaseDLL.decode_uint32(buffer, ref pos_, ref totalMoney);
			BaseDLL.decode_int8(buffer, ref pos_, ref totalNum);
			BaseDLL.decode_int8(buffer, ref pos_, ref remainNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref guildTimeStamp);
		}


		#endregion

	}

}
