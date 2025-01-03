using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 
	/// </summary>
	[AdvancedInspector.Descriptor("", "")]
	public class WorldQueryHireTaskAccidListRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601789;
		public UInt32 Sequence;
		/// <summary>
		/// 结果
		/// </summary>
		[AdvancedInspector.Descriptor("结果", "结果")]
		public string[] nameList = new string[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)nameList.Length);
			for(int i = 0; i < nameList.Length; i++)
			{
				byte[] nameListBytes = StringHelper.StringToUTF8Bytes(nameList[i]);
				BaseDLL.encode_string(buffer, ref pos_, nameListBytes, (UInt16)(buffer.Length - pos_));
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 nameListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameListCnt);
			nameList = new string[nameListCnt];
			for(int i = 0; i < nameList.Length; i++)
			{
				UInt16 nameListLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameListLen);
				byte[] nameListBytes = new byte[nameListLen];
				for(int j = 0; j < nameListLen; j++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameListBytes[j]);
				}
				nameList[i] = StringHelper.BytesToString(nameListBytes);
			}
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
