using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  申请被拒绝通知
	/// </summary>
	[AdvancedInspector.Descriptor(" 申请被拒绝通知", " 申请被拒绝通知")]
	public class TeamCopyApplyRefuseNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100065;
		public UInt32 Sequence;

		public string chiefName;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] chiefNameBytes = StringHelper.StringToUTF8Bytes(chiefName);
			BaseDLL.encode_string(buffer, ref pos_, chiefNameBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 chiefNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref chiefNameLen);
			byte[] chiefNameBytes = new byte[chiefNameLen];
			for(int i = 0; i < chiefNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref chiefNameBytes[i]);
			}
			chiefName = StringHelper.BytesToString(chiefNameBytes);
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
