using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 请求活动详情
	/// </summary>
	[AdvancedInspector.Descriptor("请求活动详情", "请求活动详情")]
	public class WorldActivityDetailReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 602904;
		public UInt32 Sequence;

		public string activeName;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] activeNameBytes = StringHelper.StringToUTF8Bytes(activeName);
			BaseDLL.encode_string(buffer, ref pos_, activeNameBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 activeNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref activeNameLen);
			byte[] activeNameBytes = new byte[activeNameLen];
			for(int i = 0; i < activeNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref activeNameBytes[i]);
			}
			activeName = StringHelper.BytesToString(activeNameBytes);
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
