using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 师傅设置信息返回
	/// </summary>
	[AdvancedInspector.Descriptor("师傅设置信息返回", "师傅设置信息返回")]
	public class QueryMasterSettingRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601721;
		public UInt32 Sequence;

		public string masternote;
		/// <summary>
		/// 师傅公告
		/// </summary>
		[AdvancedInspector.Descriptor("师傅公告", "师傅公告")]
		public byte isRecv;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] masternoteBytes = StringHelper.StringToUTF8Bytes(masternote);
			BaseDLL.encode_string(buffer, ref pos_, masternoteBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, isRecv);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 masternoteLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref masternoteLen);
			byte[] masternoteBytes = new byte[masternoteLen];
			for(int i = 0; i < masternoteLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref masternoteBytes[i]);
			}
			masternote = StringHelper.BytesToString(masternoteBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref isRecv);
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
