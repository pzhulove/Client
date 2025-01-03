using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 设置玩家师傅公告返回
	/// </summary>
	[AdvancedInspector.Descriptor("设置玩家师傅公告返回", "设置玩家师傅公告返回")]
	public class SetMasterNoteRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601717;
		public UInt32 Sequence;

		public UInt32 code;

		public string note;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			byte[] noteBytes = StringHelper.StringToUTF8Bytes(note);
			BaseDLL.encode_string(buffer, ref pos_, noteBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			UInt16 noteLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref noteLen);
			byte[] noteBytes = new byte[noteLen];
			for(int i = 0; i < noteLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref noteBytes[i]);
			}
			note = StringHelper.BytesToString(noteBytes);
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
