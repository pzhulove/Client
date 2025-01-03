using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 检查名字合法性请求
	/// </summary>
	[AdvancedInspector.Descriptor("检查名字合法性请求", "检查名字合法性请求")]
	public class SceneCheckChangeNameReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501216;
		public UInt32 Sequence;

		public string newName;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] newNameBytes = StringHelper.StringToUTF8Bytes(newName);
			BaseDLL.encode_string(buffer, ref pos_, newNameBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
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
