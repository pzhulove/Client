using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  �����ԷѺ��
	/// </summary>
	[AdvancedInspector.Descriptor(" �����ԷѺ��", " �����ԷѺ��")]
	public class WorldSendCustomRedPacketReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607310;
		public UInt32 Sequence;
		/// <summary>
		///  reason
		/// </summary>
		[AdvancedInspector.Descriptor(" reason", " reason")]
		public UInt16 reason;
		/// <summary>
		///  �������
		/// </summary>
		[AdvancedInspector.Descriptor(" �������", " �������")]
		public string name;
		/// <summary>
		///  ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ����", " ����")]
		public byte num;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, reason);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, num);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint16(buffer, ref pos_, ref reason);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref num);
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
