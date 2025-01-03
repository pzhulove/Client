using System;
using System.Text;

namespace Mock.Protocol
{

	public class RelaySvrGameResultNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1300006;
		public UInt32 Sequence;

		public byte reason;

		public UInt64 session;

		public FightergResult[] results = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, reason);
			BaseDLL.encode_uint64(buffer, ref pos_, session);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)results.Length);
			for(int i = 0; i < results.Length; i++)
			{
				results[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref reason);
			BaseDLL.decode_uint64(buffer, ref pos_, ref session);
			UInt16 resultsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref resultsCnt);
			results = new FightergResult[resultsCnt];
			for(int i = 0; i < results.Length; i++)
			{
				results[i] = new FightergResult();
				results[i].decode(buffer, ref pos_);
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
