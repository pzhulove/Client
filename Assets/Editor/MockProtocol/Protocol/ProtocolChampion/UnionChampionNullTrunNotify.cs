using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// Union->Clinet 同步本轮轮空
	/// </summary>
	[AdvancedInspector.Descriptor("Union->Clinet 同步本轮轮空", "Union->Clinet 同步本轮轮空")]
	public class UnionChampionNullTrunNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1209816;
		public UInt32 Sequence;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
		}

		public void decode(byte[] buffer, ref int pos_)
		{
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
