using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 查询有没有在别的服绑定
	/// </summary>
	[AdvancedInspector.Descriptor("查询有没有在别的服绑定", "查询有没有在别的服绑定")]
	public class WorldQueryHireAlreadyBindReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601797;
		public UInt32 Sequence;

		public string platform;

		public UInt32 accid;

		public UInt32 zone;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] platformBytes = StringHelper.StringToUTF8Bytes(platform);
			BaseDLL.encode_string(buffer, ref pos_, platformBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, accid);
			BaseDLL.encode_uint32(buffer, ref pos_, zone);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 platformLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref platformLen);
			byte[] platformBytes = new byte[platformLen];
			for(int i = 0; i < platformLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref platformBytes[i]);
			}
			platform = StringHelper.BytesToString(platformBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref zone);
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
