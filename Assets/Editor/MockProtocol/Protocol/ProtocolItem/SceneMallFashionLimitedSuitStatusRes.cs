using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  限定商城套装条件返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 限定商城套装条件返回", " 限定商城套装条件返回")]
	public class SceneMallFashionLimitedSuitStatusRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501086;
		public UInt32 Sequence;

		public UInt32[] result = new UInt32[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)result.Length);
			for(int i = 0; i < result.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 resultCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref resultCnt);
			result = new UInt32[resultCnt];
			for(int i = 0; i < result.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result[i]);
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
