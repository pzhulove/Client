using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  �����ȡ��¼
	/// </summary>
	[AdvancedInspector.Descriptor(" �����ȡ��¼", " �����ȡ��¼")]
	public class RedPacketRecord : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  Ψһid
		/// </summary>
		[AdvancedInspector.Descriptor(" Ψһid", " Ψһid")]
		public UInt64 guid;
		/// <summary>
		///  ʱ��
		/// </summary>
		[AdvancedInspector.Descriptor(" ʱ��", " ʱ��")]
		public UInt32 time;
		/// <summary>
		///  �������������
		/// </summary>
		[AdvancedInspector.Descriptor(" �������������", " �������������")]
		public string redPackOwnerName;
		/// <summary>
		///  ����id
		/// </summary>
		[AdvancedInspector.Descriptor(" ����id", " ����id")]
		public UInt32 moneyId;
		/// <summary>
		///  ��������
		/// </summary>
		[AdvancedInspector.Descriptor(" ��������", " ��������")]
		public UInt32 moneyNum;
		/// <summary>
		///  �Ƿ���Ѻ��
		/// </summary>
		[AdvancedInspector.Descriptor(" �Ƿ���Ѻ��", " �Ƿ���Ѻ��")]
		public byte isBest;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, guid);
			BaseDLL.encode_uint32(buffer, ref pos_, time);
			byte[] redPackOwnerNameBytes = StringHelper.StringToUTF8Bytes(redPackOwnerName);
			BaseDLL.encode_string(buffer, ref pos_, redPackOwnerNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, moneyId);
			BaseDLL.encode_uint32(buffer, ref pos_, moneyNum);
			BaseDLL.encode_int8(buffer, ref pos_, isBest);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref time);
			UInt16 redPackOwnerNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref redPackOwnerNameLen);
			byte[] redPackOwnerNameBytes = new byte[redPackOwnerNameLen];
			for(int i = 0; i < redPackOwnerNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref redPackOwnerNameBytes[i]);
			}
			redPackOwnerName = StringHelper.BytesToString(redPackOwnerNameBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref moneyId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref moneyNum);
			BaseDLL.decode_int8(buffer, ref pos_, ref isBest);
		}


		#endregion

	}

}
