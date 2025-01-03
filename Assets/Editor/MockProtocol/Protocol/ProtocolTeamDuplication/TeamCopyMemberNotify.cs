using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  队员通知
	/// </summary>
	[AdvancedInspector.Descriptor(" 队员通知", " 队员通知")]
	public class TeamCopyMemberNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100049;
		public UInt32 Sequence;
		/// <summary>
		///  队员名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 队员名字", " 队员名字")]
		public string memberName;
		/// <summary>
		///  标志(0：加入，1：离开)
		/// </summary>
		[AdvancedInspector.Descriptor(" 标志(0：加入，1：离开)", " 标志(0：加入，1：离开)")]
		public UInt32 flag;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] memberNameBytes = StringHelper.StringToUTF8Bytes(memberName);
			BaseDLL.encode_string(buffer, ref pos_, memberNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, flag);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 memberNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref memberNameLen);
			byte[] memberNameBytes = new byte[memberNameLen];
			for(int i = 0; i < memberNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref memberNameBytes[i]);
			}
			memberName = StringHelper.BytesToString(memberNameBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref flag);
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
