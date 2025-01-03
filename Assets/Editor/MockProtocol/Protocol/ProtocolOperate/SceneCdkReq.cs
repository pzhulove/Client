using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneCdkReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607401;
		public UInt32 Sequence;

		public string cdk;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] cdkBytes = StringHelper.StringToUTF8Bytes(cdk);
			BaseDLL.encode_string(buffer, ref pos_, cdkBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 cdkLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref cdkLen);
			byte[] cdkBytes = new byte[cdkLen];
			for(int i = 0; i < cdkLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref cdkBytes[i]);
			}
			cdk = StringHelper.BytesToString(cdkBytes);
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
