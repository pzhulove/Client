using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 查询代付留言返回
	/// </summary>
	[AdvancedInspector.Descriptor("查询代付留言返回", "查询代付留言返回")]
	public class QueryAddonPayMsgRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501717;
		public UInt32 Sequence;

		public string words;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] wordsBytes = StringHelper.StringToUTF8Bytes(words);
			BaseDLL.encode_string(buffer, ref pos_, wordsBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 wordsLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref wordsLen);
			byte[] wordsBytes = new byte[wordsLen];
			for(int i = 0; i < wordsLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref wordsBytes[i]);
			}
			words = StringHelper.BytesToString(wordsBytes);
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
