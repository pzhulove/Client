using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 改名
	/// </summary>
	[AdvancedInspector.Descriptor("改名", "改名")]
	public class SceneChangeNameReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501206;
		public UInt32 Sequence;

		public UInt64 itemUid;

		public string newName;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
			byte[] newNameBytes = StringHelper.StringToUTF8Bytes(newName);
			BaseDLL.encode_string(buffer, ref pos_, newNameBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
			UInt16 newNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref newNameLen);
			byte[] newNameBytes = new byte[newNameLen];
			for(int i = 0; i < newNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref newNameBytes[i]);
			}
			newName = StringHelper.BytesToString(newNameBytes);
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
